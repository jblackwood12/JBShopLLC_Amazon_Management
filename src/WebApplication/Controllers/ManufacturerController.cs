using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using AmazonProductLookup.AdvApi;
using AmazonProductLookup.AmazonApis.AdvApi;
using AmazonProductLookup.AmazonApis.MwsApi.Products;
using Data;
using Models;
using Models.AdvApi;
using Utility;
using Utility.FeeStructure;
using WebApplication.Models;
using Product = Models.Product;

namespace WebApplication.Controllers
{
	public class ManufacturerController : Controller
	{
		public ManufacturerController(IAmazonMWSdbService amazonMwSdbService, IProductAdvertisingApi productAdvertisingApi, IMwsProductsApi amazonApiClient)
		{
			m_amazonMwSdbService = amazonMwSdbService;
			m_productAdvertisingApi = productAdvertisingApi;
			m_amazonApiClient = amazonApiClient;
		}

		public ActionResult Add()
		{
			return View();
		}

		public ActionResult PriceListLookup()
		{
			return View();
		}

		public ActionResult Create(CreateManufacturer createManufacturer)
		{
			m_amazonMwSdbService.InsertManufacturer(createManufacturer.ManufacturerName);

			return View("Add");
		}

		[HttpPost]
		public ActionResult Find(string manufacturerString)
		{
			List<Data.Manufacturer> manufacturers = m_amazonMwSdbService.FindManufacturers(manufacturerString);

			return Json(manufacturers.Select(s => new { s.Name, Id = s.ManufacturerId }));
		}

		[HttpPost]
		public void EvaluatePriceList(PriceListFile priceListFile)
		{
			DataTable dt = ExcelUtility.ConvertExcelFileToDataTable(priceListFile.File);

			List<ManufacturerPriceListRowInput> manufacturerPriceListRowsInputs = dt.FromDataTableToList<ManufacturerPriceListRowInput>();

			// Price is always required.
			if (manufacturerPriceListRowsInputs.Any(a => !a.Price.HasValue))
				throw new ArgumentException("Price must be set for all rows.");

			// Need either UPC or ASIN.
			if (manufacturerPriceListRowsInputs.Any(a => a.ASIN == null && a.UPC == null))
				throw new ArgumentException("ASIN or UPC must be set for all rows.");

			List<ManufacturerPriceListRowOutput> manufacturerPriceListRowsOutputs = new List<ManufacturerPriceListRowOutput>();

			// Only lookup rows where either ASIN or UPC is set.
			foreach (ManufacturerPriceListRowInput manufacturerPriceListRow in manufacturerPriceListRowsInputs
				.Where(w => !w.ASIN.IsNullOrEmptyTrimmed() || !w.UPC.IsNullOrEmptyTrimmed()))
			{
				LookupProductRequest lookupProductRequest = new LookupProductRequest
				{
					SearchIndex = priceListFile.SearchIndex,
					ResponseGroup = ResponseGroup.Medium
				};

				if (manufacturerPriceListRow.ASIN != null)
				{
					lookupProductRequest.IdType = IdType.ASIN;
					lookupProductRequest.ItemId = manufacturerPriceListRow.ASIN;
				}
				else
				{
					lookupProductRequest.IdType = IdType.UPC;
					lookupProductRequest.ItemId = manufacturerPriceListRow.UPC;
				}

				LookupProductResponse lookupProductResponse = m_productAdvertisingApi.LookupProduct(lookupProductRequest);

				if (lookupProductResponse != null)
				{
					Product product = lookupProductResponse.Product;

					Listing listing = m_amazonApiClient.GetAllListingsForAsin(new[] { product.ASIN }).FirstOrDefault();

					decimal? breakEven = null;
					decimal? profitMargin = null;
					decimal? lowestPrice = null;
					string sellersRankCategory = null;

					if (listing != null)
					{
						lowestPrice = listing.LowestPrice;
						sellersRankCategory = listing.SellersRankCategory;
					}

					if (product.Length.HasValue && product.Width.HasValue && product.Height.HasValue && product.Weight.HasValue)
					{
						DimensionContainer dimensionContainer = new DimensionContainer(
							product.Length.Value,
							product.Width.Value,
							product.Height.Value,
							product.Weight.Value);

						breakEven = FeeStructureUtility.CalculateBreakEven(
							dimensionContainer,
							priceListFile.FeeCategory,
							manufacturerPriceListRow.Price.Value);

						if (lowestPrice.HasValue && lowestPrice.Value > 0)
						{
							profitMargin = FeeStructureUtility.CalculateProfitMargin(
								dimensionContainer,
								priceListFile.FeeCategory,
								lowestPrice.Value,
								manufacturerPriceListRow.Price.Value);
						}
					}

					ManufacturerPriceListRowOutput manufacturerPriceListRowOutput = new ManufacturerPriceListRowOutput
					{
						ASIN = product.ASIN,
						UPC = manufacturerPriceListRow.UPC,
						ProductName = product.Name,
						SellersRank = lookupProductResponse.ProductMetadata.SalesRank,
						SellersRankCategory = sellersRankCategory,
						Category = lookupProductResponse.ProductMetadata.ProductGroup,
						Price = manufacturerPriceListRow.Price.Value,
						BreakEven = breakEven.HasValue
							? breakEven.Value.ToString("F", CultureInfo.InvariantCulture)
							: null,
						LowestPrice = lowestPrice.HasValue
							? lowestPrice.Value.ToString("F", CultureInfo.InvariantCulture)
							: null,
						ProfitMargin = profitMargin.HasValue
							? string.Format("{0:0.00%}", profitMargin.Value)
							: null
					};

					manufacturerPriceListRowsOutputs.Add(manufacturerPriceListRowOutput);
				}
			}

			ExcelUtility.WriteExcelFileToResponse(Response, manufacturerPriceListRowsOutputs, c_worksheetName, priceListFile.File.FileName);
		}

		private readonly IAmazonMWSdbService m_amazonMwSdbService;

		private readonly IProductAdvertisingApi m_productAdvertisingApi;
		private readonly IMwsProductsApi m_amazonApiClient;

		private const string c_worksheetName = "PriceLookup";
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using AmazonProductLookup.AdvApi;
using AmazonProductLookup.AmazonApis.AdvApi;
using Models;
using Models.AdvApi;
using Utility.FeeStructure;
using WebApplication.Models;
using WebApplication.ViewModels;
using Data;
using Product = Models.Product;

namespace WebApplication.Controllers
{
	public class ProductController : Controller
	{
		public ProductController(IAmazonMWSdbService amazonMwSdbService, IProductAdvertisingApi productAdvertisingApi)
		{
			m_amazonMwSdbService = amazonMwSdbService;
			m_productAdvertisingApi = productAdvertisingApi;
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Report(string asin)
		{
			if (asin == null)
				throw new ArgumentNullException("asin");

			LookupProductRequest lookupProductRequest = new LookupProductRequest
			{
				ItemId = asin,
				IdType = IdType.ASIN,
				ResponseGroup = ResponseGroup.Medium
			};

			Task<LookupProductResponse> productTask = Task.Factory.StartNew(() => m_productAdvertisingApi.LookupProduct(lookupProductRequest));
			Task<Data.PriceHistory> mostRecentPriceHistoryTask = Task.Factory.StartNew(() => m_amazonMwSdbService.GetMostRecentAsinPrice(asin));
			Task<FeePreview> feePreviewTask = Task.Factory.StartNew(() => m_amazonMwSdbService.GetFeePreviewForAsin(asin).FirstOrDefault());
			Task<decimal?> itemPriceTask = Task.Factory.StartNew(() => m_amazonMwSdbService.GetCostOfProductForAsin(asin));

			Task.WaitAll(new Task[] { productTask, mostRecentPriceHistoryTask, feePreviewTask, itemPriceTask, itemPriceTask });

			LookupProductResponse lookupProductResponse = productTask.Result;
			
			ProductPageViewModel viewModel = new ProductPageViewModel
			{
				Product = lookupProductResponse.Product,
				ProductMetadata = lookupProductResponse.ProductMetadata
			};

			viewModel.Product.ASIN = asin;

			viewModel.CurrentPriceBreakdownData = GetPriceBreakdown(asin, lookupProductResponse.Product, mostRecentPriceHistoryTask.Result, feePreviewTask.Result, itemPriceTask.Result);

			return View(viewModel);
		}

		public CurrentPriceBreakdown GetPriceBreakdown(string asin, Product amazonProductDetails, Data.PriceHistory priceHistory, FeePreview feePreviewForAsin, decimal? itemPrice)
		{
			if (itemPrice == null || feePreviewForAsin == null || priceHistory == null || amazonProductDetails == null ||
				amazonProductDetails.Weight == null || amazonProductDetails.Length == null || amazonProductDetails.Width == null ||
				amazonProductDetails.Height == null)
				return new CurrentPriceBreakdown
					{
						MissingInfo = string.Format(
							"Missing Info: <br>{0}{1}{2}{3}",
							itemPrice.HasValue ? "Item Price<br>" : string.Empty,
							feePreviewForAsin == null ? "Amazon Fee Preview<br>" : string.Empty,
							priceHistory == null ? "Price History<br>" : string.Empty,
							amazonProductDetails == null ? "Amazon Product Details" : string.Empty)
					};

			CurrentPriceBreakdown priceBreakdown = new CurrentPriceBreakdown
				{
					NewestPrice = priceHistory.NewPrice,
					LastUpdatedPriceDateTime = priceHistory.TimeStamp,
					AmazonReferralFee = priceHistory.NewPrice * 0.15m,
					PerOrderFeePreview = feePreviewForAsin.EstimatedOrderHandlingFeePerOrder,
					PerUnitFeePreview = feePreviewForAsin.EstimatedPickPackFeePerUnit,
					WeightFeePreview = feePreviewForAsin.EstimatedWeightHandlingFeePerUnit,
					ItemCost = itemPrice.Value,
					EstShippingToAmazon = amazonProductDetails.Weight.Value * 0.70m,
					EstStorageFee = (1.2m / (12m * 12m * 12m)) * amazonProductDetails.Length.Value * amazonProductDetails.Width.Value * amazonProductDetails.Height.Value,
					EstReturns = priceHistory.NewPrice * 0.03m,
				};
			priceBreakdown.EstimatedProfit = priceBreakdown.NewestPrice - priceBreakdown.AmazonReferralFee -
											 priceBreakdown.PerOrderFeePreview - priceBreakdown.PerUnitFeePreview - priceBreakdown.WeightFeePreview - itemPrice.Value - amazonProductDetails.Weight.Value * 0.70m - (1.2m / (12m*12m*12m)) * amazonProductDetails.Length.Value * amazonProductDetails.Width.Value * amazonProductDetails.Height.Value; //// finish
			return priceBreakdown;
		}

		public ActionResult Edit()
		{
			return View();
		}

		[System.Web.Mvc.HttpGet]
		public ActionResult Search([FromUri]string query)
		{
			List<ProductSearchResult> productSearchResults = m_amazonMwSdbService.SearchForProducts(query);

			return Json(productSearchResults.Take(c_defaultNumSearchResultsReturned), JsonRequestBehavior.AllowGet);
		}

		[System.Web.Mvc.HttpGet]
		public ActionResult Evaluate(string asin)
		{
			LookupProductRequest lookupProductRequest = new LookupProductRequest
			{
				ItemId = asin,
				IdType = IdType.ASIN,
				ResponseGroup = ResponseGroup.Medium
			};

			LookupProductResponse lookupProductResponse = m_productAdvertisingApi.LookupProduct(lookupProductRequest);

			ProductProfitabilityMetadata dimensions = null;

			if (lookupProductResponse != null)
			{
				Product product = lookupProductResponse.Product;

				ProductMetadata productMetadata = lookupProductResponse.ProductMetadata;

				dimensions = new ProductProfitabilityMetadata
				{
					ASIN = asin,
					Length = product.Length,
					Width = product.Width,
					Height = product.Height,
					Weight = product.Weight,
					SellersRank = productMetadata.SalesRank
				};
			}

			return View(dimensions);
		}

		[System.Web.Mvc.HttpPost]
		public ActionResult EvaluateCalculate(ProductProfitabilityRequest productProfitabilityRequest)
		{
			if (productProfitabilityRequest.CostPerPoundToShip.HasValue && productProfitabilityRequest.FlatCostToShip.HasValue)
				throw new InvalidOperationException("Cannot have both a CostPerPoundToShip and FlatCostToShip");

			Func<decimal, decimal> funcShippingCost = null;

			if (productProfitabilityRequest.CostPerPoundToShip.HasValue)
				funcShippingCost = weight => weight * productProfitabilityRequest.CostPerPoundToShip.Value;
			else if (productProfitabilityRequest.FlatCostToShip.HasValue)
				funcShippingCost = weight => productProfitabilityRequest.FlatCostToShip.Value;

			DimensionContainer dimensionContainer = new DimensionContainer(
				productProfitabilityRequest.Length,
				productProfitabilityRequest.Width,
				productProfitabilityRequest.Height,
				productProfitabilityRequest.Weight);

			decimal profitMargin = FeeStructureUtility.CalculateProfitMargin(
				dimensionContainer,
				productProfitabilityRequest.FeeCategory,
				productProfitabilityRequest.LowestPrice,
				productProfitabilityRequest.PurchasePrice,
				funcShippingCost);

			decimal breakEven = FeeStructureUtility.CalculateBreakEven(
				dimensionContainer,
				productProfitabilityRequest.FeeCategory,
				productProfitabilityRequest.PurchasePrice,
				funcShippingCost);

			ProductProfitabilityResponse productProfitabilityViewModel = new ProductProfitabilityResponse
			{
				BreakEvenPrice = breakEven,
				ProfitMargin = profitMargin
			};

			return Json(productProfitabilityViewModel);
		}

		private readonly IAmazonMWSdbService m_amazonMwSdbService;

		private readonly IProductAdvertisingApi m_productAdvertisingApi;

		private const int c_defaultNumSearchResultsReturned = 30;
	}
}

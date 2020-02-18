using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AmazonProductLookup.AdvApi;
using AmazonProductLookup.AmazonApis.AdvApi;
using AmazonProductLookup.AmazonApis.MwsApi.InboundShipment;
using FBAInboundServiceMWS.Model;
using Models;
using Models.AdvApi;
using Models.MwsInboundShipmentApi;
using Utility;
using WebApplication.Models;
using OfficeOpenXml;

namespace WebApplication.Controllers
{
	public class ShipmentController : Controller
	{
		public ShipmentController(IMwsInboundShipmentApi mwsInboundShipmentApi, IProductAdvertisingApi productAdvertisingApi)
		{
			m_mwsProductsApi = mwsInboundShipmentApi;

			m_productAdvertisingApi = productAdvertisingApi;
		}

		public ActionResult CreateShipment()
		{
			return View();
		}

		public ActionResult ManageShipment()
		{
			return View();
		}

		[HttpGet]
		public void CreateShipmentTemplate()
		{
			ExcelUtility.WriteExcelFileToResponse(Response, new List<ShipmentItem>(), "Template", "CreateShipmentTemplate");
		}

		[HttpPost]
		public void GetShipmentData(GetShipmentRequest getShipmentRequest)
		{
			InboundShipmentContainer inboundShipmentContainer = m_mwsProductsApi
				.GetInboundShipments(shipmentId: getShipmentRequest.ShipmentId)
				.FirstOrDefault();

			if (inboundShipmentContainer == null)
				throw new InvalidOperationException("ShipmentId not found.");

			List<InboundShipmentItemAndDetails> inboundShipmentItemAndDetailses = new List<InboundShipmentItemAndDetails>();

			foreach (InboundShipmentItem inboundShipmentItem in inboundShipmentContainer.InboundShipmentItems)
			{
				LookupProductRequest lookupProductRequest = new LookupProductRequest
				{
					ItemId = inboundShipmentItem.FulfillmentNetworkSKU,
					IdType = IdType.ASIN,
					ResponseGroup = ResponseGroup.Medium
				};

				LookupProductResponse response = m_productAdvertisingApi.LookupProduct(lookupProductRequest);

				Product product = response != null
					? response.Product
					: null;

				InboundShipmentItemAndDetails inboundShipmentItemAndDetails = new InboundShipmentItemAndDetails(
					inboundShipmentItem.FulfillmentNetworkSKU,
					inboundShipmentItem.QuantityInCase,
					inboundShipmentItem.QuantityReceived,
					inboundShipmentItem.QuantityShipped,
					inboundShipmentItem.SellerSKU,
					product != null ? product.Name : string.Empty,
					product != null ? product.ItemNumber : string.Empty,
					product != null ? product.UPC : string.Empty);

				inboundShipmentItemAndDetailses.Add(inboundShipmentItemAndDetails);
			}

			int lineNumberCounter = 0;
			DataTable shipmentItems = inboundShipmentItemAndDetailses
				.OrderBy(o => o.SKU)
				.Select(s =>
				{
					lineNumberCounter++;
					s.LineNumber = lineNumberCounter;

					return s;
				})
				.ToDataTable();

			using (ExcelPackage excel = new ExcelPackage())
			{
				ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add(getShipmentRequest.ShipmentId);

				worksheet.Cells["A1"].LoadFromDataTable(shipmentItems, true);

				ExcelUtility.CreateExcelResponse(Response, excel, string.Format("Shipment_{0}", getShipmentRequest.ShipmentId));
			}
		}

		[HttpPost]
		public void CreateShipmentResult(CreateShipmentFile createShipmentFile)
		{
			DataTable dt = ExcelUtility.ConvertExcelFileToDataTable(createShipmentFile.File);

			List<ShipmentItem> shipmentItems = dt.FromDataTableToList<ShipmentItem>()
				.Where(w => w.Quantity > 0)
				.ToList();

			if (shipmentItems.Any(a => a.QuantityInCase.HasValue) && !shipmentItems.All(a => a.QuantityInCase.HasValue))
				throw new InvalidOperationException("All, or none items in the shipment must have a QuantityInCase set.");

			bool areCasesRequired = shipmentItems.Any(a => a.QuantityInCase.HasValue);

			CreateShipmentRequest createShipmentRequest = new CreateShipmentRequest(
				shipmentItems,
				createShipmentFile.ShipmentName,
				areCasesRequired);

			CreateShipmentResponse createShipmentResponse = m_mwsProductsApi.CreateInboundShipment(createShipmentRequest);

			using (ExcelPackage excel = new ExcelPackage())
			{
				foreach (CreatedShipment createdShipment in createShipmentResponse.CreatedShipments)
				{
					string worksheetName = createdShipment.ShipmentId;

					DataTable dtShipmentItems;

					if (areCasesRequired)
					{
						dtShipmentItems = createdShipment.ItemsInShipment
							.Select(s => new { s.Name, s.ItemNumber, s.Asin, s.Quantity, s.Cost, s.QuantityInCase, Extension = s.Cost * s.Quantity })
							.ToDataTable();
					}
					else
					{
						dtShipmentItems = createdShipment.ItemsInShipment
							.Select(s => new { s.Name, s.ItemNumber, s.Asin, s.Quantity, s.Cost, Extension = s.Cost * s.Quantity })
							.ToDataTable();
					}

					ExcelWorksheet wsCreatedShipment = excel.Workbook.Worksheets.Add(worksheetName);
					wsCreatedShipment.Cells["A1"].LoadFromDataTable(dtShipmentItems, true);
				}

				ExcelUtility.CreateExcelResponse(Response, excel, createShipmentFile.ShipmentName);
			}
		}

		private readonly IMwsInboundShipmentApi m_mwsProductsApi;

		private readonly IProductAdvertisingApi m_productAdvertisingApi;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AmazonProductLookup.AdvApi;
using AmazonProductLookup.AmazonApis.AdvApi;
using Data;
using Models;
using Models.AdvApi;
using Utility;
using WebApplication.Models;
using Product = Models.Product;

namespace WebApplication.Controllers
{
	public class ReportController : Controller
	{
		public ReportController(IAmazonMWSdbService amazonMwSdbService, IProductAdvertisingApi productAdvertisingApi)
		{
			m_amazonMwSdbService = amazonMwSdbService;
			m_productAdvertisingApi = productAdvertisingApi;
		}

		public ActionResult Sales()
		{
			return View();
		}

		public ActionResult Inventory()
		{
			return View();
		}

		public ActionResult UnpricedProducts()
		{
			return View();
		}

		[HttpPost]
		public void GetInventoryReport(InventoryReportRequest inventoryReportRequest)
		{
			DateTime endDate = DateTime.UtcNow.Date;
			DateTime beginDate = endDate.AddDays(-inventoryReportRequest.DaysOrderAndInventoryHistoryToUse);

			// Find products that are associated with this manufacturer.
			Task<List<Product>> productsTask = Task.Factory.StartNew(() => m_amazonMwSdbService.GetProductsForManufacturer(inventoryReportRequest.ManufacturerId));
			Task<List<OrderSummary>> ordersTask = Task.Factory.StartNew(() => m_amazonMwSdbService.GetOrderSummaryForManufacturer(inventoryReportRequest.ManufacturerId, beginDate, endDate));
			Task<List<InventorySummary>> inventoryHistoriesTask = Task.Factory.StartNew(() => m_amazonMwSdbService.GetInventorySummaryForManufacturer(inventoryReportRequest.ManufacturerId, beginDate, endDate));

			Task.WaitAll(new Task[] { productsTask, ordersTask, inventoryHistoriesTask });

			List<Product> products = productsTask.Result
				.Where(w => w.ASIN != null)
				.ToList();

			Dictionary<string, LookupProductResponse> lookupProductResponses = products
				.Select(s => new
				{
					s.ASIN,
					LookupProductResponse = m_productAdvertisingApi.LookupProduct(new LookupProductRequest
					{
						ItemId = s.ASIN,
						IdType = IdType.ASIN,
						ResponseGroup = ResponseGroup.Medium
					})
				})
				.ToDictionary(k => k.ASIN, v => v.LookupProductResponse);

			Dictionary<string, OrderSummary> groupedOrders = ordersTask.Result
				.GroupBy(g => g.ASIN)
				.ToDictionary(k => k.Key, v => v.Single());

			Dictionary<string, InventorySummary> inventorySummaries = inventoryHistoriesTask.Result
				.ToDictionary(k => k.ASIN, v => v);

			List<InventoryReportLine> inventoryReportLines = products.Select(
				s =>
				{
					InventoryReportLine inventoryReportLine = new InventoryReportLine
					{
						Name = s.Name,
						ItemNumber = s.ItemNumber,
						ASIN = s.ASIN,
						SKU = s.SKU,
						Cost = s.Cost,
						QuantityInCase = s.QuantityInCase
					};

					if (s.ASIN != null)
					{
						InventorySummary inventorySummary = null;
						OrderSummary orderSummary = null;

						if (inventorySummaries.ContainsKey(s.ASIN))
						{
							inventorySummary = inventorySummaries[s.ASIN];

							inventoryReportLine.CurrentAfnFulfillableQuantity = inventorySummary.CurrentAfnFulfillableQuantity;
							inventoryReportLine.DaysInStockDuringTimeframe = inventorySummary.DaysInStockDuringTimeframe;
						}

						if (groupedOrders.ContainsKey(s.ASIN))
						{
							orderSummary = groupedOrders[s.ASIN];

							inventoryReportLine.QuantitySoldTimeframe = orderSummary.TotalQuantity;
							inventoryReportLine.TotalExtensionSoldTimeframe = orderSummary.TotalExtension;
						}

						if (inventorySummary != null && orderSummary != null)
						{
							int currentAfnFulfillableQuantity = inventorySummary.CurrentAfnFulfillableQuantity;

							decimal quantitySoldPerDayInStock = inventorySummary.DaysInStockDuringTimeframe > 0
								? orderSummary.TotalQuantity/inventorySummary.DaysInStockDuringTimeframe
								: 0;

							inventoryReportLine.QuantitySoldPerDayInStock = Math.Round(quantitySoldPerDayInStock, 2);

							int recommendedReorderQuantity = (int) Math.Round((quantitySoldPerDayInStock * (inventoryReportRequest.DaysToReorder + inventoryReportRequest.DaysLeadTime) - currentAfnFulfillableQuantity), 0);

							inventoryReportLine.RecommendedReorderQuantity = recommendedReorderQuantity > 0
								? recommendedReorderQuantity
								: 0;
						}

						LookupProductResponse lookupProductResponse = lookupProductResponses[s.ASIN];

						if (lookupProductResponse != null && lookupProductResponse.ProductMetadata != null)
							inventoryReportLine.SellersRank = lookupProductResponse.ProductMetadata.SalesRank;
					}

					return inventoryReportLine;
				}).ToList();

			string fileName = string.Format("{0}_InventoryReport_{1}", inventoryReportRequest.ManufacturerName, DateTime.UtcNow.ToShortDateString());

			ExcelUtility.WriteExcelFileToResponse(Response, inventoryReportLines.OrderByDescending(o => o.CurrentAfnFulfillableQuantity), c_worksheet1Name, fileName);
		}

		private readonly IAmazonMWSdbService m_amazonMwSdbService;
		private readonly IProductAdvertisingApi m_productAdvertisingApi;

		private const string c_worksheet1Name = "InventoryReport";
	}
}

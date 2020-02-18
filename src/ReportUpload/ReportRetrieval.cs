using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmazonProductLookup.AmazonApis.MwsApi.Products;
using Data;
using Data.DataModels;
using Models;
using Utility;

namespace ReportUpload
{
	public class ReportRetrieval
	{
		public ReportRetrieval(IMwsProductsApi mwsProductsApi, AmazonMWSdbService amazonMwSdbService)
		{
			m_mwsProductsApi = mwsProductsApi;
			m_amazonMwSdbService = amazonMwSdbService;
		}

		public string GetUnsuppressedInventoryReport()
		{
			string result;

			using (MemoryStream memoryStreamUnsuppressedInventory = m_mwsProductsApi.GetReportData(ReportType._GET_FBA_MYI_UNSUPPRESSED_INVENTORY_DATA_))
			{
				if (memoryStreamUnsuppressedInventory != null)
				{
					List<UnsuppressedInventoryDto> unsuppressedInventories = memoryStreamUnsuppressedInventory.ConvertFromTsvToList<UnsuppressedInventoryDto>();

					result = m_amazonMwSdbService.InsertUnsuppressedInventoryData(unsuppressedInventories);
				}
				else
				{
					result = "Error! MemoryStream in GetUnsuppressedInventoryReport was null.";
				}
			}

			return result;
		}

		public string GetAllOrdersReport()
		{
			string result;

			using (MemoryStream memoryStreamAllOrders = m_mwsProductsApi.GetReportData(ReportType._GET_FLAT_FILE_ALL_ORDERS_DATA_BY_ORDER_DATE_, m_endDate.AddDays(-30), m_endDate))
			{
				if (memoryStreamAllOrders != null)
				{
					List<AllOrderDto> allOrders = memoryStreamAllOrders.ConvertFromTsvToList<AllOrderDto>();

					result = m_amazonMwSdbService.InsertAllOrdersData(allOrders);
				}
				else
				{
					result = "Error! MemoryStream in GetAllOrdersReport was null.";
				}
			}

			return result;
		}

		public string GetFeePreviewReport()
		{
			string result;

			using (MemoryStream memoryStreamFeePreview = m_mwsProductsApi.GetReportData(ReportType._GET_FBA_ESTIMATED_FBA_FEES_TXT_DATA_, m_endDate.AddDays(-30), m_endDate))
			{
				if (memoryStreamFeePreview != null)
				{
					List<FeePreviewDto> feePreviews = memoryStreamFeePreview.ConvertFromTsvToList<FeePreviewDto>();

					result = m_amazonMwSdbService.InsertFeePreviewData(feePreviews);
				}
				else
				{
					result = "Error! MemoryStream in GetFeePreviewReport was null.";
				}
			}

			return result;
		}

		public string GetDailyInventoryReport()
		{
			string result;

			using (MemoryStream memoryStreamDailyInventory = m_mwsProductsApi.GetReportData(ReportType._GET_FBA_FULFILLMENT_CURRENT_INVENTORY_DATA_, m_endDate.AddDays(-7), m_endDate))
			{
				List<DailyInventoryDto> dailyInventoryDtos = memoryStreamDailyInventory.ConvertFromTsvToList<DailyInventoryDto>();

				List<InventoryData> inventoryDatas = dailyInventoryDtos
						.GroupBy(g => new { g.snapshot_date.Date, g.fnsku })
						.Select(s =>
							new InventoryData
								{
									Date = s.Key.Date,
									ASIN = s.Key.fnsku,
									ProductName = s.Select(z => z.product_name).FirstOrDefault(),
									SellableQuantity = s.Where(w => w.detailed_disposition == c_sellable).Sum(z => z.quantity),
									DefectiveQuantity = s.Where(w => w.detailed_disposition == c_defective).Sum(z => z.quantity),
									WarehouseDamagedQuantity = s.Where(w => w.detailed_disposition == c_warehouseDamaged).Sum(z => z.quantity),
									CustomerDamagedQuantity = s.Where(w => w.detailed_disposition == c_customerDamaged).Sum(z => z.quantity),
									DistributorDamagedQuantity = s.Where(w => w.detailed_disposition == c_distributorDamaged).Sum(z => z.quantity)
						})
						.ToList();

				result = m_amazonMwSdbService.InsertInventoryData(inventoryDatas);
			}

			return result;
		}

		private readonly DateTime m_endDate = DateTime.Now;

		private readonly IMwsProductsApi m_mwsProductsApi;
		private readonly AmazonMWSdbService m_amazonMwSdbService;

		private const string c_sellable = "SELLABLE";
		private const string c_defective = "DEFECTIVE";
		private const string c_warehouseDamaged = "WHSE_DAMAGED";
		private const string c_customerDamaged = "CUST_DAMAGED";
		private const string c_distributorDamaged = "DIST_DAMAGED";
	}
}

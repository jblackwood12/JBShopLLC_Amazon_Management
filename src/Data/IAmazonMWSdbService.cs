using System;
using System.Collections.Generic;
using Data.DataModels;
using Models;
using Utility.Models.Date;

namespace Data
{
	public interface IAmazonMWSdbService
	{
		/// <summary>
		///  This retrieves the Entity Framework Context, to manipulate the database directly.
		/// </summary>
		AmazonMWSdb GetContextInstance();

		List<UnpricedProduct> GetUnpricedProducts();

		PriceHistory GetMostRecentAsinPrice(string asin);

		List<DataPoint> GetSalesByDay(DateTime beginDate, DateTime endDate);

		List<ProductSearchResult> SearchForProducts(string query);

		List<DataPoint> GetPriceHistories(string asin, DateTime beginDate, DateTime endDate);

		decimal? GetCostOfProductForAsin(string asin);

		Models.Product GetProduct(string asin);

		Models.Product GetProduct(long productId);

		Models.Product EditProduct(Models.Product product);

		void InsertPriceHistory(List<PriceHistory> priceHistory);

		void InsertListingOffersLog(List<ListingOffersLog> listingOffersLogs);

		Dictionary<string, PriceHistory> GetLatestPriceHistoryForToday();

		void AuditLoginAttempt(string username, bool isSuccessfulLogin);

		string InsertUnsuppressedInventoryData(IEnumerable<UnsuppressedInventoryDto> unsuppressedInventoryDtos);

		string InsertAllOrdersData(IEnumerable<AllOrderDto> allOrdersDtos);

		string InsertFeePreviewData(IEnumerable<FeePreviewDto> feePreviewDtos);

		void InsertManufacturer(string manufacturerName);

		void InsertProducts(IEnumerable<Models.Product> items, int manufacturerId);

		List<Models.Product> GetProductsForManufacturer(int manufacturerId);

		List<OrderSummary> GetOrderSummaryForManufacturer(int manufacturerId, DateTime beginDate, DateTime endDate);

		List<Order> GetOrdersForManufacturer(int manufacturerId, DateTime beginDate, DateTime endDate);

		List<DataPoint> GetProductSalesByDay(string asin, DateTime beginDate, DateTime endDate);

		List<DataPoint> GetInventoryHistoryByDay(string asin, DateTime beginDate, DateTime endDate);

		List<CurrentInventory> GetCurrentInventoryForManufacturer(int manufacturerId);

		List<InventorySummary> GetInventorySummaryForManufacturer(int manufacturerId, DateTime beginDate, DateTime endDate);

		List<Manufacturer> FindManufacturers(string manufacturerString);

		List<FeePreview> GetFeePreviewForAsin(string asin = null);

		List<RepricingInformation> GetAllRepricingInformations();

		List<EFModels.RepricingInformation> FindRepricingInformations(string query, int numResults);

		decimal GetTotalInventoryValue();
	}
}

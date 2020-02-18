namespace Models
{
	public sealed class InventoryReportLine
	{
		public string Name { get; set; }

		public string ItemNumber { get; set; }

		public string ASIN { get; set; }

		public string SKU { get; set; }

		public decimal? Cost { get; set; }

		public long? QuantityInCase { get; set; }

		public long CurrentAfnFulfillableQuantity { get; set; }

		public decimal QuantitySoldTimeframe { get; set; }

		public decimal TotalExtensionSoldTimeframe { get; set; }

		public int DaysInStockDuringTimeframe { get; set; }

		public decimal? QuantitySoldPerDayInStock { get; set; }

		public string SellersRank { get; set; }

		public int RecommendedReorderQuantity { get; set; }
	}
}

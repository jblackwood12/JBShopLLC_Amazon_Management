namespace Models
{
	public sealed class ManufacturerPriceListRowOutput
	{
		public string ASIN { get; set; }

		public string UPC { get; set; }

		public string ProductName { get; set; }

		public string SellersRank { get; set; }

		public string SellersRankCategory { get; set; }

		public string Category { get; set; }

		// TODO: Add Ratings to be returned.
		////public decimal? ReviewCount { get; set; }

		////public decimal? ReviewAverageRating { get; set; }

		public decimal Price { get; set; }

		public string BreakEven { get; set; }

		public string LowestPrice { get; set; }

		public string ProfitMargin { get; set; }
	}
}

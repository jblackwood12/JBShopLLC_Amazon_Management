namespace Models
{
	public sealed class Listing
	{
		// Information from Product
		public string ASIN { get; set; }

		// Information from Listing
		public decimal? SellersRank { get; set; }

		public string SellersRankCategory { get; set; }

		public decimal? LowestPrice { get; set; }

		public int NumberOfNewOffers { get; set; }

		public int NumberOfMerchantFulfilledOffers { get; set; }

		public int NumberOfAmazonFulfilledOrders { get; set; }
	}
}

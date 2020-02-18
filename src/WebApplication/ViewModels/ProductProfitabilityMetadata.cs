namespace WebApplication.ViewModels
{
	public sealed class ProductProfitabilityMetadata
	{
		public string ASIN { get; set; }

		public decimal? Length { get; set; }

		public decimal? Width { get; set; }

		public decimal? Height { get; set; }

		public decimal? Weight { get; set; }

		public string SellersRank { get; set; }
	}
}
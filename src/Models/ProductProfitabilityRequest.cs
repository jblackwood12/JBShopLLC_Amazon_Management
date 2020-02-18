namespace Models
{
	public sealed class ProductProfitabilityRequest
	{
		public FeeCategory FeeCategory { get; set; }

		public decimal PurchasePrice { get; set; }

		public decimal LowestPrice { get; set; }

		public decimal Length { get; set; }

		public decimal Width { get; set; }

		public decimal Height { get; set; }

		public decimal Weight { get; set; }

		public decimal? CostPerPoundToShip { get; set; }

		public decimal? FlatCostToShip { get; set; }
	}
}

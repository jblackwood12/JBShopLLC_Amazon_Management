namespace Models
{
	public sealed class OrderSummary
	{
		public string ASIN { get; set; }

		public decimal TotalQuantity { get; set; }

		public decimal TotalExtension { get; set; }
	}
}

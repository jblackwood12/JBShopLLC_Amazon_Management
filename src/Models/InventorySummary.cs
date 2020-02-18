namespace Models
{
	public sealed class InventorySummary
	{
		public string ASIN { get; set; }

		public int DaysInStockDuringTimeframe { get; set; }

		public int CurrentAfnFulfillableQuantity { get; set; }
	}
}

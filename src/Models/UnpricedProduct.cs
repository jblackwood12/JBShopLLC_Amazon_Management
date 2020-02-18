namespace Models
{
	public class UnpricedProduct
	{
		public string ASIN { get; set; }

		public string SKU { get; set; }

		public string ProductName { get; set; }

		public int FulfillableQuantity { get; set; }

		public int WarehouseQuantity { get; set; }
	}
}

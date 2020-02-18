namespace Models
{
	public sealed class ShipmentItem
	{
		public string Name { get; set; }

		public string ItemNumber { get; set; }

		public string Asin { get; set; }

		public string Sku { get; set; }

		public decimal Quantity { get; set; }

		public decimal Cost { get; set; }

		public decimal? QuantityInCase { get; set; }
	}
}

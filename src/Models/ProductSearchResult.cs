namespace Models
{
	public sealed class ProductSearchResult
	{
		public long ProductId { get; set; }

		public string UPC { get; set; }

		public string Name { get; set; }

		public string ASIN { get; set; }

		public string SKU { get; set; }

		public string ItemNumber { get; set; }
	}
}

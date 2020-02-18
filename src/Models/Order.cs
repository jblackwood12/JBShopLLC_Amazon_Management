using System;

namespace Models
{
	public sealed class Order
	{
		public DateTime PurchaseDate { get; set; }

		public long Quantity { get; set; }

		public decimal ItemPrice { get; set; }

		public string SKU { get; set; }

		public string ASIN { get; set; }
	}
}

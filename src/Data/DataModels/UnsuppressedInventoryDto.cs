using Models;

namespace Data.DataModels
{
	public sealed class UnsuppressedInventoryDto
	{
		[Sanitize(maximumCharacters: 50)]
		public string sku { get; set; }

		[Sanitize(maximumCharacters: 50)]
		public string fnsku { get; set; }

		[Sanitize(maximumCharacters: 50)]
		public string asin { get; set; }

		[Alias("product-name")]
		public string product_name { get; set; }

		[Sanitize(maximumCharacters: 20)]
		public string condition { get; set; }

		[Alias("your-price")]
		public decimal? your_price { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("mfn-listing-exists")]
		public string mfn_listing_exists { get; set; }

		[Alias("mfn-fulfillable-quantity")]
		public long? mfn_fulfillable_quantity { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("afn-listing-exists")]
		public string afn_listing_exists { get; set; }

		[Alias("afn-warehouse-quantity")]
		public long? afn_warehouse_quantity { get; set; }

		[Alias("afn-fulfillable-quantity")]
		public long? afn_fulfillable_quantity { get; set; }

		[Alias("afn-unsellable-quantity")]
		public long? afn_unsellable_quantity { get; set; }

		[Alias("afn-reserved-quantity")]
		public long? afn_reserved_quantity { get; set; }

		[Alias("afn-total-quantity")]
		public long? afn_total_quantity { get; set; }

		[Alias("per-unit-volume")]
		public decimal? per_unit_volume { get; set; }

		[Alias("afn-inbound-working-quantity")]
		public long? afn_inbound_working_quantity { get; set; }

		[Alias("afn-inbound-shipped-quantity")]
		public long? afn_inbound_shipped_quantity { get; set; }

		[Alias("afn-inbound-receiving-quantity")]
		public long? afn_inbound_receiving_quantity { get; set; }
	}
}

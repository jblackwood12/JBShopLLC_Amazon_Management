using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	[Table("UnsuppressedInventoryData")]
	public class UnsuppressedInventoryData
	{
		public long UnsuppressedInventoryDataId { get; set; }

		[Column(TypeName = "date")]
		public DateTime? Date { get; set; }

		[StringLength(50)]
		public string sku { get; set; }

		[StringLength(50)]
		public string fnsku { get; set; }

		[StringLength(50)]
		public string asin { get; set; }

		[Column("product-name")]
		public string product_name { get; set; }

		[StringLength(20)]
		public string condition { get; set; }

		[Column("your-price", TypeName = "money")]
		public decimal? your_price { get; set; }

		[Column("mfn-listing-exists")]
		[StringLength(20)]
		public string mfn_listing_exists { get; set; }

		[Column("mfn-fulfillable-quantity")]
		public long? mfn_fulfillable_quantity { get; set; }

		[Column("afn-listing-exists")]
		[StringLength(20)]
		public string afn_listing_exists { get; set; }

		[Column("afn-warehouse-quantity")]
		public long? afn_warehouse_quantity { get; set; }

		[Column("afn-fulfillable-quantity")]
		public long? afn_fulfillable_quantity { get; set; }

		[Column("afn-unsellable-quantity")]
		public long? afn_unsellable_quantity { get; set; }

		[Column("afn-reserved-quantity")]
		public long? afn_reserved_quantity { get; set; }

		[Column("afn-total-quantity")]
		public long? afn_total_quantity { get; set; }

		[Column("per-unit-volume")]
		public decimal? per_unit_volume { get; set; }

		[Column("afn-inbound-working-quantity")]
		public long? afn_inbound_working_quantity { get; set; }

		[Column("afn-inbound-shipped-quantity")]
		public long? afn_inbound_shipped_quantity { get; set; }

		[Column("afn-inbound-receiving-quantity")]
		public long? afn_inbound_receiving_quantity { get; set; }
	}
}

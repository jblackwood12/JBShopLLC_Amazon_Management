using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	[Table("AllOrdersData")]
	public class AllOrdersData
	{
		[Key]
		[Column("amazon-order-id", Order = 0)]
		[StringLength(20)]
		public string amazon_order_id { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(20)]
		public string sku { get; set; }

		[StringLength(20)]
		public string asin { get; set; }

		[Column("product-name", TypeName = "text")]
		public string product_name { get; set; }

		[Column("merchant-order-id")]
		[StringLength(20)]
		public string merchant_order_id { get; set; }

		[Column("purchase-date", TypeName = "datetime2")]
		public DateTime? purchase_date { get; set; }

		[Column("last-updated-date", TypeName = "datetime2")]
		public DateTime? last_updated_date { get; set; }

		[Column("order-status")]
		[StringLength(20)]
		public string order_status { get; set; }

		[Column("fulfillment-channel")]
		[StringLength(20)]
		public string fulfillment_channel { get; set; }

		[Column("sales-channel")]
		[StringLength(20)]
		public string sales_channel { get; set; }

		[Column("order-channel")]
		[StringLength(20)]
		public string order_channel { get; set; }

		[StringLength(50)]
		public string url { get; set; }

		[Column("ship-service-level")]
		[StringLength(20)]
		public string ship_service_level { get; set; }

		[Column("item-status")]
		[StringLength(20)]
		public string item_status { get; set; }

		public long? quantity { get; set; }

		[StringLength(20)]
		public string currency { get; set; }

		[Column("item-price", TypeName = "money")]
		public decimal? item_price { get; set; }

		[Column("item-tax", TypeName = "money")]
		public decimal? item_tax { get; set; }

		[Column("shipping-price", TypeName = "money")]
		public decimal? shipping_price { get; set; }

		[Column("shipping-tax", TypeName = "money")]
		public decimal? shipping_tax { get; set; }

		[Column("gift-wrap-price", TypeName = "money")]
		public decimal? gift_wrap_price { get; set; }

		[Column("gift-wrap-tax", TypeName = "money")]
		public decimal? gift_wrap_tax { get; set; }

		[Column("item-promotion-discount", TypeName = "money")]
		public decimal? item_promotion_discount { get; set; }

		[Column("ship-promotion-discount", TypeName = "money")]
		public decimal? ship_promotion_discount { get; set; }

		[Column("ship-city")]
		[StringLength(50)]
		public string ship_city { get; set; }

		[Column("ship-state")]
		[StringLength(20)]
		public string ship_state { get; set; }

		[Column("ship-postal-code")]
		[StringLength(20)]
		public string ship_postal_code { get; set; }

		[Column("ship-country")]
		[StringLength(50)]
		public string ship_country { get; set; }

		[Column("promotion-ids")]
		[StringLength(50)]
		public string promotion_ids { get; set; }
	}
}

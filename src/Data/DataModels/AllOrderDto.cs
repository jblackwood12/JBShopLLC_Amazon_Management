using System;
using Models;

namespace Data.DataModels
{
	public sealed class AllOrderDto
	{
		[Sanitize(maximumCharacters: 20)]
		[Alias("amazon-order-id")]
		public string amazon_order_id { get; set; }

		[Sanitize(maximumCharacters: 20)]
		public string sku { get; set; }

		[Sanitize(maximumCharacters: 20)]
		public string asin { get; set; }

		[Alias("product-name")]
		public string product_name { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("merchant-order-id")]
		public string merchant_order_id { get; set; }

		[Alias("purchase-date")]
		public DateTime purchase_date { get; set; }

		[Alias("last-updated-date")]
		public DateTime last_updated_date { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("order-status")]
		public string order_status { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("fulfillment-channel")]
		public string fulfillment_channel { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("sales-channel")]
		public string sales_channel { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("order-channel")]
		public string order_channel { get; set; }

		[Sanitize(maximumCharacters: 50)]
		public string url { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("ship-service-level")]
		public string ship_service_level { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("item-status")]
		public string item_status { get; set; }

		public long? quantity { get; set; }

		[Sanitize(maximumCharacters: 20)]
		public string currency { get; set; }

		[Alias("item-price")]
		public decimal? item_price { get; set; }

		[Alias("item-tax")]
		public decimal? item_tax { get; set; }

		[Alias("shipping-price")]
		public decimal? shipping_price { get; set; }

		[Alias("shipping-tax")]
		public decimal? shipping_tax { get; set; }

		[Alias("gift-wrap-price")]
		public decimal? gift_wrap_price { get; set; }

		[Alias("gift-wrap-tax")]
		public decimal? gift_wrap_tax { get; set; }

		[Alias("item-promotion-discount")]
		public decimal? item_promotion_discount { get; set; }

		[Alias("ship-promotion-discount")]
		public decimal? ship_promotion_discount { get; set; }

		[Sanitize(maximumCharacters: 50)]
		[Alias("ship-city")]
		public string ship_city { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("ship-state")]
		public string ship_state { get; set; }

		[Sanitize(maximumCharacters: 20)]
		[Alias("ship-postal-code")]
		public string ship_postal_code { get; set; }

		[Sanitize(maximumCharacters: 50)]
		[Alias("ship-country")]
		public string ship_country { get; set; }

		[Sanitize(maximumCharacters: 50)]
		[Alias("promotion-ids")]
		public string promotion_ids { get; set; }
	}
}

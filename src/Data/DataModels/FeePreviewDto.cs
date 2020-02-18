using Models;

namespace Data.DataModels
{
	public sealed class FeePreviewDto
	{
		[Sanitize(maximumCharacters: 50)]
		public string sku { get; set; }

		[Sanitize(maximumCharacters: 50)]
		public string fnsku { get; set; }

		[Sanitize(maximumCharacters: 50)]
		public string asin { get; set; }

		[Sanitize(maximumCharacters: 255)]
		[Alias("product-name")]
		public string product_name { get; set; }

		[Sanitize(maximumCharacters: 255)]
		[Alias("product-group")]
		public string product_group { get; set; }

		[Sanitize(maximumCharacters: 255)]
		public string brand { get; set; }

		[Sanitize(maximumCharacters: 255)]
		[Alias("fulfilled-by")]
		public string fulfilled_by { get; set; }

		[Alias("your-price")]
		public decimal your_price { get; set; }

		[Alias("sales-price")]
		public decimal sales_price { get; set; }

		[Alias("longest-side")]
		public decimal longest_side { get; set; }

		[Alias("median-side")]
		public decimal median_side { get; set; }

		[Alias("shortest-side")]
		public decimal shortest_side { get; set; }

		[Alias("length-and-girth")]
		public decimal length_and_girth { get; set; }

		[Sanitize(maximumCharacters: 255)]
		[Alias("unit-of-dimension")]
		public string unit_of_dimension { get; set; }

		[Alias("item-package-weight")]
		public decimal item_package_weight { get; set; }

		[Sanitize(maximumCharacters: 255)]
		[Alias("unit-of-weight")]
		public string unit_of_weight { get; set; }

		[Sanitize(maximumCharacters: 255)]
		[Alias("product-size-tier")]
		public string product_size_tier { get; set; }

		[Sanitize(maximumCharacters: 255)]
		public string currency { get; set; }

		[Alias("estimated-fee")]
		public decimal estimated_fee { get; set; }

		[Alias("estimated-referral-fee-per-unit")]
		public decimal estimated_referral_fee_per_unit { get; set; }

		[Alias("estimated-variable-closing-fee")]
		public decimal estimated_variable_closing_fee { get; set; }

		[Alias("estimated-order-handling-fee-per-order")]
		public decimal estimated_order_handling_fee_per_order { get; set; }

		[Alias("estimated-pick-pack-fee-per-unit")]
		public decimal estimated_pick_pack_fee_per_unit { get; set; }

		[Alias("estimated-weight-handling-fee-per-unit")]
		public decimal estimated_weight_handling_fee_per_unit { get; set; }
	}
}

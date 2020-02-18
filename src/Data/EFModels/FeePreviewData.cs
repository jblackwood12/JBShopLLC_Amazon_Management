using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	[Table("FeePreviewData")]
	public class FeePreviewData
	{
		[Key]
		public long FeePreviewId { get; set; }

		[Column(TypeName = "date")]
		public DateTime Date { get; set; }

		[StringLength(50)]
		public string sku { get; set; }

		[StringLength(50)]
		public string fnsku { get; set; }

		[StringLength(50)]
		public string asin { get; set; }

		[Column("product-name")]
		[StringLength(255)]
		public string product_name { get; set; }

		[Column("product-group")]
		[StringLength(255)]
		public string product_group { get; set; }

		[StringLength(255)]
		public string brand { get; set; }

		[Column("fulfilled-by")]
		[StringLength(255)]
		public string fulfilled_by { get; set; }

		[Column("your-price")]
		public decimal your_price { get; set; }

		[Column("sales-price")]
		public decimal sales_price { get; set; }

		[Column("longest-side")]
		public decimal longest_side { get; set; }

		[Column("median-side")]
		public decimal median_side { get; set; }

		[Column("shortest-side")]
		public decimal shortest_side { get; set; }

		[Column("length-and-girth")]
		public decimal length_and_girth { get; set; }

		[Column("unit-of-dimension")]
		[StringLength(255)]
		public string unit_of_dimension { get; set; }

		[Column("item-package-weight")]
		public decimal item_package_weight { get; set; }

		[Column("unit-of-weight")]
		[StringLength(255)]
		public string unit_of_weight { get; set; }

		[Column("product-size-tier")]
		[StringLength(255)]
		public string product_size_tier { get; set; }

		[StringLength(255)]
		public string currency { get; set; }

		[Column("estimated-fee")]
		public decimal estimated_fee { get; set; }

		[Column("estimated-referral-fee-per-unit")]
		public decimal estimated_referral_fee_per_unit { get; set; }

		[Column("estimated-variable-closing-fee")]
		public decimal estimated_variable_closing_fee { get; set; }

		[Column("estimated-order-handling-fee-per-order")]
		public decimal estimated_order_handling_fee_per_order { get; set; }

		[Column("estimated-pick-pack-fee-per-unit")]
		public decimal estimated_pick_pack_fee_per_unit { get; set; }

		[Column("estimated-weight-handling-fee-per-unit")]
		public decimal estimated_weight_handling_fee_per_unit { get; set; }
	}
}

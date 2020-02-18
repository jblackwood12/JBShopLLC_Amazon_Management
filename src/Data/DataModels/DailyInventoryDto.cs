using System;
using Models;

namespace Data.DataModels
{
	public sealed class DailyInventoryDto
	{
		[Alias("snapshot-date")]
		public DateTime snapshot_date { get; set; }

		[Sanitize(maximumCharacters: 50)]
		public string fnsku { get; set; }

		[Sanitize(maximumCharacters: 50)]
		public string sku { get; set; }

		[Alias("product-name")]
		[Sanitize(maximumCharacters: 255)]
		public string product_name { get; set; }

		public int quantity { get; set; }

		[Alias("fulfillment-center-id")]
		[Sanitize(maximumCharacters: 50)]
		public string fulfillment_center_id { get; set; }

		[Alias("detailed-disposition")]
		[Sanitize(maximumCharacters: 50)]
		public string detailed_disposition { get; set; }
	}
}

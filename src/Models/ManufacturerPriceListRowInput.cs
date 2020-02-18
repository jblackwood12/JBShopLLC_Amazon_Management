namespace Models
{
	public sealed class ManufacturerPriceListRowInput
	{
		public string ItemNumber { get; set; }

		[Sanitize(InputType.ASIN)]
		public string ASIN { get; set; }

		[Sanitize(InputType.UPC)]
		public string UPC { get; set; }

		[Sanitize(InputType.Money)]
		[Alias("Cost")]
		public decimal? Price { get; set; }
	}
}
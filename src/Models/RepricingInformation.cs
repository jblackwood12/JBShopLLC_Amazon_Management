namespace Models
{
	public sealed class RepricingInformation
	{
		public int? RepricingInformationId { get; set; }

		public string SKU { get; set; }

		public string ASIN { get; set; }

		public decimal MinimumPrice { get; set; }

		public string ProductName { get; set; }

		public bool IsCurrent { get; set; }
	}
}

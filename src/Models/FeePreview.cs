namespace Models
{
	public sealed class FeePreview
	{
		public string ASIN { get; set; }

		public string SKU { get; set; }

		public string Name { get; set; }

		public decimal EstimatedOrderHandlingFeePerOrder { get; set; }

		public decimal EstimatedPickPackFeePerUnit { get; set; }

		public decimal EstimatedWeightHandlingFeePerUnit { get; set; }
	}
}

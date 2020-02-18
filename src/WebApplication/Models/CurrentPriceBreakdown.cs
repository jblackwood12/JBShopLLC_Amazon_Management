using System;

namespace WebApplication.Models
{
	public sealed class CurrentPriceBreakdown
	{
		public decimal NewestPrice { get; set; }

		public DateTime LastUpdatedPriceDateTime { get; set; }

		public decimal AmazonReferralFee { get; set; }

		public decimal PerOrderFeePreview { get; set; }

		public decimal PerUnitFeePreview { get; set; }

		public decimal WeightFeePreview { get; set; }

		public decimal ItemCost { get; set; }

		public decimal EstShippingToAmazon { get; set; }

		public decimal EstReturns { get; set; }

		public decimal EstStorageFee { get; set; }

		public decimal EstimatedProfit { get; set; }

		public string MissingInfo { get; set; }
	}
}
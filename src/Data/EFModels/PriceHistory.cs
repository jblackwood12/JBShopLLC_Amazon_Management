using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	[Table("PriceHistory")]
	public class PriceHistory
	{
		public long PriceHistoryId { get; set; }

		public decimal NewPrice { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime TimeStamp { get; set; }

		[Required]
		[StringLength(50)]
		public string ASIN { get; set; }

		public decimal? BreakEvenPrice { get; set; }

		public decimal? MyOfferPriceInNotification { get; set; }

		public decimal? AmazonsOfferPriceInNotification { get; set; }

		public decimal? LowestFbaOfferPriceInNotification { get; set; }

		public decimal? LowestNonFbaOfferPriceInNotification { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime? LastNotificationPublishDateTime { get; set; }

		public decimal? MyPriceFromProductsApi { get; set; }

		[StringLength(30)]
		public string ListingOffersSource { get; set; }
	}
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	[Table("ListingOffersLog")]
	public class ListingOffersLog
	{
		[Key]
		public long NotificationLogId { get; set; }

		[Required]
		[StringLength(20)]
		public string ASIN { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime PublishDateTime { get; set; }

		[Column(TypeName = "money")]
		public decimal? LowestFbaPrice { get; set; }

		[Column(TypeName = "money")]
		public decimal? LowestNonFbaPrice { get; set; }

		[Column(TypeName = "money")]
		public decimal? OurPrice { get; set; }

		[Required]
		[StringLength(30)]
		public string ListingOffersSource { get; set; }

		public int? BuyBoxWinnerSellerType { get; set; }

		[Column(TypeName = "money")]
		public decimal? BuyBoxPrice { get; set; }
	}
}

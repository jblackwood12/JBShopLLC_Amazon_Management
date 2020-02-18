using System.ComponentModel.DataAnnotations;
using System.Web;
using Models;

namespace WebApplication.Models
{
	public sealed class ManufacturerAssessmentFile
	{
		[Required]
		public HttpPostedFileBase File { get; set; }

		public FeeCategory FeeCategory { get; set; }

		public bool IgnoreItemsWithoutDimensions { get; set; }

		public decimal? HighestSellersRank { get; set; }

		public decimal? LowestProfitMargin { get; set; }
	}
}
using System.ComponentModel.DataAnnotations;
using System.Web;
using Models;
using Models.AdvApi;

namespace WebApplication.Models
{
	public sealed class PriceListFile
	{
		[Required]
		public HttpPostedFileBase File { get; set; }

		public SearchIndex SearchIndex { get; set; }

		public FeeCategory FeeCategory { get; set; }
	}
}
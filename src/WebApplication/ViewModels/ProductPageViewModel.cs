using Models;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
	public class ProductPageViewModel
	{
		public Product Product { get; set; }

		public ProductMetadata ProductMetadata { get; set; }

		public CurrentPriceBreakdown CurrentPriceBreakdownData { get; set; }
	}
}

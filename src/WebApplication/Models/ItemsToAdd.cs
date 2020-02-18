using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebApplication.Models
{
	public sealed class ProductsToAdd
	{
		[Required]
		public HttpPostedFileBase File { get; set; }

		public int ManufacturerId { get; set; }
	}
}
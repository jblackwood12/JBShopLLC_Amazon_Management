using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebApplication.Models
{
	public sealed class CreateShipmentFile
	{
		[Required]
		public HttpPostedFileBase File { get; set; }

		public string ShipmentName { get; set; }
	}
}
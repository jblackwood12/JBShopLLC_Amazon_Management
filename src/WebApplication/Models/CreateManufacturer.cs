using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
	public sealed class CreateManufacturer
	{
		[Required]
		public string ManufacturerName { get; set; }
	}
}
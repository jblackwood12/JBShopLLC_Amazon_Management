using System.ComponentModel.DataAnnotations;

namespace Data.EFModels
{
	public class Manufacturer
	{
		public int ManufacturerId { get; set; }

		[StringLength(50)]
		public string Name { get; set; }
	}
}

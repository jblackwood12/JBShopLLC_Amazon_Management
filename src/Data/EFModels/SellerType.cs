using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	[Table("SellerType")]
	public class SellerType
	{
		[Key]
		[Column(Order = 0)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int SellerTypeId { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(20)]
		public string Name { get; set; }
	}
}

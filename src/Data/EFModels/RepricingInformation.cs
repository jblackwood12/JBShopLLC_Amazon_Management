using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	[Table("RepricingInformation")]
	public class RepricingInformation
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RepricingInformationId { get; set; }

		[Required]
		[StringLength(20)]
		[Column(TypeName = "nvarchar")]
		public string SKU { get; set; }

		[Required]
		[StringLength(20)]
		[Column(TypeName = "nvarchar")]
		public string ASIN { get; set; }

		public decimal MinimumPrice { get; set; }

		[Required]
		[StringLength(1024)]
		[Column(TypeName = "nvarchar")]
		public string ProductName { get; set; }

		public bool IsCurrent { get; set; }
	}
}

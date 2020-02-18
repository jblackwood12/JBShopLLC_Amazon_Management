using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	[Table("ProductAmazonIdentifiersAudit")]
	public class ProductAmazonIdentifiersAudit
	{
		public long ProductAmazonIdentifiersAuditId { get; set; }

		public long ProductId { get; set; }

		[StringLength(20)]
		public string ASIN { get; set; }

		[StringLength(20)]
		public string SKU { get; set; }
	}
}

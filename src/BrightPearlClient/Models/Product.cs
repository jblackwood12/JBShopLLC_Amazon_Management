
namespace BrightPearlClient.Models
{
	public class Product
	{
		public int id { get; set; }
		public int brandId { get; set; }
		public int productTypeId { get; set; }
		public Identity identity { get; set; }
		public int productGroupId { get; set; }
		public Stock stock { get; set; }
		public FinancialDetails FinancialDetails { get; set; }
	}
}

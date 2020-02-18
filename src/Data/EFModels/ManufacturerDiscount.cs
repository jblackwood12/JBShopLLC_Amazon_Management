namespace Data.EFModels
{
	public class ManufacturerDiscount
	{
		public int ManufacturerDiscountId { get; set; }

		public int ManufacturerId { get; set; }

		public decimal DiscountPercentage { get; set; }
	}
}

using System;

namespace Models
{
	// This class must be a mirror of the table dbo.Products
	// Add superfluous fields to the class ProductMetadata
	public sealed class Product
	{
		public long? ProductId { get; set; }

		public DateTime CreatedDate { get; set; }

		public DateTime ModifiedDate { get; set; }

		public string UPC { get; set; }

		public string Name { get; set; }

		public decimal? Cost { get; set; }

		public decimal? PromotionCost { get; set; }

		public decimal? OverrideCost { get; set; }

		public decimal? MAPPrice { get; set; }

		public decimal? MinPrice { get; set; }

		public decimal? MaxPrice { get; set; }

		public decimal? BreakevenPrice { get; set; }

		public long? QuantityInCase { get; set; }

		public string ASIN { get; set; }

		public string SKU { get; set; }

		public string ItemNumber { get; set; }

		public int ManufacturerId { get; set; }

		public decimal? Length { get; set; }

		public decimal? Width { get; set; }

		public decimal? Height { get; set; }

		public decimal? Weight { get; set; }

		public bool IsMAP { get; set; }

		public bool IsDiscontinued { get; set; }

		public string Notes { get; set; }
	}
}

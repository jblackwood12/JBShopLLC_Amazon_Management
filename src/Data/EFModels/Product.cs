using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	public class Product
	{
		[Key]
		public long ProductId { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime CreatedDate { get; set; }

		[Column(TypeName = "datetime2")]
		public DateTime ModifiedDate { get; set; }

		[StringLength(20)]
		public string UPC { get; set; }

		[StringLength(255)]
		public string Name { get; set; }

		[Column(TypeName = "money")]
		public decimal? Cost { get; set; }

		[Column(TypeName = "money")]
		public decimal? PromotionCost { get; set; }

		[Column(TypeName = "money")]
		public decimal? OverrideCost { get; set; }

		[Column(TypeName = "money")]
		public decimal? MAPPrice { get; set; }

		[Column(TypeName = "money")]
		public decimal? MinPrice { get; set; }

		[Column(TypeName = "money")]
		public decimal? MaxPrice { get; set; }

		[Column(TypeName = "money")]
		public decimal? BreakevenPrice { get; set; }

		public long? QuantityInCase { get; set; }

		[StringLength(20)]
		public string ASIN { get; set; }

		[StringLength(20)]
		public string SKU { get; set; }

		[StringLength(255)]
		public string ItemNumber { get; set; }

		public int ManufacturerId { get; set; }

		public decimal? Length { get; set; }

		public decimal? Width { get; set; }

		public decimal? Height { get; set; }

		public decimal? Weight { get; set; }

		public bool IsMAP { get; set; }

		public bool IsDiscontinued { get; set; }

		[StringLength(1000)]
		public string Notes { get; set; }
	}
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.EFModels
{
	[Table("InventoryData")]
	public class InventoryData
	{
		public long InventoryDataId { get; set; }

		[Column(TypeName = "date")]
		public DateTime Date { get; set; }

		[Required]
		[StringLength(20)]
		public string ASIN { get; set; }

		[StringLength(255)]
		public string ProductName { get; set; }

		public int SellableQuantity { get; set; }

		public int DefectiveQuantity { get; set; }

		public int WarehouseDamagedQuantity { get; set; }

		public int CustomerDamagedQuantity { get; set; }

		public int DistributorDamagedQuantity { get; set; }
	}
}

using System;

namespace Data.DataModels
{
	public sealed class InventoryData
	{
		public DateTime Date { get; set; }

		public string ASIN { get; set; }

		public string ProductName { get; set; }

		public int SellableQuantity { get; set; }

		public int DefectiveQuantity { get; set; }

		public int WarehouseDamagedQuantity { get; set; }

		public int CustomerDamagedQuantity { get; set; }

		public int DistributorDamagedQuantity { get; set; }
	}
}

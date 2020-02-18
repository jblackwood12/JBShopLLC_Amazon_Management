using System;

namespace Models
{
	public sealed class InventoryHistorySnapshot
	{
		public DateTime Date { get; set; }

		public long FulfillableQuantity { get; set; }

		public long ReservedQuantity { get; set; }

		public long UnsellableQuantity { get; set; }
	}
}

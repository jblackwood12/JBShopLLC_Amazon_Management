using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
	public sealed class InventoryReportRequest
	{
		[Required]
		public int ManufacturerId { get; set; }

		[Required]
		public string ManufacturerName { get; set; }

		// Number of days in the past we want to look at for Order and Inventory history.
		public int DaysOrderAndInventoryHistoryToUse { get; set; }

		// Number of days we want the reordered quantity to last.
		public int DaysToReorder { get; set; }

		// Number of days it takes for the purchase order to reach the fulfillment centers.
		public int DaysLeadTime { get; set; }
	}
}
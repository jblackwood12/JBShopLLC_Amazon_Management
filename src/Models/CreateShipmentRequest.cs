using System.Collections.Generic;
using System.Linq;

namespace Models
{
	public sealed class CreateShipmentRequest
	{
		public List<ShipmentItem> ShipmentItems { get { return m_shipmentItems; } }

		public string ShipmentName { get { return m_shipmentName; } }

		public bool AreCasesRequired { get { return m_areCasesRequired; } }

		public CreateShipmentRequest(IEnumerable<ShipmentItem> shipmentItems, string shipmentName, bool areCasesRequired)
		{
			m_shipmentItems = shipmentItems.ToList();
			m_shipmentName = shipmentName;
			m_areCasesRequired = areCasesRequired;
		}

		private readonly List<ShipmentItem> m_shipmentItems;

		private readonly string m_shipmentName;

		private readonly bool m_areCasesRequired;
	}
}

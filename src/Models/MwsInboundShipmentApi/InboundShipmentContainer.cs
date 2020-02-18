using System.Collections.Generic;
using System.Linq;
using FBAInboundServiceMWS.Model;

namespace Models.MwsInboundShipmentApi
{
	public sealed class InboundShipmentContainer
	{
		public string ShipmentId { get { return m_shipmentId; } }

		public List<InboundShipmentItem> InboundShipmentItems { get { return m_inboundShipmentItems; } }

		public InboundShipmentContainer(string shipmentId, IEnumerable<InboundShipmentItem> inboundShipmentItems)
		{
			m_shipmentId = shipmentId;

			m_inboundShipmentItems = inboundShipmentItems.ToList();
		}

		private readonly string m_shipmentId;

		private List<InboundShipmentItem> m_inboundShipmentItems;
	}
}

using System.Collections.Generic;
using System.Linq;

namespace Models
{
	public sealed class CreatedShipment
	{
		public string ShippingDestination { get { return m_shippingDestination; } }

		public string LabelPrepType { get { return m_labelPrepType; } }

		public string ShipmentId { get { return m_shipmentId; } }

		public List<ShipmentItem> ItemsInShipment { get { return m_itemsInShipment; } }

		public CreatedShipment(string shippingDestination, string labelPrepType, string shipmentId, IEnumerable<ShipmentItem> itemsInShipment)
		{
			m_shippingDestination = shippingDestination;
			m_labelPrepType = labelPrepType;
			m_shipmentId = shipmentId;
			m_itemsInShipment = itemsInShipment.ToList();
		}

		private readonly string m_shippingDestination;

		private readonly string m_labelPrepType;

		private readonly string m_shipmentId;

		private readonly List<ShipmentItem> m_itemsInShipment;
	}
}

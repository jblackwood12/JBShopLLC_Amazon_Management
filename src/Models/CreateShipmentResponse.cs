using System.Collections.Generic;
using System.Linq;

namespace Models
{
	public sealed class CreateShipmentResponse
	{
		public List<CreatedShipment> CreatedShipments { get { return m_createdShipments; } }

		public CreateShipmentResponse(IEnumerable<CreatedShipment> createdShipments)
		{
			m_createdShipments = createdShipments.ToList();
		}

		private readonly List<CreatedShipment> m_createdShipments;
	}
}

using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Models.AdvApi
{
	public sealed class ProductAdvApiResponse
	{
		public List<XmlNode> Nodes { get { return m_nodes; } }

		public bool Errored { get { return m_errored; } }

		public ProductAdvApiResponse(IEnumerable<XmlNode> nodes, bool errored)
		{
			m_nodes = nodes.ToList();
			m_errored = errored;
		}

		public ProductAdvApiResponse(XmlNode node, bool errored)
		{
			m_nodes = new List<XmlNode> { node };
			m_errored = errored;
		}

		private readonly List<XmlNode> m_nodes;

		private readonly bool m_errored;
	}
}

using System.Xml;

namespace AmazonProductLookup.AmazonApis.AdvApi.ItemSearch
{
	internal sealed class ItemSearchProduct
	{
		public string Asin { get { return m_asin; } }

		public int? TotalNew { get { return m_totalNew; } }

		public int? SalesRank { get { return m_salesRank; } }

		public XmlNode Node { get { return m_node; } }

		public ItemSearchProduct(string asin, int? totalNew, int? salesRank, XmlNode node)
		{
			m_asin = asin;
			m_totalNew = totalNew;
			m_salesRank = salesRank;
			m_node = node;
		}

		private readonly string m_asin;

		private readonly int? m_totalNew;

		private readonly int? m_salesRank;

		private readonly XmlNode m_node;
	}
}

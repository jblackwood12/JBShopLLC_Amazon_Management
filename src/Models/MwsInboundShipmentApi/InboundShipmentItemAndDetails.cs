namespace Models.MwsInboundShipmentApi
{
	public sealed class InboundShipmentItemAndDetails
	{
		// The line number is determined by sorting by Sku Descending.
		public int LineNumber { get; set; }

		public string ItemNumber { get { return m_itemNumber; } }

		public string UPC { get { return m_upc; } }

		public string SKU { get { return m_sellerSku; } }

		public string ASIN { get { return m_fulfillmentNetworkSku; } }

		public decimal QuantityInCase { get { return m_quantityInCase; } }

		public decimal QuantityShipped { get { return m_quantityShipped; } }

		public decimal QuantityReceived { get { return m_quantityReceived; } }

		public string Name { get { return m_name; } }

		public InboundShipmentItemAndDetails(string fulfillmentNetworkSku, decimal quantityInCase, decimal quantityReceived, decimal quantityShipped, string sellerSku, string name, string itemNumber, string upc)
		{
			m_fulfillmentNetworkSku = fulfillmentNetworkSku;

			m_quantityInCase = quantityInCase;

			m_quantityReceived = quantityReceived;

			m_quantityShipped = quantityShipped;

			m_sellerSku = sellerSku;

			m_name = name;

			m_itemNumber = itemNumber;

			m_upc = upc;
		}

		private readonly string m_fulfillmentNetworkSku;

		private readonly decimal m_quantityInCase;

		private readonly decimal m_quantityReceived;

		private readonly decimal m_quantityShipped;

		private readonly string m_sellerSku;

		private readonly string m_name;

		private readonly string m_itemNumber;

		private readonly string m_upc;
	}
}

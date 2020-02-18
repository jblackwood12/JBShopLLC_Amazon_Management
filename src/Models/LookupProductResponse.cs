namespace Models
{
	public sealed class LookupProductResponse
	{
		public Product Product { get { return m_product; } }

		public ProductSearchMethod ProductSearchMethod { get { return m_productSearchMethod; } }

		public ProductMetadata ProductMetadata { get { return m_productMetadata; } }

		public LookupProductResponse(Product product, ProductSearchMethod productSearchMethod, ProductMetadata productMetadata = null)
		{
			m_product = product;

			m_productSearchMethod = productSearchMethod;

			m_productMetadata = productMetadata;
		}

		private readonly Product m_product;

		private readonly ProductMetadata m_productMetadata;

		private readonly ProductSearchMethod m_productSearchMethod;
	}
}

namespace Models
{
	public sealed class ProductAndProductMetadata
	{
		public Product Product { get { return m_product; } }

		public ProductMetadata ProductMetadata { get { return m_productMetadata; } }

		public ProductAndProductMetadata(Product product, ProductMetadata productMetadata)
		{
			m_product = product;

			m_productMetadata = productMetadata;
		}

		private readonly Product m_product;

		private readonly ProductMetadata m_productMetadata;
	}
}

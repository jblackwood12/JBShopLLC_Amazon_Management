namespace Models
{
	public sealed class Offer
	{
		public SellerType SellerType { get { return m_sellerType; } }
		public bool IsFba { get { return m_isFba; } }
		public decimal LandedPrice { get { return m_landedPrice; } }
		public bool? IsBuyBoxWinner { get { return m_isBuyBoxWinner; } }

		public Offer(SellerType sellerType, bool isFba, decimal landedPrice, bool? isBuyBoxWinner = null)
		{
			m_sellerType = sellerType;
			m_isFba = isFba;
			m_landedPrice = landedPrice;
			m_isBuyBoxWinner = isBuyBoxWinner;
		}

		private readonly SellerType m_sellerType;
		private readonly bool m_isFba;
		private readonly decimal m_landedPrice;
		private readonly bool? m_isBuyBoxWinner;
	}
}

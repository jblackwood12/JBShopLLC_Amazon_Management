using System;

namespace AmazonProductLookup.AmazonApis.MwsApi.Feeds
{
	public sealed class UpdatedItemPrice
	{
		public string Sku { get { return m_sku; } }

		public decimal UpdatedPrice { get { return m_updatedPrice; } }

		public string Asin { get { return m_asin; } }

		public decimal? BreakEvenPrice { get { return m_breakEvenPrice; } }

		public decimal? MyOfferPriceInNotification { get { return m_myOffer; } }

		public decimal? AmazonsOfferPriceInNotification { get { return m_amazonsOffer; } }

		public decimal? LowestFbaOfferPriceInNotification { get { return m_lowestFbaOffer; } }

		public decimal? LowestNonFbaOfferPriceInNotification { get { return m_lowestNonFbaOffer; } }

		public DateTime? LastNotificationPublishDateTime { get { return m_lastNotificationPublishDateTime; } }

		public decimal? MyPriceFromProductsApi { get { return m_myPriceFromProductsApi; } }

		public string ListingOffersSource { get { return m_listingOffersSource; } }

		public UpdatedItemPrice(string sku, decimal updatedPrice, string asin, decimal breakEvenPrice, decimal? myOffer, decimal? amazonsOffer, decimal? lowestFbaOffer, decimal? lowestNonFbaOffer, DateTime? lastNotificationPublishDateTime, decimal? myPriceFromProductsApi, string listingOffersSource)
		{
			m_sku = sku;
			m_updatedPrice = updatedPrice;
			m_asin = asin;
			m_breakEvenPrice = breakEvenPrice;
			m_myOffer = myOffer;
			m_amazonsOffer = amazonsOffer;
			m_lowestFbaOffer = lowestFbaOffer;
			m_lowestNonFbaOffer = lowestNonFbaOffer;
			m_lastNotificationPublishDateTime = lastNotificationPublishDateTime;
			m_myPriceFromProductsApi = myPriceFromProductsApi;
			m_listingOffersSource = listingOffersSource;
		}

		private readonly string m_sku;
		private readonly decimal m_updatedPrice;
		private readonly string m_asin;
		private readonly decimal m_breakEvenPrice;
		private readonly decimal? m_myOffer;
		private readonly decimal? m_amazonsOffer;
		private readonly decimal? m_lowestFbaOffer;
		private readonly decimal? m_lowestNonFbaOffer;
		private readonly DateTime? m_lastNotificationPublishDateTime;
		private readonly decimal? m_myPriceFromProductsApi;
		private readonly string m_listingOffersSource;
	}
}

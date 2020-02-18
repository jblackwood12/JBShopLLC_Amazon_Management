using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace RepricingScript.Models
{
	public sealed class ListingOffers
	{
		public string Asin { get { return m_asin; } }

		public DateTime PublishDateTime { get { return m_publishDateTime; } }

		public List<Offer> Offers { get { return m_offers; } }

		public ListingOffersSource ListingOffersSource { get { return m_listingOffersSource; } }

		public ListingOffers(string asin, DateTime publishDateTime, IEnumerable<Offer> offers, ListingOffersSource listingOffersSource)
		{
			m_asin = asin;

			m_publishDateTime = publishDateTime;

			m_offers = offers.ToList();

			m_listingOffersSource = listingOffersSource;
		}

		private readonly string m_asin;

		private readonly DateTime m_publishDateTime;

		private readonly List<Offer> m_offers;

		private readonly ListingOffersSource m_listingOffersSource;
	}
}
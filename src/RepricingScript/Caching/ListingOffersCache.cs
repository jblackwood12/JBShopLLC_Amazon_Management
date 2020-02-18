using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RepricingScript.Loggers;
using RepricingScript.Models;

namespace RepricingScript.Caching
{
	public sealed class ListingOffersCache
	{
		public ListingOffersCache()
		{
			m_listingOffers = new ConcurrentDictionary<string, ListingOffers>();
		}

		public static ListingOffersCache Instance
		{
			get
			{
				if (s_instance == null)
				{
					lock (s_syncRoot)
					{
						if (s_instance == null)
							s_instance = new ListingOffersCache();
					}
				}

				return s_instance;
			}
		}

		public void CacheListingOffers(ListingOffers newListingOffers)
		{
			if (newListingOffers == null || newListingOffers.Asin == null)
				return;

			ListingOffersLogger.Instance.SubmitListingOffers(newListingOffers);

			m_listingOffers.AddOrUpdate(
				newListingOffers.Asin,
				newListingOffers,
				(key, existingListingOffers) => 
						// Choose the newer ListingOffers
						(newListingOffers.PublishDateTime > existingListingOffers.PublishDateTime)
						// OR, replace the existing ListingOffers if the new ListingOffers originates from the SubscriptionService.
						|| (newListingOffers.ListingOffersSource == ListingOffersSource.SubscriptionService && existingListingOffers.ListingOffersSource == ListingOffersSource.MwsProducts)
					? newListingOffers
					: existingListingOffers);
		}

		public ListingOffers GetListingOffers(string asin)
		{
			if (asin == null)
				return null;

			ListingOffers listingOffers;
			m_listingOffers.TryGetValue(asin, out listingOffers);
			return listingOffers;
		}

		public bool HasListingOffersForAsin(string asin)
		{
			return m_listingOffers.ContainsKey(asin);
		}

		public List<ListingOffers> ListingOfferses
		{
			get { return m_listingOffers.Select(s => s.Value).ToList(); }
		}

		private static volatile ListingOffersCache s_instance;
		private static readonly object s_syncRoot = new object();

		private readonly ConcurrentDictionary<string, ListingOffers> m_listingOffers;
	}
}
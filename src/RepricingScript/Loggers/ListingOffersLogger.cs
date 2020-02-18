using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Models;
using RepricingScript.Models;
using ListingOffersLog = Data.ListingOffersLog;

namespace RepricingScript.Loggers
{
	public sealed class ListingOffersLogger
	{
		private ListingOffersLogger() { }

		public static ListingOffersLogger Instance
		{
			get
			{
				if (s_instance == null)
				{
					lock (s_syncRoot)
					{
						if (s_instance == null)
							s_instance = new ListingOffersLogger();
					}
				}

				return s_instance;
			}
		}

		public void SubmitListingOffers(ListingOffers listingOffers)
		{
			s_listingOffersLog.Add(MapToListingOffersLog(listingOffers));
		}

		public List<ListingOffersLog> ListingOffersLogs
		{
			get
			{
				List<ListingOffersLog> listingOffersLogs = new List<ListingOffersLog>();

				ListingOffersLog currentListingOffersLog;

				while (s_listingOffersLog.TryTake(out currentListingOffersLog))
					listingOffersLogs.Add(currentListingOffersLog);

				return listingOffersLogs;
			}
		}

		private static ListingOffersLog MapToListingOffersLog(ListingOffers listingOffers)
		{
			ListingOffersLog listingOffersLog = new ListingOffersLog
			{
				ASIN = listingOffers.Asin,
				PublishDateTime = listingOffers.PublishDateTime,
				ListingOffersSource = listingOffers.ListingOffersSource.ToString(),
			};

			Offer ourOffer = listingOffers.Offers.FirstOrDefault(w => w.SellerType == SellerType.JBShop);
			if (ourOffer != null)
				listingOffersLog.OurPrice = ourOffer.LandedPrice;

			Offer lowestFbaOffer = listingOffers.Offers.Where(w => w.IsFba && w.SellerType != SellerType.JBShop).OrderBy(o => o.LandedPrice).FirstOrDefault();
			if (lowestFbaOffer != null)
				listingOffersLog.LowestFbaPrice = lowestFbaOffer.LandedPrice;

			Offer lowestNonFbaOffer = listingOffers.Offers.Where(w => !w.IsFba && w.SellerType != SellerType.JBShop).OrderBy(o => o.LandedPrice).FirstOrDefault();
			if (lowestNonFbaOffer != null)
				listingOffersLog.LowestNonFbaPrice = lowestNonFbaOffer.LandedPrice;

			Offer buyBoxOffer = listingOffers.Offers.FirstOrDefault(w => w.IsBuyBoxWinner.HasValue && w.IsBuyBoxWinner.Value);
			if (buyBoxOffer != null)
			{
				listingOffersLog.BuyBoxPrice = buyBoxOffer.LandedPrice;
				listingOffersLog.BuyBoxWinnerSellerType = (int) buyBoxOffer.SellerType;
			}

			return listingOffersLog;
		}

		private static volatile ListingOffersLogger s_instance;
		private static readonly object s_syncRoot = new object();

		private static readonly BlockingCollection<ListingOffersLog> s_listingOffersLog = new BlockingCollection<ListingOffersLog>();
	}
}

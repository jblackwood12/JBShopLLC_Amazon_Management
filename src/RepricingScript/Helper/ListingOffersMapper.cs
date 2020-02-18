using System;
using System.Collections.Generic;
using System.Linq;
using AmazonProductLookup.AmazonApis.MwsApi.Subscriptions.Models;
using MarketplaceWebServiceProducts.Model;
using Models;
using RepricingScript.Models;

namespace RepricingScript.Helper
{
	public static class ListingOffersMapper
	{
		public static ListingOffers MapToListingOffers(this Notification notification)
		{
			List<Offer> offers = notification.NotificationPayload.AnyOfferChangedNotification.Offers
			.Where(w => w.SubCondition == "new")
			.Select(s =>
			{
				string sellerId = s.SellerId;
				SellerType sellerType = SellerType.Other;

				if (sellerId == c_jbShopSellerId)
					sellerType = SellerType.JBShop;
				else if (sellerId == c_amazonSellerId)
					sellerType = SellerType.Amazon;

				decimal landedPrice = s.ListingPrice.Amount + s.Shipping.Amount;

				return new Offer(sellerType, s.IsFulfilledByAmazon, landedPrice, s.IsBuyBoxWinner);
			}).ToList();

			return new ListingOffers(
				notification.NotificationPayload.AnyOfferChangedNotification.OfferChangeTrigger.ASIN,
				notification.NotificationMetaData.PublishTime,
				offers,
				ListingOffersSource.SubscriptionService);
		}

		public static ListingOffers MapToListingOffers(this KeyValuePair<string, List<LowestOfferListingType>> lowestOfferListing)
		{
			if (lowestOfferListing.Key == null || !lowestOfferListing.Value.Any())
				return null;

			string asin = lowestOfferListing.Key;

			DateTime recordDateTime = DateTime.UtcNow;

			List<Offer> offers = lowestOfferListing.Value
				.Where(w => w.Qualifiers.ItemSubcondition == "New")
				.Select(s =>
				{
					bool isFba = s.Qualifiers.FulfillmentChannel == "Amazon";

					return new Offer(isFba ? SellerType.Amazon : SellerType.Other, isFba, s.Price.LandedPrice.Amount);
				}).ToList();

			return new ListingOffers(asin, recordDateTime, offers, ListingOffersSource.MwsProducts);
		}

		private const string c_jbShopSellerId = "";
		private const string c_amazonSellerId = "";
	}
}

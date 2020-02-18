using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AmazonProductLookup.AmazonApis.MwsApi.Feeds;
using RepricingScript.Models;

namespace RepricingScript.Helper
{
	public static class CalculatePrice
	{
		public static UpdatedItemPrice CalculateNewPrice(string sku, string asin, RepricingInformation repricingInfo, ListingOffers listingOffers, decimal? myPrice)
		{
			UpdatedItemPrice updatedItemPrice = null;

			decimal minPrice = repricingInfo.MinimumPrice;

			if (listingOffers == null && myPrice.HasValue)
			{
				decimal newPrice = (myPrice.Value > minPrice) ? myPrice.Value + 0.01m : minPrice * 1.15m;

				newPrice = decimal.Round(newPrice, 2);

				updatedItemPrice = new UpdatedItemPrice(sku, newPrice, asin, minPrice, null, null, null, null, null, myPrice, null);
			}
			else if (listingOffers != null)
			{
				updatedItemPrice = CalculateNewPriceFromOffers(sku, asin, listingOffers.Offers, listingOffers.PublishDateTime, minPrice, listingOffers.ListingOffersSource);
			}

			return updatedItemPrice;
		}

		private static UpdatedItemPrice CalculateNewPriceFromOffers(string sku, string asin, List<Offer> offers, DateTime publishTime, decimal minPrice, ListingOffersSource listingOffersSource)
		{
			Offer myOffer = offers.FirstOrDefault(x => x.SellerType == SellerType.JBShop);
			Offer amazonOffer = offers.FirstOrDefault(x => x.SellerType == SellerType.Amazon);
			Offer lowestNonFbaOffer = offers.Where(x => x.SellerType == SellerType.Other && !x.IsFba).OrderBy(x => x.LandedPrice).FirstOrDefault();
			Offer lowestFbaOffer = offers.Where(x => x.SellerType == SellerType.Other && x.IsFba).OrderBy(x => x.LandedPrice).FirstOrDefault();

			decimal? competeWithAmazon = CompeteWithAmazon(amazonOffer);
			decimal? competeWithOtherFbaMerchant = CompeteWithFbaMerchant(lowestFbaOffer, minPrice);

			decimal? competeWithAllFbaSellers = ReturnMinNullableDecimal(new List<decimal?> { competeWithAmazon, competeWithOtherFbaMerchant });
			decimal? competeWithNonFbaSellers = CompeteWithNonFbaMerchant(lowestNonFbaOffer);

			decimal? newPrice;
			if (competeWithAllFbaSellers.HasValue && competeWithNonFbaSellers.HasValue)
			{
				// We are competing against fba and non-fba sellers
				decimal fbaMargin = CalculateProfitMarginFromSellingPrice(competeWithAllFbaSellers.Value, minPrice);
				decimal nonFbaMargin = CalculateProfitMarginFromSellingPrice(competeWithNonFbaSellers.Value, minPrice);

				if (nonFbaMargin < c_minMargin && fbaMargin > c_lowerMargin) // lets compete against the fba seller - the non-fba seller is too low for us and we are happy with the margin that the fba seller is at.
					newPrice = CalculateNewPriceAgainstFbaSeller(competeWithAllFbaSellers.Value, minPrice);
				else if (competeWithAllFbaSellers.Value < competeWithNonFbaSellers.Value) // lets compete against the fba sellers, since they are more competitive
					newPrice = CalculateNewPriceAgainstFbaSeller(competeWithAllFbaSellers.Value, minPrice);
				else // lets compete with the non-fba sellers, since they are more competitive
					newPrice = CalculateNewPriceAgainstNonFbaSeller(competeWithNonFbaSellers.Value, minPrice);
			}
			else if (competeWithAllFbaSellers.HasValue)
			{
				// We are competing against just fba sellers
				newPrice = CalculateNewPriceAgainstFbaSeller(competeWithAllFbaSellers.Value, minPrice);
			}
			else if (competeWithNonFbaSellers.HasValue)
			{
				// We are competing against just non-fba sellers
				newPrice = CalculateNewPriceAgainstNonFbaSeller(competeWithNonFbaSellers.Value, minPrice);
			}
			else
			{
				// We are competing against nobody
				newPrice = CalculateSellingPriceFromProfitMargin(c_higherMargin, minPrice);
			}

			// There is no need to send a new price out if it matches our offer.
			if (myOffer != null && newPrice.HasValue && ArePricesTheSameWithinThreshold(myOffer.LandedPrice, newPrice.Value))
				return null;

			if (newPrice.HasValue)
				newPrice = decimal.Round(newPrice.Value, 2);

			decimal? myOfferPrice = myOffer != null ? myOffer.LandedPrice : (decimal?)null;
			decimal? amazonOfferPrice = amazonOffer != null ? amazonOffer.LandedPrice : (decimal?)null;
			decimal? lowestNonFbaOfferPrice = lowestNonFbaOffer != null ? lowestNonFbaOffer.LandedPrice : (decimal?)null;
			decimal? lowestFbaOfferPrice = lowestFbaOffer != null ? lowestFbaOffer.LandedPrice : (decimal?)null;

			if (newPrice.HasValue)
				return new UpdatedItemPrice(sku, newPrice.Value, asin, minPrice, myOfferPrice, amazonOfferPrice, lowestFbaOfferPrice, lowestNonFbaOfferPrice, publishTime, null, listingOffersSource.ToString());

			return null;
		}

		private static decimal? CalculateNewPriceAgainstFbaSeller(decimal competeWithFbaSellerPrice, decimal minPrice)
		{
			decimal? newPrice;

			decimal profitMargin = CalculateProfitMarginFromSellingPrice(competeWithFbaSellerPrice, minPrice);
			if (profitMargin < c_minMargin)
			{
				newPrice = CalculateSellingPriceFromProfitMargin(c_lowerMargin, minPrice);
			}
			else
			{
				if (ShouldCompete(profitMargin))
					newPrice = competeWithFbaSellerPrice;
				else
					newPrice = 1.15m * competeWithFbaSellerPrice;
			}

			return newPrice;
		}

		private static decimal? CalculateNewPriceAgainstNonFbaSeller(decimal competewithNonfbaSellerPrice, decimal minPrice)
		{
			decimal? newPrice;

			decimal profitMargin = CalculateProfitMarginFromSellingPrice(competewithNonfbaSellerPrice, minPrice);
			if (profitMargin < c_minMargin)
			{
				newPrice = CalculateSellingPriceFromProfitMargin(c_lowerMargin, minPrice);
			}
			else if (profitMargin < c_maxMargin)
			{
				if (ShouldCompete(profitMargin))
					newPrice = competewithNonfbaSellerPrice;
				else
					newPrice = 1.15m * competewithNonfbaSellerPrice;
			}
			else
			{
				newPrice = CalculateSellingPriceFromProfitMargin(c_higherMargin, minPrice);
			}

			return newPrice;
		}

		private static bool ArePricesTheSameWithinThreshold(decimal value1, decimal value2)
		{
			return Math.Abs(value1 - value2) < c_arePricesTheSameThresholdCents;
		}

		// This method might seem dumb, but I'm adding a layer of protection
		private static decimal? ReturnMinNullableDecimal(IEnumerable<decimal?> values)
		{
			List<decimal> nonNullValues = values
				.Where(x => x.HasValue)
				.Select(x => x != null ? x.Value : 0)
				.ToList();

			if (!nonNullValues.Any())
				return null;

			decimal minValue = nonNullValues.Min();

			if (minValue <= 0)
				return null;

			return minValue;
		}

		private static decimal? CompeteWithAmazon(Offer amazonOffer)
		{
			if (amazonOffer == null)
				return null;

			return ((1 - .012m) * amazonOffer.LandedPrice) - 0.15m;
		}

		private static decimal? CompeteWithNonFbaMerchant(Offer nonFbaOffer)
		{
			if (nonFbaOffer == null)
				return null;

			return (nonFbaOffer.LandedPrice * (1 + 0.01m)) + 0.10m;
		}

		private static decimal? CompeteWithFbaMerchant(Offer fbaOffer, decimal breakEvenPrice)
		{
			decimal? competeWithFbaMerchantPrice = null;

			if (fbaOffer != null)
			{
				decimal fbaMerchantPrice = fbaOffer.LandedPrice;
				decimal profitMargin = CalculateProfitMarginFromSellingPrice(fbaMerchantPrice, breakEvenPrice);

				if (profitMargin < c_minMargin)
					competeWithFbaMerchantPrice = (fbaMerchantPrice * (1 + 0.005m)) + 0.05m;
				else if (profitMargin < c_lowerMargin)
					competeWithFbaMerchantPrice = fbaMerchantPrice + 0.01m;
				else if (profitMargin < c_higherMargin)
					competeWithFbaMerchantPrice = (fbaMerchantPrice * (1 - 0.005m)) - 0.02m;
				else if (profitMargin < c_maxMargin)
					competeWithFbaMerchantPrice = (fbaMerchantPrice * (1 - 0.01m)) - 0.03m;
				else if (profitMargin >= c_maxMargin)
					competeWithFbaMerchantPrice = (fbaMerchantPrice * (1 - 0.02m)) - 0.05m;
			}

			return competeWithFbaMerchantPrice;
		}

		private static bool ShouldCompete(decimal profitMargin)
		{
			decimal tryCompeteProbability;

			if (profitMargin < c_minMargin) // 0% chance of competing
				tryCompeteProbability = 0m;
			else if (profitMargin < c_lowerMargin) // 0-75% chance of competing
				tryCompeteProbability = 0.75m * (profitMargin - c_minMargin) / (c_lowerMargin - c_minMargin);
			else if (profitMargin < c_higherMargin) // 75-85% chance of competing
				tryCompeteProbability = ((0.85m - 0.75m) * (profitMargin - c_lowerMargin) / (c_higherMargin - c_lowerMargin)) + 0.75m;
			else if (profitMargin < c_maxMargin) // 85-100% chance of competing
				tryCompeteProbability = ((1.00m - 0.85m) * (profitMargin - c_higherMargin) / (c_maxMargin - c_higherMargin)) + 0.85m;
			else // Always compete
				tryCompeteProbability = 1m;

			Random rnd = new Random();
			return rnd.NextDouble() < (double)tryCompeteProbability;
		}

		private static decimal CalculateProfitMarginFromSellingPrice(decimal sellingPriceIn, decimal breakEvenPrice)
		{
			decimal profit = ((sellingPriceIn - breakEvenPrice) * (1 - c_pctFee)) - ((c_assumedReturnPct + c_waStateRevTax) * sellingPriceIn);
			return profit / sellingPriceIn;
		}

		private static decimal CalculateSellingPriceFromProfitMargin(decimal profitMarginIn, decimal breakEvenPrice)
		{
			return (breakEvenPrice * (1 - c_pctFee)) / (1 - c_pctFee - c_waStateRevTax - c_assumedReturnPct - profitMarginIn);
		}

		private const decimal c_assumedReturnPct = 0.035m; // Cost of returned/damaged items..
		private const decimal c_waStateRevTax = 0.005m; // Rev tax
		private const decimal c_pctFee = 0.15m; // Home and kitchen category fee.
		private const decimal c_minMargin = 0.005m;
		private const decimal c_lowerMargin = 0.09m; // We will try to get a profit of atleast this much
		private const decimal c_higherMargin = 0.195m; // We don't really need a profit of more than this much
		private const decimal c_maxMargin = 0.30m; // We won't try to get a margin over this..
		private const decimal c_arePricesTheSameThresholdCents = 0.02m;
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AmazonProductLookup.AmazonApis.MwsApi.Feeds;
using MoreLinq;
using System.Threading.Tasks;
using AmazonProductLookup.AmazonApis.MwsApi.Subscriptions;
using Data;
using RepricingScript.Caching;
using Models;
using FBAInventoryServiceMWS.Model;
using AmazonProductLookup.AmazonApis.MwsApi.FulfillmentInventory;
using System.Threading;
using AmazonProductLookup.AmazonApis.MwsApi.Products;
using RepricingScript.Helper;
using RepricingScript.Loggers;
using RepricingScript.Models;
using RepricingInformation = Models.RepricingInformation;

namespace RepricingScript
{
	public class RepricingScript
	{
		public RepricingScript(IMwsProductsApi mwsProductsApi, IMwsFeedsApi mwsFeedsApi, IMwsSubscriptionServiceApi mwsSubscriptionServiceApi, IAmazonMWSdbService amazonMwSdbService, IMwsFulfillmentInventoryApi mwsFullfillmentInventoryApi)
		{
			m_mwsProductsApi = mwsProductsApi;
			m_mwsFeedsApi = mwsFeedsApi;
			m_mwsSubscriptionServiceApi = mwsSubscriptionServiceApi;
			m_amazonMwSdbService = amazonMwSdbService;
			m_mwsFullfillmentInventoryApi = mwsFullfillmentInventoryApi;
		}

		public void Run()
		{
			Task.Factory.StartNew(() => LogListingOffersLog());

			ExceptionLogger.Instance.LogMessage("Populating the price history cache");
			try
			{
				Dictionary<string, Data.PriceHistory> priceHistories = m_amazonMwSdbService.GetLatestPriceHistoryForToday();
				PriceHistoryCache.Instance.ReplaceCache(priceHistories);
			}
			catch (Exception ex)
			{
				ExceptionLogger.Instance.LogException(ex);
			}

			ExceptionLogger.Instance.LogMessage("Querying and replacing the repricing information cache...");
			QueryAndCacheRepricingInformation(false);
			Task.Factory.StartNew(() => QueryAndCacheRepricingInformation());

			ExceptionLogger.Instance.LogMessage("Instantiating threads to consume and cache notifications...");
			for (int i = 0; i < c_notificationConsumerThreads; i++)
				Task.Factory.StartNew(ConsumeAndCacheNotifications);

			ExceptionLogger.Instance.LogMessage("Querying and replacing the inventory supply cache...");
			QueryAndCacheInventorySupply(false);
			Task.Factory.StartNew(() => QueryAndCacheInventorySupply());

			while (true)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				CalculateAndSendNewPrice();
				stopwatch.Stop();

				Thread.Sleep((int)Math.Max(0, (c_calculateAndSendNewPricesIntervalSeconds * 1000) - stopwatch.ElapsedMilliseconds));
			}
		}

		private void CalculateAndSendNewPrice()
		{
			IReadOnlyCollection<RepricingInformation> repricingInformations = RepricingInformationCache.Instance.RepricingInformations;

			List<RepricingInformation> inStockRepricingInformations = repricingInformations
				.Where(x => InventorySupplyCache.Instance.InStockSupplyQuantity(x.SKU) > 0)
				.ToList();

			List<RepricingInformation> inStockRepricingInformationsWithoutListingOffers = inStockRepricingInformations
				.Where(x => !ListingOffersCache.Instance.HasListingOffersForAsin(x.ASIN))
				.ToList();

			Dictionary<string, decimal> myPricesForInStockItemsWithoutListingOffers = GetMyPrices(inStockRepricingInformationsWithoutListingOffers.Select(x => x.SKU));

			List<UpdatedItemPrice> newPrices = new List<UpdatedItemPrice>();
			foreach (RepricingInformation repricingInfo in inStockRepricingInformations)
			{
				string asin = repricingInfo.ASIN;
				string sku = repricingInfo.SKU;

				ListingOffers listingOffers = ListingOffersCache.Instance.GetListingOffers(asin);

				decimal? myPrice = null;

				if (myPricesForInStockItemsWithoutListingOffers.ContainsKey(sku))
					myPrice = myPricesForInStockItemsWithoutListingOffers[sku];

				UpdatedItemPrice updatedItemPrice = CalculatePrice.CalculateNewPrice(sku, asin, repricingInfo, listingOffers, myPrice);
				if (updatedItemPrice != null) // If we can't set a price, this object will be null.
				{
					decimal newPrice = updatedItemPrice.UpdatedPrice;
					decimal? lastPrice = PriceHistoryCache.Instance.GetLastPrice(asin);

					if ((newPrice > repricingInfo.MinimumPrice) && (newPrice > 4.00m) && //// This new price has to be greater than our minimum price and greater than $4.00
						(!lastPrice.HasValue || lastPrice.Value != newPrice)) //// This new price has to be not equal to our last updated price.
					{
						newPrices.Add(updatedItemPrice);
					}
				}
			}

			UploadNewPrices(newPrices);
		}

		private void UploadNewPrices(List<UpdatedItemPrice> newPrices)
		{
			DateTime priceDateTime = DateTime.UtcNow;

			List<Data.PriceHistory> priceHistories = newPrices
				.Select(x => new Data.PriceHistory
				{
					NewPrice = x.UpdatedPrice,
					TimeStamp = priceDateTime,
					ASIN = x.Asin,
					BreakEvenPrice = x.BreakEvenPrice,
					MyOfferPriceInNotification = x.MyOfferPriceInNotification,
					AmazonsOfferPriceInNotification = x.AmazonsOfferPriceInNotification,
					LowestFbaOfferPriceInNotification = x.LowestFbaOfferPriceInNotification,
					LowestNonFbaOfferPriceInNotification = x.LowestNonFbaOfferPriceInNotification,
					LastNotificationPublishDateTime = x.LastNotificationPublishDateTime,
					MyPriceFromProductsApi = x.MyPriceFromProductsApi,
					ListingOffersSource = x.ListingOffersSource
				}).ToList();

			try
			{
				m_mwsFeedsApi.UpdateOurPrices(newPrices);
				m_amazonMwSdbService.InsertPriceHistory(priceHistories);
				priceHistories.ForEach(x => PriceHistoryCache.Instance.CachePriceHistory(x));
			}
			catch (Exception ex)
			{
				ExceptionLogger.Instance.LogException(ex);
				ExceptionLogger.Instance.LogMessage("Did not successfully send new prices to Amazon");
			}
		}

		private void ConsumeAndCacheNotifications()
		{
			while (true)
			{
				List<ListingOffers> listingOfferses = new List<ListingOffers>();
				try
				{
					listingOfferses = m_mwsSubscriptionServiceApi.ConsumeNotifications(10)
						.Select(s => s.MapToListingOffers())
						.ToList();
				}
				catch (Exception ex)
				{
					ExceptionLogger.Instance.LogException(ex);
					Thread.Sleep(90 * 1000);
				}

				foreach (ListingOffers listingOffers in listingOfferses)
				{
					try
					{
						ListingOffersCache.Instance.CacheListingOffers(listingOffers);
					}
					catch (Exception ex)
					{
						ExceptionLogger.Instance.LogException(ex);
					}
				}

				if (listingOfferses.Count == 0)
					Thread.Sleep(5 * 1000);
			}
		}

		private void LogListingOffersLog(bool runInfinite = true)
		{
			do
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				try
				{
					List<ListingOffersLog> listingOffersLogs = ListingOffersLogger.Instance.ListingOffersLogs;

					if (listingOffersLogs.Any())
						m_amazonMwSdbService.InsertListingOffersLog(listingOffersLogs);
				}
				catch (Exception ex)
				{
					ExceptionLogger.Instance.LogException(ex);
				}

				stopwatch.Stop();

				if (runInfinite)
					Thread.Sleep((int)Math.Max(0, (c_intervalSecondsForLoggingToTheDatabase * 1000) - stopwatch.ElapsedMilliseconds));

			} while (runInfinite);
		}

		private void QueryAndCacheRepricingInformation(bool runInfinite = true)
		{
			do
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				try
				{
					List<RepricingInformation> repricingInformations = m_amazonMwSdbService.GetAllRepricingInformations();

					// Take the existing RepricingInformations, if there are new entries, we should populate the ListingOffersCache.
					// We do this because a Notification may take a while to be triggered, or never happen at all.
					Dictionary<string, RepricingInformation> existingRepricingInformations = RepricingInformationCache.Instance.RepricingInformations.ToDictionary(k => k.ASIN, v => v);

					// This will contain both the new RepricingInformations found, or ones that are too old, found in the cache.
					List<string> asinsToLookupAndRecache = new List<string>();

					List<RepricingInformation> newRepricingInformations = repricingInformations
						.Where(w => !existingRepricingInformations.ContainsKey(w.ASIN)).ToList();

					asinsToLookupAndRecache.AddRange(newRepricingInformations.Select(s => s.ASIN));

					DateTime now = DateTime.UtcNow;

					// These ListingOffers are 2 or more hours old.
					// This is from us not receiving a Notification from the Subscription Service in quite some time.
					// To receive new data, lets request it from the MWS Products API.
					List<ListingOffers> oldListingOffers = ListingOffersCache.Instance.ListingOfferses
						.Where(w => now.Subtract(w.PublishDateTime).Hours >= c_numHoursStaleListingOffers)
						.ToList();

					asinsToLookupAndRecache.AddRange(oldListingOffers.Select(s => s.Asin));

					if (asinsToLookupAndRecache.Any())
					{
						List<ListingOffers> listingOfferses = m_mwsProductsApi.GetLowestOfferListingsForASINs(asinsToLookupAndRecache)
							.Select(s => s.MapToListingOffers())
							.Where(w => w != null)
							.ToList();

						listingOfferses.ForEach(f => ListingOffersCache.Instance.CacheListingOffers(f));
					}

					RepricingInformationCache.Instance.ReplaceCache(repricingInformations);
				}
				catch (Exception ex)
				{
					ExceptionLogger.Instance.LogException(ex);
				}

				stopwatch.Stop();

				if (runInfinite)
					Thread.Sleep((int)Math.Max(0, (c_repricingInformationCacheIntervalSeconds * 1000) - stopwatch.ElapsedMilliseconds));
			}
			while (runInfinite);
		}

		private void QueryAndCacheInventorySupply(bool runInfinite = true)
		{
			do
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				try
				{
					List<string> skus = RepricingInformationCache.Instance.RepricingInformations.Select(x => x.SKU).Distinct().ToList();
					List<InventorySupply> inventorySupply = GetInventorySupply(skus);
					InventorySupplyCache.Instance.ReplaceCache(inventorySupply.GroupBy(x => x.SellerSKU).ToDictionary(x => x.Key, y => y.FirstOrDefault()));
				}
				catch (Exception ex)
				{
					ExceptionLogger.Instance.LogException(ex);
				}

				stopwatch.Stop();

				if (runInfinite)
					Thread.Sleep((int)Math.Max(0, (c_inventorySupplyCacheRefreshIntervalMinutes * 60 * 1000) - stopwatch.ElapsedMilliseconds));
			}
			while (runInfinite);
		}

		private Dictionary<string, decimal> GetMyPrices(IEnumerable<string> skus)
		{
			Dictionary<string, decimal> myPrices = new Dictionary<string, decimal>();

			if (skus == null)
				return myPrices;

			foreach (IEnumerable<string> skuBatch in skus.Distinct().Batch(c_myPricesBatchSize))
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				try
				{
					Dictionary<string, decimal> myPricesBatch = m_mwsProductsApi.GetMyPriceForSKUs(skuBatch);
					
					foreach (var pricePair in myPricesBatch)
						myPrices.Add(pricePair.Key, pricePair.Value);
				}
				catch (Exception ex)
				{
					ExceptionLogger.Instance.LogException(ex);
				}

				stopwatch.Stop();
				Thread.Sleep((int)Math.Max(0, (c_getMyPricesQueryIntervalSeconds * 1000) - stopwatch.ElapsedMilliseconds));
			}

			return myPrices;
		}

		private List<InventorySupply> GetInventorySupply(IEnumerable<string> skus)
		{
			List<InventorySupply> inventorySupply = new List<InventorySupply>();

			if (skus == null)
				return inventorySupply;

			foreach (IEnumerable<string> skuBatch in skus.Batch(c_inventorySupplyBatchSize))
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				try
				{
					inventorySupply.AddRange(m_mwsFullfillmentInventoryApi.GetInventorySupply(skuBatch.ToList()));
				}
				catch (Exception ex)
				{
					ExceptionLogger.Instance.LogException(ex);
				}

				stopwatch.Stop();
				Thread.Sleep((int)Math.Max(0, (c_fulfillmentInventoryApiQueryIntervalSeconds * 1000) - stopwatch.ElapsedMilliseconds));
			}

			return inventorySupply;
		}

		private readonly IMwsProductsApi m_mwsProductsApi;
		private readonly IMwsFeedsApi m_mwsFeedsApi;
		private readonly IMwsSubscriptionServiceApi m_mwsSubscriptionServiceApi;
		private readonly IAmazonMWSdbService m_amazonMwSdbService;
		private readonly IMwsFulfillmentInventoryApi m_mwsFullfillmentInventoryApi;

		private const int c_notificationConsumerThreads = 50;
		private const int c_repricingInformationCacheIntervalSeconds = 100;
		private const int c_intervalSecondsForLoggingToTheDatabase = 100;
		private const int c_inventorySupplyBatchSize = 20;
		private const int c_myPricesBatchSize = 20;
		private const int c_fulfillmentInventoryApiQueryIntervalSeconds = 2;
		private const int c_getMyPricesQueryIntervalSeconds = 2;
		private const int c_inventorySupplyCacheRefreshIntervalMinutes = 60;
		private const int c_calculateAndSendNewPricesIntervalSeconds = 125;

		private const int c_numHoursStaleListingOffers = 2;
	}
}

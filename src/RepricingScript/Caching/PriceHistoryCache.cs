using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using PriceHistory = Data.PriceHistory;

namespace RepricingScript.Caching
{
	public sealed class PriceHistoryCache
	{
		public PriceHistoryCache()
		{
			m_priceHistories = new ConcurrentDictionary<string, PriceHistory>();
		}

		public static PriceHistoryCache Instance
		{
			get
			{
				if (s_instance == null)
				{
					lock (s_syncRoot)
					{
						if (s_instance == null)
							s_instance = new PriceHistoryCache();
					}
				}

				return s_instance;
			}
		}

		public void ReplaceCache(IDictionary<string, PriceHistory> newPriceHistories)
		{
			Interlocked.Exchange(ref m_priceHistories, new ConcurrentDictionary<string, PriceHistory>(newPriceHistories));
		}

		public void CachePriceHistory(PriceHistory priceHistory)
		{
			if (priceHistory == null || priceHistory.ASIN == null)
				return;

			m_priceHistories.AddOrUpdate(priceHistory.ASIN, priceHistory, (key, existingPriceHistory) => priceHistory);
		}

		public PriceHistory GetPriceHistory(string asin)
		{
			if (asin == null)
				return null;

			PriceHistory priceHistory;
			m_priceHistories.TryGetValue(asin, out priceHistory);
			return priceHistory;
		}

		public decimal? GetLastPrice(string asin)
		{
			PriceHistory priceHistory = GetPriceHistory(asin);

			if (priceHistory == null)
				return null;

			return priceHistory.NewPrice;
		}

		private static volatile PriceHistoryCache s_instance;
		private static readonly object s_syncRoot = new object();

		// Key is the asin
		private ConcurrentDictionary<string, PriceHistory> m_priceHistories;
	}
}

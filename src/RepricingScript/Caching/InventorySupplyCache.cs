using System.Collections.Concurrent;
using System.Collections.Generic;
using FBAInventoryServiceMWS.Model;
using System.Threading;

namespace RepricingScript.Caching
{
	public sealed class InventorySupplyCache
	{
		public InventorySupplyCache()
		{
			m_inventorySupplies = new ConcurrentDictionary<string, InventorySupply>();
		}

		public static InventorySupplyCache Instance
		{
			get
			{
				if (s_instance == null)
				{
					lock (s_syncRoot)
					{
						if (s_instance == null)
							s_instance = new InventorySupplyCache();
					}
				}

				return s_instance;
			}
		}

		public void ReplaceCache(IDictionary<string, InventorySupply> newInventorySupply)
		{
			Interlocked.Exchange(ref m_inventorySupplies, new ConcurrentDictionary<string, InventorySupply>(newInventorySupply));
		}

		public decimal InStockSupplyQuantity(string sku)
		{
			InventorySupply inventorySupply = GetInventorySupply(sku);

			if (inventorySupply == null)
				return 0;

			return inventorySupply.InStockSupplyQuantity;
		}

		private InventorySupply GetInventorySupply(string sku)
		{
			if (sku == null)
				return null;

			InventorySupply inventorySupply;
			m_inventorySupplies.TryGetValue(sku, out inventorySupply);
			return inventorySupply;
		}

		private static volatile InventorySupplyCache s_instance;
		private static readonly object s_syncRoot = new object();

		// Key is the SKU
		private ConcurrentDictionary<string, InventorySupply> m_inventorySupplies;
	}
}
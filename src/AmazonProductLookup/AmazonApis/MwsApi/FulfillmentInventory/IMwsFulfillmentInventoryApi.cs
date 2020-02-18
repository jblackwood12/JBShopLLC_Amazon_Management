using System.Collections.Generic;
using FBAInventoryServiceMWS.Model;

namespace AmazonProductLookup.AmazonApis.MwsApi.FulfillmentInventory
{
	public interface IMwsFulfillmentInventoryApi
	{
		List<InventorySupply> GetInventorySupply(List<string> skus);
	}
}

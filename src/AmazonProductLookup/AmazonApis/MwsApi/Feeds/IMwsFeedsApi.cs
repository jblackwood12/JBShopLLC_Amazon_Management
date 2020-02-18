using System.Collections.Generic;

namespace AmazonProductLookup.AmazonApis.MwsApi.Feeds
{
	public interface IMwsFeedsApi
	{
		void UpdateOurPrices(IEnumerable<UpdatedItemPrice> newPrices);
	}
}

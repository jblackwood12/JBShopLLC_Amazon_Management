using Models;

namespace AmazonProductLookup.AmazonApis.AdvApi
{
	public interface IProductAdvertisingApi
	{
		LookupProductResponse LookupProduct(LookupProductRequest lookupProductRequest);

		LookupAmazonListingResponse LookupAmazonListings(LookupAmazonListingRequest lookupAmazonListingRequest);
	}
}

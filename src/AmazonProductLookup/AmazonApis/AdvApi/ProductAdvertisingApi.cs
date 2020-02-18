using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using AmazonProductLookup.AdvApi;
using AmazonProductLookup.AmazonApis.AdvApi.ItemLookup;
using AmazonProductLookup.AmazonApis.AdvApi.ItemSearch;
using Models;
using Models.AdvApi;
using MoreLinq;

namespace AmazonProductLookup.AmazonApis.AdvApi
{
	public sealed class ProductAdvertisingApi : IProductAdvertisingApi
	{
		public ProductAdvertisingApi(string accessKeyId, string secretKey, string destination, string namespaceWebServices, string associateTag)
		{
			m_endPoint = destination;
			m_akid = accessKeyId;
			m_namespaceWebServices = namespaceWebServices;
			byte[] secret = Encoding.UTF8.GetBytes(secretKey);
			m_signer = new HMACSHA256(secret);
			m_associateTag = associateTag;
		}

		public LookupProductResponse LookupProduct(LookupProductRequest lookupProductRequest)
		{
			LookupProductResponse lookProductResponse = null;
			ProductAdvApiResponse response;

			Dictionary<string, string> optionalParameters = new Dictionary<string, string>();

			if (lookupProductRequest.IdType == IdType.ItemNumber)
			{
				optionalParameters["Keywords"] = lookupProductRequest.ItemId;
				optionalParameters["SearchIndex"] = lookupProductRequest.SearchIndex.ToString();

				if (lookupProductRequest.ParameterType.HasValue && lookupProductRequest.ParameterString != null)
					optionalParameters[lookupProductRequest.ParameterType.Value.ToString()] = lookupProductRequest.ParameterString;

				optionalParameters["Operation"] = Operation.ItemSearch.ToString();
				optionalParameters["SearchIndex"] = lookupProductRequest.SearchIndex.ToString();
				optionalParameters["ResponseGroup"] = lookupProductRequest.ResponseGroup.ToString();

				response = SignAndIssueRequest(optionalParameters);

				if (!response.Errored)
				{
					Product product = ItemSearchUtility.ReadItemSearchResponse(response.Nodes, lookupProductRequest.ItemId);
					lookProductResponse = new LookupProductResponse(product, ProductSearchMethod.ItemNumber);
				}
			}
			else if (lookupProductRequest.IdType == IdType.ASIN)
			{
				optionalParameters["ItemId"] = lookupProductRequest.ItemId;
				optionalParameters["IdType"] = lookupProductRequest.IdType.ToString();
				optionalParameters["Operation"] = Operation.ItemLookup.ToString();
				optionalParameters["ResponseGroup"] = lookupProductRequest.ResponseGroup.ToString();

				response = SignAndIssueRequest(optionalParameters);

				if (!response.Errored)
				{
					ProductAndProductMetadata productAndProductMetadata = ItemLookupUtility.ReadAsinLookupResponse(response.Nodes);
					lookProductResponse = new LookupProductResponse(productAndProductMetadata.Product, ProductSearchMethod.ASIN, productAndProductMetadata.ProductMetadata);
				}
			}
			else
			{
				optionalParameters["ItemId"] = lookupProductRequest.ItemId;
				optionalParameters["IdType"] = lookupProductRequest.IdType.ToString();
				optionalParameters["Operation"] = Operation.ItemLookup.ToString();
				optionalParameters["SearchIndex"] = lookupProductRequest.SearchIndex.ToString();
				optionalParameters["ResponseGroup"] = ResponseGroup.Medium.ToString();

				response = SignAndIssueRequest(optionalParameters);

				if (response != null && !response.Errored)
				{
					ProductAndProductMetadata productAndProductMetadata = ItemLookupUtility.ReadAsinLookupResponse(response.Nodes);
					lookProductResponse = new LookupProductResponse(productAndProductMetadata.Product, ProductSearchMethod.UPC, productAndProductMetadata.ProductMetadata);
				}
			}

			return lookProductResponse;
		}

		public LookupAmazonListingResponse LookupAmazonListings(LookupAmazonListingRequest lookupAmazonListingRequest)
		{
			Dictionary<string, decimal> amazonListings = new Dictionary<string, decimal>();

			foreach (IEnumerable<string> batchedAsins in lookupAmazonListingRequest.ASINs.Batch(c_asinBatchSize))
			{
				Dictionary<string, string> optionalParameters = new Dictionary<string, string>();

				optionalParameters["ItemId"] = string.Join(",", batchedAsins);
				optionalParameters["IdType"] = IdType.ASIN.ToString();
				optionalParameters["Operation"] = Operation.ItemLookup.ToString();
				optionalParameters["ResponseGroup"] = ResponseGroup.OfferFull.ToString();
				optionalParameters["Condition"] = "New";
				optionalParameters["MerchantId"] = "Amazon";

				ProductAdvApiResponse response = SignAndIssueRequest(optionalParameters);

				Dictionary<string, decimal> batchedAmazonListings = ItemLookupUtility.ReadAmazonListingResponse(response.Nodes);

				foreach (KeyValuePair<string, decimal> amazonListing in batchedAmazonListings)
				{
					if (!amazonListings.ContainsKey(amazonListing.Key))
						amazonListings.Add(amazonListing.Key, amazonListing.Value);
				}
			}

			return new LookupAmazonListingResponse
			{
				AmazonListings = amazonListings
			};
		}

		private ProductAdvApiResponse SignAndIssueRequest(Dictionary<string, string> optionalParameters)
		{
			string requestUrl = SigningUtility.Sign(optionalParameters, m_akid, m_endPoint, m_signer, m_associateTag);

			return ResponseUtility.IssueRequest(requestUrl, m_namespaceWebServices);
		}

		private const int c_asinBatchSize = 10;

		private readonly string m_endPoint;
		private readonly string m_akid;
		private readonly HMAC m_signer;
		private readonly string m_associateTag;
		private readonly string m_namespaceWebServices;
	}
}

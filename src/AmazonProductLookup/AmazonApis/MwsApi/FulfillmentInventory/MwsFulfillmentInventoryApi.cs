using System;
using System.Collections.Generic;
using FBAInventoryServiceMWS;
using FBAInventoryServiceMWS.Model;

namespace AmazonProductLookup.AmazonApis.MwsApi.FulfillmentInventory
{
	public sealed class MwsFulfillmentInventoryApi : IMwsFulfillmentInventoryApi
	{
		public MwsFulfillmentInventoryApi(string sellerId, string marketPlaceId, string accessKeyId, string secretAccessKeyId)
		{
			m_sellerId = sellerId;
			m_marketPlaceId = marketPlaceId;

			FBAInventoryServiceMWSConfig configService = new FBAInventoryServiceMWSConfig().WithServiceURL(c_fbaInventoryServiceUrl);
			configService.SetUserAgentHeader(string.Empty, string.Empty, "C#");

			m_service = new FBAInventoryServiceMWSClient(
					accessKeyId,
					secretAccessKeyId,
					string.Empty,
					string.Empty,
					configService);
		}

		public List<InventorySupply> GetInventorySupply(List<string> skus)
		{
			try
			{
				ListInventorySupplyRequest request = new ListInventorySupplyRequest();
				request.WithSellerSkus(new SellerSkuList().Withmember(skus.ToArray()));
				request.SellerId = m_sellerId;
				request.Marketplace = m_marketPlaceId;

				ListInventorySupplyResponse response = m_service.ListInventorySupply(request);
				if (response.IsSetListInventorySupplyResult())
				{
					ListInventorySupplyResult listInventorySupplyResult = response.ListInventorySupplyResult;
					if (listInventorySupplyResult.IsSetInventorySupplyList())
					{
						InventorySupplyList inventorySupplyList = listInventorySupplyResult.InventorySupplyList;
						List<InventorySupply> memberList = inventorySupplyList.member;
						return memberList;
					}
				}
			}
			catch (FBAInventoryServiceMWSException ex)
			{
				Console.WriteLine("Caught Exception: " + ex.Message);
				Console.WriteLine("Response Status Code: " + ex.StatusCode);
				Console.WriteLine("Error Code: " + ex.ErrorCode);
				Console.WriteLine("Error Type: " + ex.ErrorType);
				Console.WriteLine("Request ID: " + ex.RequestId);
				Console.WriteLine("XML: " + ex.XML);
			}

			return new List<InventorySupply>();
		}

		private readonly string m_sellerId;
		private readonly string m_marketPlaceId;
		private const string c_fbaInventoryServiceUrl = "https://mws.amazonservices.com/FulfillmentInventory/2010-10-01/";
		private readonly FBAInventoryServiceMWS.FBAInventoryServiceMWS m_service;
	}
}

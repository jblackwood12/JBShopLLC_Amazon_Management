using System;
using System.Collections.Generic;
using System.Linq;
using FBAInboundServiceMWS;
using FBAInboundServiceMWS.Model;
using Models;
using Models.MwsInboundShipmentApi;

namespace AmazonProductLookup.AmazonApis.MwsApi.InboundShipment
{
	public class MwsInboundShipmentApi : IMwsInboundShipmentApi
	{
		public MwsInboundShipmentApi(string sellerId, string marketPlaceId, string accessKeyId, string secretAccessKeyId, string serviceUrl)
		{
			m_sellerId = sellerId;
			m_marketPlaceId = marketPlaceId;

			FBAInboundServiceMWSConfig fbaInboundServiceConfig = new FBAInboundServiceMWSConfig()
				.WithServiceURL(serviceUrl);

			fbaInboundServiceConfig.SetUserAgentHeader(string.Empty, string.Empty, "C#");

			m_fbaInboundServiceMwsClient = new FBAInboundServiceMWSClient(accessKeyId, secretAccessKeyId, fbaInboundServiceConfig);
		}

		public List<InboundShipmentContainer> GetInboundShipments(DateTime? lastUpdatedAfter = null, DateTime? lastUpdatedBefore = null, string shipmentId = null)
		{
			if (shipmentId != null && (lastUpdatedAfter.HasValue && lastUpdatedBefore.HasValue))
				throw new InvalidOperationException("Can only set either shipmentId, or both lastUpdatedAfter and lastUpdatedBefore");

			ListInboundShipmentItemsRequest listInboundShipmentItemsRequest = new ListInboundShipmentItemsRequest
			{
				Marketplace = m_marketPlaceId,
				SellerId = m_sellerId
			};

			if (shipmentId != null)
				listInboundShipmentItemsRequest.ShipmentId = shipmentId;

			if (lastUpdatedAfter.HasValue && lastUpdatedBefore.HasValue)
			{
				listInboundShipmentItemsRequest.LastUpdatedAfter = lastUpdatedAfter.Value;
				listInboundShipmentItemsRequest.LastUpdatedBefore = lastUpdatedBefore.Value;
			}

			ListInboundShipmentItemsResponse listInboundShipmentItemsResponse = m_fbaInboundServiceMwsClient.ListInboundShipmentItems(listInboundShipmentItemsRequest);

			ListInboundShipmentItemsResult listInboundShipmentItemsResult = listInboundShipmentItemsResponse.ListInboundShipmentItemsResult;
			
			// TODO: Enumerate through all of the Shipments returned, for now we are going to just request one ShipmentId at a time.
			return new List<InboundShipmentContainer>(new[]
			{
				new InboundShipmentContainer(shipmentId, listInboundShipmentItemsResult.ItemData.member)
			});
		}

		public CreateShipmentResponse CreateInboundShipment(CreateShipmentRequest createShipmentRequest)
		{
			Address shipsFromAddress = AddressUtility.GetShipFromAddress();

			CreateShipmentResponse createShipmentResponse = null;

			List<InboundShipmentPlanRequestItem> shipmentPlanRequestItems = createShipmentRequest
				.ShipmentItems
				.Select(s =>
					new InboundShipmentPlanRequestItem
						{
							ASIN = s.Asin,
							SellerSKU = s.Sku,
							Quantity = s.Quantity,
							Condition = ItemCondition.NewItem.ToString(),
							QuantityInCase = s.QuantityInCase ?? 0
						})
				.ToList();

			InboundShipmentPlanRequestItemList inboundShipmentPlanRequestItemList = new InboundShipmentPlanRequestItemList
				{
					member = shipmentPlanRequestItems
				};

			CreateInboundShipmentPlanRequest createInboundShipmentPlanRequest = new CreateInboundShipmentPlanRequest
				{
					InboundShipmentPlanRequestItems = inboundShipmentPlanRequestItemList,
					Marketplace = m_marketPlaceId,
					SellerId = m_sellerId,
					ShipFromAddress = shipsFromAddress
				};

			CreateInboundShipmentPlanResponse createInboundShipmentPlanResponse = m_fbaInboundServiceMwsClient.CreateInboundShipmentPlan(createInboundShipmentPlanRequest);

			CreateInboundShipmentPlanResult createInboundShipmentPlanResult = createInboundShipmentPlanResponse.CreateInboundShipmentPlanResult;

			if (createInboundShipmentPlanResult.InboundShipmentPlans.member.Any())
			{
				List<InboundShipmentPlan> inboundShipmentPlans = createInboundShipmentPlanResult
					.InboundShipmentPlans
					.member
					.ToList();

				Dictionary<string, ShipmentItem> itemsToBeAddedToShipment = createShipmentRequest
					.ShipmentItems.ToDictionary(k => k.Sku, v => v);

				DateTime today = DateTime.UtcNow.Date;

				List<CreatedShipment> createdShipments = inboundShipmentPlans
					.Select(inboundShipmentPlan => CreateShipment(inboundShipmentPlan, itemsToBeAddedToShipment, shipsFromAddress, createShipmentRequest.ShipmentName, createShipmentRequest.AreCasesRequired, today))
					.ToList();

				createShipmentResponse = new CreateShipmentResponse(createdShipments);
			}

			return createShipmentResponse;
		}

		private CreatedShipment CreateShipment(
			InboundShipmentPlan inboundShipmentPlan,
			Dictionary<string, ShipmentItem> itemsToBeAddedToShipment,
			Address shipsFromAddress,
			string shipmentName,
			bool areCasesRequired,
			DateTime today)
		{
			List<InboundShipmentItem> inboundShipmentItems = new List<InboundShipmentItem>();

			foreach (InboundShipmentPlanItem inboundShipmentPlanItem in inboundShipmentPlan.Items.member)
			{
				if (itemsToBeAddedToShipment.ContainsKey(inboundShipmentPlanItem.SellerSKU))
				{
					ShipmentItem shipmentItem = itemsToBeAddedToShipment[inboundShipmentPlanItem.SellerSKU];

					InboundShipmentItem inboundShipmentItem = new InboundShipmentItem
					{
						FulfillmentNetworkSKU = inboundShipmentPlanItem.FulfillmentNetworkSKU,
						SellerSKU = inboundShipmentPlanItem.SellerSKU,
						QuantityShipped = inboundShipmentPlanItem.Quantity,
						QuantityInCase = shipmentItem.QuantityInCase ?? 0,
						ShipmentId = inboundShipmentPlan.ShipmentId
					};

					inboundShipmentItems.Add(inboundShipmentItem);
				}
			}

			string shipmentNameHeader = string.Concat(
				shipmentName,
				"_",
				inboundShipmentPlan.DestinationFulfillmentCenterId,
				"-",
				inboundShipmentPlan.LabelPrepType,
				"_",
				today.ToShortDateString());

			InboundShipmentHeader header = new InboundShipmentHeader
			{
				AreCasesRequired = areCasesRequired,
				ShipmentName = shipmentNameHeader,
				DestinationFulfillmentCenterId = inboundShipmentPlan.DestinationFulfillmentCenterId,
				ShipFromAddress = shipsFromAddress,
				ShipmentStatus = "WORKING"
			};

			InboundShipmentItemList inboundShipmentItemList = new InboundShipmentItemList { member = inboundShipmentItems };

			CreateInboundShipmentRequest request = new CreateInboundShipmentRequest
			{
				InboundShipmentItems = inboundShipmentItemList,
				Marketplace = m_marketPlaceId,
				SellerId = m_sellerId,
				ShipmentId = inboundShipmentPlan.ShipmentId,
				InboundShipmentHeader = header,
			};

			CreateInboundShipmentResponse createInboundShipmentResponse = m_fbaInboundServiceMwsClient.CreateInboundShipment(request);

			List<ShipmentItem> itemsInShipment = inboundShipmentItems
				.Select(s =>
					{
						ShipmentItem shipmentItem = new ShipmentItem
							{
								Asin = s.FulfillmentNetworkSKU,
								Sku = s.SellerSKU,
								Quantity = s.QuantityShipped,
								QuantityInCase = s.QuantityInCase
							};

						if (itemsToBeAddedToShipment.ContainsKey(s.SellerSKU))
						{
							ShipmentItem originalShipmentItem = itemsToBeAddedToShipment[s.SellerSKU];

							shipmentItem.Name = originalShipmentItem.Name;
							shipmentItem.ItemNumber = originalShipmentItem.ItemNumber;
							shipmentItem.Cost = originalShipmentItem.Cost;
						}
						else
						{
							throw new InvalidOperationException(string.Format("SKU : {0} should not be in this shipment.", s.SellerSKU));
						}

						return shipmentItem;
					})
				.ToList();

			return new CreatedShipment(
				inboundShipmentPlan.DestinationFulfillmentCenterId,
				inboundShipmentPlan.LabelPrepType,
				createInboundShipmentResponse.CreateInboundShipmentResult.ShipmentId,
				itemsInShipment);
		}

		private readonly string m_sellerId;
		private readonly string m_marketPlaceId;
		private readonly FBAInboundServiceMWSClient m_fbaInboundServiceMwsClient;
	}
}

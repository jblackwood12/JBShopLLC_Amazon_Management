using System.Collections.Generic;
using AmazonProductLookup.AmazonApis.MwsApi.Subscriptions.Models;
using AmazonProductLookup.WebServices;
using MWSSubscriptionsService;
using MWSSubscriptionsService.Model;

namespace AmazonProductLookup.AmazonApis.MwsApi.Subscriptions
{
	public sealed class MwsSubscriptionsApi : IMwsSubscriptionServiceApi
	{
		public MwsSubscriptionsApi(string sellerId, string marketPlaceId, string accessKeyId, string secretAccessKeyId, SimpleQueueService simpleQueueService)
		{
			MWSSubscriptionsServiceConfig mwsSubscriptionsServiceConfig = new MWSSubscriptionsServiceConfig
			{
				ServiceURL = "https://mws.amazonservices.com/Subscriptions/2013-07-01/"
			};

			m_mwsSubscriptionServiceClient = new MWSSubscriptionsServiceClient(accessKeyId, secretAccessKeyId, string.Empty, string.Empty, mwsSubscriptionsServiceConfig);

			m_sellerId = sellerId;
			m_marketPlaceId = marketPlaceId;

			m_simpleQueueService = simpleQueueService;
		}

		public void CreateSubscriptionAnyOfferChanged()
		{
			List<AttributeKeyValue> attributeKeyValues = new List<AttributeKeyValue>
			{
				new AttributeKeyValue
				{
					Key = "sqsQueueUrl",
					Value = c_sqsQueueUrl
				}
			};

			AttributeKeyValueList attributeKeyValueList = new AttributeKeyValueList
			{
				Member = attributeKeyValues
			};

			Destination destination = new Destination
			{
				DeliveryChannel = "SQS",
				AttributeList = attributeKeyValueList
			};

			Subscription subscription = new Subscription
			{
				IsEnabled = true,
				NotificationType = "AnyOfferChanged",
				Destination = destination
			};

			CreateSubscriptionInput createSubscriptionInput = new CreateSubscriptionInput
			{
				SellerId = m_sellerId,
				MarketplaceId = m_marketPlaceId,
				Subscription = subscription
			};

			m_mwsSubscriptionServiceClient.CreateSubscription(createSubscriptionInput);
	}

		public List<Notification> ConsumeNotifications(int countNotifications)
		{
			return m_simpleQueueService.ConsumeMessages<Notification>(c_sqsQueueUrl, true, countNotifications);
		}

		private readonly SimpleQueueService m_simpleQueueService;
		private readonly MWSSubscriptionsServiceClient m_mwsSubscriptionServiceClient;

		private readonly string m_sellerId;
		private readonly string m_marketPlaceId;

		private const string c_sqsQueueUrl = "https://sqs.us-east-1.amazonaws.com/616520042730/MWSSubscriptionsService_AnyOfferChangedNotification";
	}
}

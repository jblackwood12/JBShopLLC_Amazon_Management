using AmazonProductLookup.AmazonApis.AdvApi;
using AmazonProductLookup.AmazonApis.MwsApi.CustomerInformation;
using AmazonProductLookup.AmazonApis.MwsApi.Feeds;
using AmazonProductLookup.AmazonApis.MwsApi.FulfillmentInventory;
using AmazonProductLookup.AmazonApis.MwsApi.InboundShipment;
using AmazonProductLookup.AmazonApis.MwsApi.Products;
using AmazonProductLookup.AmazonApis.MwsApi.Subscriptions;
using AmazonProductLookup.WebServices;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace AmazonProductLookup.Infrastructure
{
	public static class AmazonTypeRegistry
	{
		public static Registry GetRegistry()
		{
			Registry registry = new Registry();

			registry.For<IProductAdvertisingApi>().Use(
				new ProductAdvertisingApi(
					Properties.Settings.Default.accessKeyIdAdv,
					Properties.Settings.Default.secretKeyAdv,
					Properties.Settings.Default.destination,
					Properties.Settings.Default.namespaceWebServices,
					Properties.Settings.Default.associateTag));

			registry.For<IMwsProductsApi>().Use(
				new MwsProductsApi(
					Properties.Settings.Default.sellerId,
					Properties.Settings.Default.marketPlaceId,
					Properties.Settings.Default.accessKeyIdMws,
					Properties.Settings.Default.secretAccessKeyIdMws,
					Properties.Settings.Default.serviceUrl));

			registry.For<IMwsFeedsApi>().Use(
				new MwsFeedsApi(
					Properties.Settings.Default.sellerId,
					Properties.Settings.Default.marketPlaceId,
					Properties.Settings.Default.accessKeyIdMws,
					Properties.Settings.Default.secretAccessKeyIdMws,
					Properties.Settings.Default.MerchantIdentifier));

			registry.For<IMwsInboundShipmentApi>().Use(
				new MwsInboundShipmentApi(
					Properties.Settings.Default.sellerId,
					Properties.Settings.Default.marketPlaceId,
					Properties.Settings.Default.accessKeyIdMws,
					Properties.Settings.Default.secretAccessKeyIdMws,
					Properties.Settings.Default.InboundShipmentServiceUrl));

			registry.For<IMwsSubscriptionServiceApi>().Use(
				new MwsSubscriptionsApi(
					Properties.Settings.Default.sellerId,
					Properties.Settings.Default.marketPlaceId,
					Properties.Settings.Default.accessKeyIdMws,
					Properties.Settings.Default.secretAccessKeyIdMws,
					new SimpleQueueService(
						Properties.Settings.Default.accessKeyIdAdv,
						Properties.Settings.Default.secretKeyAdv)));

			registry.For<IMwsCustomerInformationApi>().Use(
				new MwsCustomerInformationApi(
					Properties.Settings.Default.InspireGourmetMerchantID,
					Properties.Settings.Default.InspireGourmetMarketPlaceID,
					Properties.Settings.Default.accessKeyIdMws,
					Properties.Settings.Default.secretAccessKeyIdMws));

			registry.For<IMwsFulfillmentInventoryApi>().Use(
				new MwsFulfillmentInventoryApi(
					Properties.Settings.Default.sellerId,
					Properties.Settings.Default.marketPlaceId,
					Properties.Settings.Default.accessKeyIdMws,
					Properties.Settings.Default.secretAccessKeyIdMws));

			registry.For<SimpleNotificationService>().Use(
				new SimpleNotificationService(
					Properties.Settings.Default.accessKeyIdAdv,
					Properties.Settings.Default.secretKeyAdv));

			return registry;
		}

		public static Container RegisterTypes()
		{
			Container container = new Container();

			container.Configure(x => x.AddRegistry(GetRegistry()));

			return container;
		}
	}
}

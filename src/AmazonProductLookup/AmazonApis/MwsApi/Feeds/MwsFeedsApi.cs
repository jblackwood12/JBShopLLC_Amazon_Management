using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using MarketplaceWebService;
using MarketplaceWebService.Model;

namespace AmazonProductLookup.AmazonApis.MwsApi.Feeds
{
	public sealed class MwsFeedsApi : IMwsFeedsApi
	{
		public MwsFeedsApi(string sellerId, string marketPlaceId, string accessKeyId, string secretAccessKeyId, string merchantIdentifier)
		{
			m_merchantId = sellerId;
			m_marketPlaceId = marketPlaceId;

			m_merchantId = sellerId;
			m_merchantIdentifier = merchantIdentifier;

			const string serviceUrl = "https://mws.amazonservices.com";

			MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig { ServiceURL = serviceUrl };
			config.ServiceURL = serviceUrl;
			config.SetUserAgentHeader("applicationName", "applicationVersion", "C#", "<Parameter 1>", "<Parameter 1>");

			m_service = new MarketplaceWebServiceClient(accessKeyId, secretAccessKeyId, config);
		}

		public void UpdateOurPrices(IEnumerable<UpdatedItemPrice> newPrices)
		{
			SubmitFeedRequest request = new SubmitFeedRequest
			{
				Merchant = m_merchantId,
				MarketplaceIdList = new IdList { Id = new List<string>(new[] { m_marketPlaceId }) }
			};

			// This chunk of code takes our queue, writes the formatted xml string needed, deletes the queue.
			// Puts the xml string into a temporary stream, and makes the request to Amazon.
			string newPricesXmlString = ConstructXmlFile(newPrices);

			byte[] byteArray = Encoding.ASCII.GetBytes(newPricesXmlString);
			request.FeedContent = new MemoryStream(byteArray);
			request.ContentMD5 = MarketplaceWebServiceClient.CalculateContentMD5(request.FeedContent);
			request.FeedType = "_POST_PRODUCT_PRICING_DATA_";

			InvokeSubmitFeed(request);

			// No memory leaks?  I think this is unnecessary.
			request.FeedContent.Close();
		}

		private void InvokeSubmitFeed(SubmitFeedRequest request)
		{
			m_service.SubmitFeed(request);
		}

		private string ConstructXmlFile(IEnumerable<UpdatedItemPrice> newPrices)
		{
			string xmlString = "<?xml version=\"1.0\"?>\n";
			xmlString += "<AmazonEnvelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"amzn-envelope.xsd\">\n";
			xmlString += "\t<Header>\n\t\t<DocumentVersion>1.01</DocumentVersion>\n\t\t<MerchantIdentifier>" + m_merchantIdentifier + "</MerchantIdentifier>\n\t</Header>\n";
			xmlString += "\t<MessageType>Price</MessageType>\n";

			int messageNum = 1;
			foreach (UpdatedItemPrice newPrice in newPrices)
			{
				xmlString += "\t<Message>\n";
				xmlString += "\t\t<MessageID>" + messageNum + "</MessageID>\n";
				xmlString += "\t\t<Price>\n";
				xmlString += "\t\t\t<SKU>" + newPrice.Sku + "</SKU>\n";
				xmlString += "\t\t\t<StandardPrice currency=\"USD\">" + newPrice.UpdatedPrice.ToString("F", CultureInfo.InvariantCulture) + "</StandardPrice>\n";
				xmlString += "\t\t</Price>\n\t</Message>\n";

				// Increase the message number for each item added.
				messageNum++;
			}

			xmlString += "</AmazonEnvelope>";
			return xmlString;
		}

		private readonly string m_merchantId;
		private readonly string m_marketPlaceId;
		private readonly string m_merchantIdentifier;

		private readonly MarketplaceWebService.MarketplaceWebService m_service;
	}
}

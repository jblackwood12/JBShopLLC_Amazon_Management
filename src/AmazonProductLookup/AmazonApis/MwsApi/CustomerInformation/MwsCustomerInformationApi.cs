using System;
using System.Collections.Generic;
using MWSCustomerService;
using MWSCustomerService.Model;

namespace AmazonProductLookup.AmazonApis.MwsApi.CustomerInformation
{
	public sealed class MwsCustomerInformationApi : IMwsCustomerInformationApi
	{
		public MwsCustomerInformationApi(string sellerId, string marketPlaceId, string accessKeyId, string secretAccessKeyId)
		{
			MWSCustomerServiceConfig config = new MWSCustomerServiceConfig
			{
				ServiceURL = "https://mws.amazonservices.com/CustomerInformation/2014-03-01/"
			};

			m_sellerId = sellerId;
			m_marketPlaceId = marketPlaceId;

			m_mwsCustomerServiceClient = new MWSCustomerServiceClient(accessKeyId, secretAccessKeyId, string.Empty, string.Empty, config);
		}

		public void ListCustomers()
		{
			DateTime endDate = DateTime.UtcNow.Date;
			DateTime beginDate = endDate.AddDays(-365);

			ListCustomersResponse listCustomersResponse = m_mwsCustomerServiceClient.ListCustomers(new ListCustomersRequest
			{
				SellerId = m_sellerId,
				MarketplaceId = m_marketPlaceId,
				DateRangeType = "AssociatedDate",
				DateRangeStart = beginDate,
				DateRangeEnd = endDate
			});

			List<Customer> customers = listCustomersResponse.ListCustomersResult.CustomerList;
		}

		private readonly MWSCustomerServiceClient m_mwsCustomerServiceClient;

		private readonly string m_sellerId;

		private readonly string m_marketPlaceId;
	}
}

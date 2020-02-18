using System.Collections.Generic;
using AmazonProductLookup.AmazonApis.MwsApi.Subscriptions.Models;

namespace AmazonProductLookup.AmazonApis.MwsApi.Subscriptions
{
	public interface IMwsSubscriptionServiceApi
	{
		void CreateSubscriptionAnyOfferChanged();

		List<Notification> ConsumeNotifications(int countNotifications);
	}
}

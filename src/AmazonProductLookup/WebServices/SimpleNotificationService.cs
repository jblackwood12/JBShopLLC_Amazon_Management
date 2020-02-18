using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace AmazonProductLookup.WebServices
{
	public sealed class SimpleNotificationService
	{
		public SimpleNotificationService(string awsAccessKeyId, string awsSecretAccessKey)
		{
			m_amazonSimpleNotificationServiceClient = new AmazonSimpleNotificationServiceClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.USEast1);
		}

		/// <summary>
		/// Sends a subject/message to all recipients.
		/// </summary>
		/// <param name="subject">Displays as the main subject.</param>
		/// <param name="message">Displays only in email body, not on SMS.</param>
		public void PublishAlert(string subject, string message)
		{
			PublishRequest publishRequest = new PublishRequest(c_topicArn, message, subject);

			m_amazonSimpleNotificationServiceClient.Publish(publishRequest);
		}

		private const string c_topicArn = "arn:aws:sns:us-east-1:616520042730:JBSHOPLLC_RepricingAlerts";

		private readonly AmazonSimpleNotificationServiceClient m_amazonSimpleNotificationServiceClient;
	}
}

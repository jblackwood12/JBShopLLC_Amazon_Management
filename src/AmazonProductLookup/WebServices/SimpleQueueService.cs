using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Logging;
using Utility;

namespace AmazonProductLookup.WebServices
{
	public sealed class SimpleQueueService
	{
		public SimpleQueueService(string awsAccessKeyId, string awsSecretAccessKey)
		{
			m_amazonSqsClient = new AmazonSQSClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.USEast1);
		}

		public List<T> ConsumeMessages<T>(string queueUrl, bool deleteMessages = false, int? maxNumberOfMessages = null)
		{
			ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest
			{
				QueueUrl = queueUrl
			};

			if (maxNumberOfMessages.HasValue)
				receiveMessageRequest.MaxNumberOfMessages = maxNumberOfMessages.Value;

			ReceiveMessageResponse receiveMessageResponse = m_amazonSqsClient.ReceiveMessage(receiveMessageRequest);

			var messagesAndMd5 = receiveMessageResponse.Messages
				.Select(s => new { s.MessageId, s.Body, IsValidMD5 = Md5Utility.CompareMd5Sqs(s.Body, s.MD5OfBody), s.ReceiptHandle })
				.ToList();
			
			if (messagesAndMd5.Any(a => !a.IsValidMD5))
				LoggingRepository.Log(LoggingCategory.RepricingScript, "MD5 on IMwsSubscriptionServiceApi was not valid.");

			// Filter out messages with corrupted Body (Md5 didn't match).
			var messagesAndMd5Filtered = messagesAndMd5.Where(w => w.IsValidMD5)
				.ToDictionary(k => k.MessageId, v => v);

			if (deleteMessages && messagesAndMd5Filtered.Any())
			{
				List<DeleteMessageBatchRequestEntry> deleteMessageBatchRequestEntries = messagesAndMd5Filtered
					.Select(s => s.Value)
					.Select(s => new DeleteMessageBatchRequestEntry { Id = s.MessageId, ReceiptHandle = s.ReceiptHandle })
					.ToList();

				DeleteMessageBatchResponse deleteMessageBatchResponse = m_amazonSqsClient.DeleteMessageBatch(new DeleteMessageBatchRequest { Entries = deleteMessageBatchRequestEntries, QueueUrl = queueUrl });

				// Don't return messages that we weren't able to delete.
				if (deleteMessageBatchResponse.Failed.Any())
				{
					foreach (BatchResultErrorEntry batchResultErrorEntry in deleteMessageBatchResponse.Failed)
					{
						if (messagesAndMd5Filtered.ContainsKey(batchResultErrorEntry.Id))
							messagesAndMd5Filtered.Remove(batchResultErrorEntry.Id);
					}
				}
			}

			return messagesAndMd5Filtered.Select(s => s.Value.Body.FromXml<T>()).ToList();
		}

		private readonly AmazonSQSClient m_amazonSqsClient;
	}
}

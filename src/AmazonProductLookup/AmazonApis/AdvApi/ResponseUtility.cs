using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;
using Models.AdvApi;
using System.Linq;
using Utility;
using Utility.Exceptions;

namespace AmazonProductLookup.AmazonApis.AdvApi
{
	public static class ResponseUtility
	{
		public static ProductAdvApiResponse IssueRequest(string requestUrl, string namespaceWebServices)
		{
			Func<ProductAdvApiResponse> func = () =>
			{
				ProductAdvApiResponse returnValue;

				XmlDocument doc = new XmlDocument();

				try
				{
					WebRequest request = WebRequest.Create(requestUrl);
					WebResponse response = request.GetResponse();

					Stream stream = response.GetResponseStream();

					if (stream != null)
						doc.Load(stream);

					XmlNodeList nodeList = doc.GetElementsByTagName("Item");

					List<XmlNode> nodes = new List<XmlNode>();

					bool errored = true;

					if (IsValidResponse(doc, namespaceWebServices))
					{
						errored = false;

						nodes.AddRange(nodeList.Cast<XmlNode>());
					}

					returnValue = new ProductAdvApiResponse(nodes, errored);
				}
				catch (Exception)
				{
					throw new RetryException("Throttled, attempt again after waiting.");
				}
				finally
				{
					Thread.Sleep(c_millisecondsPerSecond / c_requestsPerSecond); //Make sure we don't run out of the allotted requests.
				}

				return returnValue;
			};

			ProductAdvApiResponse productAdvApiResponse = func.Retry(c_numberOfRetries, c_timeToWaitToBecomeUnthrottled);

			if (productAdvApiResponse == null)
			{
				XmlNode node = new XmlDocument();

				productAdvApiResponse = new ProductAdvApiResponse(node, true);
			}

			return productAdvApiResponse;
		}

		public static bool IsValidResponse(XmlDocument doc, string namespaceWebServices)
		{
			return doc.HasChildNodes && doc.GetElementsByTagName("Errors", namespaceWebServices).Count == 0;
		}

		private const int c_numberOfRetries = 3;
		private const int c_timeToWaitToBecomeUnthrottled = 3000;

		private const int c_requestsPerSecond = 1;
		private const int c_millisecondsPerSecond = 1000;
	}
}

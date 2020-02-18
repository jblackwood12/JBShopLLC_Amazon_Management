using System.Net;
using System;
using System.IO;
using BrightPearlClient.Models.Response;
using Newtonsoft.Json;

namespace BrightPearlClient.BrightPearlApi
{
	public class BrightPearlService : IBrightPearlService
	{
		public BrightPearlService(string appRef, string token, string serviceUrl)
		{
			m_appRef = appRef;
			m_token = token;
			m_serviceUrl = serviceUrl;
		}

		// TODO: Move a lot of this into the request utility
		public ProductResponse GetProducts(int start, int end)
		{
			string requestUrl = string.Format("{0}product-service/product/{1}-{2}", m_serviceUrl, start, end);
			WebHeaderCollection headers = new WebHeaderCollection
											{
												{ "brightpearl-app-ref", this.m_appRef },
												{ "brightpearl-account-token", this.m_token }
											};

			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
			webRequest.Headers = headers;

			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Accept = "application/json";

			string jsonResponse;

			using (WebResponse webResponse = webRequest.GetResponse())
			using (Stream str = webResponse.GetResponseStream())
			{
				if (str == null)
					throw new InvalidOperationException("ResponseStream was null.");

				using (StreamReader sr = new StreamReader(str))
					jsonResponse = sr.ReadToEnd();
			}

			return JsonConvert.DeserializeObject<ProductResponse>(jsonResponse);
		}

		private readonly string m_appRef;
		private readonly string m_token;
		private readonly string m_serviceUrl;
	}
}

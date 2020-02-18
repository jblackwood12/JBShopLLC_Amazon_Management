using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AmazonProductLookup.AmazonApis.AdvApi
{
	public static class SigningUtility
	{
		public static string Sign(IDictionary<string, string> request, string akid, string endPoint, HMAC signer, string associateTag)
		{
			request["Service"] = c_service;
			request["Version"] = c_version;
			request["AssociateTag"] = associateTag;

			// Use a SortedDictionary to get the parameters in natural byte order, as required by AWS.
			ParamComparer pc = new ParamComparer();
			SortedDictionary<string, string> sortedMap = new SortedDictionary<string, string>(request, pc);

			// Add the AWSAccessKeyId and Timestamp to the requests.
			sortedMap["AWSAccessKeyId"] = akid;
			sortedMap["Timestamp"] = GetTimestamp();

			// Get the canonical query string
			string canonicalQs = ConstructCanonicalQueryString(sortedMap);

			// Derive the bytes needs to be signed.
			StringBuilder builder = new StringBuilder();
			builder.Append(c_requestMethod)
				.Append("\n")
				.Append(endPoint)
				.Append("\n")
				.Append(c_requestUri)
				.Append("\n")
				.Append(canonicalQs);

			string stringToSign = builder.ToString();
			byte[] toSign = Encoding.UTF8.GetBytes(stringToSign);

			// Compute the signature and convert to Base64.
			byte[] sigBytes = signer.ComputeHash(toSign);
			string signature = Convert.ToBase64String(sigBytes);

			// now construct the complete URL and return to caller.
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("http://")
				.Append(endPoint)
				.Append(c_requestUri)
				.Append("?")
				.Append(canonicalQs)
				.Append("&Signature=")
				.Append(PercentEncodeRfc3986(signature));

			return stringBuilder.ToString();
		}

		/*
		 * Current time in IS0 8601 format as required by Amazon
		 */
		public static string GetTimestamp()
		{
			DateTime currentTime = DateTime.UtcNow;
			string timestamp = currentTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
			return timestamp;
		}

		/*
		 * Percent-encode (URL Encode) according to RFC 3986 as required by Amazon.
		 * 
		 * This is necessary because .NET's HttpUtility.UrlEncode does not encode
		 * according to the above standard. Also, .NET returns lower-case encoding
		 * by default and Amazon requires upper-case encoding.
		 */
		public static string PercentEncodeRfc3986(string str)
		{
			str = HttpUtility.UrlEncode(str, Encoding.UTF8);

			if (str == null)
				throw new NullReferenceException("str was null.");

			str = str.Replace("'", "%27").Replace("(", "%28").Replace(")", "%29").Replace("*", "%2A").Replace("!", "%21").Replace("%7e", "~").Replace("+", "%20");

			StringBuilder sbuilder = new StringBuilder(str);
			for (int i = 0; i < sbuilder.Length; i++)
			{
				if (sbuilder[i] == '%')
				{
					if (char.IsLetter(sbuilder[i + 1]) || char.IsLetter(sbuilder[i + 2]))
					{
						sbuilder[i + 1] = char.ToUpper(sbuilder[i + 1]);
						sbuilder[i + 2] = char.ToUpper(sbuilder[i + 2]);
					}
				}
			}

			return sbuilder.ToString();
		}

		public static IDictionary<string, string> CreateDictionary(string queryString)
		{
			Dictionary<string, string> map = new Dictionary<string, string>();

			string[] requestParams = queryString.Split('&');

			foreach (string t in requestParams)
			{
				if (t.Length < 1)
				{
					continue;
				}

				char[] sep = { '=' };
				string[] param = t.Split(sep, 2);
				for (int j = 0; j < param.Length; j++)
				{
					param[j] = HttpUtility.UrlDecode(param[j], Encoding.UTF8);
				}

				switch (param.Length)
				{
					case 1:
						{
							if (t.Length >= 1)
							{
								if (t.ToCharArray()[0] == '=')
								{
									map[""] = param[0];
								}
								else
								{
									map[param[0]] = "";
								}
							}

							break;
						}

					case 2:
						{
							if (!string.IsNullOrEmpty(param[0]))
							{
								map[param[0]] = param[1];
							}
						}

						break;
				}
			}

			return map;
		}

		public static string ConstructCanonicalQueryString(SortedDictionary<string, string> sortedParamMap)
		{
			StringBuilder builder = new StringBuilder();

			if (sortedParamMap.Count == 0)
			{
				builder.Append("");
				return builder.ToString();
			}

			foreach (KeyValuePair<string, string> kvp in sortedParamMap)
			{
				builder.Append(PercentEncodeRfc3986(kvp.Key));
				builder.Append("=");
				builder.Append(PercentEncodeRfc3986(kvp.Value));
				builder.Append("&");
			}

			string canonicalString = builder.ToString();
			canonicalString = canonicalString.Substring(0, canonicalString.Length - 1);
			return canonicalString;
		}

		private const string c_service = "AWSECommerceService";
		private const string c_version = "2011-08-01";
		private const string c_requestUri = "/onca/xml";
		private const string c_requestMethod = "GET";
	}

	class ParamComparer : IComparer<string>
	{
		public int Compare(string p1, string p2)
		{
			return string.CompareOrdinal(p1, p2);
		}
	}
}
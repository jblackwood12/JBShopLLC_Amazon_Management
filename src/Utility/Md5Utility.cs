using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utility
{
	public static class Md5Utility
	{
		public static string ComputeMd5(this MemoryStream memoryStream)
		{
			long originalPosition = memoryStream.Position;

			MD5 md5 = MD5.Create();

			byte[] hash = md5.ComputeHash(memoryStream);

			memoryStream.Position = originalPosition;

			return Convert.ToBase64String(hash);
		}

		public static bool CompareMd5Sqs(string message, string md5FromService)
		{
			return string.Equals(ComputeMd5Sqs(message), md5FromService, StringComparison.OrdinalIgnoreCase);
		}

		private static string ComputeMd5Sqs(this string input)
		{
			MD5 md5 = MD5.Create();

			return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);
		}
	}
}

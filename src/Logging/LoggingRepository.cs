using Loggly;
using Newtonsoft.Json;

namespace Logging
{
	public static class LoggingRepository
	{
		public static void Log(LoggingCategory loggingCategory, string message)
		{
			LogDescriptor logDescriptor = new LogDescriptor
				{
					Category = loggingCategory.ToString(),
					Message = message
				};

			Logger logger = new Logger(c_token, c_httpsEndpoint);

			try
			{
				logger.LogSync(JsonConvert.SerializeObject(logDescriptor), true);
			}
			catch
			{
			}
		}

		private const string c_token = "";

		private const string c_httpsEndpoint = "logs-01.loggly.com/";
	}
}

using System;
using System.Collections.Concurrent;
using AmazonProductLookup.WebServices;
using Logging;

namespace RepricingScript.Loggers
{
	public sealed class ExceptionLogger
	{
		private ExceptionLogger() { }

		public static ExceptionLogger Instance
		{
			get
			{
				if (s_instance == null)
				{
					lock (s_syncRoot)
					{
						if (s_instance == null)
							s_instance = new ExceptionLogger();
					}
				}

				return s_instance;
			}
		}

		public void SetUpServices(SimpleNotificationService simpleNotificationService)
		{
			m_simpleNotificationService = simpleNotificationService;
		}

		public void LogException(Exception ex)
		{
			try
			{
				string message = ex.Message;
				bool emailException = true;

				if (m_publishedExceptions.ContainsKey(message))
				{
					DateTime lastSent = m_publishedExceptions[message];
					if (lastSent.AddHours(c_emailMessageThrottleHours) >= DateTime.UtcNow)
						emailException = false;
				}

				m_publishedExceptions.AddOrUpdate(
					message,
					DateTime.UtcNow,
					(key, lastSent) => lastSent.AddHours(c_emailMessageThrottleHours) >= DateTime.UtcNow
						? lastSent
						: DateTime.UtcNow);

				string messageToLog = string.Format("Repricing script Exception: {0} StackTrace: {1}", ex.Message, ex.StackTrace);

				if (emailException)
				{
					string subject = string.Format("Repricing script Exception: {0}", message);
					subject = subject.Substring(0, Math.Min(subject.Length, c_maxSubjectLength));

					m_simpleNotificationService.PublishAlert(subject, messageToLog);
				}

				#if DEBUG
					Console.WriteLine(message);
				#else
					LoggingRepository.Log(LoggingCategory.RepricingScript, string.Format("Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace));
				#endif
			}
			catch (Exception e)
			{
				LogMessage(string.Format("Failed to LogException. Message: {0}, StackTrace: {1}", e.Message, e.StackTrace));
			}
		}

		public void LogMessage(string message)
		{
			#if DEBUG
				Console.WriteLine(message);
			#else
				LoggingRepository.Log(LoggingCategory.RepricingScript, message);
			#endif
		}

		private static volatile ExceptionLogger s_instance;
		private static readonly object s_syncRoot = new object();

		private const int c_emailMessageThrottleHours = 3;
		private const int c_maxSubjectLength = 99;

		// Key is the error message, value is the time last sent.
		private readonly ConcurrentDictionary<string, DateTime> m_publishedExceptions = new ConcurrentDictionary<string, DateTime>();

		private SimpleNotificationService m_simpleNotificationService;
	}
}

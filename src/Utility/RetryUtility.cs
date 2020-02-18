using System;
using System.Threading;
using Utility.Exceptions;

namespace Utility
{
	public static class RetryUtility
	{
		public static T Retry<T>(this Func<T> functionToRetry, int maxRetries, int? millisecondsToWaitBetweenRetries = null, bool shouldCatchQuitException = false)
		{
			if (maxRetries <= 0)
				throw new ArgumentException("Parameter maxRetries must be greater than 0.");

			T value = default(T);

			for (int i = 0; i < maxRetries; i++)
			{
				if (shouldCatchQuitException)
				{
					try
					{
						value = functionToRetry.Invoke();
						break;
					}
					catch (QuitException qe)
					{
						Console.WriteLine("Caught QuitException: {0}", qe.Message);
					}
				}
				else
				{
					try
					{
						value = functionToRetry.Invoke();
						break;
					}
					catch (RetryException re)
					{
						Console.WriteLine("Caught RetryException: {0}", re.Message);
					}
				}

				if (millisecondsToWaitBetweenRetries.HasValue && ((i + 1) < maxRetries))
					Thread.Sleep(millisecondsToWaitBetweenRetries.Value);
			}

			return value;
		}
	}
}

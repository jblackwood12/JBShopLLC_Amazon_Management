namespace Utility.Exceptions
{
	public sealed class RetryException : QuitException
	{
		public RetryException(string message)
			: base(message)
		{
		}
	}
}

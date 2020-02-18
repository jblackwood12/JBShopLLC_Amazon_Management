using System;

namespace Utility.Exceptions
{
	public class QuitException : Exception
	{
		public QuitException(string message) : base(message)
		{
		}
	}
}

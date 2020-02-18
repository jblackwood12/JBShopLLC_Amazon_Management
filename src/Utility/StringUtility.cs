namespace Utility
{
	public static class StringUtility
	{
		public static string Truncate(this string value, int maxLength)
		{
			return value.Length <= maxLength ? value : value.Substring(0, maxLength);
		}

		public static bool IsNullOrEmptyTrimmed(this string value)
		{
			bool verdict = true;

			if (value != null)
			{
				value = value.Trim();
				verdict = !(value.Length > 0);
			}

			return verdict;
		}
	}
}

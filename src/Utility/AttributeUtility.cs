using System;
using System.Linq;
using System.Reflection;
using Models;
using System.Collections.Generic;

namespace Utility
{
	public static class AttributeUtility
	{
		public static string SanitizeInput(this PropertyInfo propertyInfo, string input)
		{
			List<Sanitize> sanitizers = propertyInfo.GetCustomAttributes(typeof(Sanitize), false)
				.Cast<Sanitize>()
				.ToList();

			foreach (Sanitize sanitize in sanitizers)
			{
				if (sanitize.InputType.HasValue)
				{
					switch (sanitize.InputType)
					{
						case InputType.Money:
							input = input.Replace("$", string.Empty).Replace(",", string.Empty);
							break;

						case InputType.UPC:
							input = input.Replace(" ", string.Empty).Replace("-", string.Empty);
							break;

						case InputType.ASIN:
							input = input.Replace(" ", string.Empty);
							break;

						case InputType.CasePackQuantity:
							input = input.Replace("*", string.Empty);
							break;
					}
				}

				if (sanitize.MaximumCharacters.HasValue)
					input = input.Truncate(sanitize.MaximumCharacters.Value);
			}

			return input;
		}

		public static bool MatchesAlias(this PropertyInfo propertyInfo, string columnToCompare)
		{
			List<Alias> aliases = propertyInfo.GetCustomAttributes(typeof(Alias), false).Cast<Alias>().ToList();

			List<string> aliasNames = aliases.SelectMany(s => s.AlternateFields).ToList();
			aliasNames.Add(propertyInfo.Name);

			return aliasNames.Any(a => a.Equals(columnToCompare, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}

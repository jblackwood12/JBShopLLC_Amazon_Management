using System;
using System.Linq;
using System.Web.Mvc;

namespace Utility
{
	public static class MvcUtility
	{
		// This function converts an enum list into a SelectList, for display in html.
		public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
		{
			var values = from TEnum e in Enum.GetValues(typeof(TEnum))
						 select new { Id = e, Name = e.ToString() };

			return new SelectList(values, "Id", "Name", enumObj);
		}
	}
}
using System;
using System.Reflection;

namespace Utility
{
	public static class ReflectionUtility
	{
		public static T ModifyValue<T>(this T obj, PropertyInfo propertyInfo, string value)
		{
			try
			{
				Type underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);

				if (underlyingType != null)
				{
					Type type = Type.GetType(underlyingType.ToString());

					if (type != null)
						propertyInfo.SetValue(obj, Convert.ChangeType(value, type), null);
				}
				else
				{
					Type type = Type.GetType(propertyInfo.PropertyType.ToString());

					if (type != null)
						propertyInfo.SetValue(obj, Convert.ChangeType(value, type), null);
				}
			}
			catch (Exception)
			{
				////Console.WriteLine(e.Message);
			}

			return obj;
		}
	}
}

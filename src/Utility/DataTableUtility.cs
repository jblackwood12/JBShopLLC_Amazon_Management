using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Utility
{
	public static class DataTableUtility
	{
		public static DataTable ToDataTable<T>(this IEnumerable<T> data)
		{
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
			DataTable table = new DataTable();
			for (int i = 0; i < props.Count; i++)
			{
				PropertyDescriptor prop = props[i];

				Type underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);

				if (underlyingType != null)
				{
					string underlyingTypeString = underlyingType.ToString();

					Type nonNullableType = Type.GetType(underlyingTypeString);

					if (nonNullableType != null)
						table.Columns.Add(prop.Name, nonNullableType);
				}
				else
				{
					table.Columns.Add(prop.Name, prop.PropertyType);
				}
			}

			object[] values = new object[props.Count];
			foreach (T item in data)
			{
				for (int i = 0; i < values.Length; i++)
					values[i] = props[i].GetValue(item);

				table.Rows.Add(values);
			}

			return table;
		}

		public static List<T> FromDataTableToList<T>(this DataTable datatable) where T : new()
		{
			List<T> temp = new List<T>();

			try
			{
				List<string> columnsNames = (from DataColumn DataColumn in datatable.Columns select DataColumn.ColumnName).ToList();
				
				// Filter out rows where there is no input.
				temp = datatable
					.AsEnumerable()
					.Where(w => w.ItemArray.Any(a => !a.ToString().IsNullOrEmptyTrimmed()))
					.ToList()
					.ConvertAll(row => GetObject<T>(row, columnsNames));
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			return temp;
		}

		private static T GetObject<T>(DataRow row, IEnumerable<string> columnNames) where T : new()
		{
			List<string> enumeratedColumnNames = columnNames.ToList();

			T obj = new T();

			PropertyInfo[] propertyInfos = typeof(T).GetProperties();
			foreach (PropertyInfo propertyInfo in propertyInfos)
			{
				// Case insensitive matching of column name to variable name.
				string columName = enumeratedColumnNames.Find(name => propertyInfo.MatchesAlias(name));

				if (!string.IsNullOrEmpty(columName))
				{
					string value = row[columName].ToString();

					if (!string.IsNullOrEmpty(value))
					{
						// Remove common punctuation such as '$', '*', and '-'.
						value = propertyInfo.SanitizeInput(value);

						obj = obj.ModifyValue(propertyInfo, value);
					}
				}
			}

			return obj;
		}
	}
}
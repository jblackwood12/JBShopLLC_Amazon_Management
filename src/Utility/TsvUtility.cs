using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Utility
{
	public static class TsvUtility
	{
		public static List<T> ConvertFromTsvToList<T>(this MemoryStream memoryStream, Func<string, string> stringModification = null) where T : new()
		{
			List<T> mappedObjects = new List<T>();

			using (StreamReader reader = new StreamReader(memoryStream))
			{
				List<PropertyInfo> propertyInfoMappings = new List<PropertyInfo>();

				bool firstRow = true;

				do
				{
					string singleLine = reader.ReadLine();

					if (singleLine != null)
					{
						List<string> segments = singleLine.Split('\t').ToList();

						if (segments.Any(a => a.Length != 0))
						{
							if (firstRow)
							{
								PropertyInfo[] propertyInfos = typeof(T).GetProperties();

								foreach (string segment in segments)
								{
									PropertyInfo propertyInfoMapping = null;

									foreach (PropertyInfo propertyInfo in propertyInfos)
									{
										if (propertyInfo.MatchesAlias(segment))
											propertyInfoMapping = propertyInfo;
									}

									propertyInfoMappings.Add(propertyInfoMapping);
								}

								firstRow = false;
							}
							else
							{
								T obj = new T();

								for (int i = 0; i < propertyInfoMappings.Count; i++)
								{
									if (i <= (segments.Count - 1))
									{
										string segment = segments[i];
										PropertyInfo propertyInfo = propertyInfoMappings[i];

										if (propertyInfo != null)
										{
											string value = propertyInfo.SanitizeInput(segment);
											obj = obj.ModifyValue(propertyInfo, value);
										}
									}
								}

								mappedObjects.Add(obj);
							}
						}
					}
				}
				while (reader.Peek() >= 0);
			}

			return mappedObjects;
		}
	}
}

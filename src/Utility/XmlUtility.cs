using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Utility
{
	public static class XmlUtility
	{
		public static XmlNode GetChildXmlNode(this XmlNode parentXmlNode, string childNodeName)
		{
			return parentXmlNode.ChildNodes.Cast<XmlNode>()
				.FirstOrDefault(f => f.Name.Equals(childNodeName, StringComparison.InvariantCultureIgnoreCase));
		}

		public static bool GetValueFromChildNode<T>(this XmlNode parentXmlNode, string childNodeName, out T value, Func<XmlNode, T> valueModifier = null)
		{
			return GetValueFromChildNode(parentXmlNode.ChildNodes.Cast<XmlNode>(), childNodeName, out value, valueModifier);
		}

		public static bool GetValueFromChildNode<T>(this IEnumerable<XmlNode> xmlNodes, string childNodeName, out T value, Func<XmlNode, T> valueModifier = null)
		{
			bool success = false;
			value = default(T);

			XmlNode selectedNode = xmlNodes.FirstOrDefault(f => f.Name.Equals(childNodeName, StringComparison.InvariantCultureIgnoreCase));
			if (selectedNode != null)
			{
				try
				{
					value = (T)Convert.ChangeType(selectedNode.InnerText, typeof(T));

					if (valueModifier != null)
						value = valueModifier.Invoke(selectedNode);

					success = true;
				}
				catch
				{
				}
			}

			return success;
		}

		public static T FromXml<T>(this string xml)
		{
			T returnedXmlClass = default(T);

			using (TextReader reader = new StringReader(xml))
			{
				try
				{
					returnedXmlClass = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
				}
				catch (InvalidOperationException)
				{
					// String passed is not XML, simply return defaultXmlClass
				}
			}

			return returnedXmlClass;
		}
	}
}

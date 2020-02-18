using System;
using System.Xml;

namespace AmazonProductLookup.AmazonApis.AdvApi.ItemLookup
{
	public static class DimensionUtility
	{
		public static Func<XmlNode, decimal> GetInchesConverter()
		{
			Func<XmlNode, decimal> conversionToDimension = node =>
				{
					decimal returnValue = Convert.ToDecimal(node.InnerText);

					if (node.Attributes != null)
					{
						XmlNode attributeNode = node.Attributes.GetNamedItem("Units");

						string attributeText = attributeNode.InnerText;

						if (attributeText.Equals("hundredths-inches", StringComparison.InvariantCultureIgnoreCase))
						{
							returnValue /= 100;
						}
						else if (!attributeText.Equals("inches", StringComparison.InvariantCultureIgnoreCase))
						{
							throw new InvalidOperationException("XmlNode does not have correct units.");
						}
					}

					return returnValue;
				};

			return conversionToDimension;
		}

		public static Func<XmlNode, decimal> GetPoundsConverter()
		{
			Func<XmlNode, decimal> conversionToDimension = node =>
				{
					decimal returnValue = Convert.ToDecimal(node.InnerText);

					if (node.Attributes != null)
					{
						XmlNode attributeNode = node.Attributes.GetNamedItem("Units");

						string attributeText = attributeNode.InnerText;

						if (attributeText.Equals("hundredths-pounds", StringComparison.InvariantCultureIgnoreCase))
						{
							returnValue /= 100;
						}
						else if (!attributeText.Equals("pounds", StringComparison.InvariantCultureIgnoreCase))
						{
							throw new InvalidOperationException("XmlNode does not have correct units.");
						}
					}

					return returnValue;
				};

			return conversionToDimension;
		}
	}
}

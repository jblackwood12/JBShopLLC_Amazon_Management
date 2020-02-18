using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Models;
using Utility;

namespace AmazonProductLookup.AmazonApis.AdvApi.ItemLookup
{
	public static class ItemLookupUtility
	{
		public static ProductAndProductMetadata ReadAsinLookupResponse(IEnumerable<XmlNode> nodes)
		{
			Product product = new Product();
			ProductMetadata productMetadata = new ProductMetadata();

			if (nodes == null)
				return null;

			List<XmlNode> itemElements = nodes.ToList();

			if (!itemElements.Any())
				return null;

			XmlNode firstItemElement = itemElements.First();

			XmlNode asinElement = firstItemElement.GetChildXmlNode("ASIN");
			if (asinElement != null)
			{
				XmlNode asinElementInnerXml = asinElement.ChildNodes.Cast<XmlNode>().First();
				if (asinElementInnerXml != null)
					product.ASIN = asinElementInnerXml.InnerText;
			}

			XmlNode largeImageElement = firstItemElement.GetChildXmlNode("LargeImage");
			if (largeImageElement != null)
			{
				XmlNode largeImageElementInnerXml = largeImageElement.ChildNodes.Cast<XmlNode>().First();
				if (largeImageElementInnerXml != null)
					productMetadata.ImageUrl = largeImageElementInnerXml.InnerText;
			}

			string salesRank;
			firstItemElement.GetValueFromChildNode("SalesRank", out salesRank);
			productMetadata.SalesRank = salesRank;

			XmlNode itemAttributeNode = firstItemElement.GetChildXmlNode("ItemAttributes");
			if (itemAttributeNode != null)
			{
				string productName;
				itemAttributeNode.GetValueFromChildNode("Title", out productName);
				product.Name = productName;

				string productGroup;
				itemAttributeNode.GetValueFromChildNode("ProductGroup", out productGroup);
				productMetadata.ProductGroup = productGroup;

				string manufacturerName;
				itemAttributeNode.GetValueFromChildNode("Manufacturer", out manufacturerName);
				productMetadata.Manufacturer = manufacturerName;

				// Use name 'ItemNumber' instead of model.
				string model;
				itemAttributeNode.GetValueFromChildNode("Model", out model);
				product.ItemNumber = model;

				// Use UPC if one is found, rather than the EAN.
				string ean;
				itemAttributeNode.GetValueFromChildNode("EAN", out ean);
				product.UPC = ean;

				string upc;
				itemAttributeNode.GetValueFromChildNode("UPC", out upc);
				product.UPC = upc;

				XmlNode packageDimensionsNode = itemAttributeNode.GetChildXmlNode("PackageDimensions");
				if (packageDimensionsNode != null)
				{
					decimal height;
					if (packageDimensionsNode.GetValueFromChildNode("Height", out height, DimensionUtility.GetInchesConverter()))
						product.Height = height;

					decimal width;
					if (packageDimensionsNode.GetValueFromChildNode("Width", out width, DimensionUtility.GetInchesConverter()))
						product.Width = width;

					decimal length;
					if (packageDimensionsNode.GetValueFromChildNode("Length", out length, DimensionUtility.GetInchesConverter()))
						product.Length = length;

					decimal weight;
					if (packageDimensionsNode.GetValueFromChildNode("Weight", out weight, DimensionUtility.GetPoundsConverter()))
						product.Weight = weight;
				}

				XmlNode listPriceNode = itemAttributeNode.GetChildXmlNode("ListPrice");
				if (listPriceNode != null)
				{
					string listPrice;
					listPriceNode.GetValueFromChildNode("FormattedPrice", out listPrice);
					productMetadata.ListPrice = listPrice;
				}
			}

			return new ProductAndProductMetadata(product, productMetadata);
		}

		public static Dictionary<string, decimal> ReadAmazonListingResponse(IEnumerable<XmlNode> xmlNodes)
		{
			List<XmlNode> listingNodes = xmlNodes.ToList();

			if (!listingNodes.Any())
				return null;

			Dictionary<string, decimal> amazonListingPrices = new Dictionary<string, decimal>();

			foreach (XmlNode listingNode in listingNodes)
			{
				string asin = null;
				decimal? price = null;

				XmlNode asinElement = listingNode.GetChildXmlNode("ASIN");
				if (asinElement != null)
				{
					XmlNode asinElementInnerXml = asinElement.ChildNodes.Cast<XmlNode>().First();
					if (asinElementInnerXml != null)
						asin = asinElementInnerXml.InnerText;
				}

				XmlNode offersElement = listingNode.GetChildXmlNode("Offers");
				if (offersElement != null)
				{
					XmlNode offerElement = offersElement.GetChildXmlNode("Offer");
					if (offerElement != null)
					{
						XmlNode offerListingElement = offerElement.GetChildXmlNode("OfferListing");
						if (offerListingElement != null)
						{
							XmlNode priceElement = offerListingElement.GetChildXmlNode("Price");
							if (priceElement != null)
							{
								// Ensure that the CurrencyCode is correct.
								XmlNode currencyElement = priceElement.GetChildXmlNode("CurrencyCode");
								if (currencyElement != null)
								{
									if (currencyElement.InnerText != "USD")
										throw new InvalidOperationException("CurrencyCode not USD.");

									XmlNode formattedPriceElement = priceElement.GetChildXmlNode("FormattedPrice");
									if (formattedPriceElement != null)
									{
										string formattedPriceString = formattedPriceElement.InnerText.Replace("$", string.Empty);
										decimal formattedPrice;
										if (decimal.TryParse(formattedPriceString, out formattedPrice))
											price = formattedPrice;
										else
											throw new InvalidOperationException("FormattedPrice did not parse correctly.");
									}
								}
							}
						}
					}
				}

				if (asin != null && price.HasValue)
					amazonListingPrices.Add(asin, price.Value);
			}

			return amazonListingPrices;
		}
	}
}

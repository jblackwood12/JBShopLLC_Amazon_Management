using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AmazonProductLookup.AmazonApis.AdvApi.ItemLookup;
using Models;
using Utility;

namespace AmazonProductLookup.AmazonApis.AdvApi.ItemSearch
{
	public static class ItemSearchUtility
	{
		public static Product ReadItemSearchResponse(IEnumerable<XmlNode> nodes, string itemId)
		{
			Product product = new Product();

			List<ItemSearchProduct> itemSearchProducts = nodes.Select(s =>
				{
					int? totalNewAmount = null;
					int? salesRankAmount = null;
					string asin;

					List<XmlNode> itemElementNodes = s.ChildNodes.Cast<XmlNode>().ToList();

					itemElementNodes.GetValueFromChildNode("ASIN", out asin);

					int salesRank;
					if (itemElementNodes.GetValueFromChildNode("SalesRank", out salesRank))
						salesRankAmount = salesRank;

					XmlNode offerSummaryElement = s.GetChildXmlNode("OfferSummary");

					if (offerSummaryElement != null)
					{
						int totalNew;
						if (offerSummaryElement.GetValueFromChildNode("TotalNew", out totalNew))
							totalNewAmount = totalNew;
					}

					return new ItemSearchProduct(asin, totalNewAmount, salesRankAmount, s);
				})
				.OrderByDescending(o => o.TotalNew)
				.Where(w => w.Asin != null)
				.ToList();

			if (itemSearchProducts.Any())
			{
				var itemSearchProductsAndModels = itemSearchProducts
					.Select(s =>
					{
						string modelNumber = string.Empty;

						XmlNode itemAttributesNode = s.Node.GetChildXmlNode("ItemAttributes");

						if (itemAttributesNode != null)
						{
							if (itemAttributesNode.GetValueFromChildNode("Model", out modelNumber))
								product.ItemNumber = modelNumber;

							string title;
							if (itemAttributesNode.GetValueFromChildNode("Title", out title))
								product.Name = title;
						}

						return new { itemSearchProduct = s, modelNumber };
					})
					.ToList();

				// The first, most likely matched itemSearchProduct based on Model
				var selectedItemSearchProductAndModel = itemSearchProductsAndModels
					.Where(w => w.modelNumber != null)
					.FirstOrDefault(f => f.modelNumber.Equals(itemId, StringComparison.InvariantCultureIgnoreCase));

				ItemSearchProduct chosenProduct = selectedItemSearchProductAndModel != null
					? selectedItemSearchProductAndModel.itemSearchProduct
					: null;

				if (chosenProduct != null)
				{
					List<XmlNode> xmlNodes = new List<XmlNode> { chosenProduct.Node };

					XmlNode node = xmlNodes.FirstOrDefault();

					string asin;
					decimal length = 0;
					decimal width = 0;
					decimal height = 0;
					decimal weight = 0;

					node.GetValueFromChildNode("ASIN", out asin);

					XmlNode itemAttributesNode = node.GetChildXmlNode("ItemAttributes");

					if (itemAttributesNode != null)
					{
						// There are two different nodes for dimensions, <ItemDimensions> and <PackageDimensions>
						// PackageDimensions are the exact dimensions found on the FBA Item Details page.
						XmlNode packageDimensionsNode = itemAttributesNode.GetChildXmlNode("PackageDimensions");
						if (packageDimensionsNode != null)
						{
							List<XmlNode> dimensionNodes = packageDimensionsNode.ChildNodes.Cast<XmlNode>().ToList();

							if (dimensionNodes.Any())
							{
								dimensionNodes.GetValueFromChildNode("Length", out length, DimensionUtility.GetInchesConverter());
								dimensionNodes.GetValueFromChildNode("Width", out width, DimensionUtility.GetInchesConverter());
								dimensionNodes.GetValueFromChildNode("Height", out height, DimensionUtility.GetInchesConverter());
								dimensionNodes.GetValueFromChildNode("Weight", out weight, DimensionUtility.GetPoundsConverter());
							}
						}
					}

					product.ASIN = asin;
					product.Length = length;
					product.Width = width;
					product.Height = height;
					product.Weight = weight;
				}
			}

			return product;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace Utility.FeeStructure
{
	public static class FeeStructureUtility
	{
		public static decimal CalculateProfitMargin(DimensionContainer dimensionContainer, FeeCategory feeCategory, decimal lowestPrice, decimal purchasePrice, Func<decimal, decimal> funcShippingCost = null)
		{
			if (lowestPrice <= 0)
				throw new InvalidOperationException("lowestPrice must be greater than 0.");

			decimal breakEvenPrice = CalculateBreakEven(dimensionContainer, feeCategory, purchasePrice, funcShippingCost);

			decimal feePercentage = GetCategoryFee(feeCategory);

			return (((lowestPrice - breakEvenPrice) * (1 - feePercentage)) - (c_returnPercentage * lowestPrice)) / lowestPrice;
		}

		public static decimal CalculateBreakEven(DimensionContainer dimensionContainer, FeeCategory feeCategory, decimal purchasePrice, Func<decimal, decimal> funcShippingcost = null)
		{
			List<decimal> dimensions = new List<decimal>
				{
					dimensionContainer.Length,
					dimensionContainer.Width,
					dimensionContainer.Height,
				}
				.OrderByDescending(o => o)
				.ToList();

			decimal weight = dimensionContainer.Weight;

			decimal longestSide = dimensions[0];
			decimal medianSide = dimensions[1];
			decimal shortestSide = dimensions[2];

			ProductSizeTier productSizeTier = GetProductSize(longestSide, medianSide, shortestSide, weight);

			// Shipping Cost to Amazon (estimate) = 0.72 * max(0.25, product weight)
			decimal estimateShippingCostToAmazon = funcShippingcost != null
				? funcShippingcost.Invoke(weight)
				: c_costPerPoundToShip * Math.Max(0.25m, weight);

			// Cost to get to AMZN = Base Price + Shipping Cost to Amazon (estimate)
			decimal costToGetToAmazon = purchasePrice + estimateShippingCostToAmazon;

			// Average Storage Fee = 1.2 * (Width/12) * (Length/12) * (Height/12)
			// TODO: Update storage fee cost to update starting 3/1/2016: http://www.amazon.com/gp/help/customer/display.html?nodeId=201648210
			decimal averageStorageFee = 1.2m * (longestSide / 12) * (medianSide / 12) * (shortestSide / 12);

			// Need to use the Outbound Shipping Weight for calculating the weight fee.

			decimal dimensionalWeight = CalculateDimensionalWeight(longestSide, medianSide, shortestSide);

			decimal outboundShippingWeight = CalculateOutboundShippingWeight(weight, dimensionalWeight, productSizeTier);

			// Size Fee => Multiply flat fee * ProductSizeTier
			decimal sizeFee = GetSizeFee(productSizeTier);

			decimal weightFee = GetWeightFee(productSizeTier, outboundShippingWeight);

			decimal feeCategoryPercentage = GetCategoryFee(feeCategory);

			// TODO: Implement the Zero Fee Fulfillment. Which applies to Small Standard-Size and Large Standard-Size. Price > $300, means no fulfillment fees.
			// Break Even Price = (Cost to get to AMZN + Average Storage Fee + Size Fee + Weight Fee) / (1 - Category Fee Pct)
			return (costToGetToAmazon + averageStorageFee + sizeFee + weightFee) / (1 - feeCategoryPercentage);
		}

		// Category fees listed here: https://www.amazon.com/gp/help/customer/display.html?nodeId=1161240
		private static decimal GetCategoryFee(FeeCategory feeCategory)
		{
			decimal feePercentage = 0.15m;

			switch (feeCategory)
			{
				case FeeCategory.AmazonKindle:
					feePercentage = 0.15m;
					break;

				case FeeCategory.AutomotivePartsAndAccessories:
					feePercentage = 0.12m;
					break;

				case FeeCategory.BabyProducts:
					feePercentage = 0.15m;
					break;

				case FeeCategory.Books:
					feePercentage = 0.15m;
					break;

				case FeeCategory.CameraAndPhoto:
					feePercentage = 0.08m;
					break;

				case FeeCategory.ConsumerElectronics:
					feePercentage = 0.08m;
					break;

				case FeeCategory.ElectronicsAccessories:
					feePercentage = 0.15m;
					break;

				case FeeCategory.EntertainmentCollectibles:
					feePercentage = 0.20m;
					break;

				case FeeCategory.HomeAndGarden:
					feePercentage = 0.15m;
					break;

				case FeeCategory.IndustrialScientific:
					feePercentage = 0.12m;
					break;

				case FeeCategory.KindleAccessories:
					feePercentage = 0.25m;
					break;

				case FeeCategory.Music:
					feePercentage = 0.15m;
					break;

				case FeeCategory.MusicalInstruments:
					feePercentage = 0.15m;
					break;

				case FeeCategory.OfficeProducts:
					feePercentage = 0.15m;
					break;

				case FeeCategory.PersonalComputers:
					feePercentage = 0.06m;
					break;

				case FeeCategory.SoftwareAndComputerGames:
					feePercentage = 0.15m;
					break;

				case FeeCategory.SportingGoods:
					feePercentage = 0.15m;
					break;

				case FeeCategory.SportsCollectibles:
					feePercentage = 0.20m;
					break;

				case FeeCategory.TiresAndWheels:
					feePercentage = 0.10m;
					break;

				case FeeCategory.ToolsAndHomeImprovement:
					feePercentage = 0.12m;
					break;

				case FeeCategory.Toys:
					feePercentage = 0.15m;
					break;

				case FeeCategory.UnlockedCellPhones:
					feePercentage = 0.08m;
					break;

				case FeeCategory.VideoAndDvd:
					feePercentage = 0.15m;
					break;

				case FeeCategory.VideoGameConsoles:
					feePercentage = 0.08m;
					break;

				case FeeCategory.Watches:
					feePercentage = 0.15m;
					break;

				case FeeCategory.AnyOtherProducts:
					feePercentage = 0.15m;
					break;
			}

			return feePercentage;
		}

		// Calculate the "Product Size Tier"
		// https://www.amazon.com/gp/help/customer/display.html?nodeId=201119390
		private static ProductSizeTier GetProductSize(decimal longestSide, decimal medianSide, decimal shortestSide, decimal weight)
		{
			ProductSizeTier productSizeTier;

			decimal girth = 2 * (medianSide + shortestSide);

			decimal lengthPlusGirth = girth + longestSide;

			if (longestSide <= 15 && medianSide <= 12 && shortestSide <= 0.75m && weight <= (12 / 16))
				productSizeTier = ProductSizeTier.SmallStandardSize;
			else if (longestSide <= 18 && medianSide <= 14 && shortestSide <= 8 && weight <= 20)
				productSizeTier = ProductSizeTier.LargeStandardSize;
			else if (longestSide <= 60 && medianSide <= 30 && lengthPlusGirth <= 130 && weight <= 70)
				productSizeTier = ProductSizeTier.SmallOversize;
			else if (longestSide <= 108 && lengthPlusGirth <= 130 && weight <= 150)
				productSizeTier = ProductSizeTier.MediumOversize;
			else if (longestSide <= 108 && lengthPlusGirth <= 165 && weight <= 150)
				productSizeTier = ProductSizeTier.LargeOversize;
			else
				productSizeTier = ProductSizeTier.SpecialOversize;

			return productSizeTier;
		}

		// Follow this documentation for fulfillment fees.
		// http://www.amazon.com/gp/help/customer/display.html?nodeId=201648210
		private static decimal GetSizeFee(ProductSizeTier productSizeTier)
		{
			decimal sizeFee;

			// Each sizeFee contains both the "Order Handling" and "Pick & Pack per unit"
			switch (productSizeTier)
			{
				case ProductSizeTier.SmallStandardSize:
					sizeFee = 2.06m;
					break;

				case ProductSizeTier.LargeStandardSize:
					sizeFee = 2.06m;
					break;

				case ProductSizeTier.SmallOversize:
					sizeFee = 4.09m;
					break;

				case ProductSizeTier.MediumOversize:
					sizeFee = 5.20m;
					break;

				case ProductSizeTier.LargeOversize:
					sizeFee = 8.40m;
					break;

				case ProductSizeTier.SpecialOversize:
					sizeFee = 10.53m;
					break;

				default:
					sizeFee = 2.06m;
					break;
			}

			return sizeFee;
		}

		private static decimal GetWeightFee(ProductSizeTier productSizeTier, decimal weight)
		{
			decimal weightFee = 0.00m;

			decimal weightCeiling = Math.Ceiling(weight);

			switch (productSizeTier)
			{
				case ProductSizeTier.SmallStandardSize:
					weightFee = weightCeiling * 0.50m;
					break;

				case ProductSizeTier.LargeStandardSize:
					if (weightCeiling <= 1)
					{
						weightFee = 0.96m;
					}
					else
					{
						weightFee = 1.95m;

						if (weightCeiling > 2)
							weightFee += (weightCeiling - 2) * 0.39m;
					}

					break;

				case ProductSizeTier.SmallOversize:
					weightFee = 2.06m;

					if (weightCeiling > 2)
						weightFee += (weightCeiling - 2) * 0.39m;
					break;

				case ProductSizeTier.MediumOversize:
					weightFee = 2.73m;

					if (weightCeiling > 2)
						weightFee += (weightCeiling - 2) * 0.39m;
					break;

				case ProductSizeTier.LargeOversize:
					weightFee = 63.98m;

					if (weightCeiling > 90)
						weightFee += (weightCeiling - 90) * 0.80m;
					break;

				case ProductSizeTier.SpecialOversize:
					weightFee = 124.58m;

					if (weightCeiling > 90)
						weightFee += (weightCeiling - 90) * 0.92m;
					break;
			}

			return weightFee;
		}

		private static decimal CalculateDimensionalWeight(decimal length, decimal width, decimal height)
		{
			return (length * width * height) / 166;
		}

		// How to calculate the Outbound Shipping Weight for the Weight Handling fee
		// This is used to calculate the Weight Handling fee.
		// https://www.amazon.com/gp/help/customer/display.html/ref=help_search_1-1?ie=UTF8&nodeId=201119410&qid=1448996314&sr=1-1#calc
		private static decimal CalculateOutboundShippingWeight(decimal unitWeight, decimal dimensionalWeight, ProductSizeTier productSizeTier)
		{
			decimal outboundShippingWeight;
			decimal packagingWeight;

			if (productSizeTier == ProductSizeTier.SmallStandardSize || productSizeTier == ProductSizeTier.LargeStandardSize)
			{
				packagingWeight = 0.25m;
			}
			else if (productSizeTier == ProductSizeTier.SmallOversize || productSizeTier == ProductSizeTier.MediumOversize ||
					productSizeTier == ProductSizeTier.LargeOversize || productSizeTier == ProductSizeTier.SpecialOversize)
			{
				packagingWeight = 1.00m;
			}
			else
			{
				// Default to ensure a value is set.
				packagingWeight = 1.00m;
			}

			if (productSizeTier == ProductSizeTier.SmallStandardSize || productSizeTier == ProductSizeTier.LargeStandardSize)
			{
				if (unitWeight <= 1)
				{
					// 1 lb. or less.
					outboundShippingWeight = unitWeight + packagingWeight;
				}
				else
				{
					// more than 1 lb.
					outboundShippingWeight = Math.Max(unitWeight, dimensionalWeight) + packagingWeight;
				}
				
			}else if (productSizeTier == ProductSizeTier.SmallOversize || productSizeTier == ProductSizeTier.MediumOversize || productSizeTier == ProductSizeTier.LargeOversize)
			{
				outboundShippingWeight = Math.Max(unitWeight, dimensionalWeight) + packagingWeight;
			}
			else if (productSizeTier == ProductSizeTier.SpecialOversize)
			{
				outboundShippingWeight = unitWeight + packagingWeight;
			}
			else
			{
				// Default.
				outboundShippingWeight = Math.Max(unitWeight, dimensionalWeight) + packagingWeight;
			}

			// Round up to the nearest whole pound.
			return Math.Ceiling(outboundShippingWeight);
		}

		private const decimal c_returnPercentage = 0.03m;

		private const decimal c_costPerPoundToShip = 0.72m;
	}
}

using System;

namespace Data.Mappers
{
	internal static class ProductMapper
	{
		internal static Models.Product MapProductToModel(this Product product)
		{
			return new Models.Product
				{
					ProductId = product.ProductId,
					CreatedDate = DateTime.SpecifyKind(product.CreatedDate, DateTimeKind.Utc),
					ModifiedDate = DateTime.SpecifyKind(product.ModifiedDate, DateTimeKind.Utc),
					UPC = product.UPC,
					Name = product.Name,
					Cost = product.Cost,
					PromotionCost = product.PromotionCost,
					MAPPrice = product.MAPPrice,
					MinPrice = product.MinPrice,
					MaxPrice = product.MaxPrice,
					BreakevenPrice = product.BreakevenPrice,
					OverrideCost = product.OverrideCost,
					QuantityInCase = product.QuantityInCase,
					ASIN = product.ASIN,
					SKU = product.SKU,
					ItemNumber = product.ItemNumber,
					ManufacturerId = product.ManufacturerId,
					Length = product.Length,
					Width = product.Width,
					Height = product.Height,
					Weight = product.Weight,
					IsMAP = product.IsMAP,
					IsDiscontinued = product.IsDiscontinued,
					Notes = product.Notes
				};
		}

		internal static Product MapProductToData(this Models.Product product)
		{
			return new Product
				{
					ProductId = product.ProductId ?? 0,
					CreatedDate = product.CreatedDate,
					ModifiedDate = product.ModifiedDate,
					UPC = product.UPC,
					Name = product.Name,
					Cost = product.Cost,
					PromotionCost = product.PromotionCost,
					MAPPrice = product.MAPPrice,
					MinPrice = product.MinPrice,
					MaxPrice = product.MaxPrice,
					BreakevenPrice = product.BreakevenPrice,
					OverrideCost = product.OverrideCost,
					QuantityInCase = product.QuantityInCase,
					ASIN = product.ASIN,
					SKU = product.SKU,
					ItemNumber = product.ItemNumber,
					ManufacturerId = product.ManufacturerId,
					Length = product.Length,
					Width = product.Width,
					Height = product.Height,
					Weight = product.Weight,
					IsMAP = product.IsMAP,
					IsDiscontinued = product.IsDiscontinued,
					Notes = product.Notes
				};
		}
	}
}

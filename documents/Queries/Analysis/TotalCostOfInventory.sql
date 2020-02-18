WITH DistinctItems AS (
						SELECT AllItems.sku,
								AllItems.asin,
								AllItems.[product-name],
								AllItems.[afn-warehouse-quantity],
								AllItems.[afn-fulfillable-quantity],
								AllItems.[afn-unsellable-quantity],
								AllItems.[afn-reserved-quantity]
							FROM (SELECT *,
										ROW_NUMBER() OVER (PARTITION BY uia.asin ORDER BY uia.[your-price] ASC) as RowNumber
									FROM AmazonMWS.dbo.UnsuppressedInventoryData as uia
									WHERE Date = (SELECT MAX(Date)
													FROM AmazonMWS.dbo.UnsuppressedInventoryData)
															AND [afn-warehouse-quantity] > 0) AllItems
						WHERE AllItems.RowNumber = 1
						), ProductCosts AS
						(
							SELECT di.asin,
									di.sku,
									di.[product-name],
									di.[afn-fulfillable-quantity],
									p.Cost,
									(di.[afn-fulfillable-quantity] * COALESCE(p.OverrideCost, p.PromotionCost, ((1 - COALESCE(md.DiscountPercentage, 0)) * p.Cost))) as TotalInventoryCost
							FROM DistinctItems as di
							JOIN AmazonMWS.dbo.Products as p on p.ASIN = di.asin
						LEFT JOIN AmazonMWS.dbo.ManufacturerDiscounts md on md.ManufacturerId = p.ManufacturerId
							WHERE di.[afn-fulfillable-quantity] > 0
						)

							SELECT SUM(TotalInventoryCost)
							FROM ProductCosts
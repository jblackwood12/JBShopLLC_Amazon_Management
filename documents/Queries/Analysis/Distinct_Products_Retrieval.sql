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
						)

SELECT *
FROM DistinctItems
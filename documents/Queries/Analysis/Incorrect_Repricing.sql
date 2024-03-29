WITH DistinctItemsInStock AS (
SELECT AllItems.sku,
       AllItems.asin,
	   AllItems.[product-name],
	   AllItems.[afn-warehouse-quantity],
	   AllItems.[afn-fulfillable-quantity],
	   AllItems.[afn-unsellable-quantity],
	   AllItems.[afn-reserved-quantity]
FROM(SELECT *,
	       ROW_NUMBER() OVER (PARTITION BY uia.asin ORDER BY uia.[your-price] ASC) as RowNumber
       FROM dbo.UnsuppressedInventoryData as uia
      WHERE Date = (SELECT MAX(Date)
					  FROM dbo.UnsuppressedInventoryData)
			AND [afn-warehouse-quantity] > 0) AllItems
WHERE AllItems.RowNumber = 1
), MostRecentPriceHistory AS (
SELECT *
FROM(
SELECT ph.ASIN,
	   ph.NewPrice,
	   ROW_NUMBER() OVER (PARTITION BY ph.ASIN ORDER BY ph.TimeStamp DESC) as RowNumber
  FROM AmazonMWS.dbo.PriceHistory ph) MostRecentPrices
WHERE MostRecentPrices.RowNumber = 1
)

SELECT uid.*,
       ri.MinimumPrice,
	   p.Cost,
	   p.Cost * uid.[afn-fulfillable-quantity] as TotalCost,
	   mrph.NewPrice
  FROM DistinctItemsInStock uid
  JOIN dbo.RepricingInformation ri on ri.ASIN = uid.ASIN
LEFT JOIN dbo.Products p on p.ASIN = uid.ASIN
LEFT JOIN MostRecentPriceHistory mrph on mrph.ASIN = uid.ASIN
WHERE mrph.NewPrice > (ri.MinimumPrice * 1.6)
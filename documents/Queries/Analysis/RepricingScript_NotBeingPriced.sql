-- Find products that are in the dbo.RepricingInformation table, but no rows in the dbo.PriceHistory are being generated.
WITH RecentRepricingInformation AS (
SELECT ph.ASIN,
	   COUNT(*) as CountReprices 
  FROM AmazonMWS.dbo.PriceHistory ph
 WHERE TimeStamp > '2014-04-01'
 GROUP BY ph.ASIN
), CurrentInventory AS (
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
 WHERE Date = (SELECT MAX(uia2.Date) from dbo.UnsuppressedInventoryData uia2)
	   AND [afn-fulfillable-quantity] > 0) AllItems
WHERE AllItems.RowNumber = 1
	  AND NOT EXISTS (SELECT 'x' FROM dbo.Products p WHERE p.ASIN = AllItems.ASIN AND p.IsMap = 1)
)

SELECT ci.*,
	   rri.CountReprices
  FROM CurrentInventory ci
  LEFT JOIN RecentRepricingInformation rri on rri.ASIN = ci.ASIN
 WHERE rri.CountReprices IS NULL
  ORDER BY rri.CountReprices DESC
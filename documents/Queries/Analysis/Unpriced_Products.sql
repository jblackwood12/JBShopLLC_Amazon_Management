Use AmazonMWS;

WITH DistinctItems AS (
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
	  AND
	  ((NOT EXISTS (SELECT 'x'
						FROM dbo.RepricingInformation ri
					   WHERE ri.ASIN = AllItems.ASIN)

		   AND

		  NOT EXISTS (SELECT 'x'
	                    FROM dbo.Products p
					   WHERE p.ASIN = AllItems.ASIN
						     AND (p.MAPPrice IS NOT NULL OR p.IsMAP = 1))))
)

  SELECT *
    FROM DistinctItems as di
ORDER BY di.[afn-warehouse-quantity] DESC

SELECT *
FROM(
SELECT ASIN,
	   [product-name],
	   [afn-fulfillable-quantity],
       ROW_NUMBER() OVER (PARTITION BY uid.asin ORDER BY uid.[your-price] ASC) as RowNumber
  FROM [AmazonMWS].[dbo].[UnsuppressedInventoryData] uid
  WHERE Date = (SELECT MAX(date) FROM AmazonMWS.dbo.UnsuppressedInventoryData uid)
        AND NOT EXISTS (SELECT 'x' FROM AmazonMWS.dbo.Products p WHERE p.ASIN = uid.ASIN)
		AND uid.[afn-fulfillable-quantity] > 0) MissingProducts
		WHERE MissingProducts.RowNumber = 1

-- BELOW IS A SAMPLE INSERT STATEMENT

--INSERT INTO dbo.Products
--VALUES (GETDATE(), GETDATE(), NULL, 'Wells Lamont 7670XL All-Purpose ATV and Motorcycle Glove, X-Large', 10, NULL, NULL, NULL, NULL, 'B00622WF7G', '2A-Z01I-00MG', '7670XL', 17, 10.6, 4.2, 1, 0.2, 0, 0)
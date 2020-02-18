SELECT *
FROM(SELECT *,
	       ROW_NUMBER() OVER (PARTITION BY uia.asin ORDER BY uia.[your-price] ASC) as RowNumber
       FROM dbo.UnsuppressedInventoryData as uia
      WHERE Date = '2013-07-14' AND [afn-warehouse-quantity] > 0 AND [afn-listing-exists] = 'Yes') AllItems
WHERE AllItems.RowNumber != 1

-- Duplicate SKUs for ASIN removed:

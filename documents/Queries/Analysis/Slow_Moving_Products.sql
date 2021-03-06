DECLARE @BeginDate DateTime = DATEADD(dd, -90, GETDATE());
DECLARE @EndDate DateTime = GETDATE();

WITH MostRecentInventory AS (
SELECT *
  FROM(SELECT *,
			  ROW_NUMBER() OVER (PARTITION BY uid.asin ORDER BY uid.[your-price] ASC) as RowNumber
		 FROM dbo.UnsuppressedInventoryData uid
	    WHERE uid.Date = (SELECT MAX(uid2.Date) FROM dbo.UnsuppressedInventoryData uid2)
			  AND uid.[afn-fulfillable-quantity] > 0
			  AND uid.[your-price] IS NOT NULL) AllProducts
WHERE AllProducts.RowNumber = 1
), OrdersDuringPeriod AS (
SELECT aod.asin,
	   SUM(aod.quantity) as TotalQuantitySold
  FROM dbo.AllOrdersData aod
 WHERE CAST(aod.[purchase-date] as Date) >= @BeginDate
	   AND CAST(aod.[purchase-date] as Date) <= @EndDate
	   AND aod.[order-status] != 'Cancelled'
GROUP BY aod.asin
), PriceHistory AS (
SELECT ph.ASIN,
       COUNT(*) as NumberOfPriceChanges
  FROM dbo.PriceHistory ph
 WHERE CAST(ph.TimeStamp as Date) >= @BeginDate
	   AND CAST(ph.TimeStamp as Date) <= @EndDate
GROUP BY ph.ASIN
), DistinctRepricingInformation AS (
SELECT DISTINCT ri.ASIN, ri.SKU
  FROM dbo.RepricingInformation ri
)

   SELECT mri.[product-name],
		  mri.[asin],
		  mri.[sku],
		  mri.[afn-fulfillable-quantity],
		  mri.[your-price],
		  odp.TotalQuantitySold,
		  ph.NumberOfPriceChanges,
		  CASE WHEN dri.ASIN IS NULL THEN 0 ELSE 1 END AS IsRepricingInformation
     FROM MostRecentInventory mri
LEFT JOIN OrdersDuringPeriod odp on odp.asin = mri.asin
LEFT JOIN PriceHistory ph on ph.ASIN = mri.asin
LEFT JOIN DistinctRepricingInformation dri on dri.ASIN = mri.asin
ORDER BY odp.TotalQuantitySold ASC
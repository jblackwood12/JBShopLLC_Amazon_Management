SELECT ASIN, SUM(quantity) as TotalSold90Days
  FROM [AmazonMWS].[dbo].[AllOrdersData] aod
WHERE CAST(aod.[purchase-date] as Date) >= DATEADD(DAY, -90, getdate())
GROUP BY ASIN
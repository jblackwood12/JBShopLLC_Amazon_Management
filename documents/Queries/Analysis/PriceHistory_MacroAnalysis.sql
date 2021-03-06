WITH PriceUpdates AS (
	SELECT [PriceHistoryId]
      ,[NewPrice]
      ,[TimeStamp]
      ,[ASIN]
      ,[ExistingPrice]
      ,[SellerTypeIdWinningBuyBox]
      ,[TimeToUpdateInMS]
	  ,st.Name
  FROM [AmazonMWS].[dbo].[PriceHistory] ph
  LEFT JOIN [AmazonMWS].[dbo].[SellerType] st on st.SellerTypeId = ph.SellerTypeIdWinningBuyBox
), FirstPriceUpdates AS (
SELECT *
FROM PriceUpdates pu
WHERE PriceHistoryId >= 11367599
  AND PriceHistoryId <= 11367933
), MostRecentSubsequentUpdates AS (
SELECT *
FROM(
SELECT pu.ASIN,
       ROW_NUMBER() OVER(PARTITION BY pu.ASIN ORDER BY pu.TimeStamp DESC) as RowNumber,
	   pu.Name
  FROM PriceUpdates pu
 WHERE pu.PriceHistoryId > 11367933) LastPriceUpdates
 WHERE LastPriceUpdates.RowNumber = 1
)

-- Around 2:40pm (First Set)
-- Amazon	193
-- JB Shop	108
-- Other	34

SELECT NAME, Count(*)
FROM MostRecentSubsequentUpdates
GROUP BY Name


-- Around 3:10pm (Subsequent)
--Amazon	180
--JB Shop	123 
--Other		32

-- Around 3:14 pm (Subsequent)
--NULL		1
--Amazon	178
--JB Shop	126
--Other		30


-- Percentage of the time we are winning the buy box where our new price is less than or equal to our existing price. 
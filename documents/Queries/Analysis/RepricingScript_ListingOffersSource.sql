WITH MostRecentPriceHistoryForAsin AS (
SELECT MostRecentPrices.ASIN,
    MostRecentPrices.BreakEvenPrice,
    MostRecentPrices.NewPrice,
    MostRecentPrices.ListingOffersSource,
    MostRecentPrices.LastNotificationPublishDateTime,
    MostRecentPrices.TimeStamp
FROM(
SELECT ph.ASIN,
       ph.BreakEvenPrice,
    ph.NewPrice,
    ph.ListingOffersSource,
    ph.LastNotificationPublishDateTime,
    ph.TimeStamp,
    ROW_NUMBER() OVER (PARTITION BY ph.ASIN ORDER BY ph.TimeStamp DESC) as RowNumber
  FROM [AmazonMWS].[dbo].[PriceHistory] ph WITH (NOLOCK)) MostRecentPrices
  WHERE MostRecentPrices.RowNumber = 1
)

SELECT *
  FROM MostRecentPriceHistoryForAsin
ORDER BY ListingOffersSource DESC
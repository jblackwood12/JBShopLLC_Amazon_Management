SELECT *
FROM(
SELECT [NotificationLogId]
      ,[ASIN]
      ,[PublishDateTime]
      ,[LowestFbaPrice]
      ,[LowestNonFbaPrice]
      ,[OurPrice]
      ,[ListingOffersSource]
      ,[BuyBoxWinnerSellerType]
      ,[BuyBoxPrice]
	  , ROW_NUMBER() OVER (PARTITION BY ASIN ORDER BY PublishDateTime DESC) as RowNumber
  FROM [AmazonMWS].[dbo].[ListingOffersLog] lol
  WHERE ListingOffersSource != 'MwsProducts'
	    AND NOT EXISTS (SELECT 'x' FROM AmazonMWS.dbo.RepricingInformation ri WHERE ri.ASIN = lol.ASIN)) MissingNotifications
WHERE MissingNotifications.RowNumber = 1
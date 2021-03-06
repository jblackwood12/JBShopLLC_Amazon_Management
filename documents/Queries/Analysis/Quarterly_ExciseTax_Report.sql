DECLARE @BeginDate DateTime = '2015-10-01';
DECLARE @EndDate DateTime = '2015-12-31';

WITH Orders AS (
SELECT CAST(aod.[purchase-date] as Date) as PurchaseDate,
       aod.[item-price] as ItemPrice,
	   aod.[item-tax] as ItemTax,
	   aod.[ship-state] as ShipState,
	   aod.[ship-country] as ShipCountry,
	   aod.[ship-city] as ShipCity,
	   aod.[ship-postal-code] as ShipPostalCode
  FROM [AmazonMWS].[dbo].[AllOrdersData] aod
  WHERE CAST(aod.[purchase-date] as Date) >= @BeginDate
	    AND CAST(aod.[purchase-date]  as Date) <= @EndDate
		AND aod.[item-status] != 'Cancelled'
		AND aod.[order-status] != 'Cancelled'
), LocalSalesTaxCollected AS (
SELECT SUM(ItemTax) as TotalTaxCollected
FROM Orders o
WHERE (o.ShipState = 'WA' OR o.ShipState = 'WASHINGTON')
	  AND o.ShipCountry = 'US'
), LocalSales AS (
SELECT SUM(ItemPrice) as TotalSales
  FROM Orders o
 WHERE (o.ShipState = 'WA' OR o.ShipState = 'WASHINGTON')
       AND o.ShipCountry = 'US'
), UnitedStatesSales AS (
SELECT SUM(ItemPrice) as TotalSales
  FROM Orders o
 WHERE o.ShipCountry = 'US'
), InternationalSales AS (
SELECT SUM(ItemPrice) as TotalSales
  FROM Orders o
 WHERE o.ShipCountry != 'US'
), LocalSalesByCityCode AS (
SELECT o.ShipCity,
	   SUM(ItemPrice) as TotalSales
  FROM Orders o
 WHERE o.ShipCountry = 'US'
	   AND (o.ShipState = 'WA' or o.ShipState = 'WASHINGTON')
GROUP BY o.ShipCity
)

--SELECT lstc.TotalTaxCollected, 'Sales tax collected (Washington State)'
--FROM LocalSalesTaxCollected lstc

--UNION

--SELECT ls.TotalSales, 'Local Sales (Washington State)'
--FROM LocalSales ls

--UNION

--SELECT uss.TotalSales, 'United States Sales (includes local sales)'
--FROM UnitedStatesSales uss

--UNION

--SELECT i.TotalSales, 'International Sales (excludes sales in the U.S.)'
--FROM InternationalSales i


-- --We need to report on local sales by city.
SELECT *
  FROM LocalSalesByCityCode
ORDER BY TotalSales DESC

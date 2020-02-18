SELECT [SKU]
      ,[ASIN]
      ,[MinimumPrice]
      ,[ProductName]
      ,[IsCurrent]
  FROM dbo.RepricingInformation
  WHERE ASIN = ''

  UPDATE ri
  SET ri.SKU = ''
  FROM dbo.RepricingInformation ri
  WHERE ri.ASIN = ''


  -- SKU, ASIN, MinimumPrice, ProductName, IsCurrent
  INSERT INTO AmazonMWS.dbo.RepricingInformation
  VALUES ('', '', 0, '', 1)
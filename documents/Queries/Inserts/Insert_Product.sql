  INSERT INTO dbo.Products
  (CreatedDate,
  ModifiedDate,
  UPC,
  Name,
  Price,
  PromotionPrice,
  MAPPrice,
  OverridePrice,
  QuantityInCase,
  ASIN,
  SKU,
  ItemNumber,
  ManufacturerId,
  Length,
  Width,
  Height,
  Weight,
  IsMAP,
  IsDiscontinued)
    
  VALUES (
  GETDATE(),																						-- CreatedDate
  GETDATE(),																						-- ModifiedDate
  '',																					-- UPC
  '',					-- Name
  ,																								-- Price
  NULL,																								-- PromotionPrice
  ,																							-- MAPPrice
  NULL,																								-- OverridePrice
  ,																								-- QuantityInCase
  NULL,																						-- ASIN
  NULL,																					-- SKU
  '',																							-- ItemNumber
  9,																								-- ManufacturerId
  ,																								-- Length
  ,																								-- Width
  ,																								-- Height
  ,																								-- Weight
  1,																								-- IsMAP
  0)																								-- IsDiscontinued
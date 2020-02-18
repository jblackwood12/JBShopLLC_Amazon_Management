using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System;
using Dapper;
using Data.DataModels;
using Data.Mappers;
using Data.Mappers.Reports;
using Models;
using MoreLinq;
using Utility.Models.Date;

namespace Data
{
	public class AmazonMWSdbService : IAmazonMWSdbService
	{
		public AmazonMWSdbService()
		{
			m_amazonMWSdbDataContext = new AmazonMWSdbDataContext();
		}

		public AmazonMWSdb GetContextInstance()
		{
			return new AmazonMWSdb();
		}

		public List<UnpricedProduct> GetUnpricedProducts()
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<UnpricedProduct>(
					@"Use AmazonMWS;

					WITH DistinctItems AS (
					SELECT AllItems.sku,
						   AllItems.asin,
						   AllItems.[product-name],
						   AllItems.[afn-warehouse-quantity],
						   AllItems.[afn-fulfillable-quantity],
						   AllItems.[afn-unsellable-quantity],
						   AllItems.[afn-reserved-quantity],
						   AllItems.[condition]
					FROM(SELECT uia.*,
							   ROW_NUMBER() OVER (PARTITION BY uia.asin ORDER BY uia.[your-price] ASC) as RowNumber
						   FROM dbo.UnsuppressedInventoryData as uia
						  WHERE Date = (SELECT MAX(Date)
										  FROM dbo.UnsuppressedInventoryData)
								AND [afn-warehouse-quantity] > 0
								AND condition = 'new') AllItems
					WHERE AllItems.RowNumber = 1
					), DistinctItemsExcludingMAP AS (
					SELECT di.*
					  FROM DistinctItems di
					 WHERE NOT EXISTS (SELECT 'x'
										 FROM dbo.Products p
										WHERE p.ASIN = di.ASIN
											  AND (p.MAPPrice IS NOT NULL OR p.IsMAP = 1))
					), MissingRepricingInformation AS (
					SELECT di.*
					FROM DistinctItemsExcludingMAP di
					WHERE NOT EXISTS (SELECT 'x' FROM dbo.RepricingInformation ri WHERE ri.ASIN = di.ASIN AND ri.SKU = di.SKU)
					)

					SELECT mri.sku,
						   mri.asin,
						   mri.[product-name] as ProductName,
						   mri.[afn-warehouse-quantity] as WarehouseQuantity,
						   mri.[afn-fulfillable-quantity] as FulfillableQuantity
					FROM MissingRepricingInformation mri
					")
					.OrderByDescending(o => o.FulfillableQuantity)
					.ToList();
			}
		}

		public List<DataPoint> GetSalesByDay(DateTime beginDate, DateTime endDate)
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<DataPoint>(
				@"SELECT CAST(aod.[purchase-date] as Date) as DateTime,
					   SUM(aod.[item-price]) as Value,
					   'Amount' as SeriesName
				  FROM dbo.AllOrdersData aod
				WHERE aod.[order-status] != 'Cancelled'
					  AND CAST(aod.[purchase-date] as Date) >= @BeginDate
					  AND CAST(aod.[purchase-date] as Date) <= @EndDate
				GROUP BY CAST(aod.[purchase-date] as Date)",
				new
				{
					BeginDate = beginDate,
					EndDate = endDate
				})
				.ToList();
			}
		}

		public List<ProductSearchResult> SearchForProducts(string query)
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<ProductSearchResult>(
					@"SELECT ProductId,
							 UPC,
							 Name,
							 ASIN,
							 SKU,
							 ItemNumber
						FROM dbo.Products p
					   WHERE p.Name like '%' + @Query + '%'
							 OR ItemNumber like '%' + @Query + '%'
							 OR UPC like '%' + @Query + '%'
							 OR ASIN like '%' + @Query + '%'
							 OR SKU like '%' + @Query + '%'",
					new { Query = query })
					.ToList();
			}
		}

		public decimal? GetCostOfProductForAsin(string asin)
		{
			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateReadInstance())
			{
				Product item = dc.Products.FirstOrDefault(w => w.ASIN == asin);

				if (item == null)
					return null;

				return item.Cost;
			}
		}

		public Models.Product GetProduct(string asin)
		{
			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateReadInstance())
			{
				return dc.Products
					.FirstOrDefault(f => f.ASIN == asin)
					.MapProductToModel();
			}
		}

		public Models.Product GetProduct(long productId)
		{
			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateReadInstance())
			{
				return dc.Products
					.FirstOrDefault(f => f.ProductId == productId)
					.MapProductToModel();
			}
		}

		public Models.Product EditProduct(Models.Product product)
		{
			DateTime modifiedDate = DateTime.UtcNow;

			Product dataProduct = product.MapProductToData();

			Product productToReturn;

			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateWriteInstance())
			{
				Product databaseProduct = null;

				// Don't look up the product if we are attempting to create it.
				if (product.ProductId.HasValue)
					databaseProduct = dc.Products.SingleOrDefault(f => f.ProductId == product.ProductId);

				if (databaseProduct != null)
				{
					// ModifiedDate is updated, CreatedDate is not changed.
					databaseProduct.ModifiedDate = modifiedDate;
					databaseProduct.UPC = dataProduct.UPC != null ? dataProduct.UPC.Trim() : dataProduct.UPC;
					databaseProduct.Name = dataProduct.Name != null ? dataProduct.Name.Trim() : dataProduct.Name;
					databaseProduct.Cost = dataProduct.Cost;
					databaseProduct.PromotionCost = dataProduct.PromotionCost;
					databaseProduct.MAPPrice = dataProduct.MAPPrice;
					databaseProduct.MinPrice = dataProduct.MinPrice;
					databaseProduct.MaxPrice = dataProduct.MaxPrice;
					databaseProduct.BreakevenPrice = dataProduct.BreakevenPrice;
					databaseProduct.OverrideCost = dataProduct.OverrideCost;
					databaseProduct.QuantityInCase = dataProduct.QuantityInCase;
					databaseProduct.ASIN = dataProduct.ASIN != null ? dataProduct.ASIN.Trim() : dataProduct.ASIN;
					databaseProduct.SKU = dataProduct.SKU != null ? dataProduct.SKU.Trim() : dataProduct.SKU;
					databaseProduct.ItemNumber = dataProduct.ItemNumber != null ? dataProduct.ItemNumber.Trim() : dataProduct.ItemNumber;
					databaseProduct.ManufacturerId = dataProduct.ManufacturerId;
					databaseProduct.Length = dataProduct.Length;
					databaseProduct.Width = dataProduct.Width;
					databaseProduct.Height = dataProduct.Height;
					databaseProduct.Weight = dataProduct.Weight;
					databaseProduct.IsMAP = dataProduct.IsMAP;
					databaseProduct.IsDiscontinued = dataProduct.IsDiscontinued;
					databaseProduct.Notes = dataProduct.Notes;

					productToReturn = databaseProduct;
				}
				else
				{
					DateTime today = DateTime.UtcNow;

					dataProduct.CreatedDate = today;
					dataProduct.ModifiedDate = today;

					// If the product doesn't already exist in the database, create it.
					dc.Products.InsertOnSubmit(dataProduct);

					productToReturn = dataProduct;
				}

				dc.SubmitChanges();
			}

			return productToReturn.MapProductToModel();
		}

		public List<DataPoint> GetPriceHistories(string asin, DateTime beginDate, DateTime endDate)
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<DataPoint>(
					@"SELECT ph.TimeStamp as DateTime,
							 ph.NewPrice as Value,
							 'Price' as SeriesName
						FROM [AmazonMWS].[dbo].[PriceHistory] ph
					   WHERE ph.ASIN = @ASIN
							 AND CAST(ph.TimeStamp as Date) >= @BeginDate",
					new
					{
						BeginDate = beginDate,
						EndDate = endDate,
						ASIN = asin
					})
					.ToList();
			}
		}

		public PriceHistory GetMostRecentAsinPrice(string asin)
		{
			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateReadInstance())
			{
				return dc.PriceHistories
					.Where(w => w.ASIN == asin)
					.OrderByDescending(w => w.TimeStamp)
					.FirstOrDefault();
			}
		}

		public void InsertPriceHistory(List<PriceHistory> priceHistory)
		{
			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateWriteInstance())
			{
				foreach (IEnumerable<PriceHistory> batchedPriceHistory in priceHistory.Batch(c_defaultBatchAmount))
				{
					dc.PriceHistories.InsertAllOnSubmit(batchedPriceHistory);
					dc.SubmitChanges();
				}
			}
		}

		public void InsertListingOffersLog(List<ListingOffersLog> listingOffersLogs)
		{
			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateWriteInstance())
			{
				foreach (IEnumerable<ListingOffersLog> batchedListingOffersLogs in listingOffersLogs.Batch(c_defaultBatchAmount))
				{
					dc.ListingOffersLogs.InsertAllOnSubmit(batchedListingOffersLogs);
					dc.SubmitChanges();
				}
			}
		}

		public Dictionary<string, PriceHistory> GetLatestPriceHistoryForToday()
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<PriceHistory>(
					@"SELECT ph.PriceHistoryId, ph.NewPrice, ph.[Timestamp], ASIN
						FROM
							(SELECT PriceHistoryId,
									NewPrice,
									[Timestamp],
									ASIN,
									ROW_NUMBER() over (partition by ASIN order by PriceHistoryId desc) rank
							FROM AmazonMWS.dbo.PriceHistory
							WHERE [TimeStamp] > DATEADD(day, -1, GETDATE())) ph
						WHERE ph.[rank] = 1")
						.ToDictionary(x => x.ASIN, y => y);
			}
		}

		public void AuditLoginAttempt(string username, bool isSuccessfulLogin)
		{
			DateTime today = DateTime.UtcNow;

			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				conn.Execute(
					@"INSERT INTO dbo.LoginAttempts
					  VALUES (@Username, @LoginAttemptDateTime, @IsSuccessful)",
					new
					{
						Username = username,
						LoginAttemptDateTime = today,
						IsSuccessful = isSuccessfulLogin
					});
			}
		}

		public string InsertUnsuppressedInventoryData(IEnumerable<UnsuppressedInventoryDto> unsuppressedInventoryDtos)
		{
			DateTime today = DateTime.UtcNow.Date;

			List<UnsuppressedInventoryData> unsuppressedInventoryDatas = unsuppressedInventoryDtos
				.Select(s => s.Map())
				.ToList();

			List<UnsuppressedInventoryData> enumeratedUnsuppressedInventoryDatas = unsuppressedInventoryDatas
				.Select(s =>
				{
					s.Date = today;
					return s;
				})
				.ToList();

			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateWriteInstance())
			{
				List<UnsuppressedInventoryData> duplicateData = dc.UnsuppressedInventoryDatas
					.Where(w => w.Date == today)
					.ToList();

				if (!duplicateData.Any())
				{
					// Batch Inventory insertion so as to avoid a SqlTimeoutException.
					foreach (IEnumerable<UnsuppressedInventoryData> batchedInventory in enumeratedUnsuppressedInventoryDatas.Batch(c_defaultBatchAmount))
					{
						dc.UnsuppressedInventoryDatas.InsertAllOnSubmit(batchedInventory);
						dc.SubmitChanges();
					}
				}

				List<long?> fulfillableItems = enumeratedUnsuppressedInventoryDatas
					.GroupBy(g => g.asin)
					.Select(s => s.First().afn_fulfillable_quantity)
					.ToList();

				return string.Format("ASINs With Fulfillable Units: {0} \n Units Fulfillable: {1}", fulfillableItems.Count(w => w > 0), fulfillableItems.Sum());
			}
		}

		public string InsertAllOrdersData(IEnumerable<AllOrderDto> allOrdersDtos)
		{
			List<AllOrdersData> allOrdersDatas = allOrdersDtos.Select(s => s.Map()).ToList();

			List<AllOrdersData> groupedAllOrdersData = allOrdersDatas
				.GroupBy(g => new { g.amazon_order_id, g.sku })
				.Select(s => new { value = s.First(), ExtendedPriceSum = s.Sum(z => z.item_price), QuantitySum = s.Sum(z => z.quantity) })
				.Select(s => new AllOrdersData
					{
						amazon_order_id = s.value.amazon_order_id,
						sku = s.value.sku,
						asin = s.value.asin,
						product_name = s.value.product_name,
						merchant_order_id = s.value.merchant_order_id,
						purchase_date = s.value.purchase_date,
						last_updated_date = s.value.last_updated_date,
						order_status = s.value.order_status,
						fulfillment_channel = s.value.fulfillment_channel,
						sales_channel = s.value.sales_channel,
						order_channel = s.value.order_channel,
						url = s.value.url,
						ship_service_level = s.value.ship_service_level,
						item_status = s.value.item_status,
						quantity = s.QuantitySum, // Use the sum of the quantity
						currency = s.value.currency,
						item_price = s.ExtendedPriceSum, // Use the sum of the item_price
						item_tax = s.value.item_tax,
						shipping_price = s.value.shipping_price,
						shipping_tax = s.value.shipping_tax,
						gift_wrap_price = s.value.gift_wrap_price,
						gift_wrap_tax = s.value.gift_wrap_tax,
						item_promotion_discount = s.value.item_promotion_discount,
						ship_promotion_discount = s.value.ship_promotion_discount,
						ship_city = s.value.ship_city,
						ship_state = s.value.ship_state,
						ship_postal_code = s.value.ship_postal_code,
						ship_country = s.value.ship_country,
						promotion_ids = s.value.promotion_ids
					}).ToList();

			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateWriteInstance())
			{
				List<AllOrdersData> failedToInsertOrders = new List<AllOrdersData>();

				HashSet<AllOrdersData> last30DaysAllOrdersDatas = new HashSet<AllOrdersData>(dc.AllOrdersDatas.Where(w => w.purchase_date > DateTime.UtcNow.Date.AddDays(-30)), new AllOrdersDataComparer());

				// Use the AllOrdersDataComparer in the last30DaysAllOrdersDatas HashSet, for use by the .Contains() function.
				List<AllOrdersData> newOrdersToInsert = groupedAllOrdersData.Where(w => !last30DaysAllOrdersDatas.Contains(w)).ToList();

				long newOrdersCount = newOrdersToInsert.Count;

				foreach (AllOrdersData newOrderToInsert in newOrdersToInsert)
				{
					try
					{
						dc.AllOrdersDatas.InsertOnSubmit(newOrderToInsert);
						dc.SubmitChanges();
					}
					catch (Exception)
					{
						failedToInsertOrders.Add(newOrderToInsert);
					}
				}

				// We may fail to insert orders if an order changes it's status after 30 days.
				failedToInsertOrders.ForEach(f =>
					{
						if (!dc.AllOrdersDatas.Any(a => a.amazon_order_id == f.amazon_order_id && a.sku == f.sku))
						{
							dc.AllOrdersDatas.InsertOnSubmit(f);
							dc.SubmitChanges();
						}
						else
						{
							newOrdersCount--;
						}
					});

				List<AllOrdersData> pendingOrders = dc.AllOrdersDatas
					.Where(w => w.order_status == "Pending")
					.ToList();

				long updatedOrdersCount = 0;

				pendingOrders.ForEach(f =>
					{
						AllOrdersData selectedOrder = groupedAllOrdersData.FirstOrDefault(z => z.amazon_order_id == f.amazon_order_id && z.sku == f.sku && z.order_status != "Pending");

						if (selectedOrder != null)
						{
							updatedOrdersCount++;

							dc.AllOrdersDatas.DeleteOnSubmit(f);
							dc.AllOrdersDatas.InsertOnSubmit(selectedOrder);
							dc.SubmitChanges();
						}
					});

				// Retrieve the pending orders again since we may have new pending orders, as well as updating orders
				pendingOrders = dc.AllOrdersDatas
					.Where(w => w.order_status == "Pending")
					.ToList();

				return string.Format("New Orders: {0} \n Updated orders: {1} \n Orders pending: {2}", newOrdersCount, updatedOrdersCount, pendingOrders.Count);
			}
		}

		public string InsertFeePreviewData(IEnumerable<FeePreviewDto> feePreviewDtos)
		{
			DateTime today = DateTime.UtcNow.Date;

			List<FeePreviewData> feePreviewDatas = feePreviewDtos
				.Select(s => s.Map())
				.ToList();

			List<FeePreviewData> enumeratedFeePreviewDatas = feePreviewDatas.Select(s =>
			{
				s.Date = today;
				return s;
			}).ToList();

			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateWriteInstance())
			{
				List<FeePreviewData> duplicateData = dc.FeePreviewDatas
					.Where(w => w.Date == today)
					.ToList();

				if (!duplicateData.Any())
				{
					// Batch Inventory insertion so as to avoid a SqlTimeoutException.
					foreach (IEnumerable<FeePreviewData> batchedFeePreview in enumeratedFeePreviewDatas.Batch(c_defaultBatchAmount))
					{
						dc.FeePreviewDatas.InsertAllOnSubmit(batchedFeePreview);
						dc.SubmitChanges();
					}
				}
			}

			return string.Format("Distinct ASINs with Fee Preview: {0}", enumeratedFeePreviewDatas.Select(s => s.asin).Distinct().Count());
		}

		public string InsertInventoryData(IEnumerable<InventoryData> inventoryDatas)
		{
			List<InventoryData> enumeratedInventoryDatas = inventoryDatas.ToList();

			string result = string.Empty;

			if (enumeratedInventoryDatas.Any())
			{
				DateTime beginDate = enumeratedInventoryDatas.Select(s => s.Date).Min();
				DateTime endDate = enumeratedInventoryDatas.Select(s => s.Date).Max();

				using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
				{
					List<DateTime> existingDates = conn.Query<DateTime>(
						@"SELECT id.Date
						    FROM dbo.InventoryData as id
						   WHERE id.Date >= @BeginDate AND id.Date <= @EndDate
						GROUP BY id.Date",
							new
							{
								BeginDate = beginDate,
								EndDate = endDate
							})
						.ToList();

					// Filter out Dates where we've already inserted inventory data.
					List<InventoryData> inventoryDatasToInsert = enumeratedInventoryDatas
						.Where(w => existingDates.All(a => a.Date != w.Date))
						.ToList();

					List<DateTime> datesToInsert = inventoryDatasToInsert
						.Select(s => s.Date)
						.Distinct()
						.ToList();

					foreach (DateTime dateToInsert in datesToInsert)
					{
						DateTime copyLocalDateToInsert = dateToInsert;

						SqlTransaction transaction = conn.BeginTransaction();

						conn.Execute(
							@"INSERT INTO dbo.InventoryData
							VALUES (@Date, @ASIN, @ProductName, @SellableQuantity, @DefectiveQuantity, @WarehouseDamagedQuantity, @CustomerDamagedQuantity, @DistributorDamagedQuantity)",
							inventoryDatasToInsert.Where(w => w.Date == copyLocalDateToInsert),
							transaction);

						transaction.Commit();
					}
				}

				int unitsSellable = enumeratedInventoryDatas
					.Where(w => w.Date == endDate)
					.Sum(s => s.SellableQuantity);

				int asinsWithSellableQuantity = enumeratedInventoryDatas
					.Count(w => w.Date == endDate && w.SellableQuantity > 0);

				result = string.Format("ASINs With Sellable Units: {0} \n Units Sellable: {1}", asinsWithSellableQuantity, unitsSellable);
			}

			return result;
		}

		public void InsertManufacturer(string manufacturerName)
		{
			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateWriteInstance())
			{
				dc.Manufacturers.InsertOnSubmit(new Manufacturer { Name = manufacturerName });
				dc.SubmitChanges();
			}
		}

		public void InsertProducts(IEnumerable<Models.Product> items, int manufacturerId)
		{
			DateTime createdDate = DateTime.UtcNow;

			List<Product> enumeratedProducts = items
				.Select(s => s.MapProductToData())
				.Select(s =>
				{
					// Set certain variables no matter the input.
					s.ManufacturerId = manufacturerId;
					s.CreatedDate = createdDate;
					s.ModifiedDate = createdDate;

					return s;
				})
				.ToList();

			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateWriteInstance())
			{
				dc.Products.InsertAllOnSubmit(enumeratedProducts);
				dc.SubmitChanges();
			}
		}

		public List<Models.Product> GetProductsForManufacturer(int manufacturerId)
		{
			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateReadInstance())
			{
				return dc.Products
					.Where(w => w.ManufacturerId == manufacturerId)
					.ToList()
					.Select(s => s.MapProductToModel())
					.ToList();
			}
		}

		public List<OrderSummary> GetOrderSummaryForManufacturer(int manufacturerId, DateTime beginDate, DateTime endDate)
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<OrderSummary>(
					@"SELECT p.ASIN,
							 SUM(aod.quantity) as TotalQuantity,
							 SUM(aod.[item-price]) as TotalExtension
						FROM dbo.Products p
						JOIN dbo.AllOrdersData aod on aod.asin = p.ASIN
					   WHERE p.ManufacturerId = @ManufacturerId
							 AND CAST(aod.[purchase-date] as Date) >= @BeginDate
							 AND CAST(aod.[purchase-date] as Date) <= @EndDate
							 AND aod.quantity > 0
							 AND NOT (aod.[item-status] = 'Cancelled' OR aod.[order-status]= 'Cancelled')
							 AND aod.[purchase-date] IS NOT NULL
							 AND aod.[item-price] IS NOT NULL
					GROUP BY p.ASIN",
				new
				{
					ManufacturerId = manufacturerId,
					BeginDate = beginDate,
					EndDate = endDate
				}).ToList();
			}
		}

		public List<Order> GetOrdersForManufacturer(int manufacturerId, DateTime beginDate, DateTime endDate)
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<Order>(
					@"SELECT aod.ASIN,
							 aod.sku as SKU,
							 CAST(aod.[purchase-date] as Date) as PurchaseDate,
							 aod.quantity as Quantity,
							 aod.[item-price] as ItemPrice
						FROM dbo.Products p
						JOIN dbo.AllOrdersData aod on aod.asin = p.ASIN
						WHERE p.ManufacturerId = @ManufacturerId
							  AND CAST(aod.[purchase-date] as Date) >= @BeginDate
							  AND CAST(aod.[purchase-date] as Date) <= @EndDate
							  AND aod.quantity > 0
							  AND NOT (aod.[item-status] = 'Cancelled' OR aod.[order-status]= 'Cancelled')
							  AND aod.[purchase-date] IS NOT NULL
							  AND aod.[item-price] IS NOT NULL",
					new
					{
						BeginDate = beginDate,
						EndDate = endDate,
						ManufacturerId = manufacturerId
					})
					.ToList();
			}
		}

		public List<DataPoint> GetProductSalesByDay(string asin, DateTime beginDate, DateTime endDate)
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<DataPoint>(
					@"	SELECT CAST(aod.[purchase-date] as Date) as DateTime,
							   CAST(SUM(aod.quantity) as decimal) as Value,
							   'Quantity Sold' as SeriesName
						  FROM [AmazonMWS].[dbo].[AllOrdersData] aod
						 WHERE aod.ASIN = @ASIN
							   AND CAST(aod.[purchase-date] as Date) >= @BeginDate
							   AND CAST(aod.[purchase-date] as Date) <= @EndDate
					  GROUP BY CAST(aod.[purchase-date] as Date)",
					new
					{
						BeginDate = beginDate,
						EndDate = endDate,
						ASIN = asin
					})
					.ToList();
			}
		}

		public List<DataPoint> GetInventoryHistoryByDay(string asin, DateTime beginDate, DateTime endDate)
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<DataPoint>(
					@"	SELECT uid.Date as DateTime,
							   CAST(MAX(uid.[afn-fulfillable-quantity]) as decimal) as Value,
							   'Fulfillable Quantity' as SeriesName
						  FROM [AmazonMWS].[dbo].[UnsuppressedInventoryData] uid
						 WHERE ASIN = @ASIN
							   AND condition = 'New'
							   AND Date >= @BeginDate
							   AND Date <= @EndDate
					  GROUP BY ASIN, uid.Date",
					new
					{
						BeginDate = beginDate,
						EndDate = endDate,
						ASIN = asin
					})
					.ToList();
			}
		}

		public List<CurrentInventory> GetCurrentInventoryForManufacturer(int manufacturerId)
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<CurrentInventory>(
					@"WITH CurrentInventory AS (
						SELECT  AllItems.asin,
								AllItems.[afn-fulfillable-quantity]
							FROM (SELECT *,
										ROW_NUMBER() OVER (PARTITION BY uia.asin ORDER BY uia.[afn-fulfillable-quantity] DESC) as RowNumber
									FROM AmazonMWS.dbo.UnsuppressedInventoryData as uia
									WHERE Date = (SELECT MAX(Date)
													FROM AmazonMWS.dbo.UnsuppressedInventoryData)
														 AND [afn-warehouse-quantity] > 0) AllItems
						WHERE AllItems.RowNumber = 1
					)
					SELECT ci.ASIN,
						   ci.[afn-fulfillable-quantity] as AfnFulfillableQuantity
					  FROM CurrentInventory ci
					  JOIN [AmazonMWS].[dbo].[Products] p on p.ASIN = ci.ASIN
					 WHERE p.ManufacturerId = @ManufacturerId",
					new { ManufacturerId = manufacturerId })
					.ToList();
			}
		}

		public List<InventorySummary> GetInventorySummaryForManufacturer(int manufacturerId, DateTime beginDate, DateTime endDate)
		{
			using (SqlConnection sqlConn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return sqlConn.Query<InventorySummary>(
					@"WITH ManufacturerProducts AS (
					SELECT p.ASIN
						FROM [AmazonMWS].[dbo].[Products] p
						WHERE p.ManufacturerId = @ManufacturerId
							AND p.ASIN IS NOT NULL
					), InventoryByDate AS (
						SELECT  AllItems.asin,
								AllItems.[afn-fulfillable-quantity],
								AllItems.Date
							FROM (SELECT *,
										ROW_NUMBER() OVER (PARTITION BY uia.asin, uia.Date ORDER BY uia.[afn-fulfillable-quantity] DESC) as RowNumber
									FROM AmazonMWS.dbo.UnsuppressedInventoryData as uia
									WHERE Date >= @BeginDate
											AND Date <= @EndDate) AllItems
						WHERE AllItems.RowNumber = 1
					), CurrentInventory AS (
					SELECT InventoryHistory.ASIN,
							InventoryHistory.[afn-fulfillable-quantity]
					FROM (
						SELECT ibd.*,
								ROW_NUMBER() OVER (PARTITION BY ibd.ASIN ORDER BY ibd.Date DESC) as RowNumber
							FROM InventoryByDate ibd) InventoryHistory
					WHERE InventoryHistory.RowNumber = 1
					), InventorySummary AS (
						SELECT ibd.ASIN,
								SUM(CASE WHEN ibd.[afn-fulfillable-quantity] > 0 THEN 1 ELSE 0 END) AS DistinctDaysInStock
							FROM InventoryByDate ibd
						GROUP BY ibd.ASIN
					)

						SELECT mp.ASIN,
								COALESCE(i.DistinctDaysInStock, 0) as DaysInStockDuringTimeframe,
								COALESCE(ci.[afn-fulfillable-quantity], 0) as CurrentAfnFulfillableQuantity
							FROM ManufacturerProducts mp
					LEFT JOIN InventorySummary i on i.ASIN = mp.ASIN
					LEFT JOIN CurrentInventory ci on ci.ASIN = mp.ASIN",
					new
					{
						ManufacturerId = manufacturerId,
						BeginDate = beginDate,
						EndDate = endDate
					}, commandTimeout: 0)
					.ToList();
			}
		}

		public List<Manufacturer> FindManufacturers(string manufacturerString)
		{
			List<Manufacturer> manufacturers = new List<Manufacturer>();

			if (manufacturerString != null)
			{
				int? manufacturerId = null;

				int tryParseManufacturerId;
				if (int.TryParse(manufacturerString, out tryParseManufacturerId)) manufacturerId = tryParseManufacturerId;

				using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateReadInstance())
				{
					manufacturers.AddRange(
						manufacturerId.HasValue
							? dc.Manufacturers.Where(w => w.ManufacturerId == manufacturerId.Value).ToList()
							: dc.Manufacturers.Where(w => w.Name.Contains(manufacturerString)).ToList());
				}
			}

			return manufacturers;
		}

		public List<FeePreview> GetFeePreviewForAsin(string asin)
		{
			using (AmazonMWSdbDataContext dc = m_amazonMWSdbDataContext.CreateReadInstance())
			{
				return dc.FeePreviewDatas
					.Where(w => w.asin == asin)
					.GroupBy(g => g.asin)
					.Select(s => s.OrderByDescending(o => o.Date).First())
					.GroupBy(g => g.asin)
					.Select(s => s.OrderBy(o => o.your_price).First())
					.Select(s => new FeePreview
						{
							ASIN = s.asin,
							SKU = s.sku,
							Name = s.product_name,
							EstimatedOrderHandlingFeePerOrder = s.estimated_order_handling_fee_per_order,
							EstimatedPickPackFeePerUnit = s.estimated_pick_pack_fee_per_unit,
							EstimatedWeightHandlingFeePerUnit = s.estimated_weight_handling_fee_per_unit
						})
					.ToList();
			}
		}

		public List<RepricingInformation> GetAllRepricingInformations()
		{
			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return conn.Query<RepricingInformation>(
					@"SELECT r.SKU,
								r.ASIN,
								r.MinimumPrice
							FROM [AmazonMWS].[dbo].[RepricingInformation] r 
							WHERE r.IsCurrent = '1'")
					.ToList();
			}
		}

		public List<EFModels.RepricingInformation> FindRepricingInformations(string query, int numResults)
		{
			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return conn.Query<EFModels.RepricingInformation>(
					@"SELECT TOP (@NumResults)
							 RepricingInformationId,
							 SKU,
							 ASIN,
							 MinimumPrice,
							 ProductName,
							 IsCurrent
						FROM AmazonMWS.dbo.RepricingInformation
					   WHERE ASIN like '%' + @Query + '%'
							 OR ProductName like '%' + @Query + '%'
							 OR SKU like '%' + @Query + '%'",
					new {
							Query = query,
							NumResults = numResults
						})
						.ToList();
			}
		} 

		public decimal GetTotalInventoryValue()
		{
			using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.amazonmwsConnectionString))
			{
				return conn.Query<decimal>(
					@"WITH DistinctItems AS (
						SELECT AllItems.sku,
								AllItems.asin,
								AllItems.[product-name],
								AllItems.[afn-warehouse-quantity],
								AllItems.[afn-fulfillable-quantity],
								AllItems.[afn-unsellable-quantity],
								AllItems.[afn-reserved-quantity]
							FROM (SELECT *,
										ROW_NUMBER() OVER (PARTITION BY uia.asin ORDER BY uia.[your-price] ASC) as RowNumber
									FROM AmazonMWS.dbo.UnsuppressedInventoryData as uia
									WHERE Date = (SELECT MAX(Date)
													FROM AmazonMWS.dbo.UnsuppressedInventoryData)
															AND [afn-warehouse-quantity] > 0) AllItems
						WHERE AllItems.RowNumber = 1
						), ProductCosts AS
						(
							SELECT di.asin,
									di.sku,
									di.[product-name],
									di.[afn-fulfillable-quantity],
									p.Cost,
									(di.[afn-fulfillable-quantity] * COALESCE(p.OverrideCost, p.PromotionCost, ((1 - COALESCE(md.DiscountPercentage, 0)) * p.Cost))) as TotalInventoryCost
							FROM DistinctItems as di
							JOIN AmazonMWS.dbo.Products as p on p.ASIN = di.asin
						LEFT JOIN AmazonMWS.dbo.ManufacturerDiscounts md on md.ManufacturerId = p.ManufacturerId
							WHERE di.[afn-fulfillable-quantity] > 0
						)

							SELECT SUM(TotalInventoryCost)
							FROM ProductCosts").SingleOrDefault();
			}
		}

		private const int c_defaultBatchAmount = 100;

		private readonly AmazonMWSdbDataContext m_amazonMWSdbDataContext;
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Logging;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using MarketplaceWebServiceProducts;
using MarketplaceWebServiceProducts.Model;
using Models;
using MoreLinq;
using Utility;
using Utility.Exceptions;

namespace AmazonProductLookup.AmazonApis.MwsApi.Products
{
	public sealed class MwsProductsApi : IMwsProductsApi
	{
		public MwsProductsApi(string sellerId, string marketPlaceId, string accessKeyId, string secretAccessKeyId, string serviceUrl)
		{
			m_sellerId = sellerId;
			m_marketPlaceId = marketPlaceId;

			MarketplaceWebServiceProductsConfig config = new MarketplaceWebServiceProductsConfig { ServiceURL = serviceUrl };
			m_productClient = new MarketplaceWebServiceProductsClient(string.Empty, string.Empty, accessKeyId, secretAccessKeyId, config);

			MarketplaceWebServiceConfig configService = new MarketplaceWebServiceConfig()
				.WithServiceURL("https://mws.amazonservices.com");
			configService.SetUserAgentHeader(string.Empty, string.Empty, "C#");

			m_service = new MarketplaceWebServiceClient(accessKeyId, secretAccessKeyId, configService);
		}

		public List<Listing> GetAllListingsForAsin(IEnumerable<string> asins)
		{
			List<Listing> foundListings = new List<Listing>();

			foreach (IEnumerable<string> asinBatch in asins.Distinct().Batch(c_maxBatchAmount))
			{
				List<string> enumeratedASINs = asinBatch.ToList();

				List<GetLowestOfferListingsForASINResult> lowestOfferListingsResults = null;
				GetLowestOfferListingsForASINRequest lowestOfferListingsRequest = new GetLowestOfferListingsForASINRequest
					{
						ASINList = new ASINListType { ASIN = enumeratedASINs },
						MarketplaceId = m_marketPlaceId,
						SellerId = m_sellerId
					};

				List<GetMatchingProductResult> matchingProductResult = null;
				GetMatchingProductRequest matchingProductRequest = new GetMatchingProductRequest
					{
						ASINList = new ASINListType { ASIN = enumeratedASINs },
						MarketplaceId = m_marketPlaceId,
						SellerId = m_sellerId
					};

				// Attempt 5 times to issue requests to Amazon, since throttling causes exceptions.
				for (int i = 0; i < 5; i++)
				{
					try
					{
						GetLowestOfferListingsForASINResponse lowestOfferListingsResponse = m_productClient.GetLowestOfferListingsForASIN(lowestOfferListingsRequest);
						lowestOfferListingsResults = lowestOfferListingsResponse.GetLowestOfferListingsForASINResult;

						GetMatchingProductResponse matchingProductResponse = m_productClient.GetMatchingProduct(matchingProductRequest);
						matchingProductResult = matchingProductResponse.GetMatchingProductResult;

						// If we get to here without an exception, then we don't have to issue the request again.
						break;
					}
					catch (MarketplaceWebServiceProductsException e)
					{
						Console.WriteLine(e.Message);
						Thread.Sleep(10000);
					}
				}

				if (lowestOfferListingsResults != null && matchingProductResult != null)
				{
					foreach (string asin in enumeratedASINs)
					{
						GetLowestOfferListingsForASINResult lowestOfferListing = lowestOfferListingsResults
							.Where(w => w.Product != null)
							.FirstOrDefault(w => w.Product.Identifiers.MarketplaceASIN.ASIN == asin);

						GetMatchingProductResult matchingProduct = matchingProductResult
							.Where(w => w.Product != null)
							.FirstOrDefault(f => f.Product.Identifiers.MarketplaceASIN.ASIN == asin);

						if (lowestOfferListing != null && matchingProduct != null)
						{
							List<LowestOfferListingType> lowestofferListingTypes = lowestOfferListing
								.Product
								.LowestOfferListings
								.LowestOfferListing;

							int numberOfMerchantFulfilledOffers = lowestofferListingTypes
								.Where(w => w.Qualifiers.ItemCondition == "New")
								.Count(c => c.Qualifiers.FulfillmentChannel == FulfillmentChannel.Merchant.ToString());

							int numberOfAmazonFulfilledOffers = lowestofferListingTypes
								.Where(w => w.Qualifiers.ItemCondition == "New")
								.Count(c => c.Qualifiers.FulfillmentChannel == FulfillmentChannel.Amazon.ToString());

							decimal? lowestPrice = null;

							if (lowestofferListingTypes.Any())
								lowestPrice = lowestofferListingTypes
									.Where(w => w.Qualifiers.ItemCondition == "New")
									.Select(s => s.Price.ListingPrice.Amount + (s.Price.Shipping == null ? 0m : s.Price.Shipping.Amount)) // s.Price.Shipping can be null.
									.Where(w => w > 0)
									.OrderBy(o => o)
									.FirstOrDefault();

							var salesRank = matchingProduct
								.Product
								.SalesRankings
								.SalesRank
								.Select(s => new { Rank = (decimal?)s.Rank, ProductCategory = s.ProductCategoryId })
								.FirstOrDefault()
								??
								new { Rank = (decimal?)null, ProductCategory = string.Empty };

							foundListings.Add(new Listing
								{
									ASIN = asin,
									SellersRank = salesRank.Rank,
									SellersRankCategory = salesRank.ProductCategory,
									LowestPrice = lowestPrice,
									NumberOfMerchantFulfilledOffers = numberOfMerchantFulfilledOffers,
									NumberOfAmazonFulfilledOrders = numberOfAmazonFulfilledOffers,
								});
						}
					}
				}
			}

			return foundListings;
		}

		public MemoryStream GetReportData(ReportType reportType, DateTime? startDate = null, DateTime? endDate = null)
		{
			RequestReportRequest requestReportRequest = new RequestReportRequest()
				.WithMarketplaceIdList(new IdList { Id = new List<string> { m_marketPlaceId } })
				.WithReportType(reportType.ToString())
				.WithMerchant(m_sellerId)
				.WithReportOptions("true");

			if (startDate.HasValue)
				requestReportRequest.WithStartDate(startDate.Value);

			if (endDate.HasValue)
				requestReportRequest.WithEndDate(endDate.Value);

			Func<MemoryStream> getMemoryStream = () =>
				{
					RequestReportResponse requestReportResponse = m_service.RequestReport(requestReportRequest);
					Func<string> getReportRequestId = () =>
						{
							ReportRequestInfo reportRequestInfo = requestReportResponse.RequestReportResult.ReportRequestInfo;

							if (reportRequestInfo.ReportRequestId == null)
								throw new RetryException("Need to wait for ReportRequestId to become not null in the getReportRequestId");

							return reportRequestInfo.ReportRequestId;
						};

					string reportRequestId = getReportRequestId.Retry(12, 5000);

					if (reportRequestId == null)
						throw new QuitException("reportRequestId is null after getReportRequestId");

					GetReportRequestListRequest reportRequestList = new GetReportRequestListRequest()
						.WithMerchant(m_sellerId)
						.WithReportRequestIdList(new IdList { Id = new List<string> { reportRequestId } });

					Func<string> getGeneratedReportId = () =>
					{
						string generatedReportId;

						try
						{
							GetReportRequestListResponse reportRequestListResponse = m_service.GetReportRequestList(reportRequestList);
							ReportRequestInfo reportRequestInfo = reportRequestListResponse.GetReportRequestListResult.ReportRequestInfo
								.FirstOrDefault(w => w.ReportRequestId == reportRequestId);

							if (reportRequestInfo == null)
								throw new RetryException("reportRequestInfo is null in getReport");

							if (reportRequestInfo.ReportProcessingStatus != "_SUBMITTED_" && reportRequestInfo.ReportProcessingStatus != "_IN_PROGRESS_")
							{
								if (reportRequestInfo.GeneratedReportId != null)
									generatedReportId = reportRequestInfo.GeneratedReportId;
								else
									throw new QuitException("generatedReportId was null in waitForReportToProcess");
							}
							else
							{
								throw new RetryException("Had to wait for report to finish processing in waitForReportToProcess");
							}
						}
						catch (MarketplaceWebServiceException)
						{
							// Caused by throttling, lets wait longer.
							Thread.Sleep(60000);
							throw new RetryException("Throttled due to MarketplaceWebServiceException.");
						}

						return generatedReportId;
					};

					string returnedGeneratedReportId = getGeneratedReportId.Retry(30, 30000);

					if (returnedGeneratedReportId == null)
						throw new QuitException("reportId was null, from getReport");

					MemoryStream returnedMemoryStream = null;

					try
					{
						MemoryStream memoryStream = new MemoryStream();
						GetReportRequest reportRequest = new GetReportRequest { Merchant = m_sellerId, ReportId = returnedGeneratedReportId, Report = memoryStream };
						GetReportResponse response = m_service.GetReport(reportRequest);

						string md5String = response.GetReportResult.ContentMD5;

						if (!md5String.Equals(memoryStream.ComputeMd5(), StringComparison.InvariantCulture))
							throw new QuitException("contentMd5 did not match!");

						returnedMemoryStream = memoryStream;
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}

					if (returnedMemoryStream == null)
						throw new QuitException("returnedMemoryStream was null.");

					return returnedMemoryStream;
				};

			return getMemoryStream.Retry(30, 180000, true);
		}

		public MemoryStream GetMostRecentAutomaticallyScheduledReport(ReportType reportType)
		{
			GetReportListRequest request = new GetReportListRequest()
				.WithReportTypeList(new TypeList { Type = new List<string> { reportType.ToString() } })
				.WithMerchant(m_sellerId);

			GetReportListResponse response = m_service.GetReportList(request);

			GetReportListResult result = response.GetReportListResult;

			List<ReportInfo> reportInfo = result.ReportInfo;

			ReportInfo mostRecentScheduledReport = reportInfo.OrderByDescending(o => o.AvailableDate).FirstOrDefault();

			MemoryStream memoryStream = new MemoryStream();

			if (mostRecentScheduledReport != null)
			{
				GetReportRequest reportRequest = new GetReportRequest
					{
						Merchant = m_sellerId,
						ReportId = mostRecentScheduledReport.ReportId,
						Report = memoryStream
					};

				m_service.GetReport(reportRequest);
			}

			return memoryStream;
		}

		public Dictionary<string, decimal> GetMyPriceForSKUs(IEnumerable<string> skus)
		{
			Dictionary<string, decimal> myPriceDictionary = new Dictionary<string, decimal>();

			GetMyPriceForSKURequest requestMyPriceForSku = new GetMyPriceForSKURequest
			{
				SellerId = m_sellerId,
				MarketplaceId = m_marketPlaceId,
				SellerSKUList = new SellerSKUListType()
			};

			requestMyPriceForSku.SellerSKUList.SellerSKU.AddRange(skus);
			try
			{
				GetMyPriceForSKUResponse response = m_productClient.GetMyPriceForSKU(requestMyPriceForSku);

				List<GetMyPriceForSKUResult> getMyPriceForSkuResultList = response.GetMyPriceForSKUResult;
				foreach (GetMyPriceForSKUResult getMyPriceForSkuResult in getMyPriceForSkuResultList)
				{
					if (getMyPriceForSkuResult.IsSetProduct())
					{
						MarketplaceWebServiceProducts.Model.Product product = getMyPriceForSkuResult.Product;

						if (product.IsSetOffers())
						{
							OffersList offers = product.Offers;
							List<OfferType> offerList = offers.Offer;
							foreach (OfferType offer in offerList)
							{
								if (offer.IsSetBuyingPrice())
								{
									PriceType buyingPrice = offer.BuyingPrice;
									if (buyingPrice.IsSetLandedPrice())
									{
										MoneyType landedPrice2 = buyingPrice.LandedPrice;

										if (landedPrice2.IsSetAmount())
										{
											decimal myPrice = landedPrice2.Amount;
											myPriceDictionary.Add(offer.SellerSKU, myPrice);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (MarketplaceWebServiceProductsException e)
			{
				LoggingRepository.Log(LoggingCategory.RepricingScript, string.Format("Exception in 'GetMyPriceForSKU': {0}", e.Message));
			}

			return myPriceDictionary;
		}

		public Dictionary<string, List<LowestOfferListingType>> GetLowestOfferListingsForASINs(IEnumerable<string> asins)
		{
			List<GetLowestOfferListingsForASINResult> getLowestOfferListingsForAsinResults = new List<GetLowestOfferListingsForASINResult>();

			foreach (IEnumerable<string> asinBatch in asins.Distinct().Batch(c_maxBatchAmount))
			{
				GetLowestOfferListingsForASINRequest requestListings = new GetLowestOfferListingsForASINRequest
				{
					SellerId = m_sellerId,
					MarketplaceId = m_marketPlaceId,
					ASINList = new ASINListType().WithASIN(asinBatch.ToArray()),
					ExcludeMe = true,
					ItemCondition = "New"
				};

				const int numAttempts = 3;
				int numAttemptsCounter = 0;

				Func<List<GetLowestOfferListingsForASINResult>> tryGetLowestOfferListingsForAsin =
					() =>
					{
						numAttemptsCounter++;

						try
						{
							GetLowestOfferListingsForASINResponse response = m_productClient.GetLowestOfferListingsForASIN(requestListings);

							return response.GetLowestOfferListingsForASINResult;
						}
						catch (MarketplaceWebServiceProductsException e)
						{
							if (numAttemptsCounter == numAttempts)
								throw new QuitException(string.Format("Call to 'GetLowestOfferListingsForASIN' failed on attempt #{0}. Message: {1} StackTrace: {2}", numAttemptsCounter, e.Message, e.StackTrace));

							throw new RetryException("Try again to GetLowestOfferListingsForASINResult");
						}
					};

				List<GetLowestOfferListingsForASINResult> currentBatchLowestOfferListings = tryGetLowestOfferListingsForAsin.Retry(numAttempts, 5000);

				Thread.Sleep(2000);

				getLowestOfferListingsForAsinResults.AddRange(currentBatchLowestOfferListings);
			}

			return getLowestOfferListingsForAsinResults
				.Where(x => x.Product != null && x.Product.LowestOfferListings != null)
				.ToDictionary(x => x.ASIN, y => y.Product.LowestOfferListings.LowestOfferListing);
		}

		private readonly string m_sellerId;
		private readonly string m_marketPlaceId;
		private readonly MarketplaceWebService.MarketplaceWebService m_service;
		private readonly MarketplaceWebServiceProductsClient m_productClient;

		private const int c_maxBatchAmount = 20;
	}
}
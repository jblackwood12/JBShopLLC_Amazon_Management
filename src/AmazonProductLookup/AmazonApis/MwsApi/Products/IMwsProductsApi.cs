using System;
using System.Collections.Generic;
using System.IO;
using MarketplaceWebServiceProducts.Model;
using Models;

namespace AmazonProductLookup.AmazonApis.MwsApi.Products
{
	public interface IMwsProductsApi
	{
		List<Listing> GetAllListingsForAsin(IEnumerable<string> asin);

		MemoryStream GetReportData(ReportType reportType, DateTime? startDate = null, DateTime? endDate = null);

		MemoryStream GetMostRecentAutomaticallyScheduledReport(ReportType reportType);

		Dictionary<string, decimal> GetMyPriceForSKUs(IEnumerable<string> skus);

		Dictionary<string, List<LowestOfferListingType>> GetLowestOfferListingsForASINs(IEnumerable<string> asins);
	}
}

using System;
using System.Collections.Generic;
using System.Web.Http;
using Data;
using Models;
using Utility;
using Utility.Models.Date;

namespace WebApplication.Controllers.api
{
	public class ProductReportController : ApiController
	{
		public ProductReportController(IAmazonMWSdbService amazonMwSdbService)
		{
			m_amazonMwSdbService = amazonMwSdbService;
		}

		[HttpGet]
		[ActionName("GetProductSalesByDay")]
		public MovingAverageFlotResultContainer GetProductSalesByDay([FromUri] string asin)
		{
			DateTime endDate = DateTime.UtcNow.Date.AddDays(-1);
			DateTime beginDate = endDate.AddDays(-364);
			DateTime beginDateMovingAverage = beginDate.AddDays(-(c_daysMovingAverage - 1));

			List<DataPoint> dataPoints = m_amazonMwSdbService.GetProductSalesByDay(asin, beginDateMovingAverage, endDate)
				.FillMissingDates(beginDateMovingAverage, endDate);

			MovingAverageFlotResultContainer salesByDay = DateUtility.CalculateMovingAverage(dataPoints, c_daysMovingAverage, beginDate, endDate)
				.MovingAverageDataPointContainer
				.ConvertSeriesData();

			return salesByDay;
		}

		[HttpGet]
		[ActionName("GetProductInventoryByDay")]
		public MovingAverageFlotResultContainer GetProductInventoryByDay([FromUri] string asin)
		{
			DateTime endDate = DateTime.UtcNow.Date.AddDays(-1);
			DateTime beginDate = endDate.AddDays(-364);
			DateTime beginDateMovingAverage = beginDate.AddDays(-(c_daysMovingAverage - 1));

			List<DataPoint> dataPoints = m_amazonMwSdbService.GetInventoryHistoryByDay(asin, beginDateMovingAverage, endDate)
				.FillMissingDates(beginDateMovingAverage, endDate);

			MovingAverageFlotResultContainer salesByDay = DateUtility.CalculateMovingAverage(dataPoints, c_daysMovingAverage, beginDate, endDate)
				.MovingAverageDataPointContainer
				.ConvertSeriesData();

			return salesByDay;
		}

		[HttpGet]
		[ActionName("GetPriceHistory")]
		public MovingAverageFlotResultContainer GetPriceHistory([FromUri] string asin)
		{
			DateTime endDate = DateTime.UtcNow.Date.AddDays(-1);
			DateTime beginDate = endDate.AddDays(-364);
			DateTime beginDateMovingAverage = beginDate.AddDays(-(c_daysMovingAverage - 1));

			List<DataPoint> dataPoints = m_amazonMwSdbService.GetPriceHistories(asin, beginDateMovingAverage, endDate);

			return new MovingAverageDataPointContainer { CombinedSeries = dataPoints }
				.ConvertSeriesData();
		}

		[HttpGet]
		[ActionName("UnpricedProducts")]
		public List<UnpricedProduct> UnpricedProducts()
		{
			return m_amazonMwSdbService.GetUnpricedProducts();
		}

		private const int c_daysMovingAverage = 30;
		private readonly IAmazonMWSdbService m_amazonMwSdbService;
	}
}
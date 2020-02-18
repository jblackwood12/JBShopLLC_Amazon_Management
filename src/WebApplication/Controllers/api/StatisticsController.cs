using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using Data;
using Utility;
using Utility.Models.Date;

namespace WebApplication.Controllers.api
{
	public class StatisticsController : ApiController
	{
		public StatisticsController(IAmazonMWSdbService amazonMwSdbService)
		{
			m_amazonMwSdbService = amazonMwSdbService;
		}

		[HttpGet]
		[ActionName("GetInventoryValue")]
		public decimal GetInventoryValue()
		{
			decimal inventoryValue;

			object cachedObject = HttpRuntime.Cache.Get(c_getInventoryValueCacheKey);

			if (cachedObject != null)
			{
				inventoryValue = (decimal) cachedObject;
			}
			else
			{
				inventoryValue = m_amazonMwSdbService.GetTotalInventoryValue();
				HttpRuntime.Cache.Add(c_getInventoryValueCacheKey, inventoryValue, null, Cache.NoAbsoluteExpiration, new TimeSpan(6, 0, 0), CacheItemPriority.High, null);
			}

			return inventoryValue;
		}

		[HttpGet]
		[ActionName("GetSalesByDay")]
		public MovingAverageFlotResultContainer GetSalesByDay()
		{
			MovingAverageFlotResultContainer salesByDay;

			object cachedObject = HttpRuntime.Cache.Get(c_getSalesByDayReportCacheKey);

			if (cachedObject != null)
			{
				salesByDay = (MovingAverageFlotResultContainer)cachedObject;
			}
			else
			{
				DateTime endDate = DateTime.UtcNow.Date.AddDays(-1);
				DateTime beginDate = endDate.AddDays(-179);
				DateTime beginDateMovingAverage = beginDate.AddDays(-(c_daysMovingAverage - 1));

				List<DataPoint> dataPoints = m_amazonMwSdbService.GetSalesByDay(beginDateMovingAverage, endDate).FillMissingDates(beginDateMovingAverage, endDate);

				salesByDay = DateUtility.CalculateMovingAverage(dataPoints, c_daysMovingAverage, beginDate, endDate).MovingAverageDataPointContainer.ConvertSeriesData();

				HttpRuntime.Cache.Add(c_getSalesByDayReportCacheKey, salesByDay, null, Cache.NoAbsoluteExpiration, new TimeSpan(4, 0, 0), CacheItemPriority.High, null);
			}

			return salesByDay;
		}

		private const string c_getInventoryValueCacheKey = "GetInventoryValue";
		private const string c_getSalesByDayReportCacheKey = "GetSalesByDayReport";

		private const int c_daysMovingAverage = 30;

		private readonly IAmazonMWSdbService m_amazonMwSdbService;
	}
}

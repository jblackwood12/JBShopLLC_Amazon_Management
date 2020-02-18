using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Models.Date;

namespace Utility
{
	public static class FlotUtility
	{
		public static IEnumerable<FlotResult> ConvertSeriesData<T, T1>(this IEnumerable<T> data, Func<T, T1> x, Func<T, decimal?> y, Func<T, string> category)
		{
			var result = from d in data
				group d by category(d)
				into g
				select new { category = g.Key, value = g.Select(v => v) };

			return (from r in result select new { r.category, xy = r.value.Select(s => new object[] { x(s), y(s) }) })
				.Select(s => new FlotResult(s.category, s.xy));
		}

		public static long ConvertDateToEpoch(DateTime date)
		{
			return (long)date.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
		}

		public static IEnumerable<FlotResult> ConvertSeriesData(this IEnumerable<DataPoint> data)
		{
			return data.ConvertSeriesData(x => ConvertDateToEpoch(x.DateTime), y => y.Value, o => o.SeriesName);
		}

		public static MovingAverageFlotResultContainer ConvertSeriesData(this MovingAverageDataPointContainer movingAverageDataPointContainer)
		{
			MovingAverageFlotResultContainer movingAverageFlotResultContainer = new MovingAverageFlotResultContainer();

			if (movingAverageDataPointContainer.CombinedSeries != null)
				movingAverageFlotResultContainer.CombinedSeries = movingAverageDataPointContainer.CombinedSeries.ConvertSeriesData();

			if (movingAverageDataPointContainer.MovingAverageSeries != null)
				movingAverageFlotResultContainer.MovingAverageSeries = movingAverageDataPointContainer.MovingAverageSeries.ConvertSeriesData().SingleOrDefault();

			return movingAverageFlotResultContainer;
		}
	}
}

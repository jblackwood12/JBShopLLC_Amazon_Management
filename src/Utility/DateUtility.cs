using System;
using System.Collections.Generic;
using System.Linq;
using Deedle;
using Utility.Models.Date;

namespace Utility
{
	public static class DateUtility
	{
		public static double DateTimeToUnixTimestamp(this DateTime dateTime)
		{
			return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds;
		}

		public static IEnumerable<DateTime> GetDatesInTimeFrame(DateTime beginDate, DateTime endDate)
		{
			for (DateTime counter = beginDate; counter <= endDate; counter = counter.AddDays(1))
				yield return counter.Date;
		}

		public static MovingAverageResult CalculateMovingAverage(
			List<DataPoint> series,
			int daysMovingAverage,
			DateTime beginDate,
			DateTime endDate,
			MovingAverageType movingAverageType = MovingAverageType.Simple)
		{
			if (daysMovingAverage <= 1)
				throw new ArgumentException("daysMovingAverage has to be greater than 1");

			series = series.OrderBy(o => o.DateTime)
				.ToList();

			List<DataPoint> collapsedSeries = series.GroupBy(g => g.DateTime)
				.Select(s => new DataPoint { DateTime = s.Key, Value = s.Sum(z => z.Value) })
				.ToList();

			SeriesBuilder<DateTime, decimal> dateSeries = new SeriesBuilder<DateTime, decimal>();

			foreach (DataPoint dataPoint in collapsedSeries)
				dateSeries.Add(dataPoint.DateTime, dataPoint.Value ?? 0m);

			Series<DateTime, decimal> movingAverage = dateSeries
				.Series
				.Window(daysMovingAverage)
				.Select(s => s.Value.Mean());

			List<DataPoint> movingAverageSeries = movingAverage
				.GetAllObservations()
				.Select(s => new DataPoint
				{
					DateTime = s.Key,
					Value = s.Value.Value,
					SeriesName = string.Format("{0}-day Moving Average", daysMovingAverage)
				})
				.ToList();

			series = series
				.Where(w => w.DateTime >= beginDate)
				.Where(w => w.DateTime <= endDate)
				.OrderBy(o => o.SeriesName)
				.ThenBy(o => o.DateTime)
				.ToList();

			return new MovingAverageResult(series, movingAverageSeries, movingAverageType);
		}

		public static List<DataPoint> CalculateCumulativeSum(this IEnumerable<DataPoint> dataPoints, bool useNullForPrecedingValues = false)
		{
			decimal cumulativeSum = 0;

			bool hasHadValueForSeries = false;

			return dataPoints
				.Select(s =>
				{
					if (useNullForPrecedingValues && !hasHadValueForSeries && !s.Value.HasValue)
						s.Value = null;
					else
					{
						s.Value = cumulativeSum += s.Value ?? 0;
						hasHadValueForSeries = true;
					}

					return s;
				}).ToList();
		}

		public static List<DataPoint> FillMissingDates(this IEnumerable<DataPoint> dataPoints, DateTime beginDate, DateTime endDate, bool useNullDefaultValue = false)
		{
			List<DataPoint> combinedSeries = new List<DataPoint>();

			Dictionary<string, List<DataPoint>> distinctSeries = dataPoints
				.GroupBy(g => g.SeriesName)
				.ToDictionary(k => k.Key, v => v.Select(s => s).ToList());

			Frame<DateTime, string> defaultSeriesFrame = Frame.FromRecords(GetDatesInTimeFrame(beginDate, endDate))
				.IndexRows<DateTime>(c_dateReflectionName);

			foreach (KeyValuePair<string, List<DataPoint>> series in distinctSeries)
			{
				Frame<DateTime, string> dataPointSeriesFrame = Frame.FromRecords(series.Value)
					.IndexRows<DateTime>(c_dateTimeReflectionName)
					.OrderRows();

				Frame<DateTime, string> frameWithMissingDates = defaultSeriesFrame.Join(dataPointSeriesFrame, JoinKind.Left);

				List<DataPoint> seriesWithMissingDates = frameWithMissingDates
					.GetSeries<decimal?>(c_valueReflectionName)
					.GetAllObservations()
					.Select(s => new DataPoint
					{
						DateTime = s.Key,
						SeriesName = series.Key,
						Value = s.Value.HasValue ? s.Value.Value : (useNullDefaultValue ? (decimal?)null : 0m)
					})
					.ToList();

				combinedSeries.AddRange(seriesWithMissingDates);
			}

			return combinedSeries;
		}

		private const string c_dateReflectionName = "Date";
		private const string c_dateTimeReflectionName = "DateTime";
		private const string c_valueReflectionName = "Value";
	}
}

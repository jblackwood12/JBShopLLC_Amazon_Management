using System;

namespace Utility.Models.Date
{
	public sealed class DataPoint
	{
		public DateTime DateTime { get; set; }

		public decimal? Value { get; set; }

		public string SeriesName { get; set; }
	}
}

using System.Collections.Generic;

namespace Utility.Models.Date
{
	public sealed class MovingAverageDataPointContainer
	{
		public List<DataPoint> CombinedSeries { get; set; }

		public List<DataPoint> MovingAverageSeries { get; set; }
	}
}

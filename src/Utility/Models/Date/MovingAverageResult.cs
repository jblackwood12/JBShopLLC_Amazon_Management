using System.Collections.Generic;
using System.Linq;

namespace Utility.Models.Date
{
	public sealed class MovingAverageResult
	{
		public MovingAverageDataPointContainer MovingAverageDataPointContainer { get { return m_movingAverageDataPointContainer; } }

		public MovingAverageType MovingAverageType { get { return m_movingAverageType; } }

		public MovingAverageResult(IEnumerable<DataPoint> series, IEnumerable<DataPoint> movingAverageSeries, MovingAverageType movingAverageType)
		{
			m_movingAverageDataPointContainer = new MovingAverageDataPointContainer
			{
				CombinedSeries = series.ToList(),
				MovingAverageSeries = movingAverageSeries.ToList()
			};

			m_movingAverageType = movingAverageType;
		}

		private readonly MovingAverageDataPointContainer m_movingAverageDataPointContainer;

		private readonly MovingAverageType m_movingAverageType;
	}
}

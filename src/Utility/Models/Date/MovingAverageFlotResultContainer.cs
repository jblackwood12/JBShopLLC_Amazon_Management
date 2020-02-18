using System.Collections.Generic;

namespace Utility.Models.Date
{
	public sealed class MovingAverageFlotResultContainer
	{
		public IEnumerable<FlotResult> CombinedSeries { get; set; }

		public FlotResult MovingAverageSeries { get; set; }
	}
}

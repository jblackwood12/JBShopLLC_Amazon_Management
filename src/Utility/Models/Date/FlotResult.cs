using System.Collections.Generic;

namespace Utility.Models.Date
{
	public sealed class FlotResult
	{
		public string Label
		{
			get { return m_label; }
		}

		public IEnumerable<object[]> Data
		{
			get { return m_data; }
		}

		public FlotResult(string label, IEnumerable<object[]> data)
		{
			m_label = label;
			m_data = data;
		}

		private readonly string m_label;
		private readonly IEnumerable<object[]> m_data;
	}
}

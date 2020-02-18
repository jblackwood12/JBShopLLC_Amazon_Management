namespace Models
{
	public sealed class DimensionContainer
	{
		public decimal Length { get { return m_length; } }

		public decimal Width { get { return m_width; } }

		public decimal Height { get { return m_height; } }

		public decimal Weight { get { return m_weight; } }

		public DimensionContainer(decimal length, decimal width, decimal height, decimal weight)
		{
			m_length = length;
			m_width = width;
			m_height = height;
			m_weight = weight;
		}

		private readonly decimal m_length;
		private readonly decimal m_width;
		private readonly decimal m_height;
		private readonly decimal m_weight;
	}
}

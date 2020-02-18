using System;

namespace Models
{
	[AttributeUsage(AttributeTargets.Property)]
	public class Sanitize : Attribute
	{
		public Sanitize(InputType inputType, int maximumCharacters)
		{
			m_inputType = inputType;
			m_maximumCharacters = maximumCharacters;
		}

		public Sanitize(InputType inputType)
		{
			m_inputType = inputType;
			m_maximumCharacters = null;
		}

		public Sanitize(int maximumCharacters)
		{
			m_inputType = null;
			m_maximumCharacters = maximumCharacters;
		}

		public InputType? InputType { get { return m_inputType; } }
		public int? MaximumCharacters { get { return m_maximumCharacters; } }

		private readonly InputType? m_inputType;
		private readonly int? m_maximumCharacters;
	}
}
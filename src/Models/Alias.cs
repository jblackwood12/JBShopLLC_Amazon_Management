using System;

namespace Models
{
	[AttributeUsage(AttributeTargets.Property)]
	public class Alias : Attribute
	{
		public string[] AlternateFields { get { return m_alternateFields; } }

		public Alias(params string[] alternateFields)
		{
			m_alternateFields = alternateFields;
		}

		private readonly string[] m_alternateFields;
	}
}

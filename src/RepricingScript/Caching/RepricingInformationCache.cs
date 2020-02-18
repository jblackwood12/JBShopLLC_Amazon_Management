using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace RepricingScript.Caching
{
	public sealed class RepricingInformationCache
	{
		public RepricingInformationCache()
		{
			m_repricingInformations = new ReadOnlyCollection<RepricingInformation>(new List<RepricingInformation>());
		}

		public static RepricingInformationCache Instance
		{
			get
			{
				if (s_instance == null)
				{
					lock (s_syncRoot)
					{
						if (s_instance == null)
							s_instance = new RepricingInformationCache();
					}
				}

				return s_instance;
			}
		}

		public void ReplaceCache(List<RepricingInformation> repricingInformations)
		{
			Interlocked.Exchange(ref m_repricingInformations, new ReadOnlyCollection<RepricingInformation>(repricingInformations));
		}

		public ReadOnlyCollection<RepricingInformation> RepricingInformations { get { return m_repricingInformations; } }

		private static volatile RepricingInformationCache s_instance;
		private static readonly object s_syncRoot = new object();

		private ReadOnlyCollection<RepricingInformation> m_repricingInformations;
	}
}
using Data.Properties;

namespace Data
{
	internal partial class AmazonMWSdbDataContext
	{
		public AmazonMWSdbDataContext CreateReadInstance()
		{
			AmazonMWSdbDataContext dc = new AmazonMWSdbDataContext(Settings.Default.amazonmwsConnectionString);

			return dc;
		}

		public AmazonMWSdbDataContext CreateWriteInstance()
		{
			AmazonMWSdbDataContext dc = new AmazonMWSdbDataContext(Settings.Default.amazonmwsConnectionString);
			
			return dc;
		}
	}
}

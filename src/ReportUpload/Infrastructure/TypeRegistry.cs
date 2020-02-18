using AmazonProductLookup.Infrastructure;
using Data;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using StructureMap.ServiceLocatorAdapter;

namespace ReportUpload.Infrastructure
{
	public class TypeRegistry
	{
		public static IServiceLocator RegisterTypes()
		{
			Container container = new Container();
				
			container.Configure(x =>
				{
					x.AddRegistry(AmazonTypeRegistry.GetRegistry());
					x.For<AmazonMWSdbService>().Singleton().Use<AmazonMWSdbService>();
					x.For<ReportRetrieval>().Singleton().Use<ReportRetrieval>();
				});

			return new StructureMapServiceLocator(container);
		}
	}
}

using AmazonProductLookup.Infrastructure;
using Data;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using StructureMap.ServiceLocatorAdapter;

namespace RepricingScript.Infrastructure
{
	public static class TypeRegistry
	{
		public static IServiceLocator RegisterTypes()
		{
			Container container = new Container();

			container.Configure(x => x.AddRegistry(AmazonTypeRegistry.GetRegistry()));
			container.Configure(x => x.For<IAmazonMWSdbService>().Use(new AmazonMWSdbService()));
			container.Configure(x => x.For<RepricingScript>().Use<RepricingScript>());

			return new StructureMapServiceLocator(container);
		}
	}
}

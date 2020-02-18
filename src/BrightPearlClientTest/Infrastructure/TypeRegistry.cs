using BrightPearlClient.Infrastructure;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using StructureMap.ServiceLocatorAdapter;

namespace BrightPearlClientTest.Infrastructure
{
	public static class TypeRegistry
	{
		public static IServiceLocator RegisterTypes()
		{
			Container container = new Container();

			container.Configure(x => x.AddRegistry(BrightPearlTypeRegistry.GetRegistry()));

			return new StructureMapServiceLocator(container);
		}
	}
}

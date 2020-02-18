using AmazonProductLookup.Infrastructure;
using Data;
using StructureMap;

namespace WebApplication.Infrastructure
{
	public static class TypeRegistry
	{
		public static StructureMapDependencyResolver RegisterTypes()
		{
			Container container = new Container();

			container.Configure(x =>
				{
					x.AddRegistry(AmazonTypeRegistry.GetRegistry());
					x.For<IAmazonMWSdbService>().Use(new AmazonMWSdbService());
				});

			return new StructureMapDependencyResolver(container);
		}
	}
}
using StructureMap;
using StructureMap.Configuration.DSL;
using BrightPearlClient.BrightPearlApi;

namespace BrightPearlClient.Infrastructure
{
	public static class BrightPearlTypeRegistry
	{
		public static Registry GetRegistry()
		{
			Registry registry = new Registry();

			registry.For<IBrightPearlService>().Use(
				new BrightPearlService(
					Settings.Default.AppRef,
					Settings.Default.token,
					Settings.Default.ServiceURL));

			return registry;
		}

		public static Container RegisterTypes()
		{
			Container container = new Container();

			container.Configure(x => x.AddRegistry(GetRegistry()));

			return container;
		}
	}
}

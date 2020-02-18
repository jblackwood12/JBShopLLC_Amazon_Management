using System.Web.Http.Dependencies;
using StructureMap;

namespace WebApplication.Infrastructure
{
	public class StructureMapDependencyResolver : StructureMapDependencyScope, IDependencyResolver
	{
		#region Constructors and Destructors

		public StructureMapDependencyResolver(IContainer container)
			: base(container)
		{
		}

		#endregion

		#region Public Methods and Operators

		public IDependencyScope BeginScope()
		{
			IContainer child = Container.GetNestedContainer();
			return new StructureMapDependencyResolver(child);
		}

		#endregion
	}
}
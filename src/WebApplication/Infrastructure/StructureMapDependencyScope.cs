using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace WebApplication.Infrastructure
{
	public class StructureMapDependencyScope : ServiceLocatorImplBase, IDependencyScope
	{
		#region Constants and Fields

		protected readonly IContainer Container;

		#endregion

		#region Constructors and Destructors

		public StructureMapDependencyScope(IContainer container)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			Container = container;
		}

		#endregion

		#region Public Methods and Operators

		public void Dispose()
		{
			Container.Dispose();
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return Container.GetAllInstances(serviceType).Cast<object>();
		}

		#endregion

		#region Methods

		protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
			return Container.GetAllInstances(serviceType).Cast<object>();
		}

		protected override object DoGetInstance(Type serviceType, string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return serviceType.IsAbstract || serviceType.IsInterface
							? Container.TryGetInstance(serviceType)
							: Container.GetInstance(serviceType);
			}

			return Container.GetInstance(serviceType, key);
		}

		#endregion
	}
}

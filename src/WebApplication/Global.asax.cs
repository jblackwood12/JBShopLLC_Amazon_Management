using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApplication.App_Start;
using WebApplication.Infrastructure;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace WebApplication
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			// Https requires an SSL certificate installed locally.
			// Forces redirect to https only if the request is not local.
			if (HttpContext.Current.Request.IsSecureConnection.Equals(false) && HttpContext.Current.Request.IsLocal.Equals(false))
				Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.RawUrl);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			WebApiRouteRegistry.Register(GlobalConfiguration.Configuration);

			// Filter out unhandled exceptions for logging purposes.
			GlobalFilters.Filters.Add(new ExceptionFilter());

			// Do not remove this Attribute, this enables the [Authorize] attribute by default on all routes.
			// Applies to MVC routes
			GlobalFilters.Filters.Add(new AuthorizeAttribute());

			// Applies to WebApi routes
			GlobalConfiguration.Configuration.Filters.Add(new System.Web.Http.AuthorizeAttribute());

			StructureMapDependencyResolver serviceLocator = TypeRegistry.RegisterTypes();

			DependencyResolver.SetResolver(serviceLocator);
			GlobalConfiguration.Configuration.DependencyResolver = serviceLocator;

			RouteRegistry.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			////#if DEBUG
			////BundleTable.EnableOptimizations = false;
			////#else
			////	BundleTable.EnableOptimizations = true;
			////#endif

		}
	}
}
using System.Web.Http;

namespace WebApplication.Infrastructure
{
	public static class WebApiRouteRegistry
	{
		public static void Register(HttpConfiguration config)
		{
			// WebApi
			config.Routes.MapHttpRoute("DefaultAPI", "api/{controller}/{action}/");

			config.Filters.Add(new AuthorizeAttribute());

			// For development enable CORS, in production disallow it.
			#if DEBUG
				config.EnableCors();
			#endif
		}
	}
}

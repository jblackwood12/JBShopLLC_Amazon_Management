using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication.Infrastructure
{
	public class RouteRegistry
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "ProductReport",
				url: "Product/Report/{asin}",
				defaults: new {controller = "Product", action = "Report"}
				);

			routes.MapRoute(
				name: "ProductEdit",
				url: "Product/Edit/{asin}",
				defaults: new {controller = "Product", action = "Edit"}
				);

			routes.MapRoute(
				name: "ProductUpdate",
				url: "Product/EditProduct",
				defaults: new {controller = "Product", action = "EditProduct"}
				);

			routes.MapRoute(
				name: "ProductEvaluate",
				url: "Product/Evaluate/{asin}",
				defaults: new {controller = "Product", action = "Evaluate"}
				);

			routes.MapRoute(
				name: "ProductEvaluateCalculate",
				url: "Product/EvaluateCalculate",
				defaults: new {controller = "Product", action = "EvaluateCalculate"}
				);

			routes.MapRoute(
				name: "SalesReport",
				url: "Report/Sales",
				defaults: new {controller = "Report", action = "Sales"}
				);

			routes.MapRoute(
				name: "InventoryReport",
				url: "Report/Inventory",
				defaults: new { controller = "Report", action = "Inventory" }
				);

			routes.MapRoute(
				name: "GetInventoryReport",
				url: "Report/GetInventoryReport",
				defaults: new { controller = "Report", action = "GetInventoryReport" }
				);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional}
				);

			routes.MapRoute(
				name: "Login",
				url: "Account/Login",
				defaults: new {controller = "Account", action = "Login"}
				);
		}
	}
}
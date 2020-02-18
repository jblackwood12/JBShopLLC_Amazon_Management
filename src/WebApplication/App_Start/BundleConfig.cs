using System.Web.Optimization;

namespace WebApplication.App_Start
{
	public class LessTransform : IBundleTransform
	{
		public void Process(BundleContext context, BundleResponse response)
		{
			response.Content = dotless.Core.Less.Parse(response.Content);
			response.ContentType = "text/css";
		}
	}

	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			// Scripts
			bundles.Add(new ScriptBundle("~/bundles/js/jquery")
				.Include("~/Scripts/jquery/jquery-{version}.js")
				.Include("~/Scripts/jquery/jquery-ui-{version}.js")
				.Include("~/Scripts/jquery/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/js/angularjs")
				.Include("~/Scripts/angular.js")
				.Include("~/Scripts/angular-*"));

			bundles.Add(new ScriptBundle("~/bundles/js/typeahead")
				.Include("~/Scripts/typeahead/typeahead.bundle.js")
				.Include("~/Scripts/typeahead/handlebars.js"));

			bundles.Add(new ScriptBundle("~/bundles/js/bootstrap")
				.Include("~/Scripts/bootstrap/bootstrap.js"));

			bundles.Add(new ScriptBundle("~/bundles/js/misc")
				.Include("~/Scripts/data/_Layout.js")
				.Include("~/Scripts/flot/jquery.flot*")
				.Include("~/Scripts/flot/jquery.flot.tooltip.min")
				.Include("~/Scripts/accounting/accounting.js")
				.Include("~/Scripts/moment/moment.min.js"));

			// Styles
			Bundle lessBundle = new StyleBundle("~/bundles/css/less").Include("~/Content/site.less");
			lessBundle.Transforms.Add(new LessTransform());
			lessBundle.Transforms.Add(new CssMinify());
			bundles.Add(lessBundle);

			bundles.Add(new StyleBundle("~/bundles/css/bootstrap")
				.Include("~/Content/bootstrap/bootstrap.css")
				.Include("~/Content/font-awesome/css/font-awesome.css")
				.Include("~/Content/bootstrap/navbar.css")
				);

			bundles.Add(new StyleBundle("~/bundles/css/typeahead")
				.Include("~/Content/typeahead/typeahead.css"));

			bundles.Add(new StyleBundle("~/bundles/css/jqueryui")
					.Include(
						"~/Content/themes/base/jquery.ui.core.css",
						"~/Content/themes/base/jquery.ui.resizable.css",
						"~/Content/themes/base/jquery.ui.selectable.css",
						"~/Content/themes/base/jquery.ui.accordion.css",
						"~/Content/themes/base/jquery.ui.autocomplete.css",
						"~/Content/themes/base/jquery.ui.button.css",
						"~/Content/themes/base/jquery.ui.dialog.css",
						"~/Content/themes/base/jquery.ui.slider.css",
						"~/Content/themes/base/jquery.ui.tabs.css",
						"~/Content/themes/base/jquery.ui.datepicker.css",
						"~/Content/themes/base/jquery.ui.progressbar.css",
						"~/Content/themes/base/jquery.ui.theme.css"));
		}
	}
}
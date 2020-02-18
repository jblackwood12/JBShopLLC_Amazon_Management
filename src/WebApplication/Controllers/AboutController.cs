using System.Web.Mvc;

namespace WebApplication.Controllers
{
	public class AboutController : Controller
	{
		[AllowAnonymous]
		public ActionResult Index()
		{
			return View();
		}
	}
}
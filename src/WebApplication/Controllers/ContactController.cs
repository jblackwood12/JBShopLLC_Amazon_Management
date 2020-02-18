using System.Web.Mvc;

namespace WebApplication.Controllers
{
	public class ContactController : Controller
	{
		[AllowAnonymous]
		public ActionResult Index()
		{
			return View();
		}
	}
}
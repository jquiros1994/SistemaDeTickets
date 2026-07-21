using System.Web.Mvc;

namespace ITSupport.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UserId"] != null)
                return RedirectToAction("Index", "Dashboard");

            return View("SignIn");
        }
    }
}
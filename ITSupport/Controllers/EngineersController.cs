using System.Web.Mvc;
using ITSupport.Business;

namespace ITSupport.Controllers
{
    public class EngineersController : Controller
    {
        private readonly EngineerBusiness _engineers = new EngineerBusiness();

        private bool IsAuthenticated() => Session["UserId"] != null;

        private void LoadSessionInfo()
        {
            ViewBag.Username = Session["Username"];
            ViewBag.Role = Session["Role"];
        }

        // GET /Engineers
        public ActionResult Index()
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            LoadSessionInfo();
            var engineers = _engineers.GetAll();
            return View(engineers);
        }
    }
}

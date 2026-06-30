using System.Web.Mvc;
using ITSupport.DAL;

namespace ITSupport.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserRepository      _users     = new UserRepository();
        private readonly DashboardRepository _dashboard = new DashboardRepository();

        // POST /Dashboard/Index  — called by the Sign In form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Email and password are required.";
                return RedirectToAction("Index", "Home");
            }

            var user = _users.ValidateUser(email, password);

            if (user == null)
            {
                TempData["Error"] = "Invalid email or password.";
                return RedirectToAction("Index", "Home");
            }

            _users.UpdateLastLogin(user.UserId);

            Session["UserId"]     = user.UserId;
            Session["Username"]   = user.Username;
            Session["Email"]      = user.Email;
            Session["Role"]       = user.RoleName;
            Session["CustomerId"] = user.CustomerId;
            Session["EngineerId"] = user.EngineerId;

            return RedirectToAction("Index");
        }

        // GET /Dashboard
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Home");

            ViewBag.Username        = Session["Username"];
            ViewBag.Role            = Session["Role"];
            ViewBag.OpenCases       = _dashboard.GetOpenCasesCount();
            ViewBag.CriticalCases   = _dashboard.GetCriticalCasesCount();
            ViewBag.ResolvedToday   = _dashboard.GetResolvedTodayCount();
            ViewBag.ActiveEngineers = _dashboard.GetActiveEngineersCount();
            ViewBag.RecentCases     = _dashboard.GetRecentCases(10);

            return View();
        }

        // GET /Dashboard/SignOut
        public ActionResult SignOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

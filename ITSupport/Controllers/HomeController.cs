using System.Web.Mvc;
using ITSupport.Business;

namespace ITSupport.Controllers
{
    public class HomeController : Controller
    {
        private readonly DashboardBusiness _dashboard = new DashboardBusiness();

        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UserId"] != null)
                return RedirectToAction("Index", "Dashboard");

            return View("SignIn");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["Error"] = "Email and new password are required.";
                return RedirectToAction("ForgotPassword");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return RedirectToAction("ForgotPassword");
            }

            if (newPassword.Length < 6)
            {
                TempData["Error"] = "Password must be at least 6 characters.";
                return RedirectToAction("ForgotPassword");
            }

            var updated = _dashboard.ResetPassword(email, newPassword);

            if (!updated)
            {
                TempData["Error"] = "No account found with that email.";
                return RedirectToAction("ForgotPassword");
            }

            TempData["Success"] = "Password updated. You can now sign in.";
            return RedirectToAction("Index");
        }
    }
}
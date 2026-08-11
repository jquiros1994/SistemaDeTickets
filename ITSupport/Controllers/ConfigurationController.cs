using System.Collections.Generic;
using System.Web.Mvc;
using ITSupport.Business;

namespace ITSupport.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ConfigurationBusiness _configuration = new ConfigurationBusiness();

        private bool IsAuthenticated() => Session["UserId"] != null;

        private void LoadSessionInfo()
        {
            ViewBag.Username = Session["Username"];
            ViewBag.Role = Session["Role"];
        }

        // GET /Configuration
        public ActionResult Index()
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            LoadSessionInfo();
            var user = _configuration.GetById((int)Session["UserId"]);
            return View(user);
        }

        // POST /Configuration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username, string email, string newPassword, string confirmPassword)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            int userId = (int)Session["UserId"];

            if (!string.IsNullOrEmpty(newPassword) && newPassword != confirmPassword)
            {
                LoadSessionInfo();
                ViewBag.Errors = new List<string> { "New password and confirmation do not match." };
                return View(_configuration.GetById(userId));
            }

            var result = _configuration.UpdateProfile(userId, username, email, newPassword);
            if (!result.Success)
            {
                LoadSessionInfo();
                ViewBag.Errors = result.Errors;
                return View(_configuration.GetById(userId));
            }

            Session["Username"] = username.Trim();
            Session["Email"] = email.Trim();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }
    }
}

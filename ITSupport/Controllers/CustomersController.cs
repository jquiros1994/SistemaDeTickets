using System.Web.Mvc;
using ITSupport.Business;
using ITSupport.Models;

namespace ITSupport.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CustomerBusiness _customers = new CustomerBusiness();

        private bool IsAuthenticated() => Session["UserId"] != null;

        private void LoadSessionInfo()
        {
            ViewBag.Username = Session["Username"];
            ViewBag.Role = Session["Role"];
        }

        // GET /Customers
        public ActionResult Index()
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            LoadSessionInfo();
            var customers = _customers.GetAll();
            return View(customers);
        }

        // GET /Customers/Create
        public ActionResult Create()
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            LoadSessionInfo();
            ViewBag.Programs = _customers.GetPrograms();
            return View();
        }

        // POST /Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer model)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            var result = _customers.Create(model);
            if (!result.Success)
            {
                LoadSessionInfo();
                ViewBag.Programs = _customers.GetPrograms();
                ViewBag.Errors = result.Errors;
                return View(model);
            }

            TempData["Success"] = "Customer created successfully.";
            return RedirectToAction("Index");
        }
    }
}

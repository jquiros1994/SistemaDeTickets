using System.Web.Mvc;
using ITSupport.Business;
using ITSupport.Models;

namespace ITSupport.Controllers
{
    public class CasesController : Controller
    {
        private readonly CaseBusiness _cases = new CaseBusiness();

        private bool IsAuthenticated() => Session["UserId"] != null;

        // GET /Cases
        public ActionResult Index(int? statusId, string search)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            ViewBag.Username   = Session["Username"];
            ViewBag.Role       = Session["Role"];
            ViewBag.Statuses = _cases.GetStatuses();
            ViewBag.StatusId   = statusId;
            ViewBag.Search     = search;

            var cases = _cases.GetAllWithDetails(statusId, search);
            return View(cases);
        }

        // GET /Cases/Details/5
        public ActionResult Details(int id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            var vm = _cases.GetByIdWithDetails(id);
            if (vm == null) return HttpNotFound();

            ViewBag.Username = Session["Username"];
            ViewBag.Role     = Session["Role"];
            return View(vm);
        }

        // GET /Cases/Create
        public ActionResult Create()
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            ViewBag.Username   = Session["Username"];
            ViewBag.Role       = Session["Role"];
            ViewBag.Statuses = _cases.GetStatuses();
            ViewBag.Priorities = _cases.GetPriorities();
            ViewBag.Contacts = _cases.GetContacts();
            ViewBag.Engineers = _cases.GetEngineers();
            ViewBag.Programs = _cases.GetPrograms(); ;

            return View();
        }

        // POST /Cases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Case model)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            model.CreatedByUserId = (int)Session["UserId"];
            model.StatusId = model.StatusId == 0 ? 1 : model.StatusId; // default: New

            int newId = _cases.Create(model);
            return RedirectToAction("Details", new { id = newId });
        }
    }
}

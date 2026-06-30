using System.Web.Mvc;
using ITSupport.DAL;
using ITSupport.Models;

namespace ITSupport.Controllers
{
    public class CasesController : Controller
    {
        private readonly CaseRepository         _cases    = new CaseRepository();
        private readonly CaseStatusRepository   _statuses = new CaseStatusRepository();
        private readonly PriorityRepository     _priorities = new PriorityRepository();
        private readonly ContactRepository      _contacts = new ContactRepository();
        private readonly SupportEngineerRepository _engineers = new SupportEngineerRepository();
        private readonly ProgramRepository      _programs = new ProgramRepository();

        private bool IsAuthenticated() => Session["UserId"] != null;

        // GET /Cases
        public ActionResult Index(int? statusId, string search)
        {
            if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

            ViewBag.Username   = Session["Username"];
            ViewBag.Role       = Session["Role"];
            ViewBag.Statuses   = _statuses.GetAll();
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
            ViewBag.Statuses   = _statuses.GetAll();
            ViewBag.Priorities = _priorities.GetAll();
            ViewBag.Contacts   = _contacts.GetAll();
            ViewBag.Engineers  = _engineers.GetAll();
            ViewBag.Programs   = _programs.GetAll();

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

            int newId = _cases.Insert(model);
            return RedirectToAction("Details", new { id = newId });
        }
    }
}

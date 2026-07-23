using System.Web.Mvc;
using ITSupport.Business;
using ITSupport.Models;

namespace ITSupport.Controllers
{
	// DP:MVC
	public class CasesController : Controller
	{
		// SOLID: DIP 
		private readonly CaseBusiness _cases = new CaseBusiness();

		private bool IsAuthenticated() => Session["UserId"] != null;

		// SOLID: SRP / DRY 
		private void LoadCatalogs()
		{
			ViewBag.Statuses = _cases.GetStatuses();
			ViewBag.Priorities = _cases.GetPriorities();
			ViewBag.Contacts = _cases.GetContacts();
			ViewBag.Engineers = _cases.GetEngineers();
			ViewBag.Programs = _cases.GetPrograms();
		}

		private void LoadSessionInfo()
		{
			ViewBag.Username = Session["Username"];
			ViewBag.Role = Session["Role"];
		}

		// GET /Cases
		public ActionResult Index(int? statusId, string search)
		{
			if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

			LoadSessionInfo();
			ViewBag.Statuses = _cases.GetStatuses();
			ViewBag.StatusId = statusId;
			ViewBag.Search = search;

			var cases = _cases.GetAllWithDetails(statusId, search);
			return View(cases);
		}

		// GET /Cases/Details/5
		public ActionResult Details(int id)
		{
			if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

			var vm = _cases.GetByIdWithDetails(id);
			if (vm == null) return HttpNotFound();

			LoadSessionInfo();
			return View(vm);
		}

		// GET /Cases/Create
		public ActionResult Create()
		{
			if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

			LoadSessionInfo();
			LoadCatalogs();
			return View();
		}

		// POST /Cases/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Case model)
		{
			if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

			model.CreatedByUserId = (int)Session["UserId"];

			// DP: Result Object 
			var result = _cases.Create(model);
			if (!result.Success)
			{
				LoadSessionInfo();
				LoadCatalogs();
				ViewBag.Errors = result.Errors;
				return View(model);
			}

			TempData["Success"] = "Caso creado correctamente.";
			return RedirectToAction("Details", new { id = result.RecordId });
		}

		// GET /Cases/Edit/5
		public ActionResult Edit(int id)
		{
			if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

			var model = _cases.GetById(id);
			if (model == null) return HttpNotFound();

			LoadSessionInfo();
			LoadCatalogs();
			return View(model);
		}

		// POST /Cases/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Case model)
		{
			if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

			var result = _cases.Update(model);
			if (!result.Success)
			{
				LoadSessionInfo();
				LoadCatalogs();
				ViewBag.Errors = result.Errors;
				return View(model);
			}

			TempData["Success"] = "Caso actualizado correctamente.";
			return RedirectToAction("Details", new { id = model.CaseId });
		}

		// POST /Cases/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id)
		{
			if (!IsAuthenticated()) return RedirectToAction("Index", "Home");

			var result = _cases.Delete(id);
			if (result.Success)
				TempData["Success"] = "Caso eliminado correctamente.";
			else
				TempData["Error"] = string.Join(" ", result.Errors);

			return RedirectToAction("Index");
		}
	}
}

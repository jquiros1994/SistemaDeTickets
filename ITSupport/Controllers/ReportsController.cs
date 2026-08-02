using System.Web.Mvc;
using ITSupport.Business;

namespace ITSupport.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportsBusiness _reports = new ReportsBusiness();

        // GET /Reports
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Home");

            ViewBag.Username = Session["Username"];
            ViewBag.Role     = Session["Role"];

            ViewBag.StatusCounts     = _reports.GetCaseCountsByStatus();
            ViewBag.PriorityCounts   = _reports.GetCaseCountsByPriority();
            ViewBag.ResolvedTrend    = _reports.GetResolvedTrend(14);
            ViewBag.EngineerWorkload = _reports.GetEngineerWorkload();
            ViewBag.SlaCompliance    = _reports.GetSlaCompliance();

            return View();
        }
    }
}

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using ITSupport.DAL;

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

        // Temporary: visit /Home/SeedPasswords once to set real password hashes, then remove this action.
        [HttpGet]
        public ActionResult SeedPasswords()
        {
            var seeds = new Dictionary<string, string>
            {
                { "admin@support.com",            "Admin123" },
                { "alex.rivera@support.com",       "Engineer123" },
                { "sara.kim@support.com",          "Engineer123" },
                { "pedro.vargas@support.com",      "Engineer123" },
                { "maria.gonzalez@acme.com",       "Customer123" },
                { "john.smith@globex.com",         "Customer123" },
                { "lucia.fernandez@initec.com",    "Customer123" },
            };

            int updated = 0;
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                foreach (var kv in seeds)
                {
                    var hash = PasswordHelper.Hash(kv.Value);
                    using (var cmd = new SqlCommand(
                        "UPDATE Users SET PasswordHash = @Hash WHERE Email = @Email", conn))
                    {
                        cmd.Parameters.AddWithValue("@Hash",  hash);
                        cmd.Parameters.AddWithValue("@Email", kv.Key);
                        updated += cmd.ExecuteNonQuery();
                    }
                }
            }

            return Content($"Done. {updated} users updated with real password hashes.");
        }
    }
}

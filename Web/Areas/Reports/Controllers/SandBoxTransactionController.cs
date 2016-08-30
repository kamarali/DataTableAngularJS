using System;
using System.Web.Mvc;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
    public class SandBoxTransactionController : Controller
    {
        //
        // GET: /Reports/SandBoxTransaction/
        [ISAuthorize(Business.Security.Permissions.Sandbox.SandboxTestingReportAccess)]
        public ActionResult SandBoxTransaction()
        {
            string currdate = DateTime.UtcNow.ToString("dd-MMM-yy");

            ViewData["CurrentDate"] = currdate;
            return View();
        }
    }
}

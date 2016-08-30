using System.Web.Mvc;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
    public class UserPermissionReportController : ISController
    {
      /// <summary>
      /// Following action is used to display User Permission report.
      /// </summary>
      /// <returns></returns> 
      [ISAuthorize(Business.Security.Permissions.Reports.UserPermissionReport.Access)] 
        public ActionResult UserPermissionReport()
         {
           ViewData["UserCategory"] = (int)SessionUtil.UserCategory;
           ViewData["MemberId"] = SessionUtil.MemberId;
            return View();
         }

        
    }
}
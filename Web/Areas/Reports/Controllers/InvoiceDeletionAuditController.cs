using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{

    public class InvoiceDeletionAuditController : ISController
    {

        public IMemberManager MemberManager { get; set; }
        private readonly ICalendarManager _calenderManager;

        public InvoiceDeletionAuditController(ICalendarManager calenderManager)
        {
            _calenderManager = calenderManager;
        }

        [ISAuthorize(Business.Security.Permissions.Reports.InvoceDeletionReport.Access)]
        public ActionResult InvoiceDeletionAudit()
        {
            var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
                //GetCurrentBillingPeriod();
            ViewData["currentYear"] = currentPeriod.Year;
            ViewData["currentMonth"] = currentPeriod.Month;
            return View();
        }

        public JsonResult IsUserIdentificationInAuditTrail()
        {
            var iSDeletedByControlShow = MemberManager.IsUserIdentificationInAuditTrail(SessionUtil.MemberId);
            return Json(iSDeletedByControlShow);
        }

    }
}
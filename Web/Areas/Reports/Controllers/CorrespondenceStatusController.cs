using System;
using System.Web.Mvc;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class CorrespondenceStatusController : ISController
  {
    private readonly IMemberManager _memberManager;

    public CorrespondenceStatusController(IMemberManager memberManager)
    {
      _memberManager = memberManager;
    }

    [ISAuthorize(Business.Security.Permissions.Reports.Pax.CorrespondenceStatusAccess)]
    public ActionResult PaxCorrespondenceStatus()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var memberId = SessionUtil.MemberId;
      ViewData["Category"] = (int)BillingCategoryType.Pax;
      ViewData["CategoryName"] = "Passenger";
      ViewData["MemberId"] = _memberManager.GetMemberCodeAlpha(memberId) + "-" + _memberManager.GetMemberCode(memberId) + "" +
                             _memberManager.GetMemberCommercialName(memberId);

      string currdate = DateTime.UtcNow.ToString("dd-MMM-yy");

      ViewData["CurrentDate"] = currdate;

      return View("CorrespondenceStatus");
    }

    [ISAuthorize(Business.Security.Permissions.Reports.Misc.CorrespondenceStatusAccess)]
    public ActionResult MiscCorrespondenceStatus()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var memberId = SessionUtil.MemberId;
      ViewData["Category"] = (int)BillingCategoryType.Misc;
      ViewData["CategoryName"] = "Miscellaneous";
      ViewData["MemberId"] = _memberManager.GetMemberCodeAlpha(memberId) + "-" + _memberManager.GetMemberCode(memberId) + "" +
                             _memberManager.GetMemberCommercialName(memberId);
      string currdate = DateTime.UtcNow.ToString("dd-MMM-yy");

      ViewData["CurrentDate"] = currdate;

      return View("CorrespondenceStatus");
    }

    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.CorrespondenceStatusAccess)]
    public ActionResult CGOCorrespondenceStatus()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var memberId = SessionUtil.MemberId;
      ViewData["Category"] = (int)BillingCategoryType.Cgo;
      ViewData["CategoryName"] = "Cargo";
      ViewData["MemberId"] = _memberManager.GetMemberCodeAlpha(memberId) + "-" + _memberManager.GetMemberCode(memberId) + "" +
                             _memberManager.GetMemberCommercialName(memberId);
      string currdate = DateTime.UtcNow.ToString("dd-MMM-yy");

      ViewData["CurrentDate"] = currdate;

      return View("CorrespondenceStatus");

    } // End of CGOCorrespondenceStatus
  }
}
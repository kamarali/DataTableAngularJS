using System.Linq;
using System.Web.Mvc;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Profile;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using iPayables.UserManagement;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;

namespace Iata.IS.Web.Areas.Profile.Controllers
{
  public class AuditTrailController : Controller
  {
    //
    // GET: /Profile/AuditTrail/

    private readonly IFutureUpdatesManager _futureUpdatesManager;
    private readonly IMemberManager _IMemberManager;
    private readonly IUserManagement _IUserManagement;


    public AuditTrailController(IFutureUpdatesManager futureUpdatesManager, IMemberManager imemberManager, IUserManagement iUserManagement)
    {
      _futureUpdatesManager = futureUpdatesManager;
      _IMemberManager = imemberManager;
      _IUserManagement = iUserManagement;
    }

    [ISAuthorize(Business.Security.Permissions.Profile.ViewProfileChangesAccess)]
    public ActionResult AuditTrail()
    {
      ViewData["ReportTypeCategory"] = (int)UserCategory.Member;
      return View();
    }

    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Profile.ViewProfileChangesAccess)]
    public JsonResult AuditTrail(string elememntList, string fromdate, string todate)
    {
      UIMessageDetail details;
      try
      {
        var auditTrailGrid = new AuditTrailReport("auditTrail", Url.Action("AuditTrailReportData", "AuditTrail", new { elememntList, fromdate, todate }));
        ViewData["AuditTrailReport"] = auditTrailGrid.Instance;
        TempData[ViewDataConstants.SuccessMessage] = "Success";
        details = new UIMessageDetail
        {
          IsFailed = false,
          isRedirect = true,

          RedirectUrl = Url.Action("AuditTrailReport", "AuditTrail", new { area = "Profile", elememntList, fromdate, todate })

        };
      }
      catch (ISBusinessException)
      {
        details = new UIMessageDetail
        {
          IsFailed = true,

        };

      }
      return Json(details);
    }

   [ISAuthorize(Business.Security.Permissions.Profile.ViewIchProfileChangesAccess)]
    public ActionResult Ich()
    {
      ViewData["userCategory"] = _IUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;
      ViewData["ReportTypeCategory"] = (int) UserCategory.IchOps;
      ViewData["IsGroupNotVisible"] = true;
      return View();
    }

    /// <summary>
    /// Fetch data for Profile Update and display it in grid
    /// </summary>
    /// <returns />
    public JsonResult ProfileUpdateData()
    {

      var profileUpdateGrid = new ProfileUpdateReport("profileUpdate", Url.Action("ProfileUpdateData"));

      return null;

    }
    [ISAuthorize(Business.Security.Permissions.Profile.ViewAchProfileChangesAccess)]
    public ActionResult Ach()
    {
      ViewData["userCategory"] = _IUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;
      ViewData["ReportTypeCategory"] = (int)UserCategory.AchOps;
      ViewData["IsGroupNotVisible"] = true;
      return View();
    }

    [HttpPost]
    public JsonResult GetMemberUsers(string id)
    {
      var userList = _IMemberManager.GetUserList(System.Convert.ToInt32(id));  //SessionUtil.MemberId
      return Json(userList);
    }

    public ActionResult ProfileUpdateReport()
    {
      //Create grid instance for Profile Update grid
      var profileUpdateGrid = new ProfileUpdateReport("profileUpdate", Url.Action("ProfileUpdateData", "AuditTrail"));
      ViewData["ProfileUpdateGrid"] = profileUpdateGrid.Instance;
      return View();
    }

    public ActionResult AuditTrailReport(string elememntList, string fromdate, string todate)
    {
      //Create grid instance for Profile Update grid
      var auditTrailGrid = new AuditTrailReport("auditTrail", Url.Action("AuditTrailReportData", "AuditTrail", new { elememntList, fromdate, todate }));
      ViewData["AuditTrailReport"] = auditTrailGrid.Instance;
      return View();
    }

    /// <summary>
    /// Fetch data for Profile Update and display it in grid
    /// </summary>
    /// <returns />
    public JsonResult AuditTrailReportData(string elememntList, string fromdate, string todate)
    {

      var auditTrailGrid = new AuditTrailReport("auditTrail", Url.Action("AuditTrailReportData", "AuditTrail", new { elememntList, fromdate, todate }));
      var futureUpdateList = _futureUpdatesManager.GetFutureUpdatesList(elememntList, fromdate, todate);
      return auditTrailGrid.DataBind(futureUpdateList.AsQueryable());

    }
  }
}

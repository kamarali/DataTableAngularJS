using System.Web.Mvc;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using iPayables.UserManagement;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
    public class MemberLocationController : ISController
    {
      // SCP186215: Member Code Mismatch between Member and Location Details
      private readonly IUserManagement _iUserManagement;

      public MemberLocationController(IUserManagement iUserManagement)
      {
        _iUserManagement = iUserManagement;
      }

        // GET: /Misc/MemberLocation/
        [ISAuthorize(Business.Security.Permissions.Reports.InvReferenceData.Access)]
        public ActionResult MemberLocation()
        {
          // SCP186215: Member Code Mismatch between Member and Location Details
          //Get user category for logged in user.
          ViewData["UserCategoryID"] = _iUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;
            return View();
        }
    }
}

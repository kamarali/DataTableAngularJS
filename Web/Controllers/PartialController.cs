using System.Web.Mvc;
using Iata.IS.Business.Common;

namespace Iata.IS.Web.Controllers
{
  public class PartialController : Controller
  {
    public IUserManager UserManager { get; set; }

    [ChildActionOnly]
    [OutputCache(Duration = 5, VaryByParam = "userId")]
    public ActionResult HeaderBar(int userId)
    {
      return PartialView("HeaderBarControl");
    }

    [ChildActionOnly]
    [OutputCache(Duration = 600, VaryByParam = "userId")]
    public ActionResult Menu(int userId)
    {
      // Get the list of user permissions.
      var permissionList = UserManager.GetUserPermissions(userId);

      return PartialView("MenuControl", permissionList);
    }

    [ChildActionOnly]
    [OutputCache(Duration = 43200, VaryByParam = "none")]
    public ActionResult Footer()
    {
      return PartialView("FooterControl");
    }
  }
}

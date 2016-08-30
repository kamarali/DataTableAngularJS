using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Security;
using Iata.IS.Core.DI;
using log4net;

namespace Iata.IS.Web.Util.Filters
{
  public class ISAuthorizeAttribute : AuthorizeAttribute
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ISAuthorizeAttribute()
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public ISAuthorizeAttribute(int permission)
    {
      Permission = permission;
    }

    /// <summary>
    /// The permission that the user should have in order to be successfully authorized.
    /// </summary>
    public int Permission { get; set; }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      Logger.DebugFormat("Checking whether user id [{0}] is authorized for Permission [{1}].", SessionUtil.UserId, Permission);
      
      // If a blank permission has been passed then its unauthorized.
      if (Permission <= 0)
      {
        return false;
      }

      // Return accordingly.
      var authorizationManager = Ioc.Resolve<IAuthorizationManager>();
      return SessionUtil.IsLoggedIn && authorizationManager.IsAuthorized(SessionUtil.UserId, Permission);
    }
  }
}
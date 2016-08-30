using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Security;
using Iata.IS.Core.DI;
using log4net;

namespace Iata.IS.Web.Util.Filters
{
  public class HomeAuthorizeAttribute : AuthorizeAttribute
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Default constructor.
    /// </summary>
    public HomeAuthorizeAttribute()
    {
    }


    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      return SessionUtil.IsLoggedIn;
    }
  }
}
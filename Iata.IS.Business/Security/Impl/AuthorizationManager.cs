using System;
using System.Reflection;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using log4net;

namespace Iata.IS.Business.Security.Impl
{
  public class AuthorizationManager : IAuthorizationManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public IUserManager UserManager { get; set; }
    private readonly Guid _instanceId = Guid.NewGuid();

    /// <summary>
    /// To check the instance - whether singleton is working or not.
    /// </summary>
    public Guid InstanceId
    {
      get
      {
        return _instanceId;
      }
    }
    
    public bool IsAuthorized(int userId, int permissionId)
    {
      try
      {
        return UserManager.IsUserAuthorized(userId, permissionId);
      }
      catch (ISBusinessException ex)
      {
        Logger.Error(string.Format("Error while checking user permissions for user id [{0}] and permission id [{1}]", userId, permissionId), ex);
      }

      return false;
    }
  }
}
using System;
using System.Collections.Generic;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.DI;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Data;

namespace Iata.IS.Business.Common.Impl
{
  public class UserManager : IUserManager
  {
    /// <summary>
    /// UserRepository
    /// </summary>
    private readonly IUserRepository _userRepository;
 
    /// <summary>
    /// Initializes a new instance of the <see cref="UserManager"/> class.
    /// </summary>
    public UserManager()
    {
      _userRepository = Ioc.Resolve<IUserRepository>();
    }

    /// <summary>
    /// Adds the user permission.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="userPermissions">The user permissions.</param>
    public void SaveUserPermissions(int userId, IList<int> userPermissions)
    {
      _userRepository.SaveUserPermissions(userId, userPermissions);
    }

    /// <summary>
    /// Gets the user permission.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns></returns>
    public IList<int> GetUserPermissions(int userId)
    {
      var permissions = _userRepository.GetUserPermissions(userId);

      if (permissions == null)
      {
        var permissionManager = Ioc.Resolve<IPermissionManager>();
        permissions = permissionManager.GetUserPermissions(userId);
      }

      return permissions;
    }

    /// <summary>
    /// Removes the user permission.
    /// </summary>
    /// <param name="userId">The user id.</param>
    public void FlushUserPermissions(int userId)
    {
      _userRepository.RemoveUserPermission(userId);
    }

    public bool IsUserAuthorized(int userId, int permission)
    {
      var permissionManager = Ioc.Resolve<IPermissionManager>();
      var authorized = false;

      if (permissionManager != null)
      {
        authorized = permissionManager.IsUserAuthorized(userId, permission);
      }

      return authorized;
    }
    
    public List<User> GetUsersByMemberId(int memberId)
    {
      return _userRepository.GetUsersByMemberId(memberId);
    }
  }
}
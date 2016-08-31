using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
  public interface IUserManager
  {
    /// <summary>
    /// Adds the user permission to a repository.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="userPermissions">The user permissions.</param>
    void SaveUserPermissions(int userId, IList<int> userPermissions);

    /// <summary>
    /// Gets the user permission from a repository.
    /// </summary>
    /// <param name="userId">The user id.</param>
    IList<int> GetUserPermissions(int userId);

    /// <summary>
    /// Removes the user permissions from the repository.
    /// </summary>
    /// <param name="userId">The user id.</param>
    void FlushUserPermissions(int userId);

    /// <summary>
    /// Checks whether the user has the specified permission.
    /// </summary>
    /// <param name="userId">Id of the user.</param>
    /// <param name="permission">The permission to check</param>
    /// <returns>True if the user has the specified permission, false otherwise.</returns>
    bool IsUserAuthorized(int userId, int permission);

    /// <summary>
    /// Gets users  by member Id.
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    List<User> GetUsersByMemberId(int memberId);
  }
}
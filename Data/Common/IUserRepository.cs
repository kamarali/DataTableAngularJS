using System.Collections.Generic;
using Iata.IS.Model.Common;
namespace Iata.IS.Data.Common
{
  public interface IUserRepository
  {
    /// <summary>
    /// Adds the user permission.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="userPermissions">The user permissions.</param>
    void SaveUserPermissions(int userId, IList<int> userPermissions);

    /// <summary>
    /// Gets the user permission.
    /// </summary>
    /// <param name="userId">The user id.</param>
    IList<int> GetUserPermissions(int userId);

    /// <summary>
    /// Removes the user permission.
    /// </summary>
    /// <param name="userId">The user id.</param>
    void RemoveUserPermission(int userId);

    /// <summary>
    /// Gets users by member Id.
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    List<User> GetUsersByMemberId(int memberId);

  }
}
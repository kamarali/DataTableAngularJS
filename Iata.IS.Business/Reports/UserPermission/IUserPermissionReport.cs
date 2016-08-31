using System;
using System.Data;

namespace Iata.IS.Business.Reports.UserPermission
{
  public interface IUserPermissionReport
  {
    /// <summary>
    /// fetch user permission for generating report
    /// </summary>
    /// <param name="userCategory">user category. ex SISOps, Member users etc</param>
    /// <param name="memberId">member id</param>
    /// <param name="userName">user name i.e. email address</param>
    /// <returns>return result set</returns>
    DataTable GetUserPermissionReport(int userCategory, int memberId, string userName);
  }
}

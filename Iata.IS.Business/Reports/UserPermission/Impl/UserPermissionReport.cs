using System;
using System.Data;
using Iata.IS.Data.Reports.UserPermission;

namespace Iata.IS.Business.Reports.UserPermission.Impl
{
  public class UserPermissionReport : IUserPermissionReport
  {

    private IUserPermissionData UserPermissionData { get; set; }

    public UserPermissionReport(IUserPermissionData userPermissionData)
    {
      UserPermissionData = userPermissionData;
    }

    /// <summary>
    /// fetch user permission for generating report
    /// </summary>
    /// <param name="userCategory">user category. ex SISOps, Member users etc</param>
    /// <param name="memberId">member id</param>
    /// <param name="userName">user name i.e. email address</param>
    /// <returns>return result set</returns>
    public DataTable GetUserPermissionReport(int userCategory, int memberId, string userName)
    {
      return UserPermissionData.GetUserPermissionReportData(userCategory, memberId, userName);
    }
  }
}

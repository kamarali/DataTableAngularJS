using System.Collections.Generic;
using System.Data;
using Devart.Data.Oracle;

namespace Iata.IS.Data.Reports.UserPermission.Impl
{
  public class UserPermissionData:IUserPermissionData
  {

     private const string UserReportPermissionProc = "PROC_USER_REPORT_PERMISSION";
     private const string UserCategory = "P_USER_CATEGORY";
     private const string MemberId = "P_MEMBER_ID";
     private const string EmailId = "P_EMAIL_ID_IN";

    /// <summary>
    /// fetch user permission for generating report
    /// </summary>
    /// <param name="userCategory">user category. ex SISOps, Member users etc</param>
    /// <param name="memberId">member id</param>
    /// <param name="userName">user name i.e. email address</param>
    /// <returns>return result set</returns>
    public DataTable GetUserPermissionReportData(int userCategory, int memberId, string userName)
    {
     var connection = new OracleConnection(Core.Configuration.ConnectionString.Instance.ServiceConnectionString);
      try
      {
        connection.Open();

        var command = new OracleCommand(UserReportPermissionProc, connection)
        {
          CommandType = CommandType.StoredProcedure
        };

        command.Parameters.Add(UserCategory, userCategory);
        command.Parameters[UserCategory].Direction = System.Data.ParameterDirection.Input;

        command.Parameters.Add(MemberId, memberId);
        command.Parameters[MemberId].Direction = System.Data.ParameterDirection.Input;

        command.Parameters.Add(EmailId, userName);
        command.Parameters[EmailId].Direction = System.Data.ParameterDirection.Input;

        var ds = new DataSet();
        var adapter = new OracleDataAdapter(command);

        adapter.Fill(ds);

        connection.Close();

        return ds.Tables[0];
      }
      finally
      {
        connection.Close();
      }
    }
  }
}
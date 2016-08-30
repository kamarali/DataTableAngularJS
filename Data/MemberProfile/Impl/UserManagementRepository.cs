using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Data.Impl;
using System.Data.Objects;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class UserManagementRepository : Repository<InvoiceBase>, IUserManagementRepository
  {

    public ISUserPassReset GetIsUserPassResetData(Guid linkid, int timeFrame)
    {
      try
      {
        var parameters = new ObjectParameter[2];
        parameters[0] = new ObjectParameter("LINK_ID_I", linkid);
        parameters[1] = new ObjectParameter("TIMEFRAME_I", timeFrame);
        var list =
            ExecuteStoredFunction<ISUserPassReset>("GetUserPasswordReset", parameters);
        return list.FirstOrDefault();
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return null;
    }


    public bool UpdateIsUserPassResetData(ISUserPassReset isUserPass)
    {
      try
      {
        var parameters = new ObjectParameter[4];
        parameters[0] = new ObjectParameter("IS_USER_ID_I", isUserPass.IsUserId);
        parameters[1] = new ObjectParameter("LINK_ID_I", isUserPass.LinkId);
        parameters[2] = new ObjectParameter("USED_I", isUserPass.Used);
        parameters[3] = new ObjectParameter("RESULT_O", typeof(int));
        ExecuteStoredProcedure("UpdateUserPasswordReset", parameters);
        return Convert.ToBoolean(parameters[3].Value);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return false;
    }


  }
}

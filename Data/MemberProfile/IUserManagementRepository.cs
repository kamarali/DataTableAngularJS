using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
  public interface IUserManagementRepository
  {
    ISUserPassReset GetIsUserPassResetData(Guid linkid, int timeFrame);

    bool UpdateIsUserPassResetData(ISUserPassReset isUserPass);
  }
}

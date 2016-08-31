using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface ISisMemberStatusManager
    { 
        SisMemberStatus AddSisMemberStatus(SisMemberStatus sisMemberStatus);

        SisMemberStatus UpdateSisMemberStatus(SisMemberStatus sisMemberStatus);

        bool DeleteSisMemberStatus(int sisMemberStatusId);

        SisMemberStatus GetSisMemberStatusDetails(int sisMemberStatusId);

        List<SisMemberStatus> GetAllSisMemberStatusList();

        List<SisMemberStatus> GetSisMemberStatusList(string sisMemberStatus, string description);

    }
}

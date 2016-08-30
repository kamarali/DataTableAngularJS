using System;
using System.Collections.Generic;
using System.Data;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
    public interface IDownloadMemberDetailsRepository : IRepository<Member>
    {
        DataSet GetMemberDetails();
    }
}

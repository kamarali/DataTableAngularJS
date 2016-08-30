using System;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.MemberLocations;

namespace Iata.IS.Data.Reports.MemberLocations
{
   public interface IMemberLocationData : IRepository<InvoiceBase>
    {
       List<MemberLocationModel> GetMemberLocationDetails(int memberId, string locationId);
    }
}


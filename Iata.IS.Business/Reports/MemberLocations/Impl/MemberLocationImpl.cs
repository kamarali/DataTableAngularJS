using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Data.Reports.MemberLocations;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.Reports.MemberLocations;

namespace Iata.IS.Business.Reports.MemberLocations.Impl
{
    public class MemberLocationImpl : IMemberLocation
    {
        private IMemberLocationData MemberLocationParam { get; set; }

        public MemberLocationImpl(IMemberLocationData MemberLocationData)
        {
            MemberLocationParam = MemberLocationData;
        }
        public List<MemberLocationModel> GetMemberLocationData(int memberId, string locationId)
        {
            return MemberLocationParam.GetMemberLocationDetails(memberId, locationId);                                                                
        }
    }
}



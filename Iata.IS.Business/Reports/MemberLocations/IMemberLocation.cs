using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Reports.MemberLocations;

namespace Iata.IS.Business.Reports.MemberLocations
{
   public interface IMemberLocation
    {
       
            /// <summary>
            /// Returns list of future updates Member Location records for passed search criteria
            /// </summary>
            /// <returns>List of Future Updates class object</returns>
       List<MemberLocationModel> GetMemberLocationData(int memberId, string locationId);
                  
   }
}




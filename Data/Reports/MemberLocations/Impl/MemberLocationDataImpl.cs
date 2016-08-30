using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Reports.MemberLocations;

namespace Iata.IS.Data.Reports.MemberLocations.Impl
{
   public class MemberLocationDataImpl :  Repository<InvoiceBase>, IMemberLocationData
    {
       public List<MemberLocationModel> GetMemberLocationDetails(int memberId, string locationId)
       {
           var parameters = new ObjectParameter[2];
           parameters[0] = new ObjectParameter(MemberLocationsDataImplConstants.MemberID, memberId);
           parameters[1] = new ObjectParameter(MemberLocationsDataImplConstants.LocationID, locationId);

           var list = ExecuteStoredFunction<MemberLocationModel>(MemberLocationsDataImplConstants.GetMemberLocationDetails, parameters);


           return list.ToList();
       }
    }
}
   

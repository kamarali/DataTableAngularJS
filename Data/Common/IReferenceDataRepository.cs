using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.Common
{
    public interface IReferenceDataRepository : IRepository<Permission> 
    {
        /// <summary>
        /// Get Member reference data.
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="billingCategory"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        MemberReferenceData GetMemberReferenceData(string locationId, int billingCategory,int memberId);
    }
}

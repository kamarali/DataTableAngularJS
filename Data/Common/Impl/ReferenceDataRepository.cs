using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.Common.Impl
{
    public class ReferenceDataRepository : Repository<Permission>, IReferenceDataRepository
    {
        /// <summary>
        /// Get Member Reference Data.
        /// </summary>
        /// <param name="locationId">location code to search</param>
        ///  /// <param name="billingCategory"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public MemberReferenceData GetMemberReferenceData(string locationId, int billingCategory, int memberId)
        {
            var parameters = new ObjectParameter[3];

            parameters[0] = new ObjectParameter("LOCATION_ID_I", typeof(string))
            {
                Value = locationId
            };
            parameters[1] = new ObjectParameter("BILLING_CATEGORY_I", typeof(int))
            {
                Value = billingCategory
            };
            parameters[2] = new ObjectParameter("MEMBER_ID_I", typeof(int))
            {
                Value = memberId
            };

            var referenceData = ExecuteStoredFunction<MemberReferenceData>("GetMemberReferenceData", parameters);

            return referenceData.SingleOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.ValidationDetails;

namespace Iata.IS.Data.Reports.ValidationDetails
{
    public interface IValidationDetailsData : IRepository<InvoiceBase>
  {
        List<ValidationDetailsModel> GetValidationDetails(int clearanceMonth, int billingPeriod, string fileName, DateTime billingSubmissiondate, string BilledOrganisation, string InvoiceNo, string category, string memberCode);
  }
}

using System;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.ValidationSummary;

namespace Iata.IS.Data.Reports.ValidationSummary
{
    public interface IPaxValidationSummary : IRepository<InvoiceBase>
  {
        List<PassengerValidationSummary> GetPaxValidationSummary(int clearanceMonth, int billingPeriod, string fileName, DateTime billingSubmissiondate, string memberCode, string category);
  }
}

using System;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Reports.ValidationSummary;

namespace Iata.IS.Business.Reports.ValidationSummary

{
  public interface IValidationSummary
  {
      /// <summary>
      /// Returns list of future updates audit trail records for passed search criteria
      /// </summary>
      /// <returns>List of Future Updates class object</returns>
      List<PassengerValidationSummary> GetPaxValidationSummary(int clearanceMonth,int billingPeriod, string fileName , DateTime billingSubmissiondate, string memberCode , string category);
  }
}
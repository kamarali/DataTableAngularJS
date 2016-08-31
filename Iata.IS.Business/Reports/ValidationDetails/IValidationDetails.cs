using System;
using System.Collections.Generic;
using Iata.IS.Model.Reports.ValidationDetails;

namespace Iata.IS.Business.Reports.ValidationDetails
{
    public interface IValidationDetails
    {
        /// <summary>
        /// Returns list of future updates audit trail records for passed search criteria
        /// </summary>
        /// <returns>List of Future Updates class object</returns>
        List<ValidationDetailsModel> GetValidationDetails(int clearanceMonth, int billingPeriod, string fileName, DateTime billingSubmissiondate, string BilledOrganisation, string InvoiceNo, string category, string memberCode);
    }
}

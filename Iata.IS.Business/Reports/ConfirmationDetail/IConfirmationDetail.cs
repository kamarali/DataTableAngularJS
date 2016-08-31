using System;
using System.Collections.Generic;
using Iata.IS.Model.Reports.ConfirmationDetails;

namespace Iata.IS.Business.Reports.ConfirmationDetail
{
  public interface IConfirmationDetail
  {

    /// <summary>
    /// Returns list of future updates passenger confirmation records for passed search criteria
    /// </summary>
    /// <returns>List of Future Updates class object</returns>
    List<ConfirmationDetailModel> GetConfirmationDetails(int memberId, int clearanceMonth, int periodNo, int blingAirlineNo, int bledAirlineNo, string invoiceNo, string issuingAirline, string originalPMI, string validatedPMI, string AgreIndSupplied, string AgreIndValidated, string ATPCOReasonCode, int billingYear, int pageStartIndex, int pageEndIndex,int IsCountReq);
    /// <summary>
    /// This method is used to generate CSV for the validation confirmation details
    /// </summary>
    void AutoGenerateCSV();
  }
}

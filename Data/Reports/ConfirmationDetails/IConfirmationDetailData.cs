using System;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.ConfirmationDetails;

namespace Iata.IS.Data.Reports.ConfirmationDetails
{
   public interface IConfirmationDetailData : IRepository<InvoiceBase>
    {
       List<ConfirmationDetailModel> GetConfirmationDetails(int memberId, int clearanceMonth, int periodNo, int blingAirlineNo, int bledAirlineNo, string invvoiceNo, string issuingAirline, string originalPMI, string validatedPMI, string AgreIndSupplied, string AgreIndValidated, string ATPCOReasonCode, int billingyear, int PagestartIndex, int PageendIndex, int IsCountRequired);
    }
}

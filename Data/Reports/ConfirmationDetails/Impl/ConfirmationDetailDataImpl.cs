using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Reports.ConfirmationDetails;

namespace Iata.IS.Data.Reports.ConfirmationDetails.Impl
{
    public class ConfirmationDetailDataImpl : Repository<InvoiceBase>, IConfirmationDetailData
    {
        public List<ConfirmationDetailModel> GetConfirmationDetails(int confmemberId, int clearanceMonth, int periodNo, int blingAirlineNo, int bledAirlineNo, string invoiceNo, string issuingAirline, string originalPMI, string validatedPMI, string AgreIndSupplied, string AgreIndValidated, string ATPCOReasonCode, int billingyear, int PageStartIndex, int PageEndIndex, int IsCountRequired)
    {
         var parameters = new ObjectParameter[16];
         parameters[0] = new ObjectParameter(ConfirmationDetailDataImplConstants.memberId, confmemberId);
         parameters[1] = new ObjectParameter(ConfirmationDetailDataImplConstants.ClearanceMonth, clearanceMonth);
         parameters[2] = new ObjectParameter(ConfirmationDetailDataImplConstants.PeriodNo, periodNo);
         parameters[3] = new ObjectParameter(ConfirmationDetailDataImplConstants.BillingAirlineNo, blingAirlineNo);
         parameters[4] = new ObjectParameter(ConfirmationDetailDataImplConstants.BilledAirlineNo, bledAirlineNo);
         parameters[5] = new ObjectParameter(ConfirmationDetailDataImplConstants.InvoiceNo, invoiceNo);
         parameters[6] = new ObjectParameter(ConfirmationDetailDataImplConstants.IssuingAirlince, issuingAirline);
         parameters[7] = new ObjectParameter(ConfirmationDetailDataImplConstants.OrigonalPMI, originalPMI);
         parameters[8] = new ObjectParameter(ConfirmationDetailDataImplConstants.ValidatedPMI, validatedPMI);
         parameters[9] = new ObjectParameter(ConfirmationDetailDataImplConstants.AgreIndSupplied, AgreIndSupplied);
         parameters[10] = new ObjectParameter(ConfirmationDetailDataImplConstants.AgreIndValidated, AgreIndValidated);
         parameters[11] = new ObjectParameter(ConfirmationDetailDataImplConstants.ATPCOReasonCode, ATPCOReasonCode);
         parameters[12] = new ObjectParameter(ConfirmationDetailDataImplConstants.BillingYear ,billingyear);
         parameters[13] = new ObjectParameter(ConfirmationDetailDataImplConstants.PageStartIndex, PageStartIndex);
         parameters[14] = new ObjectParameter(ConfirmationDetailDataImplConstants.PageEndIndex, PageEndIndex);
         parameters[15] = new ObjectParameter(ConfirmationDetailDataImplConstants.IsCountRequired, IsCountRequired);
            
         var list = ExecuteStoredFunction<ConfirmationDetailModel>(ConfirmationDetailDataImplConstants.GetConfirmationDetails, parameters);
                       
         return list.ToList();
        }   
    }
}

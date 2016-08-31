using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.LateSubmission;
using Iata.IS.Model.Reports;

namespace Iata.IS.Business.LateSubmission
{
 public interface IProcessLateSubmissionManager
  {
   List<LateSubmissionMemberSummary> GetLateSubmittedInvoiceMemberSummary(string clearanceType);
   List<LateSubmittedInvoices> GetLateSubmittedInvoicesByMemberId(string clearanceType, int memberId);
   List<InvoiceBase> AcceptLateSubmittedInvoice(string invoiceIds);
   bool RejectLateSubmittedInvoice(string invoiceIds, int categoryId);
   List<InvoiceBase> GetLateSubmissionInvoicesByInvoiceIds(string invoiceIds);
   bool CloseLateSubmissionWindow(string type, BillingPeriod prevBillingDetails);
   //This code related to CMP 353 but it was commented out since Iata didn't want it to be included in CMP353
   //List<ProcessingDashboardInvoiceActionStatus> ClaimFailedInvoces();
   void AutomaticOpeningOfICHLateSubmissionWindow();
   void AutomaticOpeningOfACHLateSubmissionWindow();
   void AutomaticClosingOfICHLateSubmissionWindow(string clearenceHouse, BillingPeriod billingPeriod, DateTime actualDateTime);
   void AutomaticClosingOfACHLateSubmissionWindow();
   string CloseLateSubmissionWindow(string clearenceHouse, BillingPeriod billingPeriod, int userId, DateTime actualDateTime);
  }
}

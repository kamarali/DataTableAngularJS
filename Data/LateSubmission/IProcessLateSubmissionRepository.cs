using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.LateSubmission;

namespace Iata.IS.Data.LateSubmission
{
  public interface IProcessLateSubmissionRepository
  {
    //SCP 280475 - SRM: System Performance - ICH late submisson tab
    List<LateSubmissionMemberSummary> GetLateSubmittedInvoiceMemberSummary(string clearenceType, BillingPeriod billingPeriod);
    List<LateSubmittedInvoices> GetLateSubmittedInvoicesByMemberId(string clearenceType, int memberId, BillingPeriod billingPeriod);
    string AcceptLateSubmittedInvoice(string invoiceIds);
    //void SetInvoiceArchiveParameterForLateSubmission(Guid invoiceId,int currentBillingPeriod);
    List<LateSubmissionRejectInvoices> RejectLateSubmittedInvoice(string invoiceIds);
    List<LateSubmissionRejectInvoices> RejectLateSubmittedInvoiceOnLateSubmissionWindowClosing(string clearenceType, BillingPeriod prevBillingDetails);
  }
}

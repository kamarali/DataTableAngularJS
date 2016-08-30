namespace Iata.IS.Data.LateSubmission.Impl
{
  class ProcessLateSubmissionRepositoryConstants
  {
    public const string BillingYear = "BILLING_YEAR_i";
    public const string BillingMonth = "BILLING_MONTH_i";
    public const string BillingPeriod = "BILLING_PERIOD_i";
    public const string CurrentBillingPeriod = "CURRENT_BILLING_PERIOD_i";
    public const string ClearenceType = "CLEARENCE_TYPE_i";
    public const string BillingMemberId = "MEMBER_ID_i";
    public const string GetLateSubMemberDetail = "LateSubmissionMembersSummary";
    public const string GetLateSubMemberInvoice = "LateSubmittedInvoice";

    // accetp/reject/close invoice
    public const string InvoiceIds = "INVOICE_IDs_i";
    public const string InvoiceId = "INVOICE_ID_i";
    public const string AcceptLateSubmittedInvoice = "LateSubmissionAcceptInvoice";
    //public const string SetInvoiceLegalParameterForLateSubmission = "SetInvoiceLegalParameterForLateSubmission";
    public const string RejectLateSubmittedInvoice = "LateSubmissionRejectInvoice";
    public const string RejectLateSubmittedInvoiceOnLateSubClosing = "LateSubmissionWindowClosingRejectInvoice";


  }
}

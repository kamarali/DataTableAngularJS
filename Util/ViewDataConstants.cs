namespace Iata.IS.Web.Util
{
  public class ViewDataConstants
  {
    public const string RejectionMemoStage = "RejectionMemoStage";
    public const string BillingMemoCouponGrid = "BillingMemoCouponGrid";
    public const string CreditMemoGrid = "CreditMemoGrid";
    public const string CMCouponGrid = "CMCouponGrid";
    public const string ErrorMessage ="ErrorMessage";
    public const string SuccessMessage = "SuccessMessage";
    public const string IsBillingMember = "IsBillingMember";
    public const string SourceCodeGrid = "SourceCodeGrid";
    public const string PrimeBillingGrid = "PrimeBillingGrid";
    public const string BillingMemoGrid = "BillingMemoGrid";
    public const string BreakdownExists = "BreakdownExists";
    public const string TransactionExists = "TransactionExists";
    public const string PageMode = "PageMode";
    public const string BillingType = "BillingType";
    public const string FormCSummaryListGrid = "FormCSummaryListGrid";
    public const string FormCCouponListGrid = "FormCCouponListGrid";
    public const string FormCSearchResults = "FormCSearchResults";
    public const string SamplingConstant = "SamplingConstant";
    public const string FormFSummaryListGrid = "FormFSummaryListGrid";
    public const string RejectionMemoListGrid = "RejectionMemoListGrid";
    public const string RMCouponListGrid = "RMCouponListGrid";
    public const string NotBilateralSettlementMethod = "NotBilateralSettlementMethod";
    public const string SearchGrid = "searchGrid";
    public const string InvoiceVatGrid = "InvoiceVatGrid";
    public const string AvailableVatGrid = "AvailableVatGrid";
    public const string UnappliedAmountVatGrid = "UnappliedAmountVatGrid";
    public const string IsSubmittedStatus = "IsSubmittedStatus";
    public const string LineItemGrid = "LineItemGrid";
    public const string SubmittedErrorsGrid = "SubmittedErrorsGrid";
    public const string DerivedVatGrid = "DerivedVatGrid";
    public const string IsPostback = "IsPostback";
    public const string PrimeCouponRecord = "PrimeCouponRecord";
    public const string SamplingFormDRecord = "SamplingFormDRecord";
    public const string CorrespondenceRejectionsGrid = "CorrespondenceRejectionsGrid";
    public const string CorrespondenceHistoryGrid = "CorrespondenceHistoryGrid";
    public const string BHSearchResultsGrid = "BHSearchResultsGrid";
    public const string CorrespondenceTrailSearchResultGrid = "CorrespondenceTrailSearchResultGrid";
    public const string LineItemDetailGrid = "LineItemDetailGrid";
    public const string CorrespondenceSearch = "CorrespondenceSearch";
    public const string RejectionOnValidationFlag = "RejectionOnValidationFlag";

    public const string RetainLineItemDetailStartDate = "RetainLineItemDetailStartDate";
    public const string FieldMetaDataExists = "FieldMetaDataExists";
    public const string DefaultDigitalSignatureRequiredId = "DefaultDigitalSignatureRequiredId";

    public const string RejectedInvoiceNumberExist = "RejectedInvoiceNumberExist";
    public const string IsLocationCodePresent = "IsLocationCodePresent";
    public const string IsExceptionOccurred = "IsExceptionOccurred";
    public const string CorrespondenceInitiator = "CorrespondenceInitiator";
    public const string ReplyCorrespondence = "ReplyCorrespondence";

    public const string invoiceSearchCriteria = "invoiceSearchCriteria";
    public const string correspondenceSearchCriteria = "correspondenceSearchCriteria";
    public const string CorrespondenceTrailSearchCriteria = "CorrespondenceTrailSearchCriteria";
    
    public const string InvoiceNumber = "InvoiceNumber";
    public const string InvoiceId = "InvoiceId";
    public const string SettlementMethodId = "SMI";
    public const string IsSMILikeBilateral = "IsSMILikeBilateral";
    public const string BilateralSMIs = "BilateralSMIs";
    
    ///IS Calendar DataGrid
    public const string ISCalendarSearchGrid = "ISCalendarSearchGrid";
    public const string ISCalendarSearchYear = "ISCalenderSearchYear";
    public const string ISCalendarSearchMonth = "ISCalenderSearchMonth";
    public const string ISCalendarSearchPeriod = "ISCalenderSearchPeriod";

    // Billing History
    public const string FromBillingHistory = "FromBillingHistory";

    //Correct supporting documents search data
    public const string supportingDocumentBillingYearMonth = "supportingDocumentBillingYearMonth";
    public const string supportingDocumentBillingPeriod = "supportingDocumentBillingPeriod";
    public const string supportingDocumentBilledMember = "supportingDocumentBilledMember";
    public const string supportingDocumentInvoiceNo = "supportingDocumentInvoiceNo";
    public const string supportingDocumentFileName = "supportingDocumentFileName";
    public const string supportingDocumentSubmissionDate = "supportingDocumentSubmissionDate";
    public const string supportingDocumentSearchGrid = "supportingDocumentSearchGrid";
    
    //For Mismatch Transaction search result grid data
    public const string MismatchTransactionSearchGrid = "MismatchTransactionSearchGrid";
    public const string MismatchTransactionBillingYearMonth = "MismatchTransactionBillingYearMonth";
    public const string MismatchTransactionBillingPeriod = "MismatchTransactionBillingPeriod";
    public const string MismatchTransactionBilledMember = "MismatchTransactionBilledMember";
    public const string MismatchTransactionInvoiceNo = "MismatchTransactionInvoiceNo";
    public const string MismatchTransactionFileName = "MismatchTransactionFileName";
    public const string MismatchTransactionFormC = "MismatchTransactionFormC";
    public const string MismatchTransactionSequenceNumber = "MismatchTransactionSequenceNumber";
    public const string MismatchTransactionBatchNumber = "MismatchTransactionBatchNumber";
    public const string MismatchTransactionCouponBreakdownSerialNumber = "MismatchTransactionCouponBreakdownSerialNumber";
    public const string MismatchTransactionCases = "MismatchTransactionCases";
    public const string MismatchTransactionModel = "MismatchTransactionModel";

    //Supporting Document 
    public const string SupportingDocSearchResultGrid = "SupportingDocSearchResultGrid";
    public const string AttachmentResultGrid = "AttachmentResultGrid";

    public const string CurrentBillingPeriod = "currentBillingPeriod";

    // System Monitor
    public const string CompletedJobGrid = "CompletedJobGrid";
    public const string PendingJobGrid = "PendingJobGrid";
    public const string SystemAlertsJobGrid = "SystemAlertsJobGrid";
    public const string LoggedInUsersJobGrid = "LoggedInUsersJobGrid";
    public const string OutstandingItemsGrid = "OutstandingItemsGrid";
    public const string ISWEBResponseGrid = "ISWEBResponseGrid";
    public const string SysMonPendingInvoices = "SysMonPendingInvoices";



    //Cargo
    public const string RejectionMemoAwbPrepaidGrid = "RMAWBPrepaidGrid";
    public const string RejectionMemoAwbChargeCollectGrid = "RMAWBChargeCollectGrid";
    public const string RejectionMemoListingGrid = "RMListingGrid";
    public const string ValidationErrorCorrectionGrid = "ValidationErrorCorrection";
    public const string PayableInvoiceSearchGrid = "PayableInvoiceSearch";
    public const string PayableManageSupporingDocGrid = "PayableManageSupporingDoc";
    public const string CargoPayableSupportingDocSearchResultGrid = "CargoPayableSupportingDocSearchResultGrid";
    public const string ExceptionSummaryGrid = "ExceptionSummary";
    public const string SubTotalGrid = "SubTotalGrid";

    //UATP
    public const string UatpExceptionDetailsGrid = "UatpExceptionDetailsGrid";
    public const string UatpExceptionSummaryGrid = "UatpExceptionSummary";
    public const string IsChargeCodeTypeExists = "IsChargeCodeTypeExists";

    public const string BillingMemoListingGrid = "BMListingGrid";
    public const string BillingMemoAwbProrateLadderGrid = "BillingMemoAwbProrateLadderGrid";
    public const string RejectionMemoAwbProrateLadderGrid = "RejectionMemoAwbProrateLadderGrid";
    public const string BillingMemoAwbGrid = "BillingMemoAwbGrid";
    public const string BillingMemoAwbChargeCollectGrid = "BMAWBChargeCollectGrid";
    public const string CreditMemoAwbGrid = "CreditMemoAwbGrid";
    public const string CreditMemoAwbProrateLadderGrid = "CreditMemoAwbProrateLadderGrid";
    
    public const string AwbPrepaidRecord = "AwbPrepaidRecord";
    public const string AwbChargeCollectRecord = "AwbChargeCollectRecord";

    public const string RejectionMemoAwbGrid = "RejectionMemoAwbGrid";

    //LegalArchive
    public const string LegalArchiveSearchGrid = "LegalArchiveSearchGrid";
    public const string RetrivalJobSummaryGridViewData = "RetrivalJobSummaryGridViewData";
    public const string RetrivalJobDetailGridViewData = "RetrivalJobDetailGridViewData";
    //Pax
    public const string SamplingExceptionDetailsGrid = "SamplingExceptionDetailsGrid";
    public const string SamplingExceptionSummaryGrid = "SamplingExceptionSummaryGrid";

    /// <summary>
    /// CMP #533: RAM A13 New Validations and New Charge Code
    /// </summary>
    public const string IsProductIdDropDown = "IsProductIdDropDown";

    //CMP 527 : Constants declaration Start
    public const string CorrespondenceCanClose = "CorrespondenceCanClose";

    public const string CorrespondeneClosedScenario = "CorrespondenceClosedScenario";
	public const string ReportId = "ReportId";
	public const string RequestDateTime = "RequestDateTime";

    //CMP 527 : Constants declaration End

    //CMP573: Declare ViewData to carry Show/Hide reply button value
    public const string IsCorrespondenceEligibleForReply = "IsCorrEligibleForReply";

    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download.
    public const string LinkedRejectionMemoGridId = "LinkedRejectionMemoGridId";
  }
}

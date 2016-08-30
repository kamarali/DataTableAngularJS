namespace Iata.IS.Web.Util
{
  public class ControlIdConstants
  {
    public const string BillingYearMonthPeriodDropDown = "BillingYearMonthPeriod";
    public const string BillingYearMonthDropDown = "BillingYearMonth";
    public const string BillingCodeDropDown = "BillingCode";
    public const string FlightDate = "FlightDate";
    public const string ListingToBillingRate = "ListingToBillingRate";
    public const string ReasonCode = "ReasonCode";
    public const string BillingMemberLocationCode = "BillingMemberLocationCode";
    public const string BilledMemberLocationCode = "BilledMemberLocationCode";
    public const string MemberLocationCode = "MemberLocationCode";
    public const string BillingMemberLocationCodeForLoc = "BillingMemberLocationCodeForLoc";
    public const string BilledMemberLocationCodeForLoc = "BilledMemberLocationCodeForLoc";
    public const string SettlementMethodIdDropDown = "SettlementMethodId";
    public const string ListingCurrencyId = "ListingCurrencyId";
    public const string CurrencyId = "CurrencyId";
    public const string InvoiceListingCurrencyId = "InvoiceListingCurrencyId";
    public const string BillingCurrencyId = "BillingCurrencyId";
    public const string DigitalSignatureRequiredId = "DigitalSignatureRequiredId";
    public const string HandlingFeeTypeId = "HandlingFeeTypeId";
    public const string RejectionStage = "RejectionStage";

    public const string InvTemplateLanguage = "InvTemplateLanguage";

    public const string DateOfCarriage = "DateOfCarriage";
    public const string AwbDate = "AwbDate";
    public const string TransferDate = "TransferDate";
    public const string AwbSerialNumber = "AwbSerialNumber";
    public const string CorrespondenceOwnerId = "CorrespondenceOwnerId";
    public const string CorrespondenceOwnerName = "CorrespondenceOwnerName";
    public const string InitiatingMember = "InitiatingMember";
    public const string CorrespondenceStatusId = "CorrespondenceStatusId";
    public const string CorrespondenceSubStatusId = "CorrespondenceSubStatusId";
    public const string CorrespondenceRefNo = "CorrespondenceRefNo";

    public const string PositiveAmount = "positiveAmount";
    public const string FourDecimalPlaces = "fourDecimalPlaces";
    public const string NegativeAmount = "negativeAmount";
    public const string Percentage = "percentage";
    public const string NegativePercentage = "negativePercentage";

    public const string SettlementMethodStatusId = "SettlementMethodStatusId";
    public const string ErrorTypeId = "ErrorTypeId";


    // Prime Billing Amount fields
    // public const string CouponGrossValueOrApplicableLocalFare = "CouponGrossValueOrApplicableLocalFare";

    public const string TaxAmount = "TaxAmount";
    public const string VatAmount = "VatAmount";

    public const string InvoiceVatGridId = "InvoiceVatGrid";
    public const string AvailableVatGridId = "AvailableVatGrid";
    public const string UnappliedAmountVatGridId = "UnappliedAmountVatGrid";

    // Invoice header constants
    public const string InvoiceDate = "InvoiceDate";
    //CMP #624: ICH Rewrite-New SMI X Description: Added Invoice header constant for CH Due Date Field
    public const string ChDueDate = "ChDueDate";

    // Form C fields
    public const string ProvisionalBillingMonthDropdown = "ProvisionalBillingMonthYear";
    public const string FormCSummaryGridId = "FormCSummaryGrid";
    public const string FormCCouponGridId = "FormCCouponGrid";
    public const string ListingCurrency = "ListingCurrency";
    public const string FormCSearchGrid = "FormCSearchGrid";


    // Form D fields
    public const string ProvisionalBillingMonthFormDEDropdown = "FormDEProvisionalBillingMonth";
    public const string FormDSourceCodeGridId = "FormDSourceCodeGrid";
    public const string FormDGridId = "FormDGrid";
    public const string ProvisionalInvoiceGridId = "ProvisionalInvoiceGrid";

    // Form F fields
    public const string SamplingConstant = "SamplingConstant";
    public const string FormFSummaryGridId = "FormFSummaryGrid";
    public const string NetRejectAmountAfterSamplingConstant = "TotalNetRejectAmountAfterSamplingConstant";

    // Rejection Memo fields
    public const string RejectionMemoGridId = "RejectionMemoGridId";
    public const string RMCouponGridId = "RMCouponGridId";

    // Invoice Search Grid Id
    public const string SearchGrid = "searchGrid";

    // Form E fields
    public const string HiddenGrossValue = "hidGrossValue";
    public const string HiddenIscAmount = "hidIscAmt";
    public const string HiddenOtherCommission = "hidOtherCommissionAmt";
    public const string HiddenUatpmount = "hidUatpAmt";
    public const string HiddenHandlingFee = "hidHandlingFee";
    public const string HiddenTaxAmount = "hidTaxAmt";
    public const string HiddenVatAmount = "hidVatAmt";
    public const string HiddenListingToBillingRate = "hidListingToBillingRate";

    // Credit Memo Coupon List Grid Id
    public const string CreditMemoGrid = "CreditMemoGrid";
    public const string CreditMemoCouponGrid = "CreditMemoCouponGrid";

    public const string SourceCodeGridId = "SourceCodeGrid";
    public const string CouponGridId = "couponGrid";
    public const string TransactionGridId = "transactionGrid";
    public const string AttachmentIndicatorOriginal = "AttachmentIndicatorOriginal";
    public const string AttachmentIndicatorValidated = "AttachmentIndicatorValidated";
    public const string SuspendedInvoiceFlag = "SuspendedInvoiceFlag";

    // Misc. invoice header
    public const string TotalAddOnChargeAmount = "InvoiceSummary.TotalAddOnChargeAmount";
    public const string TotalMiscTaxAmount = "InvoiceSummary.TotalTaxAmount";
    public const string TotalMiscVatAmount = "InvoiceSummary.TotalVatAmount";
    public const string AmountInBillingCurrency = "InvoiceSummary.TotalAmount";
    public const string AmountInClearanceCurrency = "InvoiceSummary.TotalAmountInClearanceCurrency";

    public const string AdditionalDetailDropdown = "AdditionalDetailDropdown";
    public const string AdditionalDetailDropdownTemplate = "AdditionalDetailDropdownTemplate";
    public const string AdditionalDetailDescription = "AdditionalDetailDescription";
    public const string NoteDropdown = "NoteDropdown";
    public const string NoteDropdownTemplate = "NoteDropdownTemplate";
    public const string AdditionalDetailDescriptionTemplate = "AdditionalDetailDescriptionTemplate";
    public const string NoteDescription = "NoteDescription";
    public const string NoteDescriptionTemplate = "NoteDescriptionTemplate";
    public const string ChargeCategory = "ChargeCategoryId";

    public const string CorrespondenceRejectionsGridId = "CorrespondenceRejectionsGrid";
    public const string CorrespondenceHistoryGridId = "CorrespondenceHistoryGrid";
    public const string CityAirport = "CityAirportText";
    public const string CityAirportId = "CityAirportId";
    public const string LocationCode = "LocationCode";
    public const string PONumber = "PONumber";

    // Line Item 
    public const string ChargeCode = "ChargeCodeId";
    public const string LineNetTotal = "TotalNetAmount";
    public const string ChargeCodeTypeId = "ChargeCodeTypeId";
    public const string UomCodeId = "UomCodeId";
    public const string LineItemNumber = "LineItemNumber";
    public const string ServiceStartDate = "StartDate";
    public const string ServiceEndDate = "EndDate";
    public const string UnitPrice = "UnitPrice";

    // Line Item Grid
    public const string LineItemGridId = "LineItemGrid";
    public const string SubmittedErrorsGridId = "SubmittedErrorsGrid";
    public const string DerivedVatGridId = "DerivedVatGrid";

    // Tax dialog fields
    public const string TaxId = "TaxId";
    public const string TaxPercentage = "Percentage";
    public const string CalculatedAmount = "CalculatedAmount";
    public const string Description = "Description";
    public const string CategoryCode = "CategoryCode";
    public const string TaxBaseAmount = "Amount";
    public const string TaxDescription = "TaxDescription";
    public const string TAXSubType = "SubType";

    public const string VATBaseAmount = "VATBaseAmount";
    public const string VATPercent = "VATPercent";
    public const string VATCalculatedAmount = "VATCalculatedAmount";
    public const string VATDescription = "VATDescription";
    public const string VATId = "VATId";
    public const string VATCategoryCode = "VATCategoryCode";
    public const string VATSubType = "VATSubType";

    public const string BHSearchResultsGrid = "BHSearchResultsGrid";
    public const string CorrespondenceTrailSearchGrid = "CorrespondenceTrailSearchGrid";

    public const string AddCharge = "AddCharge";
    public const string ChargeableAmount = "ChargeableAmount";
    public const string AddChargePercentage = "AddChargePercentage";
    public const string AddChargeAmount = "AddChargeAmount";

    public const string AddChargeName = "Name"; // Do not change value. Used in javascript.
    public const string AddChargeId = "AddChargeId";
    public const string ChargeForLineItemNumber = "ChargeForLineItemNumber";

    //Line Item Details
    public const string ServiceStartDay = "StartDay";
    public const string ServiceEndDay = "EndDay";
    public const string ServiceStartDateDropdown = "ServiceStartDateDropdown";
    public const string ServiceEndDateDropdown = "ServiceEndDateDropdown";
    public const string UomCodeDropdown = "UomCodeId";
    public const string LineDetailNetTotal = "TotalNetAmount";
    public const string TotalTaxAmount = "TotalTaxAmount";
    public const string TotalVatAmount = "TotalVatAmount";
    public const string LineDetailTotalAddOnChargeAmount = "TotalAddOnChargeAmount";
    public const string DynamicFieldIdSuffics = "_DFId";
    public const string DynamicFieldValueSuffix = "_DFValue";
    public const string DynamicFieldGroupSuffix = "_Group";
    public const string DynamicFieldPrefix = "DF_";

    // Line Item Grid
    public const string LineItemDetailGridId = "LineItemDetailGrid";

    // Contact Information dialog fields
    public const string BillingContactType = "BillingContactType";
    public const string BillingContactValue = "BillingContactValue";
    public const string BillingContactDescription = "BillingContactDescription";
    public const string BillingContactId = "BillingContactId";

    public const string BilledContactType = "BilledContactType";
    public const string BilledContactValue = "BilledContactValue";
    public const string BilledContactDescription = "BilledContactDescription";
    public const string BilledContactId = "BilledContactId";

    public const string BilledIn = "BilledIn";
    public const string IsExpired = "IsExpired";
    public const string CorrespondenceNumber = "CorrespondenceNumber";
    public const string TransactionDate = "TransactionDate";

    public const string NetDueDate = "PaymentDetail.NetDueDate";
    public const string DiscountDueDate = "PaymentDetail.DiscountDueDate";

    public const string CorrespondenceDate = "CorrespondenceDate";

    public const string CorrespondenceFromDate = "FromDate";
    public const string CorrespondenceToDate = "ToDate";
    public const string PaxDailyRevenueRecognitionFileDate = "DailyRevenueRecognitionFileDate";

    public const string FileSubmissionDate = "FileSubmissionDate";

    public const string FromMemberText = "FromMemberText";
    public const string ToMemberText = "ToMemberText";

    public const string BillingMemberCountryCode = "BillingMemberCountryCode";
    public const string BilledMemberCountryCode = "BilledMemberCountryCode";

    public const string BillingMemberCountryName = "BillingMemberCountryName";
    public const string BilledMemberCountryName = "BilledMemberCountryName";

    public const string BillingMemberReferenceLocationCode = "MemberLocationInformation[0].MemberLocationCode";
    public const string BilledMemberReferenceLocationCode = "MemberLocationInformation[1].MemberLocationCode";

    public const string BillingMemberDSRequired = "BillingMemberDSRequired";
    public const string BilledMemberDSRequired = "BilledMemberDSRequired";

    public const string BillingMemberRefOrgName = "MemberLocationInformation[0].OrganizationName1";
    public const string BillingMemberRefAddressLine1 = "MemberLocationInformation[0].AddressLine11";
    public const string BillingMemberRefAddressLine2 = "MemberLocationInformation[0].AddressLine21";
    public const string BillingMemberRefAddressLine3 = "MemberLocationInformation[0].AddressLine31";
    public const string BillingMemberRefCity = "MemberLocationInformation[0].CityName1";
    public const string BillingMemberRefCompanyRegId = "MemberLocationInformation[0].CompanyRegistrationId1";
    public const string BillingMemberRefPostalCode = "MemberLocationInformation[0].PostalCode1";
    public const string BillingMemberRefSubdivisionName = "MemberLocationInformation[0].SubdivisionName1";
    public const string BillingMemberRefSubdivisionCode = "MemberLocationInformation[0].SubdivisionCode1";
    public const string BillingMemberRefTaxRegistrationId = "MemberLocationInformation[0].TaxRegistrationId1";
    public const string BillingMemberRefLegalText = "MemberLocationInformation[0].LegalText1";
    public const string BillingMemberAdditionalTaxVatRegNumber = "MemberLocationInformation[0].AdditionalTaxVatRegistrationNumber1";


    public const string BilledMemberRefOrgName = "MemberLocationInformation[1].OrganizationName1";
    public const string BilledMemberRefAddressLine1 = "MemberLocationInformation[1].AddressLine11";
    public const string BilledMemberRefAddressLine2 = "MemberLocationInformation[1].AddressLine21";
    public const string BilledMemberRefAddressLine3 = "MemberLocationInformation[1].AddressLine31";
    public const string BilledMemberRefCity = "MemberLocationInformation[1].CityName1";
    public const string BilledMemberRefCompanyRegId = "MemberLocationInformation[1].CompanyRegistrationId1";
    public const string BilledMemberRefPostalCode = "MemberLocationInformation[1].PostalCode1";
    public const string BilledMemberRefSubdivisionName = "MemberLocationInformation[1].SubdivisionName1";
    public const string BilledMemberRefSubdivisionCode = "MemberLocationInformation[1].SubdivisionCode1";
    public const string BilledMemberRefTaxRegistrationId = "MemberLocationInformation[1].TaxRegistrationId1";

    public const string OptionalGroupDropdownList = "OptionalGroupDropdownList";
    public const string BilledMemberAdditionalTaxVatRegNumber = "MemberLocationInformation[1].AdditionalTaxVatRegistrationNumber1";

    public const string BillingPeriod = "BillingPeriod";
    public const string AmountToBeSettled = "AmountToBeSettled";
    public const string CorrespondenceHeader = "CorrespondenceHeader";

    // IS Calendar Search
    public const string ISCalendarSearchYear = "calendarSearchYear";
    public const string ISCalendarSearchMonth = "calendarSearchMonth";
    public const string ISCalendarSearchPeriod = "calendarSearchPeriod";
    public const string ISCalendarSearchGrid = "ISCalendarSearchGrid";

    // Correct supporting documents search
    public const string SupportingDocumentBillingYearMonth = "SupportingDocumentBillingYearMonth";
    public const string SupportingDocumentBillingPeriod = "SupportingDocumentBillingPeriod";
    public const string SupportingDocumentBilledMember = "SupportingDocumentBilledMember";
    public const string SupportingDocumentInvoiceNo = "SupportingDocumentInvoiceNo";
    public const string SupportingDocumentFileName = "SupportingDocumentFileName";
    public const string SupportingDocumentSubmissionDate = "SubmissionDate";
    public const string SupportingDocumentSearchGrid = "SupportingDocumentSearchGrid";
    // For detail View of Supporting Document
    public const string SupportingDocumentBillingYearMonthDetailView = "SupportingDocumentBillingYearMonthDetailView";
    public const string SupportingDocumentBillingPeriodDetailView = "SupportingDocumentBillingPeriodDetailView";
    public const string SupportingDocumentBilledMemberDetailView = "SupportingDocumentBilledMemberDetailView";
    public const string SupportingDocumentBilledMemeberIdDetailView = "SupportingDocumentBilledMemeberIdDetailView";
    public const string SupportingDocumentInvoiceNoDetailView = "SupportingDocumentInvoiceNoDetailView";
    public const string SupportingDocumentFileNameDetailView = "SupportingDocumentFileNameDetailView";
    public const string SupportingDocumentFormCDetailView = "SupportingDocumentFormCDetailView";
    public const string SupportingDocumentSequenceNumberDetailView = "SupportingDocumentSequenceNumberDetailView";
    public const string SupportingDocumentBatchNumberDetailView = "SupportingDocumentBatchNumberDetailView";
    public const string SupportingDocumentCouponBreakdownSerialNumberDetailView = "SupportingDocumentCouponBreakdownSerialNumberDetailView";

    // For Mismatch transactions search Criteria
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
    public const string MismatchTransactionSearchGrid = "MismatchTransactionSearchGrid";
    public const string MismatchChargeCategory = "MismatchChargeCategory";

    //Supporting Document 
    public const string SupportingDocumentType = "SupportingDocumentTypeId";
    public const string RMBMCMNumber = "RMBMCMNumber";
    public const string SupportingDocSearchResultGrid = "SupportingDocSearchResultGrid";
    public const string AttachmentGrid = "AttachmentGrid";

    public const string ProrateSlipeControl = "ProrateSlipDetailsText";

    // System Monitor
    public const string CompletedJobGrid = "CompletedJobGrid";
    public const string PendingJobGrid = "PendingJobGrid";
    public const string SystemAlertGrid = "SystemAlertGrid";
    public const string LoggedInUserGrid = "LoggedInUserGrid";
    public const string OustandingItemsGrid = "OustandingItemsGrid";
    public const string IsWebResponseGrid = "IsWebResponseGrid";


    //Cargo 
    public const string RejectionMemoAwbPrepaidGrid = "RMAWBPrepaidGrid";
    public const string RejectionMemoAwbChargeCollectGrid = "RMAWBChargeCollectGrid";
    public const string RejectionMemoListingGrid = "RMListingGrid";
    public const string ValidationErrorCorrectionGrid = "ValidationErrorCorrection";
    public const string PayableInvoiceSearchGrid = "PayableInvoiceSearch";
    public const string CargoPayableSupportingDocSearchResultGrid = "CargoPayableSupportingDocSearchResultGrid";
    public static string PayableManageSupporingDocGrid = "PayableManageSupporingDoc";
    public const string ExceptionSummaryGrid = "ExceptionSummary";
    public const string SubTotalGridId = "SubTotalGrid";
    public const string CreditMemoAwbGrid = "CreditMemoAwbGrid";

    //UATP
    public const string UatpExceptionDetailsGrid = "UatpExceptionDetailsGrid";
    public const string UatpExceptionSummaryGrid = "UatpExceptionSummary";

    public const string BillingMemoAwbGrid = "BillingMemoAwbGrid";
    public const string BillingMemoAwbChargeCollectGrid = "BMAWBChargeCollectGrid";
    public const string BillingMemoListingGrid = "BMListingGrid";

    // TIme limit
    public const string EffectiveFromPeriod = "EffectiveFromPeriod";
    public const string EffectiveToPeriod = "EffectiveToPeriod";

    //LegalArchive
    public const string LegalArchiveSearchGrid = "LegalArchiveSearchGridId";
    public const string RetrivalJobSummaryGrid = "RetrivalJobSummaryGrid";
    public const string ArchiveRetrivalJobDetailsGridControl = "ArchiveRetrivalJobDetailsGridControl";
    public const string LegalArchiveType = "LegalArchiveType";
    public const string LegalArchiveBillingYear = "LegalArchiveBillingYear";

    //Pax
    public const string SamplingExceptionDetailsGrid = "SamplingExceptionDetailsGrid";
    public const string SamplingExceptionSummaryGrid = "SamplingExceptionSummaryGrid";

    /// <summary>
    /// CMP #533: RAM A13 New Validations and New Charge Code
    /// </summary>
    public const string ServiceProvider = "service provider";
    public const string Gds = "gds";

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public const string DeliveryDateFrom = "DeliveryDateFrom";
    public const string DeliveryDateTo = "DeliveryDateTo";

    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
    public const string LinkedRejectionMemoGridId = "LinkedRejectionMemoGridId";
    public const string SucessMesageForRMAuditTrail = "Generation of the audit trail package is in progress. You will be notified via email once it is ready for download.[File: {0}.ZIP]";
    // SCP450430: Exception occurred in Report Download Service. - SIS Production
    public const string ErrorMesageForRMAuditTrail = "An error occurred while generating the audit trail. Please try generation of audit trail again.";

    //CMP #675: Progress Status Bar for Processing of Billing Data Files
    public const string PDFileStatusGrid = "PDFileStatusGrid";
    public const string UploadFileStatusGrid = "UploadFileStatusGrid";
    public const string SMCurrentStatsGrid = "SMCurrentStatsGrid";
  }

 
  public struct PageMode
  {
    public const string View = "View";
    public const string Edit = "Edit";
    public const string Create = "Create";
    public const string Clone = "Clone";
  }

  public struct BillingType
  {
    public const string Payables = "Payables";
    public const string Receivables = "Receivables";
  }

  public struct TaxType
  {
    public const string Tax = "Tax";
    public const string VAT = "VAT";
  }

  public enum TransactionMode
  {
    InvoiceSearch,
    BillingHistory,
    Payables,
    Transactions,
    MiscUatpInvoiceSearch,
    MiscUatpInvoice,
    CalendarSearch,
    ProcessingDashboard,
    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    DailyMiscInvSearch
  }
}

namespace Iata.IS.Model.Enums
{
  /// <summary>
  /// Different input/output file formats that can be received by the system.
  /// </summary>
  public enum FileFormatType
  {
    /// <summary>
    /// None added to cater the requirements of CMP#596: Length of Member Accounting Code to be Increased to 12 
    /// </summary>
    None = 0,

    /// <summary>
    /// IS-IDEC file format.
    /// </summary>
    IsIdec = 1,

    /// <summary>
    /// IS-XML file format.
    /// </summary>
    IsXml = 2,

    /// <summary>
    /// ISR file format - required in the auto-billing process.
    /// </summary>
    Isr = 3,

    /// <summary>
    /// Value confirmation file format reqiured in value confirmation process.
    /// </summary>
    ValueConfirmation = 4,

    /// <summary>
    /// Usage files are sent by airlines to the system in the auto-billing process.
    /// </summary>
    Usage = 5,

    /// <summary>
    /// A compressed file containing a batch of supporting documents.
    /// </summary>
    SupportingDoc = 6,

    /// <summary>
    /// Form C XML
    /// </summary>
    FormCXml = 7,

    /// <summary>
    /// Offline archive files
    /// </summary>
    OfflineArchive = 8,

    /// <summary>
    /// Processed invoice CSV reports
    /// </summary>
    ProcessedInvoiceCsvReports = 9,

    /// <summary>
    /// Validation/sanity check reports for IS-IDEC/IS-XML
    /// </summary>
    ValidationSanityCheckReportsIsIdecIsXml = 10,

    /// <summary>
    /// Sanity check reports for supporting documents
    /// </summary>
    SanityCheckReportsForSupportingDocuments = 11,

    /// <summary>
    /// PAX auto-billing revenue recognition files
    /// </summary>
    PaxAutoBillingRevenueRecognition = 12,

    /// <summary>
    /// PAX auto-billing invoice posting files
    /// </summary>
    PaxAutoBillingInvoicePosting = 13,

    /// <summary>
    /// Old-IDEC files
    /// </summary>
    OldIdec = 14,

    /// <summary>
    ///  CMP-597 Change Information for Reference data update CSV file
    /// </summary>
    ChangeInfoReferenceDataUpdateCsv = 15,

    /// <summary>
    /// On behalf of IS-XML files (for MISC/UATP)
    /// </summary>
    OnbehalfOfIsXml = 16,

    /// <summary>
    /// ACH RECAP Sheet file
    /// </summary>
    AchRecapSheet = 17,

    /// <summary>
    /// Recharge data Xml file
    /// </summary>
    RechargeDataXml = 18,

    /// <summary>
    /// Legal Invoice Xml file
    /// </summary>
    LegalInvoiceXml = 19,
    /// <summary>
    /// Bvc Csv Report
    /// </summary>
    BvcCsvReport = 20,
    /// <summary>
    /// xml outbound
    /// </summary>
    IsXmlOutbound = 21,
    /// <summary>
    /// Idec outbound
    /// </summary>
    IsIdecOutbound = 22,
    /// <summary>
    /// MiscIsWebXml
    /// </summary>
    MiscIsWebXml = 23,

    /// <summary>
    /// This is required for correspondence report download service 
    /// </summary>
    CorrespondenceZipReport = 25,

    /// <summary>
    /// This is required for Daily AutoBilling Irregularity Report download service 
    /// </summary>
    DailyAutoBillingIrregularityReport = 26,

    SanityCheckReportsForIsrFile = 27,

    SanityCheckReportsForUsageFile = 28,

    UatpAtcanFile = 32,

    /// <summary>
    /// Usage files are sent by IS to airlines in the auto-billing process.
    /// </summary>
    IsrToMember = 30,

   
    /// <summary>
    /// Usage files are sent by IS to the ATPCO in the auto-billing process.
    /// </summary>
    UsageToAtpco = 31,

    // CMP529: Daily Output Generation for MISC Bilateral Invoices
    /// <summary>
    /// Daily Misc Bilateral IS-XML file sent to members who have opted for daily misc bilateral output.
    /// </summary>
    DailyMiscBilateralIsXml = 33,

    /// <summary>
    /// Daily Misc Bilateral OAR file sent to members who have opted for daily misc bilateral output.
    /// </summary>
    DailyMiscBilateralOfflineArchive = 34,

    /// <summary>
    ///  CMP-597 Complete Reference Data CSV file
    /// </summary>
    CompleteReferenceDataCsv = 35,

    /// <summary>
    ///  CMP-597 Complete Contacts Data CSV file
    /// </summary>
    CompleteContactsDataCsv = 36,

    #region CMP#622: MISC Outputs Split as per Location IDs.
    //CMP#622: MISC Outputs Split as per Location IDs.
    /// <summary>
    /// OnbehalfIsXmlMiscLocSpec
    /// </summary>
    OnbehalfIsXmlMiscLocSpec = 37,

    /// <summary>
    /// MiscIsWebXmlMiscLocSpec
    /// </summary>
    MiscIsWebXmlMiscLocSpec = 38,

    /// <summary>
    /// OfflineArchiveMiscLocSpec
    /// </summary>
    OfflineArchiveMiscLocSpec = 39,

    /// <summary>
    /// IsXmlOutboundMiscLocSpec
    /// </summary>
    IsXmlOutboundMiscLocSpec = 40,

    /// <summary>
    /// DailyMiscBilateralIsXmlLocSpec
    /// </summary>
    DailyMiscBilateralIsXmlLocSpec = 41,

    /// <summary>
    /// DailyMiscBilateralOARLocSpec
    /// </summary>
    DailyMiscBilateralOARLocSpec = 42,

    #endregion

    #region CMP#608: Load Member Profile - CSV Option

    MemberProfileCSVUpload = 43 

    #endregion

  }
}

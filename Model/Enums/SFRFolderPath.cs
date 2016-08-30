namespace Iata.IS.Model.Enums
{
  public enum SFRFolderPath
  {
    PathOfflineCollWebRoot = 1,
    SFRTempRootPath = 2,
    PathOfflineCollDownloadSFR = 3,
    PathIscalendarCsv = 4,
    PathFtpRoot = 5,
    EmailAttachementFile=6,
    RecapsheetPath =7,
    BVCRequestPath =8,
    InputFileParsingPath=9,
    CSVOutputPath=10,
    CorrespondenceFormatReportPath =11,
    ProcessedInputPath =12,
    LegalArchivePath = 13,
    LegalArchieveRetrieved = 14,
    LARetrieved = 15,
    LADeposit = 16,
    ISOutputFolderPath = 17,
    ISOARFolderPath = 18,
    ISBvcCsvFolder = 19,
    ISCorrRepFolder = 20,
    ISUsageReport = 21,
    ISBlockingRulesCsvFolder = 22,
    //CMP508:Audit Trail Download with Supporting Documents
    ISAuditTrailFolder = 23,
    //CMP288: Use to locate temp folder location to create preview for inovice.
    InvoicePreviewWorkFolder = 24,

    OfflineCollectionFormCUiFolder = 25,

    //SCP 245276 - SIS Legal Invoice Zip file has not been generating since October 2013
    LegalXMLZipToIATAFolder = 26,
    ReceivableRmbmcmSummaryReportPath = 27,
    PayableRmbmcmSummaryReportPath = 28,

    //CMP597: Use to locate folder location to create Member Profile Data Files.
    MemberProfileDataFile = 29,

    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
    RMAuditTrailReportPath = 30,

    //CMP#608: Load Member Profile - CSV Option
    MemberProfileCSVProcessedPath = 31,

    // CMP #659: SIS IS-WEB Usage Report.
    SisIsWebUsageReport = 32
  }
}

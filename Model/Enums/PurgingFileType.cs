namespace Iata.IS.Model.Enums
{
  /// <summary>
  /// Represents the different perging file types
  /// </summary>
  public enum PurgingFileType
  {
    /// <summary>
    /// For those file whose trasnactions has been deleted and file need to be deleted. 
    /// </summary>
    None =0,

    InputDataFile = 1,

    OutputFile = 2,

    SupportingDocument = 3,

    UnlinkedSupportingDocument = 4,

    CorrespondenceFile = 5,

    CorrespondenceReportFile = 6,

    LegalArchive = 7,

    LogFile = 8,

    TemporaryFile = 9,

    // CMP599: Added below two enums, to be used in purging process.
    InvoiceOffliceCollectionFilesFolders = 10,

    FormCOffliceCollectionFilesFolders = 11,

    //SCP317057: ISWEB requested OAR files are not getting purged
    IswebRequestedOarFiles = 12,

    //SCP317056: Purging of Email and Email attachments
    EmailAttachmentFiles = 13

  }
}

using System;
using System.IO;

namespace Iata.IS.Core.Configuration
{
  public class SystemParameters
  {
    private static SystemParameters _sInstance;

    private SystemParameters()
    {
    }

    public static SystemParameters Instance
    {
      get
      {
        return _sInstance ?? (_sInstance = new SystemParameters());
      }
    }

    public UIParameters UIParameters
    {
      get
      {
        return new UIParameters();
      }
    }

    public CalendarParameters CalendarParameters
    {
      get
      {
        return new CalendarParameters();
      }
    }

    /// <summary>
    /// This is the member id of the dummy member created in system for ACH.
    /// This member is created to enable FTP Push to ACH.
    /// </summary>
    public int ACHMemberId
    {
      get
      {
        return 1742;
      }
    }

    /// <summary>
    /// This is the member id of the dummy member created in system for IATA.
    /// This member is created to enable FTP Push to IATA.
    /// </summary>
    public int IATAMemberId
    {
      get
      {
        return 1792;
      }
    }

    /// <summary>
    /// This is the member id of the dummy member created in system for ATPCO.
    /// This member is created to enable FTP Push to ATPCO.
    /// </summary>
    public int ATPCOMemberId
    {
      get
      {
        return 1744;
      }
    }

    /// <summary>
    /// This is the Max number of records allowed per VCF File
    /// </summary>
    public int MaxCouponRecordsPerVCF
    {
      get
      {
        return 40000;
      }
    }

    /// <summary>
    /// This is the number of records for which AIA Service Level Agreement is defined
    /// </summary>
    public long AIASLANoOfRecords
    {
      get
      {
        return 34000;
      }
    }

    /// <summary>
    /// This is the number of records for which AIA Service Level Agreement is defined
    /// </summary>
    public long AIASLATimeInSeconds
    {
      get
      {
        return 1800;
      }
    }

    /// <summary>
    /// ICH Late submission acceptance flag
    /// </summary>
    public bool ICHLateAcceptanceAllowed
    {
      get { return true; }
      set { }
    }

    /// <summary>
    /// ACH Late submission acceptance flag
    /// </summary>
    public bool ACHLateAcceptanceAllowed
    {
      get { return true; }
      set { }
    }

    public int MaxNumberOfInvoicesInICHSettlementFile
    {
      get { return 1000; }
    }

    public string PathToSchemaFiles
    {
      get
      {
        return String.Format("{0}\\App_Data\\SchemaFiles",
                             Environment.GetFolderPath(Environment.SpecialFolder.System,
                                                       Environment.SpecialFolderOption.Create));
      }
    }

    public string ISAdmininstratorEmail
    {
      get { return "sujit_ghaisas@kaleconsultants.com"; }
    }

    public string ICHAdmininstratorEmail
    {
      get { return "sujit_ghaisas@kaleconsultants.com"; }
      set { ICHAdmininstratorEmail = value; }
    }

    public string ACHAdmininstratorEmail
    {
      get { return "sujit_ghaisas@kaleconsultants.com"; }
    }

    public string IAContact
    {
      get { return "sujit_ghaisas@kaleconsultants.com"; }
    }

    public int MaxNumberOfRetriesToSendICHSettlementFile
    {
      get { return 3; }
    }

    public string SISGatewayServiceUserName
    { get { return "user1"; } }

    public string SISGatewayServiceUsersPassword
    { get { return "pass1"; } }

    public string TempPathToStoreRechargeDataXMLFile
    {
      get { return @"C:\SIS_RECHARGE_DATA"; }
    }

    public string SISRechargeDataFTPPath
    {
      get { return @"C:\SIS_RECHARGE_DATA_FTP"; }
    }

    /// <summary>
    /// Default Expiry Date for Annoucements
    /// </summary>
    public int DefaultExpiryDaysforAnnoucements
    {
      get { return 3; }
    }

    /// <summary>
    /// Default Expiry Date for Messages
    /// </summary>
    public DateTime DefaultExpiryDateforMessages
    {
      get
      {
        return DateTime.UtcNow;
      }
    }

    public string AllowedDefaultAttachmentExtensions
    {
      get
      {
        return "zip,doc,pdf,xls,gif,png,jpg";
      }
    }

    // Absolute path where member logo file uploaded through member profile will be stored
    public string MemberLogoFileLocation
    {

      get { return @"C:\MemberLogos"; }

    }
    /// <summary>
    /// Max limit to resend file if upload failure
    /// </summary>
    public int FtpFileUploadMaxAttempt
    {
      get { return 3; }
    }


    public string  FtpRootBasePath
    {
      get { return @"\\Puneftp\ftpdata"; }
    }

    public string FtpRootFolderName
    {
      get { return "vinod"; }
    }


    public string  UploadFolderNamingconvention
    {
      get { return "Upload"; }
    }

    public string  DownloadFolderNamingconvention
    {
      get { return "Download"; }
    }

    public string ErrorFolderNamingconvention
    {
      get { return "Err"; }
    }


    public string  HttpsMemberBasePath
    {
      get { return "https://"; }
    }


    public string MemberFileBasePath
    {
      get { return @"Y:\FTPRoot"; }
    }


    public int LoggedUserCount
    {
      get { return 1000; }
    }
    public ATPCO Atpco
    {
      get { return new ATPCO(); }
    }

    public ACH Ach
    {
      get { return new ACH(); }
    }

    public ICH Ich
    {
      get { return new ICH(); }
    }

    public MemberType Member
    {
      get { return new MemberType(); }
    }

    public IATA Iata
    {
      get { return new IATA(); }
    }

    public iiNET iiNet
    {
      get { return new iiNET(); }
    }

    public int MaxBillingFilesAllowedPerDay
    {
      get { return 10; }
    }

    public string SubmissionDeadlineAlertCsvFileHeader
    {
        get { return "List of invoices with status 'Open' 'Ready for Submission' or 'Validation Error - WEB Invoice' for the BillingCategory - "; }
    }

    public bool IgnoreValidationOnMigrationPeriod
    {
      get;
      set;
    }

    /// <summary>
    ///  Max file size in bytes for supporting doc
    /// </summary>
    public int MaxFileSizeInBytesForSupportingDoc
    {
      get
      {
        return 26214400;
      }
    }
  }

 
  public class ATPCO
  {
    public string ServerName
    {
      get { return "10.1.2.160"; }

    }

    public string UserName
    {
      get
      {
        return @"punemailer\0AT";
      }
    }

    public string Password
    {
      get { return "test@1234"; }
    }

    public int Port
    {
      get { return 990; }
    }

    public string Security
    {
      get { return "Implicit"; }
    }

    public string ATPCOCode
    {
        get { return "ATPCO"; }
    }

  }
  
  public class ACH
  {
    public string ServerName
    {
      get { return "10.1.2.160"; }

    }

    public string UserName
    {
      get
      {
        return @"punemailer\0AC";
      }
    }

    public string Password
    {
      get { return "test@1234"; }
    }

    public int Port
    {
      get { return 990; }
    }

    public string Security
    {
      get { return "Implicit"; }
    }

  }

  public class ICH
  {
    public string ServerName
    {
      get { return "10.1.2.160"; }

    }

    public string UserName
    {
      get { return @"punemailer\sisftp"; }
    }

    public string Password
    {
      get { return "test@1234"; }
    }

    public int Port
    {
      get { return 990; }
    }

    public string Security
    {
      get { return "Implicit"; }
    }

  }
  
  public class MemberType
  {
    public string ServerName
    {
      get { return "10.1.2.160"; }

    }

    public string UserName
    {
      get { return @"punemailer\sisftp"; }
    }

    public string Password
    {
      get { return "test@1234"; }
    }

    public int Port
    {
      get { return 990; }
    }

    public string Security
    {
      get { return "Implicit"; }
    }

  }
  
  public class IATA
  {
    public string ServerName
    {
      get { return "10.1.2.160"; }

    }

    public string UserName
    {
      get
      {
        return @"punemailer\0IA";}
    }

    public string Password
    {
      get { return "test@1234"; }
    }

    public int Port
    {
      get { return 990; }
    }

    public string Security
    {
      get { return "Implicit"; }
    }

  }

  public class iiNET
  {
    public string ServerName
    {
      get { return "10.1.2.4"; }

    }

    public string UserName
    {
      get { return "kaleiata"; }
    }

    public string Password
    {
      get { return "kale#iata10"; }
    }

    public int Port
    {
      get { return 21; }
    }

    public string Security
    {
      get { return "Explicit"; }
    }

    public string  Service
    {
      get { return "FTP"; }
    }

    public string Description
    {
      get { return "Testing file transfer using iiNet<CR><LF>"; }
    }

    public string Signature
    {
      get { return "NR"; }
    }

    public string Sender
    {
      get { return "KAL2ZVPN"; }
    }

    public string FileType
    {
      get { return "GENERAL"; }
    }


    public string iiNetFolderName
    {
      get { return "1131"; }
    }



  }

}

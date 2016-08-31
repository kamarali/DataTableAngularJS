using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.BroadcastMessages.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MemberProfile.Impl;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Core.File;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Reports.UatpAtcan;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Common;
using Ionic.Zip;
using log4net;
using NVelocity;

namespace Iata.IS.Business.Common.Impl
{
  public class UatpAtcanDetailsManager : IUatpAtcanDetailsManager
  {
    // Logger instance.
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private IMemberManager _memberManager;
    private IReferenceManager _referenceManager;
    private readonly IProcessedInvoiceDetailManager _processDetailsManager;
      private readonly IBroadcastMessagesManager _broadcastMessagesManager;
    private const string UatpAtcanGenerationFailureAlert = "UATP ATCAN Output generation failure alert";
    private const string UatpAtcanZipFileFailureForMember = "UATP ATCAN zip file generation failure for member {0}";
    private const string UatpAtcanZipFileGenerationFailureAlert = "UATP ATCAN zip file generation failure alert";
    private const string UatpAtcanGenerationFailureForMember = "UATP ATCAN Output generation failure for member {0}";


    public UatpAtcanDetailsManager()
    {
      _memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
      _processDetailsManager = Ioc.Resolve<IProcessedInvoiceDetailManager>(typeof(IProcessedInvoiceDetailManager));
        _broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>(typeof (IBroadcastMessagesManager));
        _referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
    }

    /// <summary>
    /// Gets or sets 
    /// </summary>
    public IUatpAtcanDetailsRepository UatpAtcanDetailsRepository { get; set; }

    public bool CreateAndTransmitUatpAtcanCsv(int memberId, int billingMonth, int billingYear, int billingPeriod, int billingTypeId, int isReProcessing = 0, int retryCount = 1, int maxRetryCnt =1)
    { 
      var memberCodeNumeric = string.Empty;

      // To get details for Uatp Atcan details
      var uatpAtcanDetailsList = GetUatpAtcanDetails(memberId, billingPeriod, billingMonth, billingYear,
                                                     billingTypeId);

      // Get current billing period in YYYYMMDD format.
      var currentPeriodText = string.Format("{0} {1:D4} P{2}",
                                                  CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(billingMonth).ToUpper(),
                                                  billingYear,
                                                  billingPeriod);
 
      Logger.InfoFormat("Found {0} invoices for processing.", uatpAtcanDetailsList.Count);

      // If no data present result  false
      if (uatpAtcanDetailsList.Count <= 0)
      {
        return false;
      }

       // To get numeric code for memberId 
      if (memberId > 0)
      {
          if (_memberManager == null)
          { _memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)); }

          memberCodeNumeric = _memberManager.GetMemberCode(memberId);

          if (string.IsNullOrEmpty(memberCodeNumeric))
          {
              return false;
          }
          
      }
      var errorFlag = 0;
      var invoiceNo = string.Empty;
      try
      {
        var validationCsvGenerator = new CsvGenerator();

        // Get output file path.
        var billingPeriodForPath = string.Format(@"{0:D2}{1:D2}{2:D2}", billingYear, billingMonth,
                                                 billingPeriod);
        var outputFilePath = FileIo.GetFtpDownloadFolderPath(memberCodeNumeric);

        // Create output file prefix.
        var billingType = string.Empty;
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: No code change required, since complete file name is used as is.
        * Ref: FRS Section 3.6 Table 24 Row 20 */
        var outputFilePrefix = string.Format(@"ATCAN-{0}-{1}-{2}",
                                             billingTypeId == 1 ? "R" : "P",
                                             memberCodeNumeric, billingPeriodForPath);
        var outputCsvFileName = outputFilePrefix + ".CSV";
        var outputCsvFilePath = Path.Combine(outputFilePath, outputCsvFileName);
        var outputZipFileName = outputFilePrefix + ".ZIP";
        var outputZipFilePath = Path.Combine(outputFilePath, outputZipFileName);

        if (IsFileExist(outputZipFileName) || isReProcessing == 1)
        {
            // Create output csv file.
            invoiceNo = string.Empty;

            //Set here property's decimal place 
            var propNameWithDecimalPlace = new List<KeyValuePair<string, int>>();
            var kvp = new KeyValuePair<string, int>("DailyExchangeRate", 5);
            propNameWithDecimalPlace.Add(kvp);
             
            if (!validationCsvGenerator.GenerateCSV(uatpAtcanDetailsList, outputCsvFilePath, out invoiceNo, propNameWithDecimalPlace))
            {
              throw new Exception();
              //290709 - RM: UATP ATCAN Csv zip file generation failure - SIS Production
              //Remove duplicate alert code. This has been handled in catch code. 
              /* _processDetailsManager.AddISSISOpsAlert(memberCodeNumeric, UatpAtcanGenerationFailureForMember,
                                                        UatpAtcanGenerationFailureAlert,
                                                        EmailTemplateId.SISAdminAlertUatpAtcanFailureNotification,
                                                        currentPeriodText, invoiceNo);*/
            }

            errorFlag = 1;
            // Create zip of output csv file.
            if (!ProcessedInvoiceDetailManager.ZipOutputFile(outputCsvFilePath, outputZipFilePath))
            {
              throw new Exception();
               //290709 - RM: UATP ATCAN Csv zip file generation failure - SIS Production
               //Remove duplicate alert code. This has been handled in catch code. 
               /* _processDetailsManager.AddISSISOpsAlert(memberCodeNumeric, UatpAtcanZipFileFailureForMember,
                                                        UatpAtcanZipFileGenerationFailureAlert,
                                                        EmailTemplateId.SISAdminAlertUatpAtcanFailureNotificationzip, currentPeriodText,string.Empty);*/
                // continue;
            }
            errorFlag = 2;
            Logger.DebugFormat("Billing Entity Code:{0}, Invoice Count:{1}, Path:{2}", memberCodeNumeric,
                               uatpAtcanDetailsList.Count, outputZipFilePath);

            //Notification email
            /*    var memberManager = Ioc.Resolve<MemberManager>(typeof(IMemberManager));
                var memberContactList = memberManager.GetMemberContactList(memberId).ToList();
         
                var contactListForMember = memberContactList.Where(c => c.MemberId == memberId).Select(c => c.EmailAddress).ToArray();
                if (contactListForMember.Length > 0)
                {
                  Logger.Info("#" + string.Join(", ", contactListForMember) + "#");

                  // Send an email notification to corresponding member contacts.
                  if (!_broadcastMessagesManager.SendOutputAvailableAlert(memberId, contactListForMember, currentPeriodText, currentPeriodText, EmailTemplateId.OutputAvailableAlert))
                  {
                    Logger.ErrorFormat("Error while sending an email notification to member {0}", memberId);
                  }
                }
                */
            //Add entry in is-file log                
            var invoiceRepository = Ioc.Resolve<IInvoiceRepository>(typeof (IInvoiceRepository));

            var isInputFile = new IsInputFile()
                                  {
                                      Id = Guid.NewGuid(),
                                      FileName = outputZipFileName,
                                      IsIncoming = true,
                                      SenderReceiver = memberId,
                                      FileDate = DateTime.UtcNow,
                                      BillingPeriod = billingPeriod,
                                      BillingMonth = billingMonth,
                                      BillingYear = billingYear,
                                      ReceivedDate = DateTime.UtcNow,
                                      SenderReceiverIP =
                                          Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
                                      FileLocation = outputFilePath,
                                      FileFormatId = (int) FileFormatType.UatpAtcanFile,
                                      FileStatusId = (int) FileStatusType.AvailableForDownload,
                                      LastUpdatedBy = 1,
                                      FileVersion = "1",
                                      BillingCategory = (int) BillingCategoryType.Uatp,
                                      SenderRecieverType = (int) FileSenderRecieverType.Member,
                                      ExpectedResponseTime = 0,
                                      FileProcessEndTime = DateTime.UtcNow,
                                      FileProcessStartTime = DateTime.UtcNow
                                  };
            invoiceRepository.AddFileLogEntry(isInputFile);
        }
        else
        {
            Logger.Info("Exception:- File Already Exist in Is File Log table and Location");
        }
      }
      catch (Exception)
      {
        Logger.InfoFormat("Retry Count: {0}", retryCount);
        //SCP278252 - Admin Alert - Offline collection zip - SIS Production
        //Supply period number and invoice number in mail and send email after max retry attempts.
        if (retryCount == maxRetryCnt)
        {
          Logger.InfoFormat("Retry code execute with errorflag: {0}", errorFlag);
          switch (errorFlag)
          {
            case 0:
              _processDetailsManager.AddISSISOpsAlert(memberCodeNumeric, UatpAtcanGenerationFailureForMember,
                                                      UatpAtcanGenerationFailureAlert,
                                                      EmailTemplateId.SISAdminAlertUatpAtcanFailureNotification,
                                                      currentPeriodText, string.Empty);
              break;
            case 1:
              _processDetailsManager.AddISSISOpsAlert(memberCodeNumeric, UatpAtcanZipFileFailureForMember,
                                                      UatpAtcanZipFileGenerationFailureAlert,
                                                      EmailTemplateId.SISAdminAlertUatpAtcanFailureNotificationzip,
                                                      currentPeriodText, string.Empty);
              break;
          }
        }
        throw;
      }
      return true;
    }

    private List<UatpAtcanDetails> GetUatpAtcanDetails(int memberId, int period, int billingMonth, int billingYear, int billingTypeId)
    {
      return UatpAtcanDetailsRepository.GetUatpAtcanDetails(memberId, period, billingMonth, billingYear, billingTypeId);
    }

    private bool IsFileExist(string fileName)
    {
        List<IsInputFile> files = null;

        try
        {
            files = _referenceManager.GetAllIsInputFile(fileName);
        }
        catch (Exception exception)
        {
            Logger.InfoFormat("Error occured while getting file entry in Is Input File table : {0}",
                              exception.Message);
        }

        if (files == null || files.Count == 0)
        {
            return true;
        }
        else
        {
            var fileFullPath = string.Empty;

            foreach (var file in files)
            {
                var fileinfo = new FileInfo(string.Format("{0}\\{1}", file.FileLocation, fileName));

                if (fileinfo.Exists)
                {
                    fileFullPath = fileinfo.FullName;

                    break;
                }
            }
           
            return string.IsNullOrEmpty(fileFullPath);
        }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MemberProfile.Impl;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Core.File;
using Iata.IS.Data.Pax;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
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

  /// <summary>
  /// This is class to create and transmit processed invoice details csv files for current billing period.
  /// Author: Nivas Potdar.
  /// </summary>
  public class ProcessedInvoiceDetailManager : IProcessedInvoiceDetailManager
  {
    private const string ProcessedInvoiceCSVGenerationFailureAlert = "Processed Invoice CSV generation failure alert";
    private const string ProcessedInvoiceCSVGenerationFailureForMember = "Processed Invoice CSV generation failure for member {0}";
    private const string ProcessedInvoiceCSVZipGenerationFailureAlert = "Processed Invoice CSV zip generation failure alert";
    private const string ProcessedInvoiceCSVZipGenerationFailureForMember = "Processed Invoice CSV zip generation failure for member {0}";

    // Logger instance.
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private IMemberManager _memberManager;

    public ProcessedInvoiceDetailManager()
    {
      _memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
      //_calendarManager = Ioc.Resolve<ICalendarManager>();
    }

    /// <summary>
    /// Gets or sets the broadcast messages manager.
    /// </summary>
    /// <value>
    /// The broadcast messages manager.
    /// </value>
    public IBroadcastMessagesManager BroadcastMessagesManager { get; set; }

    /// <summary>
    /// Gets or sets the processed invoice detail repository.
    /// </summary>
    /// <value>
    /// The processed invoice detail repository.
    /// </value>
    public IInvoiceRepository ProcessedInvoiceDetailRepository { get; set; }

    /// <summary>
    /// Creates and compresses the processed invoice data CSV.
    /// </summary>
    /// <param name="triggerGroupName">Name of the trigger group.</param>
    /// <returns>
    /// flag indicates that output zip file created or not.
    /// </returns>
    public bool CreateAndTransmitProcessedInvoiceDataCsv(string triggerGroupName)
    {
      int billingYear, billingMonth, billingPeriod;
      try
      {
        var myDate = DateTime.ParseExact(triggerGroupName, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        billingYear = myDate.Year;
        billingMonth = myDate.Month;
        billingPeriod = myDate.Day;
      }
      catch (Exception)
      {
        throw new Exception(string.Format("The trigger group name [{0}] not in given format yyyy-MM-dd.", triggerGroupName));
      }

      return CreateAndTransmitProcessedInvoiceDataCsv(0, billingYear, billingMonth, billingPeriod, 0);
    }

    /// <summary>
    /// Creates and compresses the processed invoice data CSV.
    /// </summary>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="reGenerateFlag">The re generate flag.</param>
    /// <returns>
    /// flag indicates that output zip file created or not.
    /// </returns>
    public bool CreateAndTransmitProcessedInvoiceDataCsv(int billingMemberId, int billingYear, int billingMonth, int billingPeriod, int reGenerateFlag)
    {
      try
      {
        new DateTime(billingYear, billingMonth, billingPeriod);
      }
      catch (Exception)
      {
        throw new Exception(string.Format("The value for year month or period is invalid. {0:D4}-{1:D2}-P{2}", billingYear, billingMonth, billingPeriod));
      }

      // Get current billing period in YYYYMMDD format.
      var currentPeriodText = string.Format("{0} {1:D4} P{2}",
                                                  CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(billingMonth).ToUpper(),
                                                  billingYear,
                                                  billingPeriod);
      Logger.InfoFormat("Billing period: {0}", currentPeriodText);

      var processedInvoiceList = GetProcessedInvoiceDetails(billingMemberId, billingYear + billingMonth.ToString("D2"), billingPeriod);

      Logger.InfoFormat("Found {0} invoices for processing.", processedInvoiceList.Count);

      if (processedInvoiceList.Count <= 0)
      {
        return false;
      }

      var membersInfo = new Dictionary<int, string>();
      if (billingMemberId > 0)
      {
        if (_memberManager == null)
        { _memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)); }

        var memberCodeNumeric = _memberManager.GetMemberCode(billingMemberId);

        if (string.IsNullOrEmpty(memberCodeNumeric))
        {
          return false;
        }
        membersInfo.Add(billingMemberId, memberCodeNumeric);
      }

      // Get distinct member id as well as billing entity code.
      var memberIdList = billingMemberId > 0
                           ? membersInfo
                           : processedInvoiceList.Select(invoice => new
                           {
                             invoice.MemberId,
                             invoice.BillingEntityCode
                           }).Distinct().ToDictionary(inv => inv.MemberId, inv => inv.BillingEntityCode);

      //// Get output available contacts for all Pax, Misc, Cargo and Uatp
      //var contactList = GetOutputAvailableContactList();

      foreach (var member in memberIdList)
      {
        var errorFlag = 0;
        var invoiceNo = string.Empty;
        try
        {
          var validationCsvGenerator = new CsvGenerator();
          var id = member.Key;
          // Create csv file for each member.
          var processedInvoiceDetailsForMember = processedInvoiceList.Where(i => i.MemberId == id).Select((i, index) =>
          {
            i.SerialNo
              =
              index +
              1;
            return i;
          }).ToList();

          if (processedInvoiceDetailsForMember.Count <= 0)
          {
            continue;
          }

          /* CMP596: View (Database select Query executing within) is changed to select billing and billed member code numeric values as is. 
           * Prior to this change, values were trimmed and only 3 char. length values were selected. Casing the problem.
           * Since database change is done, possible values in field processedInvoiceDetailsForMember[0].BillingEntityCode will be - 
           * 1, 98, 125, 123456789123
           * By doing PadLeft(3, '0') file name will be kept as is/proper for existing members. e.g. - 001, 098, 125 etc.
           * For new members PadLeft() will not caiuse any problem.
           * */
          processedInvoiceDetailsForMember[0].BillingEntityCode = processedInvoiceDetailsForMember[0].BillingEntityCode.PadLeft(3, '0');

          // Get output file path.
          var billingPeriodForPath = string.Format(@"{0:D4}{1:D2}{2:D2}", billingYear, billingMonth, billingPeriod);
          var billingEntityCode = processedInvoiceDetailsForMember[0].BillingEntityCode;
          var outputFilePath = FileIo.GetFtpDownloadFolderPath(billingEntityCode);

          // Create output file prefix.
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: No code change required in output file name generation logic.
          Ref: FRS Section 3.6 Table 24 Row 8 */
          var outputFilePrefix = string.Format(@"PRINVF-{0}{1}", billingEntityCode, billingPeriodForPath.Substring(2));
          var outputCsvFileName = outputFilePrefix + ".CSV";
          var outputCsvFilePath = Path.Combine(outputFilePath, outputCsvFileName);
          var outputZipFileName = outputFilePrefix + ".ZIP";
          var outputZipFilePath = Path.Combine(outputFilePath, outputZipFileName);

          // Create output csv file.
          invoiceNo = string.Empty;
          if (!validationCsvGenerator.GenerateCSV(processedInvoiceDetailsForMember, outputCsvFilePath, out invoiceNo))
          {
            Logger.InfoFormat("Error while generating processed invoice csv for Billing Entity Code:{0}, Invoice No:{1}, Period:{2}", billingEntityCode, invoiceNo, currentPeriodText);
            AddISSISOpsAlert(member.Value, ProcessedInvoiceCSVGenerationFailureForMember, ProcessedInvoiceCSVGenerationFailureAlert, EmailTemplateId.ISAdminProcessedInvoiceCSVFailedNotification, currentPeriodText, invoiceNo);
            continue;
          }
          errorFlag = 1;

          // Create zip of output csv file.
          if (!ZipOutputFile(outputCsvFilePath, outputZipFilePath))
          {
            AddISSISOpsAlert(member.Value, ProcessedInvoiceCSVZipGenerationFailureForMember, ProcessedInvoiceCSVZipGenerationFailureAlert, EmailTemplateId.ISAdminProcessedInvoiceCSVFailedNotification, currentPeriodText, string.Empty);
            continue;
          }
          errorFlag = 2;
          Logger.DebugFormat("Billing Entity Code:{0}, Invoice Count:{1}, Path:{2}", billingEntityCode, processedInvoiceDetailsForMember.Count, outputZipFilePath);

          // Notification email
          //var memberManager = Ioc.Resolve<MemberManager>(typeof(IMemberManager));
          //  var memberContactList = memberManager.GetMemberContactList(billingMemberId).ToList();
          //  var contactListForMember = memberContactList.Where(c => c.MemberId == id).Select(c => c.EmailAddress).ToArray();

          //var contactListForMember = contactList.Where(c => c.MemberId == id).Select(c => c.EmailAddress).ToArray();
          //if (contactListForMember.Length > 0)
          //{
          //  Logger.Info("#" + string.Join(", ", contactListForMember) + "#");
          //  // Send an email notification to corresponding member contacts.
          //  if (!BroadcastMessagesManager.SendOutputAvailableAlert(billingMemberId, contactListForMember, currentPeriodText, currentPeriodText, EmailTemplateId.OutputAvailableAlert))
          //  {
          //    Logger.ErrorFormat("Error while sending an email notification to member {0}", billingMemberId);
          //  }
          //}

          //Add entry in is-file log                
          /*          var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));

                    var isInputFile = new IsInputFile()
                    {
                      FileName = outputZipFileName,
                      IsIncoming = true,
                      SenderReceiver = member.Key,
                      FileDate = DateTime.UtcNow,
                      BillingPeriod = billingPeriod,
                      BillingMonth = billingMonth,
                      BillingYear = billingYear,
                      ReceivedDate = DateTime.UtcNow,
                      SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
                      FileLocation = outputFilePath,
                      FileFormatId = (int)FileFormatType.ProcessedInvoiceCsvReports,
                      FileStatusId = (int)FileStatusType.AvailableForDownload,
                      LastUpdatedBy = 1,
                      FileVersion = "1",
                      BillingCategory = null,
                      SenderRecieverType = (int)FileSenderRecieverType.Member
                    };
                    referenceManager.AddIsInputFile(isInputFile);*/
          Logger.InfoFormat("Adding entry of file {0}", outputZipFileName);
          AddFileLogEntry(member.Key, outputZipFileName, null, new BillingPeriod(billingYear, billingMonth, billingPeriod), FileFormatType.ProcessedInvoiceCsvReports, outputFilePath, FileSenderRecieverType.Member);
          Logger.InfoFormat("Added entry of file {0}", outputZipFileName);
        }
        catch (Exception)
        {
          switch (errorFlag)
          {
            case 0:
              AddISSISOpsAlert(member.Value, ProcessedInvoiceCSVGenerationFailureForMember, ProcessedInvoiceCSVGenerationFailureAlert, EmailTemplateId.ISAdminProcessedInvoiceCSVFailedNotification, currentPeriodText,string.Empty);
              break;
            case 1:
              AddISSISOpsAlert(member.Value, ProcessedInvoiceCSVZipGenerationFailureForMember, ProcessedInvoiceCSVZipGenerationFailureAlert, EmailTemplateId.ISAdminProcessedInvoiceCSVFailedNotification, currentPeriodText,string.Empty);
              break;
          }
          throw;
        }
      }

      return true;
    }

    /// <summary>
    /// To Add the zip file entry in db.
    /// </summary>
    /// <param name="zipFilePath"></param>
    /// <param name="senderReceiver"></param>
    private void AddFileLogEntry(int memberId, string outputZipFileName, BillingCategoryType? billingCategoryType, BillingPeriod billingPeriod, FileFormatType fileFormatType, string filePath, FileSenderRecieverType fileSenderRecieverType, bool isConsolidatedFile = false)
    {
      var isInputZipFile = new IsInputFile
      {
        Id = Guid.NewGuid(),
        FileName = outputZipFileName,
        IsIncoming = true,
        FileDate = DateTime.UtcNow,
        BillingPeriod = billingPeriod.Period,
        BillingMonth = billingPeriod.Month,
        SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
        FileLocation = filePath,
        FileStatusId = (int)FileStatusType.AvailableForDownload,
        FileFormatId = (int)fileFormatType,
        BillingCategory = billingCategoryType.HasValue ? (int?)billingCategoryType : null,
        BillingYear = billingPeriod.Year,
        SenderReceiver = memberId,
        SenderRecieverType = (int)fileSenderRecieverType,
        ReceivedDate = DateTime.UtcNow,
        FileVersion = "0.1",
        OutputFileDeliveryMethodId = 1,
        ExpectedResponseTime = 0,
        FileProcessEndTime = DateTime.UtcNow,
        FileProcessStartTime = DateTime.UtcNow
      };

      Logger.Debug("Adding Zip entry in IsInputFile.");

      var invoiceRepository = Ioc.Resolve<IInvoiceRepository>(typeof(IInvoiceRepository));
      try
      {
        invoiceRepository.AddFileLogEntry(isInputZipFile);
        Logger.Debug("Zip file Entry added in IsInputFile table.");
      }
      catch (Exception exception)
      {
        Logger.InfoFormat("Error occured while adding entry in IS_FILE_LOG table : {0}", exception.Message);
      }
    }

    /// <summary>
    /// Gets the output available contact list.
    /// </summary>
    /// <returns></returns>
    private IEnumerable<Contact> GetOutputAvailableContactList()
    {
      var outputAvailableAlertContactTypeList = new List<int>
                                                  {
                                                    (int) ProcessingContactType.PAXOutputAvailableAlert,
                                                    (int) ProcessingContactType.CGOOutputAvailableAlert,
                                                    (int) ProcessingContactType.MISCOutputAvailableAlert,
                                                    (int) ProcessingContactType.UATPOutputAvailableAlert
                                                  };
      return _memberManager.GetContactsForContactTypes(outputAvailableAlertContactTypeList);
    }

    /// <summary>
    /// This method will return list of processed invoice details for given criteria.
    /// </summary>
    /// <param name="memberId">ID of member who is creating invoices</param>
    /// <param name="clearanceMonth">clearance month</param>
    /// <param name="period">period</param>
    /// <returns>
    /// list of processed invoice details
    /// </returns>
    private List<ProcessedInvoiceDetail> GetProcessedInvoiceDetails(int memberId, string clearanceMonth, int period)
    {
      return ProcessedInvoiceDetailRepository.GetProcessedInvoiceDetails(memberId, clearanceMonth, period);
    }

    /// <summary>
    /// Adds the ISSIS ops alert.
    /// </summary>
    /// <param name="billingEntityCode">The billing entity code.</param>
    /// <param name="message">The message.</param>
    /// <param name="title">The title.</param>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <param name="billingPeriodText">The billing period.</param>
    /// <param name="invoiceNo">invoice number.</param>

    public void AddISSISOpsAlert(string billingEntityCode, string message, string title, EmailTemplateId emailTemplateId, string billingPeriodText, string invoiceNo)
    {
      // Create an object of the nVelocity data dictionary
      var context = new VelocityContext();

      //Code removed for SCP258087: Admin Alert - Offline collection zip - SIS Production - 12APR2014, because it was implemented incorrect.
      //SCP278252 - Admin Alert - Offline collection zip - SIS Production
      //Supply period number and invoice number in.
      context.Put("MemberCode", billingEntityCode);
      context.Put("Period", billingPeriodText);
      context.Put("InvoiceNo", invoiceNo);

      var issisOpsAlert = new ISSISOpsAlert
      {
        Message = String.Format(message, billingEntityCode),
        AlertDateTime = DateTime.UtcNow,
        IsActive = true,
        EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
        Title = title
      };

      BroadcastMessagesManager.AddAlert(issisOpsAlert, emailTemplateId, context);
    }

    /// <summary>
    /// Creates a zip file for given file; TODO: Temporarily made public for unit testing
    /// </summary>
    /// <param name="outputFilePath">output file path</param>
    /// <param name="zipFileName">zip file name given by user along with the path to store</param>
    /// <returns></returns>
    public static bool ZipOutputFile(string outputFilePath, string zipFileName)
    {
      try
      {
        using (var zipFile = new ZipFile())
        {
          //System.Threading.Thread.Sleep(5000);
          if (File.Exists(outputFilePath))
          {
            zipFile.AddFile(outputFilePath, string.Empty);
            zipFile.Save(zipFileName);
          }
          else
          {
            Logger.Error("Source file does not exists in given location.");
            return false;
          }

          // Delete source file
          if (File.Exists(outputFilePath))
          {
            File.Delete(outputFilePath);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Error generating the zip file", ex);
        throw;
      }
    }
  }

}

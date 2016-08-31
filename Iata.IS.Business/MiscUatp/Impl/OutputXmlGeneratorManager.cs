using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.UI.WebControls;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.Output;
using Iata.IS.Core;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Model.Base;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using NVelocity;
using MailComponent = System.Net.Mail;
using Iata.IS.Core.DI;
using Castle.Core.Smtp;
using log4net;
using System.Reflection;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Model.MemberProfile;
using System.Globalization;

namespace Iata.IS.Business.MiscUatp.Impl
{
  public class OutputXmlGeneratorManager : IOutputXmlGeneratorManager
  {
    /// <summary>
    /// Gets or sets the Misc correspondence Attachment repository.
    /// </summary>
    /// <value>The country repository.</value>
    private IMiscInvoiceRepository MiscInvoiceRepository { get; set; }
    public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public IBroadcastMessagesManager BroadcastMessagesManager { get; set; }



    public OutputXmlGeneratorManager(IMiscInvoiceRepository iMiscInvoiceRepository)
    {
      MiscInvoiceRepository = iMiscInvoiceRepository;
      TemplatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
    }

    /// <summary>
    /// Gets Misc invoices matching the specified search criteria
    /// This is used by exe.
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    public List<MiscUatpInvoice> GetMiscInvoices(Model.Pax.SearchCriteria searchCriteria)
    {
      try
      {
        int? billingCategoryId = null;
        if (!string.IsNullOrEmpty(searchCriteria.BillingCategory))
          billingCategoryId = Convert.ToInt32(searchCriteria.BillingCategory);

        int? billingMemberId = null;
        int? billedMemberId = null;

        if (searchCriteria.BilledMemberId != 0) billedMemberId = searchCriteria.BilledMemberId;
        else if (searchCriteria.BillingMemberId != 0) billingMemberId = searchCriteria.BillingMemberId;

        int? submissionMethodId = null;
        if (searchCriteria.SubmissionMethodId > 0)
          submissionMethodId = searchCriteria.SubmissionMethodId;

        /*  var filteredList = MiscInvoiceRepository.GetMiscInvoices(
                      i => i.BilledMemberId == searchCriteria.BilledMemberId &&
                      i.BillingPeriod == searchCriteria.BillingPeriod &&
                      i.BillingMonth == searchCriteria.BillingMonth &&
                      i.BillingYear == searchCriteria.BillingYear &&
                      i.InvoiceStatusId == searchCriteria.InvoiceStatusId);*/

        // replaced with LoadStrategy call
        //CMP#622: MISC Outputs Split as per Location ID
        var filteredList = MiscInvoiceRepository.GetMiscUatpInvoices(billedMemberId: billedMemberId, billingMemberId: billingMemberId,
                  billingPeriod: searchCriteria.BillingPeriod, billingMonth: searchCriteria.BillingMonth,
                  billingYear: searchCriteria.BillingYear, invoiceStatusIds: searchCriteria.InvoiceStatusIds, billingCategoryId: billingCategoryId, inclusionStatus: searchCriteria.InclusionStatus, isWebGenerationDate: searchCriteria.IsWebGenerationDate, submissionMethodId: submissionMethodId, targetDate: searchCriteria.TargetDate, dailyDeliveryStatus: searchCriteria.DailyDeliveryStatusId, outputType:searchCriteria.OutputType);


        return filteredList;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Gets Misc Is Web invoices matching the specified search criteria
    /// This is used by exe.
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="invoiceStatusIds"></param>
    /// <param name="billingCategoryId"></param>
    /// <param name="isWebGenerationDate"></param>
    /// <param name="isReprocessing"></param>
    /// <param name="outputType"></param>
    /// <param name="locationId"></param>
    /// <returns></returns>
    public List<MiscUatpInvoice> GetMiscIsWebInvoices(int? billingMemberId = null, string invoiceStatusIds = null, int? billingCategoryId = null, DateTime? isWebGenerationDate = null, int? isReprocessing = null, int? outputType = null, string locationId = null)
    {
        try
        {
            //CMP#622: MISC Outputs Split as per Location ID
            var filteredList = MiscInvoiceRepository.GetMiscIsWebInvoices(billingMemberId, invoiceStatusIds, billingCategoryId, isWebGenerationDate, isReprocessing, 
                               outputType, locationId);
            
            return filteredList;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //CMP#622 : MISC Outputs Split as per Location ID 
    public List<MiscUatpInvoice> GetOnBehalfMiscInvoices(OnBehalfInvoiceSetup onBehalfInvoiceSetup, BillingPeriod billingPeriod, int billingMemberId, int outputType)
    {
      //Note : Get invoices having status presented as Service will generated o/p files for Billed member first, invoice status will be changed to Presented.
      var invoiceStatusIds = ((int)InvoiceStatusType.ProcessingComplete) + "," + ((int)InvoiceStatusType.ReadyForBilling) + "," +
                                    ((int)InvoiceStatusType.Claimed) + "," + ((int)InvoiceStatusType.Presented);
      
      Logger.InfoFormat("Fetch data for billing category {0} Invoices", onBehalfInvoiceSetup.BillingCategoryId);
      
      //417057 - KAL: SQL Optimization for output code, Misc/Uatp onBehalf output file Optimization 
      var filteredList = MiscOutputManager.GetOnBehalfMuInvoiceList(billingPeriod: billingPeriod.Period,
                                                                   billingMonth: billingPeriod.Month,
                                                                   billingYear: billingPeriod.Year,
                                                                   billingMemberId: billingMemberId,
                                                                   invoiceStatusIds: invoiceStatusIds,
                                                                   billingCategoryId:
                                                                     onBehalfInvoiceSetup.BillingCategoryId,
                                                                   chargeCategoryId:
                                                                     onBehalfInvoiceSetup.ChargeCategoryId,
                                                                   chargeCodeId: onBehalfInvoiceSetup.ChargeCodeId,
                                                                   onBehalfTransmitterCode:
                                                                     onBehalfInvoiceSetup.TransmitterCode,
                                                                   outputType: outputType);
      return filteredList;
    }

    //CMP#622 : MISC Outputs Split as per Location ID
    /// <summary>
    /// Gets the location specific on behalf misc invoices.
    /// </summary>
    /// <param name="onBehalfInvoiceSetup">The on behalf invoice setup.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="islocationSpec">The islocation spec.</param>
    /// <param name="locationId">The location id.</param>
    /// <param name="isReprocessing">if set to <c>true</c> [is reprocessing].</param>
    /// <returns></returns>
    public List<MiscUatpInvoice> GetLocationSpecificOnBehalfMiscInvoices(OnBehalfInvoiceSetup onBehalfInvoiceSetup, BillingPeriod billingPeriod, int billingMemberId, int? islocationSpec = null, string locationId = null, bool isReprocessing = false)
    {
        //Note : Get invoices having status presented as Service will generated o/p files for Billed member first, invoice status will be changed to Presented.
        var invoiceStatusIds = isReprocessing
                                   ? ((int) InvoiceStatusType.ProcessingComplete).ToString()
                                   : (((int) InvoiceStatusType.ProcessingComplete) + "," + ((int) InvoiceStatusType.Presented) + "," + ((int) InvoiceStatusType.ReadyForBilling) + "," +
                                      ((int) InvoiceStatusType.Claimed));

        var filteredList = MiscInvoiceRepository.GetMiscUatpInvoices(billingPeriod: billingPeriod.Period, billingMonth: billingPeriod.Month,
                  billingYear: billingPeriod.Year, billingMemberId: billingMemberId, invoiceStatusIds: invoiceStatusIds, billingCategoryId: onBehalfInvoiceSetup.BillingCategoryId,
                  chargeCategoryId: onBehalfInvoiceSetup.ChargeCategoryId, chargeCodeId: onBehalfInvoiceSetup.ChargeCodeId, onBehalfTransmitterCode: onBehalfInvoiceSetup.TransmitterCode, outputType: islocationSpec, locationId: locationId);


        return filteredList;
    }

    public List<MiscUatpInvoice> SystemMonitorGetOnBehalfMiscInvoices(OnBehalfInvoiceSetup onBehalfInvoiceSetup, BillingPeriod billingPeriod, int billingMemberId, string invoiceStatusIds)
    {
      //Note : Get invoices having status presented as Service will generated o/p files for Billed member first, invoice status will be changed to Presented.
      Logger.InfoFormat("Fetch data for billing category {0} Invoices--via System Monitor", onBehalfInvoiceSetup.BillingCategoryId);
      //417057 - KAL: SQL Optimization for output code, Misc/Uatp onBehalf output file Optimization
      var filteredList = MiscOutputManager.GetOnBehalfMuInvoiceList(billingPeriod: billingPeriod.Period,
                                                               billingMonth: billingPeriod.Month,
                                                               billingYear: billingPeriod.Year,
                                                               billingMemberId: billingMemberId,
                                                               invoiceStatusIds: invoiceStatusIds,
                                                               billingCategoryId:
                                                                 onBehalfInvoiceSetup.BillingCategoryId,
                                                               chargeCategoryId:
                                                                 onBehalfInvoiceSetup.ChargeCategoryId,
                                                               chargeCodeId: onBehalfInvoiceSetup.ChargeCodeId,
                                                               onBehalfTransmitterCode:
                                                                 onBehalfInvoiceSetup.TransmitterCode,
                                                               outputType: 1);

      return filteredList;
    }

    /// <summary>
    /// This will form and return the appropriate error message
    /// </summary>
    /// <param name="miscUatpInvoiceBases">miscUatpInvoiceBases</param>
    /// <returns>error message</returns>
    public string GetErrorMessage(IEnumerable<InvoiceBase> miscUatpInvoiceBases)
    {
      var errorMessageStringBuilder = new StringBuilder();
      foreach (var miscUatpInvoice in miscUatpInvoiceBases)
      {
        var error = new List<string>();
        var count = 0;
        if (miscUatpInvoice.DigitalSignatureStatus == DigitalSignatureStatus.Pending || miscUatpInvoice.DigitalSignatureStatus == DigitalSignatureStatus.Requested || miscUatpInvoice.DigitalSignatureStatus == DigitalSignatureStatus.Failed)
        {
          error.Add("Digital Signature Status");
          count++;
        }
        if (miscUatpInvoice.SupportingAttachmentStatus == SupportingAttachmentStatus.RequiredButNotRequested || miscUatpInvoice.SupportingAttachmentStatus == SupportingAttachmentStatus.InProgress)
        {
          error.Add("Supporting Attachment Status");
          count++;
        }
        if (miscUatpInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling || miscUatpInvoice.InvoiceStatus == InvoiceStatusType.Claimed)
        {
          error.Add("Invoice Status");
          count++;
        }

        var invalidStatus = String.Join(",", error);
        if (count > 0)
        {
          var errorString = String.Format("{0} of MU Invoice with Invoice Number : {1} is Invalid.<BR/>", invalidStatus, miscUatpInvoice.InvoiceNumber);
          Logger.Info(errorString);
          errorMessageStringBuilder.AppendLine();
          errorMessageStringBuilder.AppendLine(errorString);
        }
      }
      return errorMessageStringBuilder.ToString();
    }

    /// <summary>
    /// Gets MiscUatp invoices matching the specified search criteria
    /// This is used for scheduler
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    public List<MiscUatpInvoice> GetMiscUatpInvoices(Model.Pax.SearchCriteria searchCriteria)
    {
      try
      {
        /*  var filteredList = MiscInvoiceRepository.GetMiscInvoices(
                      i => i.BillingPeriod == searchCriteria.BillingPeriod &&                   
                      i.BillingMonth == searchCriteria.BillingMonth &&
                      i.BillingYear == searchCriteria.BillingYear &&
                      i.InvoiceStatusId == searchCriteria.InvoiceStatusId);*/

        // replaced with LoadStrategy call
          var filteredList = MiscInvoiceRepository.GetMiscUatpInvoices(billedMemberId: searchCriteria.BilledMemberId, billingPeriod: searchCriteria.BillingPeriod, billingMonth: searchCriteria.BillingMonth,
                  billingYear: searchCriteria.BillingYear, invoiceStatusIds: searchCriteria.InvoiceStatusIds.ToString());

        return filteredList;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

      /// <summary>
      /// Send Email Notification if User is not opted for Xml output.
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="processingContact"></param>
      /// <param name="billingCategory"></param>
      /// <returns></returns>
      public bool SendEmailNotification(int memberId, ProcessingContactType processingContact, string billingCategory)
      {
          Logger.InfoFormat("{0}-{1}-{2}", memberId, processingContact, billingCategory);

          var message = new MailComponent.MailMessage();
          try
          {
              List<Contact> contactTypeList = GetToEmailIds(memberId, processingContact);
              var emailSender = Ioc.Resolve<IEmailSender>(typeof (IEmailSender));

              IQueryable<EmailTemplate> emailSettingForOutputNotification = null;

              var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof (IRepository<EmailTemplate>));
              var toMailIds = new StringBuilder();
              var index = 0;
              if (contactTypeList != null)
              {
                  foreach (var contact in contactTypeList)
                  {
                      index += 1;
                      toMailIds.Append(index != contactTypeList.Count ? string.Format("{0}{1}", contact.EmailAddress, ",") : contact.EmailAddress);
                  }
              }
              if (!string.IsNullOrEmpty(toMailIds.ToString()))
              {
                  message.To.Add(toMailIds.ToString());
              }
              else
              {
                  Logger.Info("Email address is not available for the user, so not able to send the message");
                  return false;
              }

              //SCP 413919 - Output Generation Notification - SIS Production
              var context = new NVelocity.VelocityContext();

              if (billingCategory == "Passenger" || billingCategory == "Cargo")
              {
                emailSettingForOutputNotification = emailSettingsRepository.Get(esfirdu => esfirdu.Id == (int) EmailTemplateId.OutputGenerationNotification);
                message.From = new MailComponent.MailAddress(emailSettingForOutputNotification.SingleOrDefault().FromEmailAddress, "SIS No Reply");
                message.Subject = string.Format("{0} [Billing Category- {1}]", emailSettingForOutputNotification.SingleOrDefault().Subject, billingCategory);
                message.Body = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.OutputGenerationNotification, context);
              }
              else if (billingCategory == "Miscellaneous" || billingCategory == "Uatp")
              {
                emailSettingForOutputNotification = emailSettingsRepository.Get(esfirdu => esfirdu.Id == (int) EmailTemplateId.MuOutputGenerationNotification);
                message.From = new MailComponent.MailAddress(emailSettingForOutputNotification.SingleOrDefault().FromEmailAddress, "SIS No Reply");
                message.Subject = string.Format("{0} [Billing Category- {1}]", emailSettingForOutputNotification.SingleOrDefault().Subject, billingCategory);
                message.Body = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MuOutputGenerationNotification, context);
              }

                //message.From = new MailComponent.MailAddress(emailSettingForOutputNotification.SingleOrDefault().FromEmailAddress, "SIS No Reply");
                //message.Subject = string.Format("{0} [Billing Category- {1}", emailSettingForOutputNotification.SingleOrDefault().Subject, billingCategory);
                // string.Format("Your invoice files are ready for download. [File: {0}]", Path.GetFileName(outputZipFileName));
                /*context.Put("Message", new OutputGenerationNotificationMessage() {SisEmailId = message.From.ToString()});*/
                //message.Body = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.OutputGenerationNotification, context);
                //message.Body = string.Format("The downloadable zip file requested is ready for download at {0}", outputZipFileName);

              emailSender.Send(message);
          }
          catch (Exception ex)
          {
              Logger.ErrorFormat("Failed sending the mail {0} {1} {2}", ex, ex.StackTrace, ex.InnerException);
              message.Dispose();
              // Exception is not rethrown again as failure to send email should not affect further processing.
              return false;
          }

          return true;
      }

      /// <summary>
    /// This will send the SIS Admin alert for pending processes
    /// </summary>
    /// <param name="memberCode">memberCode</param>
    /// <param name="errorMessage">errorMessage</param>
    /// <param name="billingPeriod">billingPeriod</param>
    public void SendSisAdminAlert(string memberCode, string errorMessage, BillingPeriod billingPeriod)
    {
      var currentBillingPeriodText = string.Format("{0:D2}/{1:D4}/{2:D2}", billingPeriod.Month, billingPeriod.Year, billingPeriod.Period);
      // Create an object of the nVelocity data dictionary
      var context = new VelocityContext();
      context.Put("MemberCode", memberCode);
      context.Put("Period", currentBillingPeriodText);
      context.Put("ErrorMessage", errorMessage);
      const string message = "Output file generation failure for member {0}";
      const string title = "Output file generation failure alert";

      // SCP:55223- Adding Sis Support Email address for sending exception alerts.
      String internalTeamAdminEmailAddress = Core.Configuration.ConnectionString.GetconfigAppSetting("ExceptionEmailNotification");

      var issisOpsAlert = new ISSISOpsAlert
      {
        Message = String.Format(message, memberCode),
        AlertDateTime = DateTime.UtcNow,
        IsActive = true,
        EmailAddress = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail + "; " + internalTeamAdminEmailAddress,
        Title = title
      };

      BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.SISAdminAlertOutputFileGenerationPendingProcessesNotification, context);
    }

    /// <summary>
    ///  This will send email to SIS Admin if Weekly Invoice Posting Report is not created in case of AutoBilling
    /// </summary>
    /// <param name="memberCode">memberCode</param>
    /// <param name="billingPeriod">billingPeriod</param>
    public void SendSisAdminAlertAutoBilling( string memberCode, BillingPeriod billingPeriod)
    {
      DateTime date = new DateTime(billingPeriod.Year,billingPeriod.Month,1);
      var currentBillingPeriodText = string.Format("P{0:D1}-{1:D3}-{2:D4}", billingPeriod.Period, date.ToString("MMM"), billingPeriod.Year);
      // Create an object of the nVelocity data dictionary
      var context = new VelocityContext();
      context.Put("MemberCode", memberCode);
      context.Put("Period", currentBillingPeriodText);
      const string message = "Auto billing weekly posting report generation failure for member {0}";
      const string title = "SIS: Admin Alert- Exception in creation of Passenger Auto-Billing Weekly Invoice Posting File";

      // SCP:55223- Adding Sis Support Email address for sending exception alerts.
      String internalTeamAdminEmailAddress = Core.Configuration.ConnectionString.GetconfigAppSetting("ExceptionEmailNotification");

      var issisOpsAlert = new ISSISOpsAlert
      {
        Message = String.Format(message, memberCode),
        AlertDateTime = DateTime.UtcNow,
        IsActive = true,
        EmailAddress = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail + "; " + internalTeamAdminEmailAddress,
        Title = title
      };

      //Call to EmailTemplateID: 80
      BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.SISAdminAlertAutoBillingWeeklyPostingFailureNotification, context);
    }


    /// <summary>
    ///  This will send email to SIS Admin if Daily Revenue Recognition Report is not created in case of AutoBilling
    /// </summary>
    /// <param name="memberCode">memberCode</param>
    /// <param name="billingPeriod">billingPeriod</param>
    public void SendSisAdminAlertDailyRevenueRecognitionReport(string memberCode, BillingPeriod billingPeriod)
    {
      
      // Create an object of the nVelocity data dictionary
      var context = new VelocityContext();
      context.Put("MemberCode", memberCode);
      const string message = "Auto billing Daily Revenue Recognition report generation failure for member {0}";
      const string title = "SIS: Admin Alert- Exception in creation of Passenger Auto-Billing Daily Revenue Recognition File";

      var issisOpsAlert = new ISSISOpsAlert
      {
        Message = String.Format(message, memberCode),
        AlertDateTime = DateTime.UtcNow,
        IsActive = true,
        EmailAddress = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
        Title = title
      };

      //Call to EmailTemplateID: 84
      BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.SISAdminAlertDailyRevenueRecognitionReportGenerationFailureNotification, context);
    }

    /// <summary>
    ///  This will send email to SIS Admin if Daily AutoBilling Irregularity Report is not created.
    /// </summary>
    /// <param name="memberCode">memberCode</param>
    /// <param name="billingPeriod">billingPeriod</param>
    public void SendSisAdminAlertDailyIrregularityReport(string memberCode)
    {

      // Create an object of the nVelocity data dictionary
      var context = new VelocityContext();
      context.Put("MemberCode", memberCode);
      const string message = "Auto billing Daily Irregularity report generation failure for member {0}";
      const string title = "SIS: Admin Alert- {0} - Exception in creation of Passenger Auto-Billing/Value-Request Irregularity Report";

      var issisOpsAlert = new ISSISOpsAlert
      {
        Message = String.Format(message, memberCode),
        AlertDateTime = DateTime.UtcNow,
        IsActive = true,
        EmailAddress = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
        Title = String.Format(title, DateTime.UtcNow.ToString("dd-MMM-yyyy"))
      };

      //Call to EmailTemplateID: 89
      BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.SISAdminAlertDailyAutoBillingIrregularityReportGenerationFailureNotification, context);
    }

    /// <summary>
    ///  This will send email to SIS Admin if Daily AutoBilling Irregularity Report is not created.
    /// </summary>
    public void SendSisAdminAlertIrregularityReportRegenerationStatus(IEnumerable<string> toEmailList)
    {

      try
      {
        // Get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof (IEmailSender));
        // Get an instance of email settings repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof (IRepository<EmailTemplate>));
        // Verify and log TemplatedTextGenerator for null value.
        Logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].",
                                  TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

        // Generate email body text for ICH settlement web service failure notification email
        var context = new VelocityContext();
        context.Put("date", DateTime.UtcNow.ToString("dd-MMM-yyyy"));
        context.Put("emailID", AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        // Verify and log TemplatedTextGenerator for null value.
        Logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].",
                                  TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

        var emailSettingForISAdminAlert =
          emailSettingsRepository.Get(
            esfopu => esfopu.Id == (int) EmailTemplateId.SISAdminAlertAutoBillingIrregularityReportRegenerateStatus);

        // Generate email body text for own profile updates contact type mail
        if (TemplatedTextGenerator != null)
        {
          var emailToISAdminText =
            TemplatedTextGenerator.GenerateTemplatedText(
              EmailTemplateId.SISAdminAlertAutoBillingIrregularityReportRegenerateStatus, context);
          // Create a mail object to send mail
          var msgForISAdmin = new MailMessage
                                {
                                  From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                                  IsBodyHtml = true
                                };

          var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
          subject = subject.Replace("<date>", DateTime.UtcNow.ToString("dd-MMM-yyyy"));
          msgForISAdmin.Subject = subject;


          // loop through the contacts list and add them to To list of mail to be sent
          if (toEmailList.Count() != 0)
          {
            foreach (var mailaddr in toEmailList)
            {
              msgForISAdmin.To.Add(mailaddr);
            }
          }

          //set body text of mail
          msgForISAdmin.Body = emailToISAdminText;

          //send the mail
          emailSender.Send(msgForISAdmin);

        }
      } // End try
      catch (Exception ex)
      {

        Logger.Error("Error occurred while sending Archive Failure Notification to IS Administrator", ex);
        // throw new ISBusinessException(ErrorCodes.ICHSettlementErrorNotificationFailed, "Error occurred while sending  Archive WSConnect Failure  Notification to IS Administrator");
      } // End catch
    }

    /// <summary>
    /// To get the Contact list for memeberId for specified ProcessingContactType.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="processingContact"></param>
    /// <returns></returns>
    private static List<Contact> GetToEmailIds(int memberId, ProcessingContactType processingContact)
    {
      var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
      var contactTypeList = memberManager.GetContactsForContactType(memberId, processingContact);

      return contactTypeList;
    }

    /// <summary>
    /// CMP529: Method added to get all invoices for daily output file generation.
    /// </summary>
    /// <param name="billedMemberId">billed member id</param>
    /// <param name="targetDate">delivery date</param>
    /// <param name="dailyDeliveryStatusId">delivery status</param>
    /// <param name="invoiceStatusIds">status of invoice</param>
    /// <param name="billingCategoryId">billing category for invoice</param>
    /// <param name="locationId">location code for location specific invoices</param>
    /// <param name="islocationSpec">CMP#622 Changes: true if location spec invoices required</param>
    /// <returns>list invoices</returns>
    public List<MiscUatpInvoice> GetMiscInvoicesForDailyXmlOutput(int billedMemberId, DateTime targetDate, int? dailyDeliveryStatusId, string invoiceStatusIds, int billingCategoryId, int? islocationSpec = null, string locationId= null)
    {
      try
      {
        // replaced with LoadStrategy call
        var filteredList = MiscInvoiceRepository.GetMiscUatpInvoices(billedMemberId
          , invoiceStatusIds: invoiceStatusIds, billingCategoryId: billingCategoryId, targetDate: targetDate, dailyDeliveryStatus: dailyDeliveryStatusId, outputType:islocationSpec, locationId:locationId);


        return filteredList;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}

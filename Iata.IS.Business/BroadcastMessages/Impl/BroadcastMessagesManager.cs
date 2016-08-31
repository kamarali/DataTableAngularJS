using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using Castle.Core.Smtp;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.AdminSystem;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using log4net;
using NVelocity;

namespace Iata.IS.Business.BroadcastMessages.Impl
{
  public class BroadcastMessagesManager : IBroadcastMessagesManager
  {
    // Logger instance.
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Gets or sets the blocking rule repository.
    /// </summary>
    /// <value>The blocking rule repository.</value>
    public IRepository<ISMessagesAlerts> ISMessagesAlertsRepository { get; set; }
    public IRepository<ISMessageRecipients> ISMessagesRecipientsRepository { get; set; }
    public IRepository<ISSISOpsAlert> ISSISOpsAlertRepository { get; set; }

    public ISMessagesAlerts AddAnnouncements(ISMessagesAlerts announcement)
    {

      if (announcement != null)
      {
        //If date and time for publishing announcements is past date or current date then throw exception
        var startDateTime = CalendarManager.ConvertYmqTimeToUtc(announcement.StartDateTime);
        // if ((startDateTime.Hour < DateTime.UtcNow.Hour) || ((startDateTime.Hour == DateTime.UtcNow.Hour) && (startDateTime.Minute < DateTime.UtcNow.Minute))) throw new ISBusinessException(ErrorCodes.InvalidDateAndTimeForAnnouncements);
        announcement.StartDateTime = startDateTime;
        announcement.TypeId = (int)MessageType.Announcement;
        announcement.IsActive = true;
        ISMessagesAlertsRepository.Add(announcement);
        UnitOfWork.CommitDefault();
        return announcement;
      }
      return null;
    }

    public ISMessageRecipients AddMessages(ISMessageRecipients message)
    {
      if (message != null)
      {
        message.IsMessagesAlerts.IsActive = true;
        message.IsMessagesAlerts.TypeId = (int)MessageType.Message;

        //Determine value of 'Member category' field based on Recipients selected
        if ((message.MemberCategory != "L") && (message.MemberCategory != "I") && (message.MemberCategory != "A"))
        {
          message.MemberId = Convert.ToInt32(message.MemberCategory);
          message.MemberCategory = "S";
        }
        else
        {
          message.MemberId = null;
        }

        ISMessagesRecipientsRepository.Add(message);
        UnitOfWork.CommitDefault();
        return message;
      }
      return null;
      //throw new NotImplementedException();
    }

    public ISMessageRecipients AddAlerts(ISMessageRecipients alert)
    {

      if (alert != null)
      {
        alert.IsMessagesAlerts.IsActive = true;
        alert.IsMessagesAlerts.TypeId = (int)MessageType.Alert;
        alert.LastUpdatedOn = DateTime.UtcNow;
        ISMessagesRecipientsRepository.Add(alert);
        UnitOfWork.CommitDefault();
        return alert;
      }

      return null;

    }

    ISSISOpsAlert IBroadcastMessagesManager.AddAlert(ISSISOpsAlert alert, EmailTemplateId emailTemplateId, VelocityContext context)
    {
      return AddAlert(alert, emailTemplateId, context);
    }

    /// <summary>
    /// Adds the alert for Is Admin.
    /// </summary>
    /// <param name="alert">The alert object to be added.</param>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <param name="context">The context object populated with placeholder values.</param>
    /// <returns>
    /// Added IS SIS Ops Alert object
    /// </returns>
    public static ISSISOpsAlert AddAlert(ISSISOpsAlert alert, EmailTemplateId emailTemplateId, VelocityContext context)
    {

      if (alert != null)
      {
        var isSisOpsAlertRepository = Ioc.Resolve<IRepository<ISSISOpsAlert>>(typeof(IRepository<ISSISOpsAlert>));

        alert.IsActive = true;
        alert.LastUpdatedOn = DateTime.UtcNow;
        isSisOpsAlertRepository.Add(alert);
        UnitOfWork.CommitDefault();
        Ioc.Release(isSisOpsAlertRepository);

        SendISAdminProcessFailedNotification(alert, emailTemplateId, context);

        return alert;
      }
      return null;
    }

    /// <summary>
    /// Sends the IS admin process failed notification.
    /// </summary>
    /// <param name="alert">The alert.</param>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <param name="context">The context.</param>
    private static void SendISAdminProcessFailedNotification(ISSISOpsAlert alert, EmailTemplateId emailTemplateId, VelocityContext context)
    {
      try
      {
        // Get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        // Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        Logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", templatedTextGenerator == null ? "NULL" : "NOT NULL"));

        var emailSettingForISAdminProcessFailedNotification = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

        // Generate email body text for own profile updates contact type mail
        var emailToMemberPrimaryContactsText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
        // Create a mail object to send mail
        var msgForISAdminProcessFailedNotification = new MailMessage
        {
          From = new MailAddress(emailSettingForISAdminProcessFailedNotification.SingleOrDefault().FromEmailAddress),
          IsBodyHtml = true
        };

        // Loop through the contacts list and add them to To list of mail to be sent
        alert.EmailAddress = alert.EmailAddress.Replace(Convert.ToChar(","), Convert.ToChar(";"));
        string[] emailAddress = alert.EmailAddress.Split(Convert.ToChar(";"));

        if (emailAddress.Length > 0)
        {
          for (int i = 0; i < emailAddress.Length; i++)
          {
            msgForISAdminProcessFailedNotification.To.Add(emailAddress[i]);
          }
        }
        // Set subject of mail (replace special field placeholders with values)
        msgForISAdminProcessFailedNotification.Subject = emailSettingForISAdminProcessFailedNotification.SingleOrDefault().Subject;

        // Set body text of mail
        msgForISAdminProcessFailedNotification.Body = emailToMemberPrimaryContactsText;

        Logger.InfoFormat("Sending Email...");
        // Send the mail
        if (msgForISAdminProcessFailedNotification.To.Count() > 0)
          emailSender.Send(msgForISAdminProcessFailedNotification);

        Logger.InfoFormat("Sending Email completed.");

        return;
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred while sending an email to IS-Admin.", exception);
        return;
      }
    }

    /// <summary>
    /// Sends the IS admin process failed notification.
    /// </summary>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <param name="serviceName"></param>
    /// <param name="topLevelException"></param>
    /// <param name="emailToSisOps">emailToSisOps</param>
    public void SendISAdminExceptionNotification(EmailTemplateId emailTemplateId, string serviceName, Exception topLevelException, bool emailToSisOps = true)
    {
      try
      {
        // Get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a n-velocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        // Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        var emailSettingForISAdminProcessFailedNotification = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

        // Generate email body text for email
        var context = new VelocityContext();
        context.Put("ServiceName", serviceName);
        context.Put("OuterException", topLevelException);
        var emailBodyText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
        // Create a mail object to send mail
        var mailISAdminProcessFailedNotification = new MailMessage
        {
          From = new MailAddress(emailSettingForISAdminProcessFailedNotification.SingleOrDefault().FromEmailAddress),
          IsBodyHtml = true
        };

        // Loop through the contacts list and add them to To list of mail to be sent
        // SCP110222: Missing data from Mar P4 IS-IDEC
        if (emailToSisOps)
        {
          mailISAdminProcessFailedNotification.To.Add(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        }

        //To be uncommented in production or after go live
        // SCP:55223- Adding Sis Support Email address for sending exception alerts.
        mailISAdminProcessFailedNotification.To.Add(Core.Configuration.ConnectionString.GetconfigAppSetting("ExceptionEmailNotification"));

        // Set subject of mail (replace special field placeholders with values)
        mailISAdminProcessFailedNotification.Subject = emailSettingForISAdminProcessFailedNotification.SingleOrDefault().Subject.Replace("$ServiceName$", serviceName);

        // Set body text of mail
        mailISAdminProcessFailedNotification.Body = emailBodyText;

        // Send the mail
        if (mailISAdminProcessFailedNotification.To.Count() > 0)
          emailSender.Send(mailISAdminProcessFailedNotification);

        return;
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred while sending an email to IS-Admin.", exception);
        return;
      }
    }

    public bool SendOutputAvailableAlert(int memberId, string[] toEmailAddresses, string period, string currentBillingPeriodFormatted, EmailTemplateId emailTemplateId)
    {
      try
      {
        // Create an object of the nVelocity data dictionary
        var context = new VelocityContext();
        context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        context.Put("Period", currentBillingPeriodFormatted);

        // Get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a n-velocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        // Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        var emailSettingForOutputAvailableAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

        // Generate email body text for own profile updates contact type mail
        var emailToMemberPrimaryContactsText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
        // Create a mail object to send mail
        var msgForOutputAvailableAlert = new MailMessage
        {
          From = new MailAddress(emailSettingForOutputAvailableAlert.SingleOrDefault().FromEmailAddress),
          IsBodyHtml = true
        };

        // Loop through the contacts list and add them to To list of mail to be sent
        foreach (var emailAddress in toEmailAddresses)
        {
          msgForOutputAvailableAlert.To.Add(emailAddress);
        }

        // Set subject of mail (replace special field placeholders with values)
        msgForOutputAvailableAlert.Subject = emailSettingForOutputAvailableAlert.SingleOrDefault().Subject.Replace("$Period$", period);

        // Set body text of mail
        msgForOutputAvailableAlert.Body = emailToMemberPrimaryContactsText;

        var messageRecipients = new ISMessageRecipients
        {
          MemberId = memberId,
          ContactTypeId = string.Format("{0},{1},{2},{3}", (int)ProcessingContactType.PAXOutputAvailableAlert,
                                        (int)ProcessingContactType.CGOOutputAvailableAlert,
                                        (int)ProcessingContactType.MISCOutputAvailableAlert,
                                        (int)ProcessingContactType.UATPOutputAvailableAlert),
          IsMessagesAlerts = new ISMessagesAlerts
          {
            Message = "Output Available Alert - " + period,
            StartDateTime = DateTime.UtcNow,
            LastUpdatedOn = DateTime.UtcNow,
            IsActive = true,
            TypeId = (int)MessageType.Alert,
            RAGIndicator = (int)RAGIndicator.Green
          }
        };

        AddAlerts(messageRecipients);

        // Send the mail
        if (msgForOutputAvailableAlert.To.Count() > 0)
          emailSender.Send(msgForOutputAvailableAlert);

        return true;
      }
      catch (Exception exception)
      {
        Logger.Info(exception.StackTrace);
        Logger.Error("Error occurred in output available alert Email Handler (Send Mails for a multiple Members method).", exception);
        return false;
      }
    }

    public bool SendAutoBillInvoiceNumberAlert(int memberId, string[] toEmailAddresses, string period, string currentBillingPeriodFormatted, EmailTemplateId emailTemplateId)
    {
      try
      {
        // Create an object of the nVelocity data dictionary
        var context = new VelocityContext();
        context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

        // Get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a n-velocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        // Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        var emailSettingForOutputAvailableAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

        // Generate email body text for own profile updates contact type mail
        var emailToMemberPrimaryContactsText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
        // Create a mail object to send mail
        var msgForOutputAvailableAlert = new MailMessage
        {
          From = new MailAddress(emailSettingForOutputAvailableAlert.SingleOrDefault().FromEmailAddress),
          IsBodyHtml = true
        };

        // Loop through the contacts list and add them to To list of mail to be sent
        foreach (var emailAddress in toEmailAddresses)
        {
          msgForOutputAvailableAlert.To.Add(emailAddress);
        }

        // Set subject of mail (replace special field placeholders with values)
        msgForOutputAvailableAlert.Subject = emailSettingForOutputAvailableAlert.SingleOrDefault().Subject;

        // Set body text of mail
        msgForOutputAvailableAlert.Body = emailToMemberPrimaryContactsText;

        // Send the mail
        if (msgForOutputAvailableAlert.To.Count() > 0)
          emailSender.Send(msgForOutputAvailableAlert);

        return true;
      }
      catch (Exception exception)
      {
        Logger.Info(exception.StackTrace);
        Logger.Error("Error occurred in output available alert Email Handler (Send Mails for a multiple Members method).", exception);
        return false;
      }
    }

    public bool AddCorrespondenceAlert(int memberId, ProcessingContactType contactType, string message)
    {
      try
      {
        var messageRecipients = new ISMessageRecipients
        {
          MemberId = memberId,
          ContactTypeId = Convert.ToString((int)contactType),

          IsMessagesAlerts = new ISMessagesAlerts
          {
            Message = message,
            StartDateTime = DateTime.UtcNow,
            LastUpdatedOn = DateTime.UtcNow,
            IsActive = true,
            TypeId = (int)MessageType.Alert,
            RAGIndicator = (int)RAGIndicator.Red
          }
        };

        AddAlerts(messageRecipients);
        return true;
      }
      catch (Exception exception)
      {
        Logger.Info(exception.StackTrace);
        Logger.Error("Error occurred in Correspondence alert Handler ", exception);
        return false;
      }
    }

    /// <summary>
    /// This alert is used to alert member about the generation of ISIDEC\ISXML payable files
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="contactType"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public bool AddPayableInvoicesAvailableAlert(int memberId, ProcessingContactType contactType, string message)
    {
      try
      {
        var messageRecipients = new ISMessageRecipients
        {
          MemberId = memberId,
          ContactTypeId = Convert.ToString((int)contactType),

          IsMessagesAlerts = new ISMessagesAlerts
          {
            Message = message,
            StartDateTime = DateTime.UtcNow,
            LastUpdatedOn = DateTime.UtcNow,
            IsActive = true,
            TypeId = (int)MessageType.Alert,
            RAGIndicator = (int)RAGIndicator.Red
          }
        };

        AddAlerts(messageRecipients);
        return true;
      }
      catch (Exception exception)
      {
        Logger.Info(exception.StackTrace);
        Logger.Error("Error occurred in Correspondence alert Handler ", exception);
        return false;
      }
    }

    /// <summary>
    /// Sends the output file available notification.
    /// </summary>
    /// <param name="toEmailAddress">To email addresses.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <returns></returns>
    public bool SendOutputFileAvailableNotification(string toEmailAddress, string filePath, EmailTemplateId emailTemplateId, VelocityContext context)
    {
      try
      {
        context.Put("FilePath", filePath);

        // Get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a n-velocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        // Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        var emailSettingForOutputAvailableAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

        // Generate email body text for own profile updates contact type mail
        var emailToMemberPrimaryContactsText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
        // Create a mail object to send mail
        var msgForOutputAvailableAlert = new MailMessage
        {
          From = new MailAddress(emailSettingForOutputAvailableAlert.SingleOrDefault().FromEmailAddress),
          IsBodyHtml = true
        };

        msgForOutputAvailableAlert.To.Add(toEmailAddress);

        // Set subject of mail (replace special field placeholders with values)
        switch (emailTemplateId)
        {
          case EmailTemplateId.OutputFileAvailableAlert:
            msgForOutputAvailableAlert.Subject = emailSettingForOutputAvailableAlert.SingleOrDefault().Subject.Replace("$InvoiceNumber$", context.Get("InvoiceNumber").ToString());
            break;
          case EmailTemplateId.OutputFileAvailableForFormCAlert:
            var subject = emailSettingForOutputAvailableAlert.SingleOrDefault().Subject.Replace("$ProvisionalMonth$", context.Get("ProvisionalMonth").ToString());
            msgForOutputAvailableAlert.Subject = subject.Replace("$MemberCode$", context.Get("MemberCode").ToString());
            break;
            case EmailTemplateId.AuditTrailPackageDownloadNotification:
                msgForOutputAvailableAlert.Subject =
                    emailSettingForOutputAvailableAlert.SingleOrDefault().Subject.Replace("$FileName",context.Get("FileName").ToString());
                break;
           case EmailTemplateId.ReportDownloadNotification:
		msgForOutputAvailableAlert.Subject = emailSettingForOutputAvailableAlert.SingleOrDefault().Subject.Replace("$ReportName$", context.Get("ReportName").ToString());
		break;
          default:
            msgForOutputAvailableAlert.Subject = emailSettingForOutputAvailableAlert.SingleOrDefault().Subject;
            break;
        }

        // Set body text of mail
        msgForOutputAvailableAlert.Body = emailToMemberPrimaryContactsText;

        // Send the mail
        if (msgForOutputAvailableAlert.To.Count() > 0)
          emailSender.Send(msgForOutputAvailableAlert);

        return true;
      }
      catch (Exception exception)
      {
        Logger.Info(exception.StackTrace);
        Logger.Error("Error occurred in output available alert Email Handler (Send Mails for a multiple Members method).", exception);
        return false;
      }
    }

    public IList<AlertsMessagesAnnouncementsResultSet> GetSisOpsAlerts(DateTime? ThresholdDate)
    {
      if (ThresholdDate != null)
      {
        var thresholdDate = Convert.ToDateTime(ThresholdDate);
        var startDate = new DateTime(thresholdDate.Year, thresholdDate.Month, thresholdDate.Day, 0, 0, 0);
        var endDate = new DateTime(thresholdDate.Year, thresholdDate.Month, thresholdDate.Day, 23, 59, 59);
        var sisopasAlertList = ISSISOpsAlertRepository.Get(a => a.IsActive && a.AlertDateTime >= startDate && a.AlertDateTime <= endDate).ToList();
        var alertResultSet = (from a in sisopasAlertList
                              where a.AlertDateTime >= startDate && a.AlertDateTime <= endDate
                              select
                                  new AlertsMessagesAnnouncementsResultSet { Detail = a.Message, FromDate = a.AlertDateTime, MessageId = a.Id, RaisedDate = a.AlertDateTime, ExpiryDate = a.AlertDateTime }).ToList();
        return alertResultSet;
      }
      else
      {
        var sisopasAlertList = ISSISOpsAlertRepository.Get(a => a.IsActive).ToList();
        var alertResultSet = (from a in sisopasAlertList
                              select
                                  new AlertsMessagesAnnouncementsResultSet { Detail = a.Message, FromDate = a.AlertDateTime, MessageId = a.Id, RaisedDate = a.AlertDateTime, ExpiryDate = a.AlertDateTime }).ToList();
        return alertResultSet;
      }
    }

    public bool ClearSisOpsAlertMessage(Guid id)
    {
      bool flag = false;
      var sisopasAlert = ISSISOpsAlertRepository.Get(a => a.Id == id).SingleOrDefault();
      if (sisopasAlert != null)
      {
        sisopasAlert.IsActive = false;
        UnitOfWork.CommitDefault();
        flag = true;
      }
      return flag;
    }

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    /// <summary>
    /// Sends tan email notification.
    /// </summary>
    /// <param name="toEmailAddress">To email addresses.</param>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <param name="context">Velocity Context.</param>
    /// <returns>success/failure</returns>
    public bool SendEmailNotification(string toEmailAddress, EmailTemplateId emailTemplateId, VelocityContext context)
    {
        try
        {
            // Get an object of the EmailSender component
            var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

            // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a n-velocity template
            var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
            // Get an instance of email settings  repository
            var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

            var emailTemplate = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

            // Generate email body text for own profile updates contact type mail
            var emailToMemberPrimaryContactsText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
            // Create a mail object to send mail
            var msgForEmailNotification = new MailMessage
            {
                From = new MailAddress(emailTemplate.SingleOrDefault().FromEmailAddress),
                IsBodyHtml = true
            };

            foreach (var mailaddr in ConvertUtil.ConvertToMailAddresses(toEmailAddress))
            {
              msgForEmailNotification.To.Add(mailaddr);
            }
            
            // Set subject of mail (replace special field placeholders with values)
            switch (emailTemplateId)
            {
                case EmailTemplateId.SISAdminDailyOutputProcessFailureNotification:
                    msgForEmailNotification.Subject =
                        emailTemplate.SingleOrDefault().Subject.Replace("$TargetDate$",
                                                                        context.Get("TargetDate").ToString()).
                            Replace("$ProcessName$", context.Get("ProcessName").ToString());
                    break;
                case EmailTemplateId.SISAdminDailyOutputProcessCompletionNotification :
                    msgForEmailNotification.Subject =
                        emailTemplate.SingleOrDefault().Subject.Replace("$TargetDate$",
                                                                        context.Get("TargetDate").ToString());
                    break;
                case EmailTemplateId.CorrespondeceEmailNotification:
                    msgForEmailNotification.Subject = emailTemplate.SingleOrDefault().Subject.Replace("$BillingCategory$",
                                                                        context.Get("BillingCategory").ToString()).Replace("$CorrespondenceNo$", context.Get("CorrespondenceNumber").ToString());
                    break;
                default:
                    msgForEmailNotification.Subject = emailTemplate.SingleOrDefault().Subject;
                    break;
            }

            //Set body text of mail
            msgForEmailNotification.Body = emailToMemberPrimaryContactsText;

            // Send the mail
            if (msgForEmailNotification.To.Count() > 0)
                emailSender.Send(msgForEmailNotification);

            return true;
        }
        catch (Exception exception)
        {
            Logger.Info(exception.StackTrace);
            Logger.Error("Error occurred while sending email notification", exception);
            return false;
        }
    }

    #region CMP#608: Load Member Profile - CSV Option

    /// <summary>
    /// Sends Email Notifications for Level 1 and Level 2 Validation Failure as Per CMP#608.
    /// </summary>
    /// <param name="level">either 1 or 2</param>
    /// <param name="fileName">Name of zip file as uploaded by user</param>
    /// <param name="submissionDateTime">Date/time of file submission after conversion to EDT considering daylight savings if any. Format should be dd-Mon-YYYY, hh24:mm:ss</param>
    /// <param name="userName">Name of SIS Ops user who uploaded the file. Format should be First Name followed by space followed by Last Name</param>
    /// <param name="reasonForFailure">Error text depending on failure of specific validation.</param>
    /// <param name="failedRowNumber">Make Sense only for Level 2 mail.</param>
    /// <param name="failedField">Make Sense only for Level 2 mail.</param>
    /// <param name="failedFieldValue">Make Sense only for Level 2 mail.</param>
    public void SendMemberProfileCsvUploadEmailNotification(int level, string fileName, string submissionDateTime, 
        string userName, string reasonForFailure, string failedRowNumber, string failedField, string failedFieldValue)
    {
        try
        {
            /* Decide Email Template to use on basis of level */
            EmailTemplateId emailTemplateId;
            switch (level)
            {
                case 0:
                    emailTemplateId = EmailTemplateId.MemberProfileCsvUploadSuccessfulNotification;
                    break;
                case 1:
                    emailTemplateId = EmailTemplateId.MemberProfileCsvUploadLevel1FailureNotification;
                    break;
                case 2:
                    emailTemplateId = EmailTemplateId.MemberProfileCsvUploadLevel2FailureNotification;
                    break;
                case 3:
                    emailTemplateId = EmailTemplateId.MemberProfileCsvUploadLoadingFailureNotification;
                    break;
                default:
                    return;
            }

            /* Get an object of the EmailSender component */
            var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

            /* Get an object of the TemplatedTextGenerator that is used to generate body text of email from a n-velocity template */
            var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

            /* Get an instance of email settings repository */
            var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

            /* Get email settings for Template */
            var emailSetting = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

            /* Generate email body text for email */
            var context = new VelocityContext();

            /* Keys common to Level 1 and 2 email body */
            context.Put("Filename", fileName);
            context.Put("SubmissionDateTime", submissionDateTime);
            context.Put("UploadedBy", userName);

            /* Bypass Level 0 i.e. Success mail */
            if (level != 0)
            {
                context.Put("FailureReason", reasonForFailure);
            }

            if(level == 2)
            {
                /* Keys specific only to Level 2 failure email */
                context.Put("failedRowNumber", failedRowNumber);
                context.Put("failedField", failedField);
                context.Put("failedFieldValue", failedFieldValue);
            }

            var emailBodyText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);

            /* Create and Send Email Message */
            var emailMessage = new MailMessage();
            //emailMessage.From = new MailAddress(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
            emailMessage.IsBodyHtml = true;
            emailMessage.Subject = emailSetting.SingleOrDefault().Subject.Replace("$Filename$", fileName);
            emailMessage.Body = emailBodyText;
            string[] toEmailIds = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (toEmailIds.Length > 0)
            {
                foreach (var emailId in toEmailIds)
                {
                    emailMessage.To.Add(new MailAddress(emailId));
                }

            }

            if (emailMessage.To.Count() > 0)
                emailSender.Send(emailMessage);

            return;
        }
        catch (Exception exception)
        {
            Logger.Info("Exception occurred in Iata.IS.Business.BroadcastMessages.Impl.SendMemberProfileCsvUploadEmailNotification()");
            Logger.Error(exception);
            return;
        }
    }

    #endregion

  }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using iPayables.UserManagement;
using log4net;
using NVelocity;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.Common.Impl
{
  /// <summary>
  /// The class implement all the functionality required for CMP#527.
  /// Since the functionalities are common across all billing categories,
  /// hence it acts as base class of all correspondence manager classes specific to billing categories.
  /// </summary>
  public class CorrespondenceManager : ICorrespondenceManager
  {
    public ICorrespondenceRepository CorrespondenceRep { get; set; }
    public IMemberManager MemberManager { get; set; }
    public IUserManagement AuthManager { get; set; }
    //SCP210204: IS-WEB Outage
    public IReferenceManager ReferenceManager;
    private IBroadcastMessagesManager _broadcastMessagesManager;

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    #region Constants
    // for SendCorrepondenceEmailNotificationAndAlert
    private const string AlertScenario1 = "The time limit for responding to Received {0} correspondence number {1} is {2} 23:59:59 UTC." +
                                          " No response has been sent from your organization for this correspondence so far." +
                                          " If a response is not sent from your organization by this time limit, it will result in Expiry; and will allow a Debit Due to No Response by the Member who initiated this correspondence.";
    
    private const string AlertScenario2 = "The time limit for submitting a settlement (Billing Memo or Correspondence Invoice) for Expired {0} correspondence number {1} is the submission deadline of {2}." +
                                          " No settlement has been received in SIS for this correspondence so far." +
                                          " If a settlement is not received in SIS by this time limit, it will result in permanent Closure of this correspondence Due to Expiry.";
   
    private const string AlertScenario3 = "The time limit for responding to Received {0} correspondence number {1} is {2} 23:59:59 UTC. " +
                                          " No response has been sent from your organization for this correspondence so far." +
                                          " If a response is not sent from your organization by this time limit, it will result in permanent Closure of this correspondence Due to Expiry.";
    
    private const string AlertScenario4 = "The time limit for responding to Received {0} correspondence number {1} having an Authority to Bill is {2} 23:59:59 UTC." +
                                          " No response has been sent from your organization for this correspondence so far." +
                                          " Any response on this correspondence will not be possible beyond this time limit." +
                                          " However, in the event of no response on this correspondence by this time limit, a settlement (Billing Memo or Correspondence Invoice) will be possible till the submission deadline of {3}.";
    
    private const string AlertScenario5 = "The time limit for submitting a settlement (Billing Memo or Correspondence Invoice) for Received {0} correspondence number {1} having an Authority to Bill is the submission deadline of {2}." +
                                          " No settlement has been received in SIS for this correspondence so far." +
                                          " If a settlement is not received in SIS by this time limit, it will result in permanent Closure of this correspondence Due to Expiry.";

    #endregion

    // SCP210204: IS-WEB Outage [To resolve null reference]
    public CorrespondenceManager(IReferenceManager referenceManager)
    {
      ReferenceManager = referenceManager;
    }

    /// <summary>
    /// CMP 527
    /// Check can correspondence close or not.
    /// </summary>
    /// <param name="billingcategoryId">billing category id.</param>
    /// <param name="correspondenceId">Correspondence id</param>
    /// <returns>
    /// true: if correspondence can able to close
    /// false: if correspondece can not close.
    /// </returns>
    public int[] CanCorrespondenceClose(int billingcategoryId, string correspondenceId)
    {
      //SCP210204: IS-WEB Outage (added log)
      var stopWatch = new Stopwatch();
      stopWatch.Start();
      var canCloseCorr=CorrespondenceRep.CanCorrespondenceClose(billingcategoryId, new Guid(correspondenceId));
      stopWatch.Stop();
      var log = ReferenceManager.GetDebugLog(DateTime.UtcNow, "CanCorrespondenceClose", this.ToString(),
                                   BillingCategorys.Passenger.ToString(), "CanCorrespondenceClose", 0, stopWatch.Elapsed.ToString());
      ReferenceManager.InsertLogDebugData(log);
      return canCloseCorr;
    }

    /// <summary>
    /// This is use to close correspondence 
    /// </summary>
    /// <param name="correspondenceNo">correspondence number</param>
    /// <param name="correspondenceStage">correspondence stage</param>
    /// <param name="correspondenceStatus">correspondence status</param>
    /// <param name="correspondenceSubStatus">correspondence sub status</param>
    /// <param name="scenarioNo">scenario number - 1/2/3/4/5/6/7</param>
    //SCENARIO 1: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = SAVED
    //SCENARIO 2: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
    //SCENARIO 3: STAGE = 1,   STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED
    //SCENARIO 4: STAGE = >=2, STATUS = OPEN     AND  SUB-STATUS = RECEVIED/RESPONDED
    //SCENARIO 5: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = SAVED
    //SCENARIO 6: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
    //SCENARIO 7: STAGE = >2,  STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED 
    /// <param name="billingCategoryId">billing category id</param>
    /// <param name="acceptanceComment">comments</param>
    /// <param name="acceptanceUserId">user id</param>
    /// <param name="acceptanceDateTime">datetime</param>
    /// <param name="message">message</param>
    /// <returns></returns>
    public bool CloseCorrespondence(string correspondenceNo, string correspondenceStage, string correspondenceStatus, string correspondenceSubStatus, int scenarioNo, int billingCategoryId, string acceptanceComment, int acceptanceUserId, DateTime acceptanceDateTime, ref string message)
    {
      try
      {
        //SCP210204: IS-WEB Outage (added log)
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var saved = 0;
        message = string.Empty;

        //SCP281499 - CMP 527, Closure of Correspondence
        //For Scenario 3, we make changes to include scenario 3 for acceptance comments not delete directly.
        if (scenarioNo>2)
        {
          if (string.IsNullOrEmpty(acceptanceComment))
          {
            message = "Acceptance Comments are required.";
            return false;
          }

          if (!string.IsNullOrEmpty(acceptanceComment))
          {
            if (acceptanceComment.Trim().Length >= 2001)
            {
              message = "Acceptance Comments cannot greater then 2000 chars.";
              return false;
            }
          }

         }

        saved = CorrespondenceRep.CloseCorrespondence(correspondenceNo, correspondenceStage, correspondenceStatus, correspondenceSubStatus, scenarioNo, billingCategoryId,
                                                    acceptanceComment, acceptanceUserId, acceptanceDateTime);
     
        switch (saved)
        {
          case 1:
            message = "Correspondence Closed Successfully.";
            break;
          case 2:
            message = "Unable to perform changes to this correspondence as the state of this correspondence has been changed from another session.";
            break;
          default:
            message = "Unable to close correspondence.";
            break;
        }
        stopWatch.Stop();
        var log = ReferenceManager.GetDebugLog(DateTime.UtcNow, "CloseCorrespondence", this.ToString(),
                                     BillingCategorys.Passenger.ToString(), "CloseCorrespondence", 0, stopWatch.Elapsed.ToString());
        ReferenceManager.InsertLogDebugData(log);
        return saved == 1 ? true : false;
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
        return false;
      }
    }

    /// <summary>
    /// Send Correspondence Alert
    /// </summary>
    /// <param name="emailTemplateId">Correspondence Template</param>
    /// <param name="correspondenceNumber">corr number</param>
    /// <param name="billingCategory">billing category</param>
    /// <param name="rejectionMemos">rejection memo</param>
    /// <param name="invoiceNo">invoice no</param>
    /// <param name="billingPeriod">billing period</param>
    /// <param name="billingMember">billing member</param>
    /// <param name="toEmailIds">to email ids.</param>
    private void SendCorrespondenceDeletedAlert(EmailTemplateId emailTemplateId, string correspondenceNumber, string billingCategory, string rejectionMemos, string invoiceNo, string billingPeriod, string billingMember, string toEmailIds)
    {
      try
      {

        // CMP#657: Retention of Additional Email Addresses in Correspondences. Email sending code is moved to method SendEmailThroughEmailSender().
        
        // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a n-velocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        // Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        var emailSettingForIsAdminProcessFailedNotification = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);
        // Generate email body text for email
        var context = new VelocityContext();
        context.Put("billingCategory", billingCategory);
        context.Put("correspondenceNumber", correspondenceNumber);
        context.Put("invoiceNumber", invoiceNo);
        context.Put("billingPeriod",billingPeriod);
        context.Put("billingMember", billingMember);
        context.Put("rejectionMemoNumber",
                    !string.IsNullOrEmpty(rejectionMemos)
                      ? string.Format("Linked Rejection Memo details: {0}", rejectionMemos)
                      : string.Empty);
        context.Put("sisOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
   
        var emailBodyText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
        // Create a mail object to send mail
        var mailMessage = new MailMessage
        {
          From = new MailAddress(emailSettingForIsAdminProcessFailedNotification.SingleOrDefault().FromEmailAddress),
          IsBodyHtml = true
        };
        
        // CMP#657: Retention of Additional Email Addresses in Correspondences. Email sending code is moved to method SendEmailThroughEmailSender().
        
        var subject = emailSettingForIsAdminProcessFailedNotification.SingleOrDefault().Subject;

        // Set subject of mail (replace special field placeholders with values)
        mailMessage.Subject = subject.Replace("$billingCategory", billingCategory).Replace("$correspondenceNumber", correspondenceNumber);

        // Set body text of mail
        mailMessage.Body = emailBodyText;

        // CMP#657: Retention of Additional Email Addresses in Correspondences. Email sending code is moved to method SendEmailThroughEmailSender().
        // Send the mail
        SendEmailThroughEmailSender(toEmailIds, mailMessage);

        return;
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
        return;
      }
    }


    //SCENARIO 1: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = SAVED
    //SCENARIO 2: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
    //SCENARIO 3: STAGE = 1,   STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED
    //SCENARIO 4: STAGE = >=2, STATUS = OPEN     AND  SUB-STATUS = RECEVIED/RESPONDED
    //SCENARIO 5: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = SAVED
    //SCENARIO 6: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
    //SCENARIO 7: STAGE = >2,  STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED 
    /// <summary>
    /// Method to send correspondence alert on close
    /// </summary>
    /// <param name="rejectionMemos">rejection memos comma separted list</param>
    /// <param name="invoice">invoice</param>
    /// <param name="correspondenceNumber">corr number</param>
    /// <param name="initiatorMemberId">initiator member id</param>
    /// <param name="nonInitiatorMemberId">non initiator member id</param>
    /// <param name="scenarioId">
    /// </param>
    /// <param name="closedByUserId">user id of user that closed correspondence</param>
    /// <param name="toEmails">to email ids</param>
    /// <param name="additionalEmailInitiator">TO_ADDITIONAL_EMAIL_IDS_OF_INITIATOR</param>
    /// <param name="additionalEmailNonInitiator">TO_ADDITIONAL_EMAIL_IDS_OF_NON_INITIATOR</param>
    public void SendCorrespondenceAlertOnClose(InvoiceBase invoice, string correspondenceNumber, int initiatorMemberId, int nonInitiatorMemberId, int scenarioId, int closedByUserId, string toEmails, string additionalEmailInitiator, string additionalEmailNonInitiator, string rejectionMemos = null)
    {
      //SCP210204: IS-WEB Outage (added log)
      var stopWatch = new Stopwatch();
      stopWatch.Start();
      var billingCategory = string.Empty;

      switch (invoice.BillingCategoryId)
      {
        case 1:
          billingCategory = "Passenger";
          break;
        case 2:
          billingCategory = "Cargo";
          break;
        case 3:
          billingCategory = "Miscellaneous";
          break;
        case 4:
          billingCategory = "UATP";
          break;
      }
      
      //SCP281499 - CMP 527, Closure of Correspondence
      //For Scenario 3, we make changes to include scenario 3 for acceptance comments not delete directly.
      if (scenarioId <= 2)
      {
        var billingPeriod = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod).ToString("yyyy-MMM-dd");
        var billingMember = invoice.BillingMemberText;

        // CMP#657: Retention of Additional Email Addresses in Correspondences.
        var toEmailIds = GetToEmailIdList(initiatorMemberId, 0, string.Empty, string.Empty, string.Empty, closedByUserId, invoice.BillingCategoryId);

        SendCorrespondenceDeletedAlert(EmailTemplateId.CorrespondenceDeletedAlert,
                                       correspondenceNumber, billingCategory,
                                       rejectionMemos ?? string.Empty, invoice.InvoiceNumber, billingPeriod,
                                       billingMember, toEmailIds);
      }
      else
      {
        // CMP#657: Retention of Additional Email Addresses in Correspondences.
        var toEmailIds = GetToEmailIdList(initiatorMemberId, nonInitiatorMemberId, toEmails,
                                          additionalEmailInitiator, additionalEmailNonInitiator,
                                          closedByUserId, invoice.BillingCategoryId);

        SendCorrespondenceUpdatedToCloseAlert(EmailTemplateId.CorrespondenceUpatedToCloseAlert, correspondenceNumber,
                                              billingCategory, toEmailIds);
      }
      stopWatch.Stop();
      var log = ReferenceManager.GetDebugLog(DateTime.UtcNow, "SendCorrespondenceAlertOnClose", this.ToString(),
                                   BillingCategorys.Passenger.ToString(), "SendCorrespondenceAlertOnClose", 0, stopWatch.Elapsed.ToString());
      ReferenceManager.InsertLogDebugData(log);

    }


    /// <summary>
    /// Send alert on correspondence status to closed
    /// </summary>
    /// <param name="emailTemplateId">template id</param>
    /// <param name="correspondenceNumber">correspondence id</param>
    /// <param name="billingCategory">billing category</param>
    /// <param name="toEmailIds">to email ids</param>
    private void SendCorrespondenceUpdatedToCloseAlert(EmailTemplateId emailTemplateId, string correspondenceNumber, string billingCategory, string toEmailIds)
    {
      try
      {
        // CMP#657: Retention of Additional Email Addresses in Correspondences. Email sending code is moved to method SendEmailThroughEmailSender().

        // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a n-velocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        // Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        var emailSettingForIsAdminProcessFailedNotification = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

        // Generate email body text for email
        var context = new VelocityContext();
        context.Put("billingCategory", billingCategory);
        context.Put("correspondenceNumber", correspondenceNumber);
        context.Put("sisOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

        var emailBodyText = templatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
        // Create a mail object to send mail
        var mailMessage = new MailMessage
        {
          From = new MailAddress(emailSettingForIsAdminProcessFailedNotification.SingleOrDefault().FromEmailAddress),
          IsBodyHtml = true
        };

        // CMP#657: Retention of Additional Email Addresses in Correspondences. Email sending code is moved to method SendEmailThroughEmailSender().
      
        var subject = emailSettingForIsAdminProcessFailedNotification.SingleOrDefault().Subject;
        // Set subject of mail (replace special field placeholders with values)
        mailMessage.Subject = subject.Replace("$billingCategory", billingCategory).Replace("$correspondenceNumber", correspondenceNumber);

        // Set body text of mail
        mailMessage.Body = emailBodyText;

        // CMP#657: Retention of Additional Email Addresses in Correspondences. Email sending code is moved to method SendEmailThroughEmailSender().
        // Send the mail
        SendEmailThroughEmailSender(toEmailIds, mailMessage);

        return;
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
        return;
      }
    }

    /// <summary>
    /// Get list of to email ids.
    /// </summary>
    /// <param name="initiatorMemberId">form member id</param>
    /// <param name="nonInitiatorMemberId">to member id</param>
    /// <param name="toEmailId">to email ids</param>
    /// <param name="additionalEmailInitiator">TO_ADDITIONAL_EMAIL_IDS_OF_INITIATOR</param>
    /// <param name="additionalEmailNonInitiator">TO_ADDITIONAL_EMAIL_IDS_OF_NON_INITIATOR</param>
    /// <param name="closedByUserId">user id that closed correpondence</param>
    /// <param name="billingCategoryId">billing category id</param>
    /// <returns></returns>
    private string GetToEmailIdList(int initiatorMemberId, int nonInitiatorMemberId, string toEmailId, string additionalEmailInitiator, string additionalEmailNonInitiator, int closedByUserId, int billingCategoryId)
    {
        /*
        Email should be sent to:
        All email addresses defined in the ‘To E-Mail ID(s)’ section of the last undeleted correspondence stage
        And
        All email addresses defined in the ‘Additional E-Mail ID(s)’ portion of the last undeleted correspondence stage
        And 
        All correspondence contacts of the Billing Category to which the closed correspondence belongs (of the initiating Member)
        And
        All correspondence contacts of the Billing Category to which the closed correspondence belongs (of the non-initiating Member)
        And
        The user who closed the correspondence
        Any duplicate email IDs derived by the system as a result of combining the above email IDs should be eliminated 
       */
      var processingContactType = ProcessingContactType.PaxCorrespondence;

      switch (billingCategoryId)
      {
        case 1:
          processingContactType = ProcessingContactType.PaxCorrespondence;
          break;
        case 2:
          processingContactType = ProcessingContactType.CargoCorrespondence;
          break;
        case 3:
          processingContactType = ProcessingContactType.MiscCorrespondence;
          break;
        case 4:
          processingContactType = ProcessingContactType.UatpCorrespondence;
          break;
      }

      try
      {
        var emails = new List<string>();

        var initiatorContacts = MemberManager.GetContactsForContactType(initiatorMemberId, processingContactType);
        if (initiatorContacts != null && initiatorContacts.Count > 0)
        emails.AddRange(initiatorContacts.Select(cont => cont.EmailAddress));
        
        emails.Add(AuthManager.GetUserByUserID(closedByUserId).Email);

        if (nonInitiatorMemberId > 0)
        {
          var nonInitiatorContacts = MemberManager.GetContactsForContactType(nonInitiatorMemberId, processingContactType);
          if (nonInitiatorContacts!= null && nonInitiatorContacts.Count > 0)
          emails.AddRange(nonInitiatorContacts.Select(cont => cont.EmailAddress));

          /* Method Extracted For reuse in CMP#657 */
          return GetEmailIdsList(toEmailId, additionalEmailInitiator, additionalEmailNonInitiator, ",", emails);
        }

        return string.Join(",", emails.Distinct());
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
        return string.Empty;
      }
    }

    /// <summary>
    /// CMP 573
    /// User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert.
    /// </summary>
    /// <param name="billingcategoryId">billing category id.</param>
    /// <param name="correspondenceId">Correspondence id</param>
    /// <param name="loggedInUserId">CurrentlyLoggedInUser id</param>
    /// <param name="loggedInMemberId">CurrentlyLoggedInMember id</param>
    /// <param name="draftPermissionId">Draft Permission id</param>
    /// <returns>
    /// true: if correspondence is eligible for reply
    /// false: if correspondence is not eligible for reply
    /// </returns>
    public bool IsCorrespondenceEligibleForReply(int billingcategoryId, Guid correspondenceId, int loggedInUserId, int loggedInMemberId, int draftPermissionId)
    {
      return CorrespondenceRep.IsCorrespondenceEligibleForReply( billingcategoryId, correspondenceId, loggedInUserId, loggedInMemberId, draftPermissionId);
    }

    /// <summary>
    /// This function is used to send Email and Alert notification for correpondence. SCP247238 - Correspondence alerts
    /// CMP#616: New Contact Type for Correspondence Expiry Alerts
    /// </summary>
    /// <param name="expiredCorrespondence">expiredCorrespondence Object</param>
    /// <param name="processingCorrContactType">Pax/Cargo/Misc/Uatp</param>
    /// <param name="processingCorrExpiryContactType">Pax/Cargo/Misc/Uatp</param>
    public void SendCorrepondenceEmailNotificationAndAlert(ExpiredCorrespondence expiredCorrespondence, ProcessingContactType processingCorrContactType,
                                                                                                        ProcessingContactType processingCorrExpiryContactType)
    {
        Logger.InfoFormat("Sending Correpondence Email Notification And Alert to Correpondence No :{0}", expiredCorrespondence.CorrespondenceNumber);
        
        /* Logic used
         * 'Code'   'Corr Stage'    'Corr Status'   'Corr Sub Status'   'ATB'   CorrExpiry less 2 Days' 'CorrBMExpiry less 2 Days'  'Need to Pick'  'Scenario'
         *  OORN     Odd             Open            Responded           -      Yes                      -                           Yes             1
         *  OERN	 Odd	         Expired	     Responded	         -	    -	                     Yes	                     Yes             2
         *  EORN	 Even	         Open	         Responded	         -	    Yes	                     -	                         Yes             3
         *  EORY	 Even	         Open	         Responded	         Yes	Yes	                     -	                         Yes             4
         *  EORYB	 Even	         Open	         Responded	         Yes	-	                     Yes	                     Yes             5
         */
        
        var billingCategory = new BillingCategorys();
        switch (processingCorrContactType)
        {
            case ProcessingContactType.PaxCorrespondence:
                billingCategory = BillingCategorys.Passenger;
                break;
            case ProcessingContactType.CargoCorrespondence:
                billingCategory = BillingCategorys.Cargo;
                break;
            case ProcessingContactType.MiscCorrespondence:
                billingCategory = BillingCategorys.Miscellaneous;
                break;
            case ProcessingContactType.UatpCorrespondence:
                billingCategory = BillingCategorys.Uatp;
                break;
        }

        var toEmailAlertMemberId = -1;
        List<Contact> processingCorrContacts;
        List<Contact> processingCorrExpiryContacts;
        var toEMailAdresses = string.Empty;
        var message = string.Empty;
        var correspondenceNumber = expiredCorrespondence.CorrespondenceNumber.ToString().PadLeft(11, '0');
        var timeLimitDateForResponse = expiredCorrespondence.CorExpiryDate.HasValue ? expiredCorrespondence.CorExpiryDate.Value.Date.ToString("dd-MMM-yyyy") : string.Empty;
        var timeLimitDateForBm = expiredCorrespondence.BmExpiryDate.HasValue ? expiredCorrespondence.BmExpiryDate.Value.Date.ToString("MMM yyyy Pdd") : string.Empty;

        switch (expiredCorrespondence.AlertScenario)
        {
            case 1:
                toEmailAlertMemberId = expiredCorrespondence.ToMemberId;
                
                processingCorrContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.ToMemberId, processingCorrContactType);
                if (processingCorrContacts != null && processingCorrContacts.Count > 0)
                {
                    toEMailAdresses = string.Join(";", processingCorrContacts.Select(c => c.EmailAddress).ToArray());
                }

                // CMP#616: New Contact Type for Correspondence Expiry Alerts
                processingCorrExpiryContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.ToMemberId, processingCorrExpiryContactType);
                if (processingCorrExpiryContacts != null && processingCorrExpiryContacts.Count > 0)
                {
                    toEMailAdresses = toEMailAdresses + ";" + string.Join(";", processingCorrExpiryContacts.Select(c => c.EmailAddress).ToArray());
                }
            
                // CMP #657: Retention of Additional Email Addresses in Correspondences.
                // FRS Section: 2.4 Email Alerts on Expiry of Correspondences.))
                if (!string.IsNullOrEmpty(expiredCorrespondence.AdditionalEmailNonInitiator))
                {
                    toEMailAdresses = toEMailAdresses + ";" + expiredCorrespondence.AdditionalEmailNonInitiator.Replace(",", ";");
                }
                
                Logger.InfoFormat(" (Scenario=1) Email send for correspondence number {0} to Other Member Corr Contact:{1}", correspondenceNumber, toEMailAdresses);
                message = string.Format(AlertScenario1,billingCategory, correspondenceNumber, timeLimitDateForResponse);
                break;
            case 2:
                toEmailAlertMemberId = expiredCorrespondence.FromMemberId;
                
                processingCorrContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.FromMemberId, processingCorrContactType);
                if (processingCorrContacts != null && processingCorrContacts.Count > 0) 
                {
                    toEMailAdresses = string.Join(";", processingCorrContacts.Select(c => c.EmailAddress).ToArray());
                }

                // CMP#616: New Contact Type for Correspondence Expiry Alerts
                processingCorrExpiryContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.FromMemberId, processingCorrExpiryContactType);
                if (processingCorrExpiryContacts != null && processingCorrExpiryContacts.Count > 0)
                {
                    toEMailAdresses = toEMailAdresses + ";" + string.Join(";", processingCorrExpiryContacts.Select(c => c.EmailAddress).ToArray());
                }
                
                // CMP #657: Retention of Additional Email Addresses in Correspondences.
                // FRS Section: 2.4 Email Alerts on Expiry of Correspondences.
                if (!string.IsNullOrEmpty(expiredCorrespondence.AdditionalEmailInitiator))
                {
                    toEMailAdresses = toEMailAdresses + ";" + expiredCorrespondence.AdditionalEmailInitiator.Replace(",", ";");
                }
                
                Logger.InfoFormat(" (Scenario=2) Email send correspondence number {0} to Intiator Member Corr Contact:{1}", correspondenceNumber, toEMailAdresses);
                message = string.Format(AlertScenario2, billingCategory, correspondenceNumber, timeLimitDateForBm);
                break;
            case 3:
                toEmailAlertMemberId = expiredCorrespondence.ToMemberId;
                
                processingCorrContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.ToMemberId, processingCorrContactType);
                if (processingCorrContacts != null && processingCorrContacts.Count > 0) 
                {
                    toEMailAdresses = string.Join(";", processingCorrContacts.Select(c => c.EmailAddress).ToArray());
                }

                // CMP#616: New Contact Type for Correspondence Expiry Alerts
                processingCorrExpiryContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.ToMemberId, processingCorrExpiryContactType);
                if (processingCorrExpiryContacts != null && processingCorrExpiryContacts.Count > 0)
                {
                    toEMailAdresses = toEMailAdresses + ";" + string.Join(";", processingCorrExpiryContacts.Select(c => c.EmailAddress).ToArray());
                }

                // CMP #657: Retention of Additional Email Addresses in Correspondences.
                // FRS Section: 2.4 Email Alerts on Expiry of Correspondences.
                if (!string.IsNullOrEmpty(expiredCorrespondence.AdditionalEmailInitiator))
                {
                    toEMailAdresses = toEMailAdresses + ";" + expiredCorrespondence.AdditionalEmailInitiator.Replace(",", ";");
                }
                
                Logger.InfoFormat(" (Scenario=3) Email send correspondence number {0} to Intiator Member Corr Contact:{1}", correspondenceNumber, toEMailAdresses);
                message = string.Format(AlertScenario3, billingCategory, correspondenceNumber, timeLimitDateForResponse);
                break;
            case 4:
                toEmailAlertMemberId = expiredCorrespondence.ToMemberId;
                
                processingCorrContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.ToMemberId, processingCorrContactType);
                if (processingCorrContacts != null && processingCorrContacts.Count > 0) 
                {
                    toEMailAdresses = string.Join(";", processingCorrContacts.Select(c => c.EmailAddress).ToArray());
                }

                // CMP#616: New Contact Type for Correspondence Expiry Alerts
                processingCorrExpiryContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.ToMemberId, processingCorrExpiryContactType);
                if (processingCorrExpiryContacts != null && processingCorrExpiryContacts.Count > 0)
                {
                    toEMailAdresses = toEMailAdresses + ";" + string.Join(";", processingCorrExpiryContacts.Select(c => c.EmailAddress).ToArray());
                }

                // CMP #657: Retention of Additional Email Addresses in Correspondences.
                // FRS Section: 2.4 Email Alerts on Expiry of Correspondences.
                if (!string.IsNullOrEmpty(expiredCorrespondence.AdditionalEmailInitiator))
                {
                    toEMailAdresses = toEMailAdresses + ";" + expiredCorrespondence.AdditionalEmailInitiator.Replace(",", ";");
                }
                
                Logger.InfoFormat(" (Scenario=4) Email send correspondence number {0} to Intiator Member Corr Contact:{1}", correspondenceNumber, toEMailAdresses);
                message = string.Format(AlertScenario4, billingCategory, correspondenceNumber, timeLimitDateForResponse, timeLimitDateForBm);
                break;
            case 5:
                toEmailAlertMemberId = expiredCorrespondence.ToMemberId;
                
                processingCorrContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.ToMemberId, processingCorrContactType);
                if (processingCorrContacts != null && processingCorrContacts.Count > 0)
                {
                    toEMailAdresses = string.Join(";", processingCorrContacts.Select(c => c.EmailAddress).ToArray());
                }

                // CMP#616: New Contact Type for Correspondence Expiry Alerts
                processingCorrExpiryContacts = MemberManager.GetContactsForContactType(expiredCorrespondence.ToMemberId, processingCorrExpiryContactType);
                if (processingCorrExpiryContacts != null && processingCorrExpiryContacts.Count > 0)
                {
                    toEMailAdresses = toEMailAdresses + ";" + string.Join(";", processingCorrExpiryContacts.Select(c => c.EmailAddress).ToArray());
                }

                // CMP #657: Retention of Additional Email Addresses in Correspondences.
                // FRS Section: 2.4 Email Alerts on Expiry of Correspondences.
                if (!string.IsNullOrEmpty(expiredCorrespondence.AdditionalEmailInitiator))
                {
                    toEMailAdresses = toEMailAdresses + ";" + expiredCorrespondence.AdditionalEmailInitiator.Replace(",", ";");
                }
                
                Logger.InfoFormat(" (Scenario=5) Email send correspondence number {0} to Intiator Member Corr Contact:{1}", correspondenceNumber, toEMailAdresses);
                message = string.Format(AlertScenario5, billingCategory, correspondenceNumber, timeLimitDateForBm);
                break;
        }

        var context = new VelocityContext();
        _broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>();

        if (!string.IsNullOrEmpty(toEMailAdresses))
        {
            context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
            context.Put("BillingCategory", billingCategory);
            context.Put("CorrespondenceNumber", correspondenceNumber);
            context.Put("Message", message);
            
            #region "CMP #657: Retention of Additional Email Addresses in Correspondences."
            // CMP #657: Retention of Additional Email Addresses in Correspondences.
            // FRS Section: 2.4 Email Alerts on Expiry of Correspondences.
            
            // Get an instance of email settings  repository
            var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
            // Get eamil template.
            var emailTemplate = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.CorrespondeceEmailNotification);
            
            // Get an object of the TemplatedTextGenerator that is used to generate body text of email from a n-velocity template
            var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
            
            // Generate email body text for own profile updates contact type mail
            var emailToMemberPrimaryContactsText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.CorrespondeceEmailNotification, context);
            
            // Create a mail object to send mail
            var msgForEmailNotification = new MailMessage { From = new MailAddress(emailTemplate.SingleOrDefault().FromEmailAddress),
                                                                                   Subject = emailTemplate.SingleOrDefault().Subject.Replace("$BillingCategory$",
                                                                                   context.Get( "BillingCategory") .ToString()). Replace("$CorrespondenceNo$",
                                                                                   context.Get("CorrespondenceNumber").ToString()),
                                                                                   Body = emailToMemberPrimaryContactsText,
                                                                                   IsBodyHtml = true
                                                          };
            
            //Send email notification to email.
            SendEmailThroughEmailSender(toEMailAdresses, msgForEmailNotification);
            // _broadcastMessagesManager.SendEmailNotification(toEMailAdresses, EmailTemplateId.CorrespondeceEmailNotification, context);
            
            #endregion "CMP #657: Retention of Additional Email Addresses in Correspondences."
            
            Logger.InfoFormat("Correspondence Email pushed for eMail ids {0}, Correspondence Number {1}", toEMailAdresses, expiredCorrespondence.CorrespondenceNumber);

            _broadcastMessagesManager.AddCorrespondenceAlert(toEmailAlertMemberId, processingCorrContactType, message);
            Logger.InfoFormat("Added Correspondence alert to Processing Corr Contact Type for Member id {0}, Correspondence Number {1}", toEmailAlertMemberId, expiredCorrespondence.CorrespondenceNumber);

            _broadcastMessagesManager.AddCorrespondenceAlert(toEmailAlertMemberId, processingCorrExpiryContactType, message);
            Logger.InfoFormat("Added Correspondence alert to Processing Corr Expiry Contact Type for Member id {0}, Correspondence Number {1}", toEmailAlertMemberId, expiredCorrespondence.CorrespondenceNumber);
        }
        else
        {
            Logger.InfoFormat("No {0} Correspondence Alert Contacts found for Member id {1}, Correspondence Number {2}", billingCategory,expiredCorrespondence.ToMemberId, expiredCorrespondence.CorrespondenceNumber);
        }
    }

    // CMP#657: Retention of Additional Email Addressed in Correspondences.
    /// <summary>
    /// Method to send email alert on Sending of correspondence.
    /// </summary>
    /// <param name="billingCategory"></param>
    /// <param name="correspondPageUrl"></param>
    /// <param name="toEmailIds"></param>
    /// <param name="subject"></param>
    /// <param name="fromMemberCode"> From Member Code Alpha - To Member Code Numeric</param>
    /// <param name="toMemberCode"> To Member Code Alpha - To Member Code Numeric</param>
    /// <returns></returns>
    public bool EmailAlertsOnSendingOfCorrespondences(BillingCategoryType billingCategory, string correspondPageUrl, string toEmailIds, string subject, string fromMemberCode, string toMemberCode)
    {
      var context = new VelocityContext();
      context.Put("FromMemberCode", fromMemberCode);
      context.Put("ToMemberCode", toMemberCode);
      context.Put("CorrespondenceUrl", correspondPageUrl);
      context.Put("SisOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

      var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
      var messageBody = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.CorrespondenceResponse, context);

      var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof (IRepository<EmailTemplate>));
      var emailSettingForCorrespondence = emailSettingsRepository.Get(es => es.Id == (int) EmailTemplateId.CorrespondenceResponse);

      //var mailMessage = new MailMessage(emailSettingForCorrespondence.SingleOrDefault().FromEmailAddress, string.Empty, subject, messageBody) {IsBodyHtml = true};

      // Create a mail object to send mail
      var mailMessage = new MailMessage
      {
        From = new MailAddress(emailSettingForCorrespondence.SingleOrDefault().FromEmailAddress),
        Subject = subject,
        Body = messageBody,
        IsBodyHtml = true
      };
      
      try
      {
        SendEmailThroughEmailSender(toEmailIds, mailMessage);
      }
      catch (Exception exception)
      {
        Logger.ErrorFormat("Exception in CorrespondencesEmailAlertcommon [Billing Category: {0}] {1}", billingCategory, exception.StackTrace);
        
        if (billingCategory == BillingCategoryType.Pax)
        {
          throw new ISBusinessException(GetErrorDescription(ErrorCodes.FailedToSendMail, toEmailIds));
        }
        if (billingCategory == BillingCategoryType.Cgo)
        {
          throw new ISBusinessException(GetErrorDescription(CargoErrorCodes.FailedToSendMail, toEmailIds));
        }
        if (billingCategory == BillingCategoryType.Misc || billingCategory == BillingCategoryType.Uatp)
        {
          throw new ISBusinessException(GetErrorDescription(MiscErrorCodes.FailedToSendMail, toEmailIds));
        }
      }
      
      return true;
    }
    
    /// <summary>
    /// Method to get error description.
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="mailId"></param>
    /// <returns></returns>
    private static string GetErrorDescription(string errorCode, string mailId)
    {
      var errorDescription = Messages.ResourceManager.GetString(errorCode);

      // Replace place holders in error message with appropriate record names.
      if (!string.IsNullOrEmpty(errorDescription))
      {
        errorDescription = string.Format(errorDescription, mailId);
      }

      return errorDescription;
    }

    /// <summary>
    /// To send email to distinct email ids.
    /// </summary>
    /// <param name="toEmailIds"></param>
    /// <param name="mailMessage"></param>
    /// <returns></returns>
    public bool SendEmailThroughEmailSender(string toEmailIds, MailMessage mailMessage)
    {
      var eMailIds = toEmailIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

      var distinctEmailIds = new List<string>();

      foreach (var eMailId in eMailIds.Where(eMailId => !distinctEmailIds.Contains(eMailId.Trim())).Where(eMailId => !string.IsNullOrEmpty(eMailId.Trim())))
      {
        distinctEmailIds.Add(eMailId.Trim());
      }

      var emailSender = Ioc.Resolve<IEmailSender>();

      // To send to maximum 200 email ids per message.
      const int emailPerMsg = 200;
      for (var index = 0; index < distinctEmailIds.Count; index += emailPerMsg)
      {
        var emailIdLimited = distinctEmailIds.GetRange(index, ((distinctEmailIds.Count - index) > emailPerMsg)
                                                              ? emailPerMsg : (distinctEmailIds.Count - index)
                                                      ).Aggregate((mailOne, mailTwo) => mailOne + "," + mailTwo);
        mailMessage.To.Add(emailIdLimited);
        if (!string.IsNullOrEmpty(mailMessage.To.ToString()))
        {
          emailSender.Send(mailMessage);
        }
        mailMessage.To.Clear();
      }

      return true;
    }

    #region CMP#657

    public Member GetCachedCopyOfMemberUsingId(int memberId)
    {
        return MemberManager.GetMemberDetails(memberId);
    }

    public string GetEmailIdsList(string toEmailId, string additionalEmailInitiator, string additionalEmailNonInitiator, string joinUsing = ";",
                             List<string> emails = null)
    {
        if(emails == null)
            emails = new List<string>();

        if (!string.IsNullOrEmpty(toEmailId))
            emails.AddRange(toEmailId.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        // CMP#657: Retention of Additional Email Addresses in Correspondences.
        if (!string.IsNullOrEmpty(additionalEmailInitiator))
            emails.AddRange(additionalEmailInitiator.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        if (!string.IsNullOrEmpty(additionalEmailNonInitiator))
            emails.AddRange(additionalEmailNonInitiator.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList());

        return string.Join(joinUsing, emails.Distinct());
    }

    #endregion

  }
}

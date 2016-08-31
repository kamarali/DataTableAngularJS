using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Business.Common
{
  public interface ICorrespondenceManager
  {
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
    int[] CanCorrespondenceClose(int billingcategoryId, string correspondenceId);

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
    bool CloseCorrespondence(string correspondenceNo, string correspondenceStage, string correspondenceStatus, string correspondenceSubStatus, int scenarioNo, int billingCategoryId,
                                    string acceptanceComment, int acceptanceUserId, DateTime acceptanceDateTime,
                                    ref string message);


    //SCENARIO 1: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = SAVED
    //SCENARIO 2: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
    //SCENARIO 3: STAGE = 1,   STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED
    //SCENARIO 4: STAGE = >=2, STATUS = OPEN     AND  SUB-STATUS = RECEVIED/RESPONDED
    //SCENARIO 5: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = SAVED
    //SCENARIO 6: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
    //SCENARIO 7: STAGE = >2,  STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED 
    /// <summary>
    /// Method to send cargo correspondence alert
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
    /// CMP#657: Retention of Additional Email Addresses in Correspondences.
    void SendCorrespondenceAlertOnClose(InvoiceBase invoice, string correspondenceNumber,
                                        int initiatorMemberId, int nonInitiatorMemberId, int scenarioId,
                                        int closedByUserId, string toEmails, string additionalEmailInitiator, string additionalEmailNonInitiator, string rejectionMemos = null);

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
    bool IsCorrespondenceEligibleForReply(int billingcategoryId, Guid correspondenceId, int loggedInUserId, int loggedInMemberId, int draftPermissionId);

    /// <summary>
    /// This function is used to send email and Alert notification for correpondence.
    /// CMP#616: New Contact Type for Correspondence Expiry Alerts
    /// </summary>
    /// <param name="expiredCorrespondence"></param>
    /// <param name="processingCorrContactType"></param>
    /// <param name="processingCorrExpiryContactType"></param>
    void SendCorrepondenceEmailNotificationAndAlert(ExpiredCorrespondence expiredCorrespondence, ProcessingContactType processingCorrContactType, ProcessingContactType processingCorrExpiryContactType);
    
    // CMP#657: Retention of Additional Email Addressed in Correspondences.
    /// <summary>
    /// Method to send Email Alert on Sending of Correspondences.
    /// </summary>
    /// <param name="billingCategory"></param>
    /// <param name="correspondPageUrl"></param>
    /// <param name="toEmailIds"></param>
    /// <param name="subject"></param>
    /// <param name="fromMemberCode"> From Member Code Alpha - To Member Code Numeric</param>
    /// <param name="toMemberCode"> To Member Code Alpha - To Member Code Numeric</param>
    /// <returns></returns>
    bool EmailAlertsOnSendingOfCorrespondences(BillingCategoryType billingCategory, string correspondPageUrl, string toEmailIds, string subject, string fromMemberCode, string toMemberCode);

    /// <summary>
    /// CMP#657: Retention of Additional Email Addresses in Correspondences. Common method to Send Correspondence Emails.
    /// </summary>
    /// <param name="toEmailIds"></param>
    /// <param name="mailMessage"></param>
    /// <returns></returns>
    bool SendEmailThroughEmailSender(string toEmailIds, MailMessage mailMessage);

    /// <summary>
    /// CMP#657: Retention of Additional Email Addresses in Correspondences. Common method to Get Cached Copy of Member Using Member Id for rendering UI.
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Member GetCachedCopyOfMemberUsingId(int memberId);

      /// <summary>
      /// CMP#657: Retention of Additional Email Addresses in Correspondences. Common method to Club all email Ids and, get distinct once out of it.
      /// </summary>
      /// <param name="toEmailId"></param>
      /// <param name="additionalEmailInitiator"></param>
      /// <param name="additionalEmailNonInitiator"></param>
      /// <param name="emails"></param>
      /// <returns></returns>
      string GetEmailIdsList(string toEmailId, string additionalEmailInitiator, string additionalEmailNonInitiator, string joinUsing = ";",
                             List<string> emails = null);

  }
}

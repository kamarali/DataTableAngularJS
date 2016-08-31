using System;
using System.Collections.Generic;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using NVelocity;

namespace Iata.IS.Business.BroadcastMessages
{
  public interface IBroadcastMessagesManager
  {
    //Add announcements
    ISMessagesAlerts AddAnnouncements(ISMessagesAlerts announcement);

    //Add new messages
    ISMessageRecipients AddMessages(ISMessageRecipients message);

    //Add new alerts
    ISMessageRecipients AddAlerts(ISMessageRecipients alert);

    /// <summary>
    /// Adds the alert for Is Admin.
    /// </summary>
    /// <param name="alert">The alert object to be added.</param>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <param name="context">The context object populated with placeholder values.</param>
    /// <returns>
    /// Added IS SIS Ops Alert object
    /// </returns>
    ISSISOpsAlert AddAlert(ISSISOpsAlert alert, EmailTemplateId emailTemplateId, VelocityContext context);

    /// <summary>
    /// Sends the output available alert.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="toEmailAddresses">To email addresses.</param>
    /// <param name="period">The period.</param>
    /// <param name="currentBillingPeriodFormatted">The current billing period formatted.</param>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <returns></returns>
    bool SendOutputAvailableAlert(int memberId, string[] toEmailAddresses, string period, string currentBillingPeriodFormatted,EmailTemplateId emailTemplateId);

    /// <summary>
    /// Sends the output file available notification.
    /// </summary>
    /// <param name="toEmailAddress">To email addresses.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="emailTemplateId">The email template id.</param>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    bool SendOutputFileAvailableNotification(string toEmailAddress, string filePath, EmailTemplateId emailTemplateId, VelocityContext context);

    bool AddCorrespondenceAlert(int memberId, ProcessingContactType contactType, string message);

    /// <summary>
    /// Alert for generation of IDEC\XML files
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="contactType"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    bool AddPayableInvoicesAvailableAlert(int memberId, ProcessingContactType contactType, string message);

    IList<AlertsMessagesAnnouncementsResultSet> GetSisOpsAlerts(DateTime? ThresholdDate);

    bool ClearSisOpsAlertMessage(Guid id);
    
    // SCP110222: Missing data from Mar P4 IS-IDEC
    void SendISAdminExceptionNotification(EmailTemplateId emailTemplateId, string serviceName, Exception exception, bool emailToSisOps = true);

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    bool SendEmailNotification(string toEmailAddress, EmailTemplateId emailTemplateId, VelocityContext context);

      #region CMP#608: Load Member Profile - CSV Option

      void SendMemberProfileCsvUploadEmailNotification(int level, string fileName, string submissionDateTime,
                                                       string userName, string reasonForFailure, string failedRowNumber,
                                                       string failedField, string failedFieldValue);

      #endregion
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.Common
{
  public interface IDailyOutputProcessManager
  {
    /// <summary>
    /// Send Completion Alert
    /// </summary>
    /// <param name="targetDate"></param>
    void SendCompletionAlert(DateTime targetDate);

    /// <summary>
    /// Set Daily Output Staus For Member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="retryCount"></param>
    /// <param name="invoiceCount">CMP622: total invoice count for non location specific</param>
    /// <param name="outputProcessStatus"></param>
    /// <param name="isXmlStatus"></param>
    void SetDailyOutputStausForMember(int memberId, DateTime targetDate, int retryCount, int invoiceCount, 
                                      OutputProcessStatus outputProcessStatus, bool isXmlStatus);

    /// <summary>
    /// Set Daily Output Staus For Member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="retryCount"></param>
    /// <param name="invoiceCount">total invoice count for non location specific</param>
    /// <param name="locationId">location id</param>
    /// <param name="outputProcessStatus"></param>
    /// <param name="isXmlStatus"></param>
    void SetDailyLocSpecOutputStausForMember(int memberId, DateTime targetDate, int retryCount, int invoiceCount, string locationId,
                                      OutputProcessStatus outputProcessStatus, bool isXmlStatus);

    /// <summary>
    /// Send Sis Admin Daily output Generation Failure Notification
    /// </summary>
    /// <param name="member"></param>
    /// <param name="targetDate"></param>
    /// <param name="processName"></param>
    /// <param name="exception"></param>
    void SendSisAdminDailyOutputGenerationFailureNotification(Member member, DateTime targetDate, string processName,
                                                              Exception exception);

    /// <summary>
    /// Send Sis Admin Daily Output Pending Invoices Notification
    /// </summary>
    /// <param name="targetDate"></param>
    /// <param name="pendingInvoices"></param>
    void SendSisAdminDailyOutputPendingInvoiceNotification(DateTime targetDate,
                                                         List<InvoicePendingForDailyDelivery> pendingInvoices);

    /// <summary>
    /// Set Daily Strat time for Member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="isXmlStartTime"></param>
    void SetDailyOutputStartTimeForMember(int memberId, DateTime targetDate, bool isXmlStartTime);

    /// <summary>
    /// CMP 622 : Set Daily Strat time for Member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="locationId">location id</param>
    /// <param name="isXmlStartTime"></param>
    void SetDailyLocSpecOutputStartTimeForMember(int memberId, DateTime targetDate, string locationId, bool isXmlStartTime);
  }
}

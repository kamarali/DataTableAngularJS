using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using NVelocity;
using System.Globalization;
using Iata.IS.Model.Enums;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Data.Impl;
using log4net;
using System.Reflection;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.Common.Impl
{
  public class DailyOutputProcessManager : IDailyOutputProcessManager
  {
    private readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Send Completion Alert
    /// </summary>
    /// <param name="targetDate"></param>
    public void SendCompletionAlert(DateTime targetDate)
    {
      var isDailyOutputProcessLogRep =
          Ioc.Resolve<IRepository<IsDailyOutputProcessLog>>(typeof(IRepository<IsDailyOutputProcessLog>));

      Logger.Info("Check if All Daily IS-XML & OAR generation is complete.");

      //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
      const int dailyOutputPendingStatus = (int)OutputProcessStatus.Pending;

      long incompleteCount = isDailyOutputProcessLogRep.GetCount(
          l =>
          l.TargetDate == targetDate &&
          (l.DailyIsXmlGenerationStatus == dailyOutputPendingStatus || l.DailyOarGenerationStatus == dailyOutputPendingStatus));

      Logger.InfoFormat("No. of Daily IS-XML or OAR generation pending: {0}.",incompleteCount);

      if (incompleteCount == 0)
      {
        Logger.Info("Sending Daily Output Generation Process Completion mail to SIS Ops.");

          //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
          const int dailyOutputSuccessfulStatus = (int) OutputProcessStatus.Successful;
        
          long xmlGeneratedCount =
              isDailyOutputProcessLogRep.GetCount(
                  l =>
                  l.TargetDate == targetDate && l.DailyIsXmlGenerationStatus == dailyOutputSuccessfulStatus &&
                  l.LocationId == null);

          long oarGeneratedCount =
              isDailyOutputProcessLogRep.GetCount(
                  l =>
                  l.TargetDate == targetDate && l.DailyOarGenerationStatus == dailyOutputSuccessfulStatus &&
                  l.LocationId == null);

          long xmlLocSpecGeneratedCount =
              isDailyOutputProcessLogRep.GetCount(
                  l =>
                  l.TargetDate == targetDate && l.DailyIsXmlGenerationStatus == dailyOutputSuccessfulStatus &&
                  l.LocationId != null);

          long oarLocSpecGeneratedCount =
              isDailyOutputProcessLogRep.GetCount(
                  l =>
                  l.TargetDate == targetDate && l.DailyOarGenerationStatus == dailyOutputSuccessfulStatus && 
                  l.LocationId != null);

        VelocityContext context = new VelocityContext();
        context.Put("TargetDate", targetDate.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo));
        context.Put("XmlCount", xmlGeneratedCount);
        context.Put("OarCount", oarGeneratedCount);
        context.Put("XmlLocSpecCount", xmlLocSpecGeneratedCount);
        context.Put("OarLocSpecCount", oarLocSpecGeneratedCount);

        IBroadcastMessagesManager broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>();

        broadcastMessagesManager.SendEmailNotification(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                                                       EmailTemplateId.
                                                           SISAdminDailyOutputProcessCompletionNotification,
                                                       context);

        Logger.Info("Completed sending Daily Output Generation Process Completion mail to SIS Ops.");

      }
    }


    /// <summary>
    /// Set Daily Output Staus For Member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="retryCount"></param>
    /// <param name="invoiceCount">total invoice count for non location specific</param>
    /// <param name="outputProcessStatus"></param>
    /// <param name="isXmlStatus">true if set Xml status, false if set Oar status</param>
    public void SetDailyOutputStausForMember(int memberId, DateTime targetDate, int retryCount,int invoiceCount, OutputProcessStatus outputProcessStatus, bool isXmlStatus)
    {
      var isDailyOutputProcessLogRep =
        Ioc.Resolve<IRepository<IsDailyOutputProcessLog>>(typeof(IRepository<IsDailyOutputProcessLog>));
      if (isDailyOutputProcessLogRep != null)
      {
        var isDailyOutputProcessLog = isDailyOutputProcessLogRep.Get(op => op.MemberId == memberId && op.TargetDate == targetDate && op.LocationId == null).FirstOrDefault();

        if (isDailyOutputProcessLog != null)
        {
          // Set offlineCollectionsStatus of invoice to Success and also set the offline collection datetime.
          if (isXmlStatus)
          {
            //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery
            isDailyOutputProcessLog.IsXmlInvoiceCount = invoiceCount;
            isDailyOutputProcessLog.DailyIsXmlGenerationStatus = (int)outputProcessStatus;
            isDailyOutputProcessLog.DailyIsXmlGenerationRetryCount = retryCount;
            isDailyOutputProcessLog.DailyIsXmlGenerationEndDate = DateTime.UtcNow;
          }
          else
          {
            //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery
            isDailyOutputProcessLog.OARInvoiceCount = invoiceCount;
            isDailyOutputProcessLog.DailyOarGenerationStatus = (int)outputProcessStatus;
            isDailyOutputProcessLog.DailyOarGenerationRetryCount = retryCount;
            isDailyOutputProcessLog.DailyOarGenerationEndDate = DateTime.UtcNow;
          }
          // Save Changes done.
          UnitOfWork.CommitDefault();
        }
      }
    }


    /// <summary>
    /// Set Daily Output Staus For Member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="retryCount"></param>
    /// <param name="invoiceCount">total invoice count for non location specific</param>
    /// <param name="locationId">location id for location spec invoices</param>
    /// <param name="outputProcessStatus"></param>
    /// <param name="isXmlStatus"></param>
    public void SetDailyLocSpecOutputStausForMember(int memberId, DateTime targetDate, int retryCount, int invoiceCount, string locationId,
                                      OutputProcessStatus outputProcessStatus, bool isXmlStatus)
    {
      var isDailyOutputProcessLogRep =
        Ioc.Resolve<IRepository<IsDailyOutputProcessLog>>(typeof(IRepository<IsDailyOutputProcessLog>));
      if (isDailyOutputProcessLogRep != null)
      {
        var isDailyOutputProcessLog = isDailyOutputProcessLogRep.Get(op => op.MemberId == memberId && op.TargetDate == targetDate && op.LocationId == locationId).FirstOrDefault();

        if (isDailyOutputProcessLog != null)
        {
          // Set offlineCollectionsStatus of invoice to Success and also set the offline collection datetime.
          if (isXmlStatus)
          {
            //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery
            isDailyOutputProcessLog.IsXmlInvoiceCount = invoiceCount;
            isDailyOutputProcessLog.DailyIsXmlGenerationStatus = (int)outputProcessStatus;
            isDailyOutputProcessLog.DailyIsXmlGenerationRetryCount = retryCount;
            isDailyOutputProcessLog.DailyIsXmlGenerationEndDate = DateTime.UtcNow;
          }
          else
          {
            //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery
            isDailyOutputProcessLog.OARInvoiceCount = invoiceCount;
            isDailyOutputProcessLog.DailyOarGenerationStatus = (int)outputProcessStatus;
            isDailyOutputProcessLog.DailyOarGenerationRetryCount = retryCount;
            isDailyOutputProcessLog.DailyOarGenerationEndDate = DateTime.UtcNow;
          }
          // Save Changes done.
          UnitOfWork.CommitDefault();
        }
      }
    }


    /// <summary>
    /// Set Daily Strat time for Member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="isXmlStartTime"></param>
    public void SetDailyOutputStartTimeForMember(int memberId, DateTime targetDate, bool isXmlStartTime)
    {
        var isDailyOutputProcessLogRep =
          Ioc.Resolve<IRepository<IsDailyOutputProcessLog>>(typeof(IRepository<IsDailyOutputProcessLog>));
        if (isDailyOutputProcessLogRep != null)
        {
            var isDailyOutputProcessLog = isDailyOutputProcessLogRep.Get(op => op.MemberId == memberId && op.TargetDate == targetDate && op.LocationId == null).FirstOrDefault();
       
          if (isDailyOutputProcessLog != null)
            {
                // Set offlineCollectionsStatus of invoice to Success and also set the offline collection datetime.
                if (isXmlStartTime)
                    isDailyOutputProcessLog.DailyIsXmlGenerationStartDate = DateTime.UtcNow;
                else
                    isDailyOutputProcessLog.DailyOarGenerationStartDate = DateTime.UtcNow;
                // Save Changes done.
                UnitOfWork.CommitDefault();
            }
        }
    }

    /// <summary>
    /// Set Daily Strat time for Member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="isXmlStartTime"></param>
    /// <param name="locationId">location id of location specific invoices.</param>
    public void SetDailyLocSpecOutputStartTimeForMember(int memberId, DateTime targetDate,string locationId, bool isXmlStartTime)
    {
      var isDailyOutputProcessLogRep =
        Ioc.Resolve<IRepository<IsDailyOutputProcessLog>>(typeof(IRepository<IsDailyOutputProcessLog>));
      if (isDailyOutputProcessLogRep != null)
      {
        var isDailyOutputProcessLog = isDailyOutputProcessLogRep.Get(op => op.MemberId == memberId && op.TargetDate == targetDate && op.LocationId == locationId).FirstOrDefault();

        if (isDailyOutputProcessLog != null)
        {
          // Set offlineCollectionsStatus of invoice to Success and also set the offline collection datetime.
          if (isXmlStartTime)
            isDailyOutputProcessLog.DailyIsXmlGenerationStartDate = DateTime.UtcNow;
          else
            isDailyOutputProcessLog.DailyOarGenerationStartDate = DateTime.UtcNow;
          // Save Changes done.
          UnitOfWork.CommitDefault();
        }
      }
    }

    /// <summary>
    /// Send Sis Admin Daily output Generation Failure Notification
    /// </summary>
    /// <param name="member"></param>
    /// <param name="targetDate"></param>
    /// <param name="processName"></param>
    /// <param name="exception"></param>
    public void SendSisAdminDailyOutputGenerationFailureNotification(Member member, DateTime targetDate, string processName, Exception exception)
    {
      try
      {
        VelocityContext context = new VelocityContext();
        context.Put("ProcessName", processName);
        context.Put("TargetDate", targetDate.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo));
        context.Put("Member", String.Format("{0}-{1}", member.MemberCodeAlpha, member.MemberCodeNumeric));
        context.Put("ErrorMessage", exception.Message);

        IBroadcastMessagesManager broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>();
        broadcastMessagesManager.SendEmailNotification(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                                                       EmailTemplateId.SISAdminDailyOutputProcessFailureNotification,
                                                       context);

      }
      catch (Exception ex)
      {
        Logger.Error(ex);
        throw ex;
      }
    }

    /// <summary>
    /// Send Sis Admin Daily Output Pending Invoices Notification
    /// </summary>
    /// <param name="targetDate"></param>
    /// <param name="pendingInvoices"></param>
    public void SendSisAdminDailyOutputPendingInvoiceNotification(DateTime targetDate, List<InvoicePendingForDailyDelivery> pendingInvoices)
    {
      try
      {
        VelocityContext context = new VelocityContext();
        context.Put("TargetDate", targetDate.ToString("dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo));
        context.Put("pendingInvoiceList", pendingInvoices);

        IBroadcastMessagesManager broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>();
        broadcastMessagesManager.SendEmailNotification(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                                                       EmailTemplateId.SISAdminDailyOutputProcessPendingInvoicesNotification,
                                                       context);

      }
      catch (Exception ex)
      {
        Logger.Error(ex);
        throw ex;
      }
    }

      /*
    /// <summary>
    /// Create Daily Ouput log Entry
    /// </summary>
    /// <param name="memberId">billed member id</param>
    /// <param name="targetDate">dilvery date</param>
    /// <param name="invoiceCount">invoice count</param>
    /// <param name="nilFileRequired">nil file required</param>
    /// <param name="locationId">location id</param>
    public Guid GetDailyLocSpecOutputEntryIdForMember(int memberId, DateTime targetDate, int invoiceCount, bool nilFileRequired, string locationId)
    {
      var isDailyOutputProcessLogRep =
        Ioc.Resolve<IRepository<IsDailyOutputProcessLog>>(typeof (IRepository<IsDailyOutputProcessLog>));

      if (isDailyOutputProcessLogRep != null)
      {
        var isDailyOutputProcessLog = isDailyOutputProcessLogRep.Get(
          op => op.TargetDate == targetDate && op.MemberId == memberId && op.LocationId == locationId).FirstOrDefault();
     
        if (isDailyOutputProcessLog != null)
        {
          return isDailyOutputProcessLog.Id;
        }
        else
        {
          return Guid.Empty;
        }
      }
      else
      {
        return Guid.Empty;
      }

    }*/
  }
}

using System;

namespace Iata.IS.Core.Configuration
{
  public class CalendarParameters
  {
    public string ManualControlOnLateSubmissionsACH = "NO";
    public string ManualControlOnLateSubmissionsICH = "NO";
    public string YmqTimeZoneName = "Eastern Standard Time";

    public TimeSpan SubmissionsOpenOffset
    {
      get
      {
        return new TimeSpan(1, 0, 0, 0);
      }
    }

    public TimeSpan FutureDatedSubmissionsOpenOffset
    {
      get
      {
        return new TimeSpan(25, 15, 0, 0);
      }
    }

    public TimeSpan SupportingAttachmentLinkingDeadlineOffset
    {
      get
      {
        return new TimeSpan(0, 23, 59, 0);
      }
    }

    public int BillingOutputGenerationHoursOffset
    {
      get
      {
        return 23;
      }
    }

    /// <summary>
    /// This is applicable only for the 4th period of each month.
    /// </summary>
    public TimeSpan OldIdecSubmissionDeadlineOffset
    {
      get
      {
        return new TimeSpan(5, 23, 59, 0);
      }
    }

    /// <summary>
    /// This is applicable only for the 4th period of each month
    /// </summary>
    public TimeSpan OldIdecOutputGenerationOffset
    {
      get
      {
        return new TimeSpan(25, 12, 00, 0);
      }
    }

    public TimeSpan ClosureOfLateSubmissionsACHOffset
    {
      get
      {
        return new TimeSpan(0, 3, 0, 0);
      }
    }

    // Trigger parameters.
    public TimeSpan ACHMissingSubmissionsAlert1Offset
    {
      get
      {
        return new TimeSpan(0, -2, 0, 0);
      }
    }

    public TimeSpan FinalizeInvoiceStatusToProcessingCompleteOffset
    {
      get
      {
        return new TimeSpan(0, 4, 0, 0);
      }
    }

    public TimeSpan GenerateSISRechargeDataOffset
    {
      get
      {
        return new TimeSpan(3, 0, 0, 0);
      }
    }

    public TimeSpan GenerateNilFormCOffset
    {
      get
      {
        return new TimeSpan(0, 0, 5, 0);
      }
    }

    public TimeSpan OutputFileGenerationOffset
    {
      get
      {
        return new TimeSpan(0, -2, 0, 0);
      }
    }

    public TimeSpan SuppDocFinalizationOffset
    {
      get
      {
        return new TimeSpan(0, 0, 1, 0);
      }
    }

    public TimeSpan InvoiceOfflineCollectionGenerationOffset
    {
      get
      {
        return new TimeSpan(0, -1, 0, 0);
      }
    }

    public TimeSpan ValueConfirmationOffset
    {
      get
      {
        return new TimeSpan(0, 48, 0, 0);
      }
    }

    public TimeSpan SettlementWithAchOffset
    {
      get
      {
        return new TimeSpan(0, 1, 0, 0);
      }
    }

    public TimeSpan SubmissionDeadlineAlertsForUnclosedIchInvoicesOffset
    {
      get
      {
        return new TimeSpan(0, -36, 0, 0);
      }
    }

    public TimeSpan PendingAttachmentAlertForPaxFutureSubmissionsIchInvoicesOffset
    {
      get
      {
        return new TimeSpan(0, -60, 0, 0);
      }
    }

    public TimeSpan GenerationAndDeliveryOfLegalInvoiceDataForIataOffset
    {
      get
      {
        return new TimeSpan(0, 0, 1, 0);
      }
    }

    public TimeSpan DowngradeOnceMonthlyToOldidecAndSendToAtpOffset
    {
      get
      {
        return new TimeSpan(0, 0, 1, 0);
      }
    }

    public TimeSpan MissingSubmissionDeadlineNotificationToMembers1Offset
    {
      get
      {
        return new TimeSpan(0, 10, 30, 0);
      }
    }

    public TimeSpan MissingSubmissionDeadlineNotificationToMembers2Offset
    {
      get
      {
        return new TimeSpan(0, 12, 00, 0);
      }
    }

    public TimeSpan AutoOpeningOfICHLateSubmissionWindowOffset
    {
      get
      {
        return new TimeSpan(0, 0, 1, 0);
      }
    }

    public TimeSpan AutoOpeningOfACHLateSubmissionWindowOffset
    {
      get
      {
        return new TimeSpan(0, 0, 1, 0);
      }
    }

    public TimeSpan AutoClosingOfICHLateSubmissionWindowOffset
    {
      get
      {
        return new TimeSpan(0, 0, 1, 0);
      }
    }

    public TimeSpan AutoClosingOfACHLateSubmissionWindowOffset
    {
      get
      {
        return new TimeSpan(0, 15, 0, 0);
      }
    }
  }
}

//public TimeSpan FinalizationOfSupportingDocsForAllInvoicesOffset
//{
//  get
//  {
//    return new TimeSpan(0, 0, 1, 0);
//  }
//}
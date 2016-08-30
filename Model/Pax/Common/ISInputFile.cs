using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Pax.Common
{
  /// <summary>
  /// This class contains IS Input file related information
  /// </summary>
  public class IsInputFile : EntityBase<Guid>
  {
    public string FileName { get; set; }

    public DateTime FileDate { get; set; }

    public DateTime ReceivedDate { get; set; }

    public int? BillingCategory { get; set; }


    public FileFormatType FileFormat
    {
      get
      {
        return (FileFormatType)FileFormatId;
      }
      set
      {
        FileFormatId = Convert.ToInt32(value);
      }
    }
    public int FileFormatId { set; get; }

    /// <summary>
    /// This represents validation process status of input file data.
    /// </summary>
    public FileStatusType FileStatus
    {
      get
      {
        return (FileStatusType)FileStatusId;
      }
      set
      {
        FileStatusId = Convert.ToInt32(value);
      }
    }
    public int FileStatusId { get; set; }

    public string UploadedBy { get; set; }

    public List<PaxInvoice> Invoices { get; set; }

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public string SenderReceiverIP { get; set; }

    public int? SenderReceiver { get; set; }

    public string FileLocation { get; set; }

    public bool IsIncoming { get; set; }

    public string FileVersion { get; set; }

    public int OutputFileDeliveryMethodId { get; set; }

    public int BillingYear { get; set; }

    public int SenderRecieverType { get; set; }

    public Guid FileKey { get; set; }

    public int ExpectedResponseTime { get; set; }

    public DateTime FileProcessStartTime { get; set; }

    public DateTime FileProcessEndTime { get; set; }

    public int IsResponseRecieved { get; set; }

    public int BillingMonthFrom { get; set; }

    public int BillingMonthTo { get; set; }

    public int BillingPeriodFrom { get; set; }

    public int BillingPeriodTo { get; set; }

    public DateTime FileSubmissionFrom { get; set; }

    public DateTime FileSubmissionTo { get; set; }

    public bool IsPurged { get; set; }

    //CMP#622: MISC Outputs Split as per Location ID
    public string MiscLocationCode { get; set; }
  }
}

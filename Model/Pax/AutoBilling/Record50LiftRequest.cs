using System;
using Iata.IS.Core;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using System.ComponentModel;

namespace Iata.IS.Model.Pax.AutoBilling
{
  public class Record50LiftRequest : EntityBase<Guid>
  {
    public int ApplicationId { get; set; }

    public string SourceOfData { get; set; }

    public int BillingAirlineId { get; set; }

    public string Text { get; set; }

    public int RecordCount { get; set; }

    public string ErrorCode { get; set; }

    public int ErrorCount { get; set; }

    public DateTime TransmissionDate { get; set; }

    public int SequenceNumber { get; set; }

    [DisplayName("Document Number")]
    public long TicketDocumentNumber { get; set; }

    [DisplayName("Issuing Airline")]
    public string TicketIssuingAirline { get; set; }

    public int BillingMonth { get; set; }

    public int BillingYear { get; set; }

    public AutoBillingTransactionType TransactionType
    {
      get
      {
        return (AutoBillingTransactionType)TransactionTypeId;
      }
      set
      {
        TransactionTypeId = Convert.ToInt32(value);
      }
    }

    public int TransactionTypeId { get; set; }

    [DisplayName("Coupon Number")]
    public int CouponNumber { get; set; }

    public AutoBillingETicketIndicator ElectronicTicketIndicator
    {
      get
      {
        return (AutoBillingETicketIndicator)ElectronicTicketIndicatorId;
      }
      set
      {
        ElectronicTicketIndicatorId = Convert.ToInt32(value);
      }
    }

    public int ElectronicTicketIndicatorId { get; set; }

    public string FromAirportOfCoupon { get; set; }

    public string ToAirportOfCoupon { get; set; }

    public string AirlineFlightDesignator { get; set; }

    public int FlightNumber { get; set; }

    public DateTime FlightDate { get; set; }

    public string SettlementAuthorizationCode { get; set; }

    public AutoBillingRequestType RequestType
    {
      get
      {
        return (AutoBillingRequestType)RequestTypeId;
      }
      set
      {
        RequestTypeId = Convert.ToInt32(value);
      }
    }

    public int RequestTypeId { get; set; }

    public string CabinClass { get; set; }

    public int CouponStatusId { get; set; }

    public AutoBillingCouponStatus CouponStatusType
    {
      get
      {
        return (AutoBillingCouponStatus)CouponStatusId;
      }
      set
      {
        CouponStatusId = Convert.ToInt32(value);
      }
    }

    [DisplayName("Description of Irregularity")]
    public string CouponStatusDescription { get; set; }

    public string OriginalFileName { get; set; }

    public string NewFileName { get; set; }

    public DateTime ExpectedResponseDate { get; set; }

    [DisplayName("Prorate Results Filename")]
    public string ResponseFileName { get; set; }

    public DateTime ResponseDate { get; set; }

    public bool IncludedInIrregularityReport { get; set; }

    public bool? ResponseNotReceivedAsPerSLA { get; set; }

    public AutoBillingErrorType ErrorType
    {
      get
      {
        return (AutoBillingErrorType)ErrorTypeId;
      }
      set
      {
        ErrorTypeId = Convert.ToInt32(value);
      }
    }

    public int ErrorTypeId { get; set; }

    public Guid IsInputFileId { get; set; }

    public IsInputFile IsInputFile { get; set; }

    public string IsInputFileDisplayId
    {
      get
      {
        return IsInputFileId.Value();
      }
    }

    [DisplayName("Category")]
    public string Category
    {
      get
      {
        if (CouponStatusType == AutoBillingCouponStatus.MEM)
        {
          return "Auto-Billing Invoice" + " (" + CouponStatusType.ToString() + ")";
        }
        else if (RequestType == AutoBillingRequestType.AutoBill)
        {
          return "Auto-Billing Coupon" + " (" + CouponStatusType.ToString() + ")";
        }
        else
        {
          return "Value-Request" + " (" + CouponStatusType.ToString() + ")";
        }
      }
    }
  }
}

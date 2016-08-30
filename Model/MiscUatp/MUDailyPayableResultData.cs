using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MiscUatp
{
  public class MUDailyPayableResultData
  {
    public Guid Id
    {
      get;
      set;
    }

    public Int32 InvoiceTypeId
    {
      get;
      set;
    }

    public Int32 RejectionStage
    {
      get;
      set;
    }

    public Int32 SettlementMethodId
    {
      get;
      set;
    }

    public DateTime LastUpdatedOn
    {
      get;
      set;
    }

    public Int32 IsLegalPdfGenerated
    {
      get;
      set;
    }

    public Int32 DigitalSignatureStatusId
    {
      get;
      set;
    }

    public String DisplayBillingPeriod
    {
      get;
      set;
    }

    public DateTime TargetDate
    {
      get;
      set;
    }

    public String BillingMemberText
    {
      get;
      set;
    }

    public String InvoiceTypeDisplayText
    {
      get;
      set;
    }

    public String InvoiceNumber
    {
      get;
      set;
    }

    public String ChargeCategoryDisplayName
    {
      get;
      set;
    }

    public String SettlementMethodDisplayText
    {
      get;
      set;
    }

    public String ListingCurrencyDisplayText
    {
      get;
      set;
    }

    public Decimal BillingAmount
    {
      get;
      set;
    }

    //CMP #655: IS-WEB Display per Location ID
    public string BilledMemberLocation { get; set; }

  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.ValueConfirmation
{
  public class RequestVCFCoupon
  {
    public int BillingMemberId { get; set; }

    public string BillingMemberAccountingCode { get; set; }

    public int BilledMemberId { get; set; }

    public string BilledMemberAccountingCode { get; set; }

    public string BillingDate { get; set; }

    public string InvoiceNo { get; set; }

    public int PeriodNo { get; set; }

    public string InvoiceId { get; set; }

    public string CouponRecordId { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int BatchRecordSequenceNumber { get; set; }

    public int TicketCouponNumber { get; set; }

    public string TicketIssuingAirline { get; set; }

    public long TicketDocumentNo { get; set; }

    public int CheckDigit { get; set; }

    public string IDECStatusOfBilling { get; set; }

    public string IDECStatusOfBilled { get; set; }

    public int ListingCurrencyCodeNum { get; set;}

    public int BillingCurrencyCodeNum { get; set; }

    public decimal ExchangeRate { get; set;}

    public int SourceCode { get; set; }

    public decimal CouponGrossValue { get; set; }

    public decimal InterServChargePercent { get; set; }

    public decimal HandlingFeeAmount { get; set; }

    public decimal UATPPercent { get; set; }

    public decimal CouponTaxAmount { get; set; }

    public string ProrateMethodology { get; set; }

    public string AgreementIndicator { get; set; }

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.ExternalInterfaces.ICHInterface
{
  public class SISInvoiceValidationRequestTransmission
  {
    //CMP #624: ICH Rewrite-New SMI X


    public string BillingCategory { get; set; }

    public string InvoiceCreditNoteNumber { get; set; }

    public string SettlementMethod { get; set; }

    public string SubmissionMethod { get; set; }

    public string BillingYearMonthPeriod { get; set; }

    //OriginalBillingYearMonthPeriod changed to OriginalInvoiceGuid
    public string OriginalInvoiceSISUniqueInvoiceNumber { get; set; }

    public string InvoiceType { get; set; }

    public string BillingMemberDesignator { get; set; }

    public string BillingMemberCode { get; set; }

    public string BilledMemberDesignator { get; set; }

    public string BilledMemberCode { get; set; }

    public string InvoiceCurrency { get; set; }

    public string ClearanceCurrency { get; set; }

    /// <summary>
    /// CMP#648: Exchange rate changed to nullable
    /// </summary>
    public decimal? ExchangeRate { get; set; }

    public string CHAgreementIndicator { get; set; }

    public string CHDueDate { get; set; }
  }
}

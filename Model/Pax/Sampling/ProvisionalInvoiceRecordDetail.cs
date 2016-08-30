using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Pax.Sampling
{
  public class ProvisionalInvoiceRecordDetail : EntityBase<Guid>
  {
    public string InvoiceNumber { get; set; }

    public DateTime InvoiceDate { get; set; }

    public int BillingPeriodNo { get; set; }

    public Currency InvoiceListingCurrency { get; set; }

    public int InvoiceListingCurrencyId { get; set; }

    public decimal InvoiceListingAmount { get; set; }

    public decimal ListingToBillingRate { get; set; }

    public decimal InvoiceBillingAmount { get; set; }

    public PaxInvoice Invoice { get; set; }

    public Guid InvoiceId { get; set; }

    /// <summary>
    /// Adde property to display data in grid
    /// </summary>
    public string ListingCurrencyDisplayText
    {
        get
        {
            if (InvoiceListingCurrency != null)
                return InvoiceListingCurrency.Code;
            else
                return string.Empty;
        }
    }

  }
}

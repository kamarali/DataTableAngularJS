using Iata.IS.Model.Calendar;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.MiscUatp
{
  public struct RejectedInvoiceDetails
  {
    public BillingPeriod BillingPeriod { get; set; } 
    public int RejectionStage { get; set;}
    public string CurrentBilledIn { get; set; }
    public int CurrentBillingCurrencyCode { get; set; }
    public bool DisableBillingCurrency { get; set; }
    public string ErrorMessage { get; set; }
    public string AlertMessage { get; set; }
  }
}
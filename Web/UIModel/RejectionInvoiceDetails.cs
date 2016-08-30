namespace Iata.IS.Web.UIModel
{
  public class RejectionInvoiceDetails
  {
    public int BillingPeriod { get; set; }
    public int BillingMonth { get; set; }
    public int BillingYear { get; set; }
    public bool IsRejectionAllowed { get; set; }
    public int RejectionStage { get; set; }
    public int BilledInMonths { get; set; }
    public int CurrentBillingCurrencyId { get; set; }
    public string CurrentBilledIn { get; set; }
    public bool DisableBillingCurrency { get; set; }
    public string ErrorMessage { get; set; }
    public string AlertMessage { get; set; }
  }
}
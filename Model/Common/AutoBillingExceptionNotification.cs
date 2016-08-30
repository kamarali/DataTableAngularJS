namespace Iata.IS.Model.Common
{
  public class AutoBillingExceptionNotification
  {
    public string RecipientName { get; set; }

    public long ThreshouldLimitValue { get; set; }

    public long AvalableInvoiceCount { get; set; }
  }
}

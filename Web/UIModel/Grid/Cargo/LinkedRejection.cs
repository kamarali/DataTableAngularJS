namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class LinkedRejection
  {
    public string Id { get; set; }

    public string InvoiceId { get; set; }

    public string BillingMemberText { get; set; }

    public string DisplayBillingPeriod { get; set; }

    public string InvoiceNumber { get; set; }

    public string RejectionMemoNumber { get; set; }

    public int BillingCode { get; set; }
  }
}
using System;

namespace Iata.IS.Model.Common
{
  public class ExceptionSummarySearchResult
  {
    public Guid Id { get; set; }

    public string ExceptionCode { get; set; }

    public string ErrorDescription { get; set; }

    public string ChargeCategory { get; set; }

    public string MemberCode { get; set; }

    public string InvoiceNo { get; set; }

    public Guid InvoiceID { get; set; }

    public string FileName { get; set; }

    public bool IsBatchUpdateAllowed { get; set; }

    public string BatchUpdateAllowed
    {
      get { return IsBatchUpdateAllowed ? "Yes" : "No"; }
    }
    public int ErrorCount { get; set; }

    public int BillingMemberId { get; set; }

    public int BilledMemberId { get; set; }

    public string ProvInvoiceNo { get; set; }
  }
}

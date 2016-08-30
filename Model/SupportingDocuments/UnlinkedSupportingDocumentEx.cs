using System;

namespace Iata.IS.Model.SupportingDocuments
{
  public class UnlinkedSupportingDocumentEx : UnlinkedSupportingDocument
  {

    public string BilledMemberText { get; set; }

    public DateTime? SubmissionDate
    {
      get
      {
        return LastUpdatedOn;
      }
      set
      {
        if (value.HasValue) LastUpdatedOn = value.Value;
      }
    }

    public string BillingYearMonth
    {
      get
      {
        if (BillingMonth != 0 && BillingYear != 0)
        {
          return Convert.ToDateTime(BillingMonth + "-" + BillingYear).ToString("yyyy-MMM");
        }
        else
        {
          return string.Empty;
        }
      }
    }

  }
}

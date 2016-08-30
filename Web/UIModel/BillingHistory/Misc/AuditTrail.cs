using System.Collections.Generic;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Web.UIModel.BillingHistory.Misc
{
  public class AuditTrail
  {
    public MiscUatpInvoice OriginalInvoice { get; set; }

    public List<MiscUatpInvoice> RejectionInvoiceList { get; set; }

    public MiscUatpInvoice CorrespondenceInvoice { get; set; }

    public int InvoiceStageCount
    {
      get
      {
        return 2 + RejectionInvoiceList.Count;
      }
    }

    public AuditTrail()
    {
      RejectionInvoiceList = new List<MiscUatpInvoice>();
    }
  }
}
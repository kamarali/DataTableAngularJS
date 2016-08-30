using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MiscUatp.BillingHistory
{
  public class AuditTrailPdf
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

    public AuditTrailPdf()
    {
      RejectionInvoiceList = new List<MiscUatpInvoice>();
    }
  }
}

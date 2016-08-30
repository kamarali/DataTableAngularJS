using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports
{
  public class ProcessingDashInvoiceDeleteActionStatus
    {
        // Property to get and set Invoice
        public Guid InvoiceId { get; set; }

        // Property to get and set Billing Member Id
        public int BillingMemberId { get; set; }

        // Property to get and set MemberCode
        public string BillingMemberCode { get; set; }

        // Property to get and set MemberCommercialName
        public string BillingMemberName { get; set; }

        // Property to get and set InvoiceNumber
        public string InvoiceNo { get; set; }

        // Property to get and set  delete action status
        public string ActionStatus { get; set; }
      
    }
}

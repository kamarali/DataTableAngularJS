using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.SystemMonitor
{
  public class PendingInvoices
  {
    public Guid InvoiceId { get; set; }

    public int BillingYear { get; set; }

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public int BillingCategoryId { get; set; }

    public int BillingMemberId { get; set; }

    public int BilledMemberId { get; set; }

    public string InvoiceType { get; set;}

    public string BillingCategory { get; set; }

    public string InvoiceNumber { get; set; }

    public string BillingMember { get; set; }
     

    public string BilledMember { get; set; }
     

    public string BillingMemberAlphaNumericName
    {
      get
      {
        var strBillingMember = BillingMemberAlpha + "-" + BillingMemberNumeric + "-" + BillingMember;
        return strBillingMember;
      }
    }

    public string BilledMemberAlphaNumericName
    {
      get
      {
        var strBilledMember = BilledMemberAlpha + "-" + BilledMemberNumeric + "-" + BilledMember;
        return strBilledMember;
      }
    }

    public string InvoiceStatus { get; set; }

    public string BillingMemberAlpha { get; set; }

    public string BillingMemberNumeric { get; set; }

    public string BilledMemberAlpha { get; set; }

    public string BilledMemberNumeric { get; set; }

  }
}

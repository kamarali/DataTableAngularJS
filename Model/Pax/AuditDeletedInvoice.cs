using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax
{
    /// <summary>
    ///  CMP#400 : Audit Trail Report For Deleted Invoices
    /// Author : Vinod Patil
    /// Date : 11-09-2012
    /// </summary>
    public class AuditDeletedInvoice : MasterBase<Guid>
    {

      public int BillingCategoryId { get; set; }

      public string BillingCategory { get; set; }

      public int BillingMemberId { get; set; }

      public string BillingMember { get; set; }
        
      public int BilledMemberId { get; set; }

      public string BilledMember { get; set; }

      public int BillingYear { get; set; }

      public int BillingMonth { get; set; }

      public int BillingPeriod { get; set;}

      public string InvoiceNo { get; set; }

      public string FileName { get; set; }

      public int DeletedByUserId { get; set; }

      public string DeletedBy { get; set; }

      public DateTime DeteledOn { get; set; }

      public string DeletionDateFrom { get; set; }

      public string DeletionDateTo { get; set; }

    }
}

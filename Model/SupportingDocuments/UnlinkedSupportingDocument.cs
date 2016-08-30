using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Model.SupportingDocuments
{
  public class UnlinkedSupportingDocument : EntityBase<Guid>
  {
    public int BillingMemberId { get; set; }

    public int BilledMemberId { get; set; }

    public string InvoiceNumber { get; set; }

    public int BillingMonth { get; set; }

    public int BillingYear { get; set; }

    public int PeriodNumber { get; set; }

    public int BatchNumber { get; set; }

    public int SequenceNumber { get; set; }

    public int CouponBreakdownSerialNumber { get; set; }

    public string OriginalFileName { get; set; }

    public string FilePath { get; set; }

    public int ServerId { get; set; }

    public int InvoiceTypeId { get; set; }

    public int BillingCategoryId { get; set; }

    public FileServer FileServer { get; set; }

    public string IsFormC { get; set; }

    public string BilledMemberName { get; set; }

    public bool IsPurged { get; set; }

    //SCP 170146: Proposed improvement in Supporting Doc Linking Finalization Process
    public int LinkStatus { get; set; }

  }
}

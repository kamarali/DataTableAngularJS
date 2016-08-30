using Iata.IS.Model.Base;
using System.Collections.Generic;
using System;

namespace Iata.IS.Model.SupportingDocuments
{

  /// <summary>
  /// This entity is required to store the search criteria to find the Records for attaching supporting documents to it.
  /// </summary>
  public class RecordSearchCriteria : EntityBase<int>
  {

    public int BillingMemberId { set; get; }

    public string BillingMemberCode { set; get; }

    public int? ClearanceMonth { set; get; }

    public int? ClearancePeriod { set; get; }

    public int? BilledMemberId { set; get; }

    public string BilledMemberCode { set; get; }

    public int? BillingYear { set; get; }

    public int? BillingCategory { get; set; }

    public string InvoiceNumber { get; set; }

    //Batch Sequence Number
    public int? BatchNumber { get; set; }

    //Record Sequence Number
    public int? SequenceNumber { get; set; }

    public int? BreakdownSerialNumber { get; set; }

    public int? ChargeCategoryId { get; set; }

    public string FileName { get; set; }

    public string OriginalFileName { get; set; }

    public List<string> Files { get; set; }

    public Dictionary<string, string> OriginalFiles { get; set; }

    public bool IsFormC { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public bool IsFormD { get; set; }

    public string FormDInvoiceNumber { get; set; }

    public RecordSearchCriteria()
    {
      Files = new List<string>();
      OriginalFiles = new Dictionary<string, string>();
    }


    public int Mismatch { get; set; }

    //SCP255391: FW: Supporting Documents missing for SA Form
    public int FormDEBillingYear { get; set; }
    public int FormDEBillingMonth { get; set; }
    public int FormDEBillingPeriod { get; set; }
  }
}

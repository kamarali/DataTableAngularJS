using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Cargo
{
  /// <summary>
  /// This class is used for output LinkedCorrRejectionSearchData 
  /// </summary>
  //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
  public class CgoLinkedCorrRejectionSearchData
  {
    public Guid Id
    {
      get;
      set;
    }
    public String RejectionMemoNumber
    {
      get;
      set;
    }
    private String _correspondenceNumber
    {
      get;
      set;
    }
    public String InvoiceNumber
    {
      get;
      set;
    }
    public String BillingPeriod
    {
      get;
      set;
    }
    public String BillingMemberCode
    {
      get;
      set;
    }
    public String BilledMemberCode
    {
      get;
      set;
    }
    public String ReasonCode
    {
      get;
      set;
    }
    public String NetRejectAmount
    {
      get;
      set;
    }

    public string CorrespondenceNumber
    {
      get
      {
        return string.IsNullOrEmpty(_correspondenceNumber) ? _correspondenceNumber : string.Format("{0:00000000000}", Convert.ToInt64(_correspondenceNumber));
      }
      set
      {
        _correspondenceNumber = value;
      }
    }
  }
}

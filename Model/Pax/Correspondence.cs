using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Pax
{
  public class Correspondence : CorrespondenceBase
  {

    public Guid InvoiceId
    {
      get;
      set;
    }

    public PaxInvoice Invoice
    {
      get;
      set;
    }
  
   

    public int NoOfAttachments
    {
      get
      {
        return !Equals(Attachments, null) ? Attachments.Count : 0;
      }
    }

    //CMP526 - Passenger Correspondence Identifiable by Source Code
    public int? SourceCode { get; set; }

    public List<CorrespondenceAttachment> Attachments { get; set; }

    public List<RejectionMemo> LinkedRejections { get; private set; }

    public Correspondence()
    {
      LinkedRejections = new List<RejectionMemo>();
      Attachments = new List<CorrespondenceAttachment>();
    }
  }
}

using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Model.Cargo
{
  public class CargoCorrespondence : CorrespondenceBase
  {

    public Guid InvoiceId
    {
      get;
      set;
    }

    public CargoInvoice Invoice
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

    public List<CargoCorrespondenceAttachment> Attachments { get; set; }

    public List<CargoRejectionMemo> LinkedRejections { get; private set; }

    public CargoCorrespondence()
    {
      LinkedRejections = new List<CargoRejectionMemo>();
      Attachments = new List<CargoCorrespondenceAttachment>();
    }
  }
}

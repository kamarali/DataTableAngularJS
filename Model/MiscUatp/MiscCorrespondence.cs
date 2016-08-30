using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.MiscUatp
{
  public class MiscCorrespondence : CorrespondenceBase
  {
    /// <summary>
    /// For Navigation to <see cref="MiscUatpInvoice"/>. 
    /// </summary>
    public Guid InvoiceId { get; set; }

    public MiscUatpInvoice Invoice { get; set; }

    
    public string ChargeCode { get; set; }

    public string ChargeCategory { get; set; }

    public int NoOfAttachments
    {
      get
      {
        if (!Equals(Attachments, null))
        {
          return Attachments.Count;
        }

        return (0);
      }
    }

    /// <summary>
    /// Open - When a invoice is created, Accept - When user accepts an invoice from billing history, Reject - When user reject the invoice from billing history
    /// </summary>
    public int TransactionStatusId { set; get; }

    public TransactionStatus TransactionStatus
    {
      get
      {
        return (TransactionStatus)TransactionStatusId;
      }
      set
      {
        TransactionStatusId = Convert.ToInt32(value);
      }
    }

    public List<MiscUatpCorrespondenceAttachment> Attachments { get; set; }

    public MiscCorrespondence()
    {
      Attachments = new List<MiscUatpCorrespondenceAttachment>();
    }
  }
}

using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MU
{
  public class Correspondence : CorrespondenceBase
  {
    /// <summary>
    /// For Navigation to <see cref="MiscUatpInvoice"/>. 
    /// </summary>
    public Guid InvoiceId { get; set; }

    public MiscUatpInvoice Invoice { get; set; }

    /// <summary>
    /// For Navigation to <see cref="TODO-User"/>. 
    /// </summary>
    public int CorrespondenceOwnerId { get; set; }

    public string ChargeCode { get; set; }

    public Correspondence()
    {
      
    }
  }
}

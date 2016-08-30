using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Cargo.ParsingModel
{
  public class CargoInvoiceModel
  {
    /// <summary>
    /// This will store list of Cargo invoices for one billed airline
    /// </summary>
    public List<CargoInvoice> CgoInvoiceCollection { get; set; }

    /// <summary>
    /// This will store FileTotal model corresponding to a one Idec file
    /// </summary>
    public FileTotal FileTotal { get; set; }

    /// <summary>
    /// This will store FileHeadet model
    /// </summary>
    public FileHeader FileHeader { get; set; }

  }
}

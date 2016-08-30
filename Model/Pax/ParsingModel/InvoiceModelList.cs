using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Pax.ParsingModel
{
  public class InvoiceModelList
  {
    /// <summary>
    /// This will store total invoice model collection of one billed airline
    /// </summary>
    public List<InvoiceModel> InvoiceModelCollection { get; set; }

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

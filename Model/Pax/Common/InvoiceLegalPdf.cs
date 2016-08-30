using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Common 
{
  public class InvoiceLegalPdf : MasterBase<int>
  {
    /// <summary>
    /// Invoice Number
    /// </summary>
    public string InvoiceId { get; set; }

    /// <summary>
    /// Legal PDF generated PDF path
    /// </summary>
    public string LegalPdfLocation { get; set; }


  }
}

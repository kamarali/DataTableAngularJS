using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Base;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Model.MiscUatp
{
  public class MiscUatpInvoiceTax : MiscUatpTax
  {
     /// <summary>
    /// Gets or sets the line item additional details.
    /// </summary>
    /// <value>The line item additional details.</value>
    public List<MiscUatpInvoiceTaxAdditionalDetail> MiscUatpInvoiceTaxAdditionalDetails { get; private set; }

    public MiscUatpInvoiceTax()
    {
      MiscUatpInvoiceTaxAdditionalDetails = new List<MiscUatpInvoiceTaxAdditionalDetail>();
    }
  }
}
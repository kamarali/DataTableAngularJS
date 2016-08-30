using System.Collections.Generic;
using Iata.IS.Model.Pax.Base;

namespace Iata.IS.Model.Common
{
  public class TransmissionSummary : TransmissionSummaryBase
  {
    public TransmissionSummary()
    {
      TotalInvoiceAmount = new List<decimal>();
    }

    public int PaxSamplingFormCCount { get; set; }

    /// <summary>
    /// Removed from base class and added here as name for this element in xml file is different from PAX format.
    /// </summary>
    public List<decimal> TotalInvoiceAmount { get; private set; }
  }
}

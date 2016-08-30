using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Pax.Base;

namespace Iata.IS.Model.Common
{
  public class TransmissionHeader : TransmissionHeaderBase
  {
    /// <summary>
    /// Get or set the TransmissionHeader additional details.
    /// </summary>
    public List<AdditionalDetail> TransmissionHeaderAdditionalDetails { get; set; }

    public TransmissionHeader()
    {
      TransmissionHeaderAdditionalDetails = new List<AdditionalDetail>();
    }
  }
}

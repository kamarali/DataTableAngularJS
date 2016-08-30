using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Common
{
  public class SampleDigit : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the provisional billing month.
    /// </summary>
    /// <value>The provisional billing month.</value>
    public string ProvisionalBillingMonth { get; set; }

    /// <summary>
    /// Gets or sets the digit announcement date time.
    /// </summary>
    /// <value>The digit announcement date time.</value>
    public DateTime DigitAnnouncementDateTime { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class PaymentTermsAdditionalDetail : AdditionalDetailBase
  {
    /// <summary>
    /// Gets or sets the payment detail id.
    /// </summary>
    /// <value>The payment detail id.</value>
    public Guid PaymentDetailId { get; set; }

    /// <summary>
    /// Gets or sets the payment detail.
    /// </summary>
    /// <value>The payment detail.</value>
    public PaymentDetail PaymentDetail { get; set; }
  }
}

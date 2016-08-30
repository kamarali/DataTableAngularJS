using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Cargo.Base
{
  public class OtherCharge : EntityBase<Guid>
  {

    public string OtherChargeCode { get; set; }

    public double? OtherChargeCodeValue { get; set; }

    public Guid ParentId { get; set; }

    public string OtherChargeVatLabel { get; set; }

    public string OtherChargeVatText { get; set; }

    public double? OtherChargeVatBaseAmount { get; set; }

    public double? OtherChargeVatPercentage { get; set; }

    public double? OtherChargeVatCalculatedAmount { get; set; }

    /* CMP-613: Validation for Cargo Other Charges Breakdown */
    public string OtherChargeName { get; set; }
  }
}

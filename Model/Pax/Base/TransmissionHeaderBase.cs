using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Base
{
  public abstract class TransmissionHeaderBase : EntityBase<string>
  {
    public int TransmissionId { get; set; }

    public DateTime TransmissionDateTime { get; set; }

    public string Version { get; set; }

    public string IssuingOrganizationId { get; set; }

    public string ReceivingOrganizationId { get; set; }

    public string BillingCategory { get; set; }
  }
}

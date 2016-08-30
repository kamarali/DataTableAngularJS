using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Base;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Model.Pax
{
  public class InvoiceTransmission : InvoiceTransmissionBase
  {
    public InvoiceTransmission()
    {
      SamplingFormC = new List<SamplingFormCRecord>();
      Invoice = new List<PaxInvoice>();
    }

    public TransmissionHeader TransmissionHeader { get; set; }

    public List<PaxInvoice> Invoice { get; private set; }

    public TransmissionSummary TransmissionSummary { get; set; }

    public List<SamplingFormCRecord> SamplingFormC { get; private set; }
  }
}

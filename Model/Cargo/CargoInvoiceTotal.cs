using System.Collections.Generic;
using Iata.IS.Model.Cargo.Base;

namespace Iata.IS.Model.Cargo
{
  public class CargoInvoiceTotal : InvoiceTotalBase
  {
    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public decimal TotalIscAmount { get; set; }

    public decimal TotalWeightCharge { get; set; }

    public decimal TotalValuationCharge { get; set; }

    public decimal TotalOtherCharge { get; set; }

    /// <summary>
    /// For XML parsing and validations only
    /// </summary>
    public decimal TotalLineItemAmount { get; set; }

    public int TotalNoOfRecords { get; set; }

    public CargoInvoice Invoice { get; set; }

    public List<CargoInvoiceTotalVat> InvoiceTotalVat { get; set; }
  }
}


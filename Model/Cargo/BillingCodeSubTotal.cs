using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo.Enums;

namespace Iata.IS.Model.Cargo
{
  public class BillingCodeSubTotal : EntityBase<Guid>
  {
    public BillingCodeSubTotal()
    {
      CGOBillingCodeSubTotalVat = new List<CargoBillingCodeSubTotalVat>();
    }

    public List<CargoBillingCodeSubTotalVat> CGOBillingCodeSubTotalVat { get; set; }

    public Guid InvoiceId { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public decimal TotalIscAmount { get; set; }

    public decimal TotalWeightCharge { get; set; }

    public decimal TotalValuationCharge { get; set; }

    public decimal TotalOtherCharge { get; set; }

    public int NumberOfBillingRecords { get; set; }

    public decimal TotalNetAmount { get; set; }

    public long NumberOfChildRecords { get; set; }

    public decimal BillingCodeSbTotal { get; set; }

    public CargoInvoice Invoice { get; set; }

    public decimal TotalVatAmount { get; set; }

    //The field is not stored in database but it will be used to validate the data coming from IDEC.
    public int TotalNumberOfRecords { get; set; }

    //The field is not stored in database but it will be used to validate the data coming from IDEC.
    public string BillingCodeSubTotalDesc { get; set; }

    public int BillingCode { get; set; }

   }
}

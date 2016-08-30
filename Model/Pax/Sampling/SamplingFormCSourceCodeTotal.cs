using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Sampling
{
  public class SamplingFormCSourceCodeTotal : EntityBase<Guid>
  {
    public decimal TotalGrossValue { get; set; }
    public decimal TotalIscAmount { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public decimal TotalNetAmount { get; set; }
    public int NoOfBillingRecord { get; set; }
    public double TotalHandlingFeeAmount { get; set; }
    public int SourceId { get; set; }
    public decimal TotalOtherCommisionAmount { get; set; }
    public decimal TotalUatpAmount { get; set; }
    public decimal TotalVatAmount { get; set; }
    public decimal TotalNetAmountAfterSamplingConstant { get; set; }

    //The field is not stored in database but it will be used to validate the data coming from IDEC.
    public long TotalNumberOfRecords { get; set; }

    public Guid SamplingFormCId { get; set; }

    public SamplingFormC SamplingFormC { get; set; }

    public SourceCode SourceCode { get; set; }
  }
}
using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax
{
  public class SourceCodeTotal : EntityBase<Guid>
  {
    public Guid InvoiceId { get; set; }

    public decimal TotalIscAmount { get; set; }

    public decimal TotalOtherCommission { get; set; }

    public decimal TotalUatpAmount { get; set; }

    public double TotalHandlingFee { get; set; }

    public int NumberOfBillingRecords { get; set; }

    public decimal TotalGrossValue { get; set; }

    public decimal TotalTaxAmount { get; set; }

    public decimal TotalNetAmount { get; set; }

    /// <summary>
    /// Gets the Sampling Constant for Rejection Memo Source Code total.
    /// </summary>
    /// <remarks>
    /// No setter method is provided for this property as the value 
    /// won't be saved in database.
    /// To use this property, make sure that Invoice object is fetched along with 
    /// source code list.
    /// </remarks>
    public double SamplingConstant
    {
      get
      {
        return (Invoice == null ? 0.0 : Invoice.SamplingConstant);
      }
    }

    public decimal TotalAmountAfterSamplingConstant { get; set; }

    public decimal TotalVatAmount { get; set; }

    //The field is not stored in database but it will be used to validate the data coming from IDEC.
    public long TotalNumberOfRecords { get; set; }

    //The field is not stored in database but it will be used to validate the data coming from IDEC.
    public long NumberOfChildRecords { get; set; }

    public int SourceCodeId { get; set; }

    public PaxInvoice Invoice { get; set; }

    public List<SourceCodeVat> VatBreakdown { get; private set; }

    public SourceCodeTotal()
    {
      VatBreakdown = new List<SourceCodeVat>();
    }

  }
}

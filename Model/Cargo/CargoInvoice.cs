using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo.Common;
//using Iata.IS.Model.Cargo.Enums;
using System.Collections.Generic;
using Iata.IS.Model.Common;
//using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Enums;
using TransactionStatus = Iata.IS.Model.Pax.Enums.TransactionStatus;

namespace Iata.IS.Model.Cargo
{
  public class CargoInvoice : InvoiceBase
  {
    public bool SendIncremental { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public int InvoiceTypeId { get; set; }

    public string DisplayInvoiceStatus { get; set; }

    public IsValidationExceptionSummary ISValidationExceptionSummary { get; set; }

    public List<ValidationExceptionSummary> ValidationExceptionSummary { get; set; }

    public List<CargoRejectionMemo> CGORejectionMemo { get; private set; }

    public List<CargoBillingMemo> CGOBillingMemo { get; private set; }

    public List<CargoCreditMemo> CGOCreditMemo { get; private set; }

    public CargoInvoiceTotal CGOInvoiceTotal { get; set; }

    public List<CargoInvoiceTotalVat> CGOInvoiceTotalVat { get; private set; }

    public List<BillingCodeSubTotal> CGOBillingCodeSubTotal { get; private set; }

    public List<AwbRecord> AwbDataRecord { get; private set; }


    public InvoiceType InvoiceType
    {
      get
      {
        return (InvoiceType)InvoiceTypeId;
      }
      set
      {
        InvoiceTypeId = Convert.ToInt32(value);
      }
    }

    public CargoInvoice()
    {
      ValidationExceptionSummary = new List<ValidationExceptionSummary>();
      AwbDataRecord = new List<AwbRecord>();
      CGOBillingMemo = new List<CargoBillingMemo>();
      CGOCreditMemo = new List<CargoCreditMemo>();
      CGORejectionMemo = new List<CargoRejectionMemo>();     
      CGOInvoiceTotalVat = new List<CargoInvoiceTotalVat>();
      CGOBillingCodeSubTotal = new List<BillingCodeSubTotal>();   
      
    }
    
  }
}

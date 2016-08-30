using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Model.Common
{
  public class ValidationExceptionSummary : EntityBase<Guid>
  {
    public string FileLogId { get; set; }

    public int ExceptionCodeId { get; set; }

    /// <summary>
    /// this should be zero while parsing
    /// </summary>
    public int OnlineCorrectionStaus { get; set; }

    public string InvoiceNo { get; set; }

    public int BilledMemberId { get; set; }

    public int ChargeCategoryId { get; set; }

    public string InvoiceType { get; set; }

    public string InvoiceId { get; set; }

    public string ProvInvoiceNo { get; set; }

    public bool IsFormC { get; set; }

    public List<IsValidationExceptionDetail> ValidationExceptionDetails { get; private set; }

    public CargoInvoice Invoice { get; set; }

    public ValidationExceptionSummary()
    {
      ValidationExceptionDetails = new List<IsValidationExceptionDetail>();
    }

   
  }
}


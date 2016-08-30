using System;
namespace Iata.IS.Web.Reports.ValidationSummary
{
  public class PassengerValidationSummary
  {

    public int SerialNo { get; set;}

    public string BillingEntityCode { get; set; }

    public int ClearenceMonth { get; set;}

    public int BillingPeriod { get; set; }

    public string BillingCategory { get; set; }

    public string FileName { get; set; }

    public DateTime FileSubmissionDate { get; set; }

    public string SubmissionFormat { get; set;}

    public string BilledEntityCode { get; set; }

    public int InvoiceNo { get; set;}

    public string CurrencyOfBilling { get; set;}

    public decimal InvoiceAmmountInBilling { get; set;}

    public string InvoiceStatus { get; set; }

    public int ErrorAtInvoiceLevel { get; set; }

    public int TotalNoOfBillingRecord { get; set; }

    public int TotalRecordSuccessfullyValidated { get; set; }

    public int TotalNoOfValidationError { get; set; }

    public string ErrorLevel { get; set; }

    public DateTime Clearance { get; set; }

    /// <summary>
    /// property to get and set time on report
    /// </summary>
    public string ReportGeneratedDate { get; set; }
  }
}

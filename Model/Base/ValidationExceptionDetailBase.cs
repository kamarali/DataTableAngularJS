using System.ComponentModel;

namespace Iata.IS.Model.Base
{
  public class ValidationExceptionDetailBase 
  {
    //New Common properties

    [DisplayName("Serial No")]
    public int SerialNo { get; set; }

    [DisplayName("Billing Entity Code")]
    public string BillingEntityCode { get; set; }

    [DisplayName("Clearance Month")]
    public string ClearanceMonth { get; set; }

    [DisplayName("Period Number")]
    public int PeriodNumber { get; set; }

    [DisplayName("Billing Category")]
    public string CategoryOfBilling { get; set; }

    [DisplayName("Billing File Name")]
    public string FileName { get; set; }

    [DisplayName("Billing File Submission Date")]
    public string BillingFileSubmissionDate { get; set; }

    [DisplayName("Submission Format")]
    public string SubmissionFormat { get; set; }

    [DisplayName("Billed Entity Code")]
    public string BilledEntityCode { get; set; }

    [DisplayName("Invoice Number")]
    public string InvoiceNumber { get; set; }

    [DisplayName("MISC Charge Category / PAX Billing Code / CGO Billing Code")]
    public string ChargeCategoryOrBillingCode { get; set; }

    [DisplayName("MISC Charge Code / Pax Source Code / Pax Source Code")]
    public string SourceCodeId { get; set; }

    [DisplayName("MISC Line Item No / Pax Batch Number / CGO Batch Number")]
    public int LineItemOrBatchNo { get; set; }

    [DisplayName("MISC Line Item Detail No / Pax Seq Number / CGO Seq Number")]
    public int LineItemDetailOrSequenceNo { get; set; }

    [DisplayName("Main Doc No")]
    public string DocumentNo { get; set; }

    [DisplayName("Linked Doc No")]
    public string LinkedDocNo { get; set; }

    [DisplayName("Error Code")]
    public string ExceptionCode { get; set; }

    [DisplayName("Error Level")]
    public string ErrorLevel { get; set; }

    [DisplayName("Field Name")]
    public string FieldName { get; set; }

    [DisplayName("Field Value")]
    public string FieldValue { get; set; }

    [DisplayName("Error Description")]
    public string ErrorDescription { get; set; }

    [DisplayName("Error Status")]
    public string ErrorStatus { get; set; }

  }
}
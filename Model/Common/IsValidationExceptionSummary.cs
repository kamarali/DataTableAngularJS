using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using System.ComponentModel;

namespace Iata.IS.Model.Common
{
  public class IsValidationExceptionSummary 
  {
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
    public string BillingFileName { get; set; }

    [DisplayName("Billing File Submission Date")]
    public string BillingFileSubmissionDate { get; set; }

    [DisplayName("Submission Format")]
    public string SubmissionFormat { get; set; }

    [DisplayName("Billed Entity Code")]
    public string BilledEntityCode { get; set; }

    [DisplayName("Invoice Number")]
    public string InvoiceNumber { get; set; }

    [DisplayName("Currency Of Billing")]
    public string CurrencyOfBilling { get; set; }

    [DisplayName("Invoice Amount In BillingCurrency")]
    public decimal InvoiceAmountInBillingCurrency { get; set; }

    [DisplayName("Invoice Status")]
    public string InvoiceStatus { get; set; }

    [DisplayName("Error At Invoice Level")]
    public string ErrorAtInvoiceLevel { get; set; }

    [DisplayName("Total number of billing records")]
    public int TotalNoOfBillingRecords { get; set; }

    [DisplayName("Records successfully validated")]
    public int RecordsSuccessfullyValidated { get; set; }

    [DisplayName("Records in Validation Error")]
    public int RecordsInValidationError { get; set; }

    public IList<IsValidationExceptionDetail> isValidationExceptionDetail { get; set; }

    //SCP 257975: Issue with IIPS/ISWeb Invoice status
    //Description: Updating R1 report to reflect invoice status same as R2.
    // Added default and overloaded constructor for easy initialization.
    public IsValidationExceptionSummary()
    {
    }

    public IsValidationExceptionSummary(int serialNo, string billingEntityCode, string clearanceMonth, int periodNumber, string categoryOfBilling, 
        string billingFileName, string billingFileSubmissionDate, string submissionFormat, string billedEntityCode, string invoiceNumber, 
        string currencyOfBilling, decimal invoiceAmountInBillingCurrency, string invoiceStatus, string errorAtInvoiceLevel, int totalNoOfBillingRecords, 
        int recordsSuccessfullyValidated, int recordsInValidationError)
    {
        this.SerialNo = serialNo;
        this.BillingEntityCode = billingEntityCode;
        this.ClearanceMonth = clearanceMonth;
        this.PeriodNumber = periodNumber;
        this.CategoryOfBilling = categoryOfBilling;
        this.BillingFileName = billingFileName;
        this.BillingFileSubmissionDate = billingFileSubmissionDate;
        this.SubmissionFormat = submissionFormat;
        this.BilledEntityCode = billedEntityCode;
        this.InvoiceNumber = invoiceNumber;
        this.CurrencyOfBilling = currencyOfBilling;
        this.InvoiceAmountInBillingCurrency = invoiceAmountInBillingCurrency;
        this.InvoiceStatus = invoiceStatus;
        this.ErrorAtInvoiceLevel = errorAtInvoiceLevel;
        this.TotalNoOfBillingRecords = totalNoOfBillingRecords;
        this.RecordsSuccessfullyValidated = recordsSuccessfullyValidated;
        this.RecordsInValidationError = recordsInValidationError;
    }

  }
}

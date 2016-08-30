using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.ExternalInterfaces.ICHInterface
{
  public class ICHSettlementData
  {
    public string ClearancePeriod { get; set; }
    public string CreditorsDesignator { get; set; }
    public string CreditorsCode { get; set; }
    public string ZoneCode { get; set; }
    public string ClearanceCurrencyCode { get; set; }
    public string DebtorsDesignator { get; set; }
    public string DebtorsCode { get; set; }
    public string SponsorDesignator { get; set; }
    public string SponsorCode { get; set; }
    // CMP 514: add merge parent creditors designator, creditors code, debtors designator and debtors code properties. 
    public string MergeParentCreditorsDesignator { get; set; }
    public string MergeParentCreditorsCode { get; set; }
    public string MergeParentDebtorsDesignator { get; set; }
    public string MergeParentDebtorsCode { get; set; }
    public string BillingCategory { get; set; }
    public Guid UniqueInvoiceNumber { get; set; }
    public string TransmitterDesignator { get; set; }
    public string TransmitterCode { get; set; }
    public string InvoiceSource { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string LocalCurrencyCode { get; set; }
    public double ExchangeRate { get; set; }
    public string DebitOrCredit { get; set; }
    public string SettlementMethodIndicator { get; set; }
    public bool SuspendedInvoiceIndicator { get; set; }
    public bool LateSubmission { get; set; }
    public bool IsResubmitted { get; set; }
    //CMP #637: Changes to ICH Settlement 
    public string CHAgreementIndicator { get; set; }
    public string CHDueDate { get; set; }
    public string SalesOrderNumber { get; set; }
    public string CreditorLocationID { get; set; }
    public string DebtorLocationID { get; set; }
    public double TotalLocalCurrency { get; set; }
    /* SCP#390702 - KAL: Issue with Clearance Amount. Desc: Clearance Amount is made nullable. */
    public double? TotalClearanceCurrency { get; set; }
    public DateTime SubmissionTime { get; set; }
    public string CreditorSponsorDesignator { get; set; }
    public string CreditorSponsorCode { get; set; }
  }
}

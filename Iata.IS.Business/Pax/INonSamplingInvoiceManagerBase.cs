using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.MemberProfile.Common;

namespace Iata.IS.Business.Pax
{
  /// <summary>
  /// Common methods for sampling and non-sampling invoices functionality.
  /// </summary>
  public interface INonSamplingInvoiceManagerBase : IInvoiceManagerBase 
  {
    /// <summary>
    /// To get Billed/Billing Member's reference data 
    /// </summary>
    /// <param name="memberId">memberId</param>
    /// <returns>List of the Billed/Billing Member's reference data</returns>
    MemberLocationInformation GetMemberReferenceData(MemberLocationInformation memberId);

    /// <summary>
    /// To add Billed/Billing Member reference data to the database
    /// </summary>
    /// <param name="organizationInformation">Details of the Billed/Billing Member's reference data</param>
    /// <returns>Added Billed/Billing Member, reference data record </returns>
    MemberLocationInformation AddMemberReferenceData(MemberLocationInformation organizationInformation);

    /// <summary>
    /// To update Billed/Billing Member's reference data
    /// </summary>
    /// <param name="organizationInformation">Details of the Billed/Billing Member's reference data</param>
    /// <returns>>Updated Billed/Billing Member reference data record</returns>
    MemberLocationInformation UpdateMemberReferenceData(MemberLocationInformation organizationInformation);

    /// <summary>
    /// To get Invoice level VAT list
    /// </summary>
    /// <param name="invoiceNumber">string of the Invoice</param>
    /// <returns>List of the invoice level Vat</returns>
    IList<InvoiceTotalVat> GetInvoiceLevelVatList(string invoiceNumber);

    /// <summary>
    /// To get the derived invoice level VAT list
    /// </summary>
    /// <param name="invoiceNumber">>string of the Invoice</param>
    /// <returns>List of the derived invoice level Vat</returns>
    IList<InvoiceTotalVat> GetInvoiceLevelDerivedVatList(string invoiceNumber);

    /// <summary>
    /// To add invoice level VAT 
    /// </summary>
    /// <param name="vat">Invoice level VAT details</param>
    /// <returns>Details of the added VAT record</returns>
    InvoiceTotalVat AddInvoiceLevelVat(InvoiceTotalVat vat);

    /// <summary>
    /// To delete VAT record from database
    /// </summary>
    /// <param name="vatId">string for VAT</param>
    /// <returns>True if successfully deleted , false otherwise</returns>
    bool DeleteInvoiceLevelVat(string vatId);

    /// <summary>
    /// To get Invoice Total record
    /// </summary>
    /// <param name="invoiceNumber">string of the Invoice</param>
    /// <returns>Invoice total record </returns>
    InvoiceTotalRecord GetInvoiceTotal(string invoiceNumber);

    /// <summary>
    /// To get list of source code for Credit memo/Sampling Form
    /// </summary>
    /// <param name="invoiceId">string of the invoice</param>
    /// <returns>List of source code records</returns>
    IList<SourceCodeTotal> GetSourceCodeList(string invoiceId);

  }
}

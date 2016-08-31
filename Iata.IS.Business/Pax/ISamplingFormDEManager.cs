using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Sampling;
using System;

namespace Iata.IS.Business.Pax
{
  public interface ISamplingFormDEManager : IInvoiceManager
  {
    /// <summary>
    /// Adds the sampling form D.
    /// </summary>
    /// <param name="samplingFormD">Sampling form D record to be created</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns></returns>
    SamplingFormDRecord AddSamplingFormD(SamplingFormDRecord samplingFormD, out string duplicateErrorMessage);

    /// <summary>
    /// Updates the sampling form D.
    /// </summary>
    /// <param name="samplingFormD">Sampling form D record to be updated</param>
    /// <param name="duplicateErrorMessage"></param>
    /// <returns></returns>
    SamplingFormDRecord UpdateSamplingFormD(SamplingFormDRecord samplingFormD, out string duplicateErrorMessage);

    /// <summary>
    /// Gets the details of sampling form D.
    /// </summary>
    /// <param name="samplingFormDId">Sampling form D id</param>
    /// <returns>Sampling Form D object.</returns>
    SamplingFormDRecord GetSamplingFormD(string samplingFormDId);

    /// <summary>
    /// Deletes the sampling form D.
    /// </summary>
    /// <param name="samplingFormDId">Sampling form D id</param>
    /// <returns>true if record successfully gets deleted otherwise false.</returns>
    bool DeleteSamplingFormD(string samplingFormDId);
    
    /// <summary>
    /// Returns the list of form D records.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    IQueryable<SamplingFormDRecord> GetSamplingFormDList(string invoiceId);

    /// <summary>
    /// Creates the form E.
    /// </summary>
    /// <param name="samplingFormE"></param>
    /// <returns>Created SamplingFormE object</returns>
    SamplingFormEDetail CreateSamplingFormE(SamplingFormEDetail samplingFormE);

    /// <summary>
    /// get form E details.
    /// </summary>
    /// <param name="samplingFormEId">sampling form E id</param>
    /// <returns>SamplingFormE object</returns>
    SamplingFormEDetail GetSamplingFormE(string samplingFormEId);

    /// <summary>
    /// Update form E details.
    /// </summary>
    /// <param name="samplingFormE"></param>
    /// <returns></returns>
    SamplingFormEDetail UpdateSamplingFormE(SamplingFormEDetail samplingFormE);

    /// <summary>
    /// Add provisional Invoice.
    /// </summary>
    /// <param name="provisionalInvoice">Provisional invoice object.</param>
    /// <returns>Created Provisional invoice object.</returns>
    ProvisionalInvoiceRecordDetail AddProvisionalInvoice(ProvisionalInvoiceRecordDetail provisionalInvoice);

    /// <summary>
    /// Returns the list of provisional invoice records.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns>List of provisional invoice records</returns>
    IQueryable<ProvisionalInvoiceRecordDetail> GetProvisionalInvoiceList(string invoiceId);

    /// <summary>
    /// Returns the provisional invoice record.
    /// </summary>
    /// <param name="provisionalInvoiceId">provisional invoice id</param>
    ProvisionalInvoiceRecordDetail GetProvisionalInvoice(string provisionalInvoiceId);

    /// <summary>
    /// Deletes the provisional invoice record.
    /// </summary>
    /// <param name="provisionalInvoiceId">provisional invoice id</param>
    /// <returns>True if record gets deleted successfully otherwise false.</returns>
    bool DeleteProvisionalInvoice(string provisionalInvoiceId);

    /// <summary>
    /// Determines whether transaction exists for the specified invoice id
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// 	<c>true</c> if transaction exists for the specified invoice id; otherwise, <c>false</c>.
    /// </returns>
    bool IsTransactionExists(string invoiceId);

    /// <summary>
    /// Determines whether provisional invoices exists for the specified Invoice ID
    /// </summary>
    /// <param name="invoiceId">The Invoice id.</param>
    /// <returns>
    /// <c>true</c> if transaction exists for the specified invoice id; otherwise, <c>false</c>.
    /// </returns>
    bool IsProvisionalInvoiceExists(string invoiceId);
    
    /// <summary>
    /// Get Sampling Form D Attachment details.
    /// </summary>
    /// <param name="attachmentId">Attachment Id</param>
    /// <returns></returns>
    SamplingFormDRecordAttachment GetSamplingFormDAttachment(string attachmentId);

    /// <summary>
    /// Insert Sampling Form D Attachment
    /// </summary>
    /// <param name="attachment">Attachment record</param>
    /// <returns></returns>
    SamplingFormDRecordAttachment AddSamplingFormDAttachment(SamplingFormDRecordAttachment attachment);

    /// <summary>
    /// Update Sampling Form D attachment record parent id
    /// </summary>
    /// <param name="attachments">list of attachment</param>
    /// <param name="parentId">billing memo Id</param>
    /// <returns></returns>
    IQueryable<SamplingFormDRecordAttachment> UpdateSamplingFormDAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Check for duplicate file name for billing memo attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="samplingFormDId">sampling Form D Id</param>
    /// <returns></returns>
    bool IsDuplicateSamplingFormDAttachmentFileName(string fileName, Guid samplingFormDId);

    /// <summary>
    /// Add sampling Form E Vat
    /// </summary>
    /// <param name="vat">Sampling Form E record</param>
    /// <returns></returns>
    SamplingFormEDetailVat AddSamplingFormEVat(SamplingFormEDetailVat vat);

    /// <summary>
    /// Get Vat list associated with Form DE
    /// </summary>
    /// <param name="invoiceId">Form DE record Id</param>
    /// <returns></returns>
    IQueryable<SamplingFormEDetailVat> GetSamplingFormEVatList(string invoiceId);

    /// <summary>
    /// Delete sampling FormE Vat 
    /// </summary>
    /// <param name="formEVatId">FormEVat Id</param>
    /// <returns></returns>
    bool DeleteSamplingFormEVat(string formEVatId);

    /// <summary>
    /// Retrieves list of derived VAT in Form DE invoice
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    IList<DerivedVatDetails> GetFormDInvoiceLevelDerivedVatList(string invoiceId);

    /// <summary>
    /// Retrieves list of VAT types which are not applied in Form DE invoice
    /// </summary>
    /// <param name="invoiceId">invoice id for which vat list to be retrieved.</param>
    /// <returns></returns>
    IList<NonAppliedVatDetails> GetFormDNonAppliedVatList(string invoiceId);

    /// <summary>
    /// Gets the sampling form D record attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<SamplingFormDRecordAttachment> GetSamplingFormDRecordAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Get linked coupon details.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="ticketCouponNumber">The ticket coupon number.</param>
    /// <param name="ticketDocNumber">The ticket doc number.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <returns></returns>
    List<LinkedCoupon> GetFormDLinkedCouponDetails(Guid invoiceId, int ticketCouponNumber, long ticketDocNumber, string issuingAirline);

    /// <summary>
    /// Updates the Form E details.
    /// </summary>
    /// <param name="invoiceId"> The invoice id.</param>
    void UpdateFormEDetails(Guid invoiceId);

    IList<SamplingFormDRecord> GetSamplingFormDList(string[] rejectionIdList);

    string ValidateFormDRecord(SamplingFormDRecord formDRecord, SamplingFormDRecord formDRecordInDb, PaxInvoice samplingFormDE, bool isErrorCorrection = false);

    /// <summary>
    /// validate auto calculated amount and percentage like: ISC percentage and ISC amount.
    /// </summary>
    /// <param name="percentage">isc percentage</param>
    /// <param name="evaluatedGrossAmount">evaluated gross amount</param>
    /// <param name="amount">isc amount</param>
    void ValidateIscPerAndAmt(double evaluatedGrossAmount, double percentage, double amount);
  }
}

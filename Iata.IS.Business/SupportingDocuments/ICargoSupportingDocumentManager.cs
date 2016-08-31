using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Payables;
using Iata.IS.Model.Common;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Model.Base;

namespace Iata.IS.Business.SupportingDocuments
{
  public interface ICargoSupportingDocumentManager
  {
      #region "Old code"
      /*
    /// <summary>
    /// Process supporting documents when batch file is uploaded
    /// </summary>
    bool ProcessAttachments(IEnumerable<RecordSearchCriteria> recordSearchCriteria, string extractedDirectory, IsInputFile isInputFile);

    /// <summary>
    /// Get list of unlinked supporting documents matching given search criteria
    /// </summary>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billedMember">The billed member.</param>
    /// <param name="billingMember">The billing member.</param>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="submissionDate">The submission date.</param>
    /// <param name="batchNumber">The batch number.</param>
    /// <param name="sequenceNumber">The sequence number.</param>
    /// <param name="breakdownSerialNumber">The breakdown serial number.</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <returns></returns>
    List<UnlinkedSupportingDocumentEx> GetUnlinkedSupportingDocuments(int? billingYear, int? billingMonth, int? billingPeriod, int? billedMember, int billingMember, string invoiceNumber, string fileName, DateTime? submissionDate, int? batchNumber, int? sequenceNumber, int? breakdownSerialNumber, int? billingCategoryId);

    /// <summary>
    /// Get details of selected unlinked supporting document by id
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    UnlinkedSupportingDocumentEx GetSelectedUnlinkedSupportingDocumentDetails(Guid id);

    /// <summary>
    /// Links Unlinked Document to the respective record based on criteria
    /// </summary>
    /// <param name="unlinkedSupportingDocument">The unlinked supporting document.</param>
    /// <returns></returns>
    string LinkDocument(UnlinkedSupportingDocument unlinkedSupportingDocument);

    /// <summary>
    /// Method to return supporting document search result
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    IQueryable<SupportingDocSearchResult> GetSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);
      */
      #endregion
      /// <summary>
    /// Method to return Cargo supporting document search result
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    IQueryable<CargoSupportingDocSearchResult> GetCargoSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);

    /// <summary>
    /// Method to return supporting document search result
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
   // IList<PayableSupportingDocSearchResult> GetPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);

    /// <summary>
    /// Method to return supporting document search result for Cargo Payables
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    IList<CargoPayableSupportingDocSearchResult> GetCargoPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria);

    /// <summary>
    /// Populate attachment list for supporting document search result record
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="recordTypeId">The record type id.</param>
    /// <returns></returns>
    IList<Attachment> GetAttachmentForSearchEntity(string invoiceId, string transactionId, int recordTypeId);

    /// <summary>
    /// Add supporting document
    /// </summary>
    /// <param name="attach">The attach.</param>
    /// <param name="recordTypeId">The record type id.</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    Attachment AddSupportingDoc(SupportingDocumentAttachment attach, int recordTypeId, Guid transactionId);

    /// <summary>
    /// Populate attachment detail to download attachment in supporting document
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="recordTypeId">The record type id.</param>
    /// <returns></returns>
   Attachment GetSupportingDocumentDetail(string transactionId, int recordTypeId);

    /// <summary>
    /// Delete supporting document record as per record type
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <param name="recordTypeId">The record type id.</param>
    /// <returns></returns>
    bool DeleteSupportingDoc(string attachmentId, int recordTypeId, InvoiceBase invoice);
      
     
    /// <summary>
    /// Populate attachment detail to download attachment in supporting document
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="recordTypeId">The record type id.</param>
    /// <returns>
    ///   <c>true</c> if [is duplicate file name] [the specified file name]; otherwise, <c>false</c>.
    /// </returns>
    bool IsDuplicateFileName(string fileName, string transactionId, int recordTypeId);
      #region "old code"
      /*
         bool DeleteUnlinkedDocuments(UnlinkedSupportingDocument unlinkedSupportingDocument);

         #region Added for unit testing purpose

         List<SupportingDocumentRecord> GetRecordListWithAttachments(RecordSearchCriteria recordSearchCriteria);

         bool LinkDocuments(IEnumerable<RecordSearchCriteria> recordSearchCriteria, List<UnlinkedSupportingDocument> unlinkedSupportingDocuments);

         bool CopyDocumentsToSfr(string sourceFilePath, string destinationFileName);

         bool CopyDocumentsToIsa(string sourceFilePath, string destinationFileName, FileServer fileServer);
           */
    #endregion
  }
}

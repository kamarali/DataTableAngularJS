using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Business.Pax
{
  public interface ISamplingFormCManager
  {
    /// <summary>
    /// Gets invoices matching the specified search criteria
    /// </summary>
    /// <returns>Sampling form c matching to search criteria</returns>
    IList<SamplingFormCResultSet> GetSamplingFormCList(SearchCriteria searchCriteria);

    /// <summary>
    /// Creates the sampling form C.
    /// </summary>
    /// <param name="samplingFormCHeader">Sampling form C header to be created</param>
    /// <returns></returns>
    SamplingFormC CreateSamplingFormC(SamplingFormC samplingFormCHeader);

    /// <summary>
    /// Updates the Sampling Form C header information.
    /// </summary>
    /// <param name="samplingFormC">Sampling form C to be updated.</param>
    /// <returns></returns>
    SamplingFormC UpdateSamplingFormC(SamplingFormC samplingFormC);

    /// <summary>
    /// Gets details of sampling form c headers.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id </param>
    /// <returns>Sampling form c details</returns>
    SamplingFormC GetSamplingFormCDetails(string samplingFormCId);

    /// <summary>
    /// Gets details of sampling form c headers.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id </param>
    /// <returns>Sampling form c details</returns>
    SamplingFormC GetSamplingFormCDetailsForAttachmentUpload(string samplingFormCId);

    /// <summary>
    /// Gets details of sampling form c headers.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="invoiceStatus">The invoice status.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <returns>Sampling form c details</returns>
    SamplingFormC GetSamplingFormCDetails(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int invoiceStatus, int? listingCurrencyId);

    /// <summary>
    /// Deletes a sampling form C.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id to be deleted</param>
    /// <returns>True if successfully deleted, false otherwise</returns>
    bool DeleteSamplingFormC(string samplingFormCId);

    /// <summary>
    /// Deletes a sampling form C.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="invoiceStatus">The invoice status.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <returns>
    /// True if successfully deleted, false otherwise
    /// </returns>
    bool DeleteSamplingFormC(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int invoiceStatus, int? listingCurrencyId);

    /// <summary>
    /// Validates a sampling form C.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id</param>
    /// <returns>True if successfully validated, false otherwise</returns>
    SamplingFormC ValidateSamplingFormC(string samplingFormCId);

    /// <summary>
    /// Submits the sampling form C.
    /// </summary>
    /// <param name="samplingFormCId">The sampling form C Identifier.</param>
    /// <returns></returns>
    SamplingFormC SubmitSamplingFormC(string samplingFormCId);


    /// <summary>
    /// Submits the sampling form C.
    /// </summary>
    /// <param name="samplingFormCIdList">List of sampling form c ids to be submitted</param>
    /// <returns></returns>
    IList<SamplingFormCResultSet> SubmitSamplingFormC(List<SamplingFormCResultSet> samplingFormDetailsList);

    /// <summary>
    /// Submits the sampling form C.
    /// </summary>
    /// <param name="samplingFormCIdList">List of sampling form c ids to be submitted</param>
    /// <returns></returns>
    IList<SamplingFormC> SubmitSamplingFormC(List<string> samplingFormCIdList);

    /// <summary>
    /// Present the sampling form C.Note:- This method is only for testing
    /// </summary>
    /// <param name="samplingFormCIdList">List of sampling form c ids to be presented</param>
    /// <returns></returns>
    IList<SamplingFormCResultSet> PresentSamplingFormC(List<SamplingFormCResultSet> samplingFormDetailsList);


    /// <summary>
    /// Gets the sampling form c source code total list for given sampling form c id
    /// </summary>
    /// <param name="samplingFormCId">string of sampling form C id</param>
    /// <returns>Source code list for sampling form c id</returns>
    IQueryable<SourceCodeTotal> GetSamplingFormCSourceCodeTotal(string samplingFormCId);

    /// <summary>
    /// Gets the sampling form c source code total list for given sampling form c id
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="invoiceStatusId">The invoice status id.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <returns>Source code list for sampling form c id</returns>
    IList<SamplingFormCSourceTotal> GetSamplingFormCSourceCodeTotal(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int invoiceStatusId, int? listingCurrencyId);

    /// <summary>
    /// To get the list of Sampling Form C records.
    /// </summary>
    /// <param name="samplingFormCId">string of sampling form C id</param>
    /// <returns>list of sampling form c records</returns>
    IList<SamplingFormCRecord> GetSamplingFormCRecordList(string samplingFormCId);

    /// <summary>
    /// To get the list of Sampling Form C records.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="statusId">The status id.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <returns>list of sampling form c records</returns>
    IList<SamplingFormCRecord> GetSamplingFormCRecordList(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int statusId, int? listingCurrencyId);

    /// <summary>
    /// Gets the sampling form c record details.
    /// </summary>
    /// <param name="samplingFormCRecordId">string of sampling form C record id</param>
    /// <returns>Details of the sampling form c record matching with samplingFormCRecordId</returns>
    SamplingFormCRecord GetSamplingFormCRecordDetails(string samplingFormCRecordId);

    /// <summary>
    /// Adds the sampling form C record in database.
    /// </summary>
    /// <param name="samplingFormCRecord">Details of sampling form c record to be added</param>
    /// <returns>added sampling form c record details</returns>
    SamplingFormCRecord AddSamplingFormCRecord(SamplingFormCRecord samplingFormCRecord);

    /// <summary>
    /// Updates the sampling form C record in database.
    /// </summary>
    /// <param name="samplingFormCRecord">Details of sampling form C record to be updated</param>
    /// <returns>updated sampling form c record details</returns>
    SamplingFormCRecord UpdateSamplingFormCRecord(SamplingFormCRecord samplingFormCRecord);

    /// <summary>
    /// Deletes the sampling form C record from database for given sampling form c record id
    /// </summary>
    /// <param name="samplingFormCRecordId">string of sampling form C record id</param>
    /// <returns>True if successfully deleted,false otherwise</returns>
    bool DeleteSamplingFormCRecord(string samplingFormCRecordId);

    /// <summary>
    /// Deletes the sampling form C record from database for given sampling form c record id
    /// </summary>
    /// <param name="samplingFormCRecordId">string of sampling form C record id</param>
    /// <returns>True if successfully deleted,false otherwise</returns>
    bool DeleteSamplingFormCRecord(string samplingFormCRecordId, out SamplingFormC samplingFormCRecord);

    /// <summary>
    /// Gets the sampling form C record attachment details.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    SamplingFormCRecordAttachment GetSamplingFormCRecordAttachmentDetails(string attachmentId);

    /// <summary>
    /// Adds the sampling form C record attachment.
    /// </summary>
    /// <param name="attachment">The attachment.</param>
    /// <returns></returns>
    SamplingFormCRecordAttachment AddSamplingFormCRecordAttachment(SamplingFormCRecordAttachment attachment);

    /// <summary>
    /// Updates the sampling form C record attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    IList<SamplingFormCRecordAttachment> UpdateSamplingFormCRecordAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Determines whether [is duplicate sampling form C record attachment file name] [the specified file name].
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="samplingFormCRecordId">The sampling form C record id.</param>
    /// <returns>
    /// 	<c>true</c> if [is duplicate sampling form C record attachment file name] [the specified file name]; otherwise, <c>false</c>.
    /// </returns>
    bool IsDuplicateSamplingFormCRecordAttachmentFileName(string fileName, Guid samplingFormCRecordId);

    /// <summary>
    /// Gets the rejection memo coupon attachment.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<SamplingFormCRecordAttachment> GetSamplingFormCRecordAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Updates SamplingFormCs
    /// </summary>
    /// <param name="invoiceIdList"></param>
    /// <returns></returns>
    IList<SamplingFormC> UpdateSamplingFormCStatuses(List<Guid> invoiceIdList);

    /// <summary>
    /// Updates SamplingFormC.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    SamplingFormC UpdateSamplingFormCStatus(Guid invoiceId);

    /// <summary>
    /// Used to pre-populate Form C coupon details on UI.
    /// </summary>
    /// <param name="fromMemberId">Member who has raised the form C.</param>
    /// <param name="provisionalBillingMemberId">Billing member of the Form A/B.</param>
    /// <param name="provisionalBillingMonth"></param>
    /// <param name="provisionalBillingYear"></param>
    /// <param name="ticketIssuingAirline"></param>
    /// <param name="documentNumber"></param>
    /// <param name="couponNumber"></param>
    /// <param name="listingCurrency"></param>
    /// <returns></returns>
    LinkedCouponDetails GetLinkedCouponDetails(int fromMemberId,
                                                      int provisionalBillingMemberId,
                                                      int provisionalBillingMonth,
                                                      int provisionalBillingYear,
                                                      string ticketIssuingAirline,
                                                      long documentNumber,
                                                      int couponNumber,
                                                      string listingCurrency);

    /// <summary>
    /// Update form C invoicestatus to 'Processing Complete'.
    /// </summary>
    /// <returns></returns>
    bool UpdateFormCStatusToProcessingComplete();

    /// <summary>
    /// To generate Nil Form C.
    /// </summary>
    /// <param name="provisionalBillingMonth">provisional billing Month</param>
    void GenerateNilFormC(string provisionalBillingMonth, int memberId, Boolean isReprocess);

    /// <summary>
    /// Inserts the generate nil form C message in oracle queue.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    void InsertGenerateNilFormCMessageInOracleQueue(string provisionalBillingMonth);

    /// <summary>
    /// Gets the sampling form C payables list.
    /// </summary>
    /// <param name="searchCriteria">The search criteria.</param>
    /// <returns></returns>
    IList<SamplingFormCResultSet> GetSamplingFormCPayablesList(SearchCriteria searchCriteria);


    /// <summary>
    /// Gets the form AB listing currency.
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    /// <returns></returns>
    int? GetFormABListingCurrency(SamplingFormC samplingFormCHeader);

    /// <summary>
    /// Following method updates FormC SourceCode total and is called when FormC is submitted
    /// </summary>
    /// <param name="formCId">FormC ID</param>
    void UpdateFormCSourceCodeTotal(Guid formCId);


    void UpdateSampleDigit(string provisionalBillingMonth);

    void ValidateSamplingFormCRecord(SamplingFormCRecord samplingFormCRecord, SamplingFormCRecord samplingFormCRecordInDb, SamplingFormC samplingFormC);

    /// <summary>
    /// Gets details of sampling form c headers.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id </param>
    /// <returns>Sampling form c header detail</returns>
    //SCP334940: SRM Exception occurred in Iata.IS.Service.Iata.IS.Service.OfflineCollectionDownloadService. - SIS Production
    SamplingFormC GetSamplingFormCHeaderDetails(string samplingFormCId);
  }
}

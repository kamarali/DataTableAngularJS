using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Data.Pax
{
  public interface ISamplingFormCRepository : IRepository<SamplingFormC>
  {

    /// <summary>
    /// Gets the Sampling Form C Source total List.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="invoiceStatusId">The invoice status id.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <returns>List of Source Total grouped for specified input parameters.</returns>
    IList<SamplingFormCSourceTotal> GetSamplingFormCSourceTotalList(int provisionalBillingMonth, int provisionalBillingYear, int fromMemberId, int invoiceStatusId, int provisionalBillingMemberId, int? listingCurrencyId);

    /// <summary>
    /// Gets the sampling form C details.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    IQueryable<SamplingFormC> GetSamplingFormCDetails(System.Linq.Expressions.Expression<Func<SamplingFormC, bool>> where);


    /// <summary>
    /// Gets the sampling form C list.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="invoiceStatusId">The invoice status id.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <returns></returns>
    IList<SamplingFormCResultSet> GetSamplingFormCList(int provisionalBillingMonth, int provisionalBillingYear, int fromMemberId, int invoiceStatusId, int provisionalBillingMemberId);

    /// <summary>
    /// Get sampling form C details
    /// </summary>
    /// <param name="provisionalBillingMonth">The Provisional billing month</param>
    /// <param name="provisionalBillingYear">The provisional billing year</param>
    /// <param name="fromMemberId">The From member id</param>
    /// <param name="invoiceStatusIds">The invoice status id</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id</param>
    /// <param name="listingCurrencyCodeNum">The Listing Currency Code Num id</param>
    /// <returns>List of samplingFormC</returns>
    IList<SamplingFormC> GetSamplingFormCDetails(int? provisionalBillingMonth = null,
                                                 int? provisionalBillingYear = null,
                                                 int? fromMemberId = null,
                                                 string invoiceStatusIds = null,
                                                 int? provisionalBillingMemberId = null,
                                                 int? listingCurrencyCodeNum = null);

    /// <summary>
    /// Get list of form C 
    /// </summary>
    /// <param name="processingCompletedOnDateTime">processingCompletedOnDateTime</param>
    /// <param name="formCAttachmentCompletedOnDateInd">if equal to 1 it will compare processingCompletedOnDateTime with Form C Attachment Completed On Date</param>
    /// <returns></returns>
    List<SamplingFormC> GetSamplingFormCOfflineCollectionData(DateTime processingCompletedOnDateTime, int? formCAttachmentCompletedOnDateInd = null);

    /// <summary>
    /// Updates the multiple form C status.
    /// </summary>
    /// <param name="formCIds">The form C ids.</param>
    /// <param name="invoiceStatus">The invoice status.</param>
    void UpdateFormCInvoiceStatus(string formCIds, InvoiceStatusType invoiceStatus);

    IList<SamplingFormCResultSet> GetSamplingFormCPayablesList(int provisionalBillingMonth, int provisionalBillingYear, int toMemberId, int provisionalBilledMemberId);

    /// <summary>
    ///  Gets the Sampling form C Data for Output generation having processing completed on date as input parameter
    /// </summary>
    /// <param name="processingCompletedOnDateTime">processingCompletedOnDateTime</param>
    /// <param name="provisionalBillingMemberId">provisionalBillingMemberId</param>
    /// <param name="formCAttachmentCompletedOnDateInd">formCAttachmentCompletedOnDateInd is used if u want to fetch the attachments attached in the given Processing Completed on DateTime</param>
    /// <param name="invoiceStatusId">invoiceStatusId</param>
    /// <returns>List of Sampling form C</returns>
    IList<SamplingFormC> GetSamplingFormCDataForOutputGeneration(DateTime processingCompletedOnDateTime, int provisionalBillingMemberId, int? formCAttachmentCompletedOnDateInd = null, string invoiceStatusId = null);

    /// <summary>
    /// Returns the count of Form Cs of provisionalBillingMember in the period 
    /// </summary>
    /// <param name="processingCompletedOnDateTime"></param>
    /// <param name="provisionalBillingMemberId"></param>
    /// <returns></returns>
    List<SamplingFormC> GetOnlySamplingFormcForOutputfileGeneration(DateTime processingCompletedOnDateTime, int provisionalBillingMemberId);

    List<SamplingFormC> SystemMonitorGetOnlySamplingFormcForOutputfileGeneration(DateTime processingCompletedOnDateTime,
                                                                                 int provisionalBillingMemberId);

    /// <summary>
    /// Following method updates FormC SourceCode total. Method is executed when user submits FormC
    /// </summary>
    /// <param name="formCId">Form C Id</param>
    void UpdateFormCSourceCodeTotal(Guid formCId);

    /// <summary>
    /// Load Data to generate Form C Offline Archive
    /// </summary>
    /// <param name="processingCompletedOnDateTime">processingCompletedOnDateTime</param>
    /// <param name="formCAttachmentCompletedOnDateInd">formCAttachmentCompletedOnDateInd</param>
    /// <param name="memberId">memberId</param>
    /// <param name="isProvisional">If set then member is Provisional member else member is From member</param>
    /// <returns></returns>
    List<SamplingFormC> GetFormCDataForOfflineArchive(DateTime processingCompletedOnDateTime, int? formCAttachmentCompletedOnDateInd = null, int? memberId = null, bool isProvisional = false);
  
    
  }
}

using System;
using System.Collections.Generic;
using Devart.Data.Oracle;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.SupportingDocuments;
using log4net;

namespace Iata.IS.Business.Common
{
  public interface IInvoiceOfflineCollectionManager
  {
    void SetSuccessfullOfflineCollectionStausForInvoice(string invoiceId);

    /// <summary>
    /// This function is used to re-queue message for offline line collection and send mail to sis ops if retry count maximum
    /// </summary>
    /// <param name="invoiceStatusId"></param>
    /// <param name="invoiceId"></param>
    /// <param name="billingCategoryId"></param>
    /// <param name="isReprocess"></param>
    /// <param name="ex"></param>
    //SCP#391022 KAL: Optimization in offline collection.
    void RequeueMessage(int invoiceStatusId, string invoiceId, int billingCategoryId, int isReprocess, Exception ex);
    /// <summary>
    /// Creates the offline collection.
    /// </summary>
    /// <param name="invoiceOfflineCollectionMessage">The invoice offline collection meta data.</param>
    void CreateOfflineCollection(InvoiceOfflineCollectionMessage invoiceOfflineCollectionMessage, InvoiceBase invoice);

    /// <summary>
    /// Creates the offline collection for Form C
    /// </summary>
    /// <param name="rootPath">rootPath</param>
    /// <param name="logger">logger</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="memberId"></param>
    /// <param name="isProvisional"></param>
    void CreateFormCOfflineCollection(string rootPath, ILog logger, BillingPeriod billingPeriod, int memberId, bool isProvisional);

    /// <summary>
    /// En-queues the invoice download request to the system for background processing.
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    bool EnqueueDownloadRequest(IDictionary<string, string> messages);

    /// <summary>
    /// This function will create Form-C offlineCollection for Web click event
    /// </summary>
    /// <param name="logger">logger</param>
    /// <param name="provisionalBillingMonth">provisionalBillingMonth of sampling form C</param>
    /// <param name="provisionalBillingYear">provisionalBillingYear of sampling form C</param>
    /// <param name="fromMemberId">fromMemberId of sampling form C</param>
    /// <param name="invoiceStatusIds">invoiceStatusIds of sampling form C</param>
    /// <param name="provisionalBillingMemberId">provisionalBillingMemberId of sampling form C</param>
    /// <param name="listingCurrencyCodeNum">listingCurrencyCodeNum of sampling form C</param>
    /// <param name="offlinerootFolderPath">offlinerootFolderPath of sampling form C</param>
    /// <returns>List InvoiceOfflineCollectionMetaData</returns>
    List<InvoiceOfflineCollectionMetaData> GenerateFormCofflineCollectionForWeb(ILog logger,
                                                                                int provisionalBillingMonth,
                                                                                int provisionalBillingYear,
                                                                                int fromMemberId,
                                                                                string invoiceStatusIds,
                                                                                int provisionalBillingMemberId,
                                                                                int listingCurrencyCodeNum,
                                                                                string offlinerootFolderPath);

    /// <summary>
    /// To link unlinked documents for invoice
    /// </summary>
    /// <param name="invoiceBase"></param>
    /// <param name="skipSuppDocLinkingDeadlineCheck">Indicated whether called from Finalization process</param>
    //SCP133627 - LP/544-Mismatch in CRSupporting File
    void LinkUnlinkedDocumentsForInvoice(InvoiceBase invoiceBase, bool skipSuppDocLinkingDeadlineCheck = false);

    // SCP162502  Form C - AC OAR Jul P3 failure - No alert received
    void LinkUnlinkedDocumentsForFormC(UnlinkedSupportingDocument unlinkedSupportingDocument, bool skipSuppDocLinkingDeadlineCheck = false);

    // SCP#369538 - SRM: Daily ouputs are slow - Delivered on 16-May-2015.
    // Removed invOfflineColData cursor fetching. Instead path is build in C# code itself.
    string CreateBaseFolderStructure(string rootPath, string billingMember, string billedMember,
                                       BillingCategoryType billingCategory,
                                       BillingPeriod billingPeriod, string invoiceNumber, ILog logger, bool isFormC,
                                       int baseFolderlNameYear = 0, int baseFolderlNameMonth = 0,
                                       int baseFolderlNamePeriod = 0, bool requestViaIsWeb = false, bool getSANBasePathFromDatabase = true);

      /// <summary>
      /// This function is used to en-queue message for offline collection download
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="billingPeriod"></param>
      /// <param name="billingMonth"></param>
      /// <param name="billingYear"></param>
      /// <param name="invoiceStatusId"></param>
      /// <param name="billingCategory"></param>
      /// <param name="delays"></param>
      /// SCP419599: SIS: Admin Alert - Error in creating Legal Invoice Archive zip file of CDC - SISPROD -16oct2016
      void EnqueueOfflineCollectionDownload(int memberId, int billingPeriod, int billingMonth, int billingYear,
                                            int invoiceStatusId, string billingCategory, int delays);
  }
}

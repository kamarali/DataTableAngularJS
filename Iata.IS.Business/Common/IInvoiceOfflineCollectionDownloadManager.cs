using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.Common
{
  public interface IInvoiceOfflineCollectionDownloadManager
  {
    /// <summary>
    /// Gets the member invoices offline collection
    /// </summary>
    /// <param name="member">member</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="options">options</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="isFormC">isFormC</param>
    /// <param name="invoiceStatus"></param>
    //SCP132419 - SRM: Duplicate OAR's generated May P3.
    List<string> GetMemberInvoicesOfflineCollectionZip(Member member, BillingPeriod billingPeriod, bool isReceivable,List<string> options, BillingCategoryType billingCategoryType,bool isFormC, int invoiceStatus, bool checkOARGenerated = false);

    /// <summary>
    /// Gets the invoice offline collection.
    /// </summary>
    /// <param name="userId">Login user id</param>
    /// <param name="zipFileName">Name of the zip file.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="options">The options.</param>
    /// <param name="downloadUrl">url to download</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="isFormC">isFormC</param>
    /// <returns>Returns the output file path</returns>
    string GetInvoiceOfflineCollectionZip(int userId, string zipFileName, Guid invoiceId, List<string> options, string downloadUrl, bool isReceivable, bool isFormC);

    /// <summary>
    /// Send and email alert to member contacts
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="billingPeriod"></param>
    /// <returns></returns>
    bool SendOutputAvailableAlert(int memberId, BillingPeriod billingPeriod);

    /// <summary>
    /// Sends failure alert to IsAdmin
    /// </summary>
    /// <param name="memberCode">memberCode</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="errorMessage">errorMessage</param>
    /// <param name="isFormC">isFormC</param>
    void SendFailureAlert(string memberCode, BillingPeriod billingPeriod, BillingCategoryType billingCategoryType, string errorMessage,bool isFormC);

   /// <summary>
    /// CMP529
    /// Creates the member Daily invoices offline collection zip file.
    /// and returns the output file name on the FTP server
    /// </summary>
    /// <param name="invoiceBaseList">invoiceBaseList</param>
    /// <param name="member">member</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="targetDate">If Daily OAR required then target date is required.</param>
    /// <param name="isReprocessing">flag for reprocessing.</param>
    /// <param name="isFileExist">check if file is already exist</param>
    /// <param name="locationId">CMP622: true if location specific file is required.</param>
    /// <param name="isLocationSpecNilFile">CMP622: true if location specific file is required.</param>
    /// SCP279970: OAR Optimization

    // SCP#369538 - SRM: Daily ouputs are slow - Delivered on 16-May-2015.
    // Removed invOfflineColData cursor fetching. Instead path is build in C# code itself.
    string GetMemberDailyInvoicesOfflineCollectionZip(IQueryable<InvoiceBase> invoiceBaseList, Member member, BillingCategoryType billingCategoryType, DateTime targetDate,bool isReprocessing, out bool isFileExist, string locationId = null);


    /// <summary>
    /// Create base folder as per specification in IS File Specs
    /// </summary>
    /// <param name="member">member</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="isFormC">isFormC</param>
    /// <param name="formCProvMonth">Provisional month of Form C</param>
    /// <param name="formCProvYear">Form C Provisional Year</param>
    /// <param name="isZipFileName">Note : If this flag is set, method will return Zip file name else it will return Folder name</param>
    /// <param name="isWebZip">Set this flag to true for Web zip icon click</param>
    /// <param name="zipFileName">Pass this value only for Web click : Will be used to create unique base folder for two invoices of same member</param>
    /// <param name="targetDate">CMP529: If Daily OAR required then target date is required.</param>
    /// <param name="isDailyOARRequired">CMP529: If Dail OAR required then this flag will be true.</param>
    /// <param name="nilFile">CMP529: If append "-NODATA.TXT" in zipfile name.</param>
    /// <param name="locationId">Location ID for location specific OAR</param>
    /// <param name="isLocationSpecNilFile">if nil file for location specific is required.</param>
    /// <returns>Returns the offline archive parent folder path</returns>
    string GetParentFolderName(Member member, BillingCategoryType billingCategoryType, BillingPeriod billingPeriod, bool isReceivable, bool isFormC, int formCProvMonth, int formCProvYear, bool isZipFileName, bool isWebZip = false, string zipFileName = null, DateTime? targetDate = null, bool isDailyOARRequired = false, bool nilFile = false, string locationId = null, bool isLocationSpecNilFile = false);

    /// <summary>
    /// Check File is already exist.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    bool IsFileExist(string filePath);

      #region CMP622 : LOCATION OAR
    /// <summary>
    /// Gets the member location invoices offline collection zip.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="isReceivable">if set to <c>true</c> [is receivable].</param>
    /// <param name="options">The options.</param>
    /// <param name="billingCategoryType">Type of the billing category.</param>
    /// <param name="memberLocationCode">The member location code.</param>
    /// <param name="isNilfileRequired">if set to <c>true</c> [is nilfile required].</param>
    /// <param name="checkOARGenerated">if set to <c>true</c> [check OAR generated].</param>
    /// <param name="isRegeneration">if set to <c>true</c> [is regeneration].</param>
    /// <returns></returns>
    List<string> GetMemberLocationInvoicesOfflineCollectionZip(Member member, BillingPeriod billingPeriod, bool isReceivable, List<string> options, BillingCategoryType billingCategoryType, string memberLocationCode, bool isNilfileRequired, bool checkOARGenerated = false, bool isRegeneration = false);
      #endregion

      /// <summary>
      /// This function is used to get invoice listing PDF folder path from invoice offline collection table.
      /// CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
      /// </summary>
      /// <param name="invoiceId"></param>
      /// <returns>return listing PDF path.</returns>
      string GetInvoiceListingPdfPath(Guid invoiceId);
  }
}
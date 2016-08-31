using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Business.MiscUatp
{
  public interface IOutputXmlGeneratorManager
  {
    /// <summary>
    /// Gets Misc invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria">Search Criteria</param>
    /// <returns>Invoices matching the search criteria.</returns>
   List<MiscUatpInvoice> GetMiscInvoices(SearchCriteria searchCriteria);

    /// <summary>
    /// Gets Misc Is Web invoices matching the specified search criteria
    /// This is used by exe.
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="invoiceStatusIds"></param>
    /// <param name="billingCategoryId"></param>
    /// <param name="isWebGenerationDate"></param>
    /// <param name="isReprocessing"></param>
    /// <param name="outputType"></param>
    /// <param name="locationId"></param>
    /// <returns></returns>
    List<MiscUatpInvoice> GetMiscIsWebInvoices(int? billingMemberId = null, string invoiceStatusIds = null,
                                               int? billingCategoryId = null, DateTime? isWebGenerationDate = null,
                                               int? isReprocessing = null, int? outputType = null,
                                               string locationId = null);
   
    /// <summary>
    /// Gets MiscUatp invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
   List<MiscUatpInvoice> GetMiscUatpInvoices(SearchCriteria searchCriteria);

    /// <summary>
    /// Send Email Notification if User is not opted for Xml output.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
   bool SendEmailNotification(int memberId, ProcessingContactType processingContact, string billingCategory);

   //CMP#622 : MISC Outputs Split as per Location ID
   List<MiscUatpInvoice> GetOnBehalfMiscInvoices(OnBehalfInvoiceSetup onBehalfInvoiceSetup, BillingPeriod billingPeriod, int billingMemberId, int outputType);

    /// <summary>
    /// This will send the SIS Admin alert for pending processes
    /// </summary>
    /// <param name="memberCode">memberCode</param>
    /// <param name="errorMessage">errorMessage</param>
    /// <param name="billingPeriod">billingPeriod</param>
    void SendSisAdminAlert(string memberCode, string errorMessage, BillingPeriod billingPeriod);

    /// <summary>
    /// This will send email to SIS Admin if Weekly Invoice Posting Report is not created in case of AutoBilling
    /// </summary>
    /// <param name="memberCode">memberCode</param>
    /// <param name="billingPeriod">billingPeriod</param>
    void SendSisAdminAlertAutoBilling( string memberCode,  BillingPeriod billingPeriod);

    /// <summary>
    /// This will send email to SIS Admin if Daily Revenue Recognition Report is not created in case of AutoBilling
    /// </summary>
    /// <param name="memberCode">memberCode</param>
    /// <param name="billingPeriod">billingPeriod</param>
    void SendSisAdminAlertDailyRevenueRecognitionReport(string memberCode, BillingPeriod billingPeriod);

    /// <summary>
    /// This will send email to SIS Admin if Daily AutoBilling Irregularity Report is not created in case of AutoBilling
    /// </summary>
    /// <param name="memberCode">memberCode</param>
    void SendSisAdminAlertDailyIrregularityReport(string memberCode);

    /// <summary>
    /// This will send email to SIS Admin if Daily AutoBilling Irregularity Report is not created if no Coupons are found.
    /// </summary>
    /// <param name="emailAddress"></param>
    void SendSisAdminAlertIrregularityReportRegenerationStatus(IEnumerable<string> emailAddress);

    /// <summary>
    /// This will form and return the appropriate error message
    /// </summary>
    /// <param name="miscUatpInvoiceBases">miscUatpInvoiceBases</param>
    /// <returns>error message</returns>
    string GetErrorMessage(IEnumerable<InvoiceBase> miscUatpInvoiceBases);


    List<MiscUatpInvoice> SystemMonitorGetOnBehalfMiscInvoices(OnBehalfInvoiceSetup onBehalfInvoiceSetup,
                                                               BillingPeriod billingPeriod, int billingMemberId, string invoiceStatus);

    /// <summary>
    /// CMP529: Method added to get all invoices for daily output file generation.
    /// </summary>
    /// <param name="billedMemberId">billed member id</param>
    /// <param name="targetDate">delivery date</param>
    /// <param name="dailyDeliveryStatusId">delivery status</param>
    /// <param name="invoiceStatusIds">status of invoice</param>
    /// <param name="billingCategoryId">billing category for invoice</param>
    /// <param name="islocationSpec">CMP#622 Changes: true if location spec invoices required</param>
    /// <param name="locationId">CMP#622 Changes: location code for location spec invoices required</param>
    /// <returns>list invoices</returns>
    List<MiscUatpInvoice> GetMiscInvoicesForDailyXmlOutput(int billedMemberId, DateTime targetDate,
                                                                  int? dailyDeliveryStatusId, string invoiceStatusIds, int billingCategoryId, int? islocationSpec = null, string locationId = null);

    //CMP#622 : MISC Outputs Split as per Location ID
    /// <summary>
    /// Gets the location specific on behalf misc invoices.
    /// </summary>
    /// <param name="onBehalfInvoiceSetup">The on behalf invoice setup.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="islocationSpec">The islocation spec.</param>
    /// <param name="locationId">The location id.</param>
    /// <param name="isReprocessing">if set to <c>true</c> [is reprocessing].</param>
    /// <returns></returns>
    List<MiscUatpInvoice> GetLocationSpecificOnBehalfMiscInvoices(OnBehalfInvoiceSetup onBehalfInvoiceSetup, BillingPeriod billingPeriod, int billingMemberId, int? islocationSpec = null, string locationId = null, bool isReprocessing = false);
      
  }
}

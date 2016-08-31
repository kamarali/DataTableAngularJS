using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using System.Linq;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.MiscUatp.BillingHistory;

namespace Iata.IS.Business.MiscUatp
{
  /// <summary>
  /// Base manager interface for MISC and UATP.
  /// </summary>
  public interface IMiscUatpInvoiceManager
  {
    /// <summary>
    /// Searches the invoice for given search criteria.
    /// </summary>
    /// <param name="searchCriteria">The search criteria.</param>
    /// <returns></returns>
      IList<MiscInvoiceSearch> SearchInvoice(MiscSearchCriteria searchCriteria);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <param name="pageNo"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    IQueryable<MiscInvoiceSearchDetails> SearchInvoiceMisc(MiscSearchCriteria searchCriteria, int pageNo, int pageSize, string sortColumn, string sortOrder);

    /// <summary>
    /// Searches the payable invoices for given search criteria.
    /// </summary>
    /// <param name="searchCriteria">The search criteria.</param>
    /// <returns></returns>
    IList<MiscInvoiceSearch> SearchPayableInvoices(MiscSearchCriteria searchCriteria);

    /// <summary>
    /// Creates the misc. invoice.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc invoice.</param>
    /// <returns></returns>
    MiscUatpInvoice CreateInvoice(MiscUatpInvoice miscUatpInvoice);

    /// <summary>
    /// Creates the misc. invoice.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc invoice.</param>
    /// <returns></returns>
    MiscUatpInvoice CreateBHRejectionInvoice(MiscUatpInvoice miscUatpInvoice, string lineItemIds);

    /// <summary>
    /// Updates the misc. invoice.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc invoice.</param>
    /// <returns></returns>
    MiscUatpInvoice UpdateInvoice(MiscUatpInvoice miscUatpInvoice);

    /// <summary>
    /// Deletes the invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    bool DeleteInvoice(string invoiceId);

    /// <summary>
    /// Gets the invoice attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    IList<MiscUatpAttachment> GetInvoiceAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// adds the invoice attachment.
    /// </summary>
    /// <param name="attachment">The attachment.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="isUpdateAttachmentIndOrig">if set to <c>true</c> [is update attachment ind orig].</param>
    /// <returns></returns>
    MiscUatpAttachment AddInvoiceAttachment(MiscUatpAttachment attachment, MiscUatpInvoice invoice, bool isUpdateAttachmentIndOrig);

    /// <summary>
    /// Updates the invoice attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    IList<MiscUatpAttachment> UpdateInvoiceAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Gets the invoice attachment detail.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    MiscUatpAttachment GetInvoiceAttachmentDetail(string attachmentId);

    MiscUatpInvoice GetOriginalInvoiceDetail(string rejectedInvoiceNumber, int billingMemberId);
    /// <summary>
    /// Determines whether specified file name already exists for given invoice.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// true if specified file name found in repository; otherwise, false.
    /// </returns>
    bool IsDuplicateInvoiceAttachmentFileName(string fileName, Guid invoiceId);

    /// <summary>
    /// Validates the invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    MiscUatpInvoice ValidateInvoice(string invoiceId);

    /// <summary>
    /// Submits the invoice.
    /// </summary>
    /// <param name="invoiceIdList">The invoice id list.</param>
    /// <returns></returns>
    IList<MiscUatpInvoice> SubmitInvoices(List<string> invoiceIdList);

    /// <summary>
    /// Submits the invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    MiscUatpInvoice SubmitInvoice(string invoiceId);

    /// <summary>
    /// Marks the status of all invoices specified in the list to Processing Complete.
    /// </summary>
    /// <param name="invoiceIdList"></param>
    /// <returns></returns>
    IList<MiscUatpInvoice> ProcessingCompleteInvoices(List<string> invoiceIdList);

    /// <summary>
    /// Marks the specified invoice status to Processing Complete.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    MiscUatpInvoice ProcessingCompleteInvoice(string invoiceId);

    /// <summary>
    /// Marks the status of all invoices specified in the list to Presented.
    /// </summary>
    /// <param name="invoiceIdList"></param>
    /// <returns></returns>
    IList<MiscUatpInvoice> PresentInvoices(List<string> invoiceIdList);

    /// <summary>
    /// Marks the specified invoice status to Presented.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    MiscUatpInvoice PresentInvoice(string invoiceId);

    /// <summary>
    /// Updates the member location information.
    /// </summary>
    /// <param name="memberLocationInformation">The member location information.</param>
    /// <param name="isBillingMember">if set to true member is billing member.</param>
    /// <returns></returns>
    MemberLocationInformation UpdateMemberLocationInformation(MemberLocationInformation memberLocationInformation, bool isBillingMember, MiscUatpInvoice invoice, bool? commitChanges);

    /// <summary>
    /// Get Dynamic field metadata for given combination of Charge code and Charge code type
    /// </summary>
    /// <param name="chargeCodeId"></param>
    /// <param name="chargeCodeTypeId"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    IList<FieldMetaData> GetFieldMetadata(int chargeCodeId, Nullable<int> chargeCodeTypeId, Guid? lineItemDetailId, Int32 billingCategoryId);

    /// <summary>
    /// Get list of dictionary based values for field of type dropdown 
    /// </summary>
    /// <param name="dataSourceId"></param>
    /// <returns></returns>
    IList<DropdownDataValue> GetDataSourceValues(int dataSourceId);

    /// <summary>
    /// Gets the invoice detail.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    MiscUatpInvoice GetInvoiceDetail(string invoiceId);

    /// <summary>
    /// Gets the invoice header information.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>Invoice object with only header details populated.</returns>
    MiscUatpInvoice GetInvoiceHeader(string invoiceId);


    bool IsRejectionInvoiceExist(string invoiceId);

    /// <summary>
    /// Determines whether RejectionInvoice exists with any Status
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <returns>True if Rejection Invoice exists else false</returns>
    bool IsRejectionInvoiceExistsWithAnyStatus(string invoiceId);

    /// <summary>
    /// Adds the line item.
    /// </summary>
    /// <param name="lineItem">The line item.</param>
    /// <returns></returns>
    LineItem AddLineItem(LineItem lineItem);

    /// <summary>
    /// Updates the line item.
    /// </summary>
    /// <param name="lineItem">The line item.</param>
    /// <returns></returns>
    LineItem UpdateLineItem(LineItem lineItem);

    /// <summary>
    /// Gets the line item information.
    /// </summary>
    /// <param name="lineItemId">The line item id.</param>
    /// <returns></returns>
    LineItem GetLineItemInformation(string lineItemId);

    /// <summary>
    /// Gets the line item header information.
    /// </summary>
    /// <param name="lineItemId">The line item id.</param>
    /// <returns></returns>
    LineItem GetLineItemHeaderInformation(string lineItemId);

    /// <summary>
    /// Gets the line item list.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    IList<LineItem> GetLineItemList(string invoiceId);

    /// <summary>
    /// Deletes the line item.
    /// </summary>
    /// <param name="lineItemId">The line item id.</param>
    /// <returns></returns>
    bool DeleteLineItem(string lineItemId);

    /// <summary>
    /// Adds the line item detail.
    /// </summary>
    /// <param name="lineItemDetail">The line item detail.</param>
    /// <param name="fieldMetadata">Field metadata.</param>
    /// <returns></returns>
    LineItemDetail AddLineItemDetail(LineItemDetail lineItemDetail, IList<FieldMetaData> fieldMetadata);

    /// <summary>
    /// Updates the line item detail.
    /// </summary>
    /// <param name="lineItemDetail">The line item detail.</param>
    /// <param name="fieldMetaData">Field metadata.</param>
    /// <returns></returns>
    LineItemDetail UpdateLineItemDetail(LineItemDetail lineItemDetail, IList<FieldMetaData> fieldMetaData);

    /// <summary>
    /// Gets the line item detail information.
    /// </summary>
    /// <param name="lineItemDetailId">The line item detail id.</param>
    /// <returns></returns>
    LineItemDetail GetLineItemDetailInformation(string lineItemDetailId);

    /// <summary>
    /// Deletes the line item detail.
    /// </summary>
    /// <param name="lineItemDetailId">The line item detail id.</param>
    /// <returns></returns>
    bool DeleteLineItemDetail(string lineItemDetailId);

    /// <summary>
    /// Gets the line item detail list.
    /// </summary>
    /// <param name="lineItemId">The line item id.</param>
    /// <returns></returns>
    IList<LineItemDetail> GetLineItemDetailList(string lineItemId);

    /// <summary>
    /// Gets the member reference data.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="isBillingMember">if set to true [is billing member].</param>
    /// <param name="locationCode">The location code.</param>
    /// <returns></returns>
    MemberLocationInformation GetMemberReferenceData(string invoiceId, bool isBillingMember, string locationCode);

    /// <summary>
    /// This will return Country id of the country matching given country code
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    string GetCountryId(string countryCode);

    /// <summary>
    /// This will return the currency object matching given currency code
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <returns></returns>
    Currency GetCurrency(string currencyCode);

    /// <summary>
    /// This will return ChargeCategory matching with given chargeCategoryName
    /// </summary>
    /// <param name="chargeCategoryName"></param>
    /// <param name="billingCategoryId"></param>
    /// <returns></returns>
    ChargeCategory GetChargeCategory(string chargeCategoryName, int billingCategoryId);

    /// <summary>
    /// This will return ChargeCode matching with given chargeCodeName
    /// </summary>
    /// <param name="chargeCodeName"></param>
    /// <param name="chargeCategoryId"></param>
    /// <returns></returns>
    ChargeCode GetChargeCode(string chargeCodeName, int chargeCategoryId);

    /// <summary>
    /// This will return ChargeCodeType matching with given chargeCodeTypeName
    /// </summary>
    /// <param name="chargeCodeTypeName"></param>
    /// <returns></returns>
    ChargeCodeType GetChargeCodeType(string chargeCodeTypeName);

    // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
    /// <summary>
    /// This will return ChargeCodeType matching with given charge code type name and chargeCodeId
    /// </summary>
    /// <param name="chargeCodeTypeName">Charge Code Type Name</param>
    /// <param name="chargeCodeId">Charge Code Id</param>
    /// <returns>Charge Code Type for given Charge Code Type Name and Charge Code Id</returns>
    ChargeCodeType GetChargeCodeTypeOnChargeCodeId(string chargeCodeTypeName, int? chargeCodeId);
    // CMP # 533: RAM A13 New Validations and New Charge Code [End]

    /// <summary>
    /// Get UomCode with given UomCode name
    /// </summary>
    /// <param name="uomCodeName"></param>
    /// <returns></returns>
    UomCode GetUomCode(string uomCodeName);

    /// <summary>
    /// Determines whether field meta data exists for specified charge code.
    /// </summary>
    /// <param name="chargeCodeid">The charge code id.</param>
    /// <param name="chargeCodeTypeId">The charge code type id.</param>
    /// <returns>
    /// true if field meta data exists for specified charge code; otherwise, false.
    /// </returns>
    bool IsFieldMetaDataExists(int chargeCodeid, int? chargeCodeTypeId, int billingCategoryId);

    /// <summary>
    /// Determines whether field meta data exists for specified charge code.
    /// Note : This is a oveload method created to use the Field metadata stored in the validation cache during Parsing 
    /// </summary>
    /// <param name="chargeCodeid">The charge code id.</param>
    /// <param name="chargeCodeTypeId">The charge code type id.</param>
    /// <param name="miscUatpInvoice">misc uatp invoice</param>
    /// <returns>
    /// true if field meta data exists for specified charge code; otherwise, false.
    /// </returns>
    bool IsFieldMetaDataExists(int chargeCodeid, int? chargeCodeTypeId, MiscUatpInvoice miscUatpInvoice);

    /// <summary>
    /// This will return the all the metadata in the database 
    /// </summary>
    /// <returns></returns>
    List<FieldMetaData> GetFieldMetadata();

    /// <summary>
    /// Gets the billing period.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <returns></returns>
    BillingPeriod GetInvoiceBillingPeriod(string invoiceNumber, int billingMemberId, int billedMemberId);

    /// <summary>
    /// Gets the rejection correspondence detail.
    /// </summary>
    /// <param name="correspondenceRefNo">The correspondence ref no.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="isUpdateOperation">True if update operation, false otherwise.</param>
    /// <returns></returns>
    CorrespondenceInvoiceDetails GetRejectionCorrespondenceDetail(long correspondenceRefNo, int billingMemberId, int billedMemberId, Guid invoiceId, bool isUpdateOperation);

    /// <summary>
    /// Get dynamic field values in hierarchy of group-fields-attributes to save in DB
    /// </summary>
    /// <param name="uiFieldValues"></param>
    /// <param name="chargeCodeId"></param>
    /// <param name="chargeCodeTypeId"></param>
    /// <returns></returns>
    List<FieldValue> SetFieldValueForLineItemDetail(List<FieldValue> uiFieldValues, int chargeCodeId, Nullable<int> chargeCodeTypeId, Guid lineItemDetailId);

    /// <summary>
    /// Determines whether line item exists for specified invoice id].
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// true if line item exists for the specified invoice id; otherwise, false.
    /// </returns>
    bool IsLineItemExists(string invoiceId);

    /// <summary>
    /// Determines whether [is line item detail exists] [the specified line item id].
    /// </summary>
    /// <param name="lineItemId">The line item id.</param>
    /// <returns>
    /// 	<c>true</c> if [is line item detail exists] [the specified line item id]; otherwise, <c>false</c>.
    /// </returns>
    bool IsLineItemDetailExists(string lineItemId);

    /// <summary>
    /// Update Sampling Form D attachment record parent id
    /// </summary>
    /// <param name="attachments">list of attachment</param>
    /// <param name="parentId">billing memo Id</param>
    /// <returns></returns>
    IQueryable<MiscUatpAttachment> UpdateAttachments(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Gets the field metadata for group.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <param name="chargeCodeTypeId">The charge code type id.</param>
    /// <param name="groupId">The group id.</param>
    /// <param name="isOptionalGroup"></param>
    /// <returns></returns>
    FieldMetaData GetFieldMetadataForGroup(int chargeCodeId, int? chargeCodeTypeId, Guid groupId, bool isOptionalGroup);

    /// <summary>
    /// Deletes the attachment with the given ID.
    /// </summary>
    /// <param name="attachmentId">ID of the attachment to delete.</param>
    /// <param name="isSupportingDoc">Flag is set to true if Supporting document is deleted else set to false</param>
    /// <returns>Flag indicating the success of delete operation.</returns>
    bool DeleteAttachment(string attachmentId, bool isSupportingDoc);

    /// <summary>
    /// Gets attachments for an invoice.
    /// </summary>
    /// <param name="invoiceId">Invoice ID.</param>
    /// <returns></returns>
    IList<MiscUatpAttachment> GetAttachments(string invoiceId);


    long GetLineItemDetailsCount(string lineItemId);


    /// <summary>
    /// Determines whether [is location code present] [the specified invoice id].
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// 	<c>true</c> if [is location code present] [the specified invoice id]; otherwise, <c>false</c>.
    /// </returns>
    long IsLocationCodePresent(string invoiceId);

    NavigationDetails GetNavigationDetails(string lineItemDetailId, string lineItemId);

    /// <summary>
    /// Gets the max line item detail number.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    int GetMaxLineItemNumber(Guid invoiceId);

    /// <summary>
    /// Gets the max line item detail number.
    /// </summary>
    /// <param name="lineItemId">The line item id.</param>
    /// <returns></returns>
    int GetMaxLineItemDetailNumber(Guid lineItemId);

    LineItemDetail GetLineItemDetailHeaderInformation(Guid lineItemId, int detailNumber);

    /// <summary>
    /// Fetch data for optional groups to populate optional field dropdown
    /// </summary>
    /// <param name="chargeCodeId"></param>
    /// <param name="chargeCodeTypeId"></param>
    /// <returns></returns>
    List<DynamicGroupDetail> GetOptionalGroupDetails(int chargeCodeId, Nullable<int> chargeCodeTypeId);

    /// <summary>
    /// Gets the derived vat details for an Invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of derived vat details for the Invoice.</returns>
    IList<MiscDerivedVatDetails> GetDerivedVatDetails(string invoiceId);

    /// <summary>
    /// Retrieves Rejected invoice number for the billed member and validates against settlement method.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="smi">The settlement method id.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="settlementMonth">Settlement Month.</param>
    /// <param name="settlementYear">Settlement Year</param>
    /// <param name="settlementPeriod">Settlement Period</param>
    /// <returns>
    /// Billing rejected invoice billing period if found
    /// </returns>
    RejectedInvoiceDetails GetRejectedInvoiceDetails(string invoiceNumber, int smi, int billingMemberId, int billedMemberId, int? settlementMonth, int? settlementYear, int? settlementPeriod);

    /// <summary>
    /// Checks whether invoices are blocked due to some pending processes
    /// </summary>
    /// <param name="muInvoiceBases"></param>
    /// <returns></returns>
    bool ValidateMiscUatpInvoices(IEnumerable<InvoiceBase> muInvoiceBases);

    /// <summary>
    /// Determines whether transaction is out side time limit for specified invoice].
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <returns>
    /// true if transaction in not out side time limit for the specified invoice; otherwise, false.
    /// </returns>
    bool IsTransactionOutSideTimeLimit(MiscUatpInvoice invoice);

    /// <summary>
    /// Gets the rejection error message to be displayed when user initiates rejection through Payables.
    /// </summary>
    /// <param name="invoiceId">Invoice id of invoice being rejected.</param>
    /// <param name="isCreditNoteRejection"></param>
    /// <returns>The error/warning message.</returns>
    string GetRejectionErrorMessage(string invoiceId, out bool isCreditNoteRejection);

    /// <summary>
    /// This will return ChargeCode matching with given chargeCodeName
    /// </summary>
    /// <param name="chargeCodeName"></param>
    /// <param name="chargeCategoryId"></param>
    /// <returns></returns>
    ChargeCode GetChargeCode(string chargeCodeName);

    /// <summary>
    /// This will return ChargeCode matching with given chargeCodeId
    /// </summary>
    /// <param name="chargeCodeName"></param>
    /// <param name="chargeCategoryId"></param>
    /// <returns></returns>
    ChargeCode GetChargeCode(int chargeCodeId);

    MiscUatpInvoice GetOriginalInvoice(MiscUatpInvoice correspondenceInvoice, MiscCorrespondence miscCorrespondence);

    /// <summary>
    /// This method will validate MiscUatp Bm linking process
    /// </summary>
    /// <param name="invoiceHeader">MiscUatpInvoice</param>
    /// <returns></returns>
    bool ValidateMiscUatCorrespondenceLinking(MiscUatpInvoice invoiceHeader);

    /// <summary>
    /// This method will validate MiscUatp Rm linking process
    /// </summary>
    /// <param name="invoiceHeader">MiscUatpInvoice</param>
    /// <returns></returns>
    bool ValidateMiscUatpRmLinking(MiscUatpInvoice invoiceHeader);

    /* SCP 250695: Correspondence Invoice raised is in Ready for Billing status and is visible to both the airline on Audit-trail.
    * Description: Added memberId parameter to be used from IS-Web
    */

    List<MiscUatpInvoice> GetBillingHistoryAuditTrail(string invoiceId, int memberId = 0);

    /// <summary>
    /// Method to generate Misc Billing history Audit trail PDF
    /// </summary>
    /// <param name="auditTrail">Audit trail object on which pdf is to be generated</param>
    /// <param name="currentMemberId">Current session member</param>
    /// <param name="areaName"> Current AreaName</param>
    /// <returns>Audit trail Html string</returns>
    string GenerateMiscBillingHistoryAuditTrailPdf(AuditTrailPdf auditTrail, int currentMemberId, string areaName);

    /// <summary>
    /// Get Billing History Search Result
    /// </summary>
    /// <param name="invoiceCriteria">Invoice search criteria</param>
    /// <param name="billingCategoryId">Billing category</param>
    /// <returns>List of billing history</returns>
    IQueryable<MiscBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceCriteria, int billingCategoryId);

    /// <summary>
    /// Get billing history for correspondence
    /// </summary>
    /// <param name="corrCriteria">Correspondence search criteria</param>
    /// <param name="billingCategoryId">Billing category</param>
    /// <returns>List of billing history</returns>
    IQueryable<MiscBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria corrCriteria, int billingCategoryId);

    /// <summary>
    /// Get correspondence for Trail Report
    /// </summary>
    /// <param name="corrCriteria"></param>
    /// <param name="billingCategoryId"></param>
    /// <returns></returns>
    IQueryable<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(
      CorrespondenceTrailSearchCriteria corrCriteria, int billingCategoryId);
    /// <summary>
    /// Validates the correspondence time limit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="correspondenceStatusId">The correspondence status id.</param>
    /// <param name="authorityToBill">if set to true [authority to bill].</param>
    /// <param name="correspondenceDate">The correspondence date.</param>
    /// <returns>
    /// true if [is valid correspondence time limit] [the specified invoice id]; otherwise, false.
    /// </returns>
    bool IsCorrespondenceOutSideTimeLimit(string invoiceId, int correspondenceStatusId, bool authorityToBill, DateTime correspondenceDate);

    /// <summary>
    /// To validate error correction
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    int ValidateForErrorCorrection(string newValue, string exceptionCode, Guid? entityId = null);


    /// <summary>
    /// SCP277476: Validate MISC TAX, VAT and Add on Charge Total amount against its breakdown total
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="transactionType"></param>
    /// <param name="transactionLevelTotalTaxAmount"></param>
    /// <param name="sumofTaxTotalBrdown"></param>
    /// <param name="sumofVatTotalBrdown"></param>
    /// <param name="transactionLevelTotalVatAmount"></param>
    /// <param name="transactionLevelTotalAddOnCharge"></param>
    /// <param name="sumofAddOnTotalBrdown"></param>
    /// <returns></returns>
    string ValidateMiscInvoiceBreakDownCaptured(Guid transactionId, int transactionType,
                                                decimal transactionLevelTotalTaxAmount, decimal sumofTaxTotalBrdown,
                                                decimal sumofVatTotalBrdown, decimal transactionLevelTotalVatAmount,
                                                decimal transactionLevelTotalAddOnCharge, decimal sumofAddOnTotalBrdown);

      /// <summary>
      /// Validate location code, it should exist either invoice level or line item level. 
      /// </summary>
      /// <param name="lineItem"></param>
      /// <param name="invHeaderLocation"></param>
      /// <param name="invoiceId"></param>
    void ValidateLocationCode(LineItem lineItem, string invHeaderLocation = null, string invoiceId = null);

    void ValidateChargeCategory(LineItem lineItem, string invoiceId, int? invHeaderChargeCategory);

    //CMP508:Audit Trail Download with Supporting Documents
    /// <summary>
    /// Get billing history audit trail pdf
    /// </summary>
    /// <param name="invoiceId">invoice id</param>
    /// <returns>Audit Trail PDF</returns>
    AuditTrailPdf GetBillingHistoryAuditTrailPdf(string invoiceId);

    /// <summary>
    /// Returns Html string for audit trail with supporting docs assigned with their folder numbers
    /// </summary>
    /// <param name="auditTrail">audit trail for which html is to be genereated</param>
    /// <param name="suppDocs">out parameter for Supp Docs</param>
    /// <returns>Html for audit trail</returns>
    string GenerateMisUatpcBillingHistoryAuditTrailPackage(AuditTrailPdf auditTrail, int currentMemberId,
                                                             string areaName, out Dictionary<Attachment, int> suppDocs);
    /// <summary>
    /// get invoice type by invoice descriptions
    /// </summary>
    /// <param name="invDesc"></param>
    /// <returns></returns>
    string LookupTemplateType(string invDesc);

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    /// <summary>
    /// Search Daily payable invoices
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    List<MUDailyPayableResultData> SearchDailyPayableInvoices(MiscSearchCriteria searchCriteria);

    //CMP288: Invoice members location add for invoice preview.
   /// <summary>
   /// This method use to add memberlocation information for preview purpose only.
   /// </summary>
   /// <param name="invoice"></param>
    void UpdateInvoiceMemberLocationInfo(InvoiceBase invoice);

    /// <summary>
    /// SCP280744: MISC UATP Exchange Rate population/validation during error correction
    /// </summary>
    /// <param name="miscUatpInvoice"></param>
    /// <param name="updatedExRate"> Updated Exchange Rate (SCP321993: FW ICH Settlement Error - SIS Production) </param>
    /// <param name="updatedClearanceAmt"> Updated Clearance Amount (SCP321993: FW ICH Settlement Error - SIS Production) </param>
    string ExchangeRateValidationsOnErrorCorrection(MiscUatpInvoice miscUatpInvoice, out decimal? updatedExRate, out decimal? updatedClearanceAmt);

      /// <summary>
      /// SCP280744: MISC UATP Exchange Rate population/validation during error correction
      /// </summary>
      /// <param name="miscUatpInvoice"></param>
      /// <param name="originalInvoice"></param>
      /// <returns></returns>
    string ValidateRejInvAndCorresInvOnErrorCorrection(MiscUatpInvoice miscUatpInvoice, decimal dexchangeRateFromFDRMaster, out decimal? updatedExRate, out decimal? updatedClearanceAmt);

      /// <summary>
      /// Check Submit Invoice Permission of user.
      ///  ID : 296572 - Submission and Assign permission to user doesn't match !
      /// </summary>
      /// <param name="invIdList">Invoice id list </param>
      /// <param name="userId">user id</param>
      /// <returns></returns>
      List<string> ChkInvSubmitPermission(List<string> invIdList, int userId);

      /// <summary>
      /// Get Invoice Header only based on invoice Id
      /// ID : 325374 - File Loading & Web Response Stats -PayablesInvoiceSearch
      /// </summary>
      /// <param name="invoiceId"></param>
      /// <returns></returns>
      MiscUatpInvoice GetInvoiceHeaderForManageScreen(string invoiceId);

      //CMP#502 : [3.4] Rejection Reason for MISC Invoices
      /// <summary>
      /// Validates the rejection reason code.
      /// </summary>
      /// <param name="lineItem">The line item.</param>
      /// <param name="exceptionDetailsList">The exception details list.</param>
      /// <param name="fileName">Name of the file.</param>
      /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
      /// <param name="fileSubmissionDate">The file submission date.</param>
      /// <param name="isIsWebReq">if set to <c>true</c> [is is web req].</param>
      /// <returns></returns>
      bool ValidateRejectionReasonCode(LineItem lineItem, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MiscUatpInvoice miscUatpInvoice, DateTime fileSubmissionDate,bool isIsWebReq = false);

      /// <summary>
      /// CMP#648: function will be used to validation exchange rate,clearance currency and TotalAmount.
      /// </summary>
      /// <param name="invoice">invoice header object</param>
      /// <param name="errorMsg">return error message if any.</param>
      /// <returns></returns>
      MiscUatpInvoice ValidateIswebMiscInvExchangeRate(MiscUatpInvoice invoice, out string errorMsg);

      /// <summary>
      /// SCP#417067: Validations for Notes and Legal text
      /// </summary>
      /// <param name="invoice">MiscUatp Invoice</param>
      /// <param name="errorMsg">Error Message out parameter</param>
      /// <returns> Eerror code</returns>
      MiscUatpInvoice ValidateIswebMiscInvHeaderNoteDescription(MiscUatpInvoice invoice, out string errorMsg);

      /// <summary>
      /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
      /// </summary>
      /// <param name="yourInvoice">Invoice being rejected</param>
      /// <param name="rejectionInvoice">Invoice will create on rejected invoice</param>
      /// <param name="fileName">File Name</param>
      /// <param name="fileSubmissionDate">File Submission Date</param>
      /// <param name="exceptionDetailsList">Exception Detail List</param>
      /// <returns></returns>
      string ValidateMiscLastStageRmForTimeLimit(MiscUatpInvoice yourInvoice, MiscUatpInvoice rejectionInvoice,
                                                 RmValidationType validationType, string fileName = null,
                                                 DateTime? fileSubmissionDate = null,
                                                 IList<IsValidationExceptionDetail> exceptionDetailsList = null);

  }
}

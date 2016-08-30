using System;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Impl
{
  public class MuMaterializers
  {
    public  readonly Materializer<MiscUatpInvoiceAdditionalDetail> MiscUatpInvoiceAdditionalDetailMaterializer = new Materializer<MiscUatpInvoiceAdditionalDetail>(miad =>
    new MiscUatpInvoiceAdditionalDetail
    {
      LastUpdatedOn = miad.Field<object>("LAST_UPDATED_ON") != null ? miad.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = miad.Field<object>("LAST_UPDATED_BY") != null ? miad.Field<int>("LAST_UPDATED_BY") : 0,
      TypeId = miad.Field<object>("ADDITIONAL_DETAIL_TYPE") != null ? miad.Field<int>("ADDITIONAL_DETAIL_TYPE") : 0,
      Description = miad.Field<string>("ADDITIONAL_DETAIL_DESC"),
      Name = miad.Field<string>("ADDITIONAL_DETAIL"),
      InvoiceId = miad.Field<byte[]>("INVOICE_ID") != null ? new Guid(miad.Field<byte[]>("INVOICE_ID")) : new Guid(),
      Id = miad.Field<byte[]>("ADDITIONAL_DETAIL_ID") != null ? new Guid(miad.Field<byte[]>("ADDITIONAL_DETAIL_ID")) : new Guid(),
      RecordNumber = miad.Field<object>("RECORD_NO") != null ? miad.Field<int>("RECORD_NO") : 0,
    });

    public  readonly Materializer<ContactInformation> MiscUatpInvoiceMemberContactMaterializer = new Materializer<ContactInformation>(mimc =>
       new ContactInformation
       {
         LastUpdatedOn = mimc.Field<object>("LAST_UPDATED_ON") != null ? mimc.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
         LastUpdatedBy = mimc.Field<object>("LAST_UPDATED_BY") != null ? mimc.Field<int>("LAST_UPDATED_BY") : 0,
         MemberTypeId = mimc.Field<object>("MEMBER_TYPE") != null ? mimc.Field<int>("MEMBER_TYPE") : 0,
         Description = mimc.Field<string>("CONTACT_DESCRIPTION"),
         Value = mimc.Field<string>("CONTACT_VALUE"),
         Type = mimc.Field<string>("CONTACT_TYPE"),
         InvoiceId = mimc.Field<byte[]>("INVOICE_ID") != null ? new Guid(mimc.Field<byte[]>("INVOICE_ID")) : new Guid(),
         Id = mimc.Field<byte[]>("MEM_LOC_INFO_CONTACT_ID") != null ? new Guid(mimc.Field<byte[]>("MEM_LOC_INFO_CONTACT_ID")) : new Guid(),
       });

    public  readonly Materializer<PaymentDetail> PaymentDetailMaterializer = new Materializer<PaymentDetail>(pdm =>
     new PaymentDetail
     {
       LastUpdatedOn = pdm.Field<object>("LAST_UPDATED_ON") != null ? pdm.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
       LastUpdatedBy = pdm.Field<object>("LAST_UPDATED_BY") != null ? pdm.Field<int>("LAST_UPDATED_BY") : 0,
       NetDueDays = pdm.Field<object>("NET_DUE_DAYS") != null ? pdm.Field<int?>("NET_DUE_DAYS") : null,
       NetDueDate = pdm.Field<object>("NET_DUE_DATE") != null ? pdm.Field<DateTime?>("NET_DUE_DATE") : null,
       DiscountDueDays = pdm.Field<object>("DISCOUNT_DUE_DAYS") != null ? pdm.Field<int?>("DISCOUNT_DUE_DAYS") : null,
       DiscountDueDate = pdm.Field<object>("DISCOUNT_DUE_DATE") != null ? pdm.Field<DateTime?>("DISCOUNT_DUE_DATE") : null,
       DiscountPercent = pdm.Field<object>("DISCOUNT_PERCENT") != null ? pdm.Field<double?>("DISCOUNT_PERCENT") : null,
       DateBasis = pdm.Field<string>("DATE_BASIS"),
       Description = pdm.Field<string>("PAYMENT_TERM_DESC"),
       PaymentTermsType = pdm.Field<string>("PAYMENT_TERM_TYPE"),
       InvoiceId = pdm.Field<byte[]>("INVOICE_ID") != null ? new Guid(pdm.Field<byte[]>("INVOICE_ID")) : new Guid(),
       //This property is NOT required  
       //Id = pdm.Field<byte[]>("MEM_LOC_INFO_CONTACT_ID") != null ? new Guid(pdm.Field<byte[]>("MEM_LOC_INFO_CONTACT_ID")) : new Guid(),
     });



    public  readonly Materializer<ChargeCategory> ChargeCategoryMaterializer = new Materializer<ChargeCategory>(cc =>
      new ChargeCategory
      {
        IsActive = cc.Field<object>("IS_ACTIVE") != null ? cc.Field<int>("IS_ACTIVE") > 0 : false,
        LastUpdatedOn = cc.Field<object>("LAST_UPDATED_ON") != null ? cc.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = cc.Field<object>("LAST_UPDATED_BY") != null ? cc.Field<int>("LAST_UPDATED_BY") : 0,
        Description = cc.Field<string>("DESCRIPTION"),
        BillingCategoryId = cc.Field<object>("BILLING_CATEGORY_ID") != null ? cc.Field<int>("BILLING_CATEGORY_ID") : 0,
        Name = cc.Field<string>("CHARGE_CATEGORY_NAME"),
        Id = cc.Field<object>("CHARGE_CATEGORY_ID") != null ? cc.Field<int>("CHARGE_CATEGORY_ID") : 0,
      });

    public  readonly Materializer<MiscUatpInvoice> MiscInvoiceMaterializer = new Materializer<MiscUatpInvoice>(r => new MiscUatpInvoice
    {
      //InvoiceBase properties
      BillingCategoryId = r.TryGetField<object>("BILLING_CATEGORY_ID") != null ? r.Field<int>("BILLING_CATEGORY_ID") : 0,
      Id = r.TryGetField<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
      SubmissionMethodId = r.Field<object>("SUBMISSION_METHOD_ID") != null ? r.Field<int>("SUBMISSION_METHOD_ID") : 0,
      LegalText = r.Field<string>("LEGAL_TEXT"),
      BillingYear = r.Field<object>("BILLING_YEAR") != null ? r.Field<int>("BILLING_YEAR") : 0,
      BillingMemberId = r.Field<object>("BILLING_MEMBER_ID") != null ? r.Field<int>("BILLING_MEMBER_ID") : 0,
      BilledMemberId = r.Field<object>("BILLED_MEMBER_ID") != null ? r.Field<int>("BILLED_MEMBER_ID") : 0,
      LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      ResubmissionRemarks = r.Field<string>("RESUBMISSION_REMARKS"),
      ResubmissionStatusId = r.Field<object>("RESUBMISSION_STATUS_ID") != null ? r.Field<int>("RESUBMISSION_STATUS_ID") : 0,
      ResubmissionPeriod = r.Field<object>("RESUBMISSION_PERIOD") != null ? r.Field<int>("RESUBMISSION_PERIOD") : 0,
      ResubmissionBillingMonth = r.Field<object>("RESUBMISSION_BILLING_MONTH") != null ? r.Field<int>("RESUBMISSION_BILLING_MONTH") : 0,
      ReinstatementPeriod = r.Field<object>("REINSTATEMENT_PERIOD") != null ? r.Field<int>("REINSTATEMENT_PERIOD") : 0,
      ReinstatementMonth = r.Field<object>("REINSTATEMENT_MONTH") != null ? r.Field<int>("REINSTATEMENT_MONTH") : 0,
      SuspensionPeriod = r.Field<object>("SUSPENSION_PERIOD") != null ? r.Field<int>("SUSPENSION_PERIOD") : 0,
      SuspensionMonth = r.Field<object>("SUSPENSION_MONTH") != null ? r.Field<int>("SUSPENSION_MONTH") : 0,
      OriginalPeriod = r.Field<object>("ORIGINAL_BILLING_PERIOD") != null ? r.Field<int>("ORIGINAL_BILLING_PERIOD") : 0,
      OriginalBillingMonth = r.Field<object>("ORIGINAL_BILLING_MONTH") != null ? r.Field<int>("ORIGINAL_BILLING_MONTH") : 0,
      PresentedStatusDate = r.Field<object>("PRESENTED_STATUS_DATE") != null ? r.Field<DateTime>("PRESENTED_STATUS_DATE") : new DateTime(),
      PresentedStatusId = r.Field<object>("PRESENTED_STATUS") != null ? r.Field<int>("PRESENTED_STATUS") : 0,
      SettlementFileSentDate = r.Field<object>("SETTLEMENT_STATUS_DATE") != null ? r.Field<DateTime>("SETTLEMENT_STATUS_DATE") : new DateTime(),
      SettlementFileStatusId = r.Field<object>("SETTLEMENT_STATUS") != null ? r.Field<int>("SETTLEMENT_STATUS") : 0,
      DigitalSignatureDate = r.Field<object>("DIGITAL_SIGN_STATUS_DATE") != null ? r.Field<DateTime>("DIGITAL_SIGN_STATUS_DATE") : new DateTime(),
      DigitalSignatureStatusId = r.Field<object>("DIGITAL_SIGN_STATUS") != null ? r.Field<int>("DIGITAL_SIGN_STATUS") : 0,
      ValueConfirmationDate = r.Field<object>("VALUE_CONFIRM_STATUS_DATE") != null ? r.Field<DateTime>("VALUE_CONFIRM_STATUS_DATE") : new DateTime(),
      ValueConfirmationStatusId = r.Field<object>("VALUE_CONFIRM_STATUS") != null ? r.Field<int?>("VALUE_CONFIRM_STATUS") : null,
      ValidationDate = r.Field<object>("VALIDATION_STATUS_DATE") != null ? r.Field<DateTime>("VALIDATION_STATUS_DATE") : new DateTime(),
      ValidationStatusId = r.Field<object>("VALIDATION_STATUS") != null ? r.Field<int>("VALIDATION_STATUS") : 0,
      IsLateSubmitted = r.Field<object>("IS_LATE_SUBMITTED") != null ? (r.Field<int>("IS_LATE_SUBMITTED") == 0 ? false : true) : false,
      InvoiceStatusId = r.Field<object>("INVOICE_STATUS_ID") != null ? r.Field<int>("INVOICE_STATUS_ID") : 0,
      IsInputFileId = r.Field<byte[]>("IS_FILE_LOG_ID") != null ? new Guid(r.Field<byte[]>("IS_FILE_LOG_ID")) : new Guid(),
      SuspendedInvoiceFlag = r.Field<object>("IS_SUSPENDED") != null ? (r.Field<int>("IS_SUSPENDED") == 0 ? false : true) : false,
      /*CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.*/
      ExchangeRate = r.Field<object>("EXCHANGE_RATE") != null ? r.Field<decimal>("EXCHANGE_RATE") : (decimal?)null,
      InvoiceDate = r.Field<object>("INVOICE_DATE") != null ? r.Field<DateTime>("INVOICE_DATE") : new DateTime(), //ok
      DigitalSignatureRequiredId = r.Field<object>("DS_REQUIRED_ID") != null ? r.Field<int>("DS_REQUIRED_ID") : 0,
      SettlementMethodId = r.Field<object>("SETTLEMENT_METHOD_ID") != null ? r.Field<int>("SETTLEMENT_METHOD_ID") : 0,
      BillingPeriod = r.Field<object>("PERIOD_NO") != null ? r.Field<int>("PERIOD_NO") : 0,
      BillingCurrencyId = r.Field<object>("BILLING_CURRENCY_CODE_NUM") != null ? r.Field<int>("BILLING_CURRENCY_CODE_NUM") : 0,
      ListingCurrencyId = r.Field<object>("LISTING_CURRENCY_CODE_NUM") != null ? r.Field<int>("LISTING_CURRENCY_CODE_NUM") : 0,
      BillingMonth = r.Field<object>("BILLING_MONTH") != null ? r.Field<int>("BILLING_MONTH") : 0,
      InvoiceNumber = r.Field<string>("INVOICE_NO"),
      BillingCode = r.Field<object>("BILLING_CODE_ID") != null ? r.Field<int>("BILLING_CODE_ID") : 0,
      BillingMemberLocationCode = r.Field<string>("BILLING_MEM_LOC_CODE"),//ok
      BilledMemberLocationCode = r.Field<string>("BILLED_MEM_LOC_CODE"),
      BillingReferenceDataSourceId = r.Field<object>("BILLING_REF_DATA_SOURCE_ID") != null ? r.Field<int>("BILLING_REF_DATA_SOURCE_ID") : 0,
      BilledReferenceDataSourceId = r.Field<object>("BILLED_REF_DATA_SOURCE_ID") != null ? r.Field<int>("BILLED_REF_DATA_SOURCE_ID") : 0,
      InvoiceOwnerId = r.Field<object>("INVOICE_OWNER_ID") != null ? Convert.ToInt32(r.Field<object>("INVOICE_OWNER_ID")) : 0,
      LegalPdfLocation = r.Field<string>("LEGAL_PDF_LOCATION"),
      SupportingAttachmentStatusId = r.Field<object>("SUPPORTING_ATTACHMENT_STATUS") != null ? r.Field<int>("SUPPORTING_ATTACHMENT_STATUS") : 0,
      IsFutureSubmission = r.Field<object>("IS_FUTURE_SUBMISSION") != null ? (r.Field<int>("IS_FUTURE_SUBMISSION") == 0 ? false : true) : false,
      LegalXmlLocation = r.Field<string>("legal_xml_location"),
      //Misc Invoice mapping section
      TaxInvoiceNumber = r.Field<string>("TAX_INVOICE_NO"),
      TaxPointDate = r.Field<object>("TAX_POINT_DATE") != null ? r.Field<DateTime>("TAX_POINT_DATE") : new DateTime(),

      LocationCode = r.Field<string>("LOCATION_CODE"),
      LocationCodeIcao = r.Field<string>("LOCATION_CODE_ICAO"),
      ChargeCategoryId = r.Field<object>("CHARGE_CATEGORY_ID") != null ? r.Field<int>("CHARGE_CATEGORY_ID") : 0,
      InvoiceTypeId = r.Field<object>("INVOICE_TYPE_ID") != null ? r.Field<int>("INVOICE_TYPE_ID") : 0,
      PONumber = r.Field<string>("PO_NUMBER"),
      AttachmentIndicatorOriginal = r.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (r.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? 0 : (r.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? 0 : 1)) : 0,
      AttachmentIndicatorValidated = r.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
      AttachmentNumber = r.Field<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int?>("NO_OF_ATTACHMENTS") : null,
      BilledMemberContactName = r.Field<string>("BILLED_CONTACT_NAME"),
      BillingMemberContactName = r.Field<string>("BILLING_CONTACT_NAME"),
      SettlementYear = r.Field<object>("SETTLEMENT_YEAR") != null ? r.Field<int>("SETTLEMENT_YEAR") : 0,
      SettlementMonth = r.Field<object>("SETTLEMENT_MONTH") != null ? r.Field<int>("SETTLEMENT_MONTH") : 0,
      SettlementPeriod = r.Field<object>("SETTLEMENT_PERIOD") != null ? r.Field<int>("SETTLEMENT_PERIOD") : 0,
      RejectionStage = r.Field<object>("REJECTION_STAGE") != null ? r.Field<int>("REJECTION_STAGE") : 0,
      IsAuthorityToBill = r.Field<object>("IS_AUTHORITY_TO_BILL") != null ? (r.Field<int>("IS_AUTHORITY_TO_BILL") == 0 ? false : true) : false,
      CorrespondenceRefNo = r.Field<object>("CORRESPONDENCE_REF_NO") != null ? (long?)Convert.ToInt64(r.Field<object>("CORRESPONDENCE_REF_NO")) : null,
      RejectedInvoiceNumber = r.Field<string>("REJECTED_INVOICE_NO"),
      IsValidationFlag = r.Field<string>("IS_VALIDATION"),
      XmlSignatureLocation = r.Field<string>("XML_SIGNATURE_LOCATION"),
      XmlVerificationLogLocation = r.Field<string>("XML_VERIFICATION_LOG_LOCATION"),
      LocationName = r.Field<string>("LOCATION_NAME"),
      InclusionStatusId = r.Field<object>("INCLUSION_STATUS") != null ? r.Field<int>("INCLUSION_STATUS") : 0,
      IsWebFileGenerationDate = r.Field<object>("IS_WEB_FILE_GENERATION_DATE") != null ? r.Field<DateTime>("IS_WEB_FILE_GENERATION_DATE") : new DateTime(),
      InvTemplateLanguage = r.Field<string>("INV_TEMPLATE_LANGUAGE"),
      //SCP0000:Impact on MISC/UATP rejection linking due to purging
      ExpiryDatePeriod = r.Field<object>("EXPIRY_DATEPERIOD") != null ? r.Field<DateTime?>("EXPIRY_DATEPERIOD") : null,
      //TransactionStatusId = r.Field<object>("TRANSACTION_STATUS_ID") != null ? r.Field<int>("TRANSACTION_STATUS_ID") : 0,
      ViewableByClearingHouse = r.Field<string>("VIEWABLE_BY_CH"),
      /* CMP #624: ICH Rewrite-New SMI X. 
       * Description: Added support for new column */
      ChAgreementIndicator = r.Field<string>("CH_AGREEMENT_INDICATOR"),
      ChDueDate = r.Field<object>("CH_DUE_DATE") != null ? r.Field<DateTime?>("CH_DUE_DATE") : null,
      ChValidationResult = r.Field<string>("CH_VALIDATION_RESULT"),
      CurrencyRateIndicator = r.Field<string>("CURRENCY_RATE_INDICATOR"),
        /* CMP #622: MISC Outputs Split as per Location IDs. 
        * Description: Added new columns of member location */
      MiscBillingMemberLocCode = r.Field<string>("MISC_BIL_MEM_LOC_CODE"),
      MiscBilledMemberLocCode = r.Field<string>("MISC_BLD_MEM_LOC_CODE")
    });

    public  readonly Materializer<InvoiceAddOnCharge> InvoiceAddOnChargeMaterializer = new Materializer<InvoiceAddOnCharge>(r =>
         new InvoiceAddOnCharge
         {
           LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
           LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
           ChargeForLineItemNumber = r.Field<string>("CHARGE_FOR_LINE_ITEM_NO"),
           Amount = r.Field<object>("ADD_ON_CHARGE_AMOUNT") != null ? r.Field<decimal>("ADD_ON_CHARGE_AMOUNT") : 0,
           ChargeableAmount = r.Field<object>("ADD_ON_CHARGEABLE_AMOUNT") != null ? r.Field<decimal?>("ADD_ON_CHARGEABLE_AMOUNT") : null,
           Percentage = r.Field<object>("ADD_ON_CHARGE_PERCENTAGE") != null ? r.Field<double?>("ADD_ON_CHARGE_PERCENTAGE") : null,
           Code = r.Field<string>("ADD_ON_CHARGE_CODE"),
           Name = r.Field<string>("ADD_ON_CHARGE_NAME"),
           ParentId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
           Id = r.Field<byte[]>("INVOICE_ADD_ON_CHARGE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ADD_ON_CHARGE_ID")) : new Guid(),
         });

    public readonly Materializer<LineItem> LineItemMaterializer = new Materializer<LineItem>(li =>
         new LineItem
         {
           Id = li.Field<byte[]>("LINE_ITEM_ID") != null ? new Guid(li.Field<byte[]>("LINE_ITEM_ID")) : new Guid(),
           InvoiceId = li.Field<byte[]>("INVOICE_ID") != null ? new Guid(li.Field<byte[]>("INVOICE_ID")) : new Guid(),
           LineItemNumber = li.Field<object>("LINE_ITEM_NO") != null ? li.Field<int>("LINE_ITEM_NO") : 0,
           POLineItemNumber = li.Field<object>("PO_LINE_ITEM_NO") != null ? li.Field<int?>("PO_LINE_ITEM_NO") : null,
           ChargeCodeId = li.Field<object>("CHARGE_CODE_ID") != null ? li.Field<int>("CHARGE_CODE_ID") : 0,
           ChargeCodeTypeId = li.Field<object>("CHARGE_CODE_TYPE_ID") != null ? li.Field<int?>("CHARGE_CODE_TYPE_ID") : null,
           Description = li.Field<string>("DESCRIPTION"),
           ProductId = li.Field<string>("PRODUCT_ID"),
           StartDate = (DateTime?)(li.Field<object>("START_DATE")), //Changes to set StartDate as null if value is not selected
           EndDate = li.Field<object>("END_DATE") != null ? li.Field<DateTime>("END_DATE") : new DateTime(),
           LocationCode = li.Field<string>("CITY_CODE_ALPHA"),
           LocationCodeIcao = li.Field<string>("LOCATION_CODE_ICAO"),
           MinimumQuantityFlag = li.Field<object>("MIN_QUANTITY_FLAG") != null && li.Field<int>("MIN_QUANTITY_FLAG") > 0,
           Quantity = li.Field<object>("QUANTITY") != null ? li.Field<decimal>("QUANTITY") : 0,
           UomCodeId = li.Field<string>("UOM_CODE"),
           UnitPrice = li.Field<object>("UNIT_PRICE") != null ? li.Field<decimal>("UNIT_PRICE") : 0,
           ScalingFactor = li.Field<object>("SCALING_FACTOR") != null ? li.Field<int?>("SCALING_FACTOR") : null,
           TotalTaxAmount = li.Field<object>("TOTAL_TAX_AMOUNT") != null ? li.Field<decimal?>("TOTAL_TAX_AMOUNT") : null,
           TotalVatAmount = li.Field<object>("TOTAL_VAT_AMOUNT") != null ? li.Field<decimal?>("TOTAL_VAT_AMOUNT") : null,
           TotalAddOnChargeAmount = li.Field<object>("TOTAL_ADD_ON_CHARGE_AMOUNT") != null ? li.Field<decimal?>("TOTAL_ADD_ON_CHARGE_AMOUNT") : null,
           TotalNetAmount = li.Field<object>("TOTAL_NET_AMOUNT") != null ? li.Field<decimal>("TOTAL_NET_AMOUNT") : 0,
           OriginalLineItemNumber = li.Field<object>("ORIGINAL_LINE_ITEM_NO") != null ? li.Field<int?>("ORIGINAL_LINE_ITEM_NO") : null,
           DetailCount = li.Field<object>("DETAIL_COUNT") != null ? li.Field<int>("DETAIL_COUNT") : 0,
           LastUpdatedBy = li.Field<object>("LAST_UPDATED_BY") != null ? li.Field<int>("LAST_UPDATED_BY") : 0,
           LastUpdatedOn = li.Field<object>("LAST_UPDATED_ON") != null ? li.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
           ChargeAmount = li.Field<object>("CHARGE_AMOUNT") != null ? li.Field<decimal>("CHARGE_AMOUNT") : 0,
           LineItemStatusId = li.Field<object>("LINE_ITEM_STATUS_ID") != null ? li.Field<int>("LINE_ITEM_STATUS_ID") : 0,
           LocationName = li.Field<string>("LOCATION_NAME"),
           //CMP #502: [3.5] Rejection Reason for MISC Invoices.
           RejectionReasonCode = li.Field<string>("REJ_REASONE_CODE"),
           RejReasonCodeDescription = li.Field<string>("REJ_REASONE_CODE_DESC")
         });
    public  readonly Materializer<LineItemDetail> LineItemDetailsMaterializer = new Materializer<LineItemDetail>(lid =>
        new LineItemDetail
        {
          Id = lid.Field<byte[]>("LINE_ITEMDETAIL_ID") != null ? new Guid(lid.Field<byte[]>("LINE_ITEMDETAIL_ID")) : new Guid(),
          LineItemId = lid.Field<byte[]>("LINE_ITEM_ID") != null ? new Guid(lid.Field<byte[]>("LINE_ITEM_ID")) : new Guid(),
          DetailNumber = lid.Field<object>("DETAIL_NO") != null ? lid.Field<int>("DETAIL_NO") : 0,
          LineItemNumber = lid.Field<object>("LINE_ITEM_NO") != null ? lid.Field<int>("LINE_ITEM_NO") : 0,
          Description = lid.Field<string>("DESCRIPTION"),
          ProductId = lid.Field<string>("PRODUCT_ID"),
          StartDate = (DateTime?)(lid.Field<object>("START_DATE")), //Changes to set StartDate as null if value is not selected
          EndDate = lid.Field<object>("END_DATE") != null ? lid.Field<DateTime>("END_DATE") : new DateTime(),
          MinimumQuantityFlag = lid.Field<object>("MINIMUM_QUANTITY_FLAG") != null && lid.Field<int>("MINIMUM_QUANTITY_FLAG") > 0,
          Quantity = lid.Field<object>("QUANTITY") != null ? lid.Field<decimal>("QUANTITY") : 0,
          UomCodeId = lid.Field<string>("UOM_CODE"),
          UnitPrice = lid.Field<object>("UNIT_PRICE") != null ? lid.Field<decimal>("UNIT_PRICE") : 0,
          ScalingFactor = lid.Field<object>("SCALING_FACTOR") != null ? lid.Field<int>("SCALING_FACTOR") : 0,
          TotalNetAmount = lid.Field<object>("TOTAL_NET_AMOUNT") != null ? lid.Field<decimal>("TOTAL_NET_AMOUNT") : 0,
          LastUpdatedBy = lid.Field<object>("LAST_UPDATED_BY") != null ? lid.Field<int>("LAST_UPDATED_BY") : 0,
          LastUpdatedOn = lid.Field<object>("LAST_UPDATED_ON") != null ? lid.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
          TotalTaxAmount = lid.Field<object>("TOTAL_TAX_AMOUNT") != null ? lid.Field<decimal>("TOTAL_TAX_AMOUNT") : 0,
          TotalVatAmount = lid.Field<object>("TOTAL_VAT_AMOUNT") != null ? lid.Field<decimal>("TOTAL_VAT_AMOUNT") : 0,
          TotalAddOnChargeAmount = lid.Field<object>("TOTAL_ADD_ON_CHARGE_AMOUNT") != null ? lid.Field<decimal>("TOTAL_ADD_ON_CHARGE_AMOUNT") : 0,
          ChargeAmount = lid.Field<object>("CHARGE_AMOUNT") != null ? lid.Field<decimal>("CHARGE_AMOUNT") : 0,
          DynamicFieldsSummary = lid.Field<string>("DYNAMIC_FIELDS_SUMMARY")
        });
    //CMP #636: Standard Update Mobilization. Desc: Added new column 'IsActiveChargeCodeType' and change 'IsChargeCodeTypeRequired' bool to nullable bool
    public  readonly Materializer<ChargeCode> ChargeCodeMaterializer = new Materializer<ChargeCode>(cc =>
       new ChargeCode
       {
         Id = cc.Field<object>("CHARGE_CODE_ID") != null ? cc.Field<int>("CHARGE_CODE_ID") : 0,
         Name = cc.Field<string>("CHARGE_CODE_NAME"),
         ChargeCategoryId = cc.Field<object>("CHARGE_CATEGORY_ID") != null ? cc.Field<int>("CHARGE_CATEGORY_ID") : 0,
         Description = cc.Field<string>("DESCRIPTION"),
         IsActive = cc.Field<int>("IS_ACTIVE") > 0,
         LastUpdatedBy = cc.Field<object>("LAST_UPDATED_BY") != null ? cc.Field<int>("LAST_UPDATED_BY") : 0,
         LastUpdatedOn = cc.Field<object>("LAST_UPDATED_ON") != null ? cc.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
         IsLocationRequiredForInvoice = cc.Field<object>("LOC_REQ_INVOICE") != null && cc.Field<int>("LOC_REQ_INVOICE") > 0,
         IsLocationRequiredForLineItem = cc.Field<object>("LOC_REQ_LINE_ITEM") != null && cc.Field<int>("LOC_REQ_LINE_ITEM") > 0,
         IsChargeCodeTypeRequired = cc.Field<object>("IS_CHARGE_CODE_TYPE_REQ") != null ? cc.Field<int>("IS_CHARGE_CODE_TYPE_REQ") > 0 : (bool?)null,
         IsActiveChargeCodeType = cc.Field<object>("IS_ACTIVE_CHARGE_CODE_TYPE") != null && cc.Field<int>("IS_ACTIVE_CHARGE_CODE_TYPE") > 0
       });

    public  readonly Materializer<UomCode> UomCodeMaterializer = new Materializer<UomCode>(uc =>
      new UomCode
      {
        Id = uc.Field<string>("UOM_CODE"),
        Type = uc.Field<object>("UOM_CODE_TYPE") != null ? uc.Field<int>("UOM_CODE_TYPE") : 0,
        Description = uc.Field<string>("DESCRIPTION"),
        IsActive = uc.Field<int>("IS_ACTIVE") > 0,
        LastUpdatedBy = uc.Field<object>("LAST_UPDATED_BY") != null ? uc.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = uc.Field<object>("LAST_UPDATED_ON") != null ? uc.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      });
    public  readonly Materializer<ChargeCodeType> ChargeCodeTypeMaterializer = new Materializer<ChargeCodeType>(cct =>
      new ChargeCodeType
      {
        Id = cct.Field<object>("CHARGE_CODE_TYPE_ID") != null ? cct.Field<int>("CHARGE_CODE_TYPE_ID") : 0,
        Name = cct.Field<string>("CHARGE_CODE_TYPE"),
        ChargeCodeId = cct.Field<object>("CHARGE_CODE_ID") != null ? cct.Field<int>("CHARGE_CODE_ID") : 0,
        IsActive = cct.Field<int>("IS_ACTIVE") > 0,
        LastUpdatedBy = cct.Field<object>("LAST_UPDATED_BY") != null ? cct.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = cct.Field<object>("LAST_UPDATED_ON") != null ? cct.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      });

    public  readonly Materializer<FieldValue> FieldValueMaterializer = new Materializer<FieldValue>(fv =>
       new FieldValue
       {
         Id = fv.Field<byte[]>("FIELD_VALUE_ID") != null ? new Guid(fv.Field<byte[]>("FIELD_VALUE_ID")) : new Guid(),
         ParentId = fv.Field<byte[]>("PARENT_ID") != null ? new Guid(fv.Field<byte[]>("PARENT_ID")) : (Guid?)null,
         LineItemDetailId = fv.Field<byte[]>("LINE_ITEMDETAIL_ID") != null ? new Guid(fv.Field<byte[]>("LINE_ITEMDETAIL_ID")) : new Guid(),
         FieldMetaDataId = fv.Field<byte[]>("FIELD_METADATA_ID") != null ? new Guid(fv.Field<byte[]>("FIELD_METADATA_ID")) : new Guid(),
         Value = fv.Field<string>("FIELD_VALUE"),
         LastUpdatedBy = fv.Field<object>("LAST_UPDATED_BY") != null ? fv.Field<int>("LAST_UPDATED_BY") : 0,
         LastUpdatedOn = fv.Field<object>("LAST_UPDATED_ON") != null ? fv.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
       });
    public  readonly Materializer<FieldMetaData> FieldMetaDataMaterializer = new Materializer<FieldMetaData>(fmd =>
        new FieldMetaData
        {
          Id = fmd.Field<byte[]>("FIELD_METADATA_ID") != null ? new Guid(fmd.Field<byte[]>("FIELD_METADATA_ID")) : new Guid(),
          ParentId = fmd.Field<byte[]>("PARENT_ID") != null ? new Guid(fmd.Field<byte[]>("PARENT_ID")) : new Guid(),
          FieldName = fmd.Field<string>("FIELD_NAME"),
          DataSourceId = fmd.Field<object>("DATA_SOURCE_ID") != null ? fmd.Field<int>("DATA_SOURCE_ID") : 0,
          DisplayText = fmd.Field<string>("DISPLAY_TEXT"),
          ControlTypeId = fmd.Field<object>("CONTROL_TYPE_ID") != null ? fmd.Field<int>("CONTROL_TYPE_ID") : 0,
          ParentTagName = fmd.Field<string>("PARENT_TAG_NAME"),
          DisplayOrder = fmd.Field<object>("DISPLAY_ORDER") != null ? fmd.Field<int>("DISPLAY_ORDER") : 0,
          MaxOccurrence = fmd.Field<object>("OCCURRENCE_MAX_VALUE") != null ? fmd.Field<int>("OCCURRENCE_MAX_VALUE") : 0,
          MinOccurrence = fmd.Field<object>("OCCURRENCE_MIN_VALUE") != null ? fmd.Field<int>("OCCURRENCE_MIN_VALUE") : 0,
          DataTypeId = fmd.Field<object>("DATA_TYPE_ID") != null ? fmd.Field<int>("DATA_TYPE_ID") : 0,
          DataLength = fmd.Field<string>("DATA_LENGTH"),
          FieldTypeId = fmd.Field<object>("FIELD_TYPE") != null ? fmd.Field<int>("FIELD_TYPE") : 0,
          Level = fmd.Field<object>("GROUP_LEVEL") != null ? fmd.Field<int>("GROUP_LEVEL") : 0,
          CssClass = fmd.Field<object>("CSS_CLASS") != null ? fmd.Field<string>("CSS_CLASS") : string.Empty,
          LastUpdatedBy = fmd.Field<object>("LAST_UPDATED_BY") != null ? fmd.Field<int>("LAST_UPDATED_BY") : 0,
          RequiredTypeId = fmd.Field<object>("REQUIRED_TYPE_ID") != null ? fmd.Field<int>("REQUIRED_TYPE_ID") : 0,
          LastUpdatedOn = fmd.Field<object>("LAST_UPDATED_ON") != null ? fmd.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        });
    public readonly Materializer<LineItemTax> LineItemTaxMaterializer = new Materializer<LineItemTax>(lit =>
           new LineItemTax
           {
             Id = lit.Field<byte[]>("LINE_ITEM_TAX_ID") != null ? new Guid(lit.Field<byte[]>("LINE_ITEM_TAX_ID")) : new Guid(),
             ParentId = lit.Field<byte[]>("LINE_ITEM_ID") != null ? new Guid(lit.Field<byte[]>("LINE_ITEM_ID")) : new Guid(),
             RegistrationId = lit.Field<string>("TAX_REGISTRATION_ID"),
             CountryId = lit.Field<string>("COUNTRY_CODE"),
             SubdivisionCode = lit.Field<string>("SUBDIVISION_CODE"),
             Type = lit.Field<string>("TAX_TYPE"),
             SubType = lit.Field<string>("TAX_SUB_TYPE"),
             CategoryCode = lit.Field<string>("TAX_CATEGORY"),
             Description = lit.Field<string>("TAX_TEXT"),
             Percentage = lit.Field<object>("TAX_PERCENT") != null ? lit.Field<double?>("TAX_PERCENT") : null,
             CalculatedAmount = lit.Field<object>("TAXABLE_AMOUNT") != null ? lit.Field<decimal>("TAXABLE_AMOUNT") : 0,
             Amount = lit.Field<object>("BASE_AMOUNT") != null ? lit.Field<decimal?>("BASE_AMOUNT") : null,
             LastUpdatedBy = lit.Field<object>("LAST_UPDATED_BY") != null ? lit.Field<int>("LAST_UPDATED_BY") : 0,
             LastUpdatedOn = lit.Field<object>("LAST_UPDATED_ON") != null ? lit.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
             CountryCodeIcao = lit.Field<string>("COUNTRY_CODE_ICAO"),
           });

    public  readonly Materializer<LineItemAddOnCharge> LineItemAddOnChargeMaterializer = new Materializer<LineItemAddOnCharge>(liac =>
      new LineItemAddOnCharge
      {
        Id = liac.Field<byte[]>("LINE_ITEM_ADD_ON_CHARGE_ID") != null ? new Guid(liac.Field<byte[]>("LINE_ITEM_ADD_ON_CHARGE_ID")) : new Guid(),
        ParentId = liac.Field<byte[]>("LINE_ITEM_ID") != null ? new Guid(liac.Field<byte[]>("LINE_ITEM_ID")) : new Guid(),
        Name = liac.Field<string>("ADD_ON_CHARGE_NAME"),
        Code = liac.Field<string>("ADD_ON_CHARGE_CODE"),
        Percentage = liac.Field<object>("ADD_ON_CHARGE_PERCENTAGE") != null ? liac.Field<double?>("ADD_ON_CHARGE_PERCENTAGE") : null,
        ChargeableAmount = liac.Field<object>("ADD_ON_CHARGEABLE_AMOUNT") != null ? liac.Field<decimal?>("ADD_ON_CHARGEABLE_AMOUNT") : null,
        Amount = liac.Field<object>("ADD_ON_CHARGE_AMOUNT") != null ? liac.Field<decimal>("ADD_ON_CHARGE_AMOUNT") : 0,
        LastUpdatedBy = liac.Field<object>("LAST_UPDATED_BY") != null ? liac.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = liac.Field<object>("LAST_UPDATED_ON") != null ? liac.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      });

    public  readonly Materializer<LineItemAdditionalDetail> LineItemAdditionalDetailMaterializer = new Materializer<LineItemAdditionalDetail>(liad =>
      new LineItemAdditionalDetail
      {
        Id = liad.Field<byte[]>("ADDITIONAL_DETAIL_ID") != null ? new Guid(liad.Field<byte[]>("ADDITIONAL_DETAIL_ID")) : new Guid(),
        LineItemId = liad.Field<byte[]>("LINE_ITEM_ID") != null ? new Guid(liad.Field<byte[]>("LINE_ITEM_ID")) : new Guid(),
        Name = liad.Field<string>("ADDITIONAL_DETAIL"),
        Description = liad.Field<string>("ADDITIONAL_DETAIL_DESC"),
        TypeId = liad.Field<object>("ADDITIONAL_DETAIL_TYPE") != null ? liad.Field<int>("ADDITIONAL_DETAIL_TYPE") : 0,
        LastUpdatedBy = liad.Field<object>("LAST_UPDATED_BY") != null ? liad.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = liad.Field<object>("LAST_UPDATED_ON") != null ? liad.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        RecordNumber = liad.Field<object>("RECORD_NO") != null ? liad.Field<int>("RECORD_NO") : 0,
      });

    public  readonly Materializer<LineItemTaxAdditionalDetail> LineItemTaxAdditionalDetailMaterializer = new Materializer<LineItemTaxAdditionalDetail>(liad =>
     new LineItemTaxAdditionalDetail
     {
       Id = liad.Field<byte[]>("ADDITIONAL_DETAIL_ID") != null ? new Guid(liad.Field<byte[]>("ADDITIONAL_DETAIL_ID")) : new Guid(),
       LineItemTaxId = liad.Field<byte[]>("LINE_ITEM_TAX_ID") != null ? new Guid(liad.Field<byte[]>("LINE_ITEM_TAX_ID")) : new Guid(),
       Name = liad.Field<string>("ADDITIONAL_DETAIL"),
       Description = liad.Field<string>("ADDITIONAL_DETAIL_DESC"),
       TypeId = liad.Field<object>("ADDITIONAL_DETAIL_TYPE") != null ? liad.Field<int>("ADDITIONAL_DETAIL_TYPE") : 0,
       LastUpdatedBy = liad.Field<object>("LAST_UPDATED_BY") != null ? liad.Field<int>("LAST_UPDATED_BY") : 0,
       LastUpdatedOn = liad.Field<object>("LAST_UPDATED_ON") != null ? liad.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
     });

    public readonly Materializer<MemberLocationInfoAdditionalDetail> MemberLocationInfoAdditionDetailMaterializer = new Materializer<MemberLocationInfoAdditionalDetail>(r =>
     new MemberLocationInfoAdditionalDetail
     {
       Id = r.Field<byte[]>("ADDITIONAL_DETAIL_ID") != null ? new Guid(r.Field<byte[]>("ADDITIONAL_DETAIL_ID")) : new Guid(),
       MemberLocationId = r.Field<byte[]>("MEMBER_LOC_INFO_ID") != null ? new Guid(r.Field<byte[]>("MEMBER_LOC_INFO_ID")) : new Guid(),
       Name = r.Field<string>("ADDITIONAL_DETAIL"),
       Description = r.Field<string>("ADDITIONAL_DETAIL_DESC"),
       TypeId = r.Field<object>("ADDITIONAL_DETAIL_TYPE") != null ? r.Field<int>("ADDITIONAL_DETAIL_TYPE") : 0,
       LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
       LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
     });

    public readonly Materializer<LineItemDetailTax> LineItemDetailTaxMaterializer = new Materializer<LineItemDetailTax>(lidt =>
       new LineItemDetailTax
       {
         Id = lidt.Field<byte[]>("LINEITEM_DET_TAX_ID") != null ? new Guid(lidt.Field<byte[]>("LINEITEM_DET_TAX_ID")) : new Guid(),
         ParentId = lidt.Field<byte[]>("LINE_ITEMDETAIL_ID") != null ? new Guid(lidt.Field<byte[]>("LINE_ITEMDETAIL_ID")) : new Guid(),
         RegistrationId = lidt.Field<string>("TAX_REGISTRATION_ID"),
         CountryId = lidt.Field<string>("COUNTRY_CODE"),
         SubdivisionCode = lidt.Field<string>("SUBDIVISION_CODE"),
         Type = lidt.Field<string>("TAX_TYPE"),
         SubType = lidt.Field<string>("TAX_SUB_TYPE"),
         CategoryCode = lidt.Field<string>("TAX_CATEGORY"),
         Description = lidt.Field<string>("TAX_TEXT"),
         Percentage = lidt.Field<object>("TAX_PERCENT") != null ? lidt.Field<double?>("TAX_PERCENT") : null,
         CalculatedAmount = lidt.Field<object>("TAXABLE_AMT") != null ? lidt.Field<decimal>("TAXABLE_AMT") : 0,
         /* TAX_LEVEL -- Missing mapping in edmx file */
         Amount = lidt.Field<object>("BASE_AMOUNT") != null ? lidt.Field<decimal?>("BASE_AMOUNT") : null,
         LastUpdatedBy = lidt.Field<object>("LAST_UPDATED_BY") != null ? lidt.Field<int>("LAST_UPDATED_BY") : 0,
         LastUpdatedOn = lidt.Field<object>("LAST_UPDATED_ON") != null ? lidt.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
         CountryCodeIcao = lidt.Field<string>("COUNTRY_CODE_ICAO"),
       });
    public  readonly Materializer<LineItemDetailAddOnCharge> LineItemDetailAddOnChargeMaterializer = new Materializer<LineItemDetailAddOnCharge>(lidac =>
      new LineItemDetailAddOnCharge
      {
        Id = lidac.Field<byte[]>("LINEITEM_DET_ADD_ON_CHARGE_ID") != null ? new Guid(lidac.Field<byte[]>("LINEITEM_DET_ADD_ON_CHARGE_ID")) : new Guid(),
        ParentId = lidac.Field<byte[]>("LINE_ITEMDETAIL_ID") != null ? new Guid(lidac.Field<byte[]>("LINE_ITEMDETAIL_ID")) : new Guid(),
        Name = lidac.Field<string>("ADD_ON_CHARGE_NAME"),
        Code = lidac.Field<string>("ADD_ON_CHARGE_CODE"),
        Percentage = lidac.Field<object>("ADD_ON_CHARGE_PERCENTAGE") != null ? lidac.Field<double?>("ADD_ON_CHARGE_PERCENTAGE") : null,
        ChargeableAmount = lidac.Field<object>("ADD_ON_CHARGEABLE_AMOUNT") != null ? lidac.Field<decimal?>("ADD_ON_CHARGEABLE_AMOUNT") : null,
        Amount = lidac.Field<object>("ADD_ON_CHARGE_AMOUNT") != null ? lidac.Field<decimal>("ADD_ON_CHARGE_AMOUNT") : 0,
        LastUpdatedBy = lidac.Field<object>("LAST_UPDATED_BY") != null ? lidac.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = lidac.Field<object>("LAST_UPDATED_ON") != null ? lidac.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      });

    public  readonly Materializer<LineItemDetailAdditionalDetail> LineItemDetailAdditionalDetailMaterializer = new Materializer<LineItemDetailAdditionalDetail>(lidad =>
      new LineItemDetailAdditionalDetail
      {
        Id = lidad.Field<byte[]>("ADDITIONAL_DETAIL_ID") != null ? new Guid(lidad.Field<byte[]>("ADDITIONAL_DETAIL_ID")) : new Guid(),
        LineItemDetailId = lidad.Field<byte[]>("LINE_ITEMDETAIL_ID") != null ? new Guid(lidad.Field<byte[]>("LINE_ITEMDETAIL_ID")) : new Guid(),
        Name = lidad.Field<string>("ADDITIONAL_DETAIL"),
        Description = lidad.Field<string>("ADDITIONAL_DETAIL_DESC"),
        TypeId = lidad.Field<object>("ADDITIONAL_DETAIL_TYPE") != null ? lidad.Field<int>("ADDITIONAL_DETAIL_TYPE") : 0,
        LastUpdatedBy = lidad.Field<object>("LAST_UPDATED_BY") != null ? lidad.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = lidad.Field<object>("LAST_UPDATED_ON") != null ? lidad.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      });

    public  readonly Materializer<MiscCorrespondence> MiscCorrespondenceMaterializer = new Materializer<MiscCorrespondence>(mCorr =>
       new MiscCorrespondence
       {
         Id = mCorr.Field<byte[]>("CORRESPONDENCE_ID") != null ? new Guid(mCorr.Field<byte[]>("CORRESPONDENCE_ID")) : new Guid(),
         InvoiceId = mCorr.Field<byte[]>("INVOICE_ID") != null ? new Guid(mCorr.Field<byte[]>("INVOICE_ID")) : new Guid(),
         CorrespondenceOwnerId = mCorr.Field<object>("CORRESPONDENCE_OWNER_ID") != null ? mCorr.Field<int>("CORRESPONDENCE_OWNER_ID") : 0,
         CorrespondenceNumber = mCorr.Field<object>("CORRESPONDENCE_NO") != null ? (long?)Convert.ToInt64(mCorr.Field<object>("CORRESPONDENCE_NO")) : null,
         CorrespondenceDate = mCorr.Field<object>("CORRESPONDENCE_DATE") != null ? mCorr.Field<DateTime>("CORRESPONDENCE_DATE") : new DateTime(),
         CorrespondenceStage = mCorr.Field<object>("CORRESPONDENCE_STAGE") != null ? mCorr.Field<int>("CORRESPONDENCE_STAGE") : 0,
         FromMemberId = mCorr.Field<object>("FROM_MEMBER_ID") != null ? mCorr.Field<int>("FROM_MEMBER_ID") : 0,
         ToMemberId = mCorr.Field<object>("TO_MEMBER_ID") != null ? mCorr.Field<int>("TO_MEMBER_ID") : 0,
         ToEmailId = mCorr.Field<string>("TO_EMAILID"),
         AmountToBeSettled = mCorr.Field<object>("AMOUNT_TO_SETTLED") != null ? mCorr.Field<decimal>("AMOUNT_TO_SETTLED") : 0,
         OurReference = mCorr.Field<string>("OUR_REFERENCE"),
         YourReference = mCorr.Field<string>("YOUR_REFERENCE"),
         Subject = mCorr.Field<string>("SUBJECT"),
         CorrespondenceStatusId = mCorr.Field<object>("CORRESPONDENCE_STATUS") != null ? mCorr.Field<int>("CORRESPONDENCE_STATUS") : 0,
         CorrespondenceSubStatusId = mCorr.Field<object>("CORRESPONDENCE_SUB_STATUS") != null ? mCorr.Field<int>("CORRESPONDENCE_SUB_STATUS") : 0,
         AuthorityToBill = mCorr.Field<object>("AUTHORITY_TO_BILL") != null && mCorr.Field<int>("AUTHORITY_TO_BILL") > 0,
         NumberOfDaysToExpire = mCorr.Field<object>("NO_OF_DAYS_TO_EXPIRE") != null ? mCorr.Field<int>("NO_OF_DAYS_TO_EXPIRE") : 0,
         FromEmailId = mCorr.Field<string>("FROM_EMAILID"),
         CurrencyId = mCorr.Field<object>("CURRENCY_CODE_NUM") != null ? mCorr.Field<int?>("CURRENCY_CODE_NUM") : null,
         ExpiryDate = mCorr.Field<object>("EXPIRY_DATE") != null ? mCorr.Field<DateTime>("EXPIRY_DATE") : DateTime.MinValue,
         ExpiryDatePeriod = mCorr.Field<object>("EXPIRY_DATEPERIOD") != null ? mCorr.Field<DateTime?>("EXPIRY_DATEPERIOD") : null,
         BMExpiryPeriod = mCorr.Field<object>("BM_EXPIRY_PERIOD") != null ? mCorr.Field<DateTime?>("BM_EXPIRY_PERIOD") : null,
         ChargeCode = mCorr.Field<string>("CHARGE_CODE"),
         CorrespondenceDetails = mCorr.Field<string>("CORRESPONDENCE_DETAILS"),
         TransactionStatusId = mCorr.Field<object>("TRANSACTION_STATUS_ID") != null ? mCorr.Field<int>("TRANSACTION_STATUS_ID") : 0,
         AdditionalEmailInitiator = mCorr.Field<string>("ADDITIONAL_EMAIL_INITIATOR"),
         AdditionalEmailNonInitiator = mCorr.Field<string>("ADDITIONAL_EMAIL_NON_INITIATOR"),
         LastUpdatedBy = mCorr.Field<object>("LAST_UPDATED_BY") != null ? mCorr.Field<int>("LAST_UPDATED_BY") : 0,
         LastUpdatedOn = mCorr.Field<object>("LAST_UPDATED_ON") != null ? mCorr.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
         //CMP527: Get acceptance information from DB.
         AcceptanceComment = mCorr.Field<object>("ACCEPTANCE_COMMENTS") != null ? mCorr.Field<string>("ACCEPTANCE_COMMENTS") : null,
         AcceptanceUserName = mCorr.Field<object>("ACCEPTANCE_USER") != null ? mCorr.Field<string>("ACCEPTANCE_USER") : null,
         AcceptanceDateTime = mCorr.Field<object>("ACCEPTANCE_DATE") != null ? mCorr.Field<DateTime>("ACCEPTANCE_DATE") : new DateTime(),
       });

    public  readonly Materializer<MiscUatpCorrespondenceAttachment> MiscUatpCorrespondenceAttachmentMaterializer = new Materializer<MiscUatpCorrespondenceAttachment>(mca =>
  new MiscUatpCorrespondenceAttachment
  {
    //TODO: uncomment FileSize
    Id = mca.Field<byte[]>("CORRESPONDENCE_ATTACHMENT_ID") != null ? new Guid(mca.Field<byte[]>("CORRESPONDENCE_ATTACHMENT_ID")) : new Guid(),
    OriginalFileName = mca.Field<string>("ORG_FILE_NAME"),
    FileTypeId = mca.Field<object>("FILE_TYPE_ID") != null ? mca.Field<int>("FILE_TYPE_ID") : 0,
    FileStatusId = mca.Field<object>("FILE_STATUS_ID") != null ? mca.Field<int>("FILE_STATUS_ID") : 0,
    ReceivedDate = mca.Field<object>("RECEIVED_DATE") != null ? mca.Field<DateTime>("RECEIVED_DATE") : new DateTime(),
    ParentId = mca.Field<byte[]>("CORRESPONDENCE_ID") != null ? new Guid(mca.Field<byte[]>("CORRESPONDENCE_ID")) : new Guid(),
    FileSize = mca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(mca.Field<object>("FILE_SIZE")) : 0,
    FilePath = mca.Field<string>("FILE_PATH"),
    ServerId = mca.Field<object>("SERVER_ID") != null ? mca.Field<int>("SERVER_ID") : 0,
    LastUpdatedBy = mca.Field<object>("LAST_UPDATED_BY") != null ? mca.Field<int>("LAST_UPDATED_BY") : 0,
    LastUpdatedOn = mca.Field<object>("LAST_UPDATED_ON") != null ? mca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
  });

    public  readonly Materializer<InvoiceSummary> MiscUatpInvoiceSummaryMaterializer = new Materializer<InvoiceSummary>(ism => new InvoiceSummary
    {
      LastUpdatedOn = ism.Field<object>("LAST_UPDATED_ON") != null ? ism.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = ism.Field<object>("LAST_UPDATED_BY") != null ? ism.Field<int>("LAST_UPDATED_BY") : 0,
      TotalAmountInClearanceCurrency = ism.Field<object>("TOTAL_AMT_IN_CLEARNC_CURRENCY") != null ? ism.Field<decimal?>("TOTAL_AMT_IN_CLEARNC_CURRENCY") : (decimal?)null,
      TotalAmountWithoutVat = ism.Field<object>("TOTAL_AMT_WITHOUT_VAT") != null ? ism.Field<decimal>("TOTAL_AMT_WITHOUT_VAT") : 0,
      TotalAmount = ism.Field<object>("TOTAL_AMOUNT") != null ? ism.Field<decimal>("TOTAL_AMOUNT") : 0,
      TotalVatAmount = ism.Field<object>("TOTAL_VAT_AMOUNT") != null ? ism.Field<decimal?>("TOTAL_VAT_AMOUNT") : null,
      TotalTaxAmount = ism.Field<object>("TOTAL_TAX_AMOUNT") != null ? ism.Field<decimal?>("TOTAL_TAX_AMOUNT") : null,
      TotalAddOnChargeAmount = ism.Field<object>("TOTAL_ADD_ON_CHARGE_AMOUNT") != null ? ism.Field<decimal?>("TOTAL_ADD_ON_CHARGE_AMOUNT") : null,
      TotalLineItemAmount = ism.Field<object>("TOTAL_LINE_ITEM_AMOUNT") != null ? ism.Field<decimal>("TOTAL_LINE_ITEM_AMOUNT") : 0,
      LineItemCount = ism.Field<object>("LINE_ITEM_COUNT") != null ? ism.Field<int>("LINE_ITEM_COUNT") : 0,
      InvoiceId = ism.Field<byte[]>("INVOICE_ID") != null ? new Guid(ism.Field<byte[]>("INVOICE_ID")) : new Guid(),
    });

    public readonly Materializer<MiscUatpInvoiceTax> MiscUatpInvoiceTaxMaterializer = new Materializer<MiscUatpInvoiceTax>(mtm =>
        new MiscUatpInvoiceTax
        {
          LastUpdatedOn = mtm.Field<object>("LAST_UPDATED_ON") != null ? mtm.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
          LastUpdatedBy = mtm.Field<object>("LAST_UPDATED_BY") != null ? mtm.Field<int>("LAST_UPDATED_BY") : 0,
          CalculatedAmount = mtm.Field<object>("TAXABLE_AMOUNT") != null ? mtm.Field<decimal?>("TAXABLE_AMOUNT") : null,
          Amount = mtm.Field<object>("BASE_AMOUNT") != null ? mtm.Field<decimal?>("BASE_AMOUNT") : null,
          Percentage = mtm.Field<object>("TAX_PERCENT") != null ? mtm.Field<double?>("TAX_PERCENT") : null,
          Description = mtm.Field<string>("TAX_TEXT"),
          CategoryCode = mtm.Field<string>("TAX_CATEGORY"),
          SubType = mtm.Field<string>("TAX_SUB_TYPE"),
          Type = mtm.Field<string>("TAX_TYPE"),
          SubdivisionCode = mtm.Field<string>("SUBDIVISION_CODE"),
          CountryId = mtm.Field<string>("COUNTRY_CODE"),
          RegistrationId = mtm.Field<string>("TAX_REGISTRATION_ID"),
          ParentId = mtm.Field<byte[]>("INVOICE_ID") != null ? new Guid(mtm.Field<byte[]>("INVOICE_ID")) : new Guid(),
          Id = mtm.Field<byte[]>("INVOICE_TAX_ID") != null ? new Guid(mtm.Field<byte[]>("INVOICE_TAX_ID")) : new Guid(),
          CountryCodeIcao = mtm.Field<string>("COUNTRY_CODE_ICAO"),
        });
    public  readonly Materializer<MiscUatpAttachment> MiscUatpAttachmentMaterializer = new Materializer<MiscUatpAttachment>(mam =>
      new MiscUatpAttachment
      {
        // TODO: uncomment FileSize
        FileStatusId = mam.Field<object>("FILE_STATUS_ID") != null ? mam.Field<int>("FILE_STATUS_ID") : 0,
        FileTypeId = mam.Field<object>("FILE_TYPE_ID") != null ? mam.Field<int>("FILE_TYPE_ID") : 0,
        FilePath = mam.Field<string>("FILE_PATH"),
        ServerId = mam.Field<object>("SERVER_ID") != null ? mam.Field<int>("SERVER_ID") : 0,
        LastUpdatedOn = mam.Field<object>("LAST_UPDATED_ON") != null ? mam.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = mam.Field<object>("LAST_UPDATED_BY") != null ? mam.Field<int>("LAST_UPDATED_BY") : 0,
        FileSize = mam.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(mam.Field<object>("FILE_SIZE")) : 0,
        OriginalFileName = mam.Field<string>("FILE_NAME"),
        ParentId = mam.Field<byte[]>("INVOICE_ID") != null ? new Guid(mam.Field<byte[]>("INVOICE_ID")) : new Guid(),
        IsFullPath = mam.Field<object>("IS_FULL_PATH") != null ? (mam.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
        Id = mam.Field<byte[]>("ATTACHMENT_ID") != null ? new Guid(mam.Field<byte[]>("ATTACHMENT_ID")) : new Guid(),
      });

    public  readonly Materializer<MiscUatpInvoiceTaxAdditionalDetail> MiscUatpInvoiceTaxAdditionalDetailMaterializer =
      new Materializer<MiscUatpInvoiceTaxAdditionalDetail>(r => new MiscUatpInvoiceTaxAdditionalDetail
      {
        Id = r.Field<byte[]>("ADDITIONAL_DETAIL_ID") != null ? new Guid(r.Field<byte[]>("ADDITIONAL_DETAIL_ID")) : new Guid(),
        MiscInvoiceTaxId = r.Field<byte[]>("INVOICE_TAX_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_TAX_ID")) : new Guid(),
        Name = r.Field<object>("ADDITIONAL_DETAIL") != null ? r.Field<string>("ADDITIONAL_DETAIL") : string.Empty,
        Description = r.Field<object>("ADDITIONAL_DETAIL_DESC") != null ? r.Field<string>("ADDITIONAL_DETAIL_DESC") : string.Empty,
        TypeId = r.Field<object>("ADDITIONAL_DETAIL_TYPE") != null ? r.Field<int>("ADDITIONAL_DETAIL_TYPE") : 0,
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0
      });

    public  readonly Materializer<OtherOrganizationAdditionalDetail> OtherOrganizationAdditionalDetailMaterializer =
        new Materializer<OtherOrganizationAdditionalDetail>(r => new OtherOrganizationAdditionalDetail
        {
          Id = r.Field<byte[]>("ADDITIONAL_DETAIL_ID") != null ? new Guid(r.Field<byte[]>("ADDITIONAL_DETAIL_ID")) : new Guid(),
          OtherOrganizationInfoId = r.Field<byte[]>("OTHER_ORGANIZATION_INFO_ID") != null ? new Guid(r.Field<byte[]>("OTHER_ORGANIZATION_INFO_ID")) : new Guid(),
          AdditionalDetail = r.Field<object>("ADDITIONAL_DETAIL") != null ? r.Field<string>("ADDITIONAL_DETAIL") : string.Empty,
          AdditionalDetailDescription = r.Field<object>("ADDITIONAL_DETAIL_DESC") != null ? r.Field<string>("ADDITIONAL_DETAIL_DESC") : string.Empty,
          AdditionalDetailType = r.Field<object>("ADDITIONAL_DETAIL_TYPE") != null ? r.Field<int>("ADDITIONAL_DETAIL_TYPE") : 0,
          LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
          LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0
        });


    public  readonly Materializer<OtherOrganizationContact> OtherOrganizationContactMaterializer =
      new Materializer<OtherOrganizationContact>(r => new OtherOrganizationContact
      {
        Id = r.Field<byte[]>("OTHER_ORG_CONTACT_ID") != null ? new Guid(r.Field<byte[]>("OTHER_ORG_CONTACT_ID")) : new Guid(),
        OtherOrganizationId = r.Field<byte[]>("OTHER_ORGANIZATION_INFO_ID") != null ? new Guid(r.Field<byte[]>("OTHER_ORGANIZATION_INFO_ID")) : new Guid(),
        Type = r.Field<object>("CONTACT_TYPE") != null ? r.Field<string>("CONTACT_TYPE") : string.Empty,
        Value = r.Field<object>("CONTACT_VALUE") != null ? r.Field<string>("CONTACT_VALUE") : string.Empty,
        Description = r.Field<object>("CONTACT_DESCRIPTION") != null ? r.Field<string>("CONTACT_DESCRIPTION") : string.Empty,
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0
      });

    public  readonly Materializer<LineItemDetailTaxAdditionalDetail> LineItemDetailTaxAdditionalDetailMaterializer =
      new Materializer<LineItemDetailTaxAdditionalDetail>(r => new LineItemDetailTaxAdditionalDetail
      {
        Id = r.Field<byte[]>("ADDITIONAL_DETAIL_ID") != null ? new Guid(r.Field<byte[]>("ADDITIONAL_DETAIL_ID")) : new Guid(),
        LineItemDetailTaxId = r.Field<byte[]>("LINEITEM_DET_TAX_ID") != null ? new Guid(r.Field<byte[]>("LINEITEM_DET_TAX_ID")) : new Guid(),
        Name = r.Field<object>("ADDITIONAL_DETAIL") != null ? r.Field<string>("ADDITIONAL_DETAIL") : string.Empty,
        Description = r.Field<object>("ADDITIONAL_DETAIL_DESC") != null ? r.Field<string>("ADDITIONAL_DETAIL_DESC") : string.Empty,
        TypeId = r.Field<object>("ADDITIONAL_DETAIL_TYPE") != null ? r.Field<int>("ADDITIONAL_DETAIL_TYPE") : 0,
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0
      });

    public  readonly Materializer<OtherOrganizationInformation> OtherOrganizationInformationMaterializer =
     new Materializer<OtherOrganizationInformation>(r => new OtherOrganizationInformation
     {
       Id = r.Field<byte[]>("OTHER_ORGANIZATION_INFO_ID") != null ? new Guid(r.Field<byte[]>("OTHER_ORGANIZATION_INFO_ID")) : new Guid(),
       InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
       LegalName = r.Field<object>("ORG_LEGAL_NAME") != null ? r.Field<string>("ORG_LEGAL_NAME") : string.Empty,
       RegistrationId = r.Field<object>("ORG_REG_ID") != null ? r.Field<string>("ORG_REG_ID") : string.Empty,
       TaxVatRegistrationId = r.Field<object>("TAX_VAT_REG_ID") != null ? r.Field<string>("TAX_VAT_REG_ID") : string.Empty,
       AddTaxVatRegistrationNumber = r.Field<object>("ADD_TAX_VAT_REGISTRATION_NUM") != null ? r.Field<string>("ADD_TAX_VAT_REGISTRATION_NUM") : string.Empty,
       AddressLine1 = r.Field<object>("ADDRESS_LINE1") != null ? r.Field<string>("ADDRESS_LINE1") : string.Empty,
       AddressLine2 = r.Field<object>("ADDRESS_LINE2") != null ? r.Field<string>("ADDRESS_LINE2") : string.Empty,
       AddressLine3 = r.Field<object>("ADDRESS_LINE3") != null ? r.Field<string>("ADDRESS_LINE3") : string.Empty,
       CityName = r.Field<object>("CITY_NAME") != null ? r.Field<string>("CITY_NAME") : string.Empty,
       SubDivisionCode = r.Field<object>("SUB_DIVISION_CODE") != null ? r.Field<string>("SUB_DIVISION_CODE") : string.Empty,
       SubDivisionName = r.Field<object>("SUB_DIVISION_NAME") != null ? r.Field<string>("SUB_DIVISION_NAME") : string.Empty,
       PostalCode = r.Field<object>("POSTAL_CODE") != null ? r.Field<string>("POSTAL_CODE") : string.Empty,
       CountryCode = r.Field<object>("COUNTRY_CODE") != null ? r.Field<string>("COUNTRY_CODE") : string.Empty,
       LegalText = r.Field<object>("LEGAL_TEXT") != null ? r.Field<string>("LEGAL_TEXT") : string.Empty,
       Iban = r.Field<object>("IBAN") != null ? r.Field<string>("IBAN") : string.Empty,
       Swift = r.Field<object>("SWIFT") != null ? r.Field<string>("SWIFT") : string.Empty,
       BankCode = r.Field<object>("BANK_CODE") != null ? r.Field<string>("BANK_CODE") : string.Empty,
       BankName = r.Field<object>("BANK_NAME") != null ? r.Field<string>("BANK_NAME") : string.Empty,
       BranchCode = r.Field<object>("BRANCH_CODE") != null ? r.Field<string>("BRANCH_CODE") : string.Empty,
       BankAccountNumber = r.Field<object>("BANK_ACCOUNT_NUMBER") != null ? r.Field<string>("BANK_ACCOUNT_NUMBER") : string.Empty,
       BankAccountName = r.Field<object>("BANK_ACCOUNT_NAME") != null ? r.Field<string>("BANK_ACCOUNT_NAME") : string.Empty,
       CurrencyId = r.Field<object>("CURRENCY_CODE_NUM") != null ? r.Field<int?>("CURRENCY_CODE_NUM") : null,
       OrganizationTypeId = r.Field<object>("ORGANIZATION_TYPE_ID") != null ? r.Field<int>("ORGANIZATION_TYPE_ID") : 0,
       OrganizationId = r.Field<object>("ORGANIZATION_ID") != null ? r.Field<string>("ORGANIZATION_ID") : string.Empty,
       OrganizationDesignator = r.Field<object>("ORGANIZATION_DESIGNATOR") != null ? r.Field<string>("ORGANIZATION_DESIGNATOR") : string.Empty,
       CountryCodeIcao = r.Field<object>("COUNTRYCODE_ICAO") != null ? r.Field<string>("COUNTRYCODE_ICAO") : string.Empty,
       CountryName = r.Field<object>("COUNTRY_NAME") != null ? r.Field<string>("COUNTRY_NAME") : string.Empty,
       LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
       LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
       ContactName = r.Field<string>("OTHER_ORG_CONTACT_NAME")

     });

    public  readonly Materializer<DataSource> DataSourceMaterializer = new Materializer<DataSource>(r =>
   new DataSource
   {
     LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
     LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
     ValueColumnName = r.Field<string>("VALUE_COLUMN"),
     DisplayColumnName = r.Field<string>("DISPLAY_COLUMN"),
     TableName = r.Field<string>("TABLE_NAME"),
     Id = r.TryGetField<object>("DATA_SOURCE_ID") != null ? r.Field<int>("DATA_SOURCE_ID") : 0,
     WhereClause = r.Field<string>("WHERE_CLAUSE"),
     SubstituteValue = r.Field<string>("SUBSTITUTE_VALUE"),
   });

 }
}

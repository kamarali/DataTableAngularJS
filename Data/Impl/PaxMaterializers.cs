using System;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;

namespace Iata.IS.Data.Impl
{
  public class PaxMaterializers
  {
    public readonly Materializer<PaxInvoice> InvoiceMaterializer = new Materializer<PaxInvoice>(r =>
        new PaxInvoice
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
          InvoiceDate = r.Field<object>("INVOICE_DATE") != null ? r.Field<DateTime>("INVOICE_DATE") : new DateTime(),
          DigitalSignatureRequiredId = r.Field<object>("DS_REQUIRED_ID") != null ? r.Field<int>("DS_REQUIRED_ID") : 0,
          SettlementMethodId = r.Field<object>("SETTLEMENT_METHOD_ID") != null ? r.Field<int>("SETTLEMENT_METHOD_ID") : 0,
          BillingPeriod = r.Field<object>("PERIOD_NO") != null ? r.Field<int>("PERIOD_NO") : 0,
          BillingCurrencyId = r.Field<object>("BILLING_CURRENCY_CODE_NUM") != null ? r.Field<int>("BILLING_CURRENCY_CODE_NUM") : 0,
          ListingCurrencyId = r.Field<object>("LISTING_CURRENCY_CODE_NUM") != null ? r.Field<int>("LISTING_CURRENCY_CODE_NUM") : 0,
          BillingMonth = r.Field<object>("BILLING_MONTH") != null ? r.Field<int>("BILLING_MONTH") : 0,
          InvoiceNumber = r.Field<string>("INVOICE_NO"),
          BillingCode = r.Field<object>("BILLING_CODE_ID") != null ? r.Field<int>("BILLING_CODE_ID") : 0,
          BillingMemberLocationCode = r.Field<string>("BILLING_MEM_LOC_CODE"),
          BilledMemberLocationCode = r.Field<string>("BILLED_MEM_LOC_CODE"),
          BillingReferenceDataSourceId = r.Field<object>("BILLING_REF_DATA_SOURCE_ID") != null ? r.Field<int>("BILLING_REF_DATA_SOURCE_ID") : 0,
          BilledReferenceDataSourceId = r.Field<object>("BILLED_REF_DATA_SOURCE_ID") != null ? r.Field<int>("BILLED_REF_DATA_SOURCE_ID") : 0,
          InvoiceOwnerId = r.Field<object>("INVOICE_OWNER_ID") != null ? Convert.ToInt32(r.Field<object>("INVOICE_OWNER_ID")) : 0,
          LegalPdfLocation = r.Field<string>("LEGAL_PDF_LOCATION"),
          SupportingAttachmentStatusId = r.Field<object>("SUPPORTING_ATTACHMENT_STATUS") != null ? r.Field<int>("SUPPORTING_ATTACHMENT_STATUS") : 0,
          IsFutureSubmission = r.Field<object>("IS_FUTURE_SUBMISSION") != null ? (r.Field<int>("IS_FUTURE_SUBMISSION") == 0 ? false : true) : false,
          LegalXmlLocation = r.Field<string>("legal_xml_location"),
          ProvisionalBillingMonth = r.Field<object>("PROV_BILLING_MONTH") != null ? r.Field<int>("PROV_BILLING_MONTH") : 0,
          InvoiceTypeId = r.Field<object>("INVOICE_TYPE_ID") != null ? r.Field<int>("INVOICE_TYPE_ID") : 0,
          ProvisionalBillingYear = r.Field<object>("PROV_BILLING_YEAR") != null ? r.Field<int>("PROV_BILLING_YEAR") : 0,
          SamplingConstant = r.TryGetField<object>("SAMPLING_CONSTANT") != null ? r.Field<double>("SAMPLING_CONSTANT") : 0.0,
          IsFormDEViaIS = r.Field<object>("IS_FORM_DE_VIA_IS") != null ? (r.Field<int>("IS_FORM_DE_VIA_IS") == 0 ? false : true) : false,
          IsFormFViaIS = r.Field<object>("IS_FORM_F_VIA_IS") != null ? (r.Field<int>("IS_FORM_F_VIA_IS") == 0 ? false : true) : false,
          IsFormABViaIS = r.Field<object>("IS_FORM_AB_VIA_IS") != null ? (r.Field<int>("IS_FORM_AB_VIA_IS") == 0 ? false : true) : false,
          IsFormCViaIS = r.Field<object>("IS_FORM_C_VIA_IS") != null ? (r.Field<int>("IS_FORM_C_VIA_IS") == 0 ? false : true) : false,
          XmlSignatureLocation = r.Field<string>("XML_SIGNATURE_LOCATION"),
          XmlVerificationLogLocation = r.Field<string>("XML_VERIFICATION_LOG_LOCATION"),
          InvTemplateLanguage = r.Field<string>("INV_TEMPLATE_LANGUAGE"),
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
          MiscBilledMemberLocCode =  r.Field<string>("MISC_BLD_MEM_LOC_CODE")
        });

    // AutoBill Coupon repository is loaded for status = 3 coupon.
    public readonly Materializer<AutoBillingPrimeCoupon> AutoBillCouponMaterializer = new Materializer<AutoBillingPrimeCoupon>(r => new AutoBillingPrimeCoupon
    {
      Id = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
      BatchSequenceNumber = r.Field<object>("BATCH_SEQ_NO") != null ? r.Field<int>("BATCH_SEQ_NO") : 0,
      RecordSequenceWithinBatch = r.Field<object>("BATCH_RECORD_SEQ") != null ? r.Field<int>("BATCH_RECORD_SEQ") : 0,
      TicketOrFimIssuingAirline = r.Field<object>("TICKET_ISSUING_AIRLINE") != null ? r.Field<string>("TICKET_ISSUING_AIRLINE") : string.Empty,
      TicketOrFimCouponNumber = r.Field<object>("TICKET_COUPON_NO") != null ? r.Field<int>("TICKET_COUPON_NO") : 0,
      TicketDocOrFimNumber = r.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOC_NO")) : 0,
      CheckDigit = r.Field<object>("CHECK_DIGIT") != null ? r.Field<int>("CHECK_DIGIT") : 0, //test
      CouponGrossValueOrApplicableLocalFare = r.Field<object>("COUPON_GROSS_VALUE") != null ? r.Field<double>("COUPON_GROSS_VALUE") : 0.0,
      IscPercent = r.Field<object>("INTERLINE_SERV_CHARGE_PER") != null ? r.Field<double>("INTERLINE_SERV_CHARGE_PER") : 0.0,
      TaxAmount = r.Field<object>("COUPON_TAX_AMT") != null ? r.Field<double>("COUPON_TAX_AMT") : 0.0,
      CurrencyAdjustmentIndicator = r.Field<string>("CURRENCY_ADJSTMNT_IND"),
      SourceCodeId = r.Field<object>("SOURCE_CODE") != null ? r.Field<int>("SOURCE_CODE") : 0,
      ElectronicTicketIndicator = r.Field<object>("ETICKET_INDICATOR") != null ? (r.Field<int>("ETICKET_INDICATOR") == 0 ? false : true) : false,
      OriginalPmi = r.Field<string>("ORIGINAL_PMI"),
      ValidatedPmi = r.Field<string>("VALIDATED_PMI"),
      AirlineFlightDesignator = r.Field<string>("AIRLINE_FLIGHT_DESIGNATOR"),
      FlightNumber = r.Field<object>("FLIGHT_NO") != null ? r.Field<int?>("FLIGHT_NO") : null,
      FromAirportOfCoupon = r.Field<string>("FROM_AIRPORT_OF_COUPON"),
      ToAirportOfCoupon = r.Field<string>("TO_AIRPORT_OF_COUPON"),
      FilingReference = r.Field<string>("FILLING_REF"),//ok
      HandlingFeeTypeId = r.Field<object>("HANDLING_FEE_TYPE_ID") != null ? r.Field<string>("HANDLING_FEE_TYPE_ID") : string.Empty,
      HandlingFeeAmount = r.Field<object>("HANDLING_FEE_AMT") != null ? r.Field<double>("HANDLING_FEE_AMT") : 0.0,
      SettlementAuthorizationCode = r.Field<string>("SETTLEMENT_AUTH_CODE"),
      IscAmount = r.Field<object>("ISC_AMT") != null ? r.Field<double>("ISC_AMT") : 0.0,
      OtherCommissionPercent = r.Field<object>("OTH_COMM_PER") != null ? r.Field<double>("OTH_COMM_PER") : 0.0,
      OtherCommissionAmount = r.Field<object>("OTH_COMM_AMT") != null ? r.Field<double>("OTH_COMM_AMT") : 0.0,
      UatpPercent = r.Field<object>("UATP_PER") != null ? r.Field<double>("UATP_PER") : 0.0,
      UatpAmount = r.Field<object>("UATP_AMT") != null ? r.Field<double>("UATP_AMT") : 0.0,
      VatAmount = r.Field<object>("VAT_AMT") != null ? r.Field<double>("VAT_AMT") : 0.0,
      CouponTotalAmount = r.Field<object>("COUPON_TOTAL_AMT") != null ? r.Field<double>("COUPON_TOTAL_AMT") : 0.0,
      CabinClass = r.Field<string>("CABIN_CLASS"),
      ProrateMethodology = r.Field<string>("PRORATE_METHODOLOGY"),
      NfpReasonCode = r.Field<string>("NFP_REASON_CODE"),
      AgreementIndicatorSupplied = r.Field<string>("AGREEMENT_INDI_SUPPLIED"),
      AgreementIndicatorValidated = r.Field<string>("AGREEMENT_IND_VALIDATED"),
      AttachmentIndicatorOriginal = r.Field<object>("ATTACHMENT_IND_ORIG") != null ? (r.Field<int>("ATTACHMENT_IND_ORIG") == 0 ? 0 : (r.Field<int>("ATTACHMENT_IND_ORIG") == 0 ? 0 : 1)) : 0,
      AttachmentIndicatorValidated = r.Field<object>("ATTACHMENT_IND_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_IND_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
      NumberOfAttachments = r.Field<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int?>("NO_OF_ATTACHMENTS") : null,
      SurchargeAmount = r.Field<object>("SURCHARGE_AMOUNT") != null ? r.Field<double>("SURCHARGE_AMOUNT") : 0.0,
      ISValidationFlag = r.Field<string>("IS_VALIDATION_FLAG"),
      ReasonCode = r.Field<string>("REASON_CODE"),
      ReferenceField1 = r.Field<string>("REFERENCE_FIELD1"),
      ReferenceField2 = r.Field<string>("REFERENCE_FIELD2"),
      ReferenceField3 = r.Field<string>("REFERENCE_FIELD3"),
      ReferenceField4 = r.Field<string>("REFERENCE_FIELD4"),
      ReferenceField5 = r.Field<string>("REFERENCE_FIELD5"),
      AirlineOwnUse = r.Field<string>("AIRLINE_OWN_USE"),
      InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
      LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      FlightDate = (DateTime?)(r.Field<object>("FLIGHT_DATE")), //Changes to set FlightDate as null if value is not selected
      IncludeInDailyRevenueRecogn = r.Field<object>("INCLUDE_IN_DAILY_REV_RECOGN") != null ? (r.Field<int>("INCLUDE_IN_DAILY_REV_RECOGN") == 0 ? false : true) : false,
      AutoBillingCouponStatus = r.Field<object>("STATUS") != null ? r.Field<int>("STATUS") : 0                                                                                                                          
    });

    // AutoBill Coupon tax repository is loaded for status = 3 coupon.
    public readonly Materializer<AutoBillingPrimeCouponTax> AutoBillCouponTaxMaterializer = new Materializer<AutoBillingPrimeCouponTax>(r =>
    new AutoBillingPrimeCouponTax
    {
      Id = r.Field<byte[]>("COUPON_TAX_BD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_TAX_BD_ID")) : new Guid(),
      TaxCode = r.Field<string>("TAX_CODE"),
      Amount = r.Field<object>("TAX_AMT_BILLED") != null ? r.Field<double>("TAX_AMT_BILLED") : 0,
      Status = r.Field<object>("STATUS") != null ? r.Field<int>("STATUS") : 0,
      ParentId = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
      LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime()
    });

    // AutoBill Coupon vat repository is loaded for status = 3 coupon.
    public readonly Materializer<AutoBillingPrimeCouponVat> AutoBillCouponVatMaterializer = new Materializer<AutoBillingPrimeCouponVat>(r =>
     new AutoBillingPrimeCouponVat
     {
       Id = r.Field<byte[]>("COUPON_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_VAT_BD_ID")) : new Guid(),
       VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
       VatLabel = r.Field<string>("VAT_LABEL"),
       VatText = r.Field<string>("VAT_TEXT"),
       VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0,
       VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0,
       VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0,
       Status = r.Field<object>("STATUS") != null ? r.Field<int>("STATUS") : 0,
       ParentId = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
       LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
       LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
     });

    // AutoBill Coupon attachment repository is loaded for status = 3 coupon.
    public readonly Materializer<AutoBillingPrimeCouponAttachment> AutoBillCpnAttachmentMaterializer = new Materializer<AutoBillingPrimeCouponAttachment>(pca =>
   new AutoBillingPrimeCouponAttachment
   {
     ServerId = pca.Field<object>("SERVER_ID") != null ? pca.Field<int>("SERVER_ID") : 0,
     FilePath = pca.Field<string>("FILE_PATH"),
     OriginalFileName = pca.Field<string>("ORG_FILE_NAME"),
     LastUpdatedOn = pca.Field<object>("LAST_UPDATED_ON") != null ? pca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
     LastUpdatedBy = pca.Field<object>("LAST_UPDATED_BY") != null ? pca.Field<int>("LAST_UPDATED_BY") : 0,
     ParentId = pca.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(pca.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
     Status = pca.Field<object>("STATUS") != null ? pca.Field<int>("STATUS") : 0,
     FileStatusId = pca.Field<object>("FILE_STATUS_ID") != null ? pca.Field<int>("FILE_STATUS_ID") : 0,
     FileTypeId = pca.Field<object>("FILE_TYPE_ID") != null ? pca.Field<int>("FILE_TYPE_ID") : 0,
     FileSize = pca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(pca.Field<object>("FILE_SIZE")) : 0,
     IsFullPath = pca.Field<object>("IS_FULL_PATH") != null ? (pca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
     Id = pca.Field<byte[]>("COUPON_RECORD_ATTACHMENT_ID") != null ? new Guid(pca.Field<byte[]>("COUPON_RECORD_ATTACHMENT_ID")) : new Guid(),
   });


    public readonly Materializer<PrimeCoupon> CouponMaterializer = new Materializer<PrimeCoupon>(r =>
        new PrimeCoupon
        {
          Id = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
          BatchSequenceNumber = r.Field<object>("BATCH_SEQ_NO") != null ? r.Field<int>("BATCH_SEQ_NO") : 0,
          RecordSequenceWithinBatch = r.Field<object>("BATCH_RECORD_SEQ") != null ? r.Field<int>("BATCH_RECORD_SEQ") : 0,
          TicketOrFimIssuingAirline = r.Field<object>("TICKET_ISSUING_AIRLINE") != null ? r.Field<string>("TICKET_ISSUING_AIRLINE") : string.Empty,
          TicketOrFimCouponNumber = r.Field<object>("TICKET_COUPON_NO") != null ? r.Field<int>("TICKET_COUPON_NO") : 0,
          TicketDocOrFimNumber = r.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOC_NO")) : 0,
          CheckDigit = r.Field<object>("CHECK_DIGIT") != null ? r.Field<int>("CHECK_DIGIT") : 0, //test
          CouponGrossValueOrApplicableLocalFare = r.Field<object>("COUPON_GROSS_VALUE") != null ? r.Field<double>("COUPON_GROSS_VALUE") : 0.0,
          IscPercent = r.Field<object>("INTERLINE_SERV_CHARGE_PER") != null ? r.Field<double>("INTERLINE_SERV_CHARGE_PER") : 0.0,
          TaxAmount = r.Field<object>("COUPON_TAX_AMT") != null ? r.Field<double>("COUPON_TAX_AMT") : 0.0,
          CurrencyAdjustmentIndicator = r.Field<string>("CURRENCY_ADJSTMNT_IND"),
          SourceCodeId = r.Field<object>("SOURCE_CODE") != null ? r.Field<int>("SOURCE_CODE") : 0,
          ElectronicTicketIndicator = r.Field<object>("ETICKET_INDICATOR") != null ? (r.Field<int>("ETICKET_INDICATOR") == 0 ? false : true) : false,
          OriginalPmi = r.Field<string>("ORIGINAL_PMI"),
          ValidatedPmi = r.Field<string>("VALIDATED_PMI"),
          AirlineFlightDesignator = r.Field<string>("AIRLINE_FLIGHT_DESIGNATOR"),
          FlightNumber = r.Field<object>("FLIGHT_NO") != null ? r.Field<int?>("FLIGHT_NO") : null,
          FromAirportOfCoupon = r.Field<string>("FROM_AIRPORT_OF_COUPON"),
          ToAirportOfCoupon = r.Field<string>("TO_AIRPORT_OF_COUPON"),
          FilingReference = r.Field<string>("FILLING_REF"),//ok
          HandlingFeeTypeId = r.Field<object>("HANDLING_FEE_TYPE_ID") != null ? r.Field<string>("HANDLING_FEE_TYPE_ID") : string.Empty,
          HandlingFeeAmount = r.Field<object>("HANDLING_FEE_AMT") != null ? r.Field<double>("HANDLING_FEE_AMT") : 0.0,
          SettlementAuthorizationCode = r.Field<string>("SETTLEMENT_AUTH_CODE"),
          IscAmount = r.Field<object>("ISC_AMT") != null ? r.Field<double>("ISC_AMT") : 0.0,
          OtherCommissionPercent = r.Field<object>("OTH_COMM_PER") != null ? r.Field<double>("OTH_COMM_PER") : 0.0,
          OtherCommissionAmount = r.Field<object>("OTH_COMM_AMT") != null ? r.Field<double>("OTH_COMM_AMT") : 0.0,
          UatpPercent = r.Field<object>("UATP_PER") != null ? r.Field<double>("UATP_PER") : 0.0,
          UatpAmount = r.Field<object>("UATP_AMT") != null ? r.Field<double>("UATP_AMT") : 0.0,
          VatAmount = r.Field<object>("VAT_AMT") != null ? r.Field<double>("VAT_AMT") : 0.0,
          CouponTotalAmount = r.Field<object>("COUPON_TOTAL_AMT") != null ? r.Field<double>("COUPON_TOTAL_AMT") : 0.0,
          CabinClass = r.Field<string>("CABIN_CLASS"),
          ProrateMethodology = r.Field<string>("PRORATE_METHODOLOGY"),
          NfpReasonCode = r.Field<string>("NFP_REASON_CODE"),
          AgreementIndicatorSupplied = r.Field<string>("AGREEMENT_INDI_SUPPLIED"),
          AgreementIndicatorValidated = r.Field<string>("AGREEMENT_IND_VALIDATED"),
          AttachmentIndicatorOriginal = r.Field<object>("ATTACHMENT_IND_ORIG") != null ? (r.Field<int>("ATTACHMENT_IND_ORIG") == 0 ? 0 : (r.Field<int>("ATTACHMENT_IND_ORIG") == 1 ? 1:2) ) : 0 ,
          AttachmentIndicatorValidated = r.Field<object>("ATTACHMENT_IND_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_IND_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
          NumberOfAttachments = r.Field<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int?>("NO_OF_ATTACHMENTS") : null,
          SurchargeAmount = r.Field<object>("SURCHARGE_AMOUNT") != null ? r.Field<double>("SURCHARGE_AMOUNT") : 0.0,
          ISValidationFlag = r.Field<string>("IS_VALIDATION_FLAG"),
          ReasonCode = r.Field<string>("REASON_CODE"),
          ReferenceField1 = r.Field<string>("REFERENCE_FIELD1"),
          ReferenceField2 = r.Field<string>("REFERENCE_FIELD2"),
          ReferenceField3 = r.Field<string>("REFERENCE_FIELD3"),
          ReferenceField4 = r.Field<string>("REFERENCE_FIELD4"),
          ReferenceField5 = r.Field<string>("REFERENCE_FIELD5"),
          AirlineOwnUse = r.Field<string>("AIRLINE_OWN_USE"),
          InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
          LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
          LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
          FlightDate = (DateTime?)(r.Field<object>("FLIGHT_DATE")), //Changes to set FlightDate as null if value is not selected
          IncludeInDailyRevenueRecogn = r.Field<object>("INCLUDE_IN_DAILY_REV_RECOGN") != null ? (r.Field<int>("INCLUDE_IN_DAILY_REV_RECOGN") == 0 ? false : true) : false,
          AutoBillingCouponStatus = r.Field<object>("AUTO_BILL_COUPON_STATUS") != null ? r.Field<int>("AUTO_BILL_COUPON_STATUS") : 0
        });

    public readonly Materializer<BMCoupon> BMCouponMaterializer = new Materializer<BMCoupon>(r =>
     new BMCoupon
     {
       Id = r.Field<byte[]>("BM_COUPON_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_COUPON_BD_ID")) : new Guid(),
       SerialNo = r.Field<object>("BM_BD_SR_NO") != null ? r.Field<int>("BM_BD_SR_NO") : 0,
       TicketOrFimIssuingAirline = r.Field<string>("TICKET_ISSUING_AIRLINE"),
       TicketOrFimCouponNumber = r.Field<object>("COUPON_NO") != null ? r.Field<int>("COUPON_NO") : 0,
       TicketDocOrFimNumber = r.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOC_NO")) : 0,
       CheckDigit = r.Field<object>("CHECK_DIGIT") != null ? r.Field<int>("CHECK_DIGIT") : 0, //test
       ToAirportOfCoupon = r.Field<string>("TO_AIRPORT_OF_COUPON"),
       GrossAmountBilled = r.Field<object>("GROSS_AMT") != null ? r.Field<decimal>("GROSS_AMT") : 0,
       TaxAmount = r.Field<object>("TAX_AMT") != null ? r.Field<double>("TAX_AMT") : 0.0,
       IscPercent = r.Field<object>("ALLOWED_ISC_PER") != null ? r.Field<double>("ALLOWED_ISC_PER") : 0.0,
       IscAmountBilled = r.Field<object>("ISC_AMT") != null ? r.Field<double>("ISC_AMT") : 0.0,
       OtherCommissionPercent = r.Field<object>("ALLOWED_OTH_COMM_PER") != null ? r.Field<double>("ALLOWED_OTH_COMM_PER") : 0.0,
       OtherCommissionBilled = r.Field<object>("OTH_COMM_AMT") != null ? r.Field<double>("OTH_COMM_AMT") : 0.0,
       HandlingFeeAmount = r.Field<object>("HANDLING_FEE_AMT") != null ? r.Field<double>("HANDLING_FEE_AMT") : 0.0,
       UatpPercent = r.Field<object>("ALLOWED_UATP_PER") != null ? r.Field<double>("ALLOWED_UATP_PER") : 0.0,
       UatpAmountBilled = r.Field<object>("UATP_AMT") != null ? r.Field<double>("UATP_AMT") : 0.0,
       VatAmount = r.Field<object>("VAT_AMT") != null ? r.Field<double>("VAT_AMT") : 0.0,
       NetAmountBilled = r.Field<object>("NET_BILLED_AMT") != null ? r.Field<double>("NET_BILLED_AMT") : 0.0,
       NfpReasonCode = r.Field<string>("NFP_REASON_CODE"),
       AgreementIndicatorSupplied = r.Field<string>("AGREEMENT_IND_SUPPLIED"),
       AgreementIndicatorValidated = r.Field<string>("AGREEMENT_IND_VALIDATED"),
       OriginalPmi = r.Field<string>("ORIGINAL_PMI"),
       ValidatedPmi = r.Field<string>("VALIDATED_PMI"),
       SettlementAuthorizationCode = r.Field<string>("SETTLEMENT_AUTH_CODE"),
       AttachmentIndicatorOriginal = r.Field<object>("ATTCHMNT_IND_ORIG") != null ? (r.Field<int>("ATTCHMNT_IND_ORIG") == 0 ? 0 : (r.Field<int>("ATTCHMNT_IND_ORIG") == 1 ? 1 : 2)) : 0,
       AttachmentIndicatorValidated = r.Field<object>("ATTCHMNT_IND_VALIDATED") != null ? (r.Field<int>("ATTCHMNT_IND_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
       NumberOfAttachments = r.Field<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int?>("NO_OF_ATTACHMENTS") : null,
       ISValidationFlag = r.Field<string>("IS_VALIDATION_FLAG"),
       ElectronicTicketIndicator = r.Field<object>("ETICKET_IND") != null ? (r.Field<int>("ETICKET_IND") == 0 ? false : true) : false,
       AirlineFlightDesignator = r.Field<string>("AIRLINE_FLIGHT_DESIGNATOR"),
       FlightNumber = r.Field<object>("FLIGHT_NO") != null ? r.Field<int?>("FLIGHT_NO") : null,
       FlightDate = (DateTime?)(r.Field<object>("FLIGHT_DATE")), //Changes to set FlightDate as null if value is not selected
       CabinClass = r.Field<string>("CABIN_CLASS"),
       ProrateMethodology = r.Field<string>("PRORATE_METHODOLOGY"),
       BillingMemoId = r.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(r.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
       LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
       LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
       ProrateSlipDetails = r.Field<string>("PRORATE_SLIP_DETAIL"),
       CurrencyAdjustmentIndicator = r.Field<string>("CURRENCY_ADJSMNT_IND"),
       ReferenceField1 = r.Field<string>("REFERENCE_FIELD1"),
       ReferenceField2 = r.Field<string>("REFERENCE_FIELD2"),
       ReferenceField3 = r.Field<string>("REFERENCE_FIELD3"),
       ReferenceField4 = r.Field<string>("REFERENCE_FIELD4"),
       ReferenceField5 = r.Field<string>("REFERENCE_FIELD5"),
       AirlineOwnUse = r.Field<string>("AIRLINE_OWN_USE"),
       FromAirportOfCoupon = r.Field<string>("FROM_AIRPORT_OF_COUPON"),
       ReasonCode = r.Field<string>("REASON_CODE"),
     });

    public readonly Materializer<BMCouponTax> BMCouponTaxMaterializer = new Materializer<BMCouponTax>(r =>
new BMCouponTax
{
  LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
  LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
  ParentId = r.Field<byte[]>("BM_COUPON_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_COUPON_BD_ID")) : new Guid(),
  Amount = r.Field<object>("TAX_AMT_BILLED") != null ? r.Field<double>("TAX_AMT_BILLED") : 0,
  TaxCode = r.Field<string>("TAX_CODE"),
  Id = r.Field<byte[]>("BM_COUPON_TAX_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_COUPON_TAX_BD_ID")) : new Guid(),
});

    public readonly Materializer<BMCouponVat> BMCouponVatMaterializer = new Materializer<BMCouponVat>(r =>
new BMCouponVat
{
  LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
  LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
  ParentId = r.Field<byte[]>("BM_COUPON_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_COUPON_BD_ID")) : new Guid(),
  VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
  VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
  VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
  VatText = r.Field<string>("VAT_TEXT"),
  VatLabel = r.Field<string>("VAT_LABEL"),
  VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
  Id = r.Field<byte[]>("BM_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_VAT_BD_ID")) : new Guid(),
}
);

    public readonly Materializer<PrimeCouponTax> CouponTaxMaterializer = new Materializer<PrimeCouponTax>(r =>
      new PrimeCouponTax
      {
        Id = r.Field<byte[]>("COUPON_TAX_BD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_TAX_BD_ID")) : new Guid(),
        TaxCode = r.Field<string>("TAX_CODE"),
        Amount = r.Field<object>("TAX_AMT_BILLED") != null ? r.Field<double>("TAX_AMT_BILLED") : 0,
        ParentId = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      });

    public readonly Materializer<RMCouponTax> RMCouponTaxMaterializer = new Materializer<RMCouponTax>(r =>
    new RMCouponTax
    {
      LastUpdatedOn = r.TryGetField<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = r.TryGetField<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = r.TryGetField<byte[]>("RM_COUPON_BREAKDOWN_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_BREAKDOWN_ID")) : new Guid(),
      AmountDifference = r.TryGetField<object>("TAX_AMT_DIFF") != null ? r.Field<double>("TAX_AMT_DIFF") : 0,
      AmountAccepted = r.TryGetField<object>("TAX_AMT_ACCEPTED") != null ? r.Field<double>("TAX_AMT_ACCEPTED") : 0,
      Amount = r.TryGetField<object>("TAX_AMT_BILLED") != null ? r.Field<double>("TAX_AMT_BILLED") : 0,
      TaxCode = r.TryGetField<string>("TAX_CODE"),
      Id = r.TryGetField<byte[]>("RM_COUPON_TAX_BD_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_TAX_BD_ID")) : new Guid(),
    });

    public readonly Materializer<PrimeCouponVat> CouponVatMaterializer = new Materializer<PrimeCouponVat>(r =>
      new PrimeCouponVat
      {
        Id = r.Field<byte[]>("COUPON_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_VAT_BD_ID")) : new Guid(),
        VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
        VatLabel = r.Field<string>("VAT_LABEL"),
        VatText = r.Field<string>("VAT_TEXT"),
        VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0,
        VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0,
        VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0,
        ParentId = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      });


    public readonly Materializer<SourceCodeTotal> SourceCodeTotalMaterializer = new Materializer<SourceCodeTotal>(r =>
      new SourceCodeTotal
      {
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        TotalAmountAfterSamplingConstant = r.Field<object>("TOT_NET_AMT_AFTER_SMPLNG_CONST") != null ? r.Field<decimal>("TOT_NET_AMT_AFTER_SMPLNG_CONST") : 0,
        TotalVatAmount = r.Field<object>("TOTAL_VAT_AMT") != null ? r.Field<decimal>("TOTAL_VAT_AMT") : 0,
        TotalUatpAmount = r.Field<object>("TOTAL_UATP_AMT") != null ? r.Field<decimal>("TOTAL_UATP_AMT") : 0,
        TotalOtherCommission = r.Field<object>("TOTAL_OTHER_COMM_AMT") != null ? r.Field<decimal>("TOTAL_OTHER_COMM_AMT") : 0,
        SourceCodeId = r.Field<object>("SOURCE_CODE") != null ? r.Field<int>("SOURCE_CODE") : 0,
        TotalHandlingFee = r.Field<object>("TOTAL_HANDLING_FEE_AMT") != null ? r.Field<double>("TOTAL_HANDLING_FEE_AMT") : 0.0,
        NumberOfBillingRecords = r.Field<object>("NO_OF_BILLING_REC") != null ? r.Field<int>("NO_OF_BILLING_REC") : 0,
        TotalNetAmount = r.Field<object>("TOTAL_NET_AMT") != null ? r.Field<decimal>("TOTAL_NET_AMT") : 0,
        TotalTaxAmount = r.Field<object>("TOTAL_TAX_AMT") != null ? r.Field<decimal>("TOTAL_TAX_AMT") : 0,
        TotalIscAmount = r.Field<object>("TOTAL_ISC_AMT") != null ? r.Field<decimal>("TOTAL_ISC_AMT") : 0,
        TotalGrossValue = r.Field<object>("TOTAL_GROSS_VALUE") != null ? r.Field<decimal>("TOTAL_GROSS_VALUE") : 0,
        InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
        Id = r.Field<byte[]>("SOURCE_CODE_TOTAL_ID") != null ? new Guid(r.Field<byte[]>("SOURCE_CODE_TOTAL_ID")) : new Guid(),
      });

    public readonly Materializer<SamplingFormCSourceCodeTotal> SamplingFormCSourceCodeTotalMaterializer = new Materializer<SamplingFormCSourceCodeTotal>(r =>
     new SamplingFormCSourceCodeTotal
     {
       LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
       LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
       SamplingFormCId = r.Field<byte[]>("FORM_C_ID") != null ? new Guid(r.Field<byte[]>("FORM_C_ID")) : new Guid(),
       TotalNetAmountAfterSamplingConstant = r.Field<object>("TOT_NET_AMT_AFTER_SMPLNG_CONST") != null ? r.Field<decimal>("TOT_NET_AMT_AFTER_SMPLNG_CONST") : 0,
       TotalVatAmount = r.Field<object>("TOTAL_VAT_AMT") != null ? r.Field<decimal>("TOTAL_VAT_AMT") : 0,
       TotalUatpAmount = r.Field<object>("TOTAL_UATP_AMT") != null ? r.Field<decimal>("TOTAL_UATP_AMT") : 0,
       TotalOtherCommisionAmount = r.Field<object>("TOTAL_OTHER_COMM_AMT") != null ? r.Field<decimal>("TOTAL_OTHER_COMM_AMT") : 0,
       SourceId = r.Field<object>("SOURCE_CODE") != null ? r.Field<int>("SOURCE_CODE") : 0,
       TotalHandlingFeeAmount = r.Field<object>("TOTAL_HANDLING_FEE_AMT") != null ? r.Field<double>("TOTAL_HANDLING_FEE_AMT") : 0.0,
       NoOfBillingRecord = r.Field<object>("NO_OF_BILLING_REC") != null ? r.Field<int>("NO_OF_BILLING_REC") : 0,
       TotalNetAmount = r.Field<object>("TOTAL_NET_AMT") != null ? r.Field<decimal>("TOTAL_NET_AMT") : 0,
       TotalTaxAmount = r.Field<object>("TOTAL_TAX_AMT") != null ? r.Field<decimal>("TOTAL_TAX_AMT") : 0,
       TotalIscAmount = r.Field<object>("TOTAL_ISC_AMT") != null ? r.Field<decimal>("TOTAL_ISC_AMT") : 0,
       TotalGrossValue = r.Field<object>("TOTAL_GROSS_VALUE") != null ? r.Field<decimal>("TOTAL_GROSS_VALUE") : 0,
       Id = r.Field<byte[]>("FORM_C_SOURCE_CODE_TOTAL_ID") != null ? new Guid(r.Field<byte[]>("FORM_C_SOURCE_CODE_TOTAL_ID")) : new Guid(),
     });

    public readonly Materializer<SourceCode> SourceCodeMaterializer = new Materializer<SourceCode>(r =>
      new SourceCode
      {
        Id = r.Field<object>("SOURCE_CODE_ID") != null ? r.Field<int>("SOURCE_CODE_ID") : 0,
        SourceCodeDescription = r.Field<string>("DESCRIPTION"),
        IncludeInAtpcoReport = r.Field<object>("INCLUDE_IN_ATPCO_REPORT") != null && r.Field<int>("INCLUDE_IN_ATPCO_REPORT") > 0,
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        UtilizationType = r.Field<object>("UTILIZATION_TYPE") != null ? r.Field<string>("UTILIZATION_TYPE") : string.Empty,
        TransactionTypeId = r.Field<object>("TRANSACTION_TYPE_ID") != null ? r.Field<int>("TRANSACTION_TYPE_ID") : 0,
        SourceCodeIdentifier = r.Field<object>("SOURCE_CODE") != null ? r.Field<int>("SOURCE_CODE") : 0,
        IsFFIndicator = r.Field<object>("FF_INDICATOR") != null && r.Field<int>("FF_INDICATOR") > 0,
        IsRejectionLevel = r.Field<object>("REJECTION_LEVEL") != null && r.Field<int>("REJECTION_LEVEL") > 0,
        IsBilateralCode = r.Field<object>("BILATERAL_CODE") != null && r.Field<int>("BILATERAL_CODE") > 0,
        IsActive = r.Field<int>("IS_ACTIVE") > 0
      }
       );

    public readonly Materializer<InvoiceTotal> InvoiceTotalMaterializer = new Materializer<InvoiceTotal>(r =>
      new InvoiceTotal
      {
        SamplingConstant = r.TryGetField<object>("SAMPLING_CONSTANT") != null ? r.Field<double>("SAMPLING_CONSTANT") : 0.0,
        LastUpdatedOn = r.TryGetField<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.TryGetField<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        Id = r.TryGetField<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
        TotalNetAmountWithoutVat = r.TryGetField<object>("TOTAL_NET_AMT_WITHOUT_VAT") != null ? r.Field<decimal>("TOTAL_NET_AMT_WITHOUT_VAT") : 0,
        TotalVatAmountAfterSamplingConstant = r.TryGetField<object>("SAMPLING_CONSTANT_TOT_VAT_AMT") != null ? r.Field<decimal>("SAMPLING_CONSTANT_TOT_VAT_AMT") : 0,
        TotalProvisionalAdjustmentAmount = r.TryGetField<object>("TOTAL_PROV_ADJUSTMENT_AMT") != null ? r.Field<decimal>("TOTAL_PROV_ADJUSTMENT_AMT") : 0,
        VatAbsorptionAmount = r.TryGetField<object>("VAT_ABSORPTION_AMT") != null ? r.Field<decimal>("VAT_ABSORPTION_AMT") : 0,
        VatAbsorptionPercent = r.TryGetField<object>("VAT_ABSORPTION_PER") != null ? r.Field<double>("VAT_ABSORPTION_PER") : 0.0,
        OtherCommissionAbsorptionAmount = r.TryGetField<object>("OTHER_COMM_ABSORPTION_AMT") != null ? r.Field<decimal>("OTHER_COMM_ABSORPTION_AMT") : 0,
        OtherCommissionAbsorptionPercent = r.TryGetField<object>("OTHER_COMM_ABSORPTION_PER") != null ? r.Field<double>("OTHER_COMM_ABSORPTION_PER") : 0.0,
        HandlingFeeAbsorptionAmount = r.TryGetField<object>("HANDLING_FEE_ABSORPTION_AMT") != null ? r.Field<double>("HANDLING_FEE_ABSORPTION_AMT") : 0.0,
        HandlingFeeAbsorptionPercent = r.TryGetField<object>("HANDLING_FEE_ABSORPTION_PER") != null ? r.Field<double>("HANDLING_FEE_ABSORPTION_PER") : 0.0,
        UatpAbsorptionAmount = r.TryGetField<object>("UATP_ABSORPTION_AMT") != null ? r.Field<decimal>("UATP_ABSORPTION_AMT") : 0,
        UatpAbsorptionPercent = r.TryGetField<object>("UATP_ABSORPTION_PER") != null ? r.Field<double>("UATP_ABSORPTION_PER") : 0.0,
        TaxAbsorptionAmount = r.TryGetField<object>("TAX_ABSORPTION_AMT") != null ? r.Field<decimal>("TAX_ABSORPTION_AMT") : 0,
        TaxAbsorptionPercent = r.TryGetField<object>("TAX_ABSORPTION_PER") != null ? r.Field<double>("TAX_ABSORPTION_PER") : 0.0,
        IscAbsorptionAmount = r.TryGetField<object>("ISC_ABSORPTION_AMT") != null ? r.Field<decimal>("ISC_ABSORPTION_AMT") : 0,
        IscAbsorptionPercent = r.TryGetField<object>("ISC_ABSORPTION_PER") != null ? r.Field<double>("ISC_ABSORPTION_PER") : 0.0,
        FareAbsorptionAmount = r.TryGetField<object>("FARE_ABSORPTION_AMT") != null ? r.Field<decimal>("FARE_ABSORPTION_AMT") : 0,
        FareAbsorptionPercent = r.TryGetField<object>("FARE_ABSORPTION_PER") != null ? r.Field<double>("FARE_ABSORPTION_PER") : 0,
        NetAmountAfterSamplingConstant = r.TryGetField<object>("SAMPLING_CONST_NET_AMT") != null ? r.Field<decimal>("SAMPLING_CONST_NET_AMT") : 0,
        TotalNoOfRecords = r.TryGetField<object>("TOTAL_NO_OF_RECORDS") != null ? r.Field<int>("TOTAL_NO_OF_RECORDS") : 0,
        TotalVatAmount = r.TryGetField<object>("TOTAL_VAT_AMT") != null ? r.Field<decimal>("TOTAL_VAT_AMT") : 0,
        TotalUatpAmount = r.TryGetField<object>("TOTAL_UATP_AMT") != null ? r.Field<decimal>("TOTAL_UATP_AMT") : 0,
        TotalOtherCommission = r.TryGetField<object>("TOTAL_OTH_COMM_AMT") != null ? r.Field<decimal>("TOTAL_OTH_COMM_AMT") : 0,
        TotalHandlingFee = r.TryGetField<object>("HANDLING_FEE_AMT") != null ? r.Field<double>("HANDLING_FEE_AMT") : 0.0,
        NoOfBillingRecords = r.TryGetField<object>("NO_OF_BILLING_RECORDS") != null ? r.Field<int>("NO_OF_BILLING_RECORDS") : 0,
        NetBillingAmount = r.TryGetField<object>("NET_BILLING_AMT") != null ? r.Field<decimal>("NET_BILLING_AMT") : 0,
        NetTotal = r.TryGetField<object>("NET_TOTAL") != null ? r.Field<decimal>("NET_TOTAL") : 0,
        TotalTaxAmount = r.TryGetField<object>("TOTAL_TAX_AMT") != null ? r.Field<decimal>("TOTAL_TAX_AMT") : 0,
        TotalIscAmount = r.TryGetField<object>("TOTAL_INTERLINE_SERV_CHARGE") != null ? r.Field<decimal>("TOTAL_INTERLINE_SERV_CHARGE") : 0,
        TotalGrossValue = r.TryGetField<object>("TOTAL_GROSS_VALUE") != null ? r.Field<decimal>("TOTAL_GROSS_VALUE") : 0,
        ProvAdjustmentRate = r.TryGetField<object>("PROV_ADJSTMNT_RATE") != null ? r.Field<double>("PROV_ADJSTMNT_RATE") : 0.0,
        RecordSequenceWithinBatch = r.TryGetField<object>("BATCH_REC_SEQ_NO") != null ? r.Field<int>("BATCH_REC_SEQ_NO") : 0,
        BatchSequenceNumber = r.TryGetField<object>("BATCH_SEQ_NO") != null ? r.Field<int>("BATCH_SEQ_NO") : 0,
      }
      );

    public readonly Materializer<SourceCodeVat> SourceCodeVatMaterializer = new Materializer<SourceCodeVat>(r =>
      new SourceCodeVat
      {
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        ParentId = r.Field<byte[]>("SOURCE_CODE_TOTAL_ID") != null ? new Guid(r.Field<byte[]>("SOURCE_CODE_TOTAL_ID")) : new Guid(),
        VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
        VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
        VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
        VatText = r.Field<string>("VAT_TEXT"),
        VatLabel = r.Field<string>("VAT_LABEL"),
        VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
        Id = r.Field<byte[]>("SOURCE_CODE_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("SOURCE_CODE_VAT_BD_ID")) : new Guid(),
      }
      );

    public readonly Materializer<InvoiceVat> InvoiceVatMaterializer = new Materializer<InvoiceVat>(r =>
      new InvoiceVat
      {
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        ParentId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
        VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
        VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
        VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
        VatText = r.Field<string>("VAT_TEXT"),
        VatLabel = r.Field<string>("VAT_LABEL"),
        VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
        Id = r.Field<byte[]>("INVOICE_TOTAL_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_TOTAL_VAT_BD_ID")) : new Guid(),
      }
      );

    public readonly Materializer<VatIdentifier> VatIdentifierMaterializer = new Materializer<VatIdentifier>(r =>
      new VatIdentifier
      {
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        Identifier = r.Field<string>("VAT_IDENTIFIER"),
        Id = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
        BillingCategoryCode = r.Field<object>("BILLING_CATEGORY_ID") != null ? r.Field<int>("BILLING_CATEGORY_ID") : 0,
        IdentifierCode = r.Field<string>("DESCRIPTION"),
        Description = r.Field<string>("DESCRIPTION"),
        IsActive = r.Field<int>("IS_ACTIVE") > 0
      }
      );

    public readonly Materializer<RejectionMemoVat> RejectionMemoVatMaterializer = new Materializer<RejectionMemoVat>(r =>
     new RejectionMemoVat
     {
       LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
       LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
       ParentId = r.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(r.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
       VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
       VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
       VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
       VatText = r.Field<string>("VAT_TEXT"),
       VatLabel = r.Field<string>("VAT_LABEL"),
       VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
       Id = r.Field<byte[]>("RM_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("RM_VAT_BD_ID")) : new Guid(),
     }
     );

    public readonly Materializer<RMCouponVat> RMCouponVatMaterializer = new Materializer<RMCouponVat>(r =>
    new RMCouponVat
    {
      LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = r.Field<byte[]>("RM_COUPON_BREAKDOWN_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_BREAKDOWN_ID")) : new Guid(),
      VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
      VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
      VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
      VatText = r.Field<string>("VAT_TEXT"),
      VatLabel = r.Field<string>("VAT_LABEL"),
      VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
      Id = r.Field<byte[]>("RM_COUPON_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_VAT_BD_ID")) : new Guid(),
    }
    );

    public readonly Materializer<RejectionMemo> RejectionMemoMaterializer = new Materializer<RejectionMemo>(r =>
       new RejectionMemo
       {
         //TODO : Check TicketDocOrFimNumber object 
         Id = r.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(r.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
         BatchSequenceNumber = r.Field<object>("BATCH_SEQ_NO") != null ? r.Field<int>("BATCH_SEQ_NO") : 0,
         RecordSequenceWithinBatch = r.Field<object>("BATCH_REC_SEQ_NO") != null ? r.Field<int>("BATCH_REC_SEQ_NO") : 0,
         RejectionMemoNumber = r.Field<string>("REJECTION_MEMO_NO"),
         RejectionStage = r.Field<object>("REJECTION_STAGE") != null ? r.Field<int>("REJECTION_STAGE") : 0,
         SourceCodeId = r.Field<object>("SOURCE_CODE") != null ? r.Field<int>("SOURCE_CODE") : 0,
         ReasonCode = r.Field<string>("REASON_CODE"),
         OurRef = r.Field<object>("INTERNAL_REF") != null ? r.Field<string>("INTERNAL_REF") : null,
         YourInvoiceNumber = r.Field<string>("YOUR_INVOICE_NO"),
         YourInvoiceBillingYear = r.Field<object>("YOUR_INVOICE_BILL_YEAR") != null ? r.Field<int>("YOUR_INVOICE_BILL_YEAR") : 0,
         YourRejectionNumber = r.Field<string>("YOUR_REJ_NO"),
         FimBMCMNumber = r.Field<string>("FIM_BM_CM_NO"),
         FimCouponNumber = r.Field<object>("FIM_COUPON_NO") != null ? r.Field<int?>("FIM_COUPON_NO") : null,
         TotalGrossAmountBilled = r.Field<object>("TOTAL_GROSS_BILLED_AMT") != null ? r.Field<double>("TOTAL_GROSS_BILLED_AMT") : 0.0,
         TotalGrossAcceptedAmount = r.Field<object>("TOTAL_GROSS_ACCEPTED_AMT") != null ? r.Field<double>("TOTAL_GROSS_ACCEPTED_AMT") : 0.0,
         TotalGrossDifference = r.Field<object>("TOTAL_GROSS_DIFF") != null ? r.Field<double>("TOTAL_GROSS_DIFF") : 0.0,
         TotalTaxAmountBilled = r.Field<object>("TOTAL_TAX_BILLED_AMT") != null ? r.Field<double>("TOTAL_TAX_BILLED_AMT") : 0.0,
         TotalTaxAmountAccepted = r.Field<object>("TOTAL_TAX_ACCPTED_AMT") != null ? r.Field<double>("TOTAL_TAX_ACCPTED_AMT") : 0.0,
         TotalTaxAmountDifference = r.Field<object>("TOTAL_TAX_DIFF") != null ? r.Field<double>("TOTAL_TAX_DIFF") : 0.0,
         AllowedIscAmount = r.Field<object>("TOTAL_ISC_ALLOWED_AMT") != null ? r.Field<double>("TOTAL_ISC_ALLOWED_AMT") : 0.0,
         AcceptedIscAmount = r.Field<object>("TOTAL_ISC_ACCPTED_AMT") != null ? r.Field<double>("TOTAL_ISC_ACCPTED_AMT") : 0.0,
         IscDifference = r.Field<object>("TOTAL_ISC_DIFF") != null ? r.Field<double>("TOTAL_ISC_DIFF") : 0.0,
         AllowedOtherCommission = r.Field<object>("TOTAL_OTH_COMM_ALLOWED") != null ? r.Field<double>("TOTAL_OTH_COMM_ALLOWED") : 0.0,
         AcceptedOtherCommission = r.Field<object>("TOTAL_OTH_COMM_ACCPTED") != null ? r.Field<double>("TOTAL_OTH_COMM_ACCPTED") : 0.0,
         OtherCommissionDifference = r.Field<object>("TOTAL_OTH_COMM_DIFF") != null ? r.Field<double>("TOTAL_OTH_COMM_DIFF") : 0.0,
         AllowedHandlingFee = r.Field<object>("TOTAL_HANDLING_FEE_ALLOWED") != null ? r.Field<double>("TOTAL_HANDLING_FEE_ALLOWED") : 0.0,
         AcceptedHandlingFee = r.Field<object>("TOTAL_HANDLING_FEE_ACCPTED") != null ? r.Field<double>("TOTAL_HANDLING_FEE_ACCPTED") : 0.0,
         HandlingFeeAmountDifference = r.Field<object>("TOTAL_HANDLING_FEE_DIFF") != null ? r.Field<double>("TOTAL_HANDLING_FEE_DIFF") : 0.0,
         AllowedUatpAmount = r.Field<object>("TOTAL_UATP_ALLOWED_AMT") != null ? r.Field<double>("TOTAL_UATP_ALLOWED_AMT") : 0.0,
         AcceptedUatpAmount = r.Field<object>("TOTAL_UATP_ACCPTED_AMT") != null ? r.Field<double>("TOTAL_UATP_ACCPTED_AMT") : 0.0,
         UatpAmountDifference = r.Field<object>("TOTAL_UATP_DIFF") != null ? r.Field<double>("TOTAL_UATP_DIFF") : 0.0,
         TotalVatAmountBilled = r.Field<object>("TOTAL_BILLED_VAT_AMT") != null ? r.Field<double>("TOTAL_BILLED_VAT_AMT") : 0.0,
         TotalVatAmountAccepted = r.Field<object>("TOTAL_ACCPT_VAT_AMT") != null ? r.Field<double>("TOTAL_ACCPT_VAT_AMT") : 0.0,
         TotalVatAmountDifference = r.Field<object>("TOTAL_VAT_DIFF") != null ? r.Field<double>("TOTAL_VAT_DIFF") : 0.0,
         TotalNetRejectAmount = r.Field<object>("TOTAL_NET_REJ_AMT") != null ? r.Field<decimal>("TOTAL_NET_REJ_AMT") : 0.0m,
         SamplingConstant = r.Field<object>("SAMPLING_CONSTANT") != null ? r.Field<double>("SAMPLING_CONSTANT") : 0.0,
         TotalNetRejectAmountAfterSamplingConstant = r.Field<object>("TOTAL_NRA_AFTER_SAMPLING") != null ? r.Field<decimal>("TOTAL_NRA_AFTER_SAMPLING") : 0.0m,
         AttachmentIndicatorOriginal = r.Field<object>("ATTCHMNT_IND_ORIG") != null ? (r.Field<int>("ATTCHMNT_IND_ORIG") == 0 ? 0 : (r.Field<int>("ATTCHMNT_IND_ORIG") == 1 ? 1 : 2)) : 0,
         AttachmentIndicatorValidated = r.Field<object>("ATTACHMENT_IND_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_IND_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
         NumberOfAttachments = r.Field<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int?>("NO_OF_ATTACHMENTS") : null,
         AirlineOwnUse = r.Field<string>("AIRLINE_OWN_USE"),
         ISValidationFlag = r.Field<string>("IS_VALID_FLAG"),
         IsRejectionFlag = r.Field<string>("IS_REJECT_FLAG"),
         IsLinkingSuccessful = r.Field<object>("IS_LINKING_SUCCESSFUL") != null ? (bool?)(r.Field<int>("IS_LINKING_SUCCESSFUL") > 0) : null,
         InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
         CorrespondenceId = r.Field<byte[]>("CORRESPONDENCE_ID") != null ? (Guid?)new Guid(r.Field<byte[]>("CORRESPONDENCE_ID")) : null,
         ReasonRemarks = r.Field<string>("REASON_REMARKS"),
         YourInvoiceBillingMonth = r.Field<object>("YOUR_INVOICE_BILL_MONTH") != null ? r.Field<int>("YOUR_INVOICE_BILL_MONTH") : 0,
         YourInvoiceBillingPeriod = r.Field<object>("YOUR_INVOICE_BILL_PERIOD") != null ? r.Field<int>("YOUR_INVOICE_BILL_PERIOD") : 0,

         LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
         LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),

         IsBreakdownAllowed = r.Field<object>("IS_BREAKDOWN_ALLOWED") != null ? (r.Field<int>("IS_BREAKDOWN_ALLOWED") > 0 ? (bool?)true : (bool?)false) : null,
         FIMBMCMIndicatorId = r.Field<object>("FIM_BM_CM_INDICATOR") != null ? r.Field<int>("FIM_BM_CM_INDICATOR") : 0,
         CurrencyConversionFactor = r.Field<object>("CURR_CONVERSION_FACTOR") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("CURR_CONVERSION_FACTOR")) : null
       });

    public readonly Materializer<RMCoupon> RMCouponMaterializer = new Materializer<RMCoupon>(r =>
       new RMCoupon
       {
         //TODO : Check TicketDocOrFimNumber object 
         Id = r.TryGetField<byte[]>("RM_COUPON_BREAKDOWN_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_BREAKDOWN_ID")) : new Guid(),
         TicketOrFimIssuingAirline = r.TryGetField<object>("TICKET_ISSUING_AIRLINE") != null ? r.Field<string>("TICKET_ISSUING_AIRLINE") : string.Empty,
         TicketOrFimCouponNumber = r.TryGetField<object>("COUPON_NO") != null ? r.Field<int>("COUPON_NO") : 0,
         TicketDocOrFimNumber = r.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOC_NO")) : 0,
         CheckDigit = r.TryGetField<object>("CHECK_DIGIT") != null ? r.Field<int>("CHECK_DIGIT") : 0, //test
         FromAirportOfCoupon = r.TryGetField<string>("FROM_AIRPORT_OF_COUPON"),
         ToAirportOfCoupon = r.TryGetField<string>("TO_AIRPORT_OF_COUPON"),
         GrossAmountBilled = r.TryGetField<object>("BILLED_GROSS_AMT") != null ? r.Field<double>("BILLED_GROSS_AMT") : 0.0,
         GrossAmountAccepted = r.TryGetField<object>("ACCEPTED_GROSS_AMT") != null ? r.Field<double>("ACCEPTED_GROSS_AMT") : 0.0,
         GrossAmountDifference = r.TryGetField<object>("GROSS_AMT_DIFF") != null ? r.Field<double>("GROSS_AMT_DIFF") : 0.0,

         TaxAmountBilled = r.TryGetField<object>("BILLED_TAX_AMT") != null ? r.Field<double>("BILLED_TAX_AMT") : 0.0,
         TaxAmountAccepted = r.TryGetField<object>("ACCPT_TAX_AMT") != null ? r.Field<double>("ACCPT_TAX_AMT") : 0.0,
         TaxAmountDifference = r.TryGetField<object>("TAX_AMT_DIFF") != null ? r.Field<double>("TAX_AMT_DIFF") : 0.0,
         AllowedIscPercentage = r.TryGetField<object>("ALLOWED_ISC_PER") != null ? r.Field<double>("ALLOWED_ISC_PER") : 0.0,
         AllowedIscAmount = r.TryGetField<object>("ALLOWED_ISC_AMT") != null ? r.Field<double>("ALLOWED_ISC_AMT") : 0.0,
         AcceptedIscPercentage = r.TryGetField<object>("ACCPT_ISC_PER") != null ? r.Field<double>("ACCPT_ISC_PER") : 0.0,
         AcceptedIscAmount = r.TryGetField<object>("ACCPT_ISC_AMT") != null ? r.Field<double>("ACCPT_ISC_AMT") : 0.0,
         IscDifference = r.TryGetField<object>("ISC_AMT_DIFF") != null ? r.Field<double>("ISC_AMT_DIFF") : 0.0,

         AllowedOtherCommissionPercentage = r.TryGetField<object>("ALLOWED_OTH_COMM_PER") != null ? r.Field<double>("ALLOWED_OTH_COMM_PER") : 0.0,
         AllowedOtherCommission = r.TryGetField<object>("ALLOWED_OTH_COMM_AMT") != null ? r.Field<double>("ALLOWED_OTH_COMM_AMT") : 0.0,
         AcceptedOtherCommissionPercentage = r.TryGetField<object>("ACCPT_OTH_COMM_PER") != null ? r.Field<double>("ACCPT_OTH_COMM_PER") : 0.0,
         AcceptedOtherCommission = r.TryGetField<object>("ACCPT_OTH_COMM_AMT") != null ? r.Field<double>("ACCPT_OTH_COMM_AMT") : 0.0,
         OtherCommissionDifference = r.TryGetField<object>("OTH_COMM_AMT_DIFF") != null ? r.Field<double>("OTH_COMM_AMT_DIFF") : 0.0,

         AllowedHandlingFee = r.TryGetField<object>("ALLOWED_HANDLING_FEE_AMT") != null ? r.Field<double>("ALLOWED_HANDLING_FEE_AMT") : 0.0,
         AcceptedHandlingFee = r.TryGetField<object>("ACCPT_HANDLING_FEE_AMT") != null ? r.Field<double>("ACCPT_HANDLING_FEE_AMT") : 0.0,
         HandlingDifference = r.TryGetField<object>("ACCPT_HANDLING_AMT_DIFF") != null ? r.Field<double>("ACCPT_HANDLING_AMT_DIFF") : 0.0,

         AllowedUatpPercentage = r.TryGetField<object>("ALLOWED_UATP_PER") != null ? r.Field<double>("ALLOWED_UATP_PER") : 0.0,
         AllowedUatpAmount = r.TryGetField<object>("ALLOWED_UATP_AMT") != null ? r.Field<double>("ALLOWED_UATP_AMT") : 0.0,
         AcceptedUatpPercentage = r.TryGetField<object>("ACCPT_UATP_PER") != null ? r.Field<double>("ACCPT_UATP_PER") : 0.0,
         AcceptedUatpAmount = r.TryGetField<object>("ACCPT_UATP_AMT") != null ? r.Field<double>("ACCPT_UATP_AMT") : 0.0,
         UatpDifference = r.TryGetField<object>("UATP_AMT_DIFF") != null ? r.Field<double>("UATP_AMT_DIFF") : 0.0,

         VatAmountBilled = r.TryGetField<object>("BILLED_VAT_AMT") != null ? r.Field<double>("BILLED_VAT_AMT") : 0.0,
         VatAmountAccepted = r.TryGetField<object>("ACCPT_VAT_AMT") != null ? r.Field<double>("ACCPT_VAT_AMT") : 0.0,
         VatAmountDifference = r.TryGetField<object>("VAT_AMT_DIFF") != null ? r.Field<double>("VAT_AMT_DIFF") : 0.0,

         NetRejectAmount = r.TryGetField<object>("NET_REJECT_AMT") != null ? r.Field<double>("NET_REJECT_AMT") : 0.0,

         NfpReasonCode = r.TryGetField<string>("NFP_REASON_CODE"),
         AgreementIndicatorSupplied = r.TryGetField<string>("AGREEMENT_IND_SUPPLIED"),
         AgreementIndicatorValidated = r.TryGetField<string>("AGREEMENT_IND_VALIDATED"),
         OriginalPmi = r.TryGetField<string>("ORIGINAL_PMI"),
         ValidatedPmi = r.TryGetField<string>("VALIDATED_PMI"),

         SettlementAuthorizationCode = r.TryGetField<string>("SETTLEMENT_AUTH_CODE"),
         AttachmentIndicatorOriginal = r.Field<object>("ATTACHMENT_IND_ORIG") != null ? (r.Field<int>("ATTACHMENT_IND_ORIG") == 0 ? 0 : (r.Field<int>("ATTACHMENT_IND_ORIG") == 1 ? 1 : 2)) : 0,
         AttachmentIndicatorValidated = r.Field<object>("ATTACHMENT_IND_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_IND_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
         NumberOfAttachments = r.Field<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int?>("NO_OF_ATTACHMENTS") : null,

         ISValidationFlag = r.TryGetField<string>("IS_VALIDATION_FLAG"),
         ReasonCode = r.TryGetField<string>("REASON_CODE"),
         ReferenceField1 = r.TryGetField<string>("REFERENCE_FIELD1"),
         ReferenceField2 = r.TryGetField<string>("REFERENCE_FIELD2"),
         ReferenceField3 = r.TryGetField<string>("REFERENCE_FIELD3"),
         ReferenceField4 = r.TryGetField<string>("REFERENCE_FIELD4"),
         ReferenceField5 = r.TryGetField<string>("REFERENCE_FIELD5"),
         AirlineOwnUse = r.TryGetField<string>("AIRLINE_OWN_USE"),

         RejectionMemoId = r.TryGetField<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(r.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
         LastUpdatedBy = r.TryGetField<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
         LastUpdatedOn = r.TryGetField<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),

         ProrateSlipDetails = r.TryGetField<string>("PRORATE_SLIP_DETAIL"),
         SerialNo = r.TryGetField<object>("RM_BD_SR_NO") != null ? r.Field<int>("RM_BD_SR_NO") : 0,
       });

    public readonly Materializer<PrimeCouponAttachment> PrimeCouponAttachmentMaterializer = new Materializer<PrimeCouponAttachment>(pca =>
    new PrimeCouponAttachment
    {
      ServerId = pca.Field<object>("SERVER_ID") != null ? pca.Field<int>("SERVER_ID") : 0,
      FilePath = pca.Field<string>("FILE_PATH"),
      OriginalFileName = pca.Field<string>("ORG_FILE_NAME"),
      LastUpdatedOn = pca.Field<object>("LAST_UPDATED_ON") != null ? pca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = pca.Field<object>("LAST_UPDATED_BY") != null ? pca.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = pca.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(pca.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
      FileStatusId = pca.Field<object>("FILE_STATUS_ID") != null ? pca.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = pca.Field<object>("FILE_TYPE_ID") != null ? pca.Field<int>("FILE_TYPE_ID") : 0,
      FileSize = pca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(pca.Field<object>("FILE_SIZE")) : 0,
      IsFullPath = pca.Field<object>("IS_FULL_PATH") != null ? (pca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      Id = pca.Field<byte[]>("COUPON_RECORD_ATTACHMENT_ID") != null ? new Guid(pca.Field<byte[]>("COUPON_RECORD_ATTACHMENT_ID")) : new Guid(),
    });

    public readonly Materializer<BillingMemoAttachment> BillingMemoAttachmentMaterializer = new Materializer<BillingMemoAttachment>(bma =>
    new BillingMemoAttachment
    {
      ServerId = bma.Field<object>("SERVER_ID") != null ? bma.Field<int>("SERVER_ID") : 0,
      FilePath = bma.Field<string>("FILE_PATH"),
      OriginalFileName = bma.Field<string>("ORG_FILE_NAME"),
      LastUpdatedOn = bma.Field<object>("LAST_UPDATED_ON") != null ? bma.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = bma.Field<object>("LAST_UPDATED_BY") != null ? bma.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = bma.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(bma.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
      FileStatusId = bma.Field<object>("FILE_STATUS_ID") != null ? bma.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = bma.Field<object>("FILE_TYPE_ID") != null ? bma.Field<int>("FILE_TYPE_ID") : 0,
      FileSize = bma.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(bma.Field<object>("FILE_SIZE")) : 0,
      IsFullPath = bma.Field<object>("IS_FULL_PATH") != null ? (bma.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      Id = bma.Field<byte[]>("BILLING_MEMO_ATTACHMENT_ID") != null ? new Guid(bma.Field<byte[]>("BILLING_MEMO_ATTACHMENT_ID")) : new Guid(),
    });

    public readonly Materializer<RejectionMemoAttachment> RejectionMemoAttachmentMaterializer = new Materializer<RejectionMemoAttachment>(rma =>
    new RejectionMemoAttachment
    {
      ServerId = rma.Field<object>("SERVER_ID") != null ? rma.Field<int>("SERVER_ID") : 0,
      FilePath = rma.Field<string>("FILE_PATH"),
      OriginalFileName = rma.Field<string>("ORG_FILE_NAME"),
      LastUpdatedOn = rma.Field<object>("LAST_UPDATED_ON") != null ? rma.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = rma.Field<object>("LAST_UPDATED_BY") != null ? rma.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = rma.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(rma.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
      FileStatusId = rma.Field<object>("FILE_STATUS_ID") != null ? rma.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = rma.Field<object>("FILE_TYPE_ID") != null ? rma.Field<int>("FILE_TYPE_ID") : 0,
      FileSize = rma.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(rma.Field<object>("FILE_SIZE")) : 0,
      IsFullPath = rma.Field<object>("IS_FULL_PATH") != null ? (rma.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      Id = rma.Field<byte[]>("PAX_RM_ATTACHMENTS_ID") != null ? new Guid(rma.Field<byte[]>("PAX_RM_ATTACHMENTS_ID")) : new Guid(),
    });

    public readonly Materializer<RMCouponAttachment> RMCouponAttachmentMaterializer = new Materializer<RMCouponAttachment>(rmca =>
    new RMCouponAttachment
    {
      ServerId = rmca.Field<object>("SERVER_ID") != null ? rmca.Field<int>("SERVER_ID") : 0,
      FilePath = rmca.Field<string>("FILE_PATH"),
      OriginalFileName = rmca.Field<string>("ORG_FILE_NAME"),
      LastUpdatedOn = rmca.Field<object>("LAST_UPDATED_ON") != null ? rmca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = rmca.Field<object>("LAST_UPDATED_BY") != null ? rmca.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = rmca.Field<byte[]>("RM_COUPON_BREAKDOWN_ID") != null ? new Guid(rmca.Field<byte[]>("RM_COUPON_BREAKDOWN_ID")) : new Guid(),
      FileStatusId = rmca.Field<object>("FILE_STATUS_ID") != null ? rmca.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = rmca.Field<object>("FILE_TYPE_ID") != null ? rmca.Field<int>("FILE_TYPE_ID") : 0,
      FileSize = rmca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(rmca.Field<object>("FILE_SIZE")) : 0,
      IsFullPath = rmca.Field<object>("IS_FULL_PATH") != null ? (rmca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      Id = rmca.Field<byte[]>("RM_COUPON_ATTACHMENTS_ID") != null ? new Guid(rmca.Field<byte[]>("RM_COUPON_ATTACHMENTS_ID")) : new Guid(),
    });

    public readonly Materializer<SamplingFormDRecordAttachment> SamplingFormDRecordAttachmentMaterializer = new Materializer<SamplingFormDRecordAttachment>(rmca =>
    new SamplingFormDRecordAttachment
    {
      ServerId = rmca.Field<object>("SERVER_ID") != null ? rmca.Field<int>("SERVER_ID") : 0,
      FilePath = rmca.Field<string>("FILE_PATH"),
      OriginalFileName = rmca.Field<string>("ORG_FILE_NAME"),
      LastUpdatedOn = rmca.Field<object>("LAST_UPDATED_ON") != null ? rmca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = rmca.Field<object>("LAST_UPDATED_BY") != null ? rmca.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = rmca.Field<byte[]>("FORM_D_ID") != null ? new Guid(rmca.Field<byte[]>("FORM_D_ID")) : new Guid(),
      FileStatusId = rmca.Field<object>("FILE_STATUS_ID") != null ? rmca.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = rmca.Field<object>("FILE_TYPE_ID") != null ? rmca.Field<int>("FILE_TYPE_ID") : 0,
      FileSize = rmca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(rmca.Field<object>("FILE_SIZE")) : 0,
      IsFullPath = rmca.Field<object>("IS_FULL_PATH") != null ? (rmca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      Id = rmca.Field<byte[]>("FORM_D_ATTACHMENT_ID") != null ? new Guid(rmca.Field<byte[]>("FORM_D_ATTACHMENT_ID")) : new Guid(),
    });

    public readonly Materializer<SamplingFormCRecordAttachment> SamplingFormCRecordAttachmentMaterializer = new Materializer<SamplingFormCRecordAttachment>(rmca =>
   new SamplingFormCRecordAttachment
   {
     ServerId = rmca.Field<object>("SERVER_ID") != null ? rmca.Field<int>("SERVER_ID") : 0,
     FilePath = rmca.Field<string>("FILE_PATH"),
     OriginalFileName = rmca.Field<string>("ORG_FILE_NAME"),
     LastUpdatedOn = rmca.Field<object>("LAST_UPDATED_ON") != null ? rmca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
     LastUpdatedBy = rmca.Field<object>("LAST_UPDATED_BY") != null ? rmca.Field<int>("LAST_UPDATED_BY") : 0,
     ParentId = rmca.Field<byte[]>("FORM_C_DETAIL_ID") != null ? new Guid(rmca.Field<byte[]>("FORM_C_DETAIL_ID")) : new Guid(),
     FileStatusId = rmca.Field<object>("FILE_STATUS_ID") != null ? rmca.Field<int>("FILE_STATUS_ID") : 0,
     FileTypeId = rmca.Field<object>("FILE_TYPE_ID") != null ? rmca.Field<int>("FILE_TYPE_ID") : 0,
     FileSize = rmca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(rmca.Field<object>("FILE_SIZE")) : 0,
     IsFullPath = rmca.Field<object>("IS_FULL_PATH") != null ? (rmca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
     Id = rmca.Field<byte[]>("FORM_C_DETAIL_ATTACHMENT_ID") != null ? new Guid(rmca.Field<byte[]>("FORM_C_DETAIL_ATTACHMENT_ID")) : new Guid(),

   });

    public readonly Materializer<Correspondence> PaxCorrespondenceMaterializer = new Materializer<Correspondence>(pCorr =>
   new Correspondence
   {
     Id = pCorr.Field<byte[]>("CORRESPONDENCE_ID") != null ? new Guid(pCorr.Field<byte[]>("CORRESPONDENCE_ID")) : new Guid(),
     InvoiceId = pCorr.Field<byte[]>("INVOICE_ID") != null ? new Guid(pCorr.Field<byte[]>("INVOICE_ID")) : new Guid(),
     CorrespondenceOwnerId = pCorr.Field<object>("CORRESPONDENCE_OWNER_ID") != null ? pCorr.Field<int>("CORRESPONDENCE_OWNER_ID") : 0,
     CorrespondenceNumber = pCorr.Field<object>("CORRESPONDENCE_NO") != null ? (long?)Convert.ToInt64(pCorr.Field<object>("CORRESPONDENCE_NO")) : null,
     CorrespondenceDate = pCorr.Field<object>("CORRESPONDENCE_DATE") != null ? pCorr.Field<DateTime>("CORRESPONDENCE_DATE") : new DateTime(),
     CorrespondenceStage = pCorr.Field<object>("CORRESPONDENCE_STAGE") != null ? pCorr.Field<int>("CORRESPONDENCE_STAGE") : 0,
     FromMemberId = pCorr.Field<object>("FROM_MEMBER_ID") != null ? pCorr.Field<int>("FROM_MEMBER_ID") : 0,
     ToMemberId = pCorr.Field<object>("TO_MEMBER_ID") != null ? pCorr.Field<int>("TO_MEMBER_ID") : 0,
     /* CMP#657: Retention of Additional Email Addresses in Correspondences
       * Adding code to get email ids from initiator and non-initiator and removing
       * additional email field*/
     AdditionalEmailInitiator = pCorr.Field<string>("ADDITIONAL_EMAIL_INITIATOR"),
     AdditionalEmailNonInitiator = pCorr.Field<string>("ADDITIONAL_EMAIL_NON_INITIATOR"),
     ToEmailId = pCorr.Field<string>("TO_EMAILID"),
     AmountToBeSettled = pCorr.Field<object>("AMOUNT_TO_SETTLED") != null ? pCorr.Field<decimal>("AMOUNT_TO_SETTLED") : 0,
     OurReference = pCorr.Field<string>("OUR_REFERENCE"),
     YourReference = pCorr.Field<string>("YOUR_REFERENCE"),
     Subject = pCorr.Field<string>("SUBJECT"),
     CorrespondenceDetails = pCorr.Field<string>("CORRESPONDENCE_DETAILS"),
     CorrespondenceStatusId = pCorr.Field<object>("CORRESPONDENCE_STATUS") != null ? pCorr.Field<int>("CORRESPONDENCE_STATUS") : 0,
     CorrespondenceSubStatusId = pCorr.Field<object>("CORRESPONDENCE_SUB_STATUS") != null ? pCorr.Field<int>("CORRESPONDENCE_SUB_STATUS") : 0,
     AuthorityToBill = pCorr.Field<object>("AUTHORITY_TO_BILL") != null && pCorr.Field<int>("AUTHORITY_TO_BILL") > 0,
     NumberOfDaysToExpire = pCorr.Field<object>("NO_OF_DAYS_TO_EXPIRE") != null ? pCorr.Field<int>("NO_OF_DAYS_TO_EXPIRE") : 0,
     FromEmailId = pCorr.Field<string>("FROM_EMAILID"),
     CurrencyId = pCorr.Field<object>("CURRENCY_CODE_NUM") != null ? pCorr.Field<int?>("CURRENCY_CODE_NUM") : null,
     ExpiryDate = pCorr.Field<object>("EXPIRY_DATE") != null ? pCorr.Field<DateTime>("EXPIRY_DATE") : DateTime.MinValue,
     ExpiryDatePeriod = pCorr.Field<object>("EXPIRY_DATEPERIOD") != null ? pCorr.Field<DateTime?>("EXPIRY_DATEPERIOD") : null,
     BMExpiryPeriod = pCorr.Field<object>("BM_EXPIRY_PERIOD") != null ? pCorr.Field<DateTime?>("BM_EXPIRY_PERIOD") : null,
     LastUpdatedBy = pCorr.Field<object>("LAST_UPDATED_BY") != null ? pCorr.Field<int>("LAST_UPDATED_BY") : 0,
     LastUpdatedOn = pCorr.Field<object>("LAST_UPDATED_ON") != null ? pCorr.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
     //CMP526 - Passenger Correspondence Identifiable by Source Code
     SourceCode = pCorr.Field<object>("SOURCE_CODE") != null ? pCorr.Field<int>("SOURCE_CODE") : 0,
     //CMP527 - Start
     AcceptanceComment = pCorr.Field<object>("ACCEPTANCE_COMMENTS") != null ? pCorr.Field<string>("ACCEPTANCE_COMMENTS") : null,
     AcceptanceUserName = pCorr.Field<object>("ACCEPTANCE_USER") != null ? pCorr.Field<string>("ACCEPTANCE_USER") : null,
     AcceptanceDateTime = pCorr.Field<object>("ACCEPTANCE_DATE") != null ? pCorr.Field<DateTime>("ACCEPTANCE_DATE") : new DateTime(),
     //CMP527 - End
   });

    public readonly Materializer<CorrespondenceAttachment> CorrespondenceAttachmentMaterializer = new Materializer<CorrespondenceAttachment>(ca =>
   new CorrespondenceAttachment
   {
     // TODO: uncomment FileSize
     FileStatusId = ca.Field<object>("FILE_STATUS_ID") != null ? ca.Field<int>("FILE_STATUS_ID") : 0,
     FileTypeId = ca.Field<object>("FILE_TYPE_ID") != null ? ca.Field<int>("FILE_TYPE_ID") : 0,
     FilePath = ca.Field<string>("FILE_PATH"),
     ServerId = ca.Field<object>("SERVER_ID") != null ? ca.Field<int>("SERVER_ID") : 0,
     LastUpdatedOn = ca.Field<object>("LAST_UPDATED_ON") != null ? ca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
     LastUpdatedBy = ca.Field<object>("LAST_UPDATED_BY") != null ? ca.Field<int>("LAST_UPDATED_BY") : 0,
     ReceivedDate = ca.Field<object>("RECEIVED_DATE") != null ? ca.Field<DateTime>("RECEIVED_DATE") : new DateTime(),
     FileSize = ca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(ca.Field<object>("FILE_SIZE")) : 0,
     OriginalFileName = ca.Field<string>("ORG_FILE_NAME"),
     ParentId = ca.Field<byte[]>("CORRESPONDENCE_ID") != null ? new Guid(ca.Field<byte[]>("CORRESPONDENCE_ID")) : new Guid(),
     Id = ca.Field<byte[]>("CORRESPONDENCE_ATTACHMENT_ID") != null ? new Guid(ca.Field<byte[]>("CORRESPONDENCE_ATTACHMENT_ID")) : new Guid(),
   });

    public readonly Materializer<CreditMemoAttachment> CreditMemoAttachmentMaterializer = new Materializer<CreditMemoAttachment>(mam =>
    new CreditMemoAttachment
    {
      // TODO: uncomment FileSize
      FileStatusId = mam.Field<object>("FILE_STATUS_ID") != null ? mam.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = mam.Field<object>("FILE_TYPE_ID") != null ? mam.Field<int>("FILE_TYPE_ID") : 0,
      FilePath = mam.Field<string>("FILE_PATH"),
      ServerId = mam.Field<object>("SERVER_ID") != null ? mam.Field<int>("SERVER_ID") : 0,
      LastUpdatedOn = mam.Field<object>("LAST_UPDATED_ON") != null ? mam.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = mam.Field<object>("LAST_UPDATED_BY") != null ? mam.Field<int>("LAST_UPDATED_BY") : 0,
      FileSize = mam.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(mam.Field<object>("FILE_SIZE")) : 0,
      OriginalFileName = mam.Field<string>("ORG_FILE_NAME"),
      IsFullPath = mam.Field<object>("IS_FULL_PATH") != null ? (mam.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      ParentId = mam.Field<byte[]>("CREDIT_MEMO_ID") != null ? new Guid(mam.Field<byte[]>("CREDIT_MEMO_ID")) : new Guid(),
      Id = mam.Field<byte[]>("PAX_CREDIT_MEMO_ATTACHMENT_ID") != null ? new Guid(mam.Field<byte[]>("PAX_CREDIT_MEMO_ATTACHMENT_ID")) : new Guid(),
    });

    public readonly Materializer<CMCouponAttachment> CMCouponAttachmentMaterializer = new Materializer<CMCouponAttachment>(mam =>
    new CMCouponAttachment
    {
      // TODO: uncomment FileSize
      FileStatusId = mam.Field<object>("FILE_STATUS_ID") != null ? mam.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = mam.Field<object>("FILE_TYPE_ID") != null ? mam.Field<int>("FILE_TYPE_ID") : 0,
      FilePath = mam.Field<string>("FILE_PATH"),
      ServerId = mam.Field<object>("SERVER_ID") != null ? mam.Field<int>("SERVER_ID") : 0,
      LastUpdatedOn = mam.Field<object>("LAST_UPDATED_ON") != null ? mam.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = mam.Field<object>("LAST_UPDATED_BY") != null ? mam.Field<int>("LAST_UPDATED_BY") : 0,
      FileSize = mam.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(mam.Field<object>("FILE_SIZE")) : 0,
      OriginalFileName = mam.Field<string>("ORG_FILE_NAME"),
      IsFullPath = mam.Field<object>("IS_FULL_PATH") != null ? (mam.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      ParentId = mam.Field<byte[]>("CM_COUPON_BD_ID") != null ? new Guid(mam.Field<byte[]>("CM_COUPON_BD_ID")) : new Guid(),
      Id = mam.Field<byte[]>("CM_COUPON_ATTACHMENTS_ID") != null ? new Guid(mam.Field<byte[]>("CM_COUPON_ATTACHMENTS_ID")) : new Guid(),
    });

    public readonly Materializer<BMCouponAttachment> BMCouponAttachmentMaterializer = new Materializer<BMCouponAttachment>(bmca =>
 new BMCouponAttachment
 {
   // TODO: uncomment FileSize
   FileStatusId = bmca.Field<object>("FILE_STATUS_ID") != null ? bmca.Field<int>("FILE_STATUS_ID") : 0,
   FileTypeId = bmca.Field<object>("FILE_TYPE_ID") != null ? bmca.Field<int>("FILE_TYPE_ID") : 0,
   FilePath = bmca.Field<string>("FILE_PATH"),
   ServerId = bmca.Field<object>("SERVER_ID") != null ? bmca.Field<int>("SERVER_ID") : 0,
   LastUpdatedOn = bmca.Field<object>("LAST_UPDATED_ON") != null ? bmca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
   LastUpdatedBy = bmca.Field<object>("LAST_UPDATED_BY") != null ? bmca.Field<int>("LAST_UPDATED_BY") : 0,
   FileSize = bmca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(bmca.Field<object>("FILE_SIZE")) : 0,
   OriginalFileName = bmca.Field<string>("ORG_FILE_NAME"),
   IsFullPath = bmca.Field<object>("IS_FULL_PATH") != null ? (bmca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
   ParentId = bmca.Field<byte[]>("BM_COUPON_BD_ID") != null ? new Guid(bmca.Field<byte[]>("BM_COUPON_BD_ID")) : new Guid(),
   Id = bmca.Field<byte[]>("BM_COUPON_ATTACHMENTS_ID") != null ? new Guid(bmca.Field<byte[]>("BM_COUPON_ATTACHMENTS_ID")) : new Guid(),
 });


    public readonly Materializer<BillingMemo> BillingMemoMaterializer = new Materializer<BillingMemo>(bm =>
    new BillingMemo
    {
      Id = bm.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(bm.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
      BatchSequenceNumber = bm.Field<object>("BATCH_SEQ_NO") != null ? bm.Field<int>("BATCH_SEQ_NO") : 0,
      RecordSequenceWithinBatch = bm.Field<object>("BATCH_REC_SEQ_NO") != null ? bm.Field<int>("BATCH_REC_SEQ_NO") : 0,
      SourceCodeId = bm.Field<object>("SOURCE_CODE") != null ? bm.Field<int>("SOURCE_CODE") : 0,
      BillingMemoNumber = bm.Field<string>("BILLING_MEMO_NO"),
      ReasonCode = bm.Field<string>("REASON_CODE"),
      OurRef = bm.Field<object>("INTERNAL_REF") != null ? bm.Field<string>("INTERNAL_REF") : null,
      CorrespondenceRefNumber = bm.Field<object>("CORRESPONDENCE_REF_NO") != null ? Convert.ToInt64(bm.Field<object>("CORRESPONDENCE_REF_NO")) : 0,
      FimNumber = bm.Field<object>("FIM_NO") != null ? (long?)Convert.ToInt64(bm.Field<object>("FIM_NO")) : null,
      FimCouponNumber = bm.Field<object>("FIM_COUPON_NO") != null ? bm.Field<int?>("FIM_COUPON_NO") : null,
      YourInvoiceNumber = bm.Field<object>("YOUR_INVOICE_NO") != null ? bm.Field<string>("YOUR_INVOICE_NO") : null,
      YourInvoiceBillingYear = bm.Field<object>("YOUR_INVOICE_BILLING_YEAR") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_YEAR") : 0,
      TotalGrossAmountBilled = bm.Field<object>("TOTAL_GROSS_AMT") != null ? bm.Field<decimal>("TOTAL_GROSS_AMT") : 0,
      TaxAmountBilled = bm.Field<object>("TOTAL_TAX_AMT") != null ? bm.Field<decimal>("TOTAL_TAX_AMT") : 0,
      TotalIscAmountBilled = bm.Field<object>("TOTAL_ISC_AMT") != null ? bm.Field<decimal>("TOTAL_ISC_AMT") : 0,
      TotalOtherCommissionAmount = bm.Field<object>("TOTAL_OTH_COMM_AMT") != null ? bm.Field<decimal>("TOTAL_OTH_COMM_AMT") : 0,
      TotalHandlingFeeBilled = bm.Field<object>("TOTAL_HANDLING_FEE_AMT") != null ? bm.Field<double>("TOTAL_HANDLING_FEE_AMT") : 0.0,
      TotalUatpAmountBilled = bm.Field<object>("TOTAL_UATP_AMT") != null ? bm.Field<decimal>("TOTAL_UATP_AMT") : 0,
      TotalVatAmountBilled = bm.Field<object>("TOTAL_VAT_AMT") != null ? bm.Field<decimal>("TOTAL_VAT_AMT") : 0,
      NetAmountBilled = bm.Field<object>("NET_AMT") != null ? bm.Field<decimal>("NET_AMT") : 0,
      AttachmentIndicatorOriginal = bm.Field<object>("ATTCHMNT_IND_ORIG") != null ? (bm.Field<int>("ATTCHMNT_IND_ORIG") == 0 ? 0 : (bm.Field<int>("ATTCHMNT_IND_ORIG") == 1 ? 1 : 2)) : 0,
      AttachmentIndicatorValidated = bm.Field<object>("ATTCHMNT_IND_VALIDATED") != null ? (bm.Field<int>("ATTCHMNT_IND_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
      NumberOfAttachments = bm.Field<object>("NO_OF_ATTACHMENTS") != null ? bm.Field<int?>("NO_OF_ATTACHMENTS") : null,
      AirlineOwnUse = bm.Field<object>("AIRLINE_OWN_USE") != null ? bm.Field<string>("AIRLINE_OWN_USE") : null,
      ISValidationFlag = bm.Field<object>("IS_VALIDATION_FLAG") != null ? bm.Field<string>("IS_VALIDATION_FLAG") : null,
      InvoiceId = bm.Field<byte[]>("INVOICE_ID") != null ? new Guid(bm.Field<byte[]>("INVOICE_ID")) : new Guid(),
      ReasonRemarks = bm.Field<object>("REASON_REMARKS") != null ? bm.Field<string>("REASON_REMARKS") : null,
      YourInvoiceBillingMonth = bm.Field<object>("YOUR_INVOICE_BILLING_MONTH") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_MONTH") : 0,
      YourInvoiceBillingPeriod = bm.Field<object>("YOUR_INVOICE_BILLING_PERIOD") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_PERIOD") : 0,
      LastUpdatedBy = bm.Field<object>("LAST_UPDATED_BY") != null ? bm.Field<int>("LAST_UPDATED_BY") : 0,
      LastUpdatedOn = bm.Field<object>("LAST_UPDATED_ON") != null ? bm.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
    });

    public readonly Materializer<BillingMemoVat> BillingMemoVatMaterializer = new Materializer<BillingMemoVat>(r =>
 new BillingMemoVat
 {
   LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
   LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
   ParentId = r.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(r.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
   VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
   VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
   VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
   VatText = r.Field<string>("VAT_TEXT"),
   VatLabel = r.Field<string>("VAT_LABEL"),
   VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
   Id = r.Field<byte[]>("BM_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_VAT_BD_ID")) : new Guid(),
 }
 );

    public readonly Materializer<SamplingFormC> SamplingFormCMaterializer = new Materializer<SamplingFormC>(sfc =>
     new SamplingFormC
     {
       Id = sfc.Field<byte[]>("FORM_C_ID") != null ? new Guid(sfc.Field<byte[]>("FORM_C_ID")) : new Guid(),
       ProvisionalBillingMonth = sfc.Field<object>("PROV_BILLING_MONTH") != null ? sfc.Field<int>("PROV_BILLING_MONTH") : 0,
       ProvisionalBillingMemberId = sfc.Field<object>("PROV_BILLING_MEMBER_ID") != null ? sfc.Field<int>("PROV_BILLING_MEMBER_ID") : 0,
       NilFormCIndicator = sfc.Field<object>("NIL_FORM_C_INDICATOR") != null ? sfc.Field<string>("NIL_FORM_C_INDICATOR") : null,
       ListingCurrencyId = sfc.Field<object>("LISTING_CURRENCY_CODE_NUM") != null ? sfc.Field<int>("LISTING_CURRENCY_CODE_NUM") : 0,
       FromMemberId = sfc.Field<object>("FROM_MEMBER_ID") != null ? sfc.Field<int>("FROM_MEMBER_ID") : 0,
       InputFileId = sfc.Field<byte[]>("IS_FILE_LOG_ID") != null ? new Guid(sfc.Field<byte[]>("IS_FILE_LOG_ID")) : (Guid?)null,
       ProvisionalBillingYear = sfc.Field<object>("PROV_BILLING_YEAR") != null ? sfc.Field<int>("PROV_BILLING_YEAR") : 0,
       InvoiceStatusId = sfc.Field<object>("INVOICE_STATUS_ID") != null ? sfc.Field<int>("INVOICE_STATUS_ID") : 0,
       SubmissionMethodId = sfc.Field<object>("SUBMISSION_METHOD_ID") != null ? sfc.Field<int>("SUBMISSION_METHOD_ID") : 0,
       ValidationStatus = sfc.Field<object>("VALIDATION_STATUS") != null ? sfc.Field<int>("VALIDATION_STATUS") : 0,
       ValidationStatusDate = sfc.Field<object>("VALIDATION_STATUS_DATE") != null ? sfc.Field<DateTime>("VALIDATION_STATUS_DATE") : new DateTime(),
       PresentedStatus = sfc.Field<object>("PRESENTED_STATUS") != null ? sfc.Field<int>("PRESENTED_STATUS") : 0,
       PresentedStatusDate = sfc.Field<object>("PRESENTED_STATUS_DATE") != null ? sfc.Field<DateTime>("PRESENTED_STATUS_DATE") : new DateTime(),
       IsSuspended = sfc.Field<object>("IS_SUSPENDED") != null && sfc.Field<int>("IS_SUSPENDED") > 0,
       NetAmount = sfc.Field<object>("NET_AMOUNT") != null ? sfc.Field<decimal>("NET_AMOUNT") : 0,
       // Missing mapping  for CREATED_ON in edmx file and property in model.
       IsLinkingSuccessful = sfc.Field<object>("IS_LINKING_SUCCESSFUL") != null && sfc.Field<int>("IS_LINKING_SUCCESSFUL") > 0,
       LastUpdatedBy = sfc.Field<object>("LAST_UPDATED_BY") != null ? sfc.Field<int>("LAST_UPDATED_BY") : 0,
       LastUpdatedOn = sfc.Field<object>("LAST_UPDATED_ON") != null ? sfc.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),

     });

    public readonly Materializer<SamplingFormCRecord> SamplingFormCRecordMaterializer = new Materializer<SamplingFormCRecord>(sfcr =>
    new SamplingFormCRecord
    {
      Id = sfcr.Field<byte[]>("FORM_C_DETAIL_ID") != null ? new Guid(sfcr.Field<byte[]>("FORM_C_DETAIL_ID")) : new Guid(),
      BatchNumberOfProvisionalInvoice = sfcr.Field<object>("BATCH_SEQ_NO") != null ? sfcr.Field<int>("BATCH_SEQ_NO") : 0,
      RecordSeqNumberOfProvisionalInvoice = sfcr.Field<object>("BATCH_REC_SEQ_NO") != null ? sfcr.Field<int>("BATCH_REC_SEQ_NO") : 0,
      SourceCodeId = sfcr.Field<object>("SOURCE_CODE") != null ? sfcr.Field<int>("SOURCE_CODE") : 0,
      TicketIssuingAirline = sfcr.Field<string>("TICKET_ISSUING_AIRLINE"),
      CouponNumber = sfcr.Field<object>("COUPON_NO") != null ? sfcr.Field<int>("COUPON_NO") : 0,
      DocumentNumber = sfcr.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(sfcr.Field<object>("TICKET_DOC_NO")) : 0,
      ElectronicTicketIndicator = sfcr.Field<object>("ETICKET_IND") != null && sfcr.Field<int>("ETICKET_IND") > 0,
      GrossAmountAlf = sfcr.Field<object>("GROSS_AMT") != null ? sfcr.Field<double>("GROSS_AMT") : 0.0,
      NfpReasonCode = sfcr.Field<object>("NFP_REASON_CODE") != null ? sfcr.Field<string>("NFP_REASON_CODE") : string.Empty,
      AgreementIndicatorSupplied = sfcr.Field<object>("AGREEMENT_IND_SUPPLIED") != null ? sfcr.Field<string>("AGREEMENT_IND_SUPPLIED") : string.Empty,
      AgreementIndicatorValidated = sfcr.Field<object>("AGREEMENT_IND_VALIDATED") != null ? sfcr.Field<string>("AGREEMENT_IND_VALIDATED") : string.Empty,
      OriginalPmi = sfcr.Field<object>("ORIGINAL_PMI") != null ? sfcr.Field<string>("ORIGINAL_PMI") : string.Empty,
      ValidatedPmi = sfcr.Field<object>("VALIDATED_PMI") != null ? sfcr.Field<string>("VALIDATED_PMI") : string.Empty,
      AttachmentIndicatorOriginal = sfcr.Field<object>("ATTACHMENT_IND_ORIG") != null ? (sfcr.Field<int>("ATTACHMENT_IND_ORIG") == 0 ? 0 : (sfcr.Field<int>("ATTACHMENT_IND_ORIG") == 1 ? 1 : 2)) : 0,
      AttachmentIndicatorValidated = sfcr.Field<object>("ATTACHMENT_IND_VALID") != null ? (sfcr.Field<int>("ATTACHMENT_IND_VALID") == 0 ? (bool?)false : (bool?)true) : null,
      NumberOfAttachments = sfcr.Field<object>("NO_OF_ATTACHMENTS") != null ? sfcr.Field<int?>("NO_OF_ATTACHMENTS") : null,
      ReasonCode = sfcr.Field<object>("REASON_CODE") != null ? sfcr.Field<string>("REASON_CODE") : string.Empty,
      Remarks = sfcr.Field<object>("REMARKS") != null ? sfcr.Field<string>("REMARKS") : string.Empty,
      SamplingFormCId = sfcr.Field<byte[]>("FORM_C_ID") != null ? new Guid(sfcr.Field<byte[]>("FORM_C_ID")) : new Guid(),
      ProvisionalInvoiceNumber = sfcr.Field<object>("PROV_INVOICE_NO") != null ? sfcr.Field<string>("PROV_INVOICE_NO") : string.Empty,
      LastUpdatedBy = sfcr.Field<object>("LAST_UPDATED_BY") != null ? sfcr.Field<int>("LAST_UPDATED_BY") : 0,
      LastUpdatedOn = sfcr.Field<object>("LAST_UPDATED_ON") != null ? sfcr.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),

    });

    public readonly Materializer<SamplingFormDRecord> SamplingFormDMaterializer = new Materializer<SamplingFormDRecord>(sfd =>
      new SamplingFormDRecord
      {
        Id = sfd.Field<byte[]>("FORM_D_ID") != null ? new Guid(sfd.Field<byte[]>("FORM_D_ID")) : new Guid(),
        SourceCodeId = sfd.Field<object>("SOURCE_CODE") != null ? sfd.Field<int>("SOURCE_CODE") : 0,
        ProvisionalInvoiceNumber = sfd.Field<string>("PROV_INVOICE_NO"),
        BatchNumberOfProvisionalInvoice = sfd.Field<object>("BATCH_NO") != null ? sfd.Field<int>("BATCH_NO") : 0,
        RecordSeqNumberOfProvisionalInvoice = sfd.Field<object>("BATCH_REC_SEQ_NO") != null ? sfd.Field<int>("BATCH_REC_SEQ_NO") : 0,
        TicketIssuingAirline = sfd.Field<object>("TICKET_ISSUING_AIRLINE") != null ? sfd.Field<string>("TICKET_ISSUING_AIRLINE") : string.Empty,
        CouponNumber = sfd.Field<object>("COUPON_NO") != null ? sfd.Field<int>("COUPON_NO") : 0,
        TicketDocNumber = sfd.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(sfd.Field<object>("TICKET_DOC_NO")) : 0,
        ProvisionalGrossAlfAmount = sfd.Field<object>("PROV_GROSS_AMT") != null ? sfd.Field<double>("PROV_GROSS_AMT") : 0.0,
        EvaluatedGrossAmount = sfd.Field<object>("EVAL_GROSS_AMT") != null ? sfd.Field<double>("EVAL_GROSS_AMT") : 0.0,
        IscPercent = sfd.Field<object>("EVAL_ISC_PER") != null ? sfd.Field<double>("EVAL_ISC_PER") : 0.0,
        IscAmount = sfd.Field<object>("EVAL_ISC_AMT") != null ? sfd.Field<double>("EVAL_ISC_AMT") : 0.0,
        OtherCommissionPercent = sfd.Field<object>("EVAL_OTH_COMM_PER") != null ? sfd.Field<double>("EVAL_OTH_COMM_PER") : 0.0,
        OtherCommissionAmount = sfd.Field<object>("EVAL_OTH_COMM_AMT") != null ? sfd.Field<double>("EVAL_OTH_COMM_AMT") : 0.0,
        UatpPercent = sfd.Field<object>("EVAL_UATP_PER") != null ? sfd.Field<double>("EVAL_UATP_PER") : 0.0,
        UatpAmount = sfd.Field<object>("EVAL_UATP_AMT") != null ? sfd.Field<double>("EVAL_UATP_AMT") : 0.0,
        HandlingFeeAmount = sfd.Field<object>("EVAL_HANDLING_FEE_AMT") != null ? sfd.Field<double>("EVAL_HANDLING_FEE_AMT") : 0.0,
        TaxAmount = sfd.Field<object>("EVAL_TAX_AMT") != null ? sfd.Field<double>("EVAL_TAX_AMT") : 0.0,
        VatAmount = sfd.Field<object>("EVAL_VAT_AMT") != null ? sfd.Field<double>("EVAL_VAT_AMT") : 0.0,
        EvaluatedNetAmount = sfd.Field<object>("EVAL_NET_AMT") != null ? sfd.Field<double>("EVAL_NET_AMT") : 0.0,
        ProrateMethodology = sfd.Field<string>("PRORATE_METHODOLOGY"),
        NfpReasonCode = sfd.Field<string>("NFP_REASON_CODE"),
        AgreementIndicatorSupplied = sfd.Field<string>("AGREEMENT_IND_SUPPLIED"),
        AgreementIndicatorValidated = sfd.Field<string>("AGREEMENT_IND_VALIDATED"),
        OriginalPmi = sfd.Field<string>("ORIGINAL_PMI"),
        ValidatedPmi = sfd.Field<string>("VALIDATED_PMI"),
        AttachmentIndicatorOriginal = sfd.Field<object>("ATTCHMNT_IND_ORIG") != null ? (sfd.Field<int>("ATTCHMNT_IND_ORIG") == 0 ? 0 : (sfd.Field<int>("ATTCHMNT_IND_ORIG") == 1 ? 1 : 2)) : 0,
        AttachmentIndicatorValidated = sfd.Field<object>("ATTCHMNT_IND_VALID") != null ? (sfd.Field<int>("ATTCHMNT_IND_VALID") == 0 ? (bool?)false : (bool?)true) : null,
        NumberOfAttachments = sfd.Field<object>("NO_OF_ATTACHMENTS") != null ? sfd.Field<int?>("NO_OF_ATTACHMENTS") : null,
        ReasonCode = sfd.Field<string>("REASON_CODE"),
        ReferenceField1 = sfd.Field<string>("REFERENCE_FIELD1"),
        ReferenceField2 = sfd.Field<string>("REFERENCE_FIELD2"),
        ReferenceField3 = sfd.Field<string>("REFERENCE_FIELD3"),
        ReferenceField4 = sfd.Field<string>("REFERENCE_FIELD4"),
        ReferenceField5 = sfd.Field<string>("REFERENCE_FIELD5"),
        AirlineOwnUse = sfd.Field<string>("AIRLINE_OWN_USE"),
        InvoiceId = sfd.Field<byte[]>("INVOICE_ID") != null ? new Guid(sfd.Field<byte[]>("INVOICE_ID")) : new Guid(),
        LastUpdatedBy = sfd.Field<object>("LAST_UPDATED_BY") != null ? sfd.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = sfd.Field<object>("LAST_UPDATED_ON") != null ? sfd.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        ProrateSlipDetails = sfd.Field<string>("PRORATE_SLIP_DETAIL"),
        ProvisionalBillingMonth = sfd.Field<object>("PROV_BILLING_MONTH") != null ? sfd.Field<int>("PROV_BILLING_MONTH") : 0,
      });

    public readonly Materializer<SamplingFormDTax> SamplingFormDTaxMaterializer = new Materializer<SamplingFormDTax>(t =>
      new SamplingFormDTax
      {
        LastUpdatedOn = t.Field<object>("LAST_UPDATED_ON") != null ? t.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = t.Field<object>("LAST_UPDATED_BY") != null ? t.Field<int>("LAST_UPDATED_BY") : 0,
        ParentId = t.Field<byte[]>("FORM_D_ID") != null ? new Guid(t.Field<byte[]>("FORM_D_ID")) : new Guid(),
        Amount = t.Field<object>("TAX_AMT_BILLED") != null ? t.Field<double>("TAX_AMT_BILLED") : 0.0,
        TaxCode = t.Field<string>("TAX_CODE"),
        Id = t.Field<byte[]>("FORM_D_TAX_BD_ID") != null ? new Guid(t.Field<byte[]>("FORM_D_TAX_BD_ID")) : new Guid(),
      });

    public readonly Materializer<SamplingFormDVat> SamplingFormDVatMaterializer = new Materializer<SamplingFormDVat>(v =>
      new SamplingFormDVat
      {
        LastUpdatedOn = v.Field<object>("LAST_UPDATED_ON") != null ? v.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = v.Field<object>("LAST_UPDATED_BY") != null ? v.Field<int>("LAST_UPDATED_BY") : 0,
        ParentId = v.Field<byte[]>("FORM_D_ID") != null ? new Guid(v.Field<byte[]>("FORM_D_ID")) : new Guid(),
        VatCalculatedAmount = v.Field<object>("VAT_CALC_AMT") != null ? v.Field<double>("VAT_CALC_AMT") : 0.0,
        VatPercentage = v.Field<object>("VAT_PER") != null ? v.Field<double>("VAT_PER") : 0.0,
        VatBaseAmount = v.Field<object>("VAT_BASE_AMT") != null ? v.Field<double>("VAT_BASE_AMT") : 0.0,
        VatText = v.Field<string>("VAT_TEXT"),
        VatLabel = v.Field<string>("VAT_LABEL"),
        VatIdentifierId = v.Field<object>("VAT_IDENTIFIER_ID") != null ? v.Field<int>("VAT_IDENTIFIER_ID") : 0,
        Id = v.Field<byte[]>("FORM_D_VAT_BD_ID") != null ? new Guid(v.Field<byte[]>("FORM_D_VAT_BD_ID")) : new Guid(),
      });

    public readonly Materializer<SamplingFormEDetail> SamplingFormEDetailMaterializer = new Materializer<SamplingFormEDetail>(sfed =>
         new SamplingFormEDetail
         {
           Id = sfed.Field<byte[]>("INVOICE_ID") != null ? new Guid(sfed.Field<byte[]>("INVOICE_ID")) : new Guid(),
           GrossTotalOfUniverse = sfed.Field<object>("GROSS_TOTAL_OF_UNIVERSE") != null ? sfed.Field<decimal>("GROSS_TOTAL_OF_UNIVERSE") : 0,
           GrossTotalOfUaf = sfed.Field<object>("GROSS_TOTAL_OF_UAF") != null ? sfed.Field<decimal>("GROSS_TOTAL_OF_UAF") : 0,
           UniverseAdjustedGrossAmount = sfed.Field<object>("UNIVERSE_ADJUSTED_GROSS_AMT") != null ? sfed.Field<decimal>("UNIVERSE_ADJUSTED_GROSS_AMT") : 0,
           GrossTotalOfSample = sfed.Field<object>("GROSS_TOTAL_OF_SAMPLE") != null ? sfed.Field<decimal>("GROSS_TOTAL_OF_SAMPLE") : 0,
           GrossTotalOfUafSampleCoupon = sfed.Field<object>("GROSS_TOTAL_UAF_SAMPLE_COUPON") != null ? sfed.Field<decimal>("GROSS_TOTAL_UAF_SAMPLE_COUPON") : 0,
           SampleAdjustedGrossAmount = sfed.Field<object>("SAMPLE_ADJUSTED_GROSS_AMT") != null ? sfed.Field<decimal>("SAMPLE_ADJUSTED_GROSS_AMT") : 0,
           SamplingConstant = sfed.Field<object>("SAMPLING_CONSTANT") != null ? sfed.Field<double>("SAMPLING_CONSTANT") : 0.0,
           TotalOfGrossAmtXSamplingConstant = sfed.Field<object>("TOTAL_GROSS_AMT_X_SC") != null ? sfed.Field<decimal>("TOTAL_GROSS_AMT_X_SC") : 0,
           TotalOfIscAmtXSamplingConstant = sfed.Field<object>("TOTAL_ISC_AMT_X_SC") != null ? sfed.Field<decimal>("TOTAL_ISC_AMT_X_SC") : 0,
           TotalOfOtherCommissionAmtXSamplingConstant = sfed.Field<object>("TOTAL_OTH_COMM_AMT_X_SC") != null ? sfed.Field<decimal>("TOTAL_OTH_COMM_AMT_X_SC") : 0,
           UatpCouponTotalXSamplingConstant = sfed.Field<object>("UATP_COUPON_TOTAL_X_SC") != null ? sfed.Field<decimal>("UATP_COUPON_TOTAL_X_SC") : 0,
           HandlingFeeTotalAmtXSamplingConstant = sfed.Field<object>("TOTAL_HANDLING_FEE_AMT_X_SC") != null ? sfed.Field<double>("TOTAL_HANDLING_FEE_AMT_X_SC") : 0.0,
           TaxCouponTotalsXSamplingConstant = sfed.Field<object>("TOTAL_TAX_COUPON_X_SC") != null ? sfed.Field<decimal>("TOTAL_TAX_COUPON_X_SC") : 0,
           VatCouponTotalsXSamplingConstant = sfed.Field<object>("VAT_COUPON_TOTAL_X_SC") != null ? sfed.Field<decimal>("VAT_COUPON_TOTAL_X_SC") : 0,
           NetAmountDue = sfed.Field<object>("NET_DUE_AMT") != null ? sfed.Field<decimal>("NET_DUE_AMT") : 0,
           NetAmountDueInCurrencyOfBilling = sfed.Field<object>("NET_DUE_AMT_BILLING_CURRENCY") != null ? sfed.Field<decimal>("NET_DUE_AMT_BILLING_CURRENCY") : 0,
           ProvisionalFormBGrossBilled = sfed.Field<object>("PROV_FORM_B_GROSS_BILLED") != null ? sfed.Field<decimal>("PROV_FORM_B_GROSS_BILLED") : 0,
           ProvisionalFormBTaxAmount = sfed.Field<object>("PROV_FORM_B_TAX_AMT") != null ? sfed.Field<decimal>("PROV_FORM_B_TAX_AMT") : 0,
           ProvisionalFormBIscAmount = sfed.Field<object>("PROV_FORM_B_ISC_AMT") != null ? sfed.Field<decimal>("PROV_FORM_B_ISC_AMT") : 0,
           ProvisionalFormBOtherCommissionAmount = sfed.Field<object>("PROV_FORM_B_OTH_COMM_AMT") != null ? sfed.Field<decimal>("PROV_FORM_B_OTH_COMM_AMT") : 0,
           ProvisionalFormBUatpAmount = sfed.Field<object>("PROV_FORM_B_UATP_AMT") != null ? sfed.Field<decimal>("PROV_FORM_B_UATP_AMT") : 0,
           ProvisionalFormBHandlingFeeAmountBilled = sfed.Field<object>("PROV_FORM_B_HANDLING_FEE_AMT") != null ? sfed.Field<double>("PROV_FORM_B_HANDLING_FEE_AMT") : 0.0,
           ProvisionalFormBVatAmountBilled = sfed.Field<object>("PROV_FORM_B_VAT_AMT_BILLED") != null ? sfed.Field<decimal>("PROV_FORM_B_VAT_AMT_BILLED") : 0,
           TotalAmountFormB = sfed.Field<object>("FORM_B_TOTAL_AMT") != null ? sfed.Field<decimal>("FORM_B_TOTAL_AMT") : 0,
           NetBilledCreditedAmount = sfed.Field<object>("NET_BILLED_CREDITED_AMT") != null ? sfed.Field<decimal>("NET_BILLED_CREDITED_AMT") : 0,
           NumberOfBillingRecords = sfed.Field<object>("BILLING_RECORDS_NO") != null ? sfed.Field<int>("BILLING_RECORDS_NO") : 0,
           TotalGrossValue = sfed.Field<object>("TOTAL_GROSS_VALUE") != null ? sfed.Field<decimal>("TOTAL_GROSS_VALUE") : 0,
           TotalIscAmount = sfed.Field<object>("TOTAL_INTERLINE_SERV_CHARGE") != null ? sfed.Field<decimal>("TOTAL_INTERLINE_SERV_CHARGE") : 0,
           TotalTaxAmount = sfed.Field<object>("TOTAL_TAX_AMT") != null ? sfed.Field<decimal>("TOTAL_TAX_AMT") : 0,
           TotalOtherCommission = sfed.Field<object>("TOTAL_OTH_COMM_AMT") != null ? sfed.Field<decimal>("TOTAL_OTH_COMM_AMT") : 0,
           TotalHandlingFee = sfed.Field<object>("HANDLING_FEE_AMT") != null ? sfed.Field<double>("HANDLING_FEE_AMT") : 0.0,
           TotalUatpAmount = sfed.Field<object>("TOTAL_UATP_AMT") != null ? sfed.Field<decimal>("TOTAL_UATP_AMT") : 0,
           TotalVatAmount = sfed.Field<object>("TOTAL_VAT_AMT") != null ? sfed.Field<decimal>("TOTAL_VAT_AMT") : 0,
           LastUpdatedBy = sfed.Field<object>("LAST_UPDATED_BY") != null ? sfed.Field<int>("LAST_UPDATED_BY") : 0,
           LastUpdatedOn = sfed.Field<object>("LAST_UPDATED_ON") != null ? sfed.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
         });

    public readonly Materializer<PaxInvoice> PaxInvoiceAuditMaterializer = new Materializer<PaxInvoice>(r =>
     new PaxInvoice
     {
       Id = r.TryGetField<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
       BillingPeriod = r.Field<object>("PERIOD_NO") != null ? r.Field<int>("PERIOD_NO") : 0,
       BillingMonth = r.Field<object>("BILLING_MONTH") != null ? r.Field<int>("BILLING_MONTH") : 0,
       BillingYear = r.Field<object>("BILLING_YEAR") != null ? r.Field<int>("BILLING_YEAR") : 0,
       BillingMemberId = r.Field<object>("BILLING_MEMBER_ID") != null ? r.Field<int>("BILLING_MEMBER_ID") : 0,
       BilledMemberId = r.Field<object>("BILLED_MEMBER_ID") != null ? r.Field<int>("BILLED_MEMBER_ID") : 0,
       InvoiceNumber = r.Field<string>("INVOICE_NO"),
       BillingCode = r.Field<object>("BILLING_CODE_ID") != null ? r.Field<int>("BILLING_CODE_ID") : 0,
       ListingCurrencyId = r.Field<object>("LISTING_CURRENCY_CODE_NUM") != null ? r.Field<int>("LISTING_CURRENCY_CODE_NUM") : 0,
       ProvisionalBillingMonth = r.Field<object>("PROV_BILLING_MONTH") != null ? r.Field<int>("PROV_BILLING_MONTH") : 0,
       ProvisionalBillingYear = r.Field<object>("PROV_BILLING_YEAR") != null ? r.Field<int>("PROV_BILLING_YEAR") : 0,
     });

    public readonly Materializer<SourceCode> PaxInoiveSourceCodeAuditMaterializer = new Materializer<SourceCode>(r =>
      new SourceCode
      {
        Id = r.Field<object>("SOURCE_CODE_ID") != null ? r.Field<int>("SOURCE_CODE_ID") : 0,
        SourceCodeDescription = r.Field<string>("DESCRIPTION"),
      });

    public readonly Materializer<PrimeCoupon> PaxInvoiceCouponAuditMaterializer = new Materializer<PrimeCoupon>(r =>
        new PrimeCoupon
        {
          Id = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
          TicketOrFimIssuingAirline = r.Field<object>("TICKET_ISSUING_AIRLINE") != null ? r.Field<string>("TICKET_ISSUING_AIRLINE") : string.Empty,
          TicketOrFimCouponNumber = r.Field<object>("TICKET_COUPON_NO") != null ? r.Field<int>("TICKET_COUPON_NO") : 0,
          TicketDocOrFimNumber = r.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOC_NO")) : 0,
          InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
          SourceCodeId = r.Field<object>("SOURCE_CODE") != null ? r.Field<int>("SOURCE_CODE") : 0,
          BatchSequenceNumber = r.Field<object>("BATCH_SEQ_NO") != null ? r.Field<int>("BATCH_SEQ_NO") : 0,
          RecordSequenceWithinBatch = r.Field<object>("BATCH_RECORD_SEQ") != null ? r.Field<int>("BATCH_RECORD_SEQ") : 0,
          FromAirportOfCoupon = r.Field<string>("FROM_AIRPORT_OF_COUPON"),
          ToAirportOfCoupon = r.Field<string>("TO_AIRPORT_OF_COUPON"),
          CouponGrossValueOrApplicableLocalFare = r.Field<object>("COUPON_GROSS_VALUE") != null ? r.Field<double>("COUPON_GROSS_VALUE") : 0.0,
          IscPercent = r.Field<object>("INTERLINE_SERV_CHARGE_PER") != null ? r.Field<double>("INTERLINE_SERV_CHARGE_PER") : 0.0,
          TaxAmount = r.Field<object>("COUPON_TAX_AMT") != null ? r.Field<double>("COUPON_TAX_AMT") : 0.0,
          OriginalPmi = r.Field<string>("ORIGINAL_PMI"),
          ValidatedPmi = r.Field<string>("VALIDATED_PMI"),
          AirlineFlightDesignator = r.Field<string>("AIRLINE_FLIGHT_DESIGNATOR"),
          HandlingFeeTypeId = r.Field<object>("HANDLING_FEE_TYPE_ID") != null ? r.Field<string>("HANDLING_FEE_TYPE_ID") : string.Empty,
          HandlingFeeAmount = r.Field<object>("HANDLING_FEE_AMT") != null ? r.Field<double>("HANDLING_FEE_AMT") : 0.0,
          SettlementAuthorizationCode = r.Field<string>("SETTLEMENT_AUTH_CODE"),
          IscAmount = r.Field<object>("ISC_AMT") != null ? r.Field<double>("ISC_AMT") : 0.0,
          OtherCommissionPercent = r.Field<object>("OTH_COMM_PER") != null ? r.Field<double>("OTH_COMM_PER") : 0.0,
          OtherCommissionAmount = r.Field<object>("OTH_COMM_AMT") != null ? r.Field<double>("OTH_COMM_AMT") : 0.0,
          UatpPercent = r.Field<object>("UATP_PER") != null ? r.Field<double>("UATP_PER") : 0.0,
          UatpAmount = r.Field<object>("UATP_AMT") != null ? r.Field<double>("UATP_AMT") : 0.0,
          VatAmount = r.Field<object>("VAT_AMT") != null ? r.Field<double>("VAT_AMT") : 0.0,
          CouponTotalAmount = r.Field<object>("COUPON_TOTAL_AMT") != null ? r.Field<double>("COUPON_TOTAL_AMT") : 0.0
        });

    public readonly Materializer<PrimeCouponTax> CouponTaxAuditMaterializer = new Materializer<PrimeCouponTax>(r =>
     new PrimeCouponTax
     {
       Id = r.Field<byte[]>("COUPON_TAX_BD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_TAX_BD_ID")) : new Guid(),
       TaxCode = r.Field<string>("TAX_CODE"),
       Amount = r.Field<object>("TAX_AMT_BILLED") != null ? r.Field<double>("TAX_AMT_BILLED") : 0,
       ParentId = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
     });

    public readonly Materializer<PrimeCouponVat> CouponVatAuditMaterializer = new Materializer<PrimeCouponVat>(r =>
      new PrimeCouponVat
      {
        Id = r.Field<byte[]>("COUPON_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_VAT_BD_ID")) : new Guid(),
        VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
        VatLabel = r.Field<string>("VAT_LABEL"),
        VatText = r.Field<string>("VAT_TEXT"),
        VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0,
        VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0,
        VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0,
        ParentId = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
      });

    public readonly Materializer<SamplingFormDRecord> SamplingFormDAuditMaterializer = new Materializer<SamplingFormDRecord>(sfd =>
      new SamplingFormDRecord
      {
        Id = sfd.Field<byte[]>("FORM_D_ID") != null ? new Guid(sfd.Field<byte[]>("FORM_D_ID")) : new Guid(),
        SourceCodeId = sfd.Field<object>("SOURCE_CODE") != null ? sfd.Field<int>("SOURCE_CODE") : 0,
        ProvisionalInvoiceNumber = sfd.Field<string>("PROV_INVOICE_NO"),
        BatchNumberOfProvisionalInvoice = sfd.Field<object>("BATCH_NO") != null ? sfd.Field<int>("BATCH_NO") : 0,
        RecordSeqNumberOfProvisionalInvoice = sfd.Field<object>("BATCH_REC_SEQ_NO") != null ? sfd.Field<int>("BATCH_REC_SEQ_NO") : 0,
        TicketIssuingAirline = sfd.Field<object>("TICKET_ISSUING_AIRLINE") != null ? sfd.Field<string>("TICKET_ISSUING_AIRLINE") : string.Empty,
        CouponNumber = sfd.Field<object>("COUPON_NO") != null ? sfd.Field<int>("COUPON_NO") : 0,
        TicketDocNumber = sfd.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(sfd.Field<object>("TICKET_DOC_NO")) : 0,
        ProvisionalGrossAlfAmount = sfd.Field<object>("PROV_GROSS_AMT") != null ? sfd.Field<double>("PROV_GROSS_AMT") : 0.0,
        EvaluatedGrossAmount = sfd.Field<object>("EVAL_GROSS_AMT") != null ? sfd.Field<double>("EVAL_GROSS_AMT") : 0.0,
        IscPercent = sfd.Field<object>("EVAL_ISC_PER") != null ? sfd.Field<double>("EVAL_ISC_PER") : 0.0,
        IscAmount = sfd.Field<object>("EVAL_ISC_AMT") != null ? sfd.Field<double>("EVAL_ISC_AMT") : 0.0,
        OtherCommissionPercent = sfd.Field<object>("EVAL_OTH_COMM_PER") != null ? sfd.Field<double>("EVAL_OTH_COMM_PER") : 0.0,
        OtherCommissionAmount = sfd.Field<object>("EVAL_OTH_COMM_AMT") != null ? sfd.Field<double>("EVAL_OTH_COMM_AMT") : 0.0,
        UatpPercent = sfd.Field<object>("EVAL_UATP_PER") != null ? sfd.Field<double>("EVAL_UATP_PER") : 0.0,
        UatpAmount = sfd.Field<object>("EVAL_UATP_AMT") != null ? sfd.Field<double>("EVAL_UATP_AMT") : 0.0,
        HandlingFeeAmount = sfd.Field<object>("EVAL_HANDLING_FEE_AMT") != null ? sfd.Field<double>("EVAL_HANDLING_FEE_AMT") : 0.0,
        TaxAmount = sfd.Field<object>("EVAL_TAX_AMT") != null ? sfd.Field<double>("EVAL_TAX_AMT") : 0.0,
        VatAmount = sfd.Field<object>("EVAL_VAT_AMT") != null ? sfd.Field<double>("EVAL_VAT_AMT") : 0.0,
        EvaluatedNetAmount = sfd.Field<object>("EVAL_NET_AMT") != null ? sfd.Field<double>("EVAL_NET_AMT") : 0.0,
        ProrateMethodology = sfd.Field<string>("PRORATE_METHODOLOGY"),
        NfpReasonCode = sfd.Field<string>("NFP_REASON_CODE"),
        AgreementIndicatorSupplied = sfd.Field<string>("AGREEMENT_IND_SUPPLIED"),
        AgreementIndicatorValidated = sfd.Field<string>("AGREEMENT_IND_VALIDATED"),
        OriginalPmi = sfd.Field<string>("ORIGINAL_PMI"),
        ValidatedPmi = sfd.Field<string>("VALIDATED_PMI"),
        AttachmentIndicatorOriginal = sfd.Field<object>("ATTCHMNT_IND_ORIG") != null ? sfd.Field<int>("ATTCHMNT_IND_ORIG") : 0,
        AttachmentIndicatorValidated = sfd.Field<object>("ATTCHMNT_IND_VALID") != null && sfd.Field<int>("ATTCHMNT_IND_VALID") > 0,
        NumberOfAttachments = sfd.Field<object>("NO_OF_ATTACHMENTS") != null ? sfd.Field<int>("NO_OF_ATTACHMENTS") : 0,
        ReasonCode = sfd.Field<string>("REASON_CODE"),
        ReferenceField1 = sfd.Field<string>("REFERENCE_FIELD1"),
        ReferenceField2 = sfd.Field<string>("REFERENCE_FIELD2"),
        ReferenceField3 = sfd.Field<string>("REFERENCE_FIELD3"),
        ReferenceField4 = sfd.Field<string>("REFERENCE_FIELD4"),
        ReferenceField5 = sfd.Field<string>("REFERENCE_FIELD5"),
        AirlineOwnUse = sfd.Field<string>("AIRLINE_OWN_USE"),
        InvoiceId = sfd.Field<byte[]>("INVOICE_ID") != null ? new Guid(sfd.Field<byte[]>("INVOICE_ID")) : new Guid(),
        LastUpdatedBy = sfd.Field<object>("LAST_UPDATED_BY") != null ? sfd.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = sfd.Field<object>("LAST_UPDATED_ON") != null ? sfd.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        ProrateSlipDetails = sfd.Field<string>("PRORATE_SLIP_DETAIL"),
        ProvisionalBillingMonth = sfd.Field<object>("PROV_BILLING_MONTH") != null ? sfd.Field<int>("PROV_BILLING_MONTH") : 0,
      });

    /// <summary>
    /// Rejection Memo Materializer for stage1, 2 & 3 Rejection.
    /// </summary>
    public readonly Materializer<RejectionMemo> PaxInvoiceRejectionMemoAuditMaterializer = new Materializer<RejectionMemo>(r =>
        new RejectionMemo()
        {
          Id = r.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(r.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
          BatchSequenceNumber = r.Field<object>("BATCH_SEQ_NO") != null ? r.Field<int>("BATCH_SEQ_NO") : 0,
          RecordSequenceWithinBatch = r.Field<object>("BATCH_REC_SEQ_NO") != null ? r.Field<int>("BATCH_REC_SEQ_NO") : 0,
          RejectionMemoNumber = r.Field<string>("REJECTION_MEMO_NO"),
          RejectionStage = r.Field<object>("REJECTION_STAGE") != null ? r.Field<int>("REJECTION_STAGE") : 0,
          SourceCodeId = r.Field<object>("SOURCE_CODE") != null ? r.Field<int>("SOURCE_CODE") : 0,
          ReasonCode = r.Field<string>("REASON_CODE"),
          YourInvoiceNumber = r.Field<string>("YOUR_INVOICE_NO"),
          YourInvoiceBillingYear = r.Field<object>("YOUR_INVOICE_BILL_YEAR") != null ? r.Field<int>("YOUR_INVOICE_BILL_YEAR") : 0,
          YourRejectionNumber = r.Field<string>("YOUR_REJ_NO"),
          FimBMCMNumber = r.Field<string>("FIM_BM_CM_NO"),
          FimCouponNumber = r.Field<object>("FIM_COUPON_NO") != null ? r.Field<int?>("FIM_COUPON_NO") : null,
          TotalGrossAmountBilled = r.Field<object>("TOTAL_GROSS_BILLED_AMT") != null ? r.Field<double>("TOTAL_GROSS_BILLED_AMT") : 0.0,
          TotalGrossAcceptedAmount = r.Field<object>("TOTAL_GROSS_ACCEPTED_AMT") != null ? r.Field<double>("TOTAL_GROSS_ACCEPTED_AMT") : 0.0,
          TotalGrossDifference = r.Field<object>("TOTAL_GROSS_DIFF") != null ? r.Field<double>("TOTAL_GROSS_DIFF") : 0.0,
          TotalTaxAmountBilled = r.Field<object>("TOTAL_TAX_BILLED_AMT") != null ? r.Field<double>("TOTAL_TAX_BILLED_AMT") : 0.0,
          TotalTaxAmountAccepted = r.Field<object>("TOTAL_TAX_ACCPTED_AMT") != null ? r.Field<double>("TOTAL_TAX_ACCPTED_AMT") : 0.0,
          TotalTaxAmountDifference = r.Field<object>("TOTAL_TAX_DIFF") != null ? r.Field<double>("TOTAL_TAX_DIFF") : 0.0,
          AllowedIscAmount = r.Field<object>("TOTAL_ISC_ALLOWED_AMT") != null ? r.Field<double>("TOTAL_ISC_ALLOWED_AMT") : 0.0,
          AcceptedIscAmount = r.Field<object>("TOTAL_ISC_ACCPTED_AMT") != null ? r.Field<double>("TOTAL_ISC_ACCPTED_AMT") : 0.0,
          IscDifference = r.Field<object>("TOTAL_ISC_DIFF") != null ? r.Field<double>("TOTAL_ISC_DIFF") : 0.0,
          AllowedOtherCommission = r.Field<object>("TOTAL_OTH_COMM_ALLOWED") != null ? r.Field<double>("TOTAL_OTH_COMM_ALLOWED") : 0.0,
          AcceptedOtherCommission = r.Field<object>("TOTAL_OTH_COMM_ACCPTED") != null ? r.Field<double>("TOTAL_OTH_COMM_ACCPTED") : 0.0,
          OtherCommissionDifference = r.Field<object>("TOTAL_OTH_COMM_DIFF") != null ? r.Field<double>("TOTAL_OTH_COMM_DIFF") : 0.0,
          AllowedHandlingFee = r.Field<object>("TOTAL_HANDLING_FEE_ALLOWED") != null ? r.Field<double>("TOTAL_HANDLING_FEE_ALLOWED") : 0.0,
          AcceptedHandlingFee = r.Field<object>("TOTAL_HANDLING_FEE_ACCPTED") != null ? r.Field<double>("TOTAL_HANDLING_FEE_ACCPTED") : 0.0,
          HandlingFeeAmountDifference = r.Field<object>("TOTAL_HANDLING_FEE_DIFF") != null ? r.Field<double>("TOTAL_HANDLING_FEE_DIFF") : 0.0,
          AllowedUatpAmount = r.Field<object>("TOTAL_UATP_ALLOWED_AMT") != null ? r.Field<double>("TOTAL_UATP_ALLOWED_AMT") : 0.0,
          AcceptedUatpAmount = r.Field<object>("TOTAL_UATP_ACCPTED_AMT") != null ? r.Field<double>("TOTAL_UATP_ACCPTED_AMT") : 0.0,
          UatpAmountDifference = r.Field<object>("TOTAL_UATP_DIFF") != null ? r.Field<double>("TOTAL_UATP_DIFF") : 0.0,
          TotalVatAmountBilled = r.Field<object>("TOTAL_BILLED_VAT_AMT") != null ? r.Field<double>("TOTAL_BILLED_VAT_AMT") : 0.0,
          TotalVatAmountAccepted = r.Field<object>("TOTAL_ACCPT_VAT_AMT") != null ? r.Field<double>("TOTAL_ACCPT_VAT_AMT") : 0.0,
          TotalVatAmountDifference = r.Field<object>("TOTAL_VAT_DIFF") != null ? r.Field<double>("TOTAL_VAT_DIFF") : 0.0,
          TotalNetRejectAmount = r.Field<object>("TOTAL_NET_REJ_AMT") != null ? r.Field<decimal>("TOTAL_NET_REJ_AMT") : 0.0m,
          SamplingConstant = r.Field<object>("SAMPLING_CONSTANT") != null ? r.Field<double>("SAMPLING_CONSTANT") : 0.0,
          TotalNetRejectAmountAfterSamplingConstant = r.Field<object>("TOTAL_NRA_AFTER_SAMPLING") != null ? r.Field<decimal>("TOTAL_NRA_AFTER_SAMPLING") : 0.0m,
          AttachmentIndicatorOriginal = r.Field<object>("ATTCHMNT_IND_ORIG") != null ? (r.Field<int>("ATTCHMNT_IND_ORIG") == 0 ? 0 : (r.Field<int>("ATTCHMNT_IND_ORIG") == 1 ? 1 : 2)) : 0,
          AttachmentIndicatorValidated = r.Field<object>("ATTACHMENT_IND_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_IND_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
          NumberOfAttachments = r.Field<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int?>("NO_OF_ATTACHMENTS") : null,
          AirlineOwnUse = r.Field<string>("AIRLINE_OWN_USE"),
          ISValidationFlag = r.Field<string>("IS_VALID_FLAG"),
          IsRejectionFlag = r.Field<string>("IS_REJECT_FLAG"),
          IsLinkingSuccessful = r.Field<object>("IS_LINKING_SUCCESSFUL") != null ? (bool?)(r.Field<int>("IS_LINKING_SUCCESSFUL") > 0) : null,
          InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
          CorrespondenceId = r.Field<byte[]>("CORRESPONDENCE_ID") != null ? (Guid?)new Guid(r.Field<byte[]>("CORRESPONDENCE_ID")) : null,
          ReasonRemarks = r.Field<string>("REASON_REMARKS"),
          YourInvoiceBillingMonth = r.Field<object>("YOUR_INVOICE_BILL_MONTH") != null ? r.Field<int>("YOUR_INVOICE_BILL_MONTH") : 0,
          YourInvoiceBillingPeriod = r.Field<object>("YOUR_INVOICE_BILL_PERIOD") != null ? r.Field<int>("YOUR_INVOICE_BILL_PERIOD") : 0,
          FIMBMCMIndicatorId = r.Field<object>("FIM_BM_CM_INDICATOR") != null ? r.Field<int>("FIM_BM_CM_INDICATOR") : 0,
        });

    public readonly Materializer<RejectionMemoVat> RejectionMemoVatAuditMaterializer = new Materializer<RejectionMemoVat>(r =>
     new RejectionMemoVat
     {
       ParentId = r.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(r.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
       VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
       VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
       VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
       VatText = r.Field<string>("VAT_TEXT"),
       VatLabel = r.Field<string>("VAT_LABEL"),
       VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
       Id = r.Field<byte[]>("RM_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("RM_VAT_BD_ID")) : new Guid(),
     }
     );

    public readonly Materializer<RejectionMemoAttachment> RejectionMemoAttachmentAuditMaterializer = new Materializer<RejectionMemoAttachment>(rma =>
    new RejectionMemoAttachment
    {
      ServerId = rma.Field<object>("SERVER_ID") != null ? rma.Field<int>("SERVER_ID") : 0,
      FilePath = rma.Field<string>("FILE_PATH"),
      OriginalFileName = rma.Field<string>("ORG_FILE_NAME"),
      LastUpdatedOn = rma.Field<object>("LAST_UPDATED_ON") != null ? rma.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = rma.Field<object>("LAST_UPDATED_BY") != null ? rma.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = rma.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(rma.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
      FileStatusId = rma.Field<object>("FILE_STATUS_ID") != null ? rma.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = rma.Field<object>("FILE_TYPE_ID") != null ? rma.Field<int>("FILE_TYPE_ID") : 0,
      FileSize = rma.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(rma.Field<object>("FILE_SIZE")) : 0,
      IsFullPath = rma.Field<object>("IS_FULL_PATH") != null ? (rma.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      Id = rma.Field<byte[]>("PAX_RM_ATTACHMENTS_ID") != null ? new Guid(rma.Field<byte[]>("PAX_RM_ATTACHMENTS_ID")) : new Guid(),
    });

    public readonly Materializer<RMCoupon> RMCouponAuditMaterializer = new Materializer<RMCoupon>(r =>
       new RMCoupon
       {
         //TODO : Check TicketDocOrFimNumber object 
         Id = r.TryGetField<byte[]>("RM_COUPON_BREAKDOWN_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_BREAKDOWN_ID")) : new Guid(),
         TicketOrFimIssuingAirline = r.TryGetField<object>("TICKET_ISSUING_AIRLINE") != null ? r.Field<string>("TICKET_ISSUING_AIRLINE") : string.Empty,
         TicketOrFimCouponNumber = r.TryGetField<object>("COUPON_NO") != null ? r.Field<int>("COUPON_NO") : 0,
         TicketDocOrFimNumber = r.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOC_NO")) : 0,
         CheckDigit = r.TryGetField<object>("CHECK_DIGIT") != null ? r.Field<int>("CHECK_DIGIT") : 0, //test
         FromAirportOfCoupon = r.TryGetField<string>("FROM_AIRPORT_OF_COUPON"),
         ToAirportOfCoupon = r.TryGetField<string>("TO_AIRPORT_OF_COUPON"),
         GrossAmountBilled = r.TryGetField<object>("BILLED_GROSS_AMT") != null ? r.Field<double>("BILLED_GROSS_AMT") : 0.0,
         GrossAmountAccepted = r.TryGetField<object>("ACCEPTED_GROSS_AMT") != null ? r.Field<double>("ACCEPTED_GROSS_AMT") : 0.0,
         GrossAmountDifference = r.TryGetField<object>("GROSS_AMT_DIFF") != null ? r.Field<double>("GROSS_AMT_DIFF") : 0.0,

         TaxAmountBilled = r.TryGetField<object>("BILLED_TAX_AMT") != null ? r.Field<double>("BILLED_TAX_AMT") : 0.0,
         TaxAmountAccepted = r.TryGetField<object>("ACCPT_TAX_AMT") != null ? r.Field<double>("ACCPT_TAX_AMT") : 0.0,
         TaxAmountDifference = r.TryGetField<object>("TAX_AMT_DIFF") != null ? r.Field<double>("TAX_AMT_DIFF") : 0.0,
         AllowedIscPercentage = r.TryGetField<object>("ALLOWED_ISC_PER") != null ? r.Field<double>("ALLOWED_ISC_PER") : 0.0,
         AllowedIscAmount = r.TryGetField<object>("ALLOWED_ISC_AMT") != null ? r.Field<double>("ALLOWED_ISC_AMT") : 0.0,
         AcceptedIscPercentage = r.TryGetField<object>("ACCPT_ISC_PER") != null ? r.Field<double>("ACCPT_ISC_PER") : 0.0,
         AcceptedIscAmount = r.TryGetField<object>("ACCPT_ISC_AMT") != null ? r.Field<double>("ACCPT_ISC_AMT") : 0.0,
         IscDifference = r.TryGetField<object>("ISC_AMT_DIFF") != null ? r.Field<double>("ISC_AMT_DIFF") : 0.0,

         AllowedOtherCommissionPercentage = r.TryGetField<object>("ALLOWED_OTH_COMM_PER") != null ? r.Field<double>("ALLOWED_OTH_COMM_PER") : 0.0,
         AllowedOtherCommission = r.TryGetField<object>("ALLOWED_OTH_COMM_AMT") != null ? r.Field<double>("ALLOWED_OTH_COMM_AMT") : 0.0,
         AcceptedOtherCommissionPercentage = r.TryGetField<object>("ACCPT_OTH_COMM_PER") != null ? r.Field<double>("ACCPT_OTH_COMM_PER") : 0.0,
         AcceptedOtherCommission = r.TryGetField<object>("ACCPT_OTH_COMM_AMT") != null ? r.Field<double>("ACCPT_OTH_COMM_AMT") : 0.0,
         OtherCommissionDifference = r.TryGetField<object>("OTH_COMM_AMT_DIFF") != null ? r.Field<double>("OTH_COMM_AMT_DIFF") : 0.0,

         AllowedHandlingFee = r.TryGetField<object>("ALLOWED_HANDLING_FEE_AMT") != null ? r.Field<double>("ALLOWED_HANDLING_FEE_AMT") : 0.0,
         AcceptedHandlingFee = r.TryGetField<object>("ACCPT_HANDLING_FEE_AMT") != null ? r.Field<double>("ACCPT_HANDLING_FEE_AMT") : 0.0,
         HandlingDifference = r.TryGetField<object>("ACCPT_HANDLING_AMT_DIFF") != null ? r.Field<double>("ACCPT_HANDLING_AMT_DIFF") : 0.0,

         AllowedUatpPercentage = r.TryGetField<object>("ALLOWED_UATP_PER") != null ? r.Field<double>("ALLOWED_UATP_PER") : 0.0,
         AllowedUatpAmount = r.TryGetField<object>("ALLOWED_UATP_AMT") != null ? r.Field<double>("ALLOWED_UATP_AMT") : 0.0,
         AcceptedUatpPercentage = r.TryGetField<object>("ACCPT_UATP_PER") != null ? r.Field<double>("ACCPT_UATP_PER") : 0.0,
         AcceptedUatpAmount = r.TryGetField<object>("ACCPT_UATP_AMT") != null ? r.Field<double>("ACCPT_UATP_AMT") : 0.0,
         UatpDifference = r.TryGetField<object>("UATP_AMT_DIFF") != null ? r.Field<double>("UATP_AMT_DIFF") : 0.0,

         VatAmountBilled = r.TryGetField<object>("BILLED_VAT_AMT") != null ? r.Field<double>("BILLED_VAT_AMT") : 0.0,
         VatAmountAccepted = r.TryGetField<object>("ACCPT_VAT_AMT") != null ? r.Field<double>("ACCPT_VAT_AMT") : 0.0,
         VatAmountDifference = r.TryGetField<object>("VAT_AMT_DIFF") != null ? r.Field<double>("VAT_AMT_DIFF") : 0.0,

         NetRejectAmount = r.TryGetField<object>("NET_REJECT_AMT") != null ? r.Field<double>("NET_REJECT_AMT") : 0.0,

         NfpReasonCode = r.TryGetField<string>("NFP_REASON_CODE"),
         AgreementIndicatorSupplied = r.TryGetField<string>("AGREEMENT_IND_SUPPLIED"),
         AgreementIndicatorValidated = r.TryGetField<string>("AGREEMENT_IND_VALIDATED"),
         OriginalPmi = r.TryGetField<string>("ORIGINAL_PMI"),
         ValidatedPmi = r.TryGetField<string>("VALIDATED_PMI"),

         SettlementAuthorizationCode = r.TryGetField<string>("SETTLEMENT_AUTH_CODE"),
         AttachmentIndicatorOriginal = r.TryGetField<object>("ATTACHMENT_IND_ORIG") != null ? (r.Field<int>("ATTACHMENT_IND_ORIG") == 0 ? 0 : (r.Field<int>("ATTACHMENT_IND_ORIG") == 1 ? 1 : 2)) : 0,
         AttachmentIndicatorValidated = r.TryGetField<object>("ATTACHMENT_IND_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_IND_VALIDATED") == 0 ? false : true) : false,
         NumberOfAttachments = r.TryGetField<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int>("NO_OF_ATTACHMENTS") : 0,

         ISValidationFlag = r.TryGetField<string>("IS_VALIDATION_FLAG"),
         ReasonCode = r.TryGetField<string>("REASON_CODE"),
         ReferenceField1 = r.TryGetField<string>("REFERENCE_FIELD1"),
         ReferenceField2 = r.TryGetField<string>("REFERENCE_FIELD2"),
         ReferenceField3 = r.TryGetField<string>("REFERENCE_FIELD3"),
         ReferenceField4 = r.TryGetField<string>("REFERENCE_FIELD4"),
         ReferenceField5 = r.TryGetField<string>("REFERENCE_FIELD5"),
         AirlineOwnUse = r.TryGetField<string>("AIRLINE_OWN_USE"),

         RejectionMemoId = r.TryGetField<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(r.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
         LastUpdatedBy = r.TryGetField<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
         LastUpdatedOn = r.TryGetField<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),

         ProrateSlipDetails = r.TryGetField<string>("PRORATE_SLIP_DETAIL"),
         SerialNo = r.TryGetField<object>("RM_BD_SR_NO") != null ? r.Field<int>("RM_BD_SR_NO") : 0,
       });

    public readonly Materializer<RMCouponVat> RMCouponVatAuditMaterializer = new Materializer<RMCouponVat>(r =>
    new RMCouponVat
    {
      LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = r.Field<byte[]>("RM_COUPON_BREAKDOWN_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_BREAKDOWN_ID")) : new Guid(),
      VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
      VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
      VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
      VatText = r.Field<string>("VAT_TEXT"),
      VatLabel = r.Field<string>("VAT_LABEL"),
      VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
      Id = r.Field<byte[]>("RM_COUPON_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_VAT_BD_ID")) : new Guid(),
    }
    );

    public readonly Materializer<RMCouponAttachment> RMCouponAttachmentAuditMaterializer = new Materializer<RMCouponAttachment>(rmca =>
    new RMCouponAttachment
    {
      ServerId = rmca.Field<object>("SERVER_ID") != null ? rmca.Field<int>("SERVER_ID") : 0,
      FilePath = rmca.Field<string>("FILE_PATH"),
      OriginalFileName = rmca.Field<string>("ORG_FILE_NAME"),
      LastUpdatedOn = rmca.Field<object>("LAST_UPDATED_ON") != null ? rmca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = rmca.Field<object>("LAST_UPDATED_BY") != null ? rmca.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = rmca.Field<byte[]>("RM_COUPON_BREAKDOWN_ID") != null ? new Guid(rmca.Field<byte[]>("RM_COUPON_BREAKDOWN_ID")) : new Guid(),
      FileStatusId = rmca.Field<object>("FILE_STATUS_ID") != null ? rmca.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = rmca.Field<object>("FILE_TYPE_ID") != null ? rmca.Field<int>("FILE_TYPE_ID") : 0,
      FileSize = rmca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(rmca.Field<object>("FILE_SIZE")) : 0,
      IsFullPath = rmca.Field<object>("IS_FULL_PATH") != null ? (rmca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      Id = rmca.Field<byte[]>("RM_COUPON_ATTACHMENTS_ID") != null ? new Guid(rmca.Field<byte[]>("RM_COUPON_ATTACHMENTS_ID")) : new Guid(),
    });

    public readonly Materializer<RMCouponTax> RMCouponTaxAuditMaterializer = new Materializer<RMCouponTax>(r =>
 new RMCouponTax
 {
   LastUpdatedOn = r.TryGetField<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
   LastUpdatedBy = r.TryGetField<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
   ParentId = r.TryGetField<byte[]>("RM_COUPON_BREAKDOWN_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_BREAKDOWN_ID")) : new Guid(),
   AmountDifference = r.TryGetField<object>("TAX_AMT_DIFF") != null ? r.Field<double>("TAX_AMT_DIFF") : 0,
   AmountAccepted = r.TryGetField<object>("TAX_AMT_ACCEPTED") != null ? r.Field<double>("TAX_AMT_ACCEPTED") : 0,
   Amount = r.TryGetField<object>("TAX_AMT_BILLED") != null ? r.Field<double>("TAX_AMT_BILLED") : 0,
   TaxCode = r.TryGetField<string>("TAX_CODE"),
   Id = r.TryGetField<byte[]>("RM_COUPON_TAX_BD_ID") != null ? new Guid(r.Field<byte[]>("RM_COUPON_TAX_BD_ID")) : new Guid(),
 });

    public readonly Materializer<BMCoupon> BMCouponAuditMaterializer = new Materializer<BMCoupon>(r =>
     new BMCoupon
     {
       Id = r.Field<byte[]>("BM_COUPON_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_COUPON_BD_ID")) : new Guid(),
       SerialNo = r.Field<object>("BM_BD_SR_NO") != null ? r.Field<int>("BM_BD_SR_NO") : 0,
       TicketOrFimIssuingAirline = r.Field<string>("TICKET_ISSUING_AIRLINE"),
       TicketOrFimCouponNumber = r.Field<object>("COUPON_NO") != null ? r.Field<int>("COUPON_NO") : 0,
       TicketDocOrFimNumber = r.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOC_NO")) : 0,
       CheckDigit = r.Field<object>("CHECK_DIGIT") != null ? r.Field<int>("CHECK_DIGIT") : 0, //test
       ToAirportOfCoupon = r.Field<string>("TO_AIRPORT_OF_COUPON"),
       GrossAmountBilled = r.Field<object>("GROSS_AMT") != null ? r.Field<decimal>("GROSS_AMT") : 0,
       TaxAmount = r.Field<object>("TAX_AMT") != null ? r.Field<double>("TAX_AMT") : 0.0,
       IscPercent = r.Field<object>("ALLOWED_ISC_PER") != null ? r.Field<double>("ALLOWED_ISC_PER") : 0.0,
       IscAmountBilled = r.Field<object>("ISC_AMT") != null ? r.Field<double>("ISC_AMT") : 0.0,
       OtherCommissionPercent = r.Field<object>("ALLOWED_OTH_COMM_PER") != null ? r.Field<double>("ALLOWED_OTH_COMM_PER") : 0.0,
       OtherCommissionBilled = r.Field<object>("OTH_COMM_AMT") != null ? r.Field<double>("OTH_COMM_AMT") : 0.0,
       HandlingFeeAmount = r.Field<object>("HANDLING_FEE_AMT") != null ? r.Field<double>("HANDLING_FEE_AMT") : 0.0,
       UatpPercent = r.Field<object>("ALLOWED_UATP_PER") != null ? r.Field<double>("ALLOWED_UATP_PER") : 0.0,
       UatpAmountBilled = r.Field<object>("UATP_AMT") != null ? r.Field<double>("UATP_AMT") : 0.0,
       VatAmount = r.Field<object>("VAT_AMT") != null ? r.Field<double>("VAT_AMT") : 0.0,
       NetAmountBilled = r.Field<object>("NET_BILLED_AMT") != null ? r.Field<double>("NET_BILLED_AMT") : 0.0,
       NfpReasonCode = r.Field<string>("NFP_REASON_CODE"),
       AgreementIndicatorSupplied = r.Field<string>("AGREEMENT_IND_SUPPLIED"),
       AgreementIndicatorValidated = r.Field<string>("AGREEMENT_IND_VALIDATED"),
       OriginalPmi = r.Field<string>("ORIGINAL_PMI"),
       ValidatedPmi = r.Field<string>("VALIDATED_PMI"),
       SettlementAuthorizationCode = r.Field<string>("SETTLEMENT_AUTH_CODE"),
       AttachmentIndicatorOriginal = r.Field<object>("ATTCHMNT_IND_ORIG") != null ? (r.Field<int>("ATTCHMNT_IND_ORIG") == 0 ? 0 : (r.Field<int>("ATTCHMNT_IND_ORIG") == 1 ? 1 : 2)) : 0,
       AttachmentIndicatorValidated = r.Field<object>("ATTCHMNT_IND_VALIDATED") != null ? (r.Field<int>("ATTCHMNT_IND_VALIDATED") == 0 ? false : true) : false,
       NumberOfAttachments = r.Field<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int>("NO_OF_ATTACHMENTS") : 0,
       ISValidationFlag = r.Field<string>("IS_VALIDATION_FLAG"),
       ElectronicTicketIndicator = r.Field<object>("ETICKET_IND") != null ? (r.Field<int>("ETICKET_IND") == 0 ? false : true) : false,
       AirlineFlightDesignator = r.Field<string>("AIRLINE_FLIGHT_DESIGNATOR"),
       FlightNumber = r.Field<object>("FLIGHT_NO") != null ? r.Field<int?>("FLIGHT_NO") : null,
       FlightDate = r.Field<object>("FLIGHT_DATE") != null ? r.Field<DateTime>("FLIGHT_DATE") : new DateTime(),
       CabinClass = r.Field<string>("CABIN_CLASS"),
       ProrateMethodology = r.Field<string>("PRORATE_METHODOLOGY"),
       BillingMemoId = r.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(r.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
       LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
       LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
       ProrateSlipDetails = r.Field<string>("PRORATE_SLIP_DETAIL"),
       CurrencyAdjustmentIndicator = r.Field<string>("CURRENCY_ADJSMNT_IND"),
       ReferenceField1 = r.Field<string>("REFERENCE_FIELD1"),
       ReferenceField2 = r.Field<string>("REFERENCE_FIELD2"),
       ReferenceField3 = r.Field<string>("REFERENCE_FIELD3"),
       ReferenceField4 = r.Field<string>("REFERENCE_FIELD4"),
       ReferenceField5 = r.Field<string>("REFERENCE_FIELD5"),
       AirlineOwnUse = r.Field<string>("AIRLINE_OWN_USE"),
       FromAirportOfCoupon = r.Field<string>("FROM_AIRPORT_OF_COUPON"),
       ReasonCode = r.Field<string>("REASON_CODE"),
     });

    public readonly Materializer<BMCouponAttachment> BMCouponAttachmentAuditMaterializer = new Materializer<BMCouponAttachment>(bmca =>
new BMCouponAttachment
{
  // TODO: uncomment FileSize
  FileStatusId = bmca.Field<object>("FILE_STATUS_ID") != null ? bmca.Field<int>("FILE_STATUS_ID") : 0,
  FileTypeId = bmca.Field<object>("FILE_TYPE_ID") != null ? bmca.Field<int>("FILE_TYPE_ID") : 0,
  FilePath = bmca.Field<string>("FILE_PATH"),
  ServerId = bmca.Field<object>("SERVER_ID") != null ? bmca.Field<int>("SERVER_ID") : 0,
  LastUpdatedOn = bmca.Field<object>("LAST_UPDATED_ON") != null ? bmca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
  LastUpdatedBy = bmca.Field<object>("LAST_UPDATED_BY") != null ? bmca.Field<int>("LAST_UPDATED_BY") : 0,
  FileSize = bmca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(bmca.Field<object>("FILE_SIZE")) : 0,
  OriginalFileName = bmca.Field<string>("ORG_FILE_NAME"),
  ParentId = bmca.Field<byte[]>("BM_COUPON_BD_ID") != null ? new Guid(bmca.Field<byte[]>("BM_COUPON_BD_ID")) : new Guid(),
  IsFullPath = bmca.Field<object>("IS_FULL_PATH") != null ? (bmca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
  Id = bmca.Field<byte[]>("BM_COUPON_ATTACHMENTS_ID") != null ? new Guid(bmca.Field<byte[]>("BM_COUPON_ATTACHMENTS_ID")) : new Guid(),
});

    public readonly Materializer<BMCouponTax> BMCouponTaxAuditMaterializer = new Materializer<BMCouponTax>(r =>
new BMCouponTax
{
  LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
  LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
  ParentId = r.Field<byte[]>("BM_COUPON_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_COUPON_BD_ID")) : new Guid(),
  Amount = r.Field<object>("TAX_AMT_BILLED") != null ? r.Field<double>("TAX_AMT_BILLED") : 0,
  TaxCode = r.Field<string>("TAX_CODE"),
  Id = r.Field<byte[]>("BM_COUPON_TAX_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_COUPON_TAX_BD_ID")) : new Guid(),
});


    public readonly Materializer<BillingMemoAttachment> BillingMemoAttachmentAuditMaterializer = new Materializer<BillingMemoAttachment>(bma =>
    new BillingMemoAttachment
    {
      ServerId = bma.Field<object>("SERVER_ID") != null ? bma.Field<int>("SERVER_ID") : 0,
      FilePath = bma.Field<string>("FILE_PATH"),
      OriginalFileName = bma.Field<string>("ORG_FILE_NAME"),
      LastUpdatedOn = bma.Field<object>("LAST_UPDATED_ON") != null ? bma.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = bma.Field<object>("LAST_UPDATED_BY") != null ? bma.Field<int>("LAST_UPDATED_BY") : 0,
      ParentId = bma.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(bma.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
      FileStatusId = bma.Field<object>("FILE_STATUS_ID") != null ? bma.Field<int>("FILE_STATUS_ID") : 0,
      FileTypeId = bma.Field<object>("FILE_TYPE_ID") != null ? bma.Field<int>("FILE_TYPE_ID") : 0,
      FileSize = bma.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(bma.Field<object>("FILE_SIZE")) : 0,
      IsFullPath = bma.Field<object>("IS_FULL_PATH") != null ? (bma.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
      Id = bma.Field<byte[]>("BILLING_MEMO_ATTACHMENT_ID") != null ? new Guid(bma.Field<byte[]>("BILLING_MEMO_ATTACHMENT_ID")) : new Guid(),
    });


    public readonly Materializer<BillingMemo> PaxInvoiceBillingMemoAuditMaterializer = new Materializer<BillingMemo>(bm =>
    new BillingMemo
    {
      Id = bm.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(bm.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
      BatchSequenceNumber = bm.Field<object>("BATCH_SEQ_NO") != null ? bm.Field<int>("BATCH_SEQ_NO") : 0,
      RecordSequenceWithinBatch = bm.Field<object>("BATCH_REC_SEQ_NO") != null ? bm.Field<int>("BATCH_REC_SEQ_NO") : 0,
      SourceCodeId = bm.Field<object>("SOURCE_CODE") != null ? bm.Field<int>("SOURCE_CODE") : 0,
      BillingMemoNumber = bm.Field<string>("BILLING_MEMO_NO"),
      ReasonCode = bm.Field<string>("REASON_CODE"),
      OurRef = bm.Field<object>("INTERNAL_REF") != null ? bm.Field<string>("INTERNAL_REF") : null,
      CorrespondenceRefNumber = bm.Field<object>("CORRESPONDENCE_REF_NO") != null ? Convert.ToInt64(bm.Field<object>("CORRESPONDENCE_REF_NO")) : 0,
      FimNumber = bm.Field<object>("FIM_NO") != null ? (long?)Convert.ToInt64(bm.Field<object>("FIM_NO")) : null,
      FimCouponNumber = bm.Field<object>("FIM_COUPON_NO") != null ? bm.Field<int?>("FIM_COUPON_NO") : null,
      YourInvoiceNumber = bm.Field<object>("YOUR_INVOICE_NO") != null ? bm.Field<string>("YOUR_INVOICE_NO") : null,
      YourInvoiceBillingYear = bm.Field<object>("YOUR_INVOICE_BILLING_YEAR") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_YEAR") : 0,
      TotalGrossAmountBilled = bm.Field<object>("TOTAL_GROSS_AMT") != null ? bm.Field<decimal>("TOTAL_GROSS_AMT") : 0,
      TaxAmountBilled = bm.Field<object>("TOTAL_TAX_AMT") != null ? bm.Field<decimal>("TOTAL_TAX_AMT") : 0,
      TotalIscAmountBilled = bm.Field<object>("TOTAL_ISC_AMT") != null ? bm.Field<decimal>("TOTAL_ISC_AMT") : 0,
      TotalOtherCommissionAmount = bm.Field<object>("TOTAL_OTH_COMM_AMT") != null ? bm.Field<decimal>("TOTAL_OTH_COMM_AMT") : 0,
      TotalHandlingFeeBilled = bm.Field<object>("TOTAL_HANDLING_FEE_AMT") != null ? bm.Field<double>("TOTAL_HANDLING_FEE_AMT") : 0.0,
      TotalUatpAmountBilled = bm.Field<object>("TOTAL_UATP_AMT") != null ? bm.Field<decimal>("TOTAL_UATP_AMT") : 0,
      TotalVatAmountBilled = bm.Field<object>("TOTAL_VAT_AMT") != null ? bm.Field<decimal>("TOTAL_VAT_AMT") : 0,
      NetAmountBilled = bm.Field<object>("NET_AMT") != null ? bm.Field<decimal>("NET_AMT") : 0,
      AttachmentIndicatorOriginal = bm.Field<object>("ATTCHMNT_IND_ORIG") != null ? bm.Field<int>("ATTCHMNT_IND_ORIG") : 0,
      AttachmentIndicatorValidated = bm.Field<object>("ATTCHMNT_IND_VALIDATED") != null && bm.Field<int>("ATTCHMNT_IND_VALIDATED") > 0,
      NumberOfAttachments = bm.Field<object>("NO_OF_ATTACHMENTS") != null ? bm.Field<int>("NO_OF_ATTACHMENTS") : 0,
      AirlineOwnUse = bm.Field<object>("AIRLINE_OWN_USE") != null ? bm.Field<string>("AIRLINE_OWN_USE") : null,
      ISValidationFlag = bm.Field<object>("IS_VALIDATION_FLAG") != null ? bm.Field<string>("IS_VALIDATION_FLAG") : null,
      InvoiceId = bm.Field<byte[]>("INVOICE_ID") != null ? new Guid(bm.Field<byte[]>("INVOICE_ID")) : new Guid(),
      ReasonRemarks = bm.Field<object>("REASON_REMARKS") != null ? bm.Field<string>("REASON_REMARKS") : null,
      YourInvoiceBillingMonth = bm.Field<object>("YOUR_INVOICE_BILLING_MONTH") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_MONTH") : 0,
      YourInvoiceBillingPeriod = bm.Field<object>("YOUR_INVOICE_BILLING_PERIOD") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_PERIOD") : 0,
      LastUpdatedBy = bm.Field<object>("LAST_UPDATED_BY") != null ? bm.Field<int>("LAST_UPDATED_BY") : 0,
      LastUpdatedOn = bm.Field<object>("LAST_UPDATED_ON") != null ? bm.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
    });

    public readonly Materializer<BillingMemoVat> BillingMemoVatAuditMaterializer = new Materializer<BillingMemoVat>(r =>
 new BillingMemoVat
 {
   LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
   LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
   ParentId = r.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(r.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
   VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
   VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
   VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
   VatText = r.Field<string>("VAT_TEXT"),
   VatLabel = r.Field<string>("VAT_LABEL"),
   VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
   Id = r.Field<byte[]>("BM_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_VAT_BD_ID")) : new Guid(),
 }
 );

    public readonly Materializer<BMCouponVat> BMCouponVatAuditMaterializer = new Materializer<BMCouponVat>(r =>
new BMCouponVat
{
  LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
  LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
  ParentId = r.Field<byte[]>("BM_COUPON_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_COUPON_BD_ID")) : new Guid(),
  VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
  VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
  VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
  VatText = r.Field<string>("VAT_TEXT"),
  VatLabel = r.Field<string>("VAT_LABEL"),
  VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
  Id = r.Field<byte[]>("BM_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("BM_VAT_BD_ID")) : new Guid(),
}
);

    public readonly Materializer<Correspondence> PaxInvoiceCorrespondenceAuditMaterializer = new Materializer<Correspondence>(pCorr =>
    new Correspondence
    {
      Id = pCorr.Field<byte[]>("CORRESPONDENCE_ID") != null ? new Guid(pCorr.Field<byte[]>("CORRESPONDENCE_ID")) : new Guid(),
      InvoiceId = pCorr.Field<byte[]>("INVOICE_ID") != null ? new Guid(pCorr.Field<byte[]>("INVOICE_ID")) : new Guid(),
      CorrespondenceOwnerId = pCorr.Field<object>("CORRESPONDENCE_OWNER_ID") != null ? pCorr.Field<int>("CORRESPONDENCE_OWNER_ID") : 0,
      CorrespondenceNumber = pCorr.Field<object>("CORRESPONDENCE_NO") != null ? (long?)Convert.ToInt64(pCorr.Field<object>("CORRESPONDENCE_NO")) : null,
      CorrespondenceDate = pCorr.Field<object>("CORRESPONDENCE_DATE") != null ? pCorr.Field<DateTime>("CORRESPONDENCE_DATE") : new DateTime(),
      CorrespondenceStage = pCorr.Field<object>("CORRESPONDENCE_STAGE") != null ? pCorr.Field<int>("CORRESPONDENCE_STAGE") : 0,
      FromMemberId = pCorr.Field<object>("FROM_MEMBER_ID") != null ? pCorr.Field<int>("FROM_MEMBER_ID") : 0,
      ToMemberId = pCorr.Field<object>("TO_MEMBER_ID") != null ? pCorr.Field<int>("TO_MEMBER_ID") : 0,
      /* CMP#657: Retention of Additional Email Addresses in Correspondences
       * Adding code to get email ids from initiator and non-initiator and removing
       * additional email field*/
      AdditionalEmailInitiator = pCorr.Field<string>("ADDITIONAL_EMAIL_INITIATOR"),
      AdditionalEmailNonInitiator = pCorr.Field<string>("ADDITIONAL_EMAIL_NON_INITIATOR"),
      ToEmailId = pCorr.Field<string>("TO_EMAILID"),
      AmountToBeSettled = pCorr.Field<object>("AMOUNT_TO_SETTLED") != null ? pCorr.Field<decimal>("AMOUNT_TO_SETTLED") : 0,
      OurReference = pCorr.Field<string>("OUR_REFERENCE"),
      YourReference = pCorr.Field<string>("YOUR_REFERENCE"),
      Subject = pCorr.Field<string>("SUBJECT"),
      CorrespondenceDetails = pCorr.Field<string>("CORRESPONDENCE_DETAILS"),
      CorrespondenceStatusId = pCorr.Field<object>("CORRESPONDENCE_STATUS") != null ? pCorr.Field<int>("CORRESPONDENCE_STATUS") : 0,
      CorrespondenceSubStatusId = pCorr.Field<object>("CORRESPONDENCE_SUB_STATUS") != null ? pCorr.Field<int>("CORRESPONDENCE_SUB_STATUS") : 0,
      AuthorityToBill = pCorr.Field<object>("AUTHORITY_TO_BILL") != null && pCorr.Field<int>("AUTHORITY_TO_BILL") > 0,
      NumberOfDaysToExpire = pCorr.Field<object>("NO_OF_DAYS_TO_EXPIRE") != null ? pCorr.Field<int>("NO_OF_DAYS_TO_EXPIRE") : 0,
      FromEmailId = pCorr.Field<string>("FROM_EMAILID"),
      CurrencyId = pCorr.Field<object>("CURRENCY_CODE_NUM") != null ? pCorr.Field<int?>("CURRENCY_CODE_NUM") : null,
      //CMP526 - Passenger Correspondence Identifiable by Source Code
      SourceCode = pCorr.Field<object>("SOURCE_CODE") != null ? pCorr.Field<int?>("SOURCE_CODE") : 0,
      //CMP527 - Start
      AcceptanceComment = pCorr.Field<object>("ACCEPTANCE_COMMENTS") != null ? pCorr.Field<string>("ACCEPTANCE_COMMENTS") : null,
      AcceptanceUserName = pCorr.Field<object>("ACCEPTANCE_USER") != null ? pCorr.Field<string>("ACCEPTANCE_USER") : null,
      AcceptanceDateTime = pCorr.Field<object>("ACCEPTANCE_DATE") != null ? pCorr.Field<DateTime>("ACCEPTANCE_DATE") : new DateTime(),
      //CMP527 - End
    });


    public readonly Materializer<ProvisionalInvoiceRecordDetail> ProvisionalInvoiceMaterializer = new Materializer<ProvisionalInvoiceRecordDetail>(r =>
       new ProvisionalInvoiceRecordDetail
       {
         Id = r.TryGetField<byte[]>("FORM_E_PROV_INVOICE_ID") != null ? new Guid(r.Field<byte[]>("FORM_E_PROV_INVOICE_ID")) : new Guid(),
         InvoiceNumber = r.Field<string>("PROV_INVOICE_NO"),
         InvoiceDate = r.Field<object>("PROV_INVOICE_DATE") != null ? r.Field<DateTime>("PROV_INVOICE_DATE") : new DateTime(),
         BillingPeriodNo = r.Field<object>("PROV_BILLING_PERIOD_NO") != null ? r.Field<int>("PROV_BILLING_PERIOD_NO") : 0,
         InvoiceListingAmount = r.Field<object>("PROV_INVOICE_LISTING_AMT") != null ? r.Field<decimal>("PROV_INVOICE_LISTING_AMT") : 0,
         ListingToBillingRate = r.Field<object>("PROV_LISTING_TO_BILLING_RATE") != null ? r.Field<decimal>("PROV_LISTING_TO_BILLING_RATE") : 0,
         InvoiceListingCurrencyId = r.Field<object>("PRO_INV_LST_CURRENCY_CODE_NUM") != null ? r.Field<int>("PRO_INV_LST_CURRENCY_CODE_NUM") : 0,
         InvoiceId = r.TryGetField<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
         InvoiceBillingAmount = r.Field<object>("PROV_INV_AMT_BILLING_CURRENCY") != null ? r.Field<decimal>("PROV_INV_AMT_BILLING_CURRENCY") : 0,
         LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
         LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
       });

    public readonly Materializer<SamplingFormEDetailVat> SamplingFormEDetailVatMaterializer = new Materializer<SamplingFormEDetailVat>(ev =>
      new SamplingFormEDetailVat
      {
        LastUpdatedOn = ev.Field<object>("LAST_UPDATED_ON") != null ? ev.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = ev.Field<object>("LAST_UPDATED_BY") != null ? ev.Field<int>("LAST_UPDATED_BY") : 0,
        ParentId = ev.Field<byte[]>("INVOICE_ID") != null ? new Guid(ev.Field<byte[]>("INVOICE_ID")) : new Guid(),
        VatCalculatedAmount = ev.Field<object>("VAT_CALC_AMT") != null ? ev.Field<double>("VAT_CALC_AMT") : 0.0,
        VatPercentage = ev.Field<object>("VAT_PER") != null ? ev.Field<double>("VAT_PER") : 0.0,
        VatBaseAmount = ev.Field<object>("VAT_BASE_AMT") != null ? ev.Field<double>("VAT_BASE_AMT") : 0.0,
        VatText = ev.Field<string>("VAT_TEXT"),
        VatLabel = ev.Field<string>("VAT_LABEL"),
        VatIdentifierId = ev.Field<object>("VAT_IDENTIFIER_ID") != null ? ev.Field<int>("VAT_IDENTIFIER_ID") : 0,
        Id = ev.Field<byte[]>("FORM_E_VAT_BD_ID") != null ? new Guid(ev.Field<byte[]>("FORM_E_VAT_BD_ID")) : new Guid(),
      });


    public readonly Materializer<CreditMemo> CreditMemoMaterializer = new Materializer<CreditMemo>(cm =>
  new CreditMemo
  {
    Id = cm.Field<byte[]>("CREDIT_MEMO_ID") != null ? new Guid(cm.Field<byte[]>("CREDIT_MEMO_ID")) : new Guid(),
    BatchSequenceNumber = cm.Field<object>("BATCH_SEQ_NO") != null ? cm.Field<int>("BATCH_SEQ_NO") : 0,
    RecordSequenceWithinBatch = cm.Field<object>("BATCH_REC_SEQ_NO") != null ? cm.Field<int>("BATCH_REC_SEQ_NO") : 0,
    SourceCodeId = cm.Field<object>("SOURCE_CODE") != null ? cm.Field<int>("SOURCE_CODE") : 0,
    CreditMemoNumber = cm.Field<string>("CREDIT_MEMO_NO"),
    ReasonCode = cm.Field<string>("REASON_CODE"),
    OurRef = cm.Field<object>("INTERNAL_REF") != null ? cm.Field<string>("INTERNAL_REF") : null,
    CorrespondenceRefNumber = cm.Field<object>("CORRESPONDENCE_REF_NO") != null ? Convert.ToInt64(cm.Field<object>("CORRESPONDENCE_REF_NO")) : 0,
    FimNumber = cm.Field<object>("FIM_NO") != null ? (long?)Convert.ToInt64(cm.Field<object>("FIM_NO")) : null,
    FimCouponNumber = cm.Field<object>("FIM_COUPON_NO") != null ? cm.Field<int?>("FIM_COUPON_NO") : null,
    YourInvoiceNumber = cm.Field<object>("YOUR_INVOICE_NO") != null ? cm.Field<string>("YOUR_INVOICE_NO") : null,
    YourInvoiceBillingYear = cm.Field<object>("YOUR_INVOICE_BILLING_YEAR") != null ? cm.Field<int>("YOUR_INVOICE_BILLING_YEAR") : 0,
    TotalGrossAmountCredited = cm.Field<object>("TOTAL_GROSS_AMT") != null ? cm.Field<decimal>("TOTAL_GROSS_AMT") : 0,
    TaxAmount = cm.Field<object>("TOTAL_TAX_AMOUNT") != null ? cm.Field<decimal>("TOTAL_TAX_AMOUNT") : 0,
    TotalIscAmountCredited = cm.Field<object>("TOTAL_ISC_AMT") != null ? cm.Field<decimal>("TOTAL_ISC_AMT") : 0,
    TotalOtherCommissionAmountCredited = cm.Field<object>("TOTAL_OTH_COMM_AMT") != null ? cm.Field<decimal>("TOTAL_OTH_COMM_AMT") : 0,
    TotalHandlingFeeCredited = cm.Field<object>("TOTAL_HANDLING_FEE_AMT") != null ? cm.Field<double>("TOTAL_HANDLING_FEE_AMT") : 0.0,
    TotalUatpAmountCredited = cm.Field<object>("TOTAL_UATP_AMT") != null ? cm.Field<decimal>("TOTAL_UATP_AMT") : 0,
    VatAmount = cm.Field<object>("TOTAL_VAT_AMT") != null ? cm.Field<decimal>("TOTAL_VAT_AMT") : 0,
    NetAmountCredited = cm.Field<object>("NET_AMT") != null ? cm.Field<decimal>("NET_AMT") : 0,
    AttachmentIndicatorOriginal = cm.Field<object>("ATTCHMNT_IND_ORIG") != null ? (cm.Field<int>("ATTCHMNT_IND_ORIG") == 0 ? 0 : (cm.Field<int>("ATTCHMNT_IND_ORIG") == 1 ? 1 : 2)) : 0,
    AttachmentIndicatorValidated = cm.Field<object>("ATTCHMNT_IND_VALIDATED") != null ? (cm.Field<int>("ATTCHMNT_IND_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
    NumberOfAttachments = cm.Field<object>("NO_OF_ATTACHMENTS") != null ? cm.Field<int?>("NO_OF_ATTACHMENTS") : null,
    AirlineOwnUse = cm.Field<object>("AIRLINE_OWN_USE") != null ? cm.Field<string>("AIRLINE_OWN_USE") : null,
    ISValidationFlag = cm.Field<object>("IS_VALIDATION_FLAG") != null ? cm.Field<string>("IS_VALIDATION_FLAG") : null,
    InvoiceId = cm.Field<byte[]>("INVOICE_ID") != null ? new Guid(cm.Field<byte[]>("INVOICE_ID")) : new Guid(),
    ReasonRemarks = cm.Field<object>("REASON_REMARKS") != null ? cm.Field<string>("REASON_REMARKS") : null,
    YourInvoiceBillingMonth = cm.Field<object>("YOUR_INVOICE_BILLING_MONTH") != null ? cm.Field<int>("YOUR_INVOICE_BILLING_MONTH") : 0,
    YourInvoiceBillingPeriod = cm.Field<object>("YOUR_INVOICE_BILLING_PERIOD") != null ? cm.Field<int>("YOUR_INVOICE_BILLING_PERIOD") : 0,
    LastUpdatedBy = cm.Field<object>("LAST_UPDATED_BY") != null ? cm.Field<int>("LAST_UPDATED_BY") : 0,
    LastUpdatedOn = cm.Field<object>("LAST_UPDATED_ON") != null ? cm.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
  });

    public readonly Materializer<CreditMemoVat> CreditMemoVatMaterializer = new Materializer<CreditMemoVat>(cmvat =>
      new CreditMemoVat
      {

        Id = cmvat.Field<byte[]>("CM_VAT_BD_ID") != null ? new Guid(cmvat.Field<byte[]>("CM_VAT_BD_ID")) : new Guid(),
        ParentId = cmvat.Field<byte[]>("CREDIT_MEMO_ID") != null ? new Guid(cmvat.Field<byte[]>("CREDIT_MEMO_ID")) : new Guid(),
        VatIdentifierId = cmvat.Field<object>("VAT_IDENTIFIER_ID") != null ? cmvat.Field<int>("VAT_IDENTIFIER_ID") : 0,
        VatLabel = cmvat.Field<string>("VAT_LABEL"),
        VatText = cmvat.Field<string>("VAT_TEXT"),
        VatBaseAmount = cmvat.Field<object>("VAT_BASE_AMT") != null ? cmvat.Field<double>("VAT_BASE_AMT") : 0,
        VatPercentage = cmvat.Field<object>("VAT_PER") != null ? cmvat.Field<double>("VAT_PER") : 0,
        VatCalculatedAmount = cmvat.Field<object>("VAT_CALC_AMT") != null ? cmvat.Field<double>("VAT_CALC_AMT") : 0,
        LastUpdatedBy = cmvat.Field<object>("LAST_UPDATED_BY") != null ? cmvat.Field<int>("LAST_UPDATED_BY") : 0,
        LastUpdatedOn = cmvat.Field<object>("LAST_UPDATED_ON") != null ? cmvat.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      });

    public readonly Materializer<CMCoupon> CMCouponMaterializer = new Materializer<CMCoupon>(r =>
    new CMCoupon
    {
      //TODO : Check TicketDocOrFimNumber object 

      Id = r.Field<byte[]>("CM_COUPON_BD_ID") != null ? new Guid(r.Field<byte[]>("CM_COUPON_BD_ID")) : new Guid(),
      SerialNo = r.Field<object>("CM_BD_SR_NO") != null ? r.Field<int>("CM_BD_SR_NO") : 0,
      TicketOrFimIssuingAirline = r.Field<string>("TICKET_ISSUING_AIRLINE"),
      TicketOrFimCouponNumber = r.Field<object>("COUPON_NO") != null ? r.Field<int>("COUPON_NO") : 0,
      TicketDocOrFimNumber = r.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOC_NO")) : 0,
      CheckDigit = r.Field<object>("CHECK_DIGIT") != null ? r.Field<int>("CHECK_DIGIT") : 0, //test
      FromAirportOfCoupon = r.Field<string>("FROM_AIRPORT_OF_COUPON"),
      ToAirportOfCoupon = r.Field<string>("TO_AIRPORT_OF_COUPON"),
      GrossAmountCredited = r.Field<object>("GROSS_AMT") != null ? r.Field<decimal>("GROSS_AMT") : 0,
      TaxAmount = r.Field<object>("TAX_AMT") != null ? r.Field<double>("TAX_AMT") : 0.0,
      IscPercent = r.Field<object>("ALLOWED_ISC_PER") != null ? r.Field<double>("ALLOWED_ISC_PER") : 0.0,
      IscAmountBilled = r.Field<object>("ISC_AMT") != null ? r.Field<double>("ISC_AMT") : 0.0,
      OtherCommissionPercent = r.Field<object>("ALLOWED_OTH_COMM_PER") != null ? r.Field<double>("ALLOWED_OTH_COMM_PER") : 0.0,
      OtherCommissionBilled = r.Field<object>("OTH_COMM_AMT") != null ? r.Field<double>("OTH_COMM_AMT") : 0.0,
      HandlingFeeAmount = r.Field<object>("HANDLING_FEE_AMT") != null ? r.Field<double>("HANDLING_FEE_AMT") : 0.0,
      UatpPercent = r.Field<object>("ALLOWED_UATP_PER") != null ? r.Field<double>("ALLOWED_UATP_PER") : 0.0,
      UatpAmountBilled = r.Field<object>("UATP_AMT") != null ? r.Field<double>("UATP_AMT") : 0.0,
      VatAmount = r.Field<object>("VAT_AMT") != null ? r.Field<double>("VAT_AMT") : 0.0,
      NetAmountCredited = r.Field<object>("NET_AMT") != null ? r.Field<double>("NET_AMT") : 0.0,
      NfpReasonCode = r.Field<string>("NFP_REASON_CODE"),
      AgreementIndicatorSupplied = r.Field<string>("AGREEMENT_IND_SUPPLIED"),
      AgreementIndicatorValidated = r.Field<string>("AGREEMENT_IND_VALIDATED"),//ok
      OriginalPmi = r.Field<string>("ORIGINAL_PMI"),
      ValidatedPmi = r.Field<string>("VALIDATED_PMI"),
      SettlementAuthorizationCode = r.Field<string>("SETTLEMENT_AUTH_CODE"),
      AttachmentIndicatorOriginal = r.Field<object>("ATTACHMENT_IND_ORIG") != null ? (r.Field<int>("ATTACHMENT_IND_ORIG") == 0 ? 0 : (r.Field<int>("ATTACHMENT_IND_ORIG") == 1 ? 1 : 2)) : 0,
      AttachmentIndicatorValidated = r.Field<object>("ATTACHMENT_IND_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_IND_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
      NumberOfAttachments = r.Field<object>("NO_OF_ATTACHMENTS") != null ? r.Field<int?>("NO_OF_ATTACHMENTS") : null,
      ISValidationFlag = r.Field<string>("IS_VALIDATION_FLAG"),
      CurrencyAdjustmentIndicator = r.Field<string>("CURRENCY_ADJSTMNT_IND"),
      ElectronicTicketIndicator = r.Field<object>("ETICKET_IND") != null && r.Field<int>("ETICKET_IND") > 0,
      AirlineFlightDesignator = r.Field<string>("AIRLINE_FLIGHT_DESIGNATOR"),
      FlightNumber = r.Field<object>("FLIGHT_NO") != null ? r.Field<int?>("FLIGHT_NO") : null,
      CabinClass = r.Field<string>("CABIN_CLASS"),//ok 2
      ProrateMethodology = r.Field<string>("PRORATE_METHODOLOGY"),
      ReasonCode = r.Field<string>("REASON_CODE"),
      ReferenceField1 = r.Field<string>("REFERENCE_FIELD1"),
      ReferenceField2 = r.Field<string>("REFERENCE_FIELD2"),
      ReferenceField3 = r.Field<string>("REFERENCE_FIELD3"),
      ReferenceField4 = r.Field<string>("REFERENCE_FIELD4"),
      ReferenceField5 = r.Field<string>("REFERENCE_FIELD5"),
      AirlineOwnUse = r.Field<string>("AIRLINE_OWN_USE"),
      CreditMemoId = r.Field<byte[]>("CREDIT_MEMO_ID") != null ? new Guid(r.Field<byte[]>("CREDIT_MEMO_ID")) : new Guid(),
      LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      ProrateSlipDetails = r.Field<string>("PRORATE_SLIP_DETAIL"),
      FlightDate = (DateTime?)(r.Field<object>("FLIGHT_DATE")), //Changes to set FlightDate as null if value is not selected
    });

    public readonly Materializer<CMCouponVat> CMCouponVatMaterializer = new Materializer<CMCouponVat>(r =>
    new CMCouponVat
    {
      Id = r.Field<byte[]>("CM_VAT_BD_ID") != null ? new Guid(r.Field<byte[]>("CM_VAT_BD_ID")) : new Guid(),
      VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
      VatLabel = r.Field<string>("VAT_LABEL"),
      VatText = r.Field<string>("VAT_TEXT"),
      VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
      VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
      VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
      ParentId = r.Field<byte[]>("CM_COUPON_BD_ID") != null ? new Guid(r.Field<byte[]>("CM_COUPON_BD_ID")) : new Guid(),
      LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
    });

    public readonly Materializer<CMCouponTax> CMCouponTaxMaterializer = new Materializer<CMCouponTax>(r =>
      new CMCouponTax
      {
        Id = r.Field<byte[]>("CM_COUPON_TAX_BD_ID") != null ? new Guid(r.Field<byte[]>("CM_COUPON_TAX_BD_ID")) : new Guid(),
        TaxCode = r.Field<string>("TAX_CODE"),
        Amount = r.Field<object>("TAX_AMT_BILLED") != null ? r.Field<double>("TAX_AMT_BILLED") : 0,
        ParentId = r.Field<byte[]>("CM_COUPON_BD_ID") != null ? new Guid(r.Field<byte[]>("CM_COUPON_BD_ID")) : new Guid(),
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      });

    public readonly Materializer<RMCoupon> LinkedRMCouponMaterializer = new Materializer<RMCoupon>(r =>
   new RMCoupon
   {
     TicketOrFimIssuingAirline = r.TryGetField<object>("TICKET_ISSUING_AIRLINE") != null ? r.Field<string>("TICKET_ISSUING_AIRLINE") : string.Empty,
     TicketOrFimCouponNumber = r.TryGetField<object>("COUPON_NO") != null ? r.Field<int>("COUPON_NO") : 0,
     TicketDocOrFimNumber = r.Field<object>("TICKET_DOC_NO") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOC_NO")) : 0,
     CheckDigit = r.TryGetField<object>("CHECK_DIGIT") != null ? r.Field<int>("CHECK_DIGIT") : 0, //test
     FromAirportOfCoupon = r.TryGetField<string>("FROM_AIRPORT_OF_COUPON"),
     ToAirportOfCoupon = r.TryGetField<string>("TO_AIRPORT_OF_COUPON"),
     GrossAmountBilled = r.TryGetField<object>("BILLED_GROSS_AMT") != null ? r.Field<double>("BILLED_GROSS_AMT") : 0.0,
     GrossAmountAccepted = r.TryGetField<object>("ACCEPTED_GROSS_AMT") != null ? r.Field<double>("ACCEPTED_GROSS_AMT") : 0.0,
     GrossAmountDifference = r.TryGetField<object>("GROSS_AMT_DIFF") != null ? r.Field<double>("GROSS_AMT_DIFF") : 0.0,

     TaxAmountBilled = r.TryGetField<object>("BILLED_TAX_AMT") != null ? r.Field<double>("BILLED_TAX_AMT") : 0.0,
     TaxAmountAccepted = r.TryGetField<object>("ACCPT_TAX_AMT") != null ? r.Field<double>("ACCPT_TAX_AMT") : 0.0,
     TaxAmountDifference = r.TryGetField<object>("TAX_AMT_DIFF") != null ? r.Field<double>("TAX_AMT_DIFF") : 0.0,
     AllowedIscPercentage = r.TryGetField<object>("ALLOWED_ISC_PER") != null ? r.Field<double>("ALLOWED_ISC_PER") : 0.0,
     AllowedIscAmount = r.TryGetField<object>("ALLOWED_ISC_AMT") != null ? r.Field<double>("ALLOWED_ISC_AMT") : 0.0,
     AcceptedIscPercentage = r.TryGetField<object>("ACCPT_ISC_PER") != null ? r.Field<double>("ACCPT_ISC_PER") : 0.0,
     AcceptedIscAmount = r.TryGetField<object>("ACCPT_ISC_AMT") != null ? r.Field<double>("ACCPT_ISC_AMT") : 0.0,
     IscDifference = r.TryGetField<object>("ISC_AMT_DIFF") != null ? r.Field<double>("ISC_AMT_DIFF") : 0.0,

     AllowedOtherCommissionPercentage = r.TryGetField<object>("ALLOWED_OTH_COMM_PER") != null ? r.Field<double>("ALLOWED_OTH_COMM_PER") : 0.0,
     AllowedOtherCommission = r.TryGetField<object>("ALLOWED_OTH_COMM_AMT") != null ? r.Field<double>("ALLOWED_OTH_COMM_AMT") : 0.0,
     AcceptedOtherCommissionPercentage = r.TryGetField<object>("ACCPT_OTH_COMM_PER") != null ? r.Field<double>("ACCPT_OTH_COMM_PER") : 0.0,
     AcceptedOtherCommission = r.TryGetField<object>("ACCPT_OTH_COMM_AMT") != null ? r.Field<double>("ACCPT_OTH_COMM_AMT") : 0.0,
     OtherCommissionDifference = r.TryGetField<object>("OTH_COMM_AMT_DIFF") != null ? r.Field<double>("OTH_COMM_AMT_DIFF") : 0.0,

     AllowedHandlingFee = r.TryGetField<object>("ALLOWED_HANDLING_FEE_AMT") != null ? r.Field<double>("ALLOWED_HANDLING_FEE_AMT") : 0.0,
     AcceptedHandlingFee = r.TryGetField<object>("ACCPT_HANDLING_FEE_AMT") != null ? r.Field<double>("ACCPT_HANDLING_FEE_AMT") : 0.0,
     HandlingDifference = r.TryGetField<object>("ACCPT_HANDLING_AMT_DIFF") != null ? r.Field<double>("ACCPT_HANDLING_AMT_DIFF") : 0.0,

     AllowedUatpPercentage = r.TryGetField<object>("ALLOWED_UATP_PER") != null ? r.Field<double>("ALLOWED_UATP_PER") : 0.0,
     AllowedUatpAmount = r.TryGetField<object>("ALLOWED_UATP_AMT") != null ? r.Field<double>("ALLOWED_UATP_AMT") : 0.0,
     AcceptedUatpPercentage = r.TryGetField<object>("ACCPT_UATP_PER") != null ? r.Field<double>("ACCPT_UATP_PER") : 0.0,
     AcceptedUatpAmount = r.TryGetField<object>("ACCPT_UATP_AMT") != null ? r.Field<double>("ACCPT_UATP_AMT") : 0.0,
     UatpDifference = r.TryGetField<object>("UATP_AMT_DIFF") != null ? r.Field<double>("UATP_AMT_DIFF") : 0.0,

     VatAmountBilled = r.TryGetField<object>("BILLED_VAT_AMT") != null ? r.Field<double>("BILLED_VAT_AMT") : 0.0,
     VatAmountAccepted = r.TryGetField<object>("ACCPT_VAT_AMT") != null ? r.Field<double>("ACCPT_VAT_AMT") : 0.0,
     VatAmountDifference = r.TryGetField<object>("VAT_AMT_DIFF") != null ? r.Field<double>("VAT_AMT_DIFF") : 0.0,

     AgreementIndicatorSupplied = r.TryGetField<string>("AGREEMENT_IND_SUPPLIED"),
     AgreementIndicatorValidated = r.TryGetField<string>("AGREEMENT_IND_VALIDATED"),
     OriginalPmi = r.TryGetField<string>("ORIGINAL_PMI"),
     ValidatedPmi = r.TryGetField<string>("VALIDATED_PMI"),

     SettlementAuthorizationCode = r.TryGetField<string>("SETTLEMENT_AUTH_CODE"),

   });

    public readonly Materializer<PrimeCouponMarketingDetails> CouponTktMarketingMaterializer = new Materializer<PrimeCouponMarketingDetails>(r => new PrimeCouponMarketingDetails
    {
      Id = r.Field<byte[]>("COUPON_RECORD_ID") != null ? new Guid(r.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),
      TicketIssuingAirline = r.Field<string>("TICKET_ISSUING_AIRLINE"),
      TicketDocumentNumber = r.Field<object>("TICKET_DOCUMENT_NUMBER") != null ? Convert.ToInt64(r.Field<object>("TICKET_DOCUMENT_NUMBER")) : 0,
      CouponNumber = r.TryGetField<object>("COUPON_NUMBER") != null ? r.Field<int>("COUPON_NUMBER") : 0,
      CouponMarketingCarrier = r.Field<string>("CPN_MKT_CARRIER"),
      CouponMarketingFlightNumber = r.Field<string>("CPN_MKT_FLIGHTNO"),
      CouponTicketingClassofService = r.Field<string>("CPN_TKT_CLASS_OF_SERVICE"),
      CouponFareBasisTktDesignator = r.Field<string>("CPN_TKT_DESIGNATOR"),
      LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0
    });

  }
  
}

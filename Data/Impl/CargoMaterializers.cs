using System;
using System.IO;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Impl
{
    public class CargoMaterializers
    {
        #region Cargo Invoice Materializers
        /// <summary>
        /// Cargo Invoice
        /// </summary>
        public  readonly Materializer<CargoInvoice> CargoInvoiceMaterializer = new Materializer<CargoInvoice>(r => new CargoInvoice
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
            //  ProvisionalBillingMonth = r.Field<object>("PROV_BILLING_MONTH") != null ? r.Field<int>("PROV_BILLING_MONTH") : 0,
            InvoiceTypeId = r.Field<object>("INVOICE_TYPE_ID") != null ? r.Field<int>("INVOICE_TYPE_ID") : 0,
            // ProvisionalBillingYear = r.Field<object>("PROV_BILLING_YEAR") != null ? r.Field<int>("PROV_BILLING_YEAR") : 0,
            // SamplingConstant = r.TryGetField<object>("SAMPLING_CONSTANT") != null ? r.Field<double>("SAMPLING_CONSTANT") : 0.0,
            // IsFormDEViaIS = r.Field<object>("IS_FORM_DE_VIA_IS") != null ? (r.Field<int>("IS_FORM_DE_VIA_IS") == 0 ? false : true) : false,
            //  IsFormFViaIS = r.Field<object>("IS_FORM_F_VIA_IS") != null ? (r.Field<int>("IS_FORM_F_VIA_IS") == 0 ? false : true) : false,
            //  IsFormABViaIS = r.Field<object>("IS_FORM_AB_VIA_IS") != null ? (r.Field<int>("IS_FORM_AB_VIA_IS") == 0 ? false : true) : false,
            //  IsFormCViaIS = r.Field<object>("IS_FORM_C_VIA_IS") != null ? (r.Field<int>("IS_FORM_C_VIA_IS") == 0 ? false : true) : false,
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
            MiscBilledMemberLocCode = r.Field<string>("MISC_BLD_MEM_LOC_CODE")
        });
        

        public  readonly Materializer<CgoVatIdentifier> CargoVatIdentifierMaterializer = new Materializer<CgoVatIdentifier>(r =>
        new CgoVatIdentifier
        {
            LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
            Identifier = r.Field<string>("VAT_IDENTIFIER"),
            Id = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
            IsOcApplicable = r.Field<object>("APPLICABLE_FOR_OC") != null ? (r.Field<int>("APPLICABLE_FOR_OC") == 0 ? false : true) : false,
            IdentifierCode = r.Field<string>("DESCRIPTION"),
            Description = r.Field<string>("DESCRIPTION"),
            IsActive = r.Field<int>("IS_ACTIVE") > 0
        });

        #endregion

        #region Cargo BillingCode SubTotal Materializers

        public  readonly Materializer<BillingCodeSubTotal> BillingCodeSubTotalMaterializer = new Materializer<BillingCodeSubTotal>(cBillingCode => new BillingCodeSubTotal
        {
            Id = cBillingCode.Field<byte[]>("BILLING_CODE_SUBTOTAL_ID") != null ? new Guid(cBillingCode.Field<byte[]>("BILLING_CODE_SUBTOTAL_ID")) : new Guid(),
            InvoiceId = cBillingCode.Field<byte[]>("INVOICE_ID") != null ? new Guid(cBillingCode.Field<byte[]>("INVOICE_ID")) : new Guid(),
            LastUpdatedBy = cBillingCode.Field<object>("LAST_UPDATED_BY") != null ? cBillingCode.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cBillingCode.Field<object>("LAST_UPDATED_ON") != null ? cBillingCode.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            BillingCode = cBillingCode.Field<object>("BILLING_CODE") != null ? cBillingCode.Field<int>("BILLING_CODE") : 0,
            NumberOfBillingRecords = cBillingCode.Field<object>("TOTAL_BILLING_RECORDS_NO") != null ? cBillingCode.Field<int>("TOTAL_BILLING_RECORDS_NO") : 0,
            BillingCodeSubTotalDesc = cBillingCode.Field<string>("BILLING_CODE_SUBTOTAL_DESC"),
            TotalWeightCharge = cBillingCode.Field<object>("TOTAL_WEIGHT_CHARGES") != null ? Convert.ToDecimal(cBillingCode.Field<object>("TOTAL_WEIGHT_CHARGES")) : 0,
            TotalOtherCharge = cBillingCode.Field<object>("TOTAL_OTHER_CHARGES") != null ? Convert.ToDecimal(cBillingCode.Field<object>("TOTAL_OTHER_CHARGES")) : 0,
            TotalIscAmount = cBillingCode.Field<object>("TOTAL_INTERLINE_SERV_CHARGE") != null ? Convert.ToDecimal(cBillingCode.Field<object>("TOTAL_INTERLINE_SERV_CHARGE")) : 0,
            BillingCodeSbTotal = cBillingCode.Field<object>("BILLING_CODE_SUBTOTAL") != null ? Convert.ToDecimal(cBillingCode.Field<object>("BILLING_CODE_SUBTOTAL")) : 0,
            TotalValuationCharge = cBillingCode.Field<object>("TOTAL_VALUATION_CHARGES") != null ? Convert.ToDecimal(cBillingCode.Field<object>("TOTAL_VALUATION_CHARGES")) : 0,
            TotalVatAmount = cBillingCode.Field<object>("TOTAL_VAT_AMT") != null ? Convert.ToDecimal(cBillingCode.Field<object>("TOTAL_VAT_AMT")) : 0,
            TotalNumberOfRecords = cBillingCode.Field<object>("TOTAL_RECORDS_NO") != null ? cBillingCode.Field<int>("TOTAL_RECORDS_NO") : 0,
            TotalNetAmount = cBillingCode.Field<object>("TOTAL_NET_AMT") != null ? Convert.ToDecimal(cBillingCode.Field<object>("TOTAL_NET_AMT")) : 0,
            BatchSequenceNumber = cBillingCode.Field<object>("BATCH_SEQ_NO") != null ? cBillingCode.Field<int>("BATCH_SEQ_NO") : 0,
            RecordSequenceWithinBatch = cBillingCode.Field<object>("BATCH_REC_SEQ_NO") != null ? cBillingCode.Field<int>("BATCH_REC_SEQ_NO") : 0,

        });

        public  readonly Materializer<CargoBillingCodeSubTotalVat> CargoBillingCodeSubTotalVatMaterializer = new Materializer<CargoBillingCodeSubTotalVat>(cBillingCodeVat => new CargoBillingCodeSubTotalVat
        {
            Id = cBillingCodeVat.Field<byte[]>("BILLING_CODE_SUBTOTAL_VAT_ID") != null ? new Guid(cBillingCodeVat.Field<byte[]>("BILLING_CODE_SUBTOTAL_VAT_ID")) : new Guid(),
            ParentId = cBillingCodeVat.Field<byte[]>("BILLING_CODE_SUBTOTAL_ID") != null ? new Guid(cBillingCodeVat.Field<byte[]>("BILLING_CODE_SUBTOTAL_ID")) : new Guid(),
            LastUpdatedBy = cBillingCodeVat.Field<object>("LAST_UPDATED_BY") != null ? cBillingCodeVat.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cBillingCodeVat.Field<object>("LAST_UPDATED_ON") != null ? cBillingCodeVat.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            VatCalculatedAmount = cBillingCodeVat.Field<object>("VAT_CALC_AMT") != null ? cBillingCodeVat.Field<double>("VAT_CALC_AMT") : 0,
            VatPercentage = cBillingCodeVat.Field<object>("VAT_PER") != null ? cBillingCodeVat.Field<double>("VAT_PER") : 0,
            VatBaseAmount = cBillingCodeVat.Field<object>("VAT_BASE_AMT") != null ? cBillingCodeVat.Field<double>("VAT_BASE_AMT") : 0,
            VatText = cBillingCodeVat.Field<string>("VAT_TEXT"),
            VatLabel = cBillingCodeVat.Field<string>("VAT_LABEL"),
            VatIdentifierId = cBillingCodeVat.Field<object>("VAT_IDENTIFIER_ID") != null ? cBillingCodeVat.Field<int>("VAT_IDENTIFIER_ID") : 0,
            OtVatCalculatedAmount = cBillingCodeVat.Field<object>("OT_VAT_CALC_AMT") != null ? cBillingCodeVat.Field<double>("OT_VAT_CALC_AMT") : 0,
            VatIdentifierText = cBillingCodeVat.Field<string>("VAT_IDENTIFIER"),
        });

        #endregion

        #region Cargo AWB Materializers

        public  readonly Materializer<AwbRecord> CargoAirwayBillMaterializer = new Materializer<AwbRecord>(cAwb => new AwbRecord
        {

            Id = cAwb.Field<byte[]>("AIR_WAY_BILL_ID") != null ? new Guid(cAwb.Field<byte[]>("AIR_WAY_BILL_ID")) : new Guid(),
            InvoiceId = cAwb.Field<byte[]>("INVOICE_ID") != null ? new Guid(cAwb.Field<byte[]>("INVOICE_ID")) : new Guid(),
            BatchSequenceNumber = cAwb.Field<object>("BATCH_SEQ_NO") != null ? cAwb.Field<int>("BATCH_SEQ_NO") : 0,
            RecordSequenceWithinBatch = cAwb.Field<object>("BATCH_REC_SEQ_NO") != null ? cAwb.Field<int>("BATCH_REC_SEQ_NO") : 0,
            AwbDate = cAwb.Field<object>("AWB_DATE") != null ? cAwb.Field<DateTime?>("AWB_DATE") : null,
            AwbSerialNumber = cAwb.Field<object>("AWB_SR_NO") != null ? cAwb.Field<int>("AWB_SR_NO") : 0,
            BillingCodeId = cAwb.Field<object>("AWB_BILLING_CODE") != null ? cAwb.Field<int>("AWB_BILLING_CODE") : 0,
            AwbIssueingAirline = cAwb.Field<object>("AWB_ISSUING_AIRLINE") != null ? cAwb.Field<string>("AWB_ISSUING_AIRLINE") : null,
            AwbCheckDigit = cAwb.Field<object>("CHECK_DIGIT") != null ? cAwb.Field<int>("CHECK_DIGIT") : 0,
            ConsignmentOriginId = cAwb.Field<object>("CONSIGNMENT_ORIGIN_ID") != null ? cAwb.Field<string>("CONSIGNMENT_ORIGIN_ID") : null,
            ConsignmentDestinationId = cAwb.Field<object>("CONSIGNMENT_DESTINATION_ID") != null ? cAwb.Field<string>("CONSIGNMENT_DESTINATION_ID") : null,
            CarriageFromId = cAwb.Field<object>("CARRIAGE_FROM_ID") != null ? cAwb.Field<string>("CARRIAGE_FROM_ID") : null,
            CarriageToId = cAwb.Field<object>("CARRIAGE_TO_ID") != null ? cAwb.Field<string>("CARRIAGE_TO_ID") : null,
            DateOfCarriage = cAwb.Field<object>("CARRIAGE_TRANSFER_DATE") != null ? cAwb.Field<DateTime?>("CARRIAGE_TRANSFER_DATE") : null,
            WeightCharges = cAwb.Field<object>("WEIGHT_CHARGES") != null ? cAwb.Field<double?>("WEIGHT_CHARGES") : 0,
            OtherCharges = cAwb.Field<object>("OTHER_CHARGES") != null ? cAwb.Field<double>("OTHER_CHARGES") : 0,
            AmountSubjectToIsc = cAwb.Field<object>("AMT_SUB_INTERLINE_SERV_CHARGE") != null ? cAwb.Field<double>("AMT_SUB_INTERLINE_SERV_CHARGE") : 0,
            IscPer = cAwb.Field<object>("INTERLINE_SERV_CHARGE_PER") != null ? cAwb.Field<double>("INTERLINE_SERV_CHARGE_PER") : 0,
            CurrencyAdjustmentIndicator = cAwb.Field<object>("CURRENCY_ADJUSTMENT_IND") != null ? cAwb.Field<string>("CURRENCY_ADJUSTMENT_IND") : null,
            BilledWeight = cAwb.Field<object>("BILLED_WEIGHT") != null ? cAwb.Field<int?>("BILLED_WEIGHT") : 0,
            ProvisoReqSpa = cAwb.Field<object>("PROVISO_REQ_SPA") != null ? cAwb.Field<string>("PROVISO_REQ_SPA") : null,
            ProratePer = cAwb.Field<object>("PRORATE_PERCENT") != null ? cAwb.Field<int?>("PRORATE_PERCENT") : 0,
            PartShipmentIndicator = cAwb.Field<object>("PART_SHIPMENT_IND") != null ? cAwb.Field<string>("PART_SHIPMENT_IND") : null,
            FilingReference = cAwb.Field<object>("FILING_REFERENCE") != null ? cAwb.Field<string>("FILING_REFERENCE") : null,
            ValuationCharges = cAwb.Field<object>("VALUATION_CHARGES") != null ? cAwb.Field<double?>("VALUATION_CHARGES") : 0,
            KgLbIndicator = cAwb.Field<object>("KG_LB_INDICATOR") != null ? cAwb.Field<string>("KG_LB_INDICATOR") : null,
            VatAmount = cAwb.Field<object>("VAT_AMT") != null ? cAwb.Field<double?>("VAT_AMT") : 0,
            IscAmount = cAwb.Field<object>("INTERLINE_SERV_CHARGE_AMT") != null ? cAwb.Field<double>("INTERLINE_SERV_CHARGE_AMT") : 0,
            AwbTotalAmount = cAwb.Field<object>("AWB_TOTAL_AMT") != null ? cAwb.Field<double?>("AWB_TOTAL_AMT") : 0,
            CcaIndicator = cAwb.Field<object>("CCA_INDICATOR") != null ? (cAwb.Field<int>("CCA_INDICATOR") == 0 ? false : true) : false,
            OurReference = cAwb.Field<object>("OUR_REFERENCE") != null ? cAwb.Field<string>("OUR_REFERENCE") : null,
            AttachmentIndicatorOriginal = cAwb.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (cAwb.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? false : true) : false,
            AttachmentIndicatorValidated = cAwb.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (cAwb.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
            NumberOfAttachments = cAwb.Field<object>("ATTACHMENT_NO") != null ? cAwb.Field<int?>("ATTACHMENT_NO") : 0,
            ISValidationFlag = cAwb.Field<object>("IS_VALIDATION_FLAG") != null ? cAwb.Field<string>("IS_VALIDATION_FLAG") : null,
            ReasonCode = cAwb.Field<object>("REASON_CODE") != null ? cAwb.Field<string>("REASON_CODE") : null,
            ReferenceField1 = cAwb.Field<object>("REFERENCE_FIELD_1") != null ? cAwb.Field<string>("REFERENCE_FIELD_1") : null,
            ReferenceField2 = cAwb.Field<object>("REFERENCE_FIELD_2") != null ? cAwb.Field<string>("REFERENCE_FIELD_2") : null,
            ReferenceField3 = cAwb.Field<object>("REFERENCE_FIELD_3") != null ? cAwb.Field<string>("REFERENCE_FIELD_3") : null,
            ReferenceField4 = cAwb.Field<object>("REFERENCE_FIELD_4") != null ? cAwb.Field<string>("REFERENCE_FIELD_4") : null,
            ReferenceField5 = cAwb.Field<object>("REFERENCE_FIELD_5") != null ? cAwb.Field<string>("REFERENCE_FIELD_5") : null,
            AirlineOwnUse = cAwb.Field<object>("AIRLINE_OWN_USE") != null ? cAwb.Field<string>("AIRLINE_OWN_USE") : null,
            LastUpdatedBy = cAwb.Field<object>("LAST_UPDATED_BY") != null ? cAwb.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cAwb.Field<object>("LAST_UPDATED_ON") != null ? cAwb.Field<DateTime>("LAST_UPDATED_ON") : new DateTime()
        });

        public  readonly Materializer<AwbVat> CargoAwbVatMaterializer = new Materializer<AwbVat>(cAwbVat => new AwbVat
        {

            Id = cAwbVat.Field<byte[]>("AWB_VAT_ID") != null ? new Guid(cAwbVat.Field<byte[]>("AWB_VAT_ID")) : new Guid(),
            ParentId = cAwbVat.Field<byte[]>("AIR_WAY_BILL_ID") != null ? new Guid(cAwbVat.Field<byte[]>("AIR_WAY_BILL_ID")) : new Guid(),
            LastUpdatedBy = cAwbVat.Field<object>("LAST_UPDATED_BY") != null ? cAwbVat.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cAwbVat.Field<object>("LAST_UPDATED_ON") != null ? cAwbVat.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            VatCalculatedAmount = cAwbVat.Field<object>("VAT_CALC_AMT") != null ? cAwbVat.Field<double>("VAT_CALC_AMT") : 0,
            VatPercentage = cAwbVat.Field<object>("VAT_PER") != null ? cAwbVat.Field<double>("VAT_PER") : 0,
            VatBaseAmount = cAwbVat.Field<object>("VAT_BASE_AMT") != null ? cAwbVat.Field<double>("VAT_BASE_AMT") : 0,
            VatText = cAwbVat.Field<object>("VAT_TEXT") != null ? cAwbVat.Field<string>("VAT_TEXT") : null,
            VatLabel = cAwbVat.Field<object>("VAT_LABEL") != null ? cAwbVat.Field<string>("VAT_LABEL") : null,
            VatIdentifierId = cAwbVat.Field<object>("VAT_IDENTIFIER_ID") != null ? cAwbVat.Field<int>("VAT_IDENTIFIER_ID") : 0

        });

        public  readonly Materializer<AwbAttachment> CargoAwbAttachmentMaterializer = new Materializer<AwbAttachment>(cAwbA => new AwbAttachment
        {
            ServerId = cAwbA.Field<object>("SERVER_ID") != null ? cAwbA.Field<int>("SERVER_ID") : 0,
            FilePath = cAwbA.Field<string>("FILE_PATH"),
            OriginalFileName = cAwbA.Field<string>("FILE_NAME"),
            LastUpdatedOn = cAwbA.Field<object>("LAST_UPDATED_ON") != null ? cAwbA.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cAwbA.Field<object>("LAST_UPDATED_BY") != null ? cAwbA.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cAwbA.Field<byte[]>("AIR_WAY_BILL_ID") != null ? (Guid?)new Guid(cAwbA.Field<byte[]>("AIR_WAY_BILL_ID")) : null,
            FileTypeId = cAwbA.Field<object>("FILE_TYPE_ID") != null ? cAwbA.Field<int>("FILE_TYPE_ID") : 0,
            FileSize = cAwbA.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(cAwbA.Field<object>("FILE_SIZE")) : 0,
            IsFullPath = cAwbA.Field<object>("IS_FULL_PATH") != null ? (cAwbA.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
            Id = cAwbA.Field<byte[]>("AWB_RECORD_ATTACHMENT_ID") != null ? new Guid(cAwbA.Field<byte[]>("AWB_RECORD_ATTACHMENT_ID")) : new Guid(),
        });

        public  readonly Materializer<AwbOtherCharge> CargoAwbOtherChargeMaterializer = new Materializer<AwbOtherCharge>(cAwbOc => new AwbOtherCharge
        {

            Id = cAwbOc.Field<byte[]>("AWB_OTHER_CHARGE_ID") != null ? new Guid(cAwbOc.Field<byte[]>("AWB_OTHER_CHARGE_ID")) : new Guid(),
            ParentId = cAwbOc.Field<byte[]>("AIR_WAY_BILL_ID") != null ? new Guid(cAwbOc.Field<byte[]>("AIR_WAY_BILL_ID")) : new Guid(),
            LastUpdatedBy = cAwbOc.Field<object>("LAST_UPDATED_BY") != null ? cAwbOc.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cAwbOc.Field<object>("LAST_UPDATED_ON") != null ? cAwbOc.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            OtherChargeCode = cAwbOc.Field<string>("OTHER_CHARGE_CODE"),
            OtherChargeCodeValue = cAwbOc.Field<object>("OTHER_CHARGE_CODE_VALUE") != null ? cAwbOc.Field<double>("OTHER_CHARGE_CODE_VALUE") : 0,
            OtherChargeVatCalculatedAmount = cAwbOc.Field<object>("VAT_CALC_AMT") != null ? cAwbOc.Field<double?>("VAT_CALC_AMT") : null,
            OtherChargeVatPercentage = cAwbOc.Field<object>("VAT_PER") != null ? cAwbOc.Field<double?>("VAT_PER") : null,
            OtherChargeVatBaseAmount = cAwbOc.Field<object>("VAT_BASE_AMT") != null ? cAwbOc.Field<double?>("VAT_BASE_AMT") : null,
            OtherChargeVatText = cAwbOc.Field<string>("VAT_TEXT"),
            OtherChargeVatLabel = cAwbOc.Field<string>("VAT_LABEL")

        });

        #endregion

        #region Cargo Rejection Memo Materializers

        public  readonly Materializer<CargoRejectionMemo> CargoRejectionMemoMaterializer = new Materializer<CargoRejectionMemo>(r =>
        new CargoRejectionMemo
        {
            Id = r.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(r.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
            InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
            BatchSequenceNumber = r.Field<object>("BATCH_SEQ_NO") != null ? r.Field<int>("BATCH_SEQ_NO") : 0,
            RecordSequenceWithinBatch = r.Field<object>("BATCH_REC_SEQ_NO") != null ? r.Field<int>("BATCH_REC_SEQ_NO") : 0,
            RejectionMemoNumber = r.Field<string>("REJECTION_MEMO_NO"),
            RejectionStage = r.Field<object>("REJECTION_STAGE") != null ? r.Field<int>("REJECTION_STAGE") : 0,
            ReasonCode = r.Field<string>("REASON_CODE"),
            AirlineOwnUse = r.Field<string>("AIRLINE_OWN_USE"),
            OurRef = r.Field<string>("INTERNAL_REF"),
            BillingCode = r.Field<object>("BILLING_CODE") != null ? r.Field<int>("BILLING_CODE") : 0,
            YourInvoiceNumber = r.Field<string>("YOUR_INVOICE_NO"),
            YourInvoiceBillingMonth = r.Field<object>("YOUR_INVOICE_BILLING_MONTH") != null ? r.Field<int>("YOUR_INVOICE_BILLING_MONTH") : 0,
            YourInvoiceBillingYear = r.Field<object>("YOUR_INVOICE_BILLING_YEAR") != null ? r.Field<int>("YOUR_INVOICE_BILLING_YEAR") : 0,
            YourInvoiceBillingPeriod = r.Field<object>("YOUR_INVOICE_BILLING_PERIOD") != null ? r.Field<int>("YOUR_INVOICE_BILLING_PERIOD") : 0,
            YourBillingMemoNumber = r.Field<string>("YOUR_BM_CM_NO"),
            BMCMIndicatorId = r.Field<object>("BM_CM_INDICATOR") != null ? r.Field<int>("BM_CM_INDICATOR") : 0,
            YourRejectionNumber = r.Field<string>("YOUR_REJECTION_MEMO_NO"),
            BilledTotalWeightCharge = r.Field<object>("BILLED_TOTAL_WEIGHT_CHARGE") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("BILLED_TOTAL_WEIGHT_CHARGE")) : null,
            AcceptedTotalWeightCharge = r.Field<object>("ACCEPTED_TOTAL_WEIGHT_CHARGE") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("ACCEPTED_TOTAL_WEIGHT_CHARGE")) : null,
            TotalWeightChargeDifference = r.Field<object>("TOTAL_WEIGHT_CHARGES_DIFF") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("TOTAL_WEIGHT_CHARGES_DIFF")) : null,
            BilledTotalValuationCharge = r.Field<object>("BILLED_TOTAL_VALUATION_CHARGES") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("BILLED_TOTAL_VALUATION_CHARGES")) : null,
            AcceptedTotalValuationCharge = r.Field<object>("ACCPTED_TOTAL_VALUATION_CHARGE") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("ACCPTED_TOTAL_VALUATION_CHARGE")) : null,
            TotalValuationChargeDifference = r.Field<object>("TOTAL_VALUATION_CHARGES_DIFF") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("TOTAL_VALUATION_CHARGES_DIFF")) : null,
            BilledTotalOtherChargeAmount = r.Field<object>("BILLED_TOTAL_OTHER_CHARGES_AMT") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("BILLED_TOTAL_OTHER_CHARGES_AMT")) : null,
            AcceptedTotalOtherChargeAmount = r.Field<object>("ACCEPTED_TOTAL_OTH_CHARGES_AMT") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("ACCEPTED_TOTAL_OTH_CHARGES_AMT")) : null,
            TotalOtherChargeDifference = r.Field<object>("TOTAL_OTH_CHARGES_DIFF") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("TOTAL_OTH_CHARGES_DIFF")) : null,
            AllowedTotalIscAmount = r.Field<object>("ALLOWED_TOTAL_ISC_AMT") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("ALLOWED_TOTAL_ISC_AMT")) : null,
            AcceptedTotalIscAmount = r.Field<object>("ACCEPTED_TOTAL_ISC_AMT") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("ACCEPTED_TOTAL_ISC_AMT")) : null,
            TotalIscAmountDifference = r.Field<object>("TOTAL_ISC_AMT_DIFF") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("TOTAL_ISC_AMT_DIFF")) : null,
            BilledTotalVatAmount = r.Field<object>("BILLED_TOTAL_VAT_AMT") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("BILLED_TOTAL_VAT_AMT")) : null,
            AcceptedTotalVatAmount = r.Field<object>("ACCEPTED_TOTAL_VAT_AMT") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("ACCEPTED_TOTAL_VAT_AMT")) : null,
            TotalVatAmountDifference = r.Field<object>("TOTAL_VAT_AMT_DIFF") != null ? r.Field<double?>("TOTAL_VAT_AMT_DIFF") : null,
            TotalNetRejectAmount = r.Field<object>("TOTAL_NET_REJECT_AMT") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("TOTAL_NET_REJECT_AMT")) : null,
            CorrespondenceId = r.Field<byte[]>("CORRESPONDENCE_ID") != null ? (Guid?)new Guid(r.Field<byte[]>("CORRESPONDENCE_ID")) : null,
            AttachmentIndicatorOriginal = r.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (r.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? false : true) : false,
            AttachmentIndicatorValidated = r.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
            NumberOfAttachments = r.Field<object>("ATTACHMENT_NO") != null ? r.Field<int?>("ATTACHMENT_NO") : null,
            ISValidationFlag = r.Field<string>("IS_VALIDATION_FLAG"),
            IsLinkingSuccessful = r.Field<object>("IS_LINKING_SUCCESSFUL") != null ? (bool?)(r.Field<int>("IS_LINKING_SUCCESSFUL") > 0) : null,
            IsBreakdownAllowed = r.Field<object>("IS_BREAKDOWN_ALLOWED") != null ? (r.Field<int>("IS_BREAKDOWN_ALLOWED") > 0 ? (bool?)true : (bool?)false) : null,
            CurrencyConversionFactor = r.Field<object>("CURR_CONVERSION_FACTOR") != null ? (decimal?)Convert.ToDecimal(r.Field<object>("CURR_CONVERSION_FACTOR")) : null,
            ReasonRemarks = r.Field<string>("REASON_REMARKS"),
            LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        });

        public  readonly Materializer<CgoRejectionMemoAttachment> CgoRejectionMemoAttachmentMaterializer = new Materializer<CgoRejectionMemoAttachment>(cRejectionMemoA => new CgoRejectionMemoAttachment
        {
            ServerId = cRejectionMemoA.Field<object>("SERVER_ID") != null ? cRejectionMemoA.Field<int>("SERVER_ID") : 0,
            FilePath = cRejectionMemoA.Field<string>("FILE_PATH"),
            OriginalFileName = cRejectionMemoA.Field<string>("FILE_NAME"),
            LastUpdatedOn = cRejectionMemoA.Field<object>("LAST_UPDATED_ON") != null ? cRejectionMemoA.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cRejectionMemoA.Field<object>("LAST_UPDATED_BY") != null ? cRejectionMemoA.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cRejectionMemoA.Field<byte[]>("REJECTION_MEMO_ID") != null ? (Guid?)new Guid(cRejectionMemoA.Field<byte[]>("REJECTION_MEMO_ID")) : null,
            FileTypeId = cRejectionMemoA.Field<object>("FILE_TYPE_ID") != null ? cRejectionMemoA.Field<int>("FILE_TYPE_ID") : 0,
            SerialNo = cRejectionMemoA.Field<object>("SR_NO") != null ? cRejectionMemoA.Field<int>("SR_NO") : 0,
            FileStatusId = cRejectionMemoA.Field<object>("FILE_STATUS_ID") != null ? cRejectionMemoA.Field<int>("FILE_STATUS_ID") : 0,
            FileSize = cRejectionMemoA.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(cRejectionMemoA.Field<object>("FILE_SIZE")) : 0,
            IsFullPath = cRejectionMemoA.Field<object>("IS_FULL_PATH") != null ? (cRejectionMemoA.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
            Id = cRejectionMemoA.Field<byte[]>("RM_ATTACHMENT_ID") != null ? new Guid(cRejectionMemoA.Field<byte[]>("RM_ATTACHMENT_ID")) : new Guid(),
        });

        public  readonly Materializer<CgoRejectionMemoVat> CgoRejectionMemoVatMaterializer = new Materializer<CgoRejectionMemoVat>(r => new CgoRejectionMemoVat
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
            Id = r.Field<byte[]>("RM_VAT_ID") != null ? new Guid(r.Field<byte[]>("RM_VAT_ID")) : new Guid(),
        });

        public  readonly Materializer<RMAwb> CgoRmAwbMaterializer = new Materializer<RMAwb>(rmAwb => new RMAwb
        {

            Id = rmAwb.Field<byte[]>("RM_AWB_ID") != null ? new Guid(rmAwb.Field<byte[]>("RM_AWB_ID")) : new Guid(),
            LastUpdatedOn = rmAwb.Field<object>("LAST_UPDATED_ON") != null ? rmAwb.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = rmAwb.Field<object>("LAST_UPDATED_BY") != null ? rmAwb.Field<int>("LAST_UPDATED_BY") : 0,
            RejectionMemoId = rmAwb.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(rmAwb.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
            BdSerialNumber = rmAwb.Field<object>("BD_SR_NO") != null ? rmAwb.Field<int>("BD_SR_NO") : 0,
            AwbDate = rmAwb.Field<object>("AWB_DATE") != null ? rmAwb.Field<DateTime?>("AWB_DATE") : null,
            AwbIssueingAirline = rmAwb.Field<string>("AWB_ISSUING_AIRLINE"),
            AwbSerialNumber = rmAwb.Field<object>("AWB_SR_NO") != null ? rmAwb.Field<int>("AWB_SR_NO") : 0,
            AwbCheckDigit = rmAwb.Field<object>("AWB_CHECK_DIGIT") != null ? rmAwb.Field<int>("AWB_CHECK_DIGIT") : 0,
            ConsignmentOriginId = rmAwb.Field<string>("CONSIGNMENT_ORIGIN_ID"),
            ConsignmentDestinationId = rmAwb.Field<string>("CONSIGNMENT_DESTINATION_ID"),
            CarriageFromId = rmAwb.Field<string>("CARRIAGE_FROM_ID"),
            CarriageToId = rmAwb.Field<string>("CARRIAGE_TO_ID"),
            TransferDate = rmAwb.Field<object>("TRANSFER_DATE") != null ? rmAwb.Field<DateTime?>("TRANSFER_DATE") : null,
            BilledWeightCharge = rmAwb.Field<object>("BILLED_WEIGHT_CHARGES") != null ? rmAwb.Field<double?>("BILLED_WEIGHT_CHARGES") : 0,
            AcceptedWeightCharge = rmAwb.Field<object>("ACCEPTED_WEIGHT_CHARGES") != null ? rmAwb.Field<double?>("ACCEPTED_WEIGHT_CHARGES") : 0,
            WeightChargeDiff = rmAwb.Field<object>("WEIGHT_CHARGES_DIFF") != null ? rmAwb.Field<double?>("WEIGHT_CHARGES_DIFF") : 0,
            BilledValuationCharge = rmAwb.Field<object>("BILLED_VALUATION_CHARGES") != null ? rmAwb.Field<double?>("BILLED_VALUATION_CHARGES") : 0,
            AcceptedValuationCharge = rmAwb.Field<object>("ACCEPTED_VALUATION_CHARGES") != null ? rmAwb.Field<double?>("ACCEPTED_VALUATION_CHARGES") : 0,
            ValuationChargeDiff = rmAwb.Field<object>("VALUATION_CHARGES_DIFF") != null ? rmAwb.Field<double?>("VALUATION_CHARGES_DIFF") : 0,
            BilledOtherCharge = rmAwb.Field<object>("BILLED_OTHER_CHARGES_AMT") != null ? rmAwb.Field<double>("BILLED_OTHER_CHARGES_AMT") : 0,
            AcceptedOtherCharge = rmAwb.Field<object>("ACCEPTED_OTHER_CHARGES_AMT") != null ? rmAwb.Field<double>("ACCEPTED_OTHER_CHARGES_AMT") : 0,
            OtherChargeDiff = rmAwb.Field<object>("OTHER_CHARGES_DIFF") != null ? rmAwb.Field<double>("OTHER_CHARGES_DIFF") : 0,
            AllowedAmtSubToIsc = rmAwb.Field<object>("ALLOWED_AMOUNT_SUB_TO_ISC") != null ? rmAwb.Field<double>("ALLOWED_AMOUNT_SUB_TO_ISC") : 0,
            AcceptedAmtSubToIsc = rmAwb.Field<object>("ACCEPTED_AMOUNT_SUB_TO_ISC") != null ? rmAwb.Field<double>("ACCEPTED_AMOUNT_SUB_TO_ISC") : 0,
            AllowedIscPercentage = rmAwb.Field<object>("ALLOWED_ISC_PER") != null ? rmAwb.Field<double>("ALLOWED_ISC_PER") : 0,
            AcceptedIscPercentage = rmAwb.Field<object>("ACCEPTED_ISC_PER") != null ? rmAwb.Field<double>("ACCEPTED_ISC_PER") : 0,
            AllowedIscAmount = rmAwb.Field<object>("ALLOWED_ISC_AMT") != null ? rmAwb.Field<double>("ALLOWED_ISC_AMT") : 0,
            AcceptedIscAmount = rmAwb.Field<object>("ACCEPTED_ISC_AMT") != null ? rmAwb.Field<double>("ACCEPTED_ISC_AMT") : 0,
            IscAmountDifference = rmAwb.Field<object>("ISC_AMT_DIFF") != null ? rmAwb.Field<double>("ISC_AMT_DIFF") : 0,
            BilledVatAmount = rmAwb.Field<object>("BILLED_VAT_AMT") != null ? rmAwb.Field<double?>("BILLED_VAT_AMT") : 0,
            AcceptedVatAmount = rmAwb.Field<object>("ACCEPTED_VAT_AMT") != null ? rmAwb.Field<double?>("ACCEPTED_VAT_AMT") : 0,
            VatAmountDifference = rmAwb.Field<object>("VAT_AMT_DIFF") != null ? rmAwb.Field<double?>("VAT_AMT_DIFF") : 0,
            NetRejectAmount = rmAwb.Field<object>("NET_REJECT_AMT") != null ? rmAwb.Field<double>("NET_REJECT_AMT") : 0,
            AwbBillingCode = rmAwb.Field<object>("AWB_BILLING_CODE") != null ? rmAwb.Field<int>("AWB_BILLING_CODE") : 0,
            OurReference = rmAwb.Field<string>("OUR_REFERENCE"),
            CurrencyAdjustmentIndicator = rmAwb.Field<string>("CURRENCY_ADJUSTMENT_IND"),
            BilledWeight = rmAwb.Field<object>("BILLED_WEIGHT") != null ? rmAwb.Field<int?>("BILLED_WEIGHT") : null,
            ProvisionalReqSpa = rmAwb.Field<string>("PROVISO_REQ_SPA"),
            ProratePercentage = rmAwb.Field<object>("PRORATE_PER") != null ? rmAwb.Field<int?>("PRORATE_PER") : null,
            PartShipmentIndicator = rmAwb.Field<string>("PART_SHIPMENT_IND"),
            KgLbIndicator = rmAwb.Field<string>("KG_LB_INDICATOR"),
            CcaIndicator = rmAwb.Field<object>("CCA_INDICATOR") != null ? (rmAwb.Field<int>("CCA_INDICATOR") == 0 ? false : true) : false,
            AttachmentIndicatorOriginal = rmAwb.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (rmAwb.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? false : true) : false,
            AttachmentIndicatorValidated = rmAwb.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (rmAwb.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
            NumberOfAttachments = rmAwb.Field<object>("ATTACHMENTS_NO") != null ? rmAwb.Field<int?>("ATTACHMENTS_NO") : null,
            ISValidationFlag = rmAwb.Field<string>("IS_VALIDATION_FLAG"),
            ReasonCode = rmAwb.Field<string>("REASON_CODE"),
            ReferenceField1 = rmAwb.Field<string>("REFERENCE_FIELD_1"),
            ReferenceField2 = rmAwb.Field<string>("REFERENCE_FIELD_2"),
            ReferenceField3 = rmAwb.Field<string>("REFERENCE_FIELD_3"),
            ReferenceField4 = rmAwb.Field<string>("REFERENCE_FIELD_4"),
            ReferenceField5 = rmAwb.Field<string>("REFERENCE_FIELD_5"),
            AirlineOwnUse = rmAwb.Field<string>("AIRLINE_OWN_USE"),
            //SCP:53226 - Prorate Ladder information Currency not being displayed Correctly in PDF Rejection Memo generated by SIS - Air Calin
            // check if ProrateCalCurrencyId is null then replace with empty string.
            ProrateCalCurrencyId = rmAwb.Field<string>("PRORATE_CALC_CURRENCY_ID") ?? string.Empty,
            TotalProrateAmount = rmAwb.Field<object>("TOTAL_PRORATE_AMOUNT") != null ? rmAwb.Field<double?>("TOTAL_PRORATE_AMOUNT") : null,

        });

        public  readonly Materializer<RMAwb> LinkedRMAwbMaterializer = new Materializer<RMAwb>(rmAwb => new RMAwb
        {

            Id = rmAwb.Field<byte[]>("RM_AWB_ID") != null ? new Guid(rmAwb.Field<byte[]>("RM_AWB_ID")) : new Guid(),
            AwbDate = rmAwb.Field<object>("AWB_DATE") != null ? rmAwb.Field<DateTime?>("AWB_DATE") : null,
            AwbCheckDigit = rmAwb.Field<object>("AWB_CHECK_DIGIT") != null ? rmAwb.Field<int>("AWB_CHECK_DIGIT") : 0,
            AwbIssueingAirline = rmAwb.Field<string>("AWB_ISSUING_AIRLINE"),
            AwbSerialNumber = rmAwb.Field<object>("AWB_SR_NO") != null ? rmAwb.Field<int>("AWB_SR_NO") : 0,
            ConsignmentOriginId = rmAwb.Field<string>("CONSIGNMENT_ORIGIN_ID"),
            ConsignmentDestinationId = rmAwb.Field<string>("CONSIGNMENT_DESTINATION_ID"),
            CarriageFromId = rmAwb.Field<string>("CARRIAGE_FROM_ID"),
            CarriageToId = rmAwb.Field<string>("CARRIAGE_TO_ID"),
            TransferDate = rmAwb.Field<object>("TRANSFER_DATE") != null ? rmAwb.Field<DateTime?>("TRANSFER_DATE") : null,
            BilledWeightCharge = rmAwb.Field<object>("BILLED_WEIGHT_CHARGES") != null ? rmAwb.Field<double?>("BILLED_WEIGHT_CHARGES") : 0,
            AcceptedWeightCharge = rmAwb.Field<object>("ACCEPTED_WEIGHT_CHARGES") != null ? rmAwb.Field<double?>("ACCEPTED_WEIGHT_CHARGES") : 0,
            WeightChargeDiff = rmAwb.Field<object>("WEIGHT_CHARGES_DIFF") != null ? rmAwb.Field<double?>("WEIGHT_CHARGES_DIFF") : 0,
            BilledValuationCharge = rmAwb.Field<object>("BILLED_VAL_AMT") != null ? rmAwb.Field<double?>("BILLED_VAL_AMT") : 0,
            AcceptedValuationCharge = rmAwb.Field<object>("ACCPT_VAL_AMT") != null ? rmAwb.Field<double?>("ACCPT_VAL_AMT") : 0,
            ValuationChargeDiff = rmAwb.Field<object>("VAL_AMT_DIFF") != null ? rmAwb.Field<double?>("VAL_AMT_DIFF") : 0,
            BilledOtherCharge = rmAwb.Field<object>("ALLOWED_OTHER_CHARGES_AMT") != null ? rmAwb.Field<double>("ALLOWED_OTHER_CHARGES_AMT") : 0,
            AcceptedOtherCharge = rmAwb.Field<object>("ACCPT_OTHER_CHARGES_AMT") != null ? rmAwb.Field<double>("ACCPT_OTHER_CHARGES_AMT") : 0,
            OtherChargeDiff = rmAwb.Field<object>("OTHER_CHARGES_AMT_DIFF") != null ? rmAwb.Field<double>("OTHER_CHARGES_AMT_DIFF") : 0,
            AllowedAmtSubToIsc = rmAwb.Field<object>("ALLOWED_AMOUNT_SUB_TO_ISC") != null ? rmAwb.Field<double>("ALLOWED_AMOUNT_SUB_TO_ISC") : 0,
            AcceptedAmtSubToIsc = rmAwb.Field<object>("ACCEPTED_AMOUNT_SUB_TO_ISC") != null ? rmAwb.Field<double>("ACCEPTED_AMOUNT_SUB_TO_ISC") : 0,
            AllowedIscPercentage = rmAwb.Field<object>("ALLOWED_ISC_PER") != null ? rmAwb.Field<double>("ALLOWED_ISC_PER") : 0,
            AcceptedIscPercentage = rmAwb.Field<object>("ACCPT_ISC_PER") != null ? rmAwb.Field<double>("ACCPT_ISC_PER") : 0,
            AllowedIscAmount = rmAwb.Field<object>("ALLOWED_ISC_AMT") != null ? rmAwb.Field<double>("ALLOWED_ISC_AMT") : 0,
            AcceptedIscAmount = rmAwb.Field<object>("ACCPT_ISC_AMT") != null ? rmAwb.Field<double>("ACCPT_ISC_AMT") : 0,
            IscAmountDifference = rmAwb.Field<object>("ISC_AMT_DIFF") != null ? rmAwb.Field<double>("ISC_AMT_DIFF") : 0,
            BilledVatAmount = rmAwb.Field<object>("BILLED_VAT_AMT") != null ? rmAwb.Field<double?>("BILLED_VAT_AMT") : 0,
            AcceptedVatAmount = rmAwb.Field<object>("ACCPT_VAT_AMT") != null ? rmAwb.Field<double?>("ACCPT_VAT_AMT") : 0,
            VatAmountDifference = rmAwb.Field<object>("VAT_AMT_DIFF") != null ? rmAwb.Field<double?>("VAT_AMT_DIFF") : 0,
            AwbBillingCode = rmAwb.Field<object>("AWB_BILLING_CODE") != null ? rmAwb.Field<int>("AWB_BILLING_CODE") : 0
        });

        public  readonly Materializer<RMAwbVat> CargoRMAwbVatMaterializer = new Materializer<RMAwbVat>(r =>
        new RMAwbVat
        {
            LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = r.Field<byte[]>("RM_AWB_ID") != null ? new Guid(r.Field<byte[]>("RM_AWB_ID")) : new Guid(),
            VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0,
            VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0,
            VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0,
            VatText = r.Field<object>("VAT_TEXT") != null ? r.Field<string>("VAT_TEXT") : null,
            VatLabel = r.Field<object>("VAT_LABEL") != null ? r.Field<string>("VAT_LABEL") : null,
            VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
            Id = r.Field<byte[]>("RM_AWB_VAT_ID") != null ? new Guid(r.Field<byte[]>("RM_AWB_VAT_ID")) : new Guid(),
        });

       

        public  readonly Materializer<RMAwbProrateLadderDetail> RMAwbProrateLadderDetailMaterializer = new Materializer<RMAwbProrateLadderDetail>(cRmawbPLadder => new RMAwbProrateLadderDetail
        {
            LastUpdatedBy = cRmawbPLadder.Field<object>("LAST_UPDATED_BY") != null ? cRmawbPLadder.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cRmawbPLadder.Field<object>("LAST_UPDATED_ON") != null ? cRmawbPLadder.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            ParentId = cRmawbPLadder.Field<byte[]>("RM_AWB_ID") != null ? new Guid(cRmawbPLadder.Field<byte[]>("RM_AWB_ID")) : new Guid(),
            FromSector = cRmawbPLadder.Field<string>("FROM_SECTOR_ID"),
            ToSector = cRmawbPLadder.Field<string>("TO_SECTOR_ID"),
            CarrierPrefix = cRmawbPLadder.Field<string>("CARRIER_PREFIX"),
            ProvisoReqSpa = cRmawbPLadder.Field<string>("PROVISO_REQ_SPA"),
            ProrateFactor = cRmawbPLadder.Field<object>("PRORATE_FACTOR") != null ? (long?)Convert.ToInt64(cRmawbPLadder.Field<object>("PRORATE_FACTOR")) : null,
            PercentShare = cRmawbPLadder.Field<object>("PERCENT_SHARE") != null ? cRmawbPLadder.Field<double?>("PERCENT_SHARE") : null,
            Amount = cRmawbPLadder.Field<object>("AMOUNT") != null ? cRmawbPLadder.Field<double?>("AMOUNT") : null,
            SequenceNumber = cRmawbPLadder.Field<object>("SEQUENCE_NO") != null ? cRmawbPLadder.Field<int?>("SEQUENCE_NO") : null,
            Id = cRmawbPLadder.Field<byte[]>("RM_AWB_LADDER_DETAIL_ID") != null ? new Guid(cRmawbPLadder.Field<byte[]>("RM_AWB_LADDER_DETAIL_ID")) : new Guid()
        });
        public  readonly Materializer<RMAwbAttachment> RMAwbAttachmentMaterializer = new Materializer<RMAwbAttachment>(cRmAwbA => new RMAwbAttachment
        {
            ServerId = cRmAwbA.Field<object>("SERVER_ID") != null ? cRmAwbA.Field<int>("SERVER_ID") : 0,
            FilePath = cRmAwbA.Field<string>("FILE_PATH"),
            OriginalFileName = cRmAwbA.Field<string>("ORG_FILE_NAME"),
            LastUpdatedOn = cRmAwbA.Field<object>("LAST_UPDATED_ON") != null ? cRmAwbA.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cRmAwbA.Field<object>("LAST_UPDATED_BY") != null ? cRmAwbA.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cRmAwbA.Field<byte[]>("RM_AWB_ID") != null ? (Guid?)new Guid(cRmAwbA.Field<byte[]>("RM_AWB_ID")) : null,
            FileTypeId = cRmAwbA.Field<object>("FILE_TYPE_ID") != null ? cRmAwbA.Field<int>("FILE_TYPE_ID") : 0,
            FileStatusId = cRmAwbA.Field<object>("FILE_STATUS_ID") != null ? cRmAwbA.Field<int>("FILE_STATUS_ID") : 0,
            FileSize = cRmAwbA.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(cRmAwbA.Field<object>("FILE_SIZE")) : 0,
            IsFullPath = cRmAwbA.Field<object>("IS_FULL_PATH") != null ? (cRmAwbA.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
            Id = cRmAwbA.Field<byte[]>("RM_AWB_ATTACHMENTS_ID") != null ? new Guid(cRmAwbA.Field<byte[]>("RM_AWB_ATTACHMENTS_ID")) : new Guid(),
        });

        public  readonly Materializer<RMAwbOtherCharge> RMAwbOtherChargeMaterializer = new Materializer<RMAwbOtherCharge>(cRmAwbOc => new RMAwbOtherCharge
        {

            Id = cRmAwbOc.Field<byte[]>("RM_AWB_OTHER_CHARGE_ID") != null ? new Guid(cRmAwbOc.Field<byte[]>("RM_AWB_OTHER_CHARGE_ID")) : new Guid(),
            ParentId = cRmAwbOc.Field<byte[]>("RM_AWB_ID") != null ? new Guid(cRmAwbOc.Field<byte[]>("RM_AWB_ID")) : new Guid(),
            LastUpdatedBy = cRmAwbOc.Field<object>("LAST_UPDATED_BY") != null ? cRmAwbOc.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cRmAwbOc.Field<object>("LAST_UPDATED_ON") != null ? cRmAwbOc.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            OtherChargeCode = cRmAwbOc.Field<string>("OTHER_CHARGE_CODE"),
            OtherChargeCodeValue = cRmAwbOc.Field<object>("OTHER_CHARGE_CODE_VALUE") != null ? cRmAwbOc.Field<double>("OTHER_CHARGE_CODE_VALUE") : 0,
            OtherChargeVatCalculatedAmount = cRmAwbOc.Field<object>("VAT_CALC_AMT") != null ? cRmAwbOc.Field<double?>("VAT_CALC_AMT") : null,
            OtherChargeVatPercentage = cRmAwbOc.Field<object>("VAT_PER") != null ? cRmAwbOc.Field<double?>("VAT_PER") : null,
            OtherChargeVatBaseAmount = cRmAwbOc.Field<object>("VAT_BASE_AMT") != null ? cRmAwbOc.Field<double?>("VAT_BASE_AMT") : null,
            OtherChargeVatText = cRmAwbOc.Field<string>("VAT_TEXT"),
            OtherChargeVatLabel = cRmAwbOc.Field<string>("VAT_LABEL")

        });

        #endregion

        #region Cargo Billing Memo Materializers

        public  readonly Materializer<CargoBillingMemo> CargoBillingMemoMaterializer = new Materializer<CargoBillingMemo>(bm =>
        new CargoBillingMemo
        {
            Id = bm.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(bm.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
            InvoiceId = bm.Field<byte[]>("INVOICE_ID") != null ? new Guid(bm.Field<byte[]>("INVOICE_ID")) : new Guid(),
            BatchSequenceNumber = bm.Field<object>("BATCH_SEQ_NO") != null ? bm.Field<int>("BATCH_SEQ_NO") : 0,
            RecordSequenceWithinBatch = bm.Field<object>("BATCH_REC_SEQ_NO") != null ? bm.Field<int>("BATCH_REC_SEQ_NO") : 0,
            BillingMemoNumber = bm.Field<string>("BILLING_MEMO_NO"),
            ReasonCode = bm.Field<string>("REASON_CODE"),
            OurRef = bm.Field<object>("OUR_REF") != null ? bm.Field<string>("OUR_REF") : null,
            CorrespondenceReferenceNumber = bm.Field<object>("CORRESPONDENCE_REF_NO") != null ? Convert.ToInt64(bm.Field<object>("CORRESPONDENCE_REF_NO")) : 0,
            YourInvoiceNumber = bm.Field<object>("YOUR_INVOICE_NO") != null ? bm.Field<string>("YOUR_INVOICE_NO") : null,
            BilledTotalWeightCharge = bm.Field<object>("TOTAL_WEIGHT_CHARGES") != null ? (decimal?)Convert.ToDecimal(bm.Field<object>("TOTAL_WEIGHT_CHARGES")) : null,
            BilledTotalValuationAmount = bm.Field<object>("TOTAL_VALUATION_AMT") != null ? (decimal?)Convert.ToDecimal(bm.Field<object>("TOTAL_VALUATION_AMT")) : null,
            BilledTotalOtherChargeAmount = bm.Field<object>("TOTAL_OTHER_CHARGE_AMT") != null ? (decimal)bm.Field<double>("TOTAL_OTHER_CHARGE_AMT") : 0,
            BilledTotalIscAmount = bm.Field<object>("TOTAL_ISC_AMT") != null ? (decimal)bm.Field<double>("TOTAL_ISC_AMT") : 0,
            BilledTotalVatAmount = bm.Field<object>("TOTAL_VAT_AMT") != null ? (decimal?)Convert.ToDecimal(bm.Field<object>("TOTAL_VAT_AMT")) : null,
            NetBilledAmount = bm.Field<object>("NET_BILLED_AMT") != null ? (decimal?)Convert.ToDecimal(bm.Field<double>("NET_BILLED_AMT")) : null,
            AttachmentIndicatorOriginal = bm.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (bm.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? false : true) : false,
            AttachmentIndicatorValidated = bm.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (bm.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
            NumberOfAttachments = bm.Field<object>("ATTACHMENT_NO") != null ? bm.Field<int?>("ATTACHMENT_NO") : null,
            AirlineOwnUse = bm.Field<object>("AIRLINE_OWN_USE") != null ? bm.Field<string>("AIRLINE_OWN_USE") : null,
            ISValidationFlag = bm.Field<object>("IS_VALIDATION_FLAG") != null ? bm.Field<string>("IS_VALIDATION_FLAG") : null,
            ReasonRemarks = bm.Field<object>("REASON_REMARKS") != null ? bm.Field<string>("REASON_REMARKS") : null,
            LastUpdatedBy = bm.Field<object>("LAST_UPDATED_BY") != null ? bm.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = bm.Field<object>("LAST_UPDATED_ON") != null ? bm.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            YourInvoiceBillingYear = bm.Field<object>("YOUR_INVOICE_BILLING_YEAR") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_YEAR") : 0,
            YourInvoiceBillingMonth = bm.Field<object>("YOUR_INVOICE_BILLING_MONTH") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_MONTH") : 0,
            YourInvoiceBillingPeriod = bm.Field<object>("YOUR_INVOICE_BILLING_PERIOD") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_PERIOD") : 0,
            BillingCode = bm.Field<object>("BILLING_CODE") != null ? bm.Field<int>("BILLING_CODE") : 0,
        });

        public  readonly Materializer<CargoBillingMemoVat> CargoBillingMemoVatMaterializer = new Materializer<CargoBillingMemoVat>(r =>
        new CargoBillingMemoVat
        {
            Id = r.Field<byte[]>("BM_VAT_ID") != null ? new Guid(r.Field<byte[]>("BM_VAT_ID")) : new Guid(),
            ParentId = r.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(r.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
            VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
            VatLabel = r.Field<string>("VAT_LABEL"),
            VatText = r.Field<string>("VAT_TEXT"),
            VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
            VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
            VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
            LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,

        });

        public  readonly Materializer<CargoBillingMemoAttachment> CargoBillingMemoAttachmentMaterializer = new Materializer<CargoBillingMemoAttachment>(bma =>
            new CargoBillingMemoAttachment
            {
                Id = bma.Field<byte[]>("BM_ATTACHMENT_ID") != null ? new Guid(bma.Field<byte[]>("BM_ATTACHMENT_ID")) : new Guid(),
                ParentId = bma.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(bma.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
                OriginalFileName = bma.Field<string>("FILE_NAME"),
                FileSize = bma.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(bma.Field<object>("FILE_SIZE")) : 0,
                FileTypeId = bma.Field<object>("FILE_TYPE_ID") != null ? bma.Field<int>("FILE_TYPE_ID") : 0,
                FileStatusId = bma.Field<object>("FILE_STATUS_ID") != null ? bma.Field<int>("FILE_STATUS_ID") : 0,
                LastUpdatedBy = bma.Field<object>("LAST_UPDATED_BY") != null ? bma.Field<int>("LAST_UPDATED_BY") : 0,
                LastUpdatedOn = bma.Field<object>("LAST_UPDATED_ON") != null ? bma.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
                FilePath = bma.Field<string>("FILE_PATH"),
                ServerId = bma.Field<object>("SERVER_ID") != null ? bma.Field<int>("SERVER_ID") : 0,
                IsFullPath = bma.Field<object>("IS_FULL_PATH") != null ? (bma.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
            });

        public  readonly Materializer<CargoBillingMemoAwb> CargoBillingMemoAwbMaterializer = new Materializer<CargoBillingMemoAwb>(p1 => new CargoBillingMemoAwb
        {
            Id = p1.Field<byte[]>("BM_AWB_ID") != null ? new Guid(p1.Field<byte[]>("BM_AWB_ID")) : new Guid(),
            LastUpdatedOn = p1.Field<object>("LAST_UPDATED_ON") != null ? p1.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = p1.Field<object>("LAST_UPDATED_BY") != null ? p1.Field<int>("LAST_UPDATED_BY") : 0,
            BillingMemoId = p1.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(p1.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
            BdSerialNumber = p1.Field<object>("BD_SR_NO") != null ? p1.Field<int>("BD_SR_NO") : 0,
            AwbBillingCode = p1.Field<object>("AWB_BILLING_CODE") != null ? p1.Field<int>("AWB_BILLING_CODE") : 0,
            AwbDate = p1.Field<object>("AWB_DATE") != null ? p1.Field<DateTime?>("AWB_DATE") : null,
            AwbIssueingAirline = p1.Field<string>("AWB_ISSUING_AIRLINE"),
            AwbSerialNumber = p1.Field<object>("AWB_SR_NO") != null ? p1.Field<int>("AWB_SR_NO") : 0,
            AwbCheckDigit = p1.Field<object>("AWB_CHECK_DIGIT") != null ? p1.Field<int>("AWB_CHECK_DIGIT") : 0,
            ConsignmentOriginId = p1.Field<string>("CONSIGNMENT_ORIGIN_ID"),
            ConsignmentDestinationId = p1.Field<string>("CONSIGNMENT_DESTINATION_ID"),
            CarriageFromId = p1.Field<string>("CARRIAGE_FROM_ID"),
            CarriageToId = p1.Field<string>("CARRIAGE_TO_ID"),
            TransferDate = p1.Field<object>("TRANSFER_DATE") != null ? p1.Field<DateTime?>("TRANSFER_DATE") : null,
            BilledWeightCharge = p1.Field<object>("WEIGHT_CHARGES") != null ? p1.Field<double?>("WEIGHT_CHARGES") : null,
            BilledValuationCharge = p1.Field<object>("VALUATION_CHARGES") != null ? p1.Field<double?>("VALUATION_CHARGES") : null,
            BilledOtherCharge = p1.Field<object>("OTHER_CHARGES_AMT") != null ? p1.Field<double>("OTHER_CHARGES_AMT") : 0,
            BilledAmtSubToIsc = p1.Field<object>("BIILED_AMT_SUB_TO_ISC") != null ? p1.Field<double>("BIILED_AMT_SUB_TO_ISC") : 0,
            BilledIscPercentage = p1.Field<object>("ISC_PER") != null ? p1.Field<double>("ISC_PER") : 0,
            BilledIscAmount = p1.Field<object>("ISC_AMT") != null ? p1.Field<double>("ISC_AMT") : 0,
            BilledVatAmount = p1.Field<object>("VAT_AMT") != null ? p1.Field<double?>("VAT_AMT") : null,
            TotalAmount = p1.Field<object>("TOTAL_AMT") != null ? p1.Field<double>("TOTAL_AMT") : 0,
            CurrencyAdjustmentIndicator = p1.Field<string>("CURRENCY_ADJUSTMENT_IND"),
            BilledWeight = p1.Field<object>("BILLED_WEIGHT") != null ? p1.Field<int?>("BILLED_WEIGHT") : null,
            // OurReference = p1.Field<string>("OUR_REFERENCE"),
            ProvisionalReqSpa = p1.Field<string>("PROVISO_REQ_SPA"),
            PrpratePercentage = p1.Field<object>("PRORATE_PER") != null ? p1.Field<int?>("PRORATE_PER") : null,
            PartShipmentIndicator = p1.Field<string>("PART_SHIPMENT_IND"),
            KgLbIndicator = p1.Field<string>("KG_LB_INDICATOR"),
            CcaIndicator = p1.Field<object>("CCA_INDICATOR") != null ? (p1.Field<int>("CCA_INDICATOR") == 0 ? false : true) : false,
            AttachmentIndicatorOriginal = p1.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (p1.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? false : true) : false,
            AttachmentIndicatorValidated = p1.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (p1.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
            NumberOfAttachments = p1.Field<object>("ATTACHMENT_NO") != null ? p1.Field<int?>("ATTACHMENT_NO") : null,
            ISValidationFlag = p1.Field<string>("IS_VALIDATION_FLAG"),
            ReasonCode = p1.Field<string>("REASON_CODE"),
            ReferenceField1 = p1.Field<string>("REFERENCE_FIELD_1"),
            ReferenceField2 = p1.Field<string>("REFERENCE_FIELD_2"),
            ReferenceField3 = p1.Field<string>("REFERENCE_FIELD_3"),
            ReferenceField4 = p1.Field<string>("REFERENCE_FIELD_4"),
            ReferenceField5 = p1.Field<string>("REFERENCE_FIELD_5"),
            AirlineOwnUse = p1.Field<string>("AIRLINE_OWN_USE"),
            //SCP:53226 - Prorate Ladder information Currency not being displayed Correctly in PDF Rejection Memo generated by SIS - Air Calin
            // check if ProrateCalCurrencyId is null then replace with empty string.
            ProrateCalCurrencyId = p1.Field<string>("PRORATE_CALC_CURRENCY_ID")??string.Empty,
            TotalProrateAmount = p1.Field<object>("TOTAL_PRORATE_AMOUNT") != null ? p1.Field<double?>("TOTAL_PRORATE_AMOUNT") : null,
        });

        public  readonly Materializer<BMAwbOtherCharge> BMAwbOtherChargeMaterializer = new Materializer<BMAwbOtherCharge>(cBmAwbOc => new BMAwbOtherCharge
        {
            Id = cBmAwbOc.Field<byte[]>("BM_AWB_OTHER_CHARGE_ID") != null ? new Guid(cBmAwbOc.Field<byte[]>("BM_AWB_OTHER_CHARGE_ID")) : new Guid(),
            LastUpdatedBy = cBmAwbOc.Field<object>("LAST_UPDATED_BY") != null ? cBmAwbOc.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cBmAwbOc.Field<object>("LAST_UPDATED_ON") != null ? cBmAwbOc.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            ParentId = cBmAwbOc.Field<byte[]>("BM_AWB_ID") != null ? new Guid(cBmAwbOc.Field<byte[]>("BM_AWB_ID")) : new Guid(),
            OtherChargeCode = cBmAwbOc.Field<string>("OTHER_CHARGE_CODE"),
            OtherChargeCodeValue = cBmAwbOc.Field<object>("OTHER_CHARGE_CODE_VALUE") != null ? cBmAwbOc.Field<double>("OTHER_CHARGE_CODE_VALUE") : 0.0,
            OtherChargeVatCalculatedAmount = cBmAwbOc.Field<object>("VAT_CALC_AMT") != null ? cBmAwbOc.Field<double?>("VAT_CALC_AMT") : null,
            OtherChargeVatPercentage = cBmAwbOc.Field<object>("VAT_PER") != null ? cBmAwbOc.Field<double?>("VAT_PER") : null,
            OtherChargeVatBaseAmount = cBmAwbOc.Field<object>("VAT_BASE_AMT") != null ? cBmAwbOc.Field<double?>("VAT_BASE_AMT") : null,
            OtherChargeVatText = cBmAwbOc.Field<string>("VAT_TEXT"),
            OtherChargeVatLabel = cBmAwbOc.Field<string>("VAT_LABEL"),

        });

        

        public  readonly Materializer<BMAwbProrateLadderDetail> BMAwbProrateLadderDetailMaterializer = new Materializer<BMAwbProrateLadderDetail>(cBmawbPLadder => new BMAwbProrateLadderDetail
        {
            LastUpdatedBy = cBmawbPLadder.Field<object>("LAST_UPDATED_BY") != null ? cBmawbPLadder.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cBmawbPLadder.Field<object>("LAST_UPDATED_ON") != null ? cBmawbPLadder.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            ParentId = cBmawbPLadder.Field<byte[]>("BM_AWB_ID") != null ? new Guid(cBmawbPLadder.Field<byte[]>("BM_AWB_ID")) : new Guid(),
            FromSector = cBmawbPLadder.Field<string>("FROM_SECTOR_ID"),
            ToSector = cBmawbPLadder.Field<string>("TO_SECTOR_ID"),
            CarrierPrefix = cBmawbPLadder.Field<string>("CARRIER_PREFIX"),
            ProvisoReqSpa = cBmawbPLadder.Field<string>("PROVISO_REQ_SPA"),
            ProrateFactor =  cBmawbPLadder.Field<object>("PRORATE_FACTOR") != null ? (long?)Convert.ToInt64(cBmawbPLadder.Field<object>("PRORATE_FACTOR")) : null,
            PercentShare = cBmawbPLadder.Field<object>("PERCENT_SHARE") != null ? cBmawbPLadder.Field<double?>("PERCENT_SHARE") : null,
            Amount = cBmawbPLadder.Field<object>("AMOUNT") != null ? cBmawbPLadder.Field<double?>("AMOUNT") : null,
            SequenceNumber = cBmawbPLadder.Field<object>("SEQUENCE_NO") != null ? cBmawbPLadder.Field<int?>("SEQUENCE_NO") : null,
            Id = cBmawbPLadder.Field<byte[]>("BM_AWB_LADDER_DETAIL_ID") != null ? new Guid(cBmawbPLadder.Field<byte[]>("BM_AWB_LADDER_DETAIL_ID")) : new Guid()
        });

        public  readonly Materializer<BMAwbVat> BMAwbVatMaterializer = new Materializer<BMAwbVat>(cBmAwbVat => new BMAwbVat
        {
            LastUpdatedOn = cBmAwbVat.Field<object>("LAST_UPDATED_ON") != null ? cBmAwbVat.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cBmAwbVat.Field<object>("LAST_UPDATED_BY") != null ? cBmAwbVat.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cBmAwbVat.Field<byte[]>("BM_AWB_ID") != null ? new Guid(cBmAwbVat.Field<byte[]>("BM_AWB_ID")) : new Guid(),
            VatCalculatedAmount = cBmAwbVat.Field<object>("VAT_CALC_AMT") != null ? cBmAwbVat.Field<double>("VAT_CALC_AMT") : 0,
            VatPercentage = cBmAwbVat.Field<object>("VAT_PER") != null ? cBmAwbVat.Field<double>("VAT_PER") : 0,
            VatBaseAmount = cBmAwbVat.Field<object>("VAT_BASE_AMT") != null ? cBmAwbVat.Field<double>("VAT_BASE_AMT") : 0,
            VatText = cBmAwbVat.Field<object>("VAT_TEXT") != null ? cBmAwbVat.Field<string>("VAT_TEXT") : null,
            VatLabel = cBmAwbVat.Field<object>("VAT_LABEL") != null ? cBmAwbVat.Field<string>("VAT_LABEL") : null,
            VatIdentifierId = cBmAwbVat.Field<object>("VAT_IDENTIFIER_ID") != null ? cBmAwbVat.Field<int>("VAT_IDENTIFIER_ID") : 0,
            Id = cBmAwbVat.Field<byte[]>("AWB_VAT_ID") != null ? new Guid(cBmAwbVat.Field<byte[]>("AWB_VAT_ID")) : new Guid(),
        });

        public  readonly Materializer<BMAwbAttachment> BMAwbAttachmentMaterializer = new Materializer<BMAwbAttachment>(cBmAwbA => new BMAwbAttachment
        {
            ServerId = cBmAwbA.Field<object>("SERVER_ID") != null ? cBmAwbA.Field<int>("SERVER_ID") : 0,
            FilePath = cBmAwbA.Field<string>("FILE_PATH"),
            OriginalFileName = cBmAwbA.Field<string>("FILE_NAME"),
            LastUpdatedOn = cBmAwbA.Field<object>("LAST_UPDATED_ON") != null ? cBmAwbA.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cBmAwbA.Field<object>("LAST_UPDATED_BY") != null ? cBmAwbA.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cBmAwbA.Field<byte[]>("BM_AWB_ID") != null ? (Guid?)new Guid(cBmAwbA.Field<byte[]>("BM_AWB_ID")) : null,
            FileTypeId = cBmAwbA.Field<object>("FILE_TYPE_ID") != null ? cBmAwbA.Field<int>("FILE_TYPE_ID") : 0,
            FileSize = cBmAwbA.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(cBmAwbA.Field<object>("FILE_SIZE")) : 0,
            IsFullPath = cBmAwbA.Field<object>("IS_FULL_PATH") != null ? (cBmAwbA.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
            Id = cBmAwbA.Field<byte[]>("BM_AWB_ATTACHMENT_ID") != null ? new Guid(cBmAwbA.Field<byte[]>("BM_AWB_ATTACHMENT_ID")) : new Guid(),
        });

        #endregion

        #region Cargo Credit Memo Materializers

        public readonly Materializer<CargoCreditMemo> CargoCreditMemoMaterializer = new Materializer<CargoCreditMemo>(cCreditMemo => new CargoCreditMemo
        {
            Id = cCreditMemo.Field<byte[]>("CREDIT_MEMO_ID") != null ? new Guid(cCreditMemo.Field<byte[]>("CREDIT_MEMO_ID")) : new Guid(),
            InvoiceId = cCreditMemo.Field<byte[]>("INVOICE_ID") != null ? new Guid(cCreditMemo.Field<byte[]>("INVOICE_ID")) : new Guid(),
            ReasonCode = cCreditMemo.Field<string>("REASON_CODE"),
            LastUpdatedBy = cCreditMemo.Field<object>("LAST_UPDATED_BY") != null ? cCreditMemo.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cCreditMemo.Field<object>("LAST_UPDATED_ON") != null ? cCreditMemo.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            YourInvoiceBillingPeriod = cCreditMemo.Field<object>("YOUR_INVOICE_BILLING_PERIOD") != null ? cCreditMemo.Field<int>("YOUR_INVOICE_BILLING_PERIOD") : 0,
            YourInvoiceBillingMonth = cCreditMemo.Field<object>("YOUR_INVOICE_BILLING_MONTH") != null ? cCreditMemo.Field<int>("YOUR_INVOICE_BILLING_MONTH") : 0,
            YourInvoiceBillingYear = cCreditMemo.Field<object>("YOUR_INVOICE_BILLING_YEAR") != null ? cCreditMemo.Field<int>("YOUR_INVOICE_BILLING_YEAR") : 0,
            ReasonRemarks = cCreditMemo.Field<string>("REASON_REMARKS"),
            ISValidationFlag = cCreditMemo.Field<string>("IS_VALIDATION_FLAG"),
            AirlineOwnUse = cCreditMemo.Field<string>("AIRLINE_OWN_USE"),
            AttachmentIndicatorOriginal = cCreditMemo.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (cCreditMemo.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0) ? false : true : false,
            AttachmentIndicatorValidated = cCreditMemo.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (cCreditMemo.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0) ? (bool?)false : (bool?)true : null,
            NumberOfAttachments = cCreditMemo.Field<object>("ATTACHMENT_NO") != null ? cCreditMemo.Field<int?>("ATTACHMENT_NO") : null,
            NetAmountCredited = cCreditMemo.Field<object>("NET_AMT") != null ? (decimal?)Convert.ToDecimal(cCreditMemo.Field<object>("NET_AMT")) : null,
            TotalWeightCharges = cCreditMemo.Field<object>("TOTAL_WEIGHT_CHARGES") != null ? (decimal?)Convert.ToDecimal(cCreditMemo.Field<object>("TOTAL_WEIGHT_CHARGES")) : null,
            TotalValuationAmt = cCreditMemo.Field<object>("TOTAL_VALUATION_AMT") != null ? (decimal?)Convert.ToDecimal(cCreditMemo.Field<object>("TOTAL_VALUATION_AMT")) : null,
            TotalOtherChargeAmt = cCreditMemo.Field<object>("TOTAL_OTHER_CHARGE_AMT") != null ? Convert.ToDecimal(cCreditMemo.Field<object>("TOTAL_OTHER_CHARGE_AMT")) : 0,
            TotalIscAmountCredited = cCreditMemo.Field<object>("TOTAL_ISC_AMT") != null ? Convert.ToDecimal(cCreditMemo.Field<object>("TOTAL_ISC_AMT")) : 0,
            TotalVatAmountCredited = cCreditMemo.Field<object>("TOTAL_VAT_AMT") != null ? (decimal?)Convert.ToDecimal(cCreditMemo.Field<object>("TOTAL_VAT_AMT")) : null,
            YourInvoiceNumber = cCreditMemo.Field<string>("YOUR_INVOICE_NO"),
            CorrespondenceRefNumber = cCreditMemo.Field<object>("CORRESPONDENCE_REF_NO") != null ? Convert.ToInt64(cCreditMemo.Field<object>("CORRESPONDENCE_REF_NO")) : 0,
            OurRef = cCreditMemo.Field<string>("OUR_REF"),
            CreditMemoNumber = cCreditMemo.Field<string>("CREDIT_MEMO_NO"),
            RecordSequenceWithinBatch = cCreditMemo.Field<object>("BATCH_REC_SEQ_NO") != null ? cCreditMemo.Field<int>("BATCH_REC_SEQ_NO") : 0,
            BatchSequenceNumber = cCreditMemo.Field<object>("BATCH_SEQ_NO") != null ? cCreditMemo.Field<int>("BATCH_SEQ_NO") : 0,
            BillingCode = cCreditMemo.Field<object>("BILLING_CODE") != null ? cCreditMemo.Field<int>("BILLING_CODE") : 0

        });

        public  readonly Materializer<CargoCreditMemoVat> CargoCreditMemoVatMaterializer = new Materializer<CargoCreditMemoVat>(ccreditMemobVat => new CargoCreditMemoVat
        {
            Id = ccreditMemobVat.Field<byte[]>("CM_VAT_ID") != null ? new Guid(ccreditMemobVat.Field<byte[]>("CM_VAT_ID")) : new Guid(),
            ParentId = ccreditMemobVat.Field<byte[]>("CREDIT_MEMO_ID") != null ? new Guid(ccreditMemobVat.Field<byte[]>("CREDIT_MEMO_ID")) : new Guid(),
            LastUpdatedBy = ccreditMemobVat.Field<object>("LAST_UPDATED_BY") != null ? ccreditMemobVat.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = ccreditMemobVat.Field<object>("LAST_UPDATED_ON") != null ? ccreditMemobVat.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            VatCalculatedAmount = ccreditMemobVat.Field<object>("VAT_CALC_AMT") != null ? ccreditMemobVat.Field<double>("VAT_CALC_AMT") : 0,
            VatPercentage = ccreditMemobVat.Field<object>("VAT_PER") != null ? ccreditMemobVat.Field<double>("VAT_PER") : 0,
            VatBaseAmount = ccreditMemobVat.Field<object>("VAT_BASE_AMT") != null ? ccreditMemobVat.Field<double>("VAT_BASE_AMT") : 0,
            VatText = ccreditMemobVat.Field<object>("VAT_TEXT") != null ? ccreditMemobVat.Field<string>("VAT_TEXT") : null,
            VatLabel = ccreditMemobVat.Field<object>("VAT_LABEL") != null ? ccreditMemobVat.Field<string>("VAT_LABEL") : null,
            VatIdentifierId = ccreditMemobVat.Field<object>("VAT_IDENTIFIER_ID") != null ? ccreditMemobVat.Field<int>("VAT_IDENTIFIER_ID") : 0

        });

        public  readonly Materializer<CargoCreditMemoAttachment> CargoCreditMemoAttachmentMaterializer = new Materializer<CargoCreditMemoAttachment>(cAwbA => new CargoCreditMemoAttachment
        {
            OriginalFileName = cAwbA.Field<string>("FILE_NAME"),
            FilePath = cAwbA.Field<string>("FILE_PATH"),
            LastUpdatedOn = cAwbA.Field<object>("LAST_UPDATED_ON") != null ? cAwbA.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cAwbA.Field<object>("LAST_UPDATED_BY") != null ? cAwbA.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cAwbA.Field<byte[]>("CREDIT_MEMO_ID") != null ? (Guid?)new Guid(cAwbA.Field<byte[]>("CREDIT_MEMO_ID")) : null,
            FileTypeId = cAwbA.Field<object>("FILE_TYPE_ID") != null ? cAwbA.Field<int>("FILE_TYPE_ID") : 0,
            FileSize = cAwbA.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(cAwbA.Field<object>("FILE_SIZE")) : 0,
            FileStatusId = cAwbA.Field<object>("FILE_STATUS_ID") != null ? cAwbA.Field<int>("FILE_STATUS_ID") : 0,
            Id = cAwbA.Field<byte[]>("CM_ATTACHMENTS_ID") != null ? new Guid(cAwbA.Field<byte[]>("CM_ATTACHMENTS_ID")) : new Guid(),
        });

        public  readonly Materializer<CMAirWayBill> CmAirWayBillMaterializer = new Materializer<CMAirWayBill>(cm =>
              new CMAirWayBill
              {
                  Id = cm.Field<byte[]>("CM_AWB_ID") != null ? new Guid(cm.Field<byte[]>("CM_AWB_ID")) : new Guid(),
                  LastUpdatedOn = cm.Field<object>("LAST_UPDATED_ON") != null ? cm.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
                  LastUpdatedBy = cm.Field<object>("LAST_UPDATED_BY") != null ? cm.Field<int>("LAST_UPDATED_BY") : 0,
                  CreditMemoId = cm.Field<byte[]>("CREDIT_MEMO_ID") != null ? new Guid(cm.Field<byte[]>("CREDIT_MEMO_ID")) : new Guid(),
                  BdSerialNumber = cm.Field<object>("BD_SR_NO") != null ? cm.Field<int>("BD_SR_NO") : 0,
                  AwbBillingCode = cm.Field<object>("AWB_BILLING_CODE") != null ? cm.Field<int>("AWB_BILLING_CODE") : 0,
                  AwbDate = cm.Field<object>("LAST_UPDATED_ON") != null ? cm.Field<DateTime?>("LAST_UPDATED_ON") : null,
                  AwbIssueingAirline = cm.Field<string>("AWB_ISSUING_AIRLINE"),
                  AwbSerialNumber = cm.Field<object>("AWB_SR_NO") != null ? cm.Field<int>("AWB_SR_NO") : 0,
                  AwbCheckDigit = cm.Field<object>("AWB_CHECK_DIGIT") != null ? cm.Field<int>("AWB_CHECK_DIGIT") : 0,
                  ConsignmentOriginId = cm.Field<string>("CONSIGNMENT_ORIGIN"),
                  ConsignmentDestinationId = cm.Field<string>("CONSIGNMENT_DEST"),
                  CarriageFromId = cm.Field<string>("CARRIAGE_FROM"),
                  CarriageToId = cm.Field<string>("CARRIAGE_TO"),
                  TransferDate = cm.Field<object>("TRANSFER_DATE") != null ? cm.Field<DateTime?>("TRANSFER_DATE") : null,
                  CreditedWeightCharge = cm.Field<object>("WEIGHT_CHARGES") != null ? cm.Field<double?>("WEIGHT_CHARGES") : null,
                  CreditedValuationCharge = cm.Field<object>("VALUATION_CHARGES") != null ? cm.Field<double?>("VALUATION_CHARGES") : null,
                  CreditedOtherCharge = cm.Field<object>("OTHER_CHARGES_AMT") != null ? cm.Field<double>("OTHER_CHARGES_AMT") : 0,
                  CreditedAmtSubToIsc = cm.Field<object>("CREDITED_AMT_SUB_TO_ISC") != null ? cm.Field<double>("CREDITED_AMT_SUB_TO_ISC") : 0,
                  CreditedIscPercentage = cm.Field<object>("ISC_PER") != null ? cm.Field<double>("ISC_PER") : 0,
                  CreditedIscAmount = cm.Field<object>("ISC_AMT") != null ? cm.Field<double>("ISC_AMT") : 0,
                  CreditedVatAmount = cm.Field<object>("VAT_AMT") != null ? cm.Field<double?>("VAT_AMT") : null,
                  TotalAmountCredited = cm.Field<object>("TOTAL_CREDITED_AMT") != null ? cm.Field<double>("TOTAL_CREDITED_AMT") : 0,
                  CurrencyAdjustmentIndicator = cm.Field<string>("CURRENCY_ADJUSTMENT_IND"),
                  BilledWeight = cm.Field<object>("BILLED_WEIGHT") != null ? cm.Field<int?>("BILLED_WEIGHT") : null,
                  ProvisionalReqSpa = cm.Field<string>("PROVISO_REQ_SPA"),
                  ProratePercentage = cm.Field<object>("PRORATE_PER") != null ? cm.Field<int?>("PRORATE_PER") : null,
                  PartShipmentIndicator = cm.Field<string>("PART_SHIPMENT_IND"),
                  KgLbIndicator = cm.Field<string>("KG_LB_INDICATOR"),
                  CcaIndicator = cm.Field<object>("CCA_INDICATOR") != null ? (cm.Field<int>("CCA_INDICATOR") == 0 ? false : true) : false,
                  AttachmentIndicatorOriginal = cm.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (cm.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? false : true) : false,
                  AttachmentIndicatorValidated = cm.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (cm.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
                  NumberOfAttachments = cm.Field<object>("ATTACHMENT_NO") != null ? cm.Field<int?>("ATTACHMENT_NO") : null,
                  ISValidationFlag = cm.Field<string>("IS_VALIDATION_FLAG"),
                  ReasonCode = cm.Field<string>("REASON_CODE"),
                  ReferenceField1 = cm.Field<string>("REFERENCE_FIELD_1"),
                  ReferenceField2 = cm.Field<string>("REFERENCE_FIELD_2"),
                  ReferenceField3 = cm.Field<string>("REFERENCE_FIELD_3"),
                  ReferenceField4 = cm.Field<string>("REFERENCE_FIELD_4"),
                  ReferenceField5 = cm.Field<string>("REFERENCE_FIELD_5"),
                  AirlineOwnUse = cm.Field<string>("AIRLINE_OWN_USE"),
                  //SCP:53226 - Prorate Ladder information Currency not being displayed Correctly in PDF Rejection Memo generated by SIS - Air Calin
                  // check if ProrateCalCurrencyId is null then replace with empty string.
                  ProrateCalCurrencyId = cm.Field<string>("PRORATE_CALC_CURRENCY_ID")??string.Empty,
                  TotalProrateAmount = cm.Field<object>("TOTAL_PRORATE_AMOUNT") != null ? cm.Field<double?>("TOTAL_PRORATE_AMOUNT") : null,
              });

        public  readonly Materializer<CMAwbAttachment> CmAwbAttachmentMaterializer = new Materializer<CMAwbAttachment>(cCMAwbA => new CMAwbAttachment
        {
            ServerId = cCMAwbA.Field<object>("SERVER_ID") != null ? cCMAwbA.Field<int>("SERVER_ID") : 0,
            FilePath = cCMAwbA.Field<string>("FILE_PATH"),
            OriginalFileName = cCMAwbA.Field<string>("FILE_NAME"),
            LastUpdatedOn = cCMAwbA.Field<object>("LAST_UPDATED_ON") != null ? cCMAwbA.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cCMAwbA.Field<object>("LAST_UPDATED_BY") != null ? cCMAwbA.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cCMAwbA.Field<byte[]>("CM_AWB_ID") != null ? (Guid?)new Guid(cCMAwbA.Field<byte[]>("CM_AWB_ID")) : null,
            FileTypeId = cCMAwbA.Field<object>("FILE_TYPE_ID") != null ? cCMAwbA.Field<int>("FILE_TYPE_ID") : 0,
            FileStatusId = cCMAwbA.Field<object>("FILE_STATUS_ID") != null ? cCMAwbA.Field<int>("FILE_STATUS_ID") : 0,
            FileSize = cCMAwbA.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(cCMAwbA.Field<object>("FILE_SIZE")) : 0,
            IsFullPath = cCMAwbA.Field<object>("IS_FULL_PATH") != null ? (cCMAwbA.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
            Id = cCMAwbA.Field<byte[]>("CM_AWB_ATTACHMENT_ID") != null ? new Guid(cCMAwbA.Field<byte[]>("CM_AWB_ATTACHMENT_ID")) : new Guid(),
        });

        public  readonly Materializer<CMAwbVat> CmAwbVatMaterializer = new Materializer<CMAwbVat>(cCMAwbVat => new CMAwbVat
        {
            LastUpdatedOn = cCMAwbVat.Field<object>("LAST_UPDATED_ON") != null ? cCMAwbVat.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cCMAwbVat.Field<object>("LAST_UPDATED_BY") != null ? cCMAwbVat.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cCMAwbVat.Field<byte[]>("CM_AWB_ID") != null ? new Guid(cCMAwbVat.Field<byte[]>("CM_AWB_ID")) : new Guid(),
            VatCalculatedAmount = cCMAwbVat.Field<object>("VAT_CALC_AMT") != null ? cCMAwbVat.Field<double>("VAT_CALC_AMT") : 0,
            VatPercentage = cCMAwbVat.Field<object>("VAT_PER") != null ? cCMAwbVat.Field<double>("VAT_PER") : 0,
            VatBaseAmount = cCMAwbVat.Field<object>("VAT_BASE_AMT") != null ? cCMAwbVat.Field<double>("VAT_BASE_AMT") : 0,
            VatText = cCMAwbVat.Field<object>("VAT_TEXT") != null ? cCMAwbVat.Field<string>("VAT_TEXT") : null,
            VatLabel = cCMAwbVat.Field<object>("VAT_LABEL") != null ? cCMAwbVat.Field<string>("VAT_LABEL") : null,
            VatIdentifierId = cCMAwbVat.Field<object>("VAT_IDENTIFIER_ID") != null ? cCMAwbVat.Field<int>("VAT_IDENTIFIER_ID") : 0,
            Id = cCMAwbVat.Field<byte[]>("CM_AWB_VAT_ID") != null ? new Guid(cCMAwbVat.Field<byte[]>("CM_AWB_VAT_ID")) : new Guid(),
        });

        public  readonly Materializer<CMAwbProrateLadderDetail> CMAwbProrateLadderDetailMaterializer = new Materializer<CMAwbProrateLadderDetail>(cCmawbPLadder => new CMAwbProrateLadderDetail
        {
          LastUpdatedBy = cCmawbPLadder.Field<object>("LAST_UPDATED_BY") != null ? cCmawbPLadder.Field<int>("LAST_UPDATED_BY") : 0,
          LastUpdatedOn = cCmawbPLadder.Field<object>("LAST_UPDATED_ON") != null ? cCmawbPLadder.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
          ParentId = cCmawbPLadder.Field<byte[]>("CM_AWB_ID") != null ? new Guid(cCmawbPLadder.Field<byte[]>("CM_AWB_ID")) : new Guid(),
          FromSector = cCmawbPLadder.Field<string>("FROM_SECTOR_ID"),
          ToSector = cCmawbPLadder.Field<string>("TO_SECTOR_ID"),
          CarrierPrefix = cCmawbPLadder.Field<string>("CARRIER_PREFIX"),
          ProvisoReqSpa = cCmawbPLadder.Field<string>("PROVISO_REQ_SPA"),
          ProrateFactor = cCmawbPLadder.Field<object>("PRORATE_FACTOR") != null ? (long?)Convert.ToInt64(cCmawbPLadder.Field<object>("PRORATE_FACTOR")) : null,
          PercentShare = cCmawbPLadder.Field<object>("PERCENT_SHARE") != null ? cCmawbPLadder.Field<double?>("PERCENT_SHARE") : null,
          Amount = cCmawbPLadder.Field<object>("AMOUNT") != null ? cCmawbPLadder.Field<double?>("AMOUNT") : null,
          SequenceNumber = cCmawbPLadder.Field<object>("SEQUENCE_NO") != null ? cCmawbPLadder.Field<int?>("SEQUENCE_NO") : null,
          Id = cCmawbPLadder.Field<byte[]>("CM_AWB_LADDER_DETAIL_ID") != null ? new Guid(cCmawbPLadder.Field<byte[]>("CM_AWB_LADDER_DETAIL_ID")) : new Guid()
        });

        public  readonly Materializer<CMAwbOtherCharge> CmAwbOtherChargeMaterializer = new Materializer<CMAwbOtherCharge>(cCMAwbOc => new CMAwbOtherCharge
        {

            Id = cCMAwbOc.Field<byte[]>("CM_AWB_OTHER_CHARGE_ID") != null ? new Guid(cCMAwbOc.Field<byte[]>("CM_AWB_OTHER_CHARGE_ID")) : new Guid(),
            ParentId = cCMAwbOc.Field<byte[]>("CM_AWB_ID") != null ? new Guid(cCMAwbOc.Field<byte[]>("CM_AWB_ID")) : new Guid(),
            //LastUpdatedBy = cCMAwbOc.Field<object>("LAST_UPDATED_BY") != null ? cCMAwbOc.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cCMAwbOc.Field<object>("LAST_UPDATED_ON") != null ? cCMAwbOc.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            OtherChargeCode = cCMAwbOc.Field<string>("OTHER_CHARGE_CODE"),
            OtherChargeCodeValue = cCMAwbOc.Field<object>("OTHER_CHARGE_CODE_VALUE") != null ? cCMAwbOc.Field<double?>("OTHER_CHARGE_CODE_VALUE") : null,
            OtherChargeVatCalculatedAmount = cCMAwbOc.Field<object>("VAT_CALC_AMT") != null ? cCMAwbOc.Field<double?>("VAT_CALC_AMT") : null,
            OtherChargeVatPercentage = cCMAwbOc.Field<object>("VAT_PER") != null ? cCMAwbOc.Field<double?>("VAT_PER") : null,
            OtherChargeVatBaseAmount = cCMAwbOc.Field<object>("VAT_BASE_AMT") != null ? cCMAwbOc.Field<double?>("VAT_BASE_AMT") : null,
            OtherChargeVatText = cCMAwbOc.Field<string>("VAT_TEXT"),
            OtherChargeVatLabel = cCMAwbOc.Field<string>("VAT_LABEL")

        });

        

        #endregion

        #region Cargo InvoiceTotal Materializers

        public  readonly Materializer<CargoInvoiceTotal> CargoInvoiceTotalMaterializer = new Materializer<CargoInvoiceTotal>(cInvTotal => new CargoInvoiceTotal
        {
            LastUpdatedOn = cInvTotal.TryGetField<object>("LAST_UPDATED_ON") != null ? cInvTotal.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cInvTotal.TryGetField<object>("LAST_UPDATED_BY") != null ? cInvTotal.Field<int>("LAST_UPDATED_BY") : 0,
            Id = cInvTotal.TryGetField<byte[]>("INVOICE_ID") != null ? new Guid(cInvTotal.Field<byte[]>("INVOICE_ID")) : new Guid(),
            BatchSequenceNumber = cInvTotal.TryGetField<object>("BATCH_SEQ_NO") != null ? cInvTotal.Field<int>("BATCH_SEQ_NO") : 0,
            RecordSequenceWithinBatch = cInvTotal.TryGetField<object>("BATCH_REC_SEQ_NO") != null ? cInvTotal.Field<int>("BATCH_REC_SEQ_NO") : 0,
            TotalWeightCharge = cInvTotal.TryGetField<object>("TOTAL_WEIGHT_CHARGES") != null ? Convert.ToDecimal(cInvTotal.Field<object>("TOTAL_WEIGHT_CHARGES")) : 0,
            TotalOtherCharge = cInvTotal.TryGetField<object>("TOTAL_OTHER_CHARGES") != null ? Convert.ToDecimal(cInvTotal.Field<object>("TOTAL_OTHER_CHARGES")) : 0,
            TotalIscAmount = cInvTotal.TryGetField<object>("TOT_INTERLINE_SERV_CHARGE_AMT") != null ? Convert.ToDecimal(cInvTotal.Field<object>("TOT_INTERLINE_SERV_CHARGE_AMT")) : 0,
            NetTotal = cInvTotal.TryGetField<object>("NET_INVOICE_TOTAL") != null ? Convert.ToDecimal(cInvTotal.Field<object>("NET_INVOICE_TOTAL")) : 0,
            NetBillingAmount = cInvTotal.TryGetField<object>("NET_INVOICE_BILLING_TOTAL") != null ? Convert.ToDecimal(cInvTotal.Field<object>("NET_INVOICE_BILLING_TOTAL")) : 0,
            NoOfBillingRecords = cInvTotal.TryGetField<object>("TOTAL_BILLING_RECORDS_NO") != null ? cInvTotal.Field<int>("TOTAL_BILLING_RECORDS_NO") : 0,
            //   NetBillingAmount = r.TryGetField<object>("NET_BILLING_AMT") != null ? r.Field<decimal>("NET_BILLING_AMT") : 0,
            //   NetTotal = r.TryGetField<object>("NET_TOTAL") != null ? r.Field<decimal>("NET_TOTAL") : 0,
            TotalValuationCharge = cInvTotal.TryGetField<object>("TOTAL_VALUATION_CHARGES") != null ? Convert.ToDecimal(cInvTotal.Field<object>("TOTAL_VALUATION_CHARGES")) : 0,
            TotalVatAmount = cInvTotal.TryGetField<object>("TOTAL_VAT_AMOUNT") != null ? Convert.ToDecimal(cInvTotal.Field<object>("TOTAL_VAT_AMOUNT")) : 0,
            TotalNoOfRecords = cInvTotal.TryGetField<object>("TOTAL_NO_OF_RECORDS") != null ? cInvTotal.Field<int>("TOTAL_NO_OF_RECORDS") : 0,
            TotalNetAmountWithoutVat = cInvTotal.TryGetField<object>("TOTAL_NET_AMT_WITHOUT_VAT") != null ? Convert.ToDecimal(cInvTotal.Field<object>("TOTAL_NET_AMT_WITHOUT_VAT")) : 0,

        });

        public  readonly Materializer<CargoInvoiceTotalVat> CargoInvoiceTotalVatMaterializer = new Materializer<CargoInvoiceTotalVat>(cInvTotalVat => new CargoInvoiceTotalVat
        {
            Id = cInvTotalVat.Field<byte[]>("INVOICE_TOTAL_VAT_ID") != null ? new Guid(cInvTotalVat.Field<byte[]>("INVOICE_TOTAL_VAT_ID")) : new Guid(),
            ParentId = cInvTotalVat.Field<byte[]>("INVOICE_ID") != null ? new Guid(cInvTotalVat.Field<byte[]>("INVOICE_ID")) : new Guid(),
            LastUpdatedBy = cInvTotalVat.Field<object>("LAST_UPDATED_BY") != null ? cInvTotalVat.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = cInvTotalVat.Field<object>("LAST_UPDATED_ON") != null ? cInvTotalVat.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            VatCalculatedAmount = cInvTotalVat.Field<object>("VAT_CALC_AMT") != null ? cInvTotalVat.Field<double>("VAT_CALC_AMT") : 0,
            VatPercentage = cInvTotalVat.Field<object>("VAT_PER") != null ? cInvTotalVat.Field<double>("VAT_PER") : 0,
            VatBaseAmount = cInvTotalVat.Field<object>("VAT_BASE_AMT") != null ? cInvTotalVat.Field<double>("VAT_BASE_AMT") : 0,
            VatText = cInvTotalVat.Field<string>("VAT_TEXT"),
            VatLabel = cInvTotalVat.Field<string>("VAT_LABEL"),
            VatIdentifierId = cInvTotalVat.Field<object>("VAT_IDENTIFIER_ID") != null ? cInvTotalVat.Field<int>("VAT_IDENTIFIER_ID") : 0

        });

        #endregion

        #region Cargo Billing History Materializers

        public  readonly Materializer<CargoInvoice> CargoInvoiceAuditMaterializer = new Materializer<CargoInvoice>(r => new CargoInvoice
        {
            Id = r.TryGetField<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
            BillingPeriod = r.Field<object>("PERIOD_NO") != null ? r.Field<int>("PERIOD_NO") : 0,
            BillingMonth = r.Field<object>("BILLING_MONTH") != null ? r.Field<int>("BILLING_MONTH") : 0,
            BillingYear = r.Field<object>("BILLING_YEAR") != null ? r.Field<int>("BILLING_YEAR") : 0,
            BillingMemberId = r.Field<object>("BILLING_MEMBER_ID") != null ? r.Field<int>("BILLING_MEMBER_ID") : 0,
            BilledMemberId = r.Field<object>("BILLED_MEMBER_ID") != null ? r.Field<int>("BILLED_MEMBER_ID") : 0,
            InvoiceNumber = r.Field<string>("INVOICE_NO"),
            BillingCode = r.Field<object>("BILLING_CODE_ID") != null ? r.Field<int>("BILLING_CODE_ID") : 0,
            ListingCurrencyId = r.Field<object>("LISTING_CURRENCY_CODE_NUM") != null ? r.Field<int>("LISTING_CURRENCY_CODE_NUM") : 0
        });

        public  readonly Materializer<AwbRecord> CargoInvoiceAwbAuditMaterializer = new Materializer<AwbRecord>(r => new AwbRecord
        {
            Id = r.Field<byte[]>("AIR_WAY_BILL_ID") != null ? new Guid(r.Field<byte[]>("AIR_WAY_BILL_ID")) : new Guid(),
            AwbIssueingAirline = r.Field<object>("AWB_ISSUING_AIRLINE") != null ? r.Field<string>("AWB_ISSUING_AIRLINE") : string.Empty,
            AwbSerialNumber = r.Field<object>("AWB_SR_NO") != null ? Convert.ToInt32(r.Field<object>("AWB_SR_NO")) : 0,
            InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
            BatchSequenceNumber = r.Field<object>("BATCH_SEQ_NO") != null ? r.Field<int>("BATCH_SEQ_NO") : 0,
            RecordSequenceWithinBatch = r.Field<object>("BATCH_REC_SEQ_NO") != null ? r.Field<int>("BATCH_REC_SEQ_NO") : 0,
            IscPer = r.Field<object>("INTERLINE_SERV_CHARGE_PER") != null ? r.Field<double>("INTERLINE_SERV_CHARGE_PER") : 0.0,
            IscAmount = r.Field<object>("INTERLINE_SERV_CHARGE_AMT") != null ? r.Field<double>("INTERLINE_SERV_CHARGE_AMT") : 0.0,
            OtherCharges = r.Field<object>("OTHER_CHARGES") != null ? r.Field<double>("OTHER_CHARGES") : 0.0,
            WeightCharges = r.Field<object>("WEIGHT_CHARGES") != null ? r.Field<double>("WEIGHT_CHARGES") : 0.0,
            VatAmount = r.Field<object>("VAT_AMT") != null ? r.Field<double>("VAT_AMT") : 0.0,
            AwbTotalAmount = r.Field<object>("AWB_TOTAL_AMT") != null ? r.Field<double>("AWB_TOTAL_AMT") : 0.0,
            CarriageFromId = r.Field<object>("CARRIAGE_FROM_ID") != null ? r.Field<string>("CARRIAGE_FROM_ID") : string.Empty,
            CarriageToId = r.Field<object>("CARRIAGE_TO_ID") != null ? r.Field<string>("CARRIAGE_TO_ID") : string.Empty,
            ValuationCharges = r.Field<object>("VALUATION_CHARGES") != null ? r.Field<double>("VALUATION_CHARGES") : 0.0,
            KgLbIndicator = r.Field<object>("KG_LB_INDICATOR") != null ? r.Field<string>("KG_LB_INDICATOR") : string.Empty,
        });

        public  readonly Materializer<AwbVat> AwbVatAuditMaterializer = new Materializer<AwbVat>(r =>
        new AwbVat
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

        public  readonly Materializer<AwbAttachment> AwbAttachmentMaterializer = new Materializer<AwbAttachment>(pca =>
        new AwbAttachment
        {
            ServerId = pca.Field<object>("SERVER_ID") != null ? pca.Field<int>("SERVER_ID") : 0,
            FilePath = pca.Field<string>("FILE_PATH"),
            OriginalFileName = pca.Field<string>("FILE_NAME"),
            LastUpdatedOn = pca.Field<object>("LAST_UPDATED_ON") != null ? pca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = pca.Field<object>("LAST_UPDATED_BY") != null ? pca.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = pca.Field<byte[]>("AIR_WAY_BILL_ID") != null ? new Guid(pca.Field<byte[]>("AIR_WAY_BILL_ID")) : new Guid(),
            FileStatusId = pca.Field<object>("FILE_STATUS_ID") != null ? pca.Field<int>("FILE_STATUS_ID") : 0,
            FileTypeId = pca.Field<object>("FILE_TYPE_ID") != null ? pca.Field<int>("FILE_TYPE_ID") : 0,
            FileSize = pca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(pca.Field<object>("FILE_SIZE")) : 0,
            IsFullPath = pca.Field<object>("IS_FULL_PATH") != null ? (pca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
            Id = pca.Field<byte[]>("AWB_RECORD_ATTACHMENT_ID") != null ? new Guid(pca.Field<byte[]>("AWB_RECORD_ATTACHMENT_ID")) : new Guid(),
        });

        public  readonly Materializer<CargoRejectionMemo> CargoInvoiceRejectionMemoAuditMaterializer = new Materializer<CargoRejectionMemo>(r =>
        new CargoRejectionMemo
        {
            Id = r.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(r.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
            InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
            BatchSequenceNumber = r.Field<object>("BATCH_SEQ_NO") != null ? r.Field<int>("BATCH_SEQ_NO") : 0,
            RecordSequenceWithinBatch = r.Field<object>("BATCH_REC_SEQ_NO") != null ? r.Field<int>("BATCH_REC_SEQ_NO") : 0,
            RejectionMemoNumber = r.Field<string>("REJECTION_MEMO_NO"),
            RejectionStage = r.Field<object>("REJECTION_STAGE") != null ? r.Field<int>("REJECTION_STAGE") : 0,
            ReasonCode = r.Field<string>("REASON_CODE"),
            AirlineOwnUse = r.Field<string>("AIRLINE_OWN_USE"),
            YourInvoiceNumber = r.Field<string>("YOUR_INVOICE_NO"),
            YourInvoiceBillingMonth = r.Field<object>("YOUR_INVOICE_BILLING_MONTH") != null ? r.Field<int>("YOUR_INVOICE_BILLING_MONTH") : 0,
            YourInvoiceBillingYear = r.Field<object>("YOUR_INVOICE_BILLING_YEAR") != null ? r.Field<int>("YOUR_INVOICE_BILLING_YEAR") : 0,
            YourInvoiceBillingPeriod = r.Field<object>("YOUR_INVOICE_BILLING_PERIOD") != null ? r.Field<int>("YOUR_INVOICE_BILLING_PERIOD") : 0,
            YourBillingMemoNumber = r.Field<string>("YOUR_BM_CM_NO"),
            BMCMIndicatorId = r.Field<object>("BM_CM_INDICATOR") != null ? r.Field<int>("BM_CM_INDICATOR") : 0,
            YourRejectionNumber = r.Field<string>("YOUR_REJECTION_MEMO_NO"),
            BilledTotalWeightCharge = r.Field<object>("BILLED_TOTAL_WEIGHT_CHARGE") != null ? Convert.ToDecimal(r.Field<object>("BILLED_TOTAL_WEIGHT_CHARGE")) : 0,
            AcceptedTotalWeightCharge = r.Field<object>("ACCEPTED_TOTAL_WEIGHT_CHARGE") != null ? Convert.ToDecimal(r.Field<object>("ACCEPTED_TOTAL_WEIGHT_CHARGE")) : 0,
            TotalWeightChargeDifference = r.Field<object>("TOTAL_WEIGHT_CHARGES_DIFF") != null ? Convert.ToDecimal(r.Field<object>("TOTAL_WEIGHT_CHARGES_DIFF")) : 0,
            BilledTotalValuationCharge = r.Field<object>("BILLED_TOTAL_VALUATION_CHARGES") != null ? Convert.ToDecimal(r.Field<object>("BILLED_TOTAL_VALUATION_CHARGES")) : 0,
            AcceptedTotalValuationCharge = r.Field<object>("ACCPTED_TOTAL_VALUATION_CHARGE") != null ? Convert.ToDecimal(r.Field<object>("ACCPTED_TOTAL_VALUATION_CHARGE")) : 0,
            TotalValuationChargeDifference = r.Field<object>("TOTAL_VALUATION_CHARGES_DIFF") != null ? Convert.ToDecimal(r.Field<object>("TOTAL_VALUATION_CHARGES_DIFF")) : 0,
            BilledTotalOtherChargeAmount = r.Field<object>("BILLED_TOTAL_OTHER_CHARGES_AMT") != null ? Convert.ToDecimal(r.Field<object>("BILLED_TOTAL_OTHER_CHARGES_AMT")) : 0,
            AcceptedTotalOtherChargeAmount = r.Field<object>("ACCEPTED_TOTAL_OTH_CHARGES_AMT") != null ? Convert.ToDecimal(r.Field<object>("ACCEPTED_TOTAL_OTH_CHARGES_AMT")) : 0,
            TotalOtherChargeDifference = r.Field<object>("TOTAL_OTH_CHARGES_DIFF") != null ? Convert.ToDecimal(r.Field<object>("TOTAL_OTH_CHARGES_DIFF")) : 0,
            AllowedTotalIscAmount = r.Field<object>("ALLOWED_TOTAL_ISC_AMT") != null ? Convert.ToDecimal(r.Field<object>("ALLOWED_TOTAL_ISC_AMT")) : 0,
            AcceptedTotalIscAmount = r.Field<object>("ACCEPTED_TOTAL_ISC_AMT") != null ? Convert.ToDecimal(r.Field<object>("ACCEPTED_TOTAL_ISC_AMT")) : 0,
            TotalIscAmountDifference = r.Field<object>("TOTAL_ISC_AMT_DIFF") != null ? Convert.ToDecimal(r.Field<object>("TOTAL_ISC_AMT_DIFF")) : 0,
            BilledTotalVatAmount = r.Field<object>("BILLED_TOTAL_VAT_AMT") != null ? Convert.ToDecimal(r.Field<object>("BILLED_TOTAL_VAT_AMT")) : 0,
            AcceptedTotalVatAmount = r.Field<object>("ACCEPTED_TOTAL_VAT_AMT") != null ? Convert.ToDecimal(r.Field<object>("ACCEPTED_TOTAL_VAT_AMT")) : 0,
            TotalVatAmountDifference = r.Field<object>("TOTAL_VAT_AMT_DIFF") != null ? r.Field<double>("TOTAL_VAT_AMT_DIFF") : 0.0,
            TotalNetRejectAmount = r.Field<object>("TOTAL_NET_REJECT_AMT") != null ? Convert.ToDecimal(r.Field<object>("TOTAL_NET_REJECT_AMT")) : 0,
            CorrespondenceId = r.Field<byte[]>("CORRESPONDENCE_ID") != null ? (Guid?)new Guid(r.Field<byte[]>("CORRESPONDENCE_ID")) : null,
            AttachmentIndicatorOriginal = r.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (r.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? false : true) : false,
            AttachmentIndicatorValidated = r.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (r.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
            NumberOfAttachments = r.Field<object>("ATTACHMENT_NO") != null ? r.Field<int?>("ATTACHMENT_NO") : null,
            ISValidationFlag = r.Field<string>("IS_VALIDATION_FLAG"),
            ReasonRemarks = r.Field<string>("REASON_REMARKS"),
            LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        });

        public  readonly Materializer<CargoCorrespondence> CargoInvoiceCorrespondenceAuditMaterializer = new Materializer<CargoCorrespondence>(pCorr => new CargoCorrespondence
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
                 Adding code to get email ids from initiator and non-initiator and removing
                 additional email field*/
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
            AcceptanceComment = pCorr.Field<object>("ACCEPTANCE_COMMENTS") != null ? pCorr.Field<string>("ACCEPTANCE_COMMENTS") : null,
            AcceptanceUserName = pCorr.Field<object>("ACCEPTANCE_USER") != null ? pCorr.Field<string>("ACCEPTANCE_USER") : null,
            AcceptanceDateTime = pCorr.Field<object>("ACCEPTANCE_DATE") != null ? pCorr.Field<DateTime>("ACCEPTANCE_DATE") : new DateTime(),
            CurrencyId = pCorr.Field<object>("CURRENCY_CODE_NUM") != null ? pCorr.Field<int?>("CURRENCY_CODE_NUM") : null,
        });

        public  readonly Materializer<CargoCorrespondenceAttachment> CargoCorrespondenceAttachmentMaterializer = new Materializer<CargoCorrespondenceAttachment>(ca =>
       new CargoCorrespondenceAttachment
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

        public  readonly Materializer<RMAwb> CgoRmAwbAuditMaterializer = new Materializer<RMAwb>(rmAwb => new RMAwb
        {
            Id = rmAwb.Field<byte[]>("RM_AWB_ID") != null ? new Guid(rmAwb.Field<byte[]>("RM_AWB_ID")) : new Guid(),
            LastUpdatedOn = rmAwb.Field<object>("LAST_UPDATED_ON") != null ? rmAwb.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = rmAwb.Field<object>("LAST_UPDATED_BY") != null ? rmAwb.Field<int>("LAST_UPDATED_BY") : 0,
            RejectionMemoId = rmAwb.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(rmAwb.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
            BdSerialNumber = rmAwb.Field<object>("BD_SR_NO") != null ? rmAwb.Field<int>("BD_SR_NO") : 0,
            AwbDate = rmAwb.Field<object>("AWB_DATE") != null ? rmAwb.Field<DateTime?>("AWB_DATE") : null,
            AwbIssueingAirline = rmAwb.Field<string>("AWB_ISSUING_AIRLINE"),
            AwbSerialNumber = rmAwb.Field<object>("AWB_SR_NO") != null ? rmAwb.Field<int>("AWB_SR_NO") : 0,
            AwbCheckDigit = rmAwb.Field<object>("AWB_CHECK_DIGIT") != null ? rmAwb.Field<int>("AWB_CHECK_DIGIT") : 0,
            ConsignmentOriginId = rmAwb.Field<string>("CONSIGNMENT_ORIGIN_ID"),
            ConsignmentDestinationId = rmAwb.Field<string>("CONSIGNMENT_DESTINATION_ID"),
            CarriageFromId = rmAwb.Field<string>("CARRIAGE_FROM_ID"),
            CarriageToId = rmAwb.Field<string>("CARRIAGE_TO_ID"),
            TransferDate = rmAwb.Field<object>("TRANSFER_DATE") != null ? rmAwb.Field<DateTime?>("TRANSFER_DATE") : null,
            BilledWeightCharge = rmAwb.Field<object>("BILLED_WEIGHT_CHARGES") != null ? rmAwb.Field<double>("BILLED_WEIGHT_CHARGES") : 0,
            AcceptedWeightCharge = rmAwb.Field<object>("ACCEPTED_WEIGHT_CHARGES") != null ? rmAwb.Field<double>("ACCEPTED_WEIGHT_CHARGES") : 0,
            WeightChargeDiff = rmAwb.Field<object>("WEIGHT_CHARGES_DIFF") != null ? rmAwb.Field<double>("WEIGHT_CHARGES_DIFF") : 0,
            BilledValuationCharge = rmAwb.Field<object>("BILLED_VALUATION_CHARGES") != null ? rmAwb.Field<double>("BILLED_VALUATION_CHARGES") : 0,
            AcceptedValuationCharge = rmAwb.Field<object>("ACCEPTED_VALUATION_CHARGES") != null ? rmAwb.Field<double>("ACCEPTED_VALUATION_CHARGES") : 0,
            ValuationChargeDiff = rmAwb.Field<object>("VALUATION_CHARGES_DIFF") != null ? rmAwb.Field<double>("VALUATION_CHARGES_DIFF") : 0,
            BilledOtherCharge = rmAwb.Field<object>("BILLED_OTHER_CHARGES_AMT") != null ? rmAwb.Field<double>("BILLED_OTHER_CHARGES_AMT") : 0,
            AcceptedOtherCharge = rmAwb.Field<object>("ACCEPTED_OTHER_CHARGES_AMT") != null ? rmAwb.Field<double>("ACCEPTED_OTHER_CHARGES_AMT") : 0,
            //SCP:53226 - Prorate Ladder information Currency not being displayed Correctly in PDF Rejection Memo generated by SIS - Air Calin
            // check if ProrateCalCurrencyId is null then replace with empty string.
            ProrateCalCurrencyId = rmAwb.Field<string>("PRORATE_CALC_CURRENCY_ID")??string.Empty,
            TotalProrateAmount = rmAwb.Field<object>("TOTAL_PRORATE_AMOUNT") != null ? rmAwb.Field<double?>("TOTAL_PRORATE_AMOUNT") : null,
            NetRejectAmount = rmAwb.Field<object>("NET_REJECT_AMT") != null ? rmAwb.Field<double>("NET_REJECT_AMT") : 0,
            KgLbIndicator = rmAwb.Field<object>("KG_LB_INDICATOR") != null ? rmAwb.Field<string>("KG_LB_INDICATOR") : string.Empty,
            PartShipmentIndicator = rmAwb.Field<object>("PART_SHIPMENT_IND") != null ? rmAwb.Field<string>("PART_SHIPMENT_IND") : string.Empty,
            IscAmountDifference = rmAwb.Field<object>("ISC_AMT_DIFF") != null ? rmAwb.Field<double>("ISC_AMT_DIFF") : 0,
            VatAmountDifference = rmAwb.Field<object>("VAT_AMT_DIFF") != null ? rmAwb.Field<double>("VAT_AMT_DIFF") : 0,
            OtherChargeDiff = rmAwb.Field<object>("OTHER_CHARGES_DIFF") != null ? rmAwb.Field<double>("OTHER_CHARGES_DIFF") : 0,
            BilledVatAmount = rmAwb.Field<object>("BILLED_VAT_AMT") != null ? rmAwb.Field<double>("BILLED_VAT_AMT") : 0,
            AcceptedVatAmount = rmAwb.Field<object>("ACCEPTED_VAT_AMT") != null ? rmAwb.Field<double>("ACCEPTED_VAT_AMT") : 0,
            AcceptedIscAmount = rmAwb.Field<object>("ACCEPTED_ISC_AMT") != null ? rmAwb.Field<double>("ACCEPTED_ISC_AMT") : 0,
            AllowedIscAmount = rmAwb.Field<object>("ALLOWED_ISC_AMT") != null ? rmAwb.Field<double>("ALLOWED_ISC_AMT") : 0,
            AcceptedIscPercentage = rmAwb.Field<object>("ACCEPTED_ISC_PER") != null ? rmAwb.Field<double>("ACCEPTED_ISC_PER") : 0,
            AllowedIscPercentage = rmAwb.Field<object>("ALLOWED_ISC_PER") != null ? rmAwb.Field<double>("ALLOWED_ISC_PER") : 0,
        });

        public  readonly Materializer<RMAwbVat> CargoRMAwbVatAuditMaterializer = new Materializer<RMAwbVat>(r => new RMAwbVat
        {
            LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = r.Field<byte[]>("RM_AWB_ID") != null ? new Guid(r.Field<byte[]>("RM_AWB_ID")) : new Guid(),
            VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0,
            VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0,
            VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0,
            VatText = r.Field<object>("VAT_TEXT") != null ? r.Field<string>("VAT_TEXT") : null,
            VatLabel = r.Field<object>("VAT_LABEL") != null ? r.Field<string>("VAT_LABEL") : null,
            VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
            Id = r.Field<byte[]>("RM_AWB_VAT_ID") != null ? new Guid(r.Field<byte[]>("RM_AWB_VAT_ID")) : new Guid(),
        });

        public  readonly Materializer<RMAwbAttachment> RMAwbAttachmentAuditMaterializer = new Materializer<RMAwbAttachment>(rmca => new RMAwbAttachment
        {
            ServerId = rmca.Field<object>("SERVER_ID") != null ? rmca.Field<int>("SERVER_ID") : 0,
            FilePath = rmca.Field<string>("FILE_PATH"),
            OriginalFileName = rmca.Field<string>("ORG_FILE_NAME"),
            LastUpdatedOn = rmca.Field<object>("LAST_UPDATED_ON") != null ? rmca.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = rmca.Field<object>("LAST_UPDATED_BY") != null ? rmca.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = rmca.Field<byte[]>("RM_AWB_ID") != null ? new Guid(rmca.Field<byte[]>("RM_AWB_ID")) : new Guid(),
            FileStatusId = rmca.Field<object>("FILE_STATUS_ID") != null ? rmca.Field<int>("FILE_STATUS_ID") : 0,
            FileTypeId = rmca.Field<object>("FILE_TYPE_ID") != null ? rmca.Field<int>("FILE_TYPE_ID") : 0,
            FileSize = rmca.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(rmca.Field<object>("FILE_SIZE")) : 0,
            IsFullPath = rmca.Field<object>("IS_FULL_PATH") != null ? (rmca.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
            Id = rmca.Field<byte[]>("RM_AWB_ATTACHMENTS_ID") != null ? new Guid(rmca.Field<byte[]>("RM_AWB_ATTACHMENTS_ID")) : new Guid(),
        });

        public  readonly Materializer<CgoRejectionMemoVat> CgoRejectionMemoVatAuditMaterializer = new Materializer<CgoRejectionMemoVat>(r => new CgoRejectionMemoVat
        {
            ParentId = r.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(r.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
            VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
            VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
            VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
            VatText = r.Field<string>("VAT_TEXT"),
            VatLabel = r.Field<string>("VAT_LABEL"),
            VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
            Id = r.Field<byte[]>("RM_VAT_ID") != null ? new Guid(r.Field<byte[]>("RM_VAT_ID")) : new Guid(),
        });

        public  readonly Materializer<CargoBillingMemo> CargoBillingMemoAuditMaterializer = new Materializer<CargoBillingMemo>(bm => new CargoBillingMemo
        {
            Id = bm.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(bm.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
            InvoiceId = bm.Field<byte[]>("INVOICE_ID") != null ? new Guid(bm.Field<byte[]>("INVOICE_ID")) : new Guid(),
            BatchSequenceNumber = bm.Field<object>("BATCH_SEQ_NO") != null ? bm.Field<int>("BATCH_SEQ_NO") : 0,
            RecordSequenceWithinBatch = bm.Field<object>("BATCH_REC_SEQ_NO") != null ? bm.Field<int>("BATCH_REC_SEQ_NO") : 0,
            BillingMemoNumber = bm.Field<string>("BILLING_MEMO_NO"),
            ReasonCode = bm.Field<string>("REASON_CODE"),
            OurRef = bm.Field<object>("OUR_REF") != null ? bm.Field<string>("OUR_REF") : null,
            CorrespondenceReferenceNumber = bm.Field<object>("CORRESPONDENCE_REF_NO") != null ? Convert.ToInt64(bm.Field<object>("CORRESPONDENCE_REF_NO")) : 0,
            YourInvoiceNumber = bm.Field<object>("YOUR_INVOICE_NO") != null ? bm.Field<string>("YOUR_INVOICE_NO") : null,
            BilledTotalWeightCharge = bm.Field<object>("TOTAL_WEIGHT_CHARGES") != null ? (decimal)bm.Field<double>("TOTAL_WEIGHT_CHARGES") : 0,
            BilledTotalValuationAmount = bm.Field<object>("TOTAL_VALUATION_AMT") != null ? (decimal)bm.Field<double>("TOTAL_VALUATION_AMT") : 0,
            BilledTotalOtherChargeAmount = bm.Field<object>("TOTAL_OTHER_CHARGE_AMT") != null ? (decimal)bm.Field<double>("TOTAL_OTHER_CHARGE_AMT") : 0,
            BilledTotalIscAmount = bm.Field<object>("TOTAL_ISC_AMT") != null ? (decimal)bm.Field<double>("TOTAL_ISC_AMT") : 0,
            BilledTotalVatAmount = bm.Field<object>("TOTAL_VAT_AMT") != null ? (decimal)bm.Field<double>("TOTAL_VAT_AMT") : 0,
            NetBilledAmount = bm.Field<object>("NET_BILLED_AMT") != null ? (decimal)bm.Field<double>("NET_BILLED_AMT") : 0,
            AttachmentIndicatorOriginal = bm.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (bm.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? false : true) : false,
            AttachmentIndicatorValidated = bm.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (bm.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
            NumberOfAttachments = bm.Field<object>("ATTACHMENT_NO") != null ? bm.Field<int?>("ATTACHMENT_NO") : null,
            AirlineOwnUse = bm.Field<object>("AIRLINE_OWN_USE") != null ? bm.Field<string>("AIRLINE_OWN_USE") : null,
            ISValidationFlag = bm.Field<object>("IS_VALIDATION_FLAG") != null ? bm.Field<string>("IS_VALIDATION_FLAG") : null,
            ReasonRemarks = bm.Field<object>("REASON_REMARKS") != null ? bm.Field<string>("REASON_REMARKS") : null,
            LastUpdatedBy = bm.Field<object>("LAST_UPDATED_BY") != null ? bm.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = bm.Field<object>("LAST_UPDATED_ON") != null ? bm.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            YourInvoiceBillingYear = bm.Field<object>("YOUR_INVOICE_BILLING_YEAR") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_YEAR") : 0,
            YourInvoiceBillingMonth = bm.Field<object>("YOUR_INVOICE_BILLING_MONTH") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_MONTH") : 0,
            YourInvoiceBillingPeriod = bm.Field<object>("YOUR_INVOICE_BILLING_PERIOD") != null ? bm.Field<int>("YOUR_INVOICE_BILLING_PERIOD") : 0,
        });

        public  readonly Materializer<CargoBillingMemoVat> CargoBillingMemoVatAuditMaterializer = new Materializer<CargoBillingMemoVat>(r =>
        new CargoBillingMemoVat
        {
            Id = r.Field<byte[]>("BM_VAT_ID") != null ? new Guid(r.Field<byte[]>("BM_VAT_ID")) : new Guid(),
            ParentId = r.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(r.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
            VatIdentifierId = r.Field<object>("VAT_IDENTIFIER_ID") != null ? r.Field<int>("VAT_IDENTIFIER_ID") : 0,
            VatLabel = r.Field<string>("VAT_LABEL"),
            VatText = r.Field<string>("VAT_TEXT"),
            VatBaseAmount = r.Field<object>("VAT_BASE_AMT") != null ? r.Field<double>("VAT_BASE_AMT") : 0.0,
            VatPercentage = r.Field<object>("VAT_PER") != null ? r.Field<double>("VAT_PER") : 0.0,
            VatCalculatedAmount = r.Field<object>("VAT_CALC_AMT") != null ? r.Field<double>("VAT_CALC_AMT") : 0.0,
            LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,

        });

        public  readonly Materializer<CargoBillingMemoAttachment> CargoBillingMemoAttachmentAuditMaterializer = new Materializer<CargoBillingMemoAttachment>(bma =>
        new CargoBillingMemoAttachment
        {
            Id = bma.Field<byte[]>("BM_ATTACHMENT_ID") != null ? new Guid(bma.Field<byte[]>("BM_ATTACHMENT_ID")) : new Guid(),
            ParentId = bma.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(bma.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
            OriginalFileName = bma.Field<string>("FILE_NAME"),
            FileSize = bma.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(bma.Field<object>("FILE_SIZE")) : 0,
            FileTypeId = bma.Field<object>("FILE_TYPE_ID") != null ? bma.Field<int>("FILE_TYPE_ID") : 0,
            FileStatusId = bma.Field<object>("FILE_STATUS_ID") != null ? bma.Field<int>("FILE_STATUS_ID") : 0,
            LastUpdatedBy = bma.Field<object>("LAST_UPDATED_BY") != null ? bma.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = bma.Field<object>("LAST_UPDATED_ON") != null ? bma.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            FilePath = bma.Field<string>("FILE_PATH"),
            ServerId = bma.Field<object>("SERVER_ID") != null ? bma.Field<int>("SERVER_ID") : 0,
            IsFullPath = bma.Field<object>("IS_FULL_PATH") != null ? (bma.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
        });

        public  readonly Materializer<CargoBillingMemoAwb> CargoBillingMemoAwbAuditMaterializer = new Materializer<CargoBillingMemoAwb>(p1 => new CargoBillingMemoAwb
        {
            Id = p1.Field<byte[]>("BM_AWB_ID") != null ? new Guid(p1.Field<byte[]>("BM_AWB_ID")) : new Guid(),
            LastUpdatedOn = p1.Field<object>("LAST_UPDATED_ON") != null ? p1.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = p1.Field<object>("LAST_UPDATED_BY") != null ? p1.Field<int>("LAST_UPDATED_BY") : 0,
            BillingMemoId = p1.Field<byte[]>("BILLING_MEMO_ID") != null ? new Guid(p1.Field<byte[]>("BILLING_MEMO_ID")) : new Guid(),
            BdSerialNumber = p1.Field<object>("BD_SR_NO") != null ? p1.Field<int>("BD_SR_NO") : 0,
            AwbBillingCode = p1.Field<object>("AWB_BILLING_CODE") != null ? p1.Field<int>("AWB_BILLING_CODE") : 0,
            AwbDate = p1.Field<object>("AWB_DATE") != null ? p1.Field<DateTime?>("AWB_DATE") : null,
            AwbIssueingAirline = p1.Field<string>("AWB_ISSUING_AIRLINE"),
            AwbSerialNumber = p1.Field<object>("AWB_SR_NO") != null ? p1.Field<int>("AWB_SR_NO") : 0,
            // AwbCheckDigit = p1.Field<object>("AWB_CHECK_DIGIT") != null ? (p1.Field<int>("AWB_CHECK_DIGIT") == 0 ? false : true) : false,
            ConsignmentOriginId = p1.Field<string>("CONSIGNMENT_ORIGIN_ID"),
            ConsignmentDestinationId = p1.Field<string>("CONSIGNMENT_DESTINATION_ID"),
            CarriageFromId = p1.Field<string>("CARRIAGE_FROM_ID"),
            CarriageToId = p1.Field<string>("CARRIAGE_TO_ID"),
            TransferDate = p1.Field<object>("TRANSFER_DATE") != null ? p1.Field<DateTime?>("TRANSFER_DATE") : null,
            BilledWeightCharge = p1.Field<object>("WEIGHT_CHARGES") != null ? p1.Field<double>("WEIGHT_CHARGES") : 0,
            BilledValuationCharge = p1.Field<object>("VALUATION_CHARGES") != null ? p1.Field<double>("VALUATION_CHARGES") : 0,
            BilledOtherCharge = p1.Field<object>("OTHER_CHARGES_AMT") != null ? p1.Field<double>("OTHER_CHARGES_AMT") : 0,
            BilledAmtSubToIsc = p1.Field<object>("BIILED_AMT_SUB_TO_ISC") != null ? p1.Field<double>("BIILED_AMT_SUB_TO_ISC") : 0,
            BilledIscPercentage = p1.Field<object>("ISC_PER") != null ? p1.Field<double>("ISC_PER") : 0,
            BilledIscAmount = p1.Field<object>("ISC_AMT") != null ? p1.Field<double>("ISC_AMT") : 0,
            BilledVatAmount = p1.Field<object>("VAT_AMT") != null ? p1.Field<double>("VAT_AMT") : 0,
            TotalAmount = p1.Field<object>("TOTAL_AMT") != null ? p1.Field<double>("TOTAL_AMT") : 0,
            CurrencyAdjustmentIndicator = p1.Field<string>("CURRENCY_ADJUSTMENT_IND"),
            BilledWeight = p1.Field<object>("BILLED_WEIGHT") != null ? p1.Field<int>("BILLED_WEIGHT") : 0,
            // OurReference = p1.Field<string>("OUR_REFERENCE"),
            ProvisionalReqSpa = p1.Field<string>("PROVISO_REQ_SPA"),
            PrpratePercentage = p1.Field<object>("PRORATE_PER") != null ? p1.Field<int>("PRORATE_PER") : 0,
            PartShipmentIndicator = p1.Field<string>("PART_SHIPMENT_IND"),
            KgLbIndicator = p1.Field<string>("KG_LB_INDICATOR"),
            CcaIndicator = p1.Field<object>("CCA_INDICATOR") != null ? (p1.Field<int>("CCA_INDICATOR") == 0 ? false : true) : false,
            AttachmentIndicatorOriginal = p1.Field<object>("ATTACHMENT_INDICATOR_ORIGINAL") != null ? (p1.Field<int>("ATTACHMENT_INDICATOR_ORIGINAL") == 0 ? false : true) : false,
            AttachmentIndicatorValidated = p1.Field<object>("ATTACHMENT_INDICATOR_VALIDATED") != null ? (p1.Field<int>("ATTACHMENT_INDICATOR_VALIDATED") == 0 ? (bool?)false : (bool?)true) : null,
            NumberOfAttachments = p1.Field<object>("ATTACHMENT_NO") != null ? p1.Field<int?>("ATTACHMENT_NO") : null,
            ISValidationFlag = p1.Field<string>("IS_VALIDATION_FLAG"),
            ReasonCode = p1.Field<string>("REASON_CODE"),
            ReferenceField1 = p1.Field<string>("REFERENCE_FIELD_1"),
            ReferenceField2 = p1.Field<string>("REFERENCE_FIELD_2"),
            ReferenceField3 = p1.Field<string>("REFERENCE_FIELD_3"),
            ReferenceField4 = p1.Field<string>("REFERENCE_FIELD_4"),
            ReferenceField5 = p1.Field<string>("REFERENCE_FIELD_5"),
            AirlineOwnUse = p1.Field<string>("AIRLINE_OWN_USE")
        });

        public  readonly Materializer<BMAwbVat> CargoBillingMemoAwbVatAuditMaterializer = new Materializer<BMAwbVat>(cBmAwbVat => new BMAwbVat
        {
            LastUpdatedOn = cBmAwbVat.Field<object>("LAST_UPDATED_ON") != null ? cBmAwbVat.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cBmAwbVat.Field<object>("LAST_UPDATED_BY") != null ? cBmAwbVat.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cBmAwbVat.Field<byte[]>("BM_AWB_ID") != null ? new Guid(cBmAwbVat.Field<byte[]>("BM_AWB_ID")) : new Guid(),
            VatCalculatedAmount = cBmAwbVat.Field<object>("VAT_CALC_AMT") != null ? cBmAwbVat.Field<double>("VAT_CALC_AMT") : 0,
            VatPercentage = cBmAwbVat.Field<object>("VAT_PER") != null ? cBmAwbVat.Field<double>("VAT_PER") : 0,
            VatBaseAmount = cBmAwbVat.Field<object>("VAT_BASE_AMT") != null ? cBmAwbVat.Field<double>("VAT_BASE_AMT") : 0,
            VatText = cBmAwbVat.Field<object>("VAT_TEXT") != null ? cBmAwbVat.Field<string>("VAT_TEXT") : null,
            VatLabel = cBmAwbVat.Field<object>("VAT_LABEL") != null ? cBmAwbVat.Field<string>("VAT_LABEL") : null,
            VatIdentifierId = cBmAwbVat.Field<object>("VAT_IDENTIFIER_ID") != null ? cBmAwbVat.Field<int>("VAT_IDENTIFIER_ID") : 0,
            Id = cBmAwbVat.Field<byte[]>("AWB_VAT_ID") != null ? new Guid(cBmAwbVat.Field<byte[]>("AWB_VAT_ID")) : new Guid(),
        });

        public readonly Materializer<CgoRejectionMemoAttachment> CargoRejectionMemoAuditAttachmentMaterializer = new Materializer<CgoRejectionMemoAttachment>(rma =>
       new CgoRejectionMemoAttachment
       {
         Id = rma.Field<byte[]>("RM_ATTACHMENT_ID") != null ? new Guid(rma.Field<byte[]>("RM_ATTACHMENT_ID")) : new Guid(),
         ParentId = rma.Field<byte[]>("REJECTION_MEMO_ID") != null ? new Guid(rma.Field<byte[]>("REJECTION_MEMO_ID")) : new Guid(),
         OriginalFileName = rma.Field<string>("FILE_NAME"),
         FileSize = rma.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(rma.Field<object>("FILE_SIZE")) : 0,
         FileTypeId = rma.Field<object>("FILE_TYPE_ID") != null ? rma.Field<int>("FILE_TYPE_ID") : 0,
         FileStatusId = rma.Field<object>("FILE_STATUS_ID") != null ? rma.Field<int>("FILE_STATUS_ID") : 0,
         LastUpdatedBy = rma.Field<object>("LAST_UPDATED_BY") != null ? rma.Field<int>("LAST_UPDATED_BY") : 0,
         LastUpdatedOn = rma.Field<object>("LAST_UPDATED_ON") != null ? rma.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
         FilePath = rma.Field<string>("FILE_PATH"),
         ServerId = rma.Field<object>("SERVER_ID") != null ? rma.Field<int>("SERVER_ID") : 0,
         IsFullPath = rma.Field<object>("IS_FULL_PATH") != null ? (rma.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
       });

        public  readonly Materializer<BMAwbAttachment> CargoBillingMemoAwbAttachmentMaterializer = new Materializer<BMAwbAttachment>(cBmAwbA => new BMAwbAttachment
        {
            ServerId = cBmAwbA.Field<object>("SERVER_ID") != null ? cBmAwbA.Field<int>("SERVER_ID") : 0,
            FilePath = cBmAwbA.Field<string>("FILE_PATH"),
            OriginalFileName = cBmAwbA.Field<string>("FILE_NAME"),
            LastUpdatedOn = cBmAwbA.Field<object>("LAST_UPDATED_ON") != null ? cBmAwbA.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
            LastUpdatedBy = cBmAwbA.Field<object>("LAST_UPDATED_BY") != null ? cBmAwbA.Field<int>("LAST_UPDATED_BY") : 0,
            ParentId = cBmAwbA.Field<byte[]>("BM_AWB_ID") != null ? (Guid?)new Guid(cBmAwbA.Field<byte[]>("BM_AWB_ID")) : null,
            FileTypeId = cBmAwbA.Field<object>("FILE_TYPE_ID") != null ? cBmAwbA.Field<int>("FILE_TYPE_ID") : 0,
            FileSize = cBmAwbA.Field<object>("FILE_SIZE") != null ? Convert.ToInt64(cBmAwbA.Field<object>("FILE_SIZE")) : 0,
            IsFullPath = cBmAwbA.Field<object>("IS_FULL_PATH") != null ? (cBmAwbA.Field<int>("IS_FULL_PATH") == 0 ? false : true) : false,
            Id = cBmAwbA.Field<byte[]>("BM_AWB_ATTACHMENT_ID") != null ? new Guid(cBmAwbA.Field<byte[]>("BM_AWB_ATTACHMENT_ID")) : new Guid(),
        });

        public  readonly Materializer<CargoCorrespondence> CargoCorrespondenceMaterializer = new Materializer<CargoCorrespondence>(pCorr => new CargoCorrespondence
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
                 Adding code to get email ids from initiator and non-initiator and removing
                 additional email field*/
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
            AcceptanceComment = pCorr.Field<object>("ACCEPTANCE_COMMENTS") != null ? pCorr.Field<string>("ACCEPTANCE_COMMENTS") : null,
            AcceptanceUserName = pCorr.Field<object>("ACCEPTANCE_USER") != null ? pCorr.Field<string>("ACCEPTANCE_USER") : null,
            AcceptanceDateTime = pCorr.Field<object>("ACCEPTANCE_DATE") != null ? pCorr.Field<DateTime>("ACCEPTANCE_DATE") : new DateTime(),
            LastUpdatedBy = pCorr.Field<object>("LAST_UPDATED_BY") != null ? pCorr.Field<int>("LAST_UPDATED_BY") : 0,
            LastUpdatedOn = pCorr.Field<object>("LAST_UPDATED_ON") != null ? pCorr.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        });

        #endregion
    }
}

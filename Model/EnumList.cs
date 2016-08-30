using System.Collections.Generic;
//using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Pax.Enums;
using Cargoenums = Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp.Enums;
//using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Reports.Enums;
using Iata.IS.Model.SupportingDocuments.Enums;

namespace Iata.IS.Model
{
    public sealed class EnumList
    {
        /// <summary>
        /// For performance reasons.
        /// </summary>
        private EnumList()
        {
        }

        /// <summary>
        /// Holds mapping of rejection stage and corresponding display value.
        /// </summary>
        public static readonly Dictionary<RejectionStage, string> RejectionStageValues = new Dictionary<RejectionStage, string>
                                                                                        {
                                                                                          { RejectionStage.StageOne, "1" },
                                                                                          { RejectionStage.StageTwo, "2" },
                                                                                          { RejectionStage.StageThree, "3" },
                                                                                        };


        /// <summary>
        /// Holds mapping of rejection stage and corresponding display value.
        /// </summary>
        public static readonly Dictionary<MiscRejectionStage, string> MiscRejectionStageValues = new Dictionary<MiscRejectionStage, string>
                                                                                        {
                                                                                          { MiscRejectionStage.StageOne, "1" },
                                                                                          { MiscRejectionStage.StageTwo, "2" }
                                                                                        };

        /// <summary>
        /// Holds mapping of billing currency and corresponding display value.
        /// </summary>
        public static readonly Dictionary<BillingCurrency, string> BillingCurrencyValues = new Dictionary<BillingCurrency, string>
                                                                                          {
                                                                                            { BillingCurrency.USD, "USD" },
                                                                                            { BillingCurrency.GBP, "GBP" },
                                                                                            { BillingCurrency.EUR, "EUR" },
                                                                                            { BillingCurrency.CAD, "CAD" },
                                                                                          };

        /// <summary>
        /// Holds mapping of billing code and corresponding display value.
        /// </summary>
        public static readonly Dictionary<Pax.Enums.BillingCode, string> BillingCodeValuesList = new Dictionary<Pax.Enums.BillingCode, string>
                                                                                  {
                                                                                    { BillingCode.NonSampling, "NS: Non-Sampling Invoice" },
                                                                                    { BillingCode.SamplingFormAB, "S-A/B: Sampling Form A/B" },
                                                                                    { BillingCode.SamplingFormC, "S-C: Sampling Form C" },
                                                                                    { BillingCode.SamplingFormDE, "S-D/E: Sampling Form D/E" },
                                                                                    { BillingCode.SamplingFormF, "S-F: Sampling Form F" },
                                                                                    { BillingCode.SamplingFormXF, "S-XF: Sampling Form XF" },
                                                                                  };

        public static readonly Dictionary<Pax.Enums.BillingCode, string> BillingCodeValues = new Dictionary<Pax.Enums.BillingCode, string>
                                                                                  {
                                                                                    { BillingCode.NonSampling, "NS" },
                                                                                    { BillingCode.SamplingFormAB, "S-A/B" },
                                                                                    { BillingCode.SamplingFormC, "S-C" },
                                                                                    { BillingCode.SamplingFormDE, "S-D/E" },
                                                                                    { BillingCode.SamplingFormF, "S-F" },
                                                                                    { BillingCode.SamplingFormXF, "S-XF" },
                                                                                  };

        /// <summary>
        /// Holds mapping of Cargo Billing Codes and corresponding display value.
        /// </summary>
        public static readonly Dictionary<Cargo.Enums.BillingCode, string> CgoBillingCodeValues = new Dictionary<Cargo.Enums.BillingCode, string>
                                                                     {
                                                                       {Cargoenums.BillingCode.AWBPrepaid, "AWB - Prepaid"},
                                                                       {Cargoenums.BillingCode.AWBChargeCollect, "AWB - Charge Collect"},
                                                                       {Cargoenums.BillingCode.BillingMemo, "Billing Memo"},
                                                                       {Cargoenums.BillingCode.CreditNote , "Credit Note"},
                                                                       {Cargoenums.BillingCode.RejectionMemo, "Rejection Memo"},
                                                                     };
        /// <summary>
        /// Return display value for given Cargo Billing Code.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCgoBillingCodeValue(Cargo.Enums.BillingCode key)
        {
            return CgoBillingCodeValues.ContainsKey(key) ? CgoBillingCodeValues[key] : string.Empty;
        }

        public static readonly Dictionary<InvoiceStatusType, string> InvoiceStatusValuesForFormC = new Dictionary<InvoiceStatusType, string>
                                                                                     {
                                                                                       { InvoiceStatusType.Open, "Open" },
                                                                                       //{ InvoiceStatus.PendingForCorrections, "Web Form C In Error " },
                                                                                       { InvoiceStatusType.ReadyForSubmission, "Ready for Submission" },
                                                                                       { InvoiceStatusType.ReadyForBilling, "Ready for Billing" },
                                                                                       { InvoiceStatusType.ProcessingComplete, "Processing Complete" },
                                                                                       { InvoiceStatusType.Presented, "Presented" },
                                                                                     };

        /// <summary>
        /// Holds mapping of Digital Signature Required and corresponding display values.
        /// </summary>
        public static readonly Dictionary<DigitalSignatureRequired, string> DigitalSignatureRequiredValues = new Dictionary<DigitalSignatureRequired, string>
                                                                                                            {
                                                                                                              { DigitalSignatureRequired.Yes, "Y: Yes" },
                                                                                                              { DigitalSignatureRequired.No, "N: No" },
                                                                                                              { DigitalSignatureRequired.Default, "D: Default" },
                                                                                                            };

        /// <summary>
        /// Holds mapping ofInvoice Type and corresponding display values.
        /// </summary>
        public static readonly Dictionary<InvoiceType, string> InvoiceTypeValues = new Dictionary<InvoiceType, string>
                                                                                  {
                                                                                    { InvoiceType.Invoice, "IV: Invoice" },
                                                                                    { InvoiceType.CreditNote, "CN: CreditNote" },
                                                                                  };


        /// <summary>
        /// Holds mapping of InvoiceDownloadOptions and corresponding display values.
        /// </summary>
        public static readonly Dictionary<InvoiceDownloadOptions, string> InvoiceDownloadOptionsValues = new Dictionary<InvoiceDownloadOptions, string>
                                                                             {
                                                                               { InvoiceDownloadOptions.EInvoicingFiles, "E-Invoicing Files" },
                                                                               { InvoiceDownloadOptions.ListingReport, "Listing Report" },
                                                                               { InvoiceDownloadOptions.Memos, "Memos" },
                                                                               { InvoiceDownloadOptions.SupportingDocuments, "Supporting Documents" },
                                                                             };

        /// <summary>
        /// Holds mapping of Memo Type and corresponding display values.
        /// </summary>
        public static readonly Dictionary<TransactionType, string> MemoTypes = new Dictionary<TransactionType, string>
                                                                             {
                                                                               {TransactionType.Coupon, "Prime Coupon"},
                                                                               //{TransactionType.SamplingFormD, "Form D"},
                                                                               { TransactionType.RejectionMemo3, "Rejection Memo" },
                                                                               { TransactionType.BillingMemo  , "Billng Memo" },
                                                                               { TransactionType.CreditMemo, "Credit Memo" }
                                                                             };

        /// <summary>
        /// Holds mapping of Cargo Memo Type and corresponding display values.
        /// </summary>
        public static readonly Dictionary<TransactionType, string> CargoMemoTypes = new Dictionary<TransactionType, string>
                                                                             {
                                                                               {TransactionType.CargoPrimePrepaid, "Prime AWB"},
                                                                               { TransactionType.CargoRejectionMemoStage1, "Rejection Memo" },
                                                                               { TransactionType.CargoBillingMemo, "Billng Memo" },
                                                                               { TransactionType.CargoCreditMemo, "Credit Memo" }
                                                                             };


        /// <summary>
        /// Holds mapping of Submission Method and corresponding display values.
        /// </summary>
        public static readonly Dictionary<SubmissionMethod, string> SubmissionMethodValues = new Dictionary<SubmissionMethod, string>
                                                                                            {
                                                                                              { SubmissionMethod.IsIdec, "IS-IDEC" },
                                                                                              { SubmissionMethod.IsWeb, "IS-WEB" },
                                                                                              { SubmissionMethod.IsXml, "IS-XML" }
                                                                                            };

        /// <summary>
        /// Holds mapping of Action Type and corresponding display values.
        /// </summary>
        public static readonly Dictionary<ActionType, string> ActionTypeValue = new Dictionary<ActionType, string>
                                                                               {
                                                                                 { ActionType.Update, "Update" },
                                                                                 { ActionType.Delete, "Delete" },
                                                                                 { ActionType.Create, "Create" },
                                                                               };

        /// <summary>
        /// Holds mapping of Correspondence Status and corresponding display values.
        /// </summary>
        public static readonly Dictionary<CorrespondenceStatus, string> CorrespondenceStatusValues = new Dictionary<CorrespondenceStatus, string>
                                                                               {
                                                                                 { CorrespondenceStatus.Open, "Open" },
                                                                                 { CorrespondenceStatus.Closed, "Closed" },
                                                                                 { CorrespondenceStatus.Expired, "Expired" },
                                                                               };

        /// <summary>
        /// Holds mapping of Correspondence Status and corresponding display values.
        /// </summary>
        public static readonly Dictionary<CorrespondenceSubStatus, string> CorrespondenceSubStatusValues = new Dictionary<CorrespondenceSubStatus, string>
                                                                               {
                                                                                 { CorrespondenceSubStatus.Received, "Received" },
                                                                                 { CorrespondenceSubStatus.Saved, "Saved" },
                                                                                 { CorrespondenceSubStatus.Responded, "Responded" },
                                                                                 { CorrespondenceSubStatus.ReadyForSubmit, "Ready For Submit" },
                                                                                 { CorrespondenceSubStatus.Billed, "Billed" },
                                                                                 { CorrespondenceSubStatus.DueToExpiry, "Due To Expiry" },
                                                                                 { CorrespondenceSubStatus.Pending, "Pending" },
                                                                                 { CorrespondenceSubStatus.AcceptedByCorrespondenceInitiator, "Accepted By Correspondence Initiator" },
                                                                               };


        /// <summary>
        /// Returns display value for given correspondence status.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCorrespondenceStatusDisplayValue(CorrespondenceStatus key)
        {
            return CorrespondenceStatusValues.ContainsKey(key) ? CorrespondenceStatusValues[key] : string.Empty;
        }

        /// <summary>
        /// Returns display value for given correspondence status.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCorrespondenceSubStatusDisplayValue(CorrespondenceSubStatus key)
        {
            return CorrespondenceSubStatusValues.ContainsKey(key) ? CorrespondenceSubStatusValues[key] : string.Empty;
        }


        /// <summary>
        ///   Holds mapping of Tax Category and corresponding display value.
        /// </summary>
        public static Dictionary<VatSubType, string> VatSubTypes = new Dictionary<VatSubType, string>
                                                                                  {
                                                                                    {
                                                                                      VatSubType.VAT,
                                                                                      "VAT"
                                                                                    },
                                                                                    {
                                                                                      VatSubType.GST,
                                                                                      "GST"
                                                                                    },
                                                                                    {
                                                                                      VatSubType.QST,
                                                                                      "QST"
                                                                                    },
                                                                                    {
                                                                                      VatSubType.HST,
                                                                                      "HST"
                                                                                    },
                                                                                    {
                                                                                      VatSubType.Sales,
                                                                                      "Sales"
                                                                                    },
                                                                                    {
                                                                                      VatSubType.State,
                                                                                      "State"
                                                                                    },
                                                                                    {
                                                                                       VatSubType.Federal,
                                                                                      "Federal"
                                                                                    },
                                                                                    {
                                                                                      VatSubType.IVA,
                                                                                      "IVA"
                                                                                    }
                                                                                  };

        /// <summary>
        /// Holds mapping of billing category and corresponding display value.
        /// </summary>
        public static readonly Dictionary<BillingCategoryType, string> BillingCategoryValues = new Dictionary<BillingCategoryType, string>
                                                                                          {
                                                                                            { BillingCategoryType.Pax, "PAX" },
                                                                                            { BillingCategoryType.Cgo, "CGO" },
                                                                                            { BillingCategoryType.Misc, "MISC" },
                                                                                            { BillingCategoryType.Uatp, "UATP" },
                                                                                          };

        /// <summary>
        /// Holds mapping of  Clearance type and corresponding display value.
        /// </summary>
        public static readonly Dictionary<SMI, string> ClearanceTypeValues = new Dictionary<SMI, string>
                                                                                          {
                                                                                            { SMI.Ich, "ICH" },
                                                                                            { SMI.Ach, "ACH" },
                                                                                            { SMI.AchUsingIataRules, "ACH using IATA Rules" },
                                                                                            { SMI.Bilateral, "Bilateral Settlement" },
                                                                                            { SMI.AdjustmentDueToProtest, "Adjustment Due To Protest"   },
                                                                                          };

        /// <summary>
        /// Holds mapping of all months in year and corresponding display value.
        /// </summary>
        public static readonly Dictionary<Month, string> MonthValues = new Dictionary<Month, string>
                                                                     {
                                                                       {Month.Jan, "Jan"},
                                                                       {Month.Feb, "Feb"},
                                                                       {Month.Mar, "Mar"},
                                                                       {Month.Apr, "Apr"},
                                                                       {Month.May, "May"},
                                                                       {Month.Jun, "Jun"},
                                                                       {Month.Jul, "Jul"},
                                                                       {Month.Aug, "Aug"},
                                                                       {Month.Sep, "Sep"},
                                                                       {Month.Oct, "Oct"},
                                                                       {Month.Nov, "Nov"},
                                                                       {Month.Dec, "Dec"},
                                                                     };

        /// <summary>
        /// Holds mapping of all periods in a month and corresponding display value.
        /// </summary>
        public static readonly Dictionary<InvoicePeriod, string> InvoicePeriodValues = new Dictionary<InvoicePeriod, string>
                                                                     {
                                                                       {InvoicePeriod.Period1, "1"},
                                                                       {InvoicePeriod.Period2, "2"},
                                                                       {InvoicePeriod.Period3, "3"},
                                                                       {InvoicePeriod.Period4, "4"},  
                                                                     };

        /// <summary>
        /// Holds mapping of Supporting document search criteria Type and corresponding display value.
        /// </summary>
        public static readonly Dictionary<SupportingDocType, string> SupportingDocTypeValues = new Dictionary<SupportingDocType, string>
                                                                     {
                                                                       {SupportingDocType.InvoiceCreditNote, "Invoice/Credit Note"},
                                                                       {SupportingDocType.FormC, "Form C"}, 
                                                                     };

        /// <summary>
        /// Holds mapping of Supporting document search criteria Attachment Indicator and corresponding display value.
        /// </summary>
        public static readonly Dictionary<SupportingDocAttachmentIndicator, string> SupportingDocAttachmentIndicatorValues = new Dictionary<SupportingDocAttachmentIndicator, string>
                                                                     {
                                                                       {SupportingDocAttachmentIndicator.All, "All"}, 
                                                                       {SupportingDocAttachmentIndicator.Yes, "Yes"},
                                                                       {SupportingDocAttachmentIndicator.No, "No"},
                                                                       {SupportingDocAttachmentIndicator.Pending, "Pending"},
                                                                     };
        /// <summary>
        /// Holds mapping of validated PMI in the pax. confirmation detail and corresponding display value.
        /// </summary>
        public static Dictionary<PaxCouponRecord, string> PaxCouponRecordValue = new Dictionary<PaxCouponRecord, string>
                                                                     {
                                                                       {PaxCouponRecord.ValidatedM, "M"},
                                                                       {PaxCouponRecord.ValidatedQ, "Q"},
                                                                       {PaxCouponRecord.ValidatedR, "R"},
                                                                       {PaxCouponRecord.ValidatedS, "S"},  
                                                                       {PaxCouponRecord.ValidatedT, "T"},
                                                                       {PaxCouponRecord.ValidatedU, "U"},
                                                                       {PaxCouponRecord.ValidatedV, "V"},
                                                                       {PaxCouponRecord.ValidatedW, "W"},
                                                                       {PaxCouponRecord.ValidatedX, "X"},
                                                                       {PaxCouponRecord.ValidatedZ, "Z"},                                                                       
                                                                     };


        /// <summary>
        /// Holds mapping of all file format  and corresponding display value.
        /// </summary>
        public static readonly Dictionary<FileFormatType, string> FileFormatValues = new Dictionary<FileFormatType, string>
                                                                     {
                                                                       {FileFormatType.IsIdec, "IsIdec"},
                                                                       {FileFormatType.IsXml, "IsXml"},
                                                                       {FileFormatType.Isr, "Isr"},
                                                                       {FileFormatType.ValueConfirmation, "Value Confirmation"}, 
                                                                       {FileFormatType.Usage, "Usage"},
                                                                       {FileFormatType.SupportingDoc, "Supporting Doc"},
                                                                     };


        /// <summary>
        /// Holds mapping of Misc Payment Status Applicable For display value.
        /// </summary>
        public static readonly Dictionary<InvPaymentStatusApplicableFor, string> ApplicableForValues = new Dictionary<InvPaymentStatusApplicableFor, string>
                                                                                          {
                                                                                            { InvPaymentStatusApplicableFor.BillingMember, "Billing Member" },
                                                                                            { InvPaymentStatusApplicableFor.BilledMember, "Billed Member" },
                                                                                            
                                                                                          };

        
        /// <summary>
        /// Returns display value for given billing code.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetBillingCodeDisplayValue(BillingCode key)
        {
            return BillingCodeValues.ContainsKey(key) ? BillingCodeValues[key] : string.Empty;
        }

        /// <summary>
        /// Looks up into Invoice Status list for Form C and returns display value for given invoice status.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetFormCInvoiceStatusDisplayValue(InvoiceStatusType key)
        {
            return InvoiceStatusValuesForFormC.ContainsKey(key) ? InvoiceStatusValuesForFormC[key] : string.Empty;
        }

        /// <summary>
        /// Returns display value for given Digital Signature Required
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetDigitalSignatureRequiredDisplayValue(DigitalSignatureRequired key)
        {
            return DigitalSignatureRequiredValues.ContainsKey(key) ? DigitalSignatureRequiredValues[key] : string.Empty;
        }

        /// <summary>
        /// Returns display value for given Invoice Type
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetInvoiceTypeDisplayValue(InvoiceType key)
        {
            return InvoiceTypeValues.ContainsKey(key) ? InvoiceTypeValues[key] : string.Empty;
        }

        /// <summary>
        /// Returns display value for given ActionType.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetActionTypeDisplayValue(ActionType key)
        {
            return ActionTypeValue.ContainsKey(key) ? ActionTypeValue[key] : string.Empty;
        }


        public static string GetBillingCurrencyDisplayValue(BillingCurrency key)
        {
            return BillingCurrencyValues.ContainsKey(key) ? BillingCurrencyValues[key] : string.Empty;
        }

        public static string GetSubmissionMethodDisplayValue(SubmissionMethod key)
        {
            return SubmissionMethodValues.ContainsKey(key) ? SubmissionMethodValues[key] : string.Empty;
        }

        public static string GetRejectionStageDisplayValue(RejectionStage key)
        {
            return RejectionStageValues.ContainsKey(key) ? RejectionStageValues[key] : string.Empty;
        }

        /// <summary>
        /// Returns dispaly value for given month.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetMonthDisplayValue(Month key)
        {
            return MonthValues.ContainsKey(key) ? MonthValues[key] : string.Empty;
        }

        /// <summary>
        /// Return display value for given month.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetPeriodDisplayValue(InvoicePeriod key)
        {
            return InvoicePeriodValues.ContainsKey(key) ? InvoicePeriodValues[key] : string.Empty;
        }

        /// <summary>
        /// Holds mapping of Correspondence Initiating Member values
        /// </summary>
        public static readonly Dictionary<CorrespondenceInitiatingMember, string> CorrespondenceInitiatingMemberValues = new Dictionary<CorrespondenceInitiatingMember, string>
                                                                                        {
                                                                                          { CorrespondenceInitiatingMember.Self, "Self" },
                                                                                          { CorrespondenceInitiatingMember.OtherCarrier, "Other Carrier" },
                                                                                          { CorrespondenceInitiatingMember.Either, "Either" },
                                                                                        };


        /// <summary>
        /// Dictionary for Invoice Types.
        /// </summary>
        public static readonly Dictionary<InvoiceType, string> InvoiceTypeDictionary = new Dictionary<InvoiceType, string>{
                                                                                    {
                                                                                     InvoiceType.Invoice,
                                                                                      "Original Invoice" 
                                                                                    },
                                                                                    {
                                                                                      InvoiceType.RejectionInvoice,
                                                                                      "Rejection Invoice"
                                                                                    },
                                                                                    {
                                                                                      InvoiceType.CorrespondenceInvoice,
                                                                                      "Correspondence Invoice"
                                                                                    },
                                                                                    {
                                                                                      InvoiceType.CreditNote,
                                                                                      "Credit Note"
                                                                                    }
                                                                                  };

        /// <summary>
        /// Holds mapping of settlement method id and displaying settlement method
        /// </summary>
        public static readonly Dictionary<SettlementMethodValues, string> SettlementMethodStatusValues = new Dictionary<SettlementMethodValues, string>
                                                                               {
                                                                                 { SettlementMethodValues.Ich, "ICH" },
                                                                                 { SettlementMethodValues.Ach, "ACH" },
                                                                                 { SettlementMethodValues.ICHAndAch, "ICH & ACH" },
                                                                                 {SettlementMethodValues.Bilateral,"Bilateral"},
                                                                                 {SettlementMethodValues.AchUsingIATARules,"ACH USING IATA RULES (MITA)"}
                                                                               };

        //SCP#436677 - Typo RM BM CM Reports is corrected. "BIlling Memo" was misspelled as "Billng Memo"
        public static readonly Dictionary<TransactionType, string> MemoTypesForReport = new Dictionary<TransactionType, string>
                                                                             {
                                                                               { TransactionType.RejectionMemo3, "Rejection Memo" },
                                                                               { TransactionType.BillingMemo  , "Billing Memo" },
                                                                               { TransactionType.CreditMemo, "Credit Memo" }
                                                                             };
        /// <summary>
        /// Holds mapping of Error Type and displaying Error Type
        /// </summary>
        public static readonly Dictionary<InvoiceStatusType, string> ErrorTypeValues = new Dictionary<InvoiceStatusType, string>
                                                                               {
                                                                                 { InvoiceStatusType.ErrorCorrectable, "Correctable" },
                                                                                 { InvoiceStatusType.ErrorNonCorrectable, "Non Correctable" }
                                                                               };

        public static readonly  Dictionary<ApplicableMinimumField,string> ApplicableAmountFieldValues=new Dictionary<ApplicableMinimumField, string>
                                                                               {
                                                                                {ApplicableMinimumField.AmountToBeSettled,"Amount To be Settled"},
                                                                                {ApplicableMinimumField.TotalAmount,"Total Amount"},
                                                                                {ApplicableMinimumField.TotalGrossDifference,"Total Gross Difference"},
                                                                                {ApplicableMinimumField.TotalNetRejectAmount,"TotalNetRejectAmount"},
                                                                                {ApplicableMinimumField.TotalTaxDifference,"TotalTaxDifference"},
                                                                                {ApplicableMinimumField.TotalWeightChargesValueChargesDiff,"TotalWeightChargesValueChargesDiff"}
                                                                               };

        /// <summary>
        /// Ich Zone List without zone C and display values in ICH Tab at member profile screen.
        /// </summary>
        public static readonly Dictionary<int, string> IchZoneList = new Dictionary<int, string>
                                                                                        {
                                                                                          { (int)IchZones.A, IchZones.A.ToString() },
                                                                                          { (int)IchZones.B, IchZones.B.ToString() },
                                                                                          { (int)IchZones.D, IchZones.D.ToString() },
                                                                                        };


      // CMP # 533: RAM A13 New Validations and New Charge Code [Start] 
      /// <summary>
      /// Product List for Billing Category = Misc, ChargeCategory = Service Provider and Charge Code = GDS
      /// </summary>
      public static readonly Dictionary<ProductId, string> ProductIdList = new Dictionary<ProductId, string>
                                                                             {
                                                                               {ProductId.GDF1, "GDF1"},
                                                                               {ProductId.ICP1, "ICP1"},
                                                                               {ProductId.IHH1, "IHH1"},
                                                                               {ProductId.GSS1, "GSS1"}
                                                                             };

      // CMP # 533: RAM A13 New Validations and New Charge Code [End] 
    }
}

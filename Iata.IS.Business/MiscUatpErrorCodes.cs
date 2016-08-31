using System;

namespace Iata.IS.Business
{
    /// <summary>
    /// Holds the Error code for the Business logic validation intended for Misc/UATP Billing Code.
    /// </summary>
    public class MiscUatpErrorCodes 
    {
        /// <summary>
        /// TotalTaxAmount should be equal to sum of Calculated amount in TaxBreakdown
        /// </summary>
        public virtual string InvalidTotalTaxAmount { get { return "BMISC_10101"; } }

        /// <summary>
        /// TotalVatAmount should be equal to sum of Calculated amount in TaxBreakdown
        /// </summary>
        public virtual string InvalidTotalVatAmount { get { return "BMISC_10102"; } }

        /// <summary>
        /// TotalAddOnChargeAmount should be equal to sum of Calculated amount in TaxBreakdown
        /// </summary>
        public virtual string InvalidTotalAddOnChargeAmount { get { return "BMISC_10103"; } }

        /// <summary>
        /// ‘TotalAmount’ at Invoice Summary level should be equal to ‘TotalLineItemAmount’+’TotalTaxAmountes+’TotalVATAmount
        /// +±ATotalAddOnChargeAmount’ at the Invoice summary level.
        /// </summary>
        public virtual string InvalidTotalAmount { get { return "BMISC_10104"; } }

        public virtual string InvalidCityAirport { get { return "BMISC_10105"; } }

        public virtual string InvalidRejectionInvoiceNumber { get { return "BMISC_10106"; } }

        public virtual string InvalidRejectionInvoiceTotalAmount { get { return "BMISC_10107"; } }

        public virtual string InvalidBilledMemberMigrationStatusForRejectedInvoice { get { return "BMISC_10108"; } }

        public virtual string InvalidTotalAmountInClearanceCurrency { get { return "BMISC_10109"; } }

        public virtual string InvalidServiceDateRange { get { return "BMISC_10110"; } }

        public virtual string InvalidLineTotal { get { return "BMISC_10111"; } }

        public virtual string InvalidRejectionOnValidationFailureStatus { get { return "BMISC_10112"; } }

        public virtual string InvalidRejectionStage { get { return "BMISC_10113"; } }

        public virtual string InvalidClearanceCurrency { get { return "BMISC_10114"; } }

        public virtual string InvalidSettlementMonthPeriod { get { return "BMISC_10115"; } }

        public virtual string InvalidInvoiceDate { get { return "BMISC_10116"; } }

        /// <summary>
        /// Used when invoice has been already rejected.
        /// </summary>
        public virtual string InvoiceAlreadyRejected { get { return "BMISC_10118"; } }

        public virtual string InvalidAmountToBeBilled { get { return "BMISC_10123"; } }

        public virtual string InvalidStartDate { get { return "BMISC_10125"; } }

        public virtual string InvalidChargeAmount { get { return "BMISC_10126"; } }

        //Amount to be settled should not be blank

        public virtual string InvalidCurrency { get { return "BMISC_10124"; } }

        public virtual string InvalidTaxSubType { get { return "BMISC_10127"; } }

        public virtual string InvalidAddOnChargeAmount { get { return "BMISC_10129"; } }

        public virtual string MandatoryRecGroupsMissing { get { return "BMISC_10130"; } }

        public virtual string InvalidParentChildRelation { get { return "BMISC_10131"; } }

        /// <summary>
        /// LineItem Number should be > 0
        /// </summary>
        public virtual string InvalidLineItemNumber { get { return "BMISC_10132"; } }

        /// <summary>
        /// PO LineItem Number >0
        /// </summary>
        public virtual string InvalidPOLineItemNumber { get { return "BMISC_10133"; } }

        /// <summary>
        /// Charge code is invalid for the defined Charge Category
        /// </summary>
       // public virtual string InvalidChargeCode { get { return "BMISC_10134"; } }

        /// <summary>
        /// Charge code is invalid for the defined Charge Category
        /// </summary>
        public virtual string ChargeCodeIsNotPresentInDatabase { get { return "BMISC_10243"; } }

        /// <summary>
        /// Total Net Amount should be equal to ChargeAmount+TotalTaxAmount+TotalVATAmount+TotalAddOnChargeAmount
        /// </summary>
        public virtual string InvalidTotalNetAmount { get { return "BMISC_10135"; } }

        /// <summary>
        /// Original LineItem Number should be > 0
        /// </summary>
        public virtual string InvalidOriginalLineItemNumber { get { return "BMISC_10146"; } }

        /// <summary>
        /// TaxAmount Should be equal to the TaxPercent into TaxableAmount if TaxPercent is provided
        /// </summary>
        public virtual string InvalidTaxAmount { get { return "BMISC_10137"; } }

        /// <summary>
        /// Validation will fail if PaymentDiscountPercent > 100
        /// </summary>
        public virtual string InvalidPaymentDiscountPercent { get { return "BMISC_10153"; } }

        /// <summary>
        /// Validation will fail if PaymentDiscountDueDays > 366
        /// </summary>
        public virtual string InvalidPaymentDiscountDueDays { get { return "BMISC_10154"; } }

        /// <summary>
        /// Validation will fail if PaymentNetDueDays > 366
        /// </summary>
        public virtual string InvalidPaymentNetDueDays { get { return "BMISC_10155"; } }

        /// <summary>
        /// Validation will fail if all filed from Rejection details are not provided.
        /// </summary>
        public virtual string InvalidRejectionDetails { get { return "BMISC_10156"; } }

        /// <summary>
        /// Validation will fail if ChargeCategory is not provided.
        /// </summary>
        public virtual string InvalidChargeCategory { get { return "BMISC_10157"; } }

        /// <summary>
        /// Validation will fail if DigitalSignitureFlag is not provided.
        /// </summary>
        public virtual string InvalidDigitalSignatureFlag { get { return "BMISC_10158"; } }

        /// <summary>
        /// Validation will fail if TotalLineItemAmount is not valid.
        /// </summary>
        public virtual string InvalidTotalLineItemAmount { get { return "BMISC_10159"; } }

        /// <summary>
        /// Used when contact type name entered while creating new contact type already exists in database
        /// </summary>
        public virtual string InvalidMemberContactType { get { return "BMISC_10160"; } }
       
        /// <summary>
        /// Used when PaymentDetailsSection is invalid for settlement method other than B.
        /// </summary>
        public virtual string InvalidPaymentDetailsSection { get { return "BMISC_10161"; } }

        /// <summary>
        /// Used when Line Item start date is not given and line item detail start date is given.
        /// </summary>
        public virtual string InvalidLineItemDetailStartDate { get { return "BMISC_10152"; } }

        /// <summary>
        /// Used when IataAirPrtCityLocationCode is invalid for.
        /// </summary>
        public virtual string InvalidIataAirPrtCityLocationCode { get { return "BMISC_10162"; } }

        /// <summary>
        /// Used when OtherOrganizationType is invalid when account details are available.
        /// </summary>
        public virtual string InvalidOtherOrganizationAccountDetails { get { return "BMISC_10163"; } }

        /// <summary>
        /// Used when Uom Code is Invalid.
        /// </summary>  
        public virtual string InvalidUomCode { get { return "BMISC_10164"; } }
        public virtual string DuplicateCorrspondenceNumber { get { return "BMISC_10274"; } }

    //Amount to be settled should not be blank

        /// <summary>
        /// Used when Line Item Numbers are not in order/Linear.
        /// </summary>  
        public virtual string InvalidLineItemNumberOrder { get { return "BMISC_10165"; } }

        /// <summary>
        /// Used when Line Item Detail Numbers are duplicate.
        /// </summary>  
        public virtual string DuplicateLineItemNumber { get { return "BMISC_10166"; } }

        /// <summary>
        ///  Used when Line Item Detail Numbers are not in order/Linear.
        /// </summary>  
        public virtual string InvalidLineItemDetailNumberOrder { get { return "BMISC_10167"; } }

        /// <summary>
        /// Used when Line Item Detail Numbers are duplicate.
        /// </summary>  
        public virtual string DuplicateLineItemDetailNumber { get { return "BMISC_10168"; } }

        /// <summary>
        /// Used to validate location defined at LineItem level.
        /// </summary>
        public virtual string InvalidLineItemLocationCode { get { return "BMISC_10169"; } }

        /// <summary>
        /// Should allow entry in LineItem field only if a PO Number has been populated in the invoice header.
        /// </summary>
        public virtual string InvalidLineItemPOLineItemNumber { get { return "BMISC_10170"; } }

        /// <summary>
        /// This should show only when there is a Charge Code Type associated with the Charge Code
        /// </summary>
        public virtual string InvalidChargeCodeTypeForChargeCode { get { return "BMISC_10171"; } }

        /// <summary>
        /// Invalid charge code type validation
        /// </summary>
        public virtual string InvalidChargeCodeType { get { return "BMISC_10172"; } }

        /// <summary>
        /// validation :- This should be displayed only for a Rejection Invoice
        /// </summary>
        public virtual string InvalidLineItemOriginalLineItemNumber { get { return "BMISC_10173"; } }

        /// <summary>
        /// validation :- This will be shown only if there are no line item details required for the Charge Category Charge Code. 
        /// </summary>
        public virtual string InvalidMinimumQuantityFlag { get { return "BMISC_10174"; } }

        /// <summary>
        /// Used for LineItem and LineItem details quantity calculations
        /// </summary>
        public virtual string InvalidLineItemQuantity { get { return "BMISC_10175"; } }

        /// <summary>
        ///  Used for LineItem and LineItem details Unit Price calculations
        /// </summary>
        public virtual string InvalidLineItemUnitPrice { get { return "BMISC_10176"; } }

        /// <summary>
        ///  Used for LineItem and LineItem details Total calculations
        /// </summary>
        public virtual string InvalidLineItemTotal { get { return "BMISC_10177"; } }

        /// <summary>
        ///validation :- should be same for LineItem and LineItemDetails
        /// </summary>
        public virtual string InvalidScalingFactor { get { return "BMISC_10178"; } }

        /// <summary>
        /// should be same for LineItem and LineItemDetails
        /// </summary>
        public virtual string InvalidLineItemUomCode { get { return "BMISC_10179"; } }

        /// <summary>
        ///validation :- UomCode should be 'EA'
        /// </summary>
        public virtual string InvalidLineItemConstUomCode { get { return "BMISC_10180"; } }

        /// <summary>
        ///validation :- should be 1
        /// </summary>
        public virtual string InvalidConstScalingFactor { get { return "BMISC_10181"; } }

        /// <summary>
        /// Used when -
        /// Line Item Detail has date range, user can not remove start date from line item.
        /// </summary>
        public virtual string InvalidStartDateForLineItemDetailWithStartDate { get { return "BMISC_10182"; } }

        /// <summary>
        /// If user removes start date from line item detail while lineItem details exists for it, then allow if all line item detail having same End Date.
        /// </summary>
        public virtual string InvalidStartDateForLineItemDetailEndDateNotSame { get { return "BMISC_10183"; } }

        /// <summary>
        /// Rejection invoice number cannot be correspondence.
        /// </summary>
        public virtual string InvalidRejectionInvoiceNumberIfCorrospondence { get { return "BMISC_10184"; } }

        /// <summary>
        /// Invoice already created for Correspondence Reference Number.
        /// </summary>
        public virtual string DuplicateInvoiceForCorrespondenceRefNo { get { return "BMISC_10185"; } }

        /// <summary>
        /// For Credit note invoice the value of the fields ‘Rejection Flag’ and ‘Correspondence Flag’ should not be ‘Y’ 
        /// </summary>
        public virtual string InvalidInvoiceType { get { return "BMISC_10186"; } }

        /// <summary>
        /// For Credit Note	Rejection and Correspondence Section will not be shown.
        /// </summary>
        public virtual string InvalidRejectionCorrespondenceNodes { get { return "BMISC_10187"; } }



        public virtual string InvalidTotalAmountOutsideLimit { get { return "BMISC_10189"; } }

        /// <summary>
        /// If Correspondence amount settled is invalid
        /// </summary>
        public virtual string InvalidCorrspondenceAmountSettled { get { return "BMISC_10190"; } }

        public virtual string InvalidMemberLocationInformation { get { return "BMISC_10191"; } }

        /// <summary>
        /// Used when the invoice of type correspondence is already created.
        /// </summary>
        public virtual string InvoiceAlreadyCreatedForCorrespondence { get { return "BMISC_10192"; } }

        /// <summary>
        /// Used when the invoice doest not exist mentioned under correspondence section.
        /// </summary>
        public virtual string InvoiceNotExistForCorrespondence { get { return "BMISC_10193"; } }

        /// <summary>
        /// Used when  Specified Correspondence is in open state but authority to bill is not set.
        /// </summary>
        public virtual string InvalidCorrespondenceStatusAuthorityToBill { get { return "BMISC_10194"; } }

        /// <summary>
        /// Used when  Invalid Billed or Billing Member for Specified Correspondence Reference Number.
        /// </summary>
        public virtual string InvalidCorrespondenceFromTo { get { return "BMISC_10195"; } }

        /// <summary>
        /// Used when Correspondence is not Expired.
        /// </summary>
        public virtual string CorrespondenceNotExpired { get { return "BMISC_10196"; } }

        /// <summary>
        /// Used when mandatory field is missing.
        /// </summary>
        public virtual string MandatoryFieldAttributeMissing { get { return "BMISC_10197"; } }

        /// <summary>
        /// Used if correspondence invoice is being rejected.
        /// </summary>
        public virtual string CorrespondenceInvoiceCannotBeRejected { get { return "BMISC_10198"; } }

        /// <summary>
        /// Used if exchange rate is zero.
        /// </summary>
        public virtual string InvalidExchangeRate { get { return "BMISC_10199"; } }
       
        /// <summary>
        /// Used if exchange rate is zero.
        /// //SCP279473 - Misc credit note- inconsistency between ISWEB and File behavior
        //Exchange rate for Misc Credit note, should behave like a Bileatral. So it's exchange rate must be same and no correction should be done.
        /// </summary>
        public virtual string ExchangeRateZero { get { return "BMISC_10803"; } }


        public virtual string InvalidCorrRefNo { get { return "BMISC_10200"; } }

        public virtual string CorrRefNoClosed { get { return "BMISC_10201"; } }

        public virtual string OpenInvoiceCannotBeRejected { get { return "BMISC_10202"; } }

        /// <summary>
        /// Location code is required at Invoice or Line Item.
        /// </summary>
        public virtual string LocationCodeRequiredAtInvoiceOrLineItem { get { return "BMISC_10203"; } }

        /// <summary>
        /// Location code is required at line item.
        /// </summary>
        public static string LocationCodeRequiredAtLineItem { get { return "BMISC_10204"; } }

        public virtual string StageTwoRejectionCannotBeRejected { get { return "BMISC_10205"; } }

        public virtual string InvoiceAlreadyRejectedForPayables { get { return "BMISC_10206"; } }

        //SCP0000:Impact on MISC/UATP rejection linking due to purging
        public virtual string InvoicePurged { get { return "BMISC_10763"; } }

        //public virtual string InvoiceRejectedForPayablesAndStatusIsPresented { get { return "BMISC_10231"; } }

        public virtual string UomCodeRequired { get { return "BMISC_10207"; } }

        public virtual string InvalidDSFlagAsCountryNotSpecified { get { return "BMISC_10208"; } }

        public virtual string InvalidCharAmountForLinkedLineItemDetail { get { return "BMISC_10209"; } }

        /// <summary>
        /// IsDigitalSigniture required should be true for billing member in eBilling configuration.
        /// </summary>
        public virtual string InvalidDigitalSignatureRequired { get { return "BMISC_10210"; } }

        /// <summary>
        /// Used for Invalid amount considering Min - Max amount in master for the specific transaction Type.
        /// </summary>
        public virtual string InvalidMinMaxAmount { get { return "BMISC_10211"; } }

        /// <summary>
        /// Used when BRD doesn't contain the invoice mentioned in rejection invoice detail section.
        /// </summary>
        public virtual string RejectionInvoiceNumberNotExist { get { return "BMISC_10212"; } }

        /// <summary>
        /// Used when invalid alphabetic value.
        /// </summary>
        public virtual string InvalidADataValue { get { return "BMISC_10213"; } }

        /// <summary>
        /// Used when invalid Numeric value.
        /// </summary>
        public virtual string InvalidNDataValue { get { return "BMISC_10214"; } }

        /// <summary>
        /// Used when invalid Positive numeric value.
        /// </summary>
        public virtual string InvalidPnDataValue { get { return "BMISC_10215"; } }

        /// <summary>
        /// Used when invalid Date value.
        /// </summary>
        public virtual string InvalidDateDataValue { get { return "BMISC_10216"; } }

        /// <summary>
        /// Used when Credit Note total amount is non-negative.
        /// </summary>
        public virtual string InvalidCreditNoteTotalAmount { get { return "BMISC_10217"; } }

        public virtual string InvalidAttributeValue { get { return "BMISC_10218"; } }

        public virtual string InvalidLocationCodeIcao { get { return "BMISC_10219"; } }

        public virtual string InvalidCountryCodeIcao { get { return "BMISC_10220"; } }

        public virtual string InvalidLocationCodeIcaoType { get { return "BMISC_10221"; } }

        public virtual string InvalidCountryCodeIcaoType { get { return "BMISC_10222"; } }

        public virtual string InvalidRouteDateTimeType { get { return "BMISC_10223"; } }

        public virtual string InvalidWaypointCodeType { get { return "BMISC_10224"; } }

        public virtual string InvalidRouteDataType { get { return "BMISC_10225"; } }

        public virtual string InvalidLocationType { get { return "BMISC_10226"; } }

        public virtual string InvalidCountryCode { get { return "BMISC_10227"; } }

        public virtual string InvalidDistanceUomcode { get { return "BMISC_10228"; } }

        public virtual string LocationOrLocationCodeIcaoRequired { get { return "BMISC_10229"; } }
        public virtual string AircraftTypeCodeOrAircraftTypeCodeIcaoRequired { get { return "BMISC_10230"; } }

        public virtual string InvoiceTotalAmountNegative { get { return "BMISC_10232"; } }

        public virtual string CreditNoteRejectionMessage { get { return "BMISC_10233"; } }

        public virtual string InvalidStationCode { get { return "BMISC_10234"; } }

        public virtual string InvalidReferenceNumber { get { return "BMISC_10235"; } }

        public virtual string InvalidMaxTakeOffWeight { get { return "BMISC_10236"; } }

        public virtual string TimeLimitExpiryForCorrespondence { get { return "BMISC_10238"; } }

        public virtual string MemberIsNotMigrated { get { return "BMISC_10241"; } }

        /// <summary>
        /// To check whether lineItem detail is compulsory
        /// </summary>  
        public virtual string MandatoryLineItemDetail { get { return "BMISC_10239"; } }

        public virtual string LineItemDetailExpectedButNotFound { get { return "BMISC_10240"; } }

        public virtual string InvalidChargeCateGoryAndCodeForOnBehalfFile { get { return "BMISC_10242"; } }

        public virtual string InvalidAircraftTypeCode { get { return "BMISC_10244"; } }

        public virtual string InvalidAircraftTypeIcaoCode { get { return "BMISC_10245"; } }

        public virtual string InvalidAmountName { get { return "BMISC_10246"; } }

        public virtual string InvalidPartialPaymentsAttribute { get { return "BMISC_10247"; } }

        public virtual string InvalidAreaSizeUomcode { get { return "BMISC_10248"; } }

        public virtual string FieldValueRequiredForAttribute { get { return "BMISC_10249"; } }

        public virtual string InvalidBillingBurrencyOfRejectedInvoice { get { return "BMISC_10250"; } }

        public virtual string InvalidBillingBurrencyOfCorrInvoice { get { return "BMISC_10251"; } }

        public virtual string ExchangeRateIsMadatoryForAmountInClearanceCurrency { get { return "BMISC_10252"; } }

        public virtual string InvoiceIsNotRejectedInvoice { get { return "BMISC_10253"; } }

        public virtual string ExchangeRateCannotBeZero { get { return "BMISC_10254"; } }

        public virtual string InvalidSuspendedAirline { get { return "BMISC_10255"; } }

        public const string RejInvNotFoundForCorrRefNo = "RejInvNotFoundForCorrRefNo";

        public const string CorrRefNoOpen = "CorrRefNoOpen";

        public const string CorrRefNoInvalidMembers = "CorrRefNoInvalidMembers";

        public virtual string DuplicateFileName { get { return "BMISC_10256"; } }

        public virtual string VoidPeriodValidationMsg { get { return "BMISC_10257"; } }

        public virtual string InvoiceLateSubmitted { get { return "BMISC_10258"; } }

        public virtual string InvalidBillingPeriod { get { return "BMISC_10259"; } }

        public virtual string InvalidBillingIsMembershipStatus { get { return "BMISC_10260"; } }

        public virtual string InvalidBilledIsMembershipStatus { get { return "BMISC_10261"; } }

        public virtual string TransactionLineItemNotAvailable { get { return "BMISC_10262"; } }

        public virtual string SameBillingAndBilledMember { get { return "BMISC_10263"; } }

        public virtual string InvalidDigitalSignatureValue { get { return "BMISC_10264"; } }

        public virtual string InvalidBillingMember { get { return "BMISC_10265"; } }

        public virtual string InvalidBilledMember { get { return "BMISC_10266"; } }

        public virtual string InvalidSettlementMethod { get { return "BMISC_10267"; } }

        public virtual string InvalidBillingCurrency { get { return "BMISC_10268"; } }

        public virtual string InvalidBillingMemberStatus { get { return "BMISC_10269"; } }

        public virtual string InvalidBilledMemberStatus { get { return "BMISC_10270"; } }

        public virtual string InvalidBillingFromMember { get { return "BMISC_10271"; } }

        public virtual string InvoiceValidForLateSubmission { get { return "BMISC_10272"; } }

        public virtual string InvalidLanguage { get { return "BMISC_10273"; } }
       
        public virtual string ErrorCorrespondenceAlreadyCreated { get { return "BMISC_10275"; } }

        public const string InvalidTaxCategory = "BMISC_10276";

        public virtual string ListingCurrencyMustHaveValue { get { return "BMISC_10277"; } }

        public virtual string BillingCurrencyMustHaveValue { get { return "BMISC_10278"; } }

        public virtual string RequiredTotalAmountInBillingCurrency { get { return "BMISC_10279"; } }
     
        public const string InvoiceNumberMandatory = "BMISC_10282";

        public const string SettlementPeriodMandatory = "BMISC_10281";

        public const string RequiredChargeCodeType = "BMISC_10283";

        public const string TransactionUdatedByOtherUser = "BMISC_10285";

        public const string TotalAmountAndBreakdownMisMatch = "BMISC_10288";

        public virtual string InvalidRejectionStageOfCurrentRejection { get { return "BMISC_10284"; } }

        public static string LocationCodeNotRequiredAtInvoiceLevel { get { return "BMISC_10301"; } }

        public const string InvalidInvoiceMemberLocationInformation = "BMISC_10761";

        public const string RejectionInvoiceNumberMandatory = "BMISC_10291";

        public const string RejectionSettlementPeriodMandatory = "BMISC_10292";

        public const string InvalidRejectionDetailStage = "BMISC_10293";

        /// <summary>
        /// Validation error code: Scaling factor can not be '0'.
        /// </summary>
        // SCP220346: Inward Billing-XML file mandatory field.
        public virtual string InvalidScalingFactorValue {
          get {
          return "BMISC_10302";
          }
        }

        public const string InvalidFieldValue = "BMISC_10303";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #5
        /// </summary>
        public const string MuRejctionInvoiceLinkingCheckForSmiX = "BMISC_10784";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #9
        /// </summary>
        public const string MuRejctionInvoiceSmiCheckForSmiX = "BMISC_10785";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #6
        /// </summary>
        public const string MuCorrespondenceInvoiceLinkingCheckForSmiX = "BMISC_10786";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #10
        /// </summary>
        public const string MuCorrespondenceInvoiceSmiCheckForSmiX = "BMISC_10787";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #11
        /// </summary>
        public const string MuClearanceCurrencyCheckForSmiX = "BMISC_10788";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #11
        /// </summary>
        public const string MuExchangeRateCheckForSmiX = "BMISC_10789";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.11,  New Validation #8
        /// </summary>
        public const string MuRejctionInvoiceSmiCheckForSmiOtherThanX = "BMISC_10790";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.11,  New Validation #9
        /// </summary>
        public const string MuCorrespondenceInvoiceLinkingCheckForSmiOtherThanX = "BMISC_10791";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.17,  Change #16
        /// </summary>
        public const string MuInvoiceSmiChangeXtoOtherSmi = "BMISC_10792";

        /// <summary>
        ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.17,   Change #17
        /// </summary>
        public const string MuInvoiceSmiChangeOtherSmiToX = "BMISC_10793";

        /// <summary>
        /// SCP327666 - AIATSL - Query about SIS charge category/code for invoice MA68123027
        /// </summary>
        public static string ChargeCategoryMismatchWithChargeCodeHeaderLevel { get { return "BMISC_10799"; } }
        
        /// <summary>
        /// SCP327666 - AIATSL - Query about SIS charge category/code for invoice MA68123027
        /// </summary>
        public static string ChargeCategoryMismatchWithChargeCodeLineItemLevel { get { return "BMISC_10800"; } }
        // SCP301747: SRM: MiscData @Name is not mandatory in IS-WEB
        /// <summary>
        /// Name attribute is required for the field Misc Data.
        /// </summary>
        public virtual string NameAttributeIsRequiredForTheFieldMiscData { get { return "BMISC_10805"; } }
        public virtual string FieldMiscDataIsRequiredForTheAttribute { get { return "BMISC_10806"; } }
        public virtual string FieldMiscDataAndAttributeIsRequiredForTheAttribute { get { return "BMISC_10807"; } }

        /// <summary>
        ///CMP #553: ACH Requirement for Multiple Currency Handling Validation #2
        /// </summary>
        public const string InvalidOrInactiveCurrencyOfClearance = "BMISC_21812";

        /// <summary>
        ///CMP #553: ACH Requirement for Multiple Currency Handling Validation #3
        /// </summary>
        public const string CurrencyCodeShouldBeSameAsClearanceCurrencyCode = "BMISC_21813";

        /// <summary>
        ///CMP #553: ACH Requirement for Multiple Currency Handling Validation #4 for isweb
        /// </summary>
        public const string CurrencyOfBillingShouldBeSameAsCurrencyOfClearance = "BMISC_21815";

        /// <summary>
        /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
        /// </summary>
        public const string ParsedRejInvTimeLimitRecNotFound = "BMISC_21816";
        
        /// <summary>
        /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
        /// </summary>
        public const string ParsedRejInvTimeLimitValidationFailed = "BMISC_21817";

        /// <summary>
        /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
        /// </summary>
        public const string ErrorCorrectionRejInvTimeLimitRecNotFound = "BMISC_21818";

        /// <summary>
        /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
        /// </summary>
        public const string ErrorCorrectionRejInvTimeLimitValidationFailed = "BMISC_21819";

        /// <summary>
        /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
        /// </summary>
        public const string RejInvTimeLimitRecNotFound = "BMISC_21820";

        /// <summary>
        /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
        /// </summary>
        public const string RejInvTimeLimitValidationFailed = "BMISC_21821";

        /// <summary>
        /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
        /// </summary>
        public const string BillingHistoryRejInvTimeLimitRecNotFound = "BMISC_21822";

        /// <summary>
        /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
        /// </summary>
        public const string BillingHistoryRejInvTimeLimitValidationFailed = "BMISC_21823";

        /// <summary>
        /// SCP485960: Request to re-interface MXMLT-62920160501 file to be uploaded in SAP UNIX Server
        /// </summary>
        public const string CorrespondenceDetailsNotProvidedForMiscCorrInvoice = "BMISC_21824";
    }
}

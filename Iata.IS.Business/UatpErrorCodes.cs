namespace Iata.IS.Business
{
    /// <summary>
    /// Holds the Error code for the Business logic validation intended for UATP Billing Code.
    /// </summary>
    public class UatpErrorCodes : MiscUatpErrorCodes
    {
        /// <summary>
        /// TotalTaxAmount should be equal to sum of Calculated amount in TaxBreakdown
        /// </summary>
        public override string InvalidTotalTaxAmount { get { return "BUATP_10101"; } }


        /// <summary>
        /// TotalVatAmount should be equal to sum of Calculated amount in TaxBreakdown
        /// </summary>
        public override string InvalidTotalVatAmount { get { return "BUATP_10102"; } }

        /// <summary>
        /// TotalAddOnChargeAmount should be equal to sum of Calculated amount in TaxBreakdown
        /// </summary>
        public override string InvalidTotalAddOnChargeAmount { get { return "BUATP_10103"; } }

        /// <summary>
        /// ‘TotalAmount’ at Invoice Summary level should be equal to ‘TotalLineItemAmount’+’TotalTaxAmountes+’TotalVATAmount
        /// +±ATotalAddOnChargeAmount’ at the Invoice summary level.
        /// </summary>
        public override string InvalidTotalAmount { get { return "BUATP_10104"; } }

        public override string InvalidCityAirport { get { return "BUATP_10105"; } }

        public override string InvalidRejectionInvoiceNumber { get { return "BUATP_10106"; } }

        public override string InvalidRejectionInvoiceTotalAmount { get { return "BUATP_10107"; } }

        public override string InvalidBilledMemberMigrationStatusForRejectedInvoice { get { return "BUATP_10108"; } }

        public override string InvalidTotalAmountInClearanceCurrency { get { return "BUATP_10109"; } }

        public override string InvalidServiceDateRange { get { return "BUATP_10110"; } }

        public override string InvalidLineTotal { get { return "BUATP_10111"; } }

        public override string InvalidRejectionOnValidationFailureStatus { get { return "BUATP_10112"; } }

        public override string InvalidRejectionStage { get { return "BUATP_10113"; } }

        public override string InvalidClearanceCurrency { get { return "BUATP_10114"; } }

        public const string EnterEmailIds = "BUATP_10117";

        public override string InvalidSettlementMonthPeriod { get { return "BUATP_10115"; } }

        public override string InvalidInvoiceDate { get { return "BUATP_10116"; } }

        /// <summary>
        /// Used when invoice has been already rejected.
        /// </summary>
        public override string InvoiceAlreadyRejected { get { return "BUATP_10118"; } }

        public const string InvalidCorrespondenceSubject = "BUATP_10119"; 

        public const string InvalidAmountToBeSettled  = "BUATP_10121"; 

        public const string InvalidCorrespondenceNumber = "BUATP_10120"; 

        public const string ExpiredCorrespondence = "BUATP_10122"; 

        public override string InvalidAmountToBeBilled { get { return "BUATP_10123"; } }

        public override string InvalidStartDate { get { return "BUATP_10125"; } }

        public override string InvalidChargeAmount { get { return "BUATP_10126"; } }

        //Amount to be settled should not be blank

        public override string InvalidCurrency { get { return "BUATP_10124"; } }

        public override string InvalidTaxSubType { get { return "BUATP_10127"; } }

        public const string InvalidAuthorityToBill = "BUATP_10128";

        public override string InvalidAddOnChargeAmount { get { return "BUATP_10129"; } }

        public override string MandatoryRecGroupsMissing { get { return "BUATP_10130"; } }

        public override string InvalidParentChildRelation { get { return "BUATP_10131"; } }

        /// <summary>
        /// LineItem Number should be > 0
        /// </summary>
        public override string InvalidLineItemNumber { get { return "BUATP_10132"; } }

        /// <summary>
        /// PO LineItem Number >0
        /// </summary>
        public override string InvalidPOLineItemNumber { get { return "BUATP_10133"; } }

        /// <summary>
        /// Charge code is invalid for the defined Charge Category
        /// </summary>
        public const string InvalidChargeCode = "BUATP_10134"; 

        /// <summary>
        /// Charge code is invalid for the defined Charge Category
        /// </summary>
        public override string ChargeCodeIsNotPresentInDatabase { get { return "BUATP_10243"; } }

        /// <summary>
        /// Total Net Amount should be equal to ChargeAmount+TotalTaxAmount+TotalVATAmount+TotalAddOnChargeAmount
        /// </summary>
        public override string InvalidTotalNetAmount { get { return "BUATP_10135"; } }

        /// <summary>
        /// Original LineItem Number should be > 0
        /// </summary>
        public override string InvalidOriginalLineItemNumber { get { return "BUATP_10146"; } }

        /// <summary>
        /// TaxAmount Should be equal to the TaxPercent into TaxableAmount if TaxPercent is provided
        /// </summary>
        public override string InvalidTaxAmount { get { return "BUATP_10137"; } }

        /// <summary>
        /// Validation will fail if PaymentDiscountPercent > 100
        /// </summary>
        public override string InvalidPaymentDiscountPercent { get { return "BUATP_10153"; } }

        /// <summary>
        /// Validation will fail if PaymentDiscountDueDays > 366
        /// </summary>
        public override string InvalidPaymentDiscountDueDays { get { return "BUATP_10154"; } }

        /// <summary>
        /// Validation will fail if PaymentNetDueDays > 366
        /// </summary>
        public override string InvalidPaymentNetDueDays { get { return "BUATP_10155"; } }

        /// <summary>
        /// Validation will fail if all filed from Rejection details are not provided.
        /// </summary>
        public override string InvalidRejectionDetails { get { return "BUATP_10156"; } }

        /// <summary>
        /// Validation will fail if ChargeCategory is not provided.
        /// </summary>
        public override string InvalidChargeCategory { get { return "BUATP_10157"; } }

        /// <summary>
        /// Validation will fail if DigitalSignitureFlag is not provided.
        /// </summary>
        public override string InvalidDigitalSignatureFlag { get { return "BUATP_10158"; } }

        /// <summary>
        /// Validation will fail if TotalLineItemAmount is not valid.
        /// </summary>
        public override string InvalidTotalLineItemAmount { get { return "BUATP_10159"; } }

        /// <summary>
        /// Used when contact type name entered while creating new contact type already exists in database
        /// </summary>
        public override string InvalidMemberContactType { get { return "BUATP_10160"; } }

        public const string FailedToSendMail = "BUATP_10151"; 

        /// <summary>
        /// Used when PaymentDetailsSection is invalid for settlement method other than B.
        /// </summary>
        public override string InvalidPaymentDetailsSection { get { return "BUATP_10161"; } }

        /// <summary>
        /// Used when Line Item start date is not given and line item detail start date is given.
        /// </summary>
        public override string InvalidLineItemDetailStartDate { get { return "BUATP_10152"; } }

        /// <summary>
        /// Used when IataAirPrtCityLocationCode is invalid for.
        /// </summary>
        public override string InvalidIataAirPrtCityLocationCode { get { return "BUATP_10162"; } }

        /// <summary>
        /// Used when OtherOrganizationType is invalid when account details are available.
        /// </summary>
        public override string InvalidOtherOrganizationAccountDetails { get { return "BUATP_10163"; } }

        /// <summary>
        /// Used when Uom Code is Invalid.
        /// </summary>  
        public override string InvalidUomCode { get { return "BUATP_10164"; } }

        /// <summary>
        /// Used when Line Item Numbers are not in order/Linear.
        /// </summary>  
        public override string InvalidLineItemNumberOrder { get { return "BUATP_10165"; } }

        /// <summary>
        /// Used when Line Item Detail Numbers are duplicate.
        /// </summary>  
        public override string DuplicateLineItemNumber { get { return "BUATP_10166"; } }

        /// <summary>
        ///  Used when Line Item Detail Numbers are not in order/Linear.
        /// </summary>  
        public override string InvalidLineItemDetailNumberOrder { get { return "BUATP_10167"; } }

        /// <summary>
        /// Used when Line Item Detail Numbers are duplicate.
        /// </summary>  
        public override string DuplicateLineItemDetailNumber { get { return "BUATP_10168"; } }

        /// <summary>
        /// Used to validate location defined at LineItem level.
        /// </summary>
        public override string InvalidLineItemLocationCode { get { return "BUATP_10169"; } }

        /// <summary>
        /// Should allow entry in LineItem field only if a PO Number has been populated in the invoice header.
        /// </summary>
        public override string InvalidLineItemPOLineItemNumber { get { return "BUATP_10170"; } }

        /// <summary>
        /// This should show only when there is a Charge Code Type associated with the Charge Code
        /// </summary>
        public override string InvalidChargeCodeTypeForChargeCode { get { return "BUATP_10171"; } }

        /// <summary>
        /// Invalid charge code type validation
        /// </summary>
        public override string InvalidChargeCodeType { get { return "BUATP_10172"; } }

        /// <summary>
        /// validation :- This should be displayed only for a Rejection Invoice
        /// </summary>
        public override string InvalidLineItemOriginalLineItemNumber { get { return "BUATP_10173"; } }

        /// <summary>
        /// validation :- This will be shown only if there are no line item details required for the Charge Category Charge Code. 
        /// </summary>
        public override string InvalidMinimumQuantityFlag { get { return "BUATP_10174"; } }

        /// <summary>
        /// Used for LineItem and LineItem details quantity calculations
        /// </summary>
        public override string InvalidLineItemQuantity { get { return "BUATP_10175"; } }

        /// <summary>
        ///  Used for LineItem and LineItem details Unit Price calculations
        /// </summary>
        public override string InvalidLineItemUnitPrice { get { return "BUATP_10176"; } }

        /// <summary>
        ///  Used for LineItem and LineItem details Total calculations
        /// </summary>
        public override string InvalidLineItemTotal { get { return "BUATP_10177"; } }

        /// <summary>
        ///validation :- should be same for LineItem and LineItemDetails
        /// </summary>
        public override string InvalidScalingFactor { get { return "BUATP_10178"; } }

        /// <summary>
        /// should be same for LineItem and LineItemDetails
        /// </summary>
        public override string InvalidLineItemUomCode { get { return "BUATP_10179"; } }

        /// <summary>
        ///validation :- UomCode should be 'EA'
        /// </summary>
        public override string InvalidLineItemConstUomCode { get { return "BUATP_10180"; } }

        /// <summary>
        ///validation :- should be 1
        /// </summary>
        public override string InvalidConstScalingFactor { get { return "BUATP_10181"; } }

        /// <summary>
        /// Used when -
        /// Line Item Detail has date range, user can not remove start date from line item.
        /// </summary>
        public override string InvalidStartDateForLineItemDetailWithStartDate { get { return "BUATP_10182"; } }

        /// <summary>
        /// If user removes start date from line item detail while lineItem details exists for it, then allow if all line item detail having same End Date.
        /// </summary>
        public override string InvalidStartDateForLineItemDetailEndDateNotSame { get { return "BUATP_10183"; } }

        /// <summary>
        /// Rejection invoice number cannot be correspondence.
        /// </summary>
        public override string InvalidRejectionInvoiceNumberIfCorrospondence { get { return "BUATP_10184"; } }

        /// <summary>
        /// Invoice already created for Correspondence Reference Number.
        /// </summary>
        public override string DuplicateInvoiceForCorrespondenceRefNo { get { return "BUATP_10185"; } }

        /// <summary>
        /// For Credit note invoice the value of the fields ‘Rejection Flag’ and ‘Correspondence Flag’ should not be ‘Y’ 
        /// </summary>
        public override string InvalidInvoiceType { get { return "BUATP_10186"; } }

        /// <summary>
        /// For Credit Note	Rejection and Correspondence Section will not be shown.
        /// </summary>
        public override string InvalidRejectionCorrespondenceNodes { get { return "BUATP_10187"; } }



        public override string InvalidTotalAmountOutsideLimit { get { return "BUATP_10189"; } }

        /// <summary>
        /// If Correspondence amount settled is invalid
        /// </summary>
        public override string InvalidCorrspondenceAmountSettled { get { return "BUATP_10190"; } }

        public override string InvalidMemberLocationInformation { get { return "BUATP_10191"; } }

        /// <summary>
        /// Used when the invoice of type correspondence is already created.
        /// </summary>
        public override string InvoiceAlreadyCreatedForCorrespondence { get { return "BUATP_10192"; } }

        /// <summary>
        /// Used when the invoice doest not exist mentioned under correspondence section.
        /// </summary>
        public override string InvoiceNotExistForCorrespondence { get { return "BUATP_10193"; } }

        /// <summary>
        /// Used when  Specified Correspondence is in open state but authority to bill is not set.
        /// </summary>
        public override string InvalidCorrespondenceStatusAuthorityToBill { get { return "BUATP_10194"; } }

        /// <summary>
        /// Used when  Invalid Billed or Billing Member for Specified Correspondence Reference Number.
        /// </summary>
        public override string InvalidCorrespondenceFromTo { get { return "BUATP_10195"; } }

        /// <summary>
        /// Used when Correspondence is not Expired.
        /// </summary>
        public override string CorrespondenceNotExpired { get { return "BUATP_10196"; } }

        /// <summary>
        /// Used when mandatory field is missing.
        /// </summary>
        public override string MandatoryFieldAttributeMissing { get { return "BUATP_10197"; } }

        /// <summary>
        /// Used if correspondence invoice is being rejected.
        /// </summary>
        public override string CorrespondenceInvoiceCannotBeRejected { get { return "BUATP_10198"; } }

        /// <summary>
        /// Used if exchange rate is zero.
        /// </summary>
        public override string InvalidExchangeRate { get { return "BUATP_10199"; } }


        public override string InvalidCorrRefNo { get { return "BUATP_10200"; } }

        public override string CorrRefNoClosed { get { return "BUATP_10201"; } }

        public override string OpenInvoiceCannotBeRejected { get { return "BUATP_10202"; } }

        /// <summary>
        /// Location code is required at Invoice or Line Item.
        /// </summary>
        public override string LocationCodeRequiredAtInvoiceOrLineItem { get { return "BUATP_10203"; } }

        /// <summary>
        /// Location code is required at line item.
        /// </summary>
        public string LocationCodeRequiredAtLineItem { get { return "BUATP_10204"; } }

        public override string StageTwoRejectionCannotBeRejected { get { return "BUATP_10205"; } }

        public override string InvoiceAlreadyRejectedForPayables { get { return "BUATP_10206"; } }

        //public override string InvoiceRejectedForPayablesAndStatusIsPresented { get { return "BUATP_10231"; } }

        public override string UomCodeRequired { get { return "BUATP_10207"; } }

        public override string InvalidDSFlagAsCountryNotSpecified { get { return "BUATP_10208"; } }

        public override string InvalidCharAmountForLinkedLineItemDetail { get { return "BUATP_10209"; } }

        /// <summary>
        /// IsDigitalSigniture required should be true for billing member in eBilling configuration.
        /// </summary>
        public override string InvalidDigitalSignatureRequired { get { return "BUATP_10210"; } }

        /// <summary>
        /// Used for Invalid amount considering Min - Max amount in master for the specific transaction Type.
        /// </summary>
        public override string InvalidMinMaxAmount { get { return "BUATP_10211"; } }

        /// <summary>
        /// Used when BRD doesn't contain the invoice mentioned in rejection invoice detail section.
        /// </summary>
        public override string RejectionInvoiceNumberNotExist { get { return "BUATP_10212"; } }

        /// <summary>
        /// Used when invalid alphabetic value.
        /// </summary>
        public override string InvalidADataValue { get { return "BUATP_10213"; } }

        /// <summary>
        /// Used when invalid Numeric value.
        /// </summary>
        public override string InvalidNDataValue { get { return "BUATP_10214"; } }

        /// <summary>
        /// Used when invalid Positive numeric value.
        /// </summary>
        public override string InvalidPnDataValue { get { return "BUATP_10215"; } }

        /// <summary>
        /// Used when invalid Date value.
        /// </summary>
        public override string InvalidDateDataValue { get { return "BUATP_10216"; } }

        /// <summary>
        /// Used when Credit Note total amount is non-negative.
        /// </summary>
        public override string InvalidCreditNoteTotalAmount { get { return "BUATP_10217"; } }

        public override string InvalidAttributeValue { get { return "BUATP_10218"; } }

        public override string InvalidLocationCodeIcao { get { return "BUATP_10219"; } }

        public override string InvalidCountryCodeIcao { get { return "BUATP_10220"; } }

        public override string InvalidLocationCodeIcaoType { get { return "BUATP_10221"; } }

        public override string InvalidCountryCodeIcaoType { get { return "BUATP_10222"; } }

        public override string InvalidRouteDateTimeType { get { return "BUATP_10223"; } }

        public override string InvalidWaypointCodeType { get { return "BUATP_10224"; } }

        public override string InvalidRouteDataType { get { return "BUATP_10225"; } }

        public override string InvalidLocationType { get { return "BUATP_10226"; } }

        public override string InvalidCountryCode { get { return "BUATP_10227"; } }

        public override string InvalidDistanceUomcode { get { return "BUATP_10228"; } }

        public override string LocationOrLocationCodeIcaoRequired { get { return "BUATP_10229"; } }
        public override string AircraftTypeCodeOrAircraftTypeCodeIcaoRequired { get { return "BUATP_10230"; } }

        public override string InvoiceTotalAmountNegative { get { return "BUATP_10232"; } }

        public override string CreditNoteRejectionMessage { get { return "BUATP_10233"; } }

        public override string InvalidStationCode { get { return "BUATP_10234"; } }

        public override string InvalidReferenceNumber { get { return "BUATP_10235"; } }

        public override string InvalidMaxTakeOffWeight { get { return "BUATP_10236"; } }

        public const string InvalidEmailIds =  "BUATP_10237";

        public override string TimeLimitExpiryForCorrespondence { get { return "BUATP_10238"; } }

        public override string MemberIsNotMigrated { get { return "BUATP_10241"; } }

        /// <summary>
        /// To check whether lineItem detail is compulsory
        /// </summary>  
        public override string MandatoryLineItemDetail { get { return "BUATP_10239"; } }

        public override string LineItemDetailExpectedButNotFound { get { return "BUATP_10240"; } }

        public override string InvalidChargeCateGoryAndCodeForOnBehalfFile { get { return "BUATP_10242"; } }

        public override string InvalidAircraftTypeCode { get { return "BUATP_10244"; } }

        public override string InvalidAircraftTypeIcaoCode { get { return "BUATP_10245"; } }

        public override string InvalidAmountName { get { return "BUATP_10246"; } }

        public override string InvalidPartialPaymentsAttribute { get { return "BUATP_10247"; } }

        public override string InvalidAreaSizeUomcode { get { return "BUATP_10248"; } }

        public override string FieldValueRequiredForAttribute { get { return "BUATP_10249"; } }

        public override string InvalidBillingBurrencyOfRejectedInvoice { get { return "BUATP_10250"; } }

        public override string InvalidBillingBurrencyOfCorrInvoice { get { return "BUATP_10251"; } }

        public override string ExchangeRateIsMadatoryForAmountInClearanceCurrency { get { return "BUATP_10252"; } }

        public override string InvoiceIsNotRejectedInvoice { get { return "BUATP_10253"; } }

        public override string ExchangeRateCannotBeZero { get { return "BUATP_10254"; } }

        public override string InvalidSuspendedAirline { get { return "BUATP_10255"; } }

        public override string DuplicateFileName { get { return "BUATP_10256"; } }

        public override string VoidPeriodValidationMsg { get { return "BUATP_10257"; } }

        public override string InvoiceLateSubmitted { get { return "BUATP_10258"; } }

        public override string InvalidBillingPeriod { get { return "BUATP_10259"; } }

        public override string InvalidBillingIsMembershipStatus { get { return "BUATP_10260"; } }

        public override string InvalidBilledIsMembershipStatus { get { return "BUATP_10261"; } }

        public override string TransactionLineItemNotAvailable { get { return "BUATP_10262"; } }

        public override string SameBillingAndBilledMember { get { return "BUATP_10263"; } }

        public override string InvalidDigitalSignatureValue { get { return "BUATP_10264"; } }

        public override string InvalidBillingMember { get { return "BUATP_10265"; } }

        public override string InvalidBilledMember { get { return "BUATP_10266"; } }

        public override string InvalidSettlementMethod { get { return "BUATP_10267"; } }

        public override string InvalidBillingCurrency { get { return "BUATP_10268"; } }

        public override string InvalidBillingMemberStatus { get { return "BUATP_10269"; } }

        public override string InvalidBilledMemberStatus { get { return "BUATP_10270"; } }

        public override string InvalidBillingFromMember { get { return "BUATP_10271"; } }

        public override string InvoiceValidForLateSubmission { get { return "BUATP_10272"; } }

        public override string InvalidLanguage { get { return "BUATP_10273"; } }

        public override string DuplicateCorrspondenceNumber { get { return "BUATP_10274"; } }

        public override string ErrorCorrespondenceAlreadyCreated { get { return "BUATP_10275"; } }

        public override string ListingCurrencyMustHaveValue { get { return "BUATP_10276"; } }

        public override string BillingCurrencyMustHaveValue { get { return "BUATP_10277"; } }

        public override string RequiredTotalAmountInBillingCurrency { get { return "BUATP_10278"; } }

        /// <summary>
        /// LineItemDetail Number should be > 0
        /// </summary>
        public const string InvalidLineItemDetailNumber = "BUATP_10287";

        public const string InvalidAttachmentIndicatorOriginalValue = "BUATP_10279";
        
        //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
        /// <summary>
        /// Can not reply/save/send/readytosumbit to uatp correspondence. 
        /// </summary>
        public const string CanNotReplyToUatpCorrespondence = "BUATP_10288";

        /// <summary>
        /// Validation error code: Scaling factor can not be '0'.
        /// </summary>
        // SCP220346: Inward Billing-XML file mandatory field.
        public override string InvalidScalingFactorValue {
          get {
          return "BUATP_10289";
          }
        }

        /// <summary>
        /// SCP485960: Request to re-interface MXMLT-62920160501 file to be uploaded in SAP UNIX Server
        /// </summary>
        public const string CorrespondenceDetailsNotProvidedForUatpCorrInvoice = "BUATP_10290";
    }
}

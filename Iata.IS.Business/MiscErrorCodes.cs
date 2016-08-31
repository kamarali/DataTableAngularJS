namespace Iata.IS.Business
{
  /// <summary>
  /// Holds the Error code for the Business logic validation intended for Misc Billing Code.
  /// </summary>
    public class MiscErrorCodes : MiscUatpErrorCodes
  {
      /// <summary>
      /// TotalTaxAmount should be equal to sum of Calculated amount in TaxBreakdown
      /// </summary>
      public override string InvalidTotalTaxAmount { get { return "BMISC_10101"; } }
     

    /// <summary>
    /// TotalVatAmount should be equal to sum of Calculated amount in TaxBreakdown
    /// </summary>
    public override string InvalidTotalVatAmount { get { return "BMISC_10102"; } }

    /// <summary>
    /// TotalAddOnChargeAmount should be equal to sum of Calculated amount in TaxBreakdown
    /// </summary>
    public override string InvalidTotalAddOnChargeAmount { get { return "BMISC_10103"; } }

    /// <summary>
    /// ‘TotalAmount’ at Invoice Summary level should be equal to ‘TotalLineItemAmount’+’TotalTaxAmountes+’TotalVATAmount
    /// +±ATotalAddOnChargeAmount’ at the Invoice summary level.
    /// </summary>
    public override string InvalidTotalAmount { get { return "BMISC_10104"; } }

    public override string InvalidCityAirport { get { return "BMISC_10105"; } }

    public override string InvalidRejectionInvoiceNumber { get { return "BMISC_10106"; } }

    public override string InvalidRejectionInvoiceTotalAmount { get { return "BMISC_10107"; } }

    public override string InvalidBilledMemberMigrationStatusForRejectedInvoice { get { return "BMISC_10108"; } }

    public override string InvalidTotalAmountInClearanceCurrency { get { return "BMISC_10109"; } }

    public override string InvalidServiceDateRange { get { return "BMISC_10110"; } }

    public override string InvalidLineTotal { get { return "BMISC_10111"; } }

    public override string InvalidRejectionOnValidationFailureStatus { get { return "BMISC_10112"; } }

    public override string InvalidRejectionStage { get { return "BMISC_10113"; } }

    public override string InvalidClearanceCurrency { get { return "BMISC_10114"; } }

    public const string EnterEmailIds = "BMISC_10117";

    public override string InvalidSettlementMonthPeriod { get { return "BMISC_10115"; } }

    public override string InvalidInvoiceDate { get { return "BMISC_10116"; } }

    /// <summary>
    /// Used when invoice has been already rejected.
    /// </summary>
    public override string InvoiceAlreadyRejected { get { return "BMISC_10118"; } }

    public const string InvalidCorrespondenceSubject =  "BMISC_10119"; 

    public const string InvalidAmountToBeSettled = "BMISC_10121"; 

    public const string InvalidCorrespondenceNumber  = "BMISC_10120"; 

    public const string ExpiredCorrespondence =  "BMISC_10122";

    public override string InvalidAmountToBeBilled { get { return "BMISC_10123"; } }

    public override string InvalidStartDate { get { return "BMISC_10125"; } }

    public override string InvalidChargeAmount { get { return "BMISC_10126"; } }

    //Amount to be settled should not be blank

    public override string InvalidCurrency { get { return "BMISC_10124"; } }

    public override string InvalidTaxSubType { get { return "BMISC_10127"; } }

    public const string InvalidAuthorityToBill  = "BMISC_10128"; 

    public override string InvalidAddOnChargeAmount { get { return "BMISC_10129"; } }

    public override string MandatoryRecGroupsMissing { get { return "BMISC_10130"; } }

    public override string InvalidParentChildRelation { get { return "BMISC_10131"; } }

    /// <summary>
    /// LineItem Number should be > 0
    /// </summary>
    public override string InvalidLineItemNumber { get { return "BMISC_10132"; } }

    /// <summary>
    /// PO LineItem Number >0
    /// </summary>
    public override string InvalidPOLineItemNumber { get { return "BMISC_10133"; } }

    /// <summary>
    /// Charge code is invalid for the defined Charge Category
    /// </summary>
    public const string InvalidChargeCode  = "BMISC_10134";

    /// <summary>
    /// Charge code is invalid for the defined Charge Category
    /// </summary>
    public override string ChargeCodeIsNotPresentInDatabase { get { return "BMISC_10243"; } }

    /// <summary>
    /// Total Net Amount should be equal to ChargeAmount+TotalTaxAmount+TotalVATAmount+TotalAddOnChargeAmount
    /// </summary>
    public override string InvalidTotalNetAmount { get { return "BMISC_10135"; } }

    /// <summary>
    /// Original LineItem Number should be > 0
    /// </summary>
    public override string InvalidOriginalLineItemNumber { get { return "BMISC_10146"; } }

    /// <summary>
    /// TaxAmount Should be equal to the TaxPercent into TaxableAmount if TaxPercent is provided
    /// </summary>
    public override string InvalidTaxAmount { get { return "BMISC_10137"; } }

    /// <summary>
    /// Validation will fail if PaymentDiscountPercent > 100
    /// </summary>
    public override string InvalidPaymentDiscountPercent { get { return "BMISC_10153"; } }

    /// <summary>
    /// Validation will fail if PaymentDiscountDueDays > 366
    /// </summary>
    public override string InvalidPaymentDiscountDueDays { get { return "BMISC_10154"; } }

    /// <summary>
    /// Validation will fail if PaymentNetDueDays > 366
    /// </summary>
    public override string InvalidPaymentNetDueDays { get { return "BMISC_10155"; } }

    /// <summary>
    /// Validation will fail if all filed from Rejection details are not provided.
    /// </summary>
    public override string InvalidRejectionDetails { get { return "BMISC_10156"; } }

    /// <summary>
    /// Validation will fail if ChargeCategory is not provided.
    /// </summary>
    public override string InvalidChargeCategory { get { return "BMISC_10157"; } }

    /// <summary>
    /// Validation will fail if DigitalSignitureFlag is not provided.
    /// </summary>
    public override string InvalidDigitalSignatureFlag { get { return "BMISC_10158"; } }

    /// <summary>
    /// Validation will fail if TotalLineItemAmount is not valid.
    /// </summary>
    public override string InvalidTotalLineItemAmount { get { return "BMISC_10159"; } }

    /// <summary>
    /// Used when contact type name entered while creating new contact type already exists in database
    /// </summary>
    public override string InvalidMemberContactType { get { return "BMISC_10160"; } }

    public const string FailedToSendMail = "BMISC_10151";

    /// <summary>
    /// Used when PaymentDetailsSection is invalid for settlement method other than B.
    /// </summary>
    public override string InvalidPaymentDetailsSection { get { return "BMISC_10161"; } }

    /// <summary>
    /// Used when Line Item start date is not given and line item detail start date is given.
    /// </summary>
    public override string InvalidLineItemDetailStartDate { get { return "BMISC_10152"; } }

    /// <summary>
    /// Used when IataAirPrtCityLocationCode is invalid for.
    /// </summary>
    public override string InvalidIataAirPrtCityLocationCode { get { return "BMISC_10162"; } }

    /// <summary>
    /// Used when OtherOrganizationType is invalid when account details are available.
    /// </summary>
    public override string InvalidOtherOrganizationAccountDetails { get { return "BMISC_10163"; } }

    /// <summary>
    /// Used when Uom Code is Invalid.
    /// </summary>  
    public override string InvalidUomCode { get { return "BMISC_10164"; } }

    /// <summary>
    /// Used when Line Item Numbers are not in order/Linear.
    /// </summary>  
    public override string InvalidLineItemNumberOrder { get { return "BMISC_10165"; } }

    /// <summary>
    /// Used when Line Item Detail Numbers are duplicate.
    /// </summary>  
    public override string DuplicateLineItemNumber { get { return "BMISC_10166"; } }

    /// <summary>
    ///  Used when Line Item Detail Numbers are not in order/Linear.
    /// </summary>  
    public override string InvalidLineItemDetailNumberOrder { get { return "BMISC_10167"; } }

    /// <summary>
    /// Used when Line Item Detail Numbers are duplicate.
    /// </summary>  
    public override string DuplicateLineItemDetailNumber { get { return "BMISC_10168"; } }

    /// <summary>
    /// Used to validate location defined at LineItem level.
    /// </summary>
    public override string InvalidLineItemLocationCode { get { return "BMISC_10169"; } }

    /// <summary>
    /// Should allow entry in LineItem field only if a PO Number has been populated in the invoice header.
    /// </summary>
    public override string InvalidLineItemPOLineItemNumber { get { return "BMISC_10170"; } }

    /// <summary>
    /// This should show only when there is a Charge Code Type associated with the Charge Code
    /// </summary>
    public override string InvalidChargeCodeTypeForChargeCode { get { return "BMISC_10171"; } }

    /// <summary>
    /// Invalid charge code type validation
    /// </summary>
    public override string InvalidChargeCodeType { get { return "BMISC_10172"; } }

    /// <summary>
    /// validation :- This should be displayed only for a Rejection Invoice
    /// </summary>
    public override string InvalidLineItemOriginalLineItemNumber { get { return "BMISC_10173"; } }

    /// <summary>
    /// validation :- This will be shown only if there are no line item details required for the Charge Category Charge Code. 
    /// </summary>
    public override string InvalidMinimumQuantityFlag { get { return "BMISC_10174"; } }

    /// <summary>
    /// Used for LineItem and LineItem details quantity calculations
    /// </summary>
    public override string InvalidLineItemQuantity { get { return "BMISC_10175"; } }

    /// <summary>
    ///  Used for LineItem and LineItem details Unit Price calculations
    /// </summary>
    public override string InvalidLineItemUnitPrice { get { return "BMISC_10176"; } }

    /// <summary>
    ///  Used for LineItem and LineItem details Total calculations
    /// </summary>
    public override string InvalidLineItemTotal { get { return "BMISC_10177"; } }

    /// <summary>
    ///validation :- should be same for LineItem and LineItemDetails
    /// </summary>
    public override string InvalidScalingFactor { get { return "BMISC_10178"; } }

    /// <summary>
    /// should be same for LineItem and LineItemDetails
    /// </summary>
    public override string InvalidLineItemUomCode { get { return "BMISC_10179"; } }

    /// <summary>
    ///validation :- UomCode should be 'EA'
    /// </summary>
    public override string InvalidLineItemConstUomCode { get { return "BMISC_10180"; } }

    /// <summary>
    ///validation :- should be 1
    /// </summary>
    public override string InvalidConstScalingFactor { get { return "BMISC_10181"; } }

    /// <summary>
    /// Used when -
    /// Line Item Detail has date range, user can not remove start date from line item.
    /// </summary>
    public override string InvalidStartDateForLineItemDetailWithStartDate { get { return "BMISC_10182"; } }

    /// <summary>
    /// If user removes start date from line item detail while lineItem details exists for it, then allow if all line item detail having same End Date.
    /// </summary>
    public override string InvalidStartDateForLineItemDetailEndDateNotSame { get { return "BMISC_10183"; } }

    /// <summary>
    /// Rejection invoice number cannot be correspondence.
    /// </summary>
    public override string InvalidRejectionInvoiceNumberIfCorrospondence { get { return "BMISC_10184"; } }

    /// <summary>
    /// Invoice already created for Correspondence Reference Number.
    /// </summary>
    public override string DuplicateInvoiceForCorrespondenceRefNo { get { return "BMISC_10185"; } }

    /// <summary>
    /// For Credit note invoice the value of the fields ‘Rejection Flag’ and ‘Correspondence Flag’ should not be ‘Y’ 
    /// </summary>
    public override string InvalidInvoiceType { get { return "BMISC_10186"; } }

    /// <summary>
    /// For Credit Note	Rejection and Correspondence Section will not be shown.
    /// </summary>
    public override string InvalidRejectionCorrespondenceNodes { get { return "BMISC_10187"; } }



    public override string InvalidTotalAmountOutsideLimit { get { return "BMISC_10189"; } }

    /// <summary>
    /// If Correspondence amount settled is invalid
    /// </summary>
    public override string InvalidCorrspondenceAmountSettled { get { return "BMISC_10190"; } }

    public override string InvalidMemberLocationInformation { get { return "BMISC_10191"; } }

    /// <summary>
    /// Used when the invoice of type correspondence is already created.
    /// </summary>
    public override string InvoiceAlreadyCreatedForCorrespondence { get { return "BMISC_10192"; } }
    
    /// <summary>
    /// Used when the invoice doest not exist mentioned under correspondence section.
    /// </summary>
    public override string InvoiceNotExistForCorrespondence { get { return "BMISC_10193"; } }

    /// <summary>
    /// Used when  Specified Correspondence is in open state but authority to bill is not set.
    /// </summary>
    public override string InvalidCorrespondenceStatusAuthorityToBill { get { return "BMISC_10194"; } }
    
    /// <summary>
    /// Used when  Invalid Billed or Billing Member for Specified Correspondence Reference Number.
    /// </summary>
    public override string InvalidCorrespondenceFromTo { get { return "BMISC_10195"; } }

    /// <summary>
    /// Used when Correspondence is not Expired.
    /// </summary>
    public override string CorrespondenceNotExpired { get { return "BMISC_10196"; } }

    /// <summary>
    /// Used when mandatory field is missing.
    /// </summary>
    public override string MandatoryFieldAttributeMissing { get { return "BMISC_10197"; } }

    /// <summary>
    /// Used if correspondence invoice is being rejected.
    /// </summary>
    public override string CorrespondenceInvoiceCannotBeRejected { get { return "BMISC_10198"; } }

    /// <summary>
    /// Used if exchange rate is zero.
    /// </summary>
    public override string InvalidExchangeRate { get { return "BMISC_10199"; } }
   
    /// <summary>
    /// Used if exchange rate is zero.
    /// </summary>
    public override string ExchangeRateZero { get { return "BMISC_10803"; } }


    public override string InvalidCorrRefNo { get { return "BMISC_10200"; } }

    public override string CorrRefNoClosed { get { return "BMISC_10201"; } }
    
    public override string OpenInvoiceCannotBeRejected { get { return "BMISC_10202"; } }

    public override string StageTwoRejectionCannotBeRejected { get { return "BMISC_10205"; } }

    public override string InvoiceAlreadyRejectedForPayables { get { return "BMISC_10206"; } }

    //public override string InvoiceRejectedForPayablesAndStatusIsPresented { get { return "BMISC_10231"; } }

    public override string UomCodeRequired { get { return "BMISC_10207"; } }

    public override string InvalidDSFlagAsCountryNotSpecified { get { return "BMISC_10208"; } }

    public override string InvalidCharAmountForLinkedLineItemDetail { get { return "BMISC_10209"; } }

    /// <summary>
    /// IsDigitalSigniture required should be true for billing member in eBilling configuration.
    /// </summary>
    public override string InvalidDigitalSignatureRequired { get { return "BMISC_10210"; } }

    /// <summary>
    /// Used for Invalid amount considering Min - Max amount in master for the specific transaction Type.
    /// </summary>
    public override string InvalidMinMaxAmount{ get { return "BMISC_10211"; } }

    /// <summary>
    /// Used when BRD doesn't contain the invoice mentioned in rejection invoice detail section.
    /// </summary>
    public override string RejectionInvoiceNumberNotExist  { get { return "BMISC_10212"; } }

    /// <summary>
    /// Used when invalid alphabetic value.
    /// </summary>
    public override string InvalidADataValue { get { return "BMISC_10213"; } }

    /// <summary>
    /// Used when invalid Numeric value.
    /// </summary>
    public override string InvalidNDataValue { get { return "BMISC_10214"; } }

    /// <summary>
    /// Used when invalid Positive numeric value.
    /// </summary>
    public override string InvalidPnDataValue { get { return "BMISC_10215"; } }

    /// <summary>
    /// Used when invalid Date value.
    /// </summary>
    public override string InvalidDateDataValue { get { return "BMISC_10216"; } }

    /// <summary>
    /// Used when Credit Note total amount is non-negative.
    /// </summary>
    public override string InvalidCreditNoteTotalAmount { get { return "BMISC_10217"; } }

    public override string InvalidAttributeValue { get { return "BMISC_10218"; } }

    public override string InvalidLocationCodeIcao { get { return "BMISC_10219"; } }

    public override string InvalidCountryCodeIcao { get { return "BMISC_10220"; } }

    public override string InvalidLocationCodeIcaoType { get { return "BMISC_10221"; } }

    public override string InvalidCountryCodeIcaoType { get { return "BMISC_10222"; } }

    public override string InvalidRouteDateTimeType { get { return "BMISC_10223"; } }

    public override string InvalidWaypointCodeType { get { return "BMISC_10224"; } }

    public override string InvalidRouteDataType { get { return "BMISC_10225"; } }

    public override string InvalidLocationType { get { return "BMISC_10226"; } }

    public override string InvalidCountryCode { get { return "BMISC_10227"; } }

    public override string InvalidDistanceUomcode { get { return "BMISC_10228"; } }

    public override string LocationOrLocationCodeIcaoRequired { get { return "BMISC_10229"; } }
    public override string AircraftTypeCodeOrAircraftTypeCodeIcaoRequired { get { return "BMISC_10230"; } }

    public override string InvoiceTotalAmountNegative { get { return "BMISC_10232"; } }

    public override string CreditNoteRejectionMessage { get { return "BMISC_10233"; } }

    public override string InvalidStationCode { get { return "BMISC_10234"; } }

    public override string InvalidReferenceNumber { get { return "BMISC_10235"; } }

    public override string InvalidMaxTakeOffWeight { get { return "BMISC_10236"; } }

    public const string InvalidEmailIds = "BMISC_10237"; 

    public override string TimeLimitExpiryForCorrespondence { get { return "BMISC_10238"; } }

    public override string MemberIsNotMigrated { get { return "BMISC_10241"; } }
    
    /// <summary>
    /// To check whether lineItem detail is compulsory
    /// </summary>  
    public override string MandatoryLineItemDetail { get { return "BMISC_10239"; } }

    public override string LineItemDetailExpectedButNotFound { get { return "BMISC_10240"; } }

    public override string InvalidChargeCateGoryAndCodeForOnBehalfFile { get { return "BMISC_10242"; } }

    public override string InvalidAircraftTypeCode { get { return "BMISC_10244"; } }

    public override string InvalidAircraftTypeIcaoCode { get { return "BMISC_10245"; } }

    public override string InvalidAmountName { get { return "BMISC_10246"; } }
    
    public override string InvalidPartialPaymentsAttribute { get { return "BMISC_10247"; } }

    public override string InvalidAreaSizeUomcode { get { return "BMISC_10248"; } }

    public override string FieldValueRequiredForAttribute { get { return "BMISC_10249"; } }

    public override string InvalidBillingBurrencyOfRejectedInvoice { get { return "BMISC_10250"; } }

    public override string InvalidBillingBurrencyOfCorrInvoice { get { return "BMISC_10251"; } }

    public override string ExchangeRateIsMadatoryForAmountInClearanceCurrency { get { return "BMISC_10252"; } }

    public override string InvoiceIsNotRejectedInvoice { get { return "BMISC_10253"; } }

    public override string ExchangeRateCannotBeZero { get { return "BMISC_10254"; } }

    public override string InvalidSuspendedAirline { get { return "BMISC_10255"; } }

    public override string DuplicateFileName { get { return "BMISC_10256"; } }

    public override string VoidPeriodValidationMsg { get { return "BMISC_10257"; } }

    public override string InvoiceLateSubmitted { get { return "BMISC_10258"; } }

    public override string InvalidBillingPeriod { get { return "BMISC_10259"; } }

    public override string InvalidBillingIsMembershipStatus { get { return "BMISC_10260"; } }

    public override string InvalidBilledIsMembershipStatus { get { return "BMISC_10261"; } }

    public override string TransactionLineItemNotAvailable { get { return "BMISC_10262"; } }

    public override string SameBillingAndBilledMember { get { return "BMISC_10263"; } }

    public override string InvalidDigitalSignatureValue { get { return "BMISC_10264"; } }

    public override string InvalidBillingMember { get { return "BMISC_10265"; } }

    public override string InvalidBilledMember { get { return "BMISC_10266"; } }

    public override string InvalidSettlementMethod { get { return "BMISC_10267"; } }

    public override string InvalidBillingCurrency { get { return "BMISC_10268"; } }

    public override string InvalidBillingMemberStatus { get { return "BMISC_10269"; } }

    public override string InvalidBilledMemberStatus { get { return "BMISC_10270"; } }

    public override string InvalidBillingFromMember { get { return "BMISC_10271"; } }

    public override string InvoiceValidForLateSubmission { get { return "BMISC_10272"; } }

    public override string DuplicateCorrspondenceNumber { get { return "BMISC_10274"; } }

    public override string ErrorCorrespondenceAlreadyCreated { get { return "BMISC_10275"; } }

    public override string ListingCurrencyMustHaveValue { get { return "BMISC_10277"; } }

    public override string BillingCurrencyMustHaveValue { get { return "BMISC_10278"; } }

    public override string RequiredTotalAmountInBillingCurrency { get { return "BMISC_10279"; } }
    /// <summary>
    /// LineItemDetail Number should be > 0
    /// </summary>
    public const string InvalidLineItemDetailNumber = "BMISC_10287";

    public const string InvalidAttachmentIndicatorOriginalValue  = "BMISC_10280"; 

    // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
    /// <summary>
    /// Used to give error when invlid Mishandling Type Value occures.
    /// </summary>
    public const string InvalidMishandlingTypeValue = "BMISC_10290";

    /// <summary>
    /// Used to give error when invlid Product ID occures.
    /// </summary>
    public const string InvalidProductId = "BMISC_10286";
    // CMP # 533: RAM A13 New Validations and New Charge Code [End]

    //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    /// <summary>
    /// Can not reply/save/send/readytosumbit to misc correspondence. 
    /// </summary>
    public const string CanNotReplyToMiscCorrespondence = "BMISC_10762";

    /// <summary>
    /// Invoice misc code
    /// </summary>
    //CMP #636: Standard Update Mobilization
    public const string InvalidMiscCode = "BMISC_10794";

    /// <summary>
    /// Validation error code: Scaling factor can not be '0'.
    /// </summary>
    // SCP220346: Inward Billing-XML file mandatory field.
    public override string InvalidScalingFactorValue {
      get {
      return "BMISC_10302";
      }
    }

    /// <summary>
    /// Invoice misc code
    /// </summary>
    //CMP #636: Standard Update Mobilization
    public const string InvalidMealTypeName = "BMISC_10795";

    //CMP #636: Standard Update Mobilization
    public const string MissingChargeCodeTypeOfLineItem = "BMISC_10796";

    //CMP #636: Standard Update Mobilization
    public const string InvalidChargeCodeTypeOfLineItem = "BMISC_10797";

    //CMP #636: Standard Update Mobilization
    public const string ChargeCodeTypeNotApplicableOfLineItem = "BMISC_10798";

    //CMP#502 : Rejection Reason for MISC Invoices
    public const string RejReasonCodeProvidedNonRejInvoice = "BMISC_10808";

    //CMP#502 : Rejection Reason for MISC Invoices
    public const string RejReasonCodeNotProvidedRejInvoice = "BMISC_10801";

    //CMP#502 : Rejection Reason for MISC Invoices
    public const string InvalidRejReasonCodeProvided = "BMISC_10802";

    //CMP#502 : Rejection Reason for MISC Invoices
    public const string InvalidRejReasonCodeProvidedIsWeb = "BMISC_10804";

    //CMP #648: Clearance Information in MISC Invoice PDFs
    public const string ExchangeRateCannotBeDefined = "BMISC_21804";
    //CMP #648: Clearance Information in MISC Invoice PDFs
    public const string ExchangeRateShouldBeEqualTo1 = "BMISC_21805";
    //CMP #648: Clearance Information in MISC Invoice PDFs
    public const string InvalidExchangeRateSupplied = "BMISC_21806";

    //CMP #648: Clearance Information in MISC Invoice PDFs
    public const string TotalAmtExRateSuppliedButNotCCurrency = "BMISC_21807";

    //CMP #648: Clearance Information in MISC Invoice PDFs
    public const string TotalAmtSuppliedButNotExRateAndCCurrency = "BMISC_21808";

    /// <summary>
    /// SCP#417067: Validations for Notes and Legal text
    /// </summary>
    public const string MaxCharLimitExceedsForNoteDescription = "BMISC_21810";


    //CMP #648: Clearance Information in MISC Invoice PDFs
    public const string NoLongerAssociatedWithLocation = "BMISC_21809";


    #region CMP:692 2.12File based Payment Status Updates by the Members
    public const string L1Dot1 = "BMISC_21825";
    public const string L1Dot2 = "BMISC_21826";
    public const string L1Dot3 = "BMISC_21827";
    public const string L1Dot4 = "BMISC_21828";
    public const string L1Dot5 = "BMISC_21829";
    public const string L1Dot6 = "BMISC_21830";
    public const string L1Dot7 = "BMISC_21831";
    public const string L1Dot8 = "BMISC_21832";
    public const string L1Dot9 = "BMISC_21833";

    public const string L2Dot1 = "BMISC_21834";
    public const string L2Dot2 = "BMISC_21835";
    public const string L2Dot3 = "BMISC_21836";
    public const string L2Dot4 = "BMISC_21837";

    public const string RecL2Dot1Dot1 = "BMISC_21838";
    public const string RecL2Dot2Dot1 = "BMISC_21839";
    public const string RecL2Dot3Dot1 = "BMISC_21840";
    public const string RecL2Dot4Dot1 = "BMISC_21841";
    public const string RecL2Dot4Dot2 = "BMISC_21842";
    public const string RecL2Dot5Dot1 = "BMISC_21843";
    public const string RecL2Dot6Dot1 = "BMISC_21845";
    public const string RecL2Dot6Dot2 = "BMISC_21846";
    public const string RecL2Dot7Dot1 = "BMISC_21847";
    public const string RecL2Dot7Dot2 = "BMISC_21848";
    public const string RecL2Dot7Dot3 = "BMISC_21849";
    public const string RecL2Dot8Dot1 = "BMISC_21850";
    public const string RecL2Dot10Dot1 = "BMISC_21851";
    public const string RecL2Dot10Dot2 = "BMISC_21852";
    public const string RecL2Dot10Dot3 = "BMISC_21853";
    public const string RecL2Dot11Dot1 = "BMISC_21854";
    public const string RecL2Dot11Dot2 = "BMISC_21855";
    public const string RecL2Dot12Dot1 = "BMISC_21856";
    public const string RecL2Dot12Dot2 = "BMISC_21857";
    public const string RecL2Dot12Dot3 = "BMISC_21858";
    public const string RecL2Dot13Dot1 = "BMISC_21859";
    public const string RecL2Dot13Dot2 = "BMISC_21860";
    public const string RecL2Dot14Dot1 = "BMISC_21861";

    public const string PayL2Dot1Dot1 = "BMISC_21862";
    public const string PayL2Dot2Dot1 = "BMISC_21863";
    public const string PayL2Dot3Dot1 = "BMISC_21864";
    public const string PayL2Dot4Dot1 = "BMISC_21865";
    public const string PayL2Dot4Dot2 = "BMISC_21866";
    public const string PayL2Dot5Dot1 = "BMISC_21867";
    public const string PayL2Dot6Dot1 = "BMISC_21868";
    public const string PayL2Dot6Dot2 = "BMISC_21869";
    public const string PayL2Dot7Dot1 = "BMISC_21870";
    public const string PayL2Dot7Dot2 = "BMISC_21871";
    public const string PayL2Dot7Dot3 = "BMISC_21872";
    public const string PayL2Dot8Dot1 = "BMISC_21873";
    public const string PayL2Dot10Dot1 = "BMISC_21874";
    public const string PayL2Dot10Dot2 = "BMISC_21875";
    public const string PayL2Dot10Dot3 = "BMISC_21876";
    public const string PayL2Dot11Dot1 = "BMISC_21877";
    public const string PayL2Dot11Dot2 = "BMISC_21878";
    public const string PayL2Dot12Dot1 = "BMISC_21879";
    public const string PayL2Dot12Dot2 = "BMISC_21880";
    public const string PayL2Dot12Dot3 = "BMISC_21881";
    public const string PayL2Dot13Dot1 = "BMISC_21882";
    public const string PayL2Dot13Dot2 = "BMISC_21883";
    public const string PayL2Dot14Dot1 = "BMISC_21884";
   
    #endregion
  }
}

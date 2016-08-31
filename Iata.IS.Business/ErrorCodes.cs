namespace Iata.IS.Business
{
  public struct ErrorCodes
  {
    /// <summary>
    /// Used when the billing member and billed members are the same in an invoice.
    /// </summary>
    public const string SameBillingAndBilledMember = "BPAXNS_10101";

    /// <summary>
    /// Used when an unexpected invoice type is used. E.g. Invoice instead of a CreditNote.
    /// </summary>
    public const string UnexpectedInvoiceType = "BPAXNS_10102";

    /// <summary>
    /// Used when the billing member is invalid.
    /// </summary>
    public const string InvalidBillingMember = "BPAXNS_10103";

    /// <summary>
    /// Used when the billed member is invalid.
    /// </summary>
    public const string InvalidBilledMember = "BPAXNS_10104";

    /// <summary>
    /// Used when the combination of settlement method for an invoice, the ICH zones of the billed + billing members and the billing currency is invalid as per the business rules.
    /// </summary>
    public const string InvalidBillingCurrency = "BPAXNS_10105";

    /// <summary>
    /// Used when the invoice billing period is invalid.
    /// </summary>
    public const string InvalidBillingPeriod = "BPAXNS_10106";

    /// <summary>
    /// Used when there is mismatch based on the settlement method of the invoice and the member status (ICH/ACH).
    /// </summary>
    public const string InvalidSettlementMethod = "BPAXNS_10107";

    /// <summary>
    /// Used when there is Tax and Tax Breakdown Total Amount mismatch.
    /// </summary>
    public const string TaxTotalAmountMismatch = "BPAXNS_10108";

    /// <summary>
    /// Used when there is VAT and VAT Breakdown Total Amount mismatch.
    /// </summary>
    public const string VatTotalAmountMismatch = "BPAXNS_10109";

    /// <summary>
    /// Used when 'From airport code' passed in coupon record is invalid
    /// </summary>
    public const string InvalidFromAirportCode = "BPAXNS_10110";

    /// <summary>
    /// Used when 'To airport code' passed in coupon record is invalid
    /// </summary>
    public const string InvalidToAirportCode = "BPAXNS_10111";

    /// <summary>
    /// Used when listing currency mismatch for billing code 3/5/6/7 
    /// </summary>
    public const string InvalidListingCurrency = "BPAXNS_10112";

    /// <summary>
    /// Used when no billing member found for the invoice
    /// </summary>
    public const string NoBillingMemberAvailable = "BPAXNS_10113";

    /// <summary>
    /// Used when single transaction line record not available for Prime Billing, Rejection Memo and Billing Memo
    /// </summary>
    public const string TransactionLineItemNotAvailable = "BPAXNS_10114";

    /// <summary>
    /// Used when invoice total's net billing amount is 0.
    /// Net Billing Amount should be equal to Net Total / Listing to Billing Rate rounded to two decimal places
    /// </summary>
    public const string InvalidNetBillingAmount = "BPAXNS_10115";

    /// <summary>
    /// Used when same invoice number found invoice repository for given calendar year and billing member.
    /// </summary>
    public const string DuplicateInvoiceFound = "BGEN_00002";

    /// <summary>
    /// Used when issuing airline member code/member type is invalid.
    /// Edited the message as per CMP #596: Length of Member Accounting Code to be Increased to 12, FRS Section 3.4 Point 24.
    /// </summary>
    public const string InvalidIssuingAirline = "BGEN_10832";

    /// <summary>
    /// Difference does not meet the expected calculation
    /// </summary>
    public const string InvalidCalculation = "BGEN_10831";

    /// <summary>
    /// Used when Allowed ISC amount is not calculated as Allowed ISC Percent of Gross Amount Billed
    /// </summary>
    public const string InvalidAllowedIscAmount = "BPAXNS_10119";

    /// <summary>
    /// Used when Accepted ISC amount is not calculated as Accepted ISC Percent of Gross Amount Accepted
    /// </summary>
    public const string InvalidAcceptedIscAmount = "BPAXNS_10120";

    /// <summary>
    /// Used when Allowed Uatp amount is not calculated as Allowed Uatp Percent of Gross Amount Billed
    /// </summary>
    public const string InvalidAllowedUatpAmount = "BPAXNS_10121";

    /// <summary>
    /// Used when Accepted Uatp amount is not calculated as Accepted Uatp Percent of Gross Amount Accepted
    /// </summary>
    public const string InvalidAcceptedUatpAmount = "BPAXNS_10122";

    /// <summary>
    /// Used when combination of ticket number, coupon number and issuing airlines find in same invoice for that coupon. 
    /// </summary>
    public const string DuplicateCouponRecordFound = "BPAXNS_10123";

    /// <summary>
    /// Used when same rejection memo found in Rejection Memo table for "Your Rejection Memo Number" for mentioned period has same rejection stage.
    /// </summary>
    public const string InvalidRejectionMemo = "BPAXNS_10125";

    /// <summary>
    /// Used when amounts for any  PM/RM/BM/CM is not positive.
    /// </summary>
    public const string InvalidAmount = "BPAXNS_10126";

    /// <summary>
    /// Used when tax breakdown record not found in coupon.
    /// </summary>
    public const string InvalidTaxAmount = "BPAXNS_10127";

    /// <summary>
    /// Used when coupon record has invalid flight date.
    /// </summary>
    public const string InvalidFlightDate = "BPAXNS_10128";

    /// <summary>
    /// Used when  E-ticket Indicator field is "E", but Settlement Authorization code is empty
    /// </summary>
    public const string InvalidSettlementAuthorizationCode = "BPAXNS_10129";

    /// <summary>
    /// Used when Your Rejection Number is empty for rejection stage 2 or 3 and billing code is 0, or Billing code is 3
    /// </summary>
    public const string InvalidYourRejectionNumber = "BPAXNS_10130";

    /// <summary>
    /// Used when billing memo vat breakdown record not found, if coupon record exist and total billed vat amount in non-zero.
    /// </summary>
    public const string BillingMemoVatBreakdownRecordNotFound = "BPAXNS_10131";

    /// <summary>
    /// Used when credit memo vat breakdown record not found, if coupon record exist and total credited vat amount in non-zero.
    /// </summary>
    public const string CreditMemoVatBreakdownRecordNotFound = "BPAXNS_10132";

    /// <summary>
    /// Used when Tax Amount Billed is non-zero and Tax Breakdown record is not available.
    /// </summary>
    public const string BillingMemoCouponTaxBreakdownNotFound = "BPAXNS_10133";

    /// <summary>
    /// Used when Vat Amount Billed is non-zero and Vat Breakdown record is not available.
    /// </summary>
    public const string BillingMemoCouponVatBreakdownNotFound = "BPAXNS_10134";

    /// <summary>
    /// Used when invoice status is not valid for updating invoice
    /// </summary>
    public const string InvalidInvoiceStatusForUpdate = "BPAXNS_10135";

    /// <summary>
    /// Used when Tax Amount Credited is non-zero and Tax Breakdown record is not available.
    /// </summary>
    public const string CreditMemoCouponTaxBreakdownNotFound = "BPAXNS_10136";

    /// <summary>
    /// Used when Vat Amount Credited is non-zero and Vat Breakdown record is not available.
    /// </summary>
    public const string CreditMemoCouponVatBreakdownNotFound = "BPAXNS_10137";

    /// <summary>
    /// Used when Member reference data missing for billing and billed member.
    /// </summary>
    public const string InvalidMemberLocationInformation = "BPAXNS_10138";

    /// <summary>
    /// Used when, duplicate rejection Memo Coupon breakdown record, found in rejection memo record
    /// </summary>
    public const string DuplicateRejectionMemoCouponBreakdownRecordFound = "BPAXNS_10139";

    /// <summary>
    /// Used when, Combination of fields Your Invoice Number’, ‘Your Billing Year’, 
    /// ‘Your Billing Month’ and ‘Your Billing Period’ passed in billing memo/credit memo object does not match with any other invoice in invoice table
    /// </summary>
    public const string LinkedInvoiceNotFound = "BPAXNS_10140";

    /// <summary>
    /// Used when, Correspondence Ref. No. mentioned does not exist in the correspondence table as a correspondence between the 2 members between which the BM is being created.
    /// </summary>
    public const string BillingMemoReferenceCorrespondenceDoesNotExist = "BPAXNS_10141";

    /// <summary>
    /// Used when, Correspondence Ref. No. mentioned does not exist in the correspondence table as a correspondence between the 2 members between which the BM is being created.
    /// </summary>
    public const string CreditMemoReferenceCorrespondenceDoesNotExist = "BPAXNS_10143";

    /// <summary>
    /// Used when Billing code in not 0 for non-sampling invoice
    /// </summary>
    public const string InvalidBillingCode = "BPAXNS_10144";

    /// <summary>
    /// Used when invalid source code entered from UI
    /// </summary>
    public const string InvalidSourceCode = "BPAXNS_10146";

    /// <summary>
    /// Used when invalid reason code entered from UI
    /// </summary>
    public const string InvalidReasonCode = "BPAXNS_10147";

    /// <summary>
    /// Used when duplicate billing memo found
    /// </summary>
    public const string DuplicateBillingMemoFound = "BPAXNS_10148";

    /// <summary>
    /// Used when duplicate credit memo found
    /// </summary>
    public const string DuplicateCreditMemoFound = "BPAXNS_10149";

    /// <summary>
    /// Used when invalid reason code entered from UI
    /// </summary>
    public const string InvalidAirportCombination = "BPAXNS_10150";

    /// <summary>
    /// Used when tax breakdown code record is invalid in coupon breakdown.
    /// </summary>
    public const string InvalidTaxCode = "BPAXNS_10151";

    /// <summary>
    /// Used when tax breakdown code record is invalid in coupon breakdown.
    /// </summary>
    public const string InvalidYourInvoiceNumber = "BPAXNS_10152";

    /// <summary>
    /// Used when tax breakdown code record is invalid in coupon breakdown.
    /// </summary>
    public const string BlankYourInvoiceNumber = "BPAXS_10280";

    /// <summary>
    /// Used when tax code is  missing in RM coupon breakdown.
    /// </summary>
    public const string MissingTaxCode = "BPAXNS_10153";

    /// <summary>
    /// Used when Billing member location is invalid.
    /// </summary>
    //public const string InvalidBillingMemberLocation = "BPAXNS_10153";

    public const string InvalidBillingMemberLocation = "BGEN_00011";

    /// <summary>
    /// Used when Billed member location is invalid.
    /// </summary>
    //public const string InvalidBilledMemberLocation = "BPAXNS_10154";

    public const string InvalidBilledMemberLocation = "BGEN_00012";

    /// <summary>
    /// Used when vat identifier is invalid.
    /// </summary>
    public const string InvalidVatIdentifier = "BPAXNS_10155";

    /// <summary>
    /// Used when vat Tic Or FIM Issuing Airline is invalid.
    /// </summary>
    public const string InvalidTicOrFimIssuingAirline = "BPAXNS_10156";

    /// <summary>
    ///  Used when there is linking error for the provisional/your invoice field.
    /// </summary>
    public const string InvoiceLinkingError = "BPAXNS_10157";

    /// <summary>
    /// Used when NFP reason code is invalid.
    /// </summary>
    public const string InvoiceNfpReasonCode = "BPAXNS_10158";

    /// <summary>
    /// Used when Agreement Indicator Supplied is invalid.
    /// </summary>
    public const string InvalidAgreementIndicatorSupplied = "BPAXNS_10159";

    /// <summary>
    /// Used when Agreement Indicator Validated is invalid.
    /// </summary>
    public const string InvalidAgreementIndicatorValidated = "BPAXNS_10160";

    /// <summary>
    ///  Used when Original PMI is invalid.
    /// </summary>
    public const string InvalidOriginalPMI = "BPAXNS_10161";

    /// <summary>
    /// Used when Validated PMI is invalid.
    /// </summary>
    public const string InvalidValidatedPMI = "BPAXNS_10162";

    /// <summary>
    /// Used when FlightNumber is invalid.
    /// </summary>
    public const string InvalidFlightNumber = "BPAXNS_10163";

    /// <summary>
    /// Used when Airline Code is invalid.
    /// </summary>
    public const string InvalidAirlineCode = "BPAXNS_10164";

    /// <summary>
    /// Used when Ticket Issuing Airline Code is invalid.
    /// </summary>
    public const string InvalidTicketIssuingAirline = "BPAXNS_10332";

    /// <summary>
    /// Used when Currency Adjustment Indicator is invalid.
    /// </summary>
    public const string InvalidCurrencyAdjustmentInd = "BPAXNS_10165";

    /// <summary>
    /// Used when Rejection Stage is invalid.
    /// </summary>
    public const string InvalidRejectionStage = "BPAXNS_10166";

    /// <summary>
    /// Used when Coupon Number is invalid.
    /// </summary>
    public const string InvalidCouponNumber = "BPAXNS_10167";

    /// <summary>
    /// Used when Total Gross Value is invalid.
    /// </summary>
    public const string InvalidTotalGrossValue = "BPAXNS_10168";

    /// <summary>
    /// Used when Total Tax Amount is invalid.
    /// </summary>
    public const string InvalidTotalTaxAmount = "BPAXNS_10169";

    /// <summary>
    /// Used when Total Vat Amount is invalid.
    /// </summary>
    public const string InvalidTotalVatAmount = "BPAXNS_10170";

    /// <summary>
    ///  Used when Total Uatp Amount is invalid.
    /// </summary>
    public const string InvalidTotalUatpAmount = "BPAXNS_10171";

    /// <summary>
    /// Used when Total ISC Amount is invalid.
    /// </summary>
    public const string InvalidTotalIscAmount = "BPAXNS_10172";

    /// <summary>
    /// Used when Total Other Commission Amount is invalid.
    /// </summary>
    public const string InvalidTotalOtherCommissionAmount = "BPAXNS_10173";

    /// <summary>
    /// Used when Total Handling Fee Amount is invalid.
    /// </summary>
    public const string InvalidTotalHandlingFeeAmount = "BPAXNS_10174";

    /// <summary>
    /// Used when Total Number Of Records is invalid.
    /// </summary>
    public const string InvalidTotalNumberOfBillingRecords = "BPAXNS_10175";

    /// <summary>
    /// Used when NetTotal is invalid.
    /// </summary>
    public const string InvalidNetTotal = "BPAXNS_10176";

    /// <summary>
    /// Used when  Total Amount After Sampling Constant  is invalid.
    /// </summary>
    public const string InvalidTotalAmountAfterSamplingConstant = "BPAXNS_10177";

    /// <summary>
    /// Used when Your Invoice Billing Data is invalid.
    /// </summary>
    public const string YourInvoiceBillingDateIsInvalid = "BPAXNS_10179";

    /// <summary>
    /// Used when Your Invoice Billing Data is invalid.
    /// </summary>
    public const string InvoiceBillingDateIsInvalid = "BPAXNS_10184";

    /// <summary>
    /// Used when Your Invoice Billing Data is invalid.
    /// </summary>
    public const string InvoiceProvisionalBillingMonthInvalid = "BPAXNS_10193";

    /// <summary>
    /// Used when contact type name entered while creating new contact type already exists in database
    /// </summary>
    public const string InvalidContactTypeName = "BMEM_10101";

    /// <summary>
    ///
    /// </summary>
    public const string InvalidListingToBillingRate = "BGEN_00005";

    public const string ParentChildErrorInvalidBillingAirlineCode = "BPAXNS_10261";

    public const string ParentChildErrorInvalidBilledAirlineCode = "BPAXNS_10262";

    public const string ParentChildErrorInvalidBillingCode = "BPAXNS_10263";

    public const string ParentChildErrorInvalidInvoiceNumber = "BPAXNS_10264";

    public const string ParentChildErrorInvalidTicketIssuingAirline = "BPAXNS_10265";

    public const string ParentChildErrorInvalidTicketCouponNumber = "BPAXNS_10266";

    public const string ParentChildErrorInvalidTicketOrDocumentNumber = "BPAXNS_10267";

    public const string ParentChildErrorInvalidSourceCode = "BPAXNS_10617";
    //Amount difference should match with Billed - Accepted

    public const string GrossAmountDifferenceShouldMatchWithBilledMinusAccepted = "BPAXNS_10268";

    public const string TaxAmountDifferenceShouldMatchWithBilledMinusAccepted = "BPAXNS_10269";

    public const string VatAmountDifferenceShouldMatchWithBilledMinusAccepted = "BPAXNS_10270";

    public const string HandlingFeeAmountDifferenceShouldMatchWithBilledMinusAccepted = "BPAXNS_10271";

    public const string IscAmountDifferenceShouldMatchWithBilledMinusAccepted = "BPAXNS_10272";

    public const string OtherCommissionAmountDifferenceShouldMatchWithBilledMinusAccepted = "BPAXNS_10273";

    public const string UatpAmountDifferenceShouldMatchWithBilledMinusAccepted = "BPAXNS_10274";

    //Amount difference should match with Accepted - Billed

    public const string GrossAmountDifferenceShouldMatchWithAcceptedMinusBilled = "BPAXNS_10275";

    public const string TaxAmountDifferenceShouldMatchWithAcceptedMinusBilled = "BPAXNS_10276";

    public const string VatAmountDifferenceShouldMatchWithAcceptedMinusBilled = "BPAXNS_10277";

    public const string HandlingFeeAmountDifferenceShouldMatchWithAcceptedMinusBilled = "BPAXNS_10278";

    public const string IscAmountDifferenceShouldMatchWithAcceptedMinusBilled = "BPAXNS_10279";

    public const string OtherCommissionAmountDifferenceShouldMatchWithAcceptedMinusBilled = "BPAXNS_10280";

    public const string UatpAmountDifferenceShouldMatchWithAcceptedMinusBilled = "BPAXNS_10281";

    //Total amount not matching with breakdowns

    public const string GrossAmountBilledIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10282";

    public const string GrossAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10283";

    public const string GrossAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10284";

    public const string TaxAmountBilledIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10285";

    public const string TaxAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10286";

    public const string TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10287";

    public const string VatAmountBilledIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10288";

    public const string VatAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10289";

    public const string VatAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10290";

    public const string HandlingFeeAmountBilledIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10291";

    public const string HandlingFeeAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10292";

    public const string HandlingFeeAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10293";

    public const string IscAmountBilledIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10294";

    public const string IscAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10295";

    public const string IscAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10296";

    public const string OtherCommissionAmountBilledIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10297";

    public const string OtherCommissionAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10298";

    public const string OtherCommissionAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10299";

    public const string UatpAmountBilledIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10300";

    public const string UatpAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10301";

    public const string UatpAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10302";

    public const string TotalNetRejectAmountIsNotMatchingWithSumOfBreakdowns = "BPAXNS_10303";

    public const string ParentChildErrorInvalidMemoNumber = "BPAXNS_10307";

    public const string FromAirportOfCouponAndToAirportOfCouponShouldNotBeSame = "BPAXNS_10308";

    public const string TotalAmountAfterSamplingConstantForSamplingXAndXF = "BPAXNS_10310";

    public const string UatpAmountIsInvalid = "BPAXNS_10224";

    public const string OtherCommissionAmountIsInvalid = "BPAXNS_10220";

    public const string IscAmountIsInvalid = "BPAXNS_10216";

    /// <summary>
    /// VAT Breakdown is not provided when the VAT amount is not zero.
    /// </summary>
    public const string VatBreakdownRecordsRequired = "BPAXNS_10311";

    public const string InvalidCouponTotalAmount = "BPAXNS_10228";

    /// <summary>
    ///
    /// </summary>
    public const string ProvisionalAdjustmentRate = "BPAXNS_10313";

    /// <summary>
    /// 
    /// </summary>
    public const string InvalidCalculatedVatAmount = "BPAXNS_10314";

    /// <summary>
    /// Other than Sampling Form A/B, Fare Absorption Amount should be 0
    /// </summary>
    public const string FareAbsorptionAmountIsInvalid = "BPAXNS_10315";

    /// <summary>
    /// Other than Sampling Form A/B, Fare Absorption Amount should be 0
    /// </summary>
    public const string FareAbsorptionPercentIsInvalid = "BPAXNS_10316";

    /// <summary>
    /// Other than Sampling Form A/B, Handling Fee Absorption Amount should be 0
    /// </summary>
    public const string HandlingFeeAbsorptionAmountIsInvalid = "BPAXNS_10317";

    /// <summary>
    /// Other than Sampling Form A/B, Handling Fee Absorption percent should be 0
    /// </summary>
    public const string HandlingFeeAbsorptionPercentIsInvalid = "BPAXNS_10318";

    /// <summary>
    /// Other than Sampling Form A/B, Isc Absorption amount should be 0
    /// </summary>
    public const string IscAbsorptionAmountIsInvalid = "BPAXNS_10319";

    /// <summary>
    /// Other than Sampling Form A/B, Isc Absorption percent should be 0
    /// </summary>
    public const string IscAbsorptionPercentIsInvalid = "BPAXNS_10320";

    /// <summary>
    /// Other than Sampling Form A/B, Other Commission Absorption amount should be 0
    /// </summary>
    public const string OtherCommissionAbsorptionAmountIsInvalid = "BPAXNS_10321";

    /// <summary>
    /// Other than Sampling Form A/B, Other Commission Absorption percent should be 0
    /// </summary>
    public const string OtherCommissionAbsorptionPercentIsInvalid = "BPAXNS_10322";

    /// <summary>
    /// Other than Sampling Form A/B, Tax Absorption amount should be 0
    /// </summary>
    public const string TaxAbsorptionAmountIsInvalid = "BPAXNS_10323";

    /// <summary>
    /// Other than Sampling Form A/B, Tax Absorption percent should be 0
    /// </summary>
    public const string TaxAbsorptionPercentIsInvalid = "BPAXNS_10324";

    /// <summary>
    /// Other than Sampling Form A/B, UATP Absorption amount should be 0
    /// </summary>
    public const string UatpAbsorptionAmountIsInvalid = "BPAXNS_10325";

    /// <summary>
    /// Other than Sampling Form A/B, UATP Absorption percent should be 0
    /// </summary>
    public const string UatpAbsorptionPercentIsInvalid = "BPAXNS_10326";

    /// <summary>
    /// Total Net Amount without VAT should be equal to Net Total – Total Vat Amount.
    /// </summary>
    public const string TotalNetAmountWithoutVatIsInvalid = "BPAXNS_10327";

    /// <summary>
    /// Duplicate file name check for attachment file names
    /// </summary>
    public const string DuplicateFileName = "BPAXNS_10328";

    /// <summary>
    /// If Vat amount > 0 then tax breakdowns should be > 0 
    /// </summary>
    public const string ZeroVatBreakdownRecords = "BPAXNS_10329";

    /// <summary>
    /// If Tax amount > 0 then tax breakdowns should be > 0 
    /// </summary>
    public const string ZeroTaxBreakdownRecords = "BPAXNS_10330";

    /// <summary>
    /// Used when Handling fee type is invalid.
    /// </summary>
    public const string InvalidHandlingFeeType = "BPAXNS_10206";

    /// <summary>
    /// Used when Handling fee amount is invalid.
    /// </summary>
    public const string InvalidHandlingFeeAmount = "BPAXNS_10214";

    /// <summary>
    /// For non sampling Vat Amount After Sampling Constant should be 0
    /// </summary>
    public const string InvalidVatAmountAfterSamplingConstant = "BPAXNS_10334";

    /// <summary>
    /// For non sampling Net Amount After Sampling Constant should be 0
    /// </summary>
    public const string InvalidNetAmountAfterSamplingConstant = "BPAXNS_10335";

    /// <summary>
    /// Invalid your invoice billing date
    /// </summary>
    public const string InvalidYourInvoiceBillingDate = "BPAXNS_10336";

    /// <summary>
    /// Vat amount does not match with totals of vat breakdowns
    /// </summary>
    public const string InvalidTotalVatBreakdownAmounts = "BPAXNS_10337";

    /// <summary>
    /// Tax amount does not match with totals of tax breakdowns
    /// </summary>
    public const string InvalidTotalTaxBreakdownAmounts = "BPAXNS_10338";

    /// <summary>
    /// Tax amount accepted does not match with totals of tax breakdowns
    /// </summary>
    public const string InvalidTotalAcceptedTaxBreakdownAmounts = "BPAXNS_10339";

    /// <summary>
    /// Tax amount difference does not match with totals of tax breakdowns
    /// </summary>
    public const string InvalidTotalDifferenceTaxBreakdownAmounts = "BPAXNS_10340";

    /// <summary>
    /// Net billed amount is invalid
    /// </summary>
    public const string InvalidNetBilledAmount = "BPAXNS_10341";

    /// <summary>
    /// Net reject amount is invalid
    /// </summary>
    public const string InvalidNetRejectAmount = "BPAXNS_10342";

    /// <summary>
    /// Sampling constant should be blank for non sampling invoice
    /// </summary>
    public const string SamplingConstantShouldBeBlankForNSInvoice = "BPAXNS_10343";

    /// <summary>
    /// Total net reject amount after sampling constant should be blank for non sampling invoice
    /// </summary>
    public const string TotalNetRejectAmountAfterSamplingConstantShouldBeZero = "BPAXNS_10344";

    /// <summary>
    /// Sampling constant should not be blank for sampling invoice
    /// </summary>
    public const string InvalidSamplingConstant = "BPAXNS_10345";

    /// <summary>
    /// Total net reject amount after sampling constant should not be blank for sampling invoice
    /// </summary>
    public const string InvalidTotalNetRejectAmountAfterSamplingConstant = "BPAXNS_10346";

    /// <summary>
    /// Allowed ISC amount does not match with Allowed ISC percentage * Gross Amount
    /// </summary>
    public const string InvalidAllowedIscPercentage = "BPAXNS_10347";

    /// <summary>
    /// Accepted ISC amount does not match with Accepted ISC percentage * Gross Amount
    /// </summary>
    public const string InvalidAcceptedIscPercentage = "BPAXNS_10348";

    /// <summary>
    /// Allowed OtherCommission amount does not match with Allowed OtherCommission percentage * Gross Amount
    /// </summary>
    public const string InvalidAllowedOtherCommissionPercentage = "BPAXNS_10349";

    /// <summary>
    /// Accepted OtherCommission amount does not match with Accepted OtherCommission percentage * Gross Amount
    /// </summary>
    public const string InvalidAcceptedOtherCommissionPercentage = "BPAXNS_10350";

    /// <summary>
    /// Allowed UATP amount does not match with Allowed UATP percentage * Gross Amount
    /// </summary>
    public const string InvalidAllowedUatpPercentage = "BPAXNS_10351";

    /// <summary>
    /// Accepted ISC amount does not match with Accepted ISC percentage * Gross Amount
    /// </summary>
    public const string InvalidAcceptedUatpPercentage = "BPAXNS_10352";

    /// <summary>
    /// Gross total of source code total record does not match with the gross total of transaction records
    /// </summary>
    public const string InvalidGrossTotalOfSourceCodeTotalRecord = "BPAXNS_10353";

    /// <summary>
    /// Gross total of source code total record does not match with the total of evaluated gross amount of Form D records
    /// </summary>
    public const string InvalidEvaluateGrossTotalOfSourceCodeTotalRecord = "BPAXNS_10353";

    /// <summary>
    /// Tax total of source code total record does not match with the tax total of transaction records
    /// </summary>
    public const string InvalidTaxTotalOfSourceCodeTotalRecord = "BPAXNS_10354";

    /// <summary>
    /// Vat total of source code total record does not match with the vat total of transaction records
    /// </summary>
    public const string InvalidVatTotalOfSourceCodeTotalRecord = "BPAXNS_10355";

    /// <summary>
    /// UATP amount total of source code total record does not match with the UATP amount total of transaction records
    /// </summary>
    public const string InvalidUatpAmountTotalOfSourceCodeTotalRecord = "BPAXNS_10359";

    /// <summary>
    /// ISC amount total of source code total record does not match with the ISC total of transaction records
    /// </summary>
    public const string InvalidIscTotalOfSourceCodeTotalRecord = "BPAXNS_10357";

    /// <summary>
    /// Other commission amount total of source code total record does not match with the other commission amount total of transaction records
    /// </summary>
    public const string InvalidOtherCommissionTotalOfSourceCodeTotalRecord = "BPAXNS_10358";

    /// <summary>
    /// Handling fee amount total of source code total record does not match with the Handling fee total of transaction records
    /// </summary>
    public const string InvalidHandlingFeeTotalOfSourceCodeTotalRecord = "BPAXNS_10356";

    /// <summary>
    /// Total number of billing records of source code total record does not match with total of transaction records of the source code.
    /// </summary>
    public const string InvalidNumberOfBillingRecordsOfSourceCodeTotalRecord = "BPAXNS_10360";

    /// <summary>
    /// Total net reject amount after sampling constant of source code total record does not match with total of transaction records of the source code.
    /// </summary>
    public const string InvalidTotalNetRejectAmountOfSourceCodeTotalRecord = "BPAXNS_10361";

    /// <summary>
    /// ISC amount does not match with ISC percentage * Gross Amount
    /// </summary>
    public const string InvalidIscPercentage = "BPAXNS_10362";

    /// <summary>
    /// Other Commission amount does not match with Other Commission percentage * Gross Amount
    /// </summary>
    public const string InvalidOtherCommissionPercentage = "BPAXNS_10363";

    /// <summary>
    /// UATP amount does not match with UATP percentage * Gross Amount
    /// </summary>
    public const string InvalidUatpPercentage = "BPAXNS_10364";

    /// <summary>
    /// Used when there is duplicate entry for Batch Number - Sequence Number with in Batch in one invoice
    /// </summary>
    public const string DuplicateBatchNoSequenceNo = "BPAXNS_10468";

    /// <summary>
    /// Used when there is duplicate entry for Batch Number with in Batch in one invoice
    /// </summary>
    public const string DuplicateBatchNo = "BPAXNS_10535";

    
      
    /// <summary>
    /// Used when there is sequence error for the serial number for RM/BM/CM Coupon Breakdown records.
    /// </summary>
    public const string InvalidSerialNumberSequence = "BPAXNS_10536";

    /// <summary>
    /// Invalid total number of records for source code total and invoice total
    /// </summary>
    public const string InvalidTotalNumberOfRecords = "BPAXNS_10537";

    /// <summary>
    /// Invalid invoice date
    /// </summary>
    public const string InvalidInvoiceDate = "BPAXNS_10522";

    /// <summary>
    /// Invalid FimDocument number
    /// </summary>
    public const string InvalidFimDocumentNumber = "BPAXNS_10523";

    /// <summary>
    /// Invalid Provisional invoice date
    /// </summary>
    public const string InvalidProvisionalInvoiceDate = "BPAXNS_10524";

    /// <summary>
    /// If a rejection memo is raised with reason code 1G, then the “Total Tax Difference” field should have a non zero value.
    /// </summary>
    public const string InvalidTotalTaxDifferenceForReasonCode1G = "BPAXNS_10525";

    /// <summary>
    /// Net evaluated amount is invalid
    /// </summary>
    public const string InvalidEvaluatedNetAmount = "BPAXNS_10593";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForYourInvoiceNumber = "BPAXNS_10611";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Your Rejection Memo Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForYourRejectionMemoNumber = "BPAXNS_10612";

    /// <summary>
    /// Reason Code “1A” cannot be used for RMs having more than one coupon breakdown record.
    /// </summary>
    public const string InvalidRejectionMemoRecordForReasonCode1A = "BPAXNS_10613";

    /// <summary>
    /// Should be a unique number within each Billed Airline in the Billing period.
    /// </summary>
    public const string DuplicateRejectionMemoNumber = "BPAXNS_10614";

    /// <summary>
    /// From airport of coupon is invalid.
    /// </summary>
    public const string FromAirportOfCouponIsInvalid = "BPAXNS_10615";

    /// <summary>
    /// To airport of coupon is invalid.
    /// </summary>
    public const string ToAirportOfCouponIsInvalid = "BPAXNS_10616";

    /// <summary>
    /// Your invoice number doesn't exists in database.
    /// </summary>
    public const string YourInvoiceNotExists = "BPAXNS_10638";

    /// <summary>
    /// Coupon Breakdown Record is mandatory for the reason code.
    /// </summary>
    public const string MandatoryCouponBreakdownRecord = "BPAXNS_10673";

    /// <summary>
    /// FIM Number and FIM Coupon Number should be blank or both fields should be captured.
    /// </summary>
    public const string MandatoryFimNumberAndCouponNumber = "BPAXNS_10674";

    /// <summary>
    /// Your invoice number and your invoice date should be blank or both fields should be captured.
    /// </summary>
    public const string MandatoryYourInvoiceNumberAndYourBillingDate = "BPAXNS_10675";

    /// <summary>
    /// Prorate slip details should not be all blanks.
    /// </summary>
    public const string MandatoryProrateSlipDetails = "BPAXNS_10676";

    /// <summary>
    /// Used when Reference field 1 is empty.
    /// </summary>
    public const string InvalidReferenceField1 = "BPAXNS_10677";

    /// <summary>
    /// Used when Reference field 2 is empty.
    /// </summary>
    public const string InvalidReferenceField2 = "BPAXNS_10678";

    /// <summary>
    /// Reason breakdown should not be all blanks.
    /// </summary>
    public const string MandatoryReasonBreakdown = "BPAXNS_10679";

    /// <summary>
    /// Used when FIM Coupon Number is invalid.
    /// </summary>
    public const string InvalidFimCouponNumber = "BPAXNS_10680";

    /// <summary>
    /// Invalid your invoice billing period
    /// </summary>
    public const string InvalidYourInvoiceBillingPeriod = "BPAXNS_10681";

    /// <summary>
    /// Used when Reference Number is invalid.
    /// </summary>
    public const string InvalidReferenceCorrespondence = "BPAXNS_10683";

    /// <summary>
    /// Other than Sampling Form A/B, Vat Absorption amount should be 0
    /// </summary>
    public const string VatAbsorptionAmountIsInvalid = "BPAXNS_10684";

    /// <summary>
    /// Other than Sampling Form A/B, Vat Absorption percent should be 0
    /// </summary>
    public const string VatAbsorptionPercentIsInvalid = "BPAXNS_10685";

    /// <summary>
    /// Other than Sampling Form A/B, total provisional adjustment amount should be 0
    /// </summary>
    public const string TotalProvisionalAdjustmentAmountIsInvalid = "BPAXNS_10686";

    /// <summary>
    /// Coupon Tax records should be present if TaxAmount > 0.
    /// </summary>
    public const string ZeroCouponTaxRecordsForCouponTaxAmount = "BPAXNS_10687";

    /// <summary>
    /// Coupon Vat records should be present if VatAmount > 0.
    /// </summary>
    public const string ZeroCouponVatRecordsForCouponVatAmount = "BPAXNS_10688";

    /// <summary>
    /// Used when coupon record tax amount is not matching with coupon tax amount.
    /// </summary>
    public const string InvalidCouponTaxBreakdownAmount = "BPAXNS_10689";

    /// <summary>
    /// Used when coupon record vat amount is not matching with coupon vat amount.
    /// </summary>
    public const string InvalidCouponVatBreakdownAmount = "BPAXNS_10690";

    /// <summary>
    /// Invalid billing month and period - Billing month and period does not match with current open period.
    /// </summary>
    public const string InvalidBillingMonthAndPeriod = "BPAXNS_10691";

    /// <summary>
    /// Invalid billing period - Billing period should not be same for Prime Billing and Rejection Memo.
    /// </summary>
    public const string InvalidBillingPeriodForAuditTrail = "BPAXNS_10692";

    /// <summary>
    /// Provisional Billing member id and ticket issuing airline can not be same.
    /// </summary>
    /// <value>The invalid ticket issuing airlines.</value>
    public const string InvalidTicketIssuingAirlines = "BPAXNS_10693";

    /// <summary>
    /// Invalid electronic ticket indicator
    /// </summary>
    public const string InvalidElectronicTicketIndicator = "BPAXNS_10200";

    /// <summary>
    /// Invalid reason remark serial number.
    /// </summary>
    public const string InvalidReasonRemarkSerialNumber = "BPAXNS_10694";

    /// <summary>
    /// Invalid reason prorate slip serial number.
    /// </summary>
    public const string InvalidProrateSlipSerialNumber = "BPAXNS_10695";

    /// <summary>
    /// Used when there is an error while setting future update data
    /// </summary>
    public const string FutureUpdateErrorMemberDetails = "BMEM_10102";

    /// <summary>
    /// Coupon Total amount is not in the range of allowed min max amount.
    /// </summary>
    public const string CouponTotalAmountIsNotInAllowedRange = "BPAXNS_10696";

    /// <summary>
    /// Coupon total amount is not equal to sum of Gross Value, Tax Amount, Handling Fee, ISC Amount, Other Commission Amount, UATP Amount and VAT Amount
    /// </summary>
    public const string CouponTotalAmountDoesNotMatchWithSumOfOtherAmount = "BPAXNS_10697";

    /// <summary>
    /// TotalVatAmountAfterSamplingConstant should be populated in case of Billing Code 6 and 7 
    /// </summary>
    public const string TotalVatAmountAfterSamplingShouldBePopulatedFormXAndXF = "BPAXNS_10698";

    /// <summary>
    /// Net reject amount is not in the range of allowed min max amount.
    /// </summary>
    public const string NetRejectAmountIsNotInAllowedRange = "BPAXNS_10699";

    /// <summary>
    /// If FIM /Billing Memo/Credit Memo Number in Rejection Memo is blank , the "Net Reject Amount" should not exceed $100,000
    /// </summary>
    public const string InvalidRejectionMemoCouponNetRejectAmount = "BPAXNS_10700";

    /// <summary>
    /// Your invoice billing date exceeds the time-limit set for the Rejection Stage.
    /// </summary>
    public const string TimeLimitExceedsForYourInvoiceBillingDate = "BPAXNS_10701";

    /// <summary>
    /// Rejection stage of current rejection should not be same as your rejection record
    /// </summary>
    public const string InvalidYourInvoiceRejectionStage = "BPAXNS_10702";

    /// <summary>
    /// Your invoice number should not be populated for reason code "6A" and "6B"
    /// </summary>
    public const string InvalidYourInvoiceNumberForReasonCode6A6B = "BPAXNS_10703";

    /// <summary>
    /// FIM Coupon Number and FIM Number should be populated if Source Code  = 44 or 45 or 46.
    /// </summary>
    public const string MandatoryFimNumberAndFimCouponNumber = "BPAXNS_10704";

    /// <summary>
    /// Billing period should be same as current billing period.
    /// </summary>
    public const string InvalidBillingDate = "BPAXNS_10705";

    /// <summary>
    /// Linked coupon does not exists as we could not found linked invoice.
    /// </summary>
    public const string LinkedCouponDoesNotExistsAsInvoiceIsNoFound = "BPAXNS_10706";

    /// <summary>
    /// Billing period should be same as current billing period.
    /// </summary>
    public const string LinkedCouponDoesNotExistsAsRejectionIsNoFound = "BPAXNS_10707";

    /// <summary>
    /// Listing to Billing Rate is not equal to 1 when Currency of Listing and Currency of Billing are the same.
    /// </summary>
    public const string InvalidListingToBillingRateForSameCurrencies = "BGEN_00004";

    /// <summary>
    /// Used when there is member code numeric is duplicate
    /// </summary>
    public const string DuplicateMemberNumericCodeFound = "BMEM_10103";

    /// <summary>
    /// Used when there is an error while setting future update data
    /// </summary>
    public const string FutureUpdateErrorEBilling = "BMEM_10104";

    /// <summary>
    /// A member cannot create an invoice/creditNote or Form C when his IS Membership is ‘Basic’, ‘Restricted’ or ‘Terminated’.
    /// </summary>
    public const string InvalidBillingIsMembershipStatus = "BPAXNS_10709";

    /// <summary>
    /// Validation of the Billed Member will fail if the IS Membership Status of the Billed Member is ‘Terminated’.This will be a non-correctable error.
    /// </summary>
    public const string InvalidBilledIsMembershipStatus = "BPAXNS_10710";

    /// <summary>
    /// Used when Reference field 1 is empty.
    /// </summary>
    public const string InvalidReferenceField1ForCoupon = "BPAXNS_10727";

    /// <summary>
    /// Used when Reference field 2 is empty.
    /// </summary>
    public const string InvalidReferenceField2ForCoupon = "BPAXNS_10728";

    /// <summary>
    /// Used when Reference field 1 is empty.
    /// </summary>
    public const string InvalidReferenceField1ForPbCoupon = "BPAXNS_10807";

    /// <summary>
    /// Used when Reference field 2 is empty.
    /// </summary>
    public const string InvalidReferenceField2ForPbCoupon = "BPAXNS_10808";
    /// <summary>
    /// Used when Reference fields are empty.
    /// </summary>
    public const string InvalidReferenceFieldsForCoupon = "BPAXNS_10806";

    /// <summary>
    /// Used when Reference field 1 is empty.
    /// </summary>
    public const string InvalidReferenceField1ForBMCoupon = "BPAXNS_10809";

    /// <summary>
    /// Used when Reference field 2 is empty.
    /// </summary>
    public const string InvalidReferenceField2ForBMCoupon = "BPAXNS_10810";

    /// <summary>
    /// Used when there is duplicate email Id
    /// </summary>
    public const string DuplicateEmailIdFound = "BMEM_10104";

    /// <summary>
    /// Used when there is duplicate Blocking Rule
    /// </summary>
    public const string DuplicateBlockingRuleFound = "BMEM_10107";

    /// <summary>
    /// Used when there is error while connecting to database.
    /// </summary>
    public const string DatabaseConnectionError = "BMEM_10111";

    public const string InvalidIsUatpInvIgnoreFromDsprocError = "BMEM_10109";

    public const string InvalidIsUatpInvoiceHandledByAtcanError = "BMEM_10110";

    /// <summary>
    /// Check digit should be between 0-6 or 9.
    /// </summary>
    public const string InvalidCheckDigit = "BPAXNS_10199";

    /// <summary>
    /// Validation if suspended flag in true..
    /// </summary>
    public const string InvalidSuspendedFlag = "BMEM_10113";

    /// <summary>
    /// Invalid country code
    /// </summary>
    public const string InvalidCountryCode = "BPAXNS_10711";

    /// <summary>
    /// Validation if suspended flag in true..
    /// </summary>
    public const string InvalidRejectionFlag = "BMEM_1011";

    /// <summary>
    /// Validation if suspended flag in true..
    /// </summary>
    public const string OversizeMemberLogo = "BMEM_10116";

    /// <summary>
    /// Validation if organization designator is invalid
    /// </summary>
    //public const string InvalidOrganizationDesignator = "BPAXNS_10712";

    /// <summary>
    /// Invalid cabin class - It should be numeric only
    /// </summary>
    public const string InvalidCabinClass = "BPAXNS_10713";

    /// <summary>
    /// Invoice not found.
    /// </summary>
    public const string InvoiceNotFound = "InvoiceNotFound";
    public const string FormCNotFound = "FormCNotFound";

    /// <summary>
    /// Correspondence number found but not for the specified members.
    /// </summary>
    public const string InvalidCorrespondenceMembers = "BPAXNS_10714";

    public const string AuthorityToBillNotSetForCorrespondence = "BPAXNS_10715";

    public const string CorrespondenceStatusNotExpired = "BPAXNS_10716";

    public const string CorrespondenceStatusIsClosed = "BPAXNS_10717";

    public const string BillingMemoExistsForCorrespondence = "BPAXNS_10718";

    public const string SubmissionNotAllowedAfterSampleDigitAnnouncementDate = "BPAXNS_10719";

    public const string NetTotalDoesNotMatchWithTotalOfIndividualAmounts = "BPAXNS_10720";

    public const string GenerateICHSettlementXMLFailed = "BIS_10721";

    public const string ICHSettlementXMLValidationFailed = "BIS_10722";

    public const string SendICHSettlementXMLFailed = "BIS_10723";

    public const string SendInvoiceFailureNotificationToISAdminFailed = "BIS_10724";

    public const string UpdateInvoiceStatusAfterSettlementFailed = "BIS_10725";

    public const string GenerateAndSendICHSettlementXMLFailed = "BIS_10726";

    public const string ICHSettlementErrorNotificationFailed = "BIS_10727";

    public const string InvalidAcceptableAmountDifference = "BPAXNS_10721";

    public const string FileUploadBusinessException = "BCAL_10001";

    public const string FileUploadSanityCheckError = "BCAL_10002";

    public const string FileUploadSuccessful = "BCAL_10003";

    public const string FileUploadUnexpectedError = "BCAL_10004";

    public const string CalendarInputColumnMismatch = "BCAL_10005";

    public const string CalendarInvalidCHId = "BCAL_10006";

    public const string CalendarInputFileMoreColumns = "BCAL_10007";

    public const string CalendarInputFileLessColumns = "BCAL_10008";

    public const string InvalidFileExtension = "InvalidFileExtension";

    public const string FileNotUploadSuccessful = "BCAL_10009";

    public const string CalendarFileContainsBothCHInfo = "BCAL_10010";

    public const string CalendarPleaseSelectFileToUpload = "BCAL_10011";

    public const string InvalidFormE = "BPAXNS_10722";

    public const string InvalidCorrespondenceRefNumber1 = "BPAXNS_10723";

    public const string InvalidCorrespondenceRefNumber2 = "BPAXNS_10724";

    public const string InvalidTotalNoOfRecordsOfInvoiceTotal = "BPAXNS_10725";

    public const string InvalidTotalNoOfBillingRecordsOfInvoiceTotal = "BPAXNS_10726";

    //Used when invoice ResubmissionStatus is "B"(Bilateral) while resubmiting. 
    public const string InvoiceIsBilaterallySettled = "BDSHB_10001";

    public const string BillingMemberSuspended = "BDSHB_10002";

    public const string BilledMemberSuspended = "BDSHB_10003";

    public const string InvalidCityCodeAlpha = "BMSTCA_10001";

    public const string InvalidCityCodeNum = "BMSTCA_10002";

    public const string InvalidCurrencyCode = "BMSTCA_10003";

    public const string InvalidSisMemberSubStatus = "BMSTCA_10004";

    public const string InvalidSisMemberStatus = "BMSTCA_10005";

    public const string InvalidLocationIcaoCode = "BMSTCA_10006";

    public const string InvalidAircraftTypeIcaoCode = "BMSTCA_10007";

    public const string InvalidCountryIcaoCode = "BMSTCA_10008";

    /// <summary>
    /// Error code denote that Invalid Group & Misc Code combination because it is unique
    /// </summary>
    public const string InvalidMiscCode = "BMSTCA_10009";

    public const string InvalidAddOnChargeName = "BMSTCA_10010";

    public const string InvalidAirportIcaoCode = "BMSTCA_10011";

    public const string InvalidCgoRMReasonAcceptableDiff = "BMSTCA_10013";

    public const string InvalidNumericCurrencyCode= "BMSTCA_10014";

    public const string ExchangeRateAlreadyExists = "BMSTCA_10015";

    public const string MinMaxAcceptableAmountAlreadyExists = "BMSTCA_10016";

    public const string InvalidPaxRMReasonAcceptableDiff = "BMSTCA_10017";

    public const string InvalidReasonCodeTransactionType = "BMSTCA_10018";

    public const string InvalidRficCode = "BMSTCA_10019";

    public const string InvalidRfiscCode = "BMSTCA_10020";

    public const string InvalidSubDivisionCode = "BMSTCA_10021";

    public const string InvalidTaxSubType = "BMSTCA_10022";

    public const string InvalidTransactionType = "BMSTCA_10023";

    public const string InvalidUNLocationCode = "BMSTCA_10024";

    public const string InvalidUomCode = "BMSTCA_10025";

    public const string InvalidDigitAnnouncementDateTime = "BMSTCA_10026";

    public const string InvalidProvisionalBillingMonthAndDigitAnnouncementDateTime = "BMSTCA_10027";

    public const string InvalidAircraftTypeCode = "BMSTCA_10028";

    public const string InvalidMemberCode = "BMSTCA_10029";

    public const string InvalidCurrencyCodeAlpha = "BMSTCA_10030";

    public const string InvalidEffectiveFromDate = "BMSTCA_10031";

    public const string InvalidLanguageCode = "BMSTCA_10035";

    /// <summary>
    /// CMP#538: Throws error when combination of members are already available in database.
    /// </summary>
    public const string BvcAgreementCombinationAlreadyExists = "BMSTCA_10036";

    /// <summary>
    /// CMP#538
    /// </summary>
    public const string InvalidBvcAgreementDetails = "BPAXS_10255";


    /// <summary>
    /// CMP#538
    /// </summary>
    public const string MemberIsNotBvcParticipant = "BPAXS_10256";
      
      /// <summary>
    /// Used when vat identifier is invalid.
    /// </summary>
    public const string VatIdentifierAlreadyExists = "BMSTCA_10032";

    public const string CountryCodeAlreadyExists = "BMSTCA_10033";

    public const string InvalidCountryCodeofUnLocation = "BMSTCA_10034";

    /// <summary>
    /// Error code denoting invalid date and time for announcements
    /// </summary>
    public const string InvalidDateAndTimeForAnnouncements = "BANN_10001";

    /// <summary>
    /// Used when the billing member status is 'Basic' or 'Restricted' or 'Terminated'
    /// </summary>
    public const string InvalidBillingMemberStatus = "BPAXNS_10729";

    /// <summary>
    /// Used when the billed member status is 'Terminated'
    /// </summary>
    public const string InvalidBilledMemberStatus = "BPAXNS_10730";

    /// <summary>
    /// Net amount billed should be equal to amount to be settled for authority to bill
    /// </summary>
    public const string InvalidAmountToBeSettled = "BPAXNS_10731";

    /// <summary>
    /// There should be one transaction in the invoice header
    /// </summary>
    public const string MandatoryTransactionInInvoice = "BPAXNS_10732";

    /// <summary>
    /// FIM Number should be blank for sampling invoice
    /// </summary>
    public const string FimNumberShouldBeBlankForSampling = "BPAXNS_10733";

    /// <summary>
    /// FIM Coupon Number should be blank for sampling invoice
    /// </summary>
    public const string FimCouponNumberShouldBeBlankForSampling = "BPAXNS_10734";

    /// <summary>
    /// Prime billing net total amount should not be 0 or negative.
    /// </summary>
    public const string PrimeBillingNetTotalAmountShouldNotBeNegative = "BPAXNS_10735";

    /// <summary>
    /// Rejection memo net total amount should not be 0 or negative.
    /// </summary>
    public const string RejectionMemoNetTotalAmountShouldNotBeNegative = "BPAXNS_10736";

    /// <summary>
    /// Rejection memo coupon net total amount should not be 0 or negative.
    /// </summary>
    public const string RejectionMemoCouponNetTotalAmountShouldNotBeNegative = "BPAXNS_10737";

    /// <summary>
    /// Billing memo net total amount should not be 0 or negative.
    /// </summary>
    public const string BillingMemoNetTotalAmountShouldNotBeNegative = "BPAXNS_10738";

    /// <summary>
    /// Billing memo coupon net total amount should not be 0 or negative.
    /// </summary>
    public const string BillingMemoCouponNetTotalAmountShouldNotBeNegative = "BPAXNS_10739";

    /// <summary>
    /// Credit memo coupon net total amount should not be 0 or negative.
    /// </summary>
    public const string CreditMemoCouponNetTotalAmountShouldNotBeNegative = "BPAXNS_10740";

    /// <summary>
    /// For invoice type as CN, only CMs are allowed.	
    /// </summary>
    public const string InvoiceShouldNotHaveCreditNoteTransactions = "BPAXNS_10741";

    /// <summary>
    /// For invoice type as IV, CMs are not allowed.	
    /// </summary>
    public const string CreditNotShouldNotHaveOtherTransactions = "BPAXNS_10742";

    /// <summary>
    /// For SMI = A and billing code is of sampling then currency of listing should be 840(USD)	
    /// </summary>
    public const string ListingCurrencyForSamplingBillingCodeShouldBeUsd = "BPAXNS_10741";

    /// <summary>
    /// Member is not migrated for prime billing IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForPrimeIsIdec = "BPAXNS_10744";

    /// <summary>
    /// Member is not migrated for prime billing IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForPrimeIsXml = "BPAXNS_10745";

    /// <summary>
    /// Member is not migrated for rejection IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForRejectionIsIdec = "BPAXNS_10746";

    /// <summary>
    /// Member is not migrated for rejection IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForRejectionIsXml = "BPAXNS_10747";

    /// <summary>
    /// Member is not migrated for billing memo IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForBillingMemoIsIdec = "BPAXNS_10748";

    /// <summary>
    /// Member is not migrated for billing memo IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForBillingMemoIsXml = "BPAXNS_10749";

    /// <summary>
    /// Member is not migrated for credit memo billing IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForCreditMemoIsIdec = "BPAXNS_10750";

    /// <summary>
    /// Member is not migrated for credit memo billing IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForCreditMemoIsXml = "BPAXNS_10751";

    /// <summary>
    /// Member is not migrated for Provisional prime billing IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForProvisionalBillingIsIdec = "BPAXNS_10752";

    /// <summary>
    /// Member is not migrated for Provisional prime billing IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForProvisionalBillingIsXml = "BPAXNS_10753";

    /// <summary>
    /// Member is not migrated for sampling DE IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForSamplingDeIsIdec = "BPAXNS_10756";

    /// <summary>
    /// Member is not migrated for sampling DE IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForSamplingDeIsXml = "BPAXNS_10757";

    /// <summary>
    /// Member is not migrated for sampling FXF IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForSamplingXfIsIdec = "BPAXNS_10758";

    /// <summary>
    /// Member is not migrated for sampling FXF IS-IDEC.
    /// </summary>
    public const string MemberIsNotMigratedForSamplingXfIsXml = "BPAXNS_10759";

    /// <summary>
    /// Error code for error generated during generating and placing Recharge Data Xml file on FTP.
    /// </summary>
    public const string ErrorSendingRechargeDataNotification = "BSRD_10001";

    /// <summary>
    /// Invalid billing period, valid for late submission.
    /// </summary>
    public const string InvoiceValidForLateSubmission = "BPAXNS_10760";

    public const string InvalidDigitalSignatureValue = "BPAX_10188";

    public const string InvalidInvoiceMemberLocationInformation = "BPAXNS_10761";

    public const string InvalidInvoiceBillingMemberLocationInformation = "BGEN_10761";

    public const string ErrorInvoiceNotFound = "BPAXNS_10762";

    public const string NegativeSamplingConstant = "BPAXNS_10763";

    public const string ErrorReasonCodeCouponCount = "BPAXNS_10764";

    public const string ErrorReasonCodeBreakdownCheck = "BPAXNS_10765";

    public const string TimeLimitExpiryForCorrespondence = "BPAXNS_10766";

    /// <summary>
    /// Sampling constant should not be blank for sampling invoice
    /// </summary>
    public const string SamplingConstantShouldNotBeBlank = "BPAXNS_10767";

    public const string InvalidAcceptableAmountDifferenceValidate = "BPAXNS_10768";

    public const string InvalidBillingToMember = "BGEN_00003";

    public const string InvalidBillingFromMember = "BPAXNS_10770";

    public const string InvalidInvoiceTotalVat = "BPAXNS_10817";

    public const string ErrorReasonCodeNoBreakdownCheck = "BPAXNS_10771";

    public const string InvoiceLateSubmitted = "BPAX_10189";

    public const string ErrorNegativeRMNetAmount = "BPAXNS_10772";

    public const string CorrespondenceAmountIsNotInAllowedRange = "BPAXNS_10773";

    /// <summary>
    /// Net reject amount of RM coupon does not match with sum of gross amount difference, tax amount difference, handling fee difference, ISC amount difference, other commission amount difference, UATP amount difference and VAT amount difference.
    /// </summary>
    public const string NetRejectAmountDoesNotMatchWithSumOfDiffAmount = "BPAXNS_10774";

    /// <summary>
    /// Net credited amount not in range of allowed min max
    /// </summary>
    public const string NetCreditedAmountNotInRangeOfMinMax = "BPAXNS_10775";

    /// <summary>
    /// Net rejected amount does not match with sum of RM coupon breakdown records
    /// </summary>
    public const string NetRejectAmountDoesNotMatchWithSumOfChildRecords = "BPAXNS_10776";

    /// <summary>
    /// Total Net Amount without VAT should be equal to Net Total – Total Vat Amount.
    /// </summary>
    public const string TotalNetAmountWithoutVatIsInvalidForNs = "BPAXNS_10777";

    /// <summary>
    /// All Rejections Memos within the invoice should have the same Sampling Constant value
    /// </summary>
    public const string SamplingConstantShouldBeSameForAllRejections = "BPAXNS_10778";

    //Invalid record sequence number.
    public const string InvalidRecordSequenceNumber = "BPAXNS_10779";

    //Special characters are not allowed in Invoice Number
    public const string SpecialCharactersAreNotAllowedInInvoiceNumber = "BGEN_00001";

    //Invoice number should not be zero
    public const string InvoiceNumberAsZero = "BPAXNS_10781";

    //Billed airline code should be 9999 in input file - file total record
    public const string InvalidBilledAirlineCodeInFileTotal = "BPAXNS_10782";

    //Digital signature required should be Y/N/D
    public const string InvalidDigitalSignatureReqiredFlag = "BPAXNS_10190";

    //Provisional billing month should be 000000 in case Billing Code = 0 or 3
    public const string InvalidProvisionalBillingMonthForNs = "BPAXNS_10783";

    //RMOrBMOrCMNumber should be blank.
    public const string RMOrBMOrCMNumberShouldBeBlank = "BPAXNS_10784";

    //Invalid nil form c indicator for invoice
    public const string InvalidNilFormCIndicatorForInvoice = "BPAXNS_10785";

    //Net Total amount should be equal to Net Amount after Sampling Constant
    public const string InvalidNetTotalAmountForSamplingAb = "BPAXNS_10786";

    //Net Total should be equal to the sum of Total Gross Value and Total Provisional Adjustment  amount
    public const string InvalidNetTotalAmountForSamplingFxf = "BPAXNS_10787";

    //.In case of Billing Code = 6 or 7 This equals the sum of "Total Net Amount After Sampling Constant"  of all Source Code Total records within the Invoice
    //Invalid nil form c indicator for invoice
    public const string InvalidTotalNetAmountAfterSamplingConstant = "BPAXNS_10788";
    

    //Invalid form c xml.
    public const string  InvalidFormCXml = "BPAXNS_10781";

    //Invlid Issuing member Id
    public const string  InvlidIssuingOrganizationId = "BPAXNS_10782";

    //Invalid form c count.
    public const string InvalidFormCRecordCount = "BPAXNS_10783";

    //Exceeds limit of file can be sent in a day
    public const string ExceedsNoOfFileSentInADay = "BPAXNS_10784";

    //Exceeds limit of file can be sent in a day
    public const string FileExtractionError = "BPAXNS_10785";

    public const string ISCalendarDataNotFoundException = "ISCalendarDataNotFoundException";

    //Net billing amount does not match with Net Total / Listing to Billing Rate.
    public const string NetBillingAmountDoesNotMatchWithNetTotAndLbr = "BPAXNS_10789";

    public const string ValidationIgnoredDueToMigration = "ValidationIgnoredDueToMigration";

    public const string AlreadyBilaterallySettledInvoice = "BDSHB_10004";

    public const string AlreadyResubmitedInvoice = "BDSHB_10005";

    //Handling fee amount should not be 0 for valid handling fee type.
    public const string InvalidHandlingFeeAmountForValidHandFeeType = "BPAXNS_10793";

    //Handling fee type should be valid for valid handling fee amount.
    public const string InvalidHandlingFeeTypeForValidHandFeeAmount = "BPAXNS_10794";

    //FIMBMCMIndicator should be populated in case FIM Number/Billing Memo/Credit Memo Number is populated.
    public const string InvalidFimbmcmIndicatorForValidFimBmCmNumber = "BPAXNS_10795";

    //FIM Number/Billing Memo/Credit Memo Number should be populated in case FIMBMCMIndicator is populated.
    public const string InvalidFimBmCmNumberForValidFimbmcmIndicator = "BPAXNS_10796";

    //Billing memo number does not exists in the database.
    public const string BillingMemoNumberDoesNotExists = "BPAXNS_10797";

    //Credit memo number does not exists in the database.
    public const string CreditMemoNumberDoesNotExists = "BPAXNS_10798";

    //FIM number does not exists in the database.
    public const string FimNumberDoesNotExists = "BPAXNS_10799";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Your billing Memo Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForYourBillingMemoNumber = "BPAXNS_10800";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Your Credit Memo Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForYourCreditMemoNumber = "BPAXNS_10801";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Your FIM Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForYourFimNumber = "BPAXNS_10802";

    /// <summary>
    /// When invoice billing period is less than suspension period for resubmition.
    /// </summary>
    public const string UnableToResubmit = "BDSHB_10006";

    /// <summary>
    /// When Billing Memebr is not allowed to bill the blocked Billed Member from group.
    /// </summary>
    //public const string InvalidBillingToMemberGroup = "BPAXNS_10803";

    /// <summary>
    /// When Billed Memebr is not allowed to accept bill from  the blocked Billing Member from group.
    /// </summary>
      //public const string InvalidBillingFromMemberGroup = "BPAXNS_10804";

      /// <summary>
      /// Used when tax breakdown code record is invalid in coupon breakdown.
      /// </summary>
      public const string InvalidYourBillingPeriod = "BPAXNS_10812";

      /// <summary>
      ///
      /// </summary>
      public const string ParRateDoesNotMatchWithSumOfIndAbsorbPercent = "BPAXNS_10811";

      public const string CorrespondenceDoesNotExistsInTheDatabase = "BPAXNS_10813";

      public const string InvalidBatchSequenceNo = "BPAXNS_10814";
    
      public const string InvalidCorrespondencRefNo = "BPAXNS_10815";

      public const string InvalidAgreementIndicatorAndOriginalPmiCombination = "BPAXNS_10816";

      public const string InvalidBatchNo = "BPAXNS_10842";

      /// <summary>
      /// Used when the issuingOrganizationId not matches with billingMemberId
      /// </summary>
      public const string InvalidIssuingOrganizationId = "BGEN_00006";

      public const string DuplicateTransmitterExceptionRecordFound = "BPAXNS_10818";

      public const string WarningMessageForReasonCode1A = "BPAXNS_10819";

      /// <summary>
      /// Detail count
      /// </summary>
      public const string InvalidDetailCount = "BPAXNS_10820";

      public const string NotTheUserOfSelectedMember = "BMEM_10117";

     /// <summary>
     /// Used to set whwn error occurs while Uploading CSV.
     /// </summary>
     public const string ErrorUploadingCsv = "BGEN_00007";

     public const string ErrorCorrespondenceAlreadyCreated = "BPAXNS_10821";

     public const string InvalidBatchSequenceNoAndRecordNo = "BPAXNS_10822";

     /// <summary>
     /// Validation if Ticket/Documnet/FIM Number is less than 0.
     /// </summary>
     public const string InvalidTicketDocumnetOrFimNumber = "BPAXNS_10823";

     /// <summary>
     /// Validation if Ticket/Documnet/FIM Number is less than 0.
     /// </summary>
     public const string InvalidTicketOrFimNumber = "BPAXNS_10824";

     /// <summary>
     /// Your Invoice Number is mandatory for Rejection Stage "1" or "2" or "3".
     /// </summary>
     public const string MandatoryYourInvoiceNumberForRej1Rej2Rej3 = "BPAXNS_10825";

     /// <summary>
     /// YourRejectionNumber is mandatory for
     /// 1. Rejection Stage "2" or "3" and Billing Code = 0.
     /// 2. Billing Code = 7.
     /// </summary>
     public const string MandatoryYourRejectionMemoNumberForRej2Rej3 = "BPAXNS_10826";

     /// <summary>
     /// Validation if organization designator is invalid
     /// </summary>
     public const string InvalidOrganizationDesignator = "BGEN_00008";

     /// <summary>
     /// When Billed Member is not allowed to accept bill from  the blocked Billing Member from group.
     /// </summary>
     public const string InvalidBillingFromMemberGroup = "BGEN_00009";

     /// <summary>
     /// When Billing Memebr is not allowed to bill the blocked Billed Member from group.
     /// </summary>
     public const string InvalidBillingToMemberGroup = "BGEN_00010";

     /// <summary>
     /// Vat label is mandatory
     /// </summary>
     public const string InvalidVatLabel = "BPAXS_10242";

     /// <summary>
     /// Vat text is mandatory
     /// </summary>
     public const string InvalidVatText = "BPAXS_10243";
     /// <summary>
     /// Vat text lenght is validation
     /// </summary>
     public const string InvalidVatTextLenght = "BPAXS_10258";

     /// <summary>
     /// Vat text is mandatory
     /// </summary>
     public const string FutureSubmissionBillingCurrency = "BPAXS_10251";

     /// <summary>
     /// Vat text is mandatory
     /// </summary>
     public const string FutureSubmissionBillingMember = "BPAXS_10246";

     /// <summary>
     /// Vat text is mandatory
     /// </summary>
     public const string FutureSubmissionBilledMember = "BPAXS_10247";

     /// <summary>
     /// Vat text is mandatory
     /// </summary>
     public const string FutureSubmissionSMI = "BPAXS_10248";

     /// <summary>
     /// Vat text is mandatory
     /// </summary>
     public const string FutureSubmissionDs = "BPAXS_10249";

     /// <summary>
     /// Vat text is mandatory
     /// </summary>
     public const string FutureSubmissionLocation = "BPAXS_10250";

     /// <summary>
     /// Tax Difference amount is not in the range of allowed min amount.
     /// </summary>
     public const string TaxDifferenceAmountIsNotInAllowedRange = "BPAXNS_10822";

     /// <summary>
     /// Gross Difference amount is not in the range of allowed min amount.
     /// </summary>
     public const string GrossDifferenceAmountIsNotInAllowedRange = "BPAXNS_10838";

     public const string VoidPeriodValidationMsg = "BPAXNS_10839";

     public const string MinAcceptableAmountAlreadyExists = "BMSTCA_10824";

     public const string MaxAcceptableAmountAlreadyExists = "BMSTCA_10825";

     public const string ReferenceDataNotProvidedForFrequentFlyerRelatedBillings = "BPAXS_10245";

     /// <summary>
     /// Error code denoting invalid date and time for announcements
     /// </summary>
     public const string InvalidDateAndTimeForAnnouncementStartAndExpiryDate = "BANN_10002";

    
     /// <summary>
     /// Status for MEM.
     /// </summary>
     public const string CouponStatuMem = "BPAXAUTOBILL_00001";

     /// <summary>
     /// Status for ERR.
     /// </summary>
     public const string CouponStatuErr = "BPAXAUTOBILL_00002";

     /// <summary>
     /// Status for RNA.
     /// </summary>
     public const string CouponStatuRna = "BPAXAUTOBILL_00003";

     /// <summary>
     /// Status for RER.
     /// </summary>
     public const string CouponStatuRer = "BPAXAUTOBILL_00004";

     /// <summary>
     /// Status for RNC.
     /// </summary>
     public const string CouponStatuRnc = "BPAXAUTOBILL_00005";

     /// <summary>
     /// Status for PEN.
     /// </summary>
     public const string CouponStatuPen = "BPAXAUTOBILL_00006";

     /// <summary>
     /// Status for PEN.
     /// </summary>
     public const string CouponStatuRnb = "BPAXAUTOBILL_00007";

     /// <summary>
     /// Status for PEN.
     /// </summary>
     public const string CouponStatuRrb = "BPAXAUTOBILL_00008";

     /// <summary>
     /// Status for RNA.
     /// </summary>
     public const string CouponStatuRnd = "BPAXAUTOBILL_00009";

     /// <summary>
     /// Status for RNA.
     /// </summary>
     public const string CouponStatuVer = "BPAXAUTOBILL_00010";

     /// <summary>
     /// Status for Response not received as per SLA
     /// </summary>
     public const string CouponStatuNotPerSla = "BPAXAUTOBILL_00011";

     public const string InvalidCorrespondenceNumber = "BPAXNS_10827";
      
     public const string ExpiredCorrespondence = "BPAXNS_10828";

     public const string InvalidAuthorityToBill = "BPAXNS_10829";

     public const string InvalidEmailIds = "BPAXNS_10830";

     public const string FailedToSendMail = "BPAXNS_10831";

     public const string InvalidDigitalSignatureRequired = "BPAXNS_10832";

     public const string MandatoryFooterDetailRecord = "BGEN_00014";

     public const string SettlementMethodAlreadyExists = "BGEN_10824";
    
    public const string SettlementMethodDuplicateDescription = "BGEN_10825";

    /// <summary>
    /// Used as error code for RM level amount validation done from Stored procedure PROC_VALIDATE_MEMO.
    /// </summary>
    public const string AmountOutsideLimit = "BPAXNS_10833";

    public const string InvalidLanguage = "BPAXNS_10834";

    public const string DuplicateLanguage = "BPAXNS_10835";
 
    public const string LanguageFolderNotFound = "BPAXNS_10836";

    public const string DuplicateCorrspondenceNumber = "BPAXNS_10837";

    // For Correspondence
    public const string EnterEmailIds = "BPAXNS_10840";

    public const string InvalidCorrespondenceSubject = "BPAXNS_10841";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Rejection Memo's Fim Number and Fim Coupon Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForFimNumber = "BPAXNS_10843";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and  Rejection Memo's Bm Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForBmNumber = "BPAXNS_10844";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Rejection Memo's  Cm Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForCmNumber = "BPAXNS_10845";

    public const string ErrorCorrespondenceAlreadySent = "BPAXNS_10846";

    /// <summary>
    /// Error code used to check if Parent member Exist then user can't perform certain activity 
    /// </summary>
    public const string CheckforParentMember = "BGEN_10826";

    public const string NetTotalNotEqualsToSumOfOtherAmounts = "BPAXNS_10847";

    /// <summary>
    /// Memo level VAT breakdown should not be provided when RM/BM/CM has coupon breakdown information.
    /// </summary>
    public const string VatPresentWhenCouponBreakdownExists = "BPAXNS_10848";

    /// <summary>
    /// Invalid FimNumber i.e. cantains characters which are not number.
    /// </summary>
    public const string InvalidFimNumber = "BPAXNS_10849";

    /// <summary>
    /// FimBMCMIndicator value 'F' is mandatory for source code 44,45,46.
    /// </summary>
    public const string InvalidFimBmCmIndicatorforFim = "BPAXNS_10850";

    /// <summary>
    /// Incomplete reference data provided.
    /// </summary>
    public const string BuyerSellerIncompleteReferenceData = "BGEN_10827";


    /// <summary>
    /// Flight Date cannot be later than billing month.
    /// </summary>
    public const string FlightDateGreaterThanBillingMonth = "BPAXNS_10851";

    /// <summary>
    /// Total length of Reason Remarks for a Memo cannot exceed 4000 characters
    /// </summary>
    public const string MaxReasonRemarkCharLength = "BGEN_10828";


    // CMP#459 : Error Messages for Scenario 1/2
    public const string MismatchOnGross = "BPAXNS_10880";
    public const string MismatchOnTax = "BPAXNS_10853";
    public const string MismatchOnIsc = "BPAXNS_10854";
    public const string MismatchOnOc = "BPAXNS_10855";
    public const string MismatchOnHandlingFee = "BPAXNS_10856"; 
    public const string MismatchOnUatp = "BPAXNS_10857";
    public const string MismatchOnVat = "BPAXNS_10858";

    // CMP#459 : Error Messages for Error Correction Scenario 1
    public const string ErrCorrMismatchOnGross = "BPAXNS_10902";
    public const string ErrCorrMismatchOnTax = "BPAXNS_10903";
    public const string ErrCorrMismatchOnIsc = "BPAXNS_10904";
    public const string ErrCorrMismatchOnOc = "BPAXNS_10905";
    public const string ErrCorrMismatchOnHandlingFee = "BPAXNS_10906";
    public const string ErrCorrMismatchOnUatp = "BPAXNS_10907";
    public const string ErrCorrMismatchOnVat = "BPAXNS_10908";

    // CMP#459 : Error Messages for Error Correction Scenario 2
    public const string ErrCorrMismatchOnTotalGrossBilled = "BPAXNS_10909";
    public const string ErrCorrMismatchOnTaxBilled = "BPAXNS_10910";
    public const string ErrCorrMismatchOnIscAllowed = "BPAXNS_10911";
    public const string ErrCorrMismatchOnOcAllowed = "BPAXNS_10912";
    public const string ErrCorrMismatchOnHandlingFeeAllowed = "BPAXNS_10913";
    public const string ErrCorrMismatchOnUatpAllowed = "BPAXNS_10914";
    public const string ErrCorrMismatchOnVatBilled = "BPAXNS_10915";

    // CMP#459 : Error Messages for Error Correction Scenario 3/4
    public const string ErrCorrMismatchOnTotalGross = "BPAXNS_10916";
    public const string ErrCorrMismatchOnTotalTax = "BPAXNS_10917";
    public const string ErrCorrMismatchOnTotalIsc = "BPAXNS_10918";
    public const string ErrCorrMismatchOnTotalOc = "BPAXNS_10919";
    public const string ErrCorrMismatchOnTotalHandlingFee = "BPAXNS_10920";
    public const string ErrCorrMismatchOnTotalUatp = "BPAXNS_10921";
    public const string ErrCorrMismatchOnTotalVat = "BPAXNS_10922";

    // CMP#459 : Error Messages for Scenario 3/4
    public const string MismatchOnTotalGross = "BPAXNS_10859";
    public const string MismatchOnTotalTax = "BPAXNS_10860";
    public const string MismatchOnTotalIsc = "BPAXNS_10861";
    public const string MismatchOnTotalOc = "BPAXNS_10862";
    public const string MismatchOnTotalHandlingFee = "BPAXNS_10863";
    public const string MismatchOnTotalUatp = "BPAXNS_10864";
    public const string MismatchOnTotalVat = "BPAXNS_10865";

    // CMP#459 : Error Messages for Scenario 5/6/8
    public const string MismatchOnAcceptedTotalGross = "BPAXNS_10866";
    public const string MismatchOnAcceptedTotalTax = "BPAXNS_10867";
    public const string MismatchOnAcceptedTotalIsc = "BPAXNS_10868";
    public const string MismatchOnAcceptedTotalOc = "BPAXNS_10869";
    public const string MismatchOnAcceptedTotalHandlingFee = "BPAXNS_10870";
    public const string MismatchOnAcceptedTotalUatp = "BPAXNS_10871";
    public const string MismatchOnAcceptedTotalVat = "BPAXNS_10872";

    // CMP#459 : Error Messages for  Error Correction Scenario 5/6/8
    public const string ErrCorrMismatchOnAcceptedTotalGross = "BPAXNS_10923";
    public const string ErrCorrMismatchOnAcceptedTotalTax = "BPAXNS_10924";
    public const string ErrCorrMismatchOnAcceptedTotalIsc = "BPAXNS_10925";
    public const string ErrCorrMismatchOnAcceptedTotalOc = "BPAXNS_10926";
    public const string ErrCorrMismatchOnAcceptedTotalHandlingFee = "BPAXNS_10927";
    public const string ErrCorrMismatchOnAcceptedTotalUatp = "BPAXNS_10928";
    public const string ErrCorrMismatchOnAcceptedTotalVat = "BPAXNS_10929";

    // CMP#459 : Error Messages for Scenario 7
    public const string MismatchOnEvaluatedGross = "BPAXNS_10873";
    public const string MismatchOnEvaluatedTax = "BPAXNS_10874";
    public const string MismatchOnEvaluatedIsc = "BPAXNS_10875";
    public const string MismatchOnEvaluatedOc = "BPAXNS_10876";
    public const string MismatchOnEvaluatedHandlingFee = "BPAXNS_10877";
    public const string MismatchOnEvaluatedUatp = "BPAXNS_10878";
    public const string MismatchOnEvaluatedVat = "BPAXNS_10879";

    // CMP#459 : Error Messages for  Error Correction Scenario 7
    public const string ErrCorrMismatchOnEvaluatedGross = "BPAXNS_10930";
    public const string ErrCorrMismatchOnEvaluatedTax = "BPAXNS_10931";
    public const string ErrCorrMismatchOnEvaluatedIsc = "BPAXNS_10932";
    public const string ErrCorrMismatchOnEvaluatedOc = "BPAXNS_10933";
    public const string ErrCorrMismatchOnEvaluatedHandlingFee = "BPAXNS_10934";
    public const string ErrCorrMismatchOnEvaluatedUatp = "BPAXNS_10935";
    public const string ErrCorrMismatchOnEvaluatedVat = "BPAXNS_10936";

    // CMP#459 : Error Messages for ISWEB Scenario 2
    public const string IsWebMismatchOnGross = "BPAXNS_10881";
    public const string IsWebMismatchOnTax = "BPAXNS_10882";
    public const string IsWebMismatchOnIsc = "BPAXNS_10883";
    public const string IsWebMismatchOnOc = "BPAXNS_10884";
    public const string IsWebMismatchOnHandlingFee = "BPAXNS_10885";
    public const string IsWebMismatchOnUatp = "BPAXNS_10886";
    public const string IsWebMismatchOnVat = "BPAXNS_10887";

    // CMP#459 : Error Messages for ISWEB Scenario 3/4
    public const string IsWebMismatchOnTotalGross = "BPAXNS_10888";
    public const string IsWebMismatchOnTotalTax = "BPAXNS_10889";
    public const string IsWebMismatchOnTotalIsc = "BPAXNS_10890";
    public const string IsWebMismatchOnTotalOc = "BPAXNS_10891";
    public const string IsWebMismatchOnTotalHandlingFee = "BPAXNS_10892";
    public const string IsWebMismatchOnTotalUatp = "BPAXNS_10893";
    public const string IsWebMismatchOnTotalVat = "BPAXNS_10894";

    // CMP#459 : Error Messages for ISWEB Scenario 5/6/8
    public const string IsWebMismatchOnAcceptedTotalGross = "BPAXNS_10895";
    public const string IsWebMismatchOnAcceptedTotalTax = "BPAXNS_10896";
    public const string IsWebMismatchOnAcceptedTotalIsc = "BPAXNS_10897";
    public const string IsWebMismatchOnAcceptedTotalOc = "BPAXNS_10898";
    public const string IsWebMismatchOnAcceptedTotalHandlingFee = "BPAXNS_10899";
    public const string IsWebMismatchOnAcceptedTotalUatp = "BPAXNS_10900";
    public const string IsWebMismatchOnAcceptedTotalVat = "BPAXNS_10901";

    /// <summary>
    /// CMP496: Add validation error code.
    /// </summary>
    public const string InvalidMemberReferenceDataInformation = "BGEN_10829";
    /// <summary>
    /// CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured
    /// Size of Ticket/Document cannot be Number is greater than 10 digits. (Non Sampling)
    /// </summary>
    public const string TicketFimDocumentNoGreaterThanTenNs = "BPAXNS_10852";

    /// <summary>
    /// Your Rejection Memo No. cannot be provided for this rejection stage.
    /// </summary>
    public const string YourRejectionNumberNotRequired = "BPAXNS_10937";

    /// <summary>
    /// CMP464: error code
    /// </summary>
    public const string InvalidTaxVatLength = "BPAXNS_10938";

    public const string InvalidYourInvoiceBillingDatePeriod = "BGEN_10830";

    /// <summary>
    /// Used when issuing airline member code is invalid
    /// </summary>
    public const string InvalidIscAmount = "BGEN_10834";

    public const string InvalidInvoiceSummaryVat = "BPAXNS_10939";

    //SCP106534: ISWEB No-02350000768 
    //Desc: Error message to communicate internal DB error while creating a correspondence
    //Date: 20/06/2013
    public const string InternalDBErrorInCorrespondenceCreation = "BGEN_10840";

    //SCP#140863 : Rounding tolerance issue 
    public const string FdrNotFound = "BGEN_10841";

    //SCP106534: ISWEB No-02350000768 
    //Desc: Error message to communicate internal DB error while creating a correspondence
    //Date: 20/06/2013
    public const string CorrespondenceConcurrentUpdateError = "BGEN_10842";

    /// <summary>
    /// Used when Record sequence number within batch is not properly ordered or missing
    /// </summary>
    public const string InvalidRecordSequenceNumberOrder = "BGEN_10835";

    //SCP210204: IS-WEB Outage (QA Issue Fix)
    public const string InvalidCorrespondenceAmountToBeSettled = "BPAXNS_10941";

     /// <summary>
    /// Used when Invalid Rejection stage is trapped.
    /// SCP225675: //Urgent// About the incoming XML file for SEP P4 
    /// </summary>
    public const string InvalidRejectionStageAttemptedToSave = "BGEN_10843";

    //SCP219674: InvalidAmountToBeSettled Validation
    public const string BMWithReasonCode6A6BcannotRejected = "BGEN_10844";

    // SCP223072: Unable to change Member Code
    /// <summary>
    /// Used when Member Code Numeric i.e Member Prefix is not able to change.
    /// </summary>
    public const string UnableToChangeMemberPrefix = "BMEM_10118";

    // SCP249173: Sampling Form XF already exists for Your Inv, Your RM, Your Billing Period (YYYYMMPP)
    public const string SamplingFormXFAlreadyExistsforYourInvRmBillingPeriod = "BPAXNS_10942";
    ///<summary>
    /// Used when record restrict for update/delete/insert.
    /// </summary>
    public const string RecordRestrictFromISWEB = "BGEN_10891";

    //CMP #614: Source Code Validation for PAX RMs
    public const string PaxSourceCodes = "BPAXS_10262";

    #region CMP #624: ICH Rewrite-New SMI X.
    ///<summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #1
    /// </summary>
    public const string IchMembershipCheckForSmiX = "BGEN_10892";

    ///<summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #2
    /// </summary>
    public const string ChAgreementIndicatorCheckForSmiX = "BGEN_10893";

    ///<summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.11,  New Validation #1
    /// </summary>
    public const string ChAgreementIndicatorCheckForSmiOtherThanX = "BGEN_10894";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.11,  New Validation #2
    /// </summary>
    public const string ChAgreementIndicatorCheckForSampling = "BGEN_10895";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.11,  New Validation #3
    /// </summary>
    public const string ChDueDateCheckForSampling = "BGEN_10896";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.11,  New Validation #4
    /// </summary>
    public const string ChDueDateCheckForSmiOtherThanX = "BGEN_10897";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.11,  New Validation #4
    /// </summary>
    public const string ChDueDateInvalidForSmiX = "BGEN_10898";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #4
    /// </summary>
    public const string PaxSamplingCheckForSmiX = "BPAXS_10261";

    
    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #7
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.14,  New Validation #9 Point 7
    /// </summary>
    public const string PaxNSRejctionInvoiceLinkingCheckForSmiX = "BPAXNS_10943";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #8
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.14,  New Validation #6
    /// </summary>
    public const string PaxNSBmInvoiceLinkingCheckForSmiX = "BPAXNS_10944";


    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.14,  New Validation #9 Point 6
    /// </summary>
    public const string PaxNSRejInvBHLinkingCheckForSmiX = "BPAXNS_10945";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.8,  New Validation #8
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.14,  New Validation #6
    /// </summary>
    public const string PaxNsStandaloneBmInvLinkCheckForSmiX = "BPAXNS_10946";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.9
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.13
    /// </summary>
    public const string IchWebServicefailureMessage = "BGEN_10899";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X.
    /// TFS BUG #
    ///Description: This is added to prevent concurrent changes to invoice header.
    /// </summary>
    public const string PreventConcurrentInvoiceHeaderUpdate = "BGEN_10900";

    #endregion

    // CMP#459 : Error Messages for ISWEB Scenario 1 
    public const string IsWebMismatchOnGross1 		= "BPAXNS_10947";
    public const string IsWebMismatchOnTax1 		= "BPAXNS_10948";
    public const string IsWebMismatchOnIsc1 		= "BPAXNS_10949";
    public const string IsWebMismatchOnOc1 			= "BPAXNS_10950";
    public const string IsWebMismatchOnHandlingFee1 = "BPAXNS_10951";
    public const string IsWebMismatchOnUatp1 		= "BPAXNS_10952";
    public const string IsWebMismatchOnVat1 		= "BPAXNS_10953";

  // CMP#459 : Error Messages for Scenario 7 (FILE)
    public const string FileMismatchOnEvaluatedGross 		= "BPAXNS_10954";
    public const string FileMismatchOnEvaluatedTax 			= "BPAXNS_10955";
    public const string FileMismatchOnEvaluatedIsc 			= "BPAXNS_10956";
    public const string FileMismatchOnEvaluatedOc 			= "BPAXNS_10957";
    public const string FileMismatchOnEvaluatedHandlingFee 	= "BPAXNS_10958";
    public const string FileMismatchOnEvaluatedUatp 		= "BPAXNS_10959";
    public const string FileMismatchOnEvaluatedVat 			= "BPAXNS_10960";

    /* error Message: Error storing invoice/credit note owner details, prlease try again. 
    * Desc: Server side validation is added to prevent value 0 as Invoice/Credit Note owner Id. 
     * This is added as a fix for SCP# 303708 - Invoice Search - Note Owner.
    */
    public const string OwnerMissing = "BGEN_10901";

    /* Name: BPAXNS_10961	
       Value: Invalid Member Type	
       Comment: CMP #596: Length of Member Accounting Code to be Increased to 12. 3.4 Point 21 - New validation #MW2 
    */
    public const string InvalidMemberType = "BPAXNS_10961";

    /* Name: BPAXNS_10961	
     Value: Restrictions exist on your type of Accounting Code to create Invoice/Credit Notes in this Billing Category
     Comment: CMP #596: Length of Member Accounting Code to be Increased to 12. 3.4 Point 22, 27 - New validation #MW3 
    */
    public const string InvalidBillingMemberType = "BPAXNS_10962";

    /* Name: BPAXS_10280	
     Value: Restrictions exist on your type of Accounting Code to create a Form C
     Comment: CMP #596: Length of Member Accounting Code to be Increased to 12. 3.4 Point 22, 27 - New validation #MW3
    */
    public const string InvalidMemberTypeForSampling = "BPAXS_10281";

    #region Error Codes Related To CMP#608: Load Member Profile - CSV Option

    #region IS-Web Related Validation

    /// <summary>
    /// Error Message: No file chosen
    /// </summary>
    public const string NoFileChosen = "BGEN_10902";

    /// <summary>
    /// Error Message: Please choose a file in compressed format with a .zip extension
    /// </summary>
    public const string FileChosenIsNotZIP = "BGEN_10903";

    /// <summary>
    /// Error Message: File name is too long. The maximum permissible length is 50 including the extension
    /// </summary>
    public const string InvalidFileNameLength = "BGEN_10904";

    public const string InvalidUnzipFileNameLength = "BGEN_10905"; 

    #endregion

    #region CSV Upload Validation Level 1

    /// <summary>
    /// Error Message: This is a duplicate of another file having the same file name uploaded in the past.
    /// </summary>
    public const string L1Dot1 = "BMEM_10119";

    /// <summary>
    /// Error Message: Decompression of the file was unsuccessful.
    /// </summary>
    public const string L1Dot2 = "BMEM_10120";

    /// <summary>
    /// Error Message: The uncompressed file did not have extension as CSV
    /// </summary>
    public const string L1Dot3 = "BMEM_10121";

    /// <summary>
    /// Error Message: Two or more files were provided in the zip file
    /// </summary>
    public const string L1Dot3a = "BMEM_10122";

    /// <summary>
    /// Error Message: The CSV file’s name is too long. The maximum permissible length is 50 including the extension
    /// </summary>
    public const string L1Dot4 = "BMEM_10123";

    /// <summary>
    /// Error Message: The uncompressed CSV file did not have enough data. A minimum of one header row and one data row is required.
    /// </summary>
    public const string L1Dot5 = "BMEM_10124";

    /// <summary>
    /// Error Message: Mismatch in number of columns in row number
    /// </summary>
    public const string L1Dot6 = "BMEM_10125";

    #endregion

    #region CSV Upload Validation Level 2

    /// <summary>
    /// Error Message: One or more characters in the value provided do not correspond to the data type defined for this field.
    /// </summary>
    public const string L2Dot1 = "BMEM_10126";

    /// <summary>
    /// Error Message: This field is mandatory, and no value was provided.
    /// </summary>
    public const string L2Dot2 = "BMEM_10127";

    /// <summary>
    /// Error Message: The length of the value provided for this field is lower than the minimum length required, which is 
    /// </summary>
    public const string L2Dot3 = "BMEM_10128";

    /// <summary>
    /// Error Message: The length of the value provided for this field exceeds the maximum permissible length, which is 
    /// </summary>
    public const string L2Dot4 = "BMEM_10129";

    /// <summary>
    /// Error Message: Duplicate value found within the CSV file.
    /// </summary>
    public const string F1Dot1 = "BMEM_10130";

    /// <summary>
    /// Error Message: Another Member with the same code already exists in the Member Profile.
    /// </summary>
    public const string F1Dot2 = "BMEM_10131";

    /// <summary>
    /// Error Message: Four numeric codes should have a value 3600 or greater.
    /// </summary>
    public const string F1Dot3 = "BMEM_10132";

    /// <summary>
    /// Error Message: Invalid value.
    /// </summary>
    public const string F5Dot1 = "BMEM_10133";

    /// <summary>
    /// Error Message: Invalid or inactive value.
    /// </summary>
    public const string F6Dot1 = "BMEM_10134";

    /// <summary>
    /// Error Message: Invalid or inactive value.
    /// </summary>
    //public const string F16Dot1 = 9,

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    public const string F17Dot1 = "BMEM_10135";

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F18Dot1 = 11,

    /// <summary>
    /// Error Message: Values cannot be provided in this field as value of DigitalSignApplication is N.
    /// </summary>
    public const string F19Dot1 = "BMEM_10136";

    /// <summary>
    /// Error Message: Incorrect data representation.
    /// </summary>
    public const string F19Dot2 = "BMEM_10137";

    /// <summary>
    /// Error Message: Country code {0} is invalid or inactive.
    /// </summary>
    public const string F19Dot3 = "BMEM_10138";

    /// <summary>
    /// Error Message: Country code {0} does not support Digital Signature.
    /// </summary>
    public const string F19Dot4 = "BMEM_10139";

    /// <summary>
    /// Error Message: Values cannot be provided in this field as value of DigitalSignApplication is N.
    /// </summary>
    //public const string F20Dot1 = 16,

    /// <summary>
    /// Error Message: Incorrect data representation.
    /// </summary>
    //public const string F20Dot2 = 17,

    /// <summary>
    /// Error Message: Country code {0} is invalid or inactive.
    /// </summary>
    //public const string F20Dot3 = 18,

    /// <summary>
    /// Error Message: Country code {0} does not support Digital Signature.
    /// </summary>
    //public const string F20Dot4 = 19,

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F22Dot1 = 20,

    /// <summary>
    /// Error Message: A value cannot be provided in this field as value of LegalArchivingRequired is N.
    /// </summary>
    public const string F23Dot1 = "BMEM_10140";

    /// <summary>
    /// Error Message: A value is missing for this field and should be provided as value of LegalArchivingRequired is Y.
    /// </summary>
    public const string F23Dot2 = "BMEM_10141";

    /// <summary>
    /// Error Message: Space(s) are not allowed in this field.
    /// </summary>
    public const string F23Dot3 = "BMEM_10142";

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F24Dot1 = 24,

    /// <summary>
    /// Error Message: Value cannot be Y as value of LegalArchivingRequired is N.
    /// </summary>
    public const string F24Dot2 = "BMEM_10143";

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F25Dot1 = 26,

    /// <summary>
    /// Error Message: Value cannot be Y as value of LegalArchivingRequired is N.
    /// </summary>
    //public const string F25Dot2 = 27,

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F26Dot1 = 28,

    /// <summary>
    /// Error Message: Value cannot be Y as value of LegalArchivingRequired is N.
    /// </summary>
    //public const string F26Dot2 = 29,

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F27Dot1 = 30,

    /// <summary>
    /// Error Message: Value cannot be Y as value of LegalArchivingRequired is N.
    /// </summary>
    //public const string F27Dot2 = 31,

    /// <summary>
    /// Error Message: Incorrect data representation.
    /// </summary>
    //public const string F28Dot1 = 32,

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F29Dot1 = 33,

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F31Dot1 = 35,

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F32Dot1 = 36,

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F33Dot1 = 37,

    /// <summary>
    /// Error Message: Invalid value. Should be Y or N.
    /// </summary>
    //public const string F34Dot1 = 38,

    /// <summary>
    /// Error Message: Invalid email address.
    /// </summary>
    public const string F36Dot1 = "BMEM_10144";

    /// <summary>
    /// Error Message: A value cannot be provided in this field as no value is provided for NewUserFirstName.
    /// </summary>
    public const string F36Dot2 = "BMEM_10145";

    /// <summary>
    /// Error Message: A value is missing for this field and should be provided as a value is provided for NewUserFirstName.
    /// </summary>
    public const string F36Dot3 = "BMEM_10146";

    /// <summary>
    /// Error Message: Duplicate value found within the CSV file.
    /// </summary>
    //public const string F36Dot4 = 42,

    /// <summary>
    /// Error Message: Another user with the same email address already exists in the system.
    /// </summary>
    public const string F36Dot5 = "BMEM_10147";

    /// <summary>
    /// Error Message: A value cannot be provided in this field as no value is provided for NewUserEmailAddress.
    /// </summary>
    public const string F37Dot1 = "BMEM_10148";

    /// <summary>
    /// Error Message: A value is missing for this field and should be provided as a value is provided for NewUserEmailAddress.
    /// </summary>
    public const string F37Dot2 = "BMEM_10149";

    /// <summary>
    /// Error Message: A value cannot be provided in this field as no value is provided for NewUserEmailAddress.
    /// </summary>
    //public const string F38Dot1 = 46,

    /// <summary>
    /// Error Message: A value cannot be provided in this field as no value is provided for NewUserEmailAddress.
    /// </summary>
    //public const string F39Dot1 = 47,

    /// <summary>
    /// Error Message: Invalid value. Should be S or N.
    /// </summary>
    public const string F39Dot2 = "BMEM_10150";

    /// <summary>
    /// Error Message: A value cannot be provided in this field as no value is provided for NewUserEmailAddress.
    /// </summary>
    //public const string F40Dot1 = 50,

    /// <summary>
    /// Error Message: Permission template does not exist in the system.
    /// </summary>
    public const string F40Dot2 = "BMEM_10151";

    /// <summary>
    /// Error Message: Invalid value.
    /// </summary>
    public const string F30Dot1 = "BMEM_10152";

    #endregion

    #endregion

    //CMP #641: Time Limit Validation on Third Stage PAX Rejections [Start]
    public const string InvalidTimeLimitIsWebPayable = "BPAXNS_10963";
    public const string InvalidTimeLimitIsWebValInvoice = "BPAXNS_10964";
    public const string InvalidTimeLimitIsWeb = "BPAXNS_10965";
    public const string InvalidTimeLimitErrorCorrection = "BPAXNS_10966";
    public const string InvalidTimeLimit = "BPAXNS_10967";
    public const string BeyondTimeLimitIsWebValInvoice = "BPAXNS_10968";
    public const string BeyondTimeLimitIsWebBillingHistory = "BPAXNS_10969";
    public const string BeyondTimeLimitIsWeb = "BPAXNS_10970";
    public const string BeyondTimeLimitErrorCorrection = "BPAXNS_10971";
    public const string BeyondTimeLimit = "BPAXNS_10972";
      //CMP #641: Time Limit Validation on Third Stage PAX Rejections [End]


      #region CMP#674: Validation of Coupon and AWB Breakdowns in Rejections

      //Error in RM No. {0}, Batch No. {1}, Seq. No. {2} due to mismatch in coupon {3}-{4}-{5}. It was billed {6} time(s) in the rejected RM; and {7} time(s) in this RM.
      public const string PaxRMCouponMismatchIsWeb = "BPAXNS_10973";
      
      //Mismatch in coupon {0}-{1}-{2}. It was billed {3} time(s) in the rejected RM; and {4} time(s) in this RM. Other mismatches if any are not included in this report.
      public const string PaxRMCouponMismatchFileValidation = "BPAXNS_10978";

      #endregion

    //CMP #553: ACH Requirement for Multiple Currency Handling-FRS-v1.1
    public const string DuplicateAchCurrency = "BMSTCA_10826";

    /// <summary>
    ///CMP #553: ACH Requirement for Multiple Currency Handling Validation #2
    /// </summary>
    public const string InvalidOrInactiveCurrencyOfBilling = "BPAXNS_10975";

    /// <summary>
    ///CMP #553: ACH Requirement for Multiple Currency Handling Validation #3
    /// </summary>
    public const string CurrencyOfListingShouldBeSameAsCurrencyOfBilling = "BPAXNS_10976";

    /// <summary>
    ///CMP #553: ACH Requirement for Multiple Currency Handling Validation #4
    /// </summary>
    public const string CurrencyOfListingEvaluationShouldBeSameAsCurrencyOfBilling = "BPAXNS_10979";


    #region CMP#672: Validation on Taxes in PAX FIM Billings

    //Taxes cannot be billed for FIMs, even with a zero value.
    public const string TaxCannotbeBilledForSourceCode14Fim = "BPAXNS_10990";

    #endregion

      // CMP#673: Validation on Correspondence Reference Number in PAX/CGO Billing Memos
      public const string CorrRefNumberCannotBeProvidedForNon6Aor6Bbm = "BPAXNS_10980";
      public const string CorrRefNumberCannotBeProvidedForNon6Aor6BbmReasonCode = "BPAXNS_10981";

      /* CMP #671: Validation of PAX CGO Stage 2 & 3 Rejection Memo Reason Text */
      //Error Desc: It is mandatory for 2nd and 3rd stage rejections to have detailed rejection reasons as per RAM CH A10 para 2.3.	
      public const string MinReasonRemarkCharLength = "BGEN_10907";
  }
}

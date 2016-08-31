namespace Iata.IS.Business
{
  /// <summary>
  /// Holds the Error code for the Business logic validation intended for Cargo Billing Code.
  /// </summary>
  public struct CargoErrorCodes
  {

     /// <summary>
    /// Used when there is VAT and VAT Breakdown Total Amount mismatch.
     /// </summary>
     public const string OtherChargeTotalAmountMismatch = "BCGONS_101011";

     /// <summary>
     /// Used when 'Origin airport code' passed in coupon record is invalid
     /// </summary>
     public const string InvalidOriginCode = "BCGONS_10110";

     /// <summary>
     /// Used when 'Destination airport code' passed in coupon record is invalid
     /// </summary>
     public const string InvalidDestinationCode = "BCGONS_10111";

     /// <summary>
     /// Used when 'From Airport and To Airport code' passed in awb record are same
     /// </summary>
     public const string InvalidFromToAirports = "BCGONS_10112";

     /// <summary>
     /// Used when Cargo billing memo vat breakdown record not found, if BM record exist and total billed vat amount in non-zero.
     /// </summary>
     public const string CgoBMVatBreakdownRecordNotFound = "CGOBM_00001";

     /// <summary>
     /// Used when Cargo billing memo Awb vat breakdown record not found, if Awb record exist and total billed vat amount in non-zero.
     /// </summary>
     public const string CgoBMAwbVatBreakdownRecordNotFound = "CGOBM_00002";

     //// SCP107981: to/point of transfer population error 
     ///// <summary>
     ///// Used when invalid Carriage entered from UI
     ///// </summary>
     //public const string InvalidCarriageCombination = "CGOBM_00003";

     /// <summary>
     /// Used when 'From Carriage code' passed in Awb record is invalid
     /// </summary>
     public const string InvalidFromCarriageCode = "BCGO_10299";

     /// <summary>
     /// Used when 'From Carriage code' passed in Awb record is invalid
     /// </summary>
     public const string InvalidToCarriageCode = "BCGO_10300";

     public const string CgoBMInvalidBatchSequenceNo = "CGOBM_00006";

     public const string CgoBMInvalidAmount = "CGOBM_00007";

     public const string CgoBillingMemoVatBreakdownRecordNotFound = "CGOBM_00008";

     public const string CgoInvalidYourBillingPeriod = "CGOBM_00009";

     public const string CgoBMExistsForCorrespondence = "CGOBM_00010";

     public const string CgoInvalidReasonCode = "CGOBM_00011";

     public const string CgoDuplicateBillingMemoFound = "CGOBM_00012";

     /// <summary>
     /// Used when, Correspondence Ref. No. mentioned does not exist in the correspondence table as a correspondence between the 2 members between which the BM is being created.
     /// </summary>
     public const string CgoBMReferenceCorrespondenceDoesNotExist = "CGOBM_00013";

     public const string CgoCorrespondenceStatusIsClosed = "CGOBM_00014";

     public const string CgoInvalidCorrespondenceMembers = "CGOBM_00015";

     public const string CgoAuthorityToBillNotSetForCorrespondence = "CGOBM_00016";

     public const string CgoCorrespondenceStatusNotExpired = "CGOBM_00017";

     public const string CgoTimeLimitExpiryForCorrespondence = "CGOBM_00018";

     /// <summary>
     /// Net amount billed should be equal to amount to be settled for authority to bill
     /// </summary>
     public const string CgoInvalidAmountToBeSettled = "CGOBM_00019";

     public const string InvalidSectorCombination = "CGOBM_00020";

     public const string InvalidFromSectorCode = "CGOBM_00021";

     public const string InvalidToSectorCode = "CGOBM_00022";

     public const string InvalidSequenceNumber = "CGOBM_00023";

     public const string InvalidMaxProrateLadderCount = "CGOBM_00024";

     public const string InvalidConsignemtOriginDestination = "CGOAWB_00001";

     /// <summary>
     /// Net billed amount is invalid
     /// </summary>
     public const string InvalidAwbTotalAmount = "BCGO_10110";

     public const string CgoCreditMemoVatBreakdownRecordNotFound = "CGOCM_00001";

     public const string CgoDuplicateCreditMemoFound = "CGOCM_00002";

     public const string ParentChildErrorInvalidAwbIssuingAirline = "BCGO_10114";

     public const string ParentChildErrorInvalidAwbSerialNumber = "BCGO_10115";

     public const string ParentChildErrorInvalidAwbCheckDigit = "BCGO_10116";

    /// <summary>
    /// Used when Consignment Origin and destination is same.
    /// </summary>
    public const string InvalidOriginDestinationCombination = "BCGO_10101";

    /// <summary>
    /// Used when 'Origin airport code' passed in coupon record is invalid
    /// </summary>
    public const string InvalidConsignmentOriginCode = "BCGO_10102";

    /// <summary>
    /// Used when 'Destination airport code' passed in coupon record is invalid
    /// </summary>
    public const string InvalidConsignmentDestinationCode = "BCGO_10103";

    // SCP107981: to/point of transfer population error
    /// <summary>
    /// Used when Carriage From Id and Carriage To Id is same.
    /// </summary>
    public const string InvalidCarriageCombination = "BCGO_10104";

    ///// <summary>
    ///// Used when 'From Carriage code' passed in Awb record is invalid
    ///// </summary>
    //public const string InvalidFromCarriageCode = "BCGO_10105";

    ///// <summary>
    ///// Used when 'From Carriage code' passed in Awb record is invalid
    ///// </summary>
    //public const string InvalidToCarriageCode = "BCGO_10106";

    /// <summary>
    /// Used when OtherCharge VAT calculated amount difference is not match with sum of Other charge breakdown VAT amount.
    /// </summary>
    public const string InvalidOtherChargeDifferenceAmount = "BCGO_10107";

    public const string AWBbreakdownMandatory = "BCGO_10108";

    public const string ProrateLadderRequired = "BCGO_10109";

    /// <summary>
    /// Code to check duplicate Batch and Record seq no.
    /// </summary>
    public const string DuplicateBatchNoSequenceNo = "BCGO_10111";

    /// <summary>
    /// awb Tax records should be present if TaxAmount > 0.
    /// </summary>
    public const string ZeroCouponVatRecordsForAwbVatAmount = "BCGO_10112";

    /// <summary>
    /// Used when Awb record vat amount is not matching with awb vat amount.
    /// </summary>
    public const string InvalidAwbVatBreakdownAmount = "BCGO_10113";

    /// <summary>
    /// Used when 'Origin' passed in Awb record is invalid
    /// </summary>
    public const string InvalidAwbOrigin = "BCGO_10117";

    /// <summary>
    /// Used when 'Destination' passed in Awb record is invalid
    /// </summary>
    public const string InvalidAwbDestination = "BCGO_10118";

    /// <summary>
    /// Origin and Desination should not be same.
    /// </summary>
    public const string OriginofAwbAndDestinationOfAwbShouldNotBeSame = "BCGO_10119";

    /// <summary>
    /// Origin  of Awb is invalid.
    /// </summary>
    public const string OriginofAwbIsInvalid = "BCGO_10120";

    /// <summary>
    /// Destination  of Awb is invalid.
    /// </summary>
    public const string DestinationofAwbIsInvalid = "BCGO_10121";

    /// <summary>
    /// Used when 'From' passed in Awb record is invalid
    /// </summary>
    public const string InvalidAwbFrom = "BCGO_10122";

    /// <summary>
    /// Used when 'To' passed in Awb record is invalid
    /// </summary>
    public const string InvalidAwbTo = "BCGO_10123";

    /// <summary>
    /// From and To should not be same.
    /// </summary>
    public const string FromofAwbAndToOfAwbShouldNotBeSame = "BCGO_10124";

    /// <summary>
    /// From  of Awb is invalid.
    /// </summary>
    public const string FromofAwbIsInvalid = "BCGO_10125";

    /// <summary>
    /// To  of Awb is invalid.
    /// </summary>
    public const string ToofAwbIsInvalid = "BCGO_10126";

    /// <summary>
    /// Awb serailNumbewr is invalid.
    /// </summary>
    public const string InvalidAwbSerialNo = "BCGO_10127";

    /// <summary>
    /// awb OC records should be present if OCAmount > 0.
    /// </summary>
    public const string ZeroCouponOcRecordsForAwbOcAmount = "BCGO_10128";

    /// <summary>
    /// Used when Awb record OC amount is not matching with awb OC amount.
    /// </summary>
    public const string InvalidAwbOcBreakdownAmount = "BCGO_10129";

    /// <summary>
    /// Used when ProvisoReqSpa is invalid.
    /// </summary>
    public const string InvalidProvisoReqSpa = "BCGO_10130";

    /// <summary>
    /// Used when ProratePer is invalid.
    /// </summary>
    public const string InvalidProratePer = "BCGO_10131";

    /// <summary>
    /// Used when KgLbIndicator is invalid.
    /// </summary>
    public const string InvalidKgLbIndicator = "BCGO_10132";

    /// <summary>
    /// Used when InvalidIscAmount is invalid.
    /// </summary>
    public const string InvalidCcIscAmount = "BCGO_10133";

    /// <summary>
    /// Used when InvalidawbTAmount is invalid.
    /// </summary>
    public const string AwbTAmountIsNotInRangeOfMinMaxAcceptableLimit = "BCGO_10134";


    /// <summary>
    /// Used when Awb Issuing Airline is invalid.
    /// </summary>
    public const string InvalidAwbIssuingAirline = "BCGO_10135";

    /// <summary>
    /// Used to VatCalulatedAmount in Cargo.
    /// </summary>
    public const string InvalidCgoCalculatedVatAmount = "BCGO_10136";

    /// <summary>
    /// Used when vat identifier is invalid.
    /// </summary>
    public const string InvalidCgoVatIdentifier = "BCGO_10137";

    /// <summary>
    /// Vat label is mandatory
    /// </summary>
    public const string InvalidCgoVatLabel = "BCGO_10138";

    /// <summary>
    /// Vat text is mandatory
    /// </summary>
    public const string InvalidCgoVatText = "BCGO_10139";

    /// <summary>
    /// Vat text Length Validation
    /// </summary>
    public const string InvalidCgoVatTextLenght = "BCGO_10354";

    /// <summary>
    /// Used to OCCalulatedAmount in Cargo.
    /// </summary>
    public const string InvalidCgoCalculatedOcAmount = "BCGO_10140";

    /// <summary>
    /// Used when OCCode is invalid.
    /// </summary>
    public const string InvalidCgoOcCode = "BCGO_10141";

    /// <summary>
    /// OC Vat label is mandatory
    /// </summary>
    public const string InvalidCgoOcVatLabel = "BCGO_10142";

    /// <summary>
    /// OC Vat text is mandatory
    /// </summary>
    public const string InvalidCgoOcVatText = "BCGO_10143";

    /// <summary>
    /// AwbCheck digit should be between 0-6 or 9.
    /// </summary>
    public const string InvalidAwbCheckDigit = "BCGO_10144";

    /// <summary>
    /// Used when Currency Adjustment Indicator is invalid.
    /// </summary>
    public const string InvalidCurrencyAdjustmentInd = "BCGO_10145";

    /// <summary>
    /// Used when invalid Billing code entered from UI
    /// </summary>
    public const string InvalidBillingCode = "BCGO_10146";

    /// <summary>
    /// Used when the invoice billing period is invalid.
    /// </summary>
    public const string InvalidBillingPeriod = "BCGO_10147";

    /// <summary>
    /// Used when the invoice type is invalid.
    /// </summary>
    public const string InvalidInvoiceType = "BCGO_10148";

    /// <summary>
    /// Used when the invoice billing period is invalid.
    /// </summary>
    public const string InvalidBillingMonthAndPeriod = "BCGO_10149";


    /// <summary>
    /// Used when the country code is invalid.
    /// </summary>
    public const string InvalidCountryCode = "BCGO_10150";

    /// <summary>
    /// Used when the Invoice Member Location Information is invalid.
    /// </summary>
    public const string InvalidInvoiceMemberLocationInformation = "BCGO_10151";

    /// <summary>
    /// Used when the invoice billing period is invalid.
    /// </summary>
    public const string InvalidBillingCurrency = "BCGO_10152";

    /// <summary>
    /// Used when the digital signature required flag is invalid.
    /// </summary>
    public const string InvalidDigitalSignatureRequiredFlag = "BCGO_10153";

    /// <summary>
    /// Used when the invoice listing currency is invalid.
    /// </summary>
    public const string InvalidListingCurrency = "BCGO_10154";

    /// <summary>
    /// Used when the billing member and billed members are the same in an invoice.
    /// </summary>
    public const string SameBillingAndBilledMember = "BCGO_10155";

    /// <summary>
    /// Used when the settlement method is invalid.
    /// </summary>
    public const string InvalidSettlementMethod = "BCGO_10156";


    /// <summary>
    /// Used when the country code is invalid.
    /// </summary>
    public const string InvalidBillingFromMember = "BCGO_10157";

    /// <summary>
    /// Used when the Invoice Member Location Information is invalid.
    /// </summary>
    public const string InvalidBillingMemberStatus = "BCGO_10158";

    /// <summary>
    /// Used when the invoice billing period is invalid.
    /// </summary>
    public const string InvalidBilledMemberStatus = "BCGO_10159";

    /// <summary>
    /// Used when the digital signature required flag is invalid.
    /// </summary>
    public const string InvalidInvoiceDate = "BCGO_10160";

    /// <summary>
    /// Used when the billing member and billed members are the same in an invoice.
    /// </summary>
    public const string DuplicateBatchNo = "BCGO_10161";

    /// <summary>
    /// Used when the settlement method is invalid.
    /// </summary>
    public const string InvoiceShouldNotHaveCreditNoteTransactions = "BCGO_10162";

    /// <summary>
    /// Used when the country code is invalid.
    /// </summary>
    public const string MemberIsNotMigratedForAWBIsIdec = "BCGO_10163";

    /// <summary>
    /// Used when the Invoice Member Location Information is invalid.
    /// </summary>
    public const string MemberIsNotMigratedForAWBIsXml = "BCGO_10164";

    /// <summary>
    /// Used when the invoice billing period is invalid.
    /// </summary>
    public const string MemberIsNotMigratedForRMIsIdec = "BCGO_10165";

    /// <summary>
    /// Used when the digital signature required flag is invalid.
    /// </summary>
    public const string MemberIsNotMigratedForRMIsXml = "BCGO_10166";

    /// <summary>
    /// Used when the invoice listing currency is invalid.
    /// </summary>
    public const string MemberIsNotMigratedForBMIsIdec = "BCGO_10167";

    /// <summary>
    /// Used when the billing member and billed members are the same in an invoice.
    /// </summary>
    public const string MemberIsNotMigratedForBMIsXml = "BCGO_10168";

    /// <summary>
    /// Used when the settlement method is invalid.
    /// </summary>
    public const string MemberIsNotMigratedForCMIsIdec = "BCGO_10169";

    /// <summary>
    /// Used when the settlement method is invalid.
    /// </summary>
    public const string MemberIsNotMigratedForCMIsXml = "BCGO_10170";

    /// <summary>
    /// Used when Total Vat Amount is invalid.
    /// </summary>
    public const string InvalidTotalVatAmount = "BCGO_10171";

    /// <summary>
    /// VAT Breakdown is not provided when the VAT amount is not zero. 
    /// </summary>
    public const string VatBreakdownRecordsRequired = "BCGO_10172";

    /// <summary>
    /// Used when Total Weight Charges is invalid.
    /// </summary>
    public const string InvalidWeightChargesOfBillingCodeTotalRecord = "BCGO_10173";

    /// <summary>
    /// Used when Total Other Charges is invalid.
    /// </summary>
    public const string InvalidOtherChargesOfBillingCodeTotalRecord = "BCGO_10174";

    /// <summary>
    /// Used when Total ISC Amount is invalid.
    /// </summary>
    public const string InvalidIscAmountOfBillingCodeTotalRecord = "BCGO_10175";

    /// <summary>
    /// Used when BillingCode Subtotal is invalid.
    /// </summary>
    public const string InvalidSubtotalOfBillingCodeTotalRecord = "BCGO_10176";

    /// <summary>
    /// Used when Total Valuation Charges is invalid.
    /// </summary>
    public const string InvalidValuationChargesOfBillingCodeTotalRecord = "BCGO_10177";

    /// <summary>
    /// Used when Total Vat Amount is invalid.
    /// </summary>
    public const string InvalidVatAmoluntOfBillingCodeTotalRecord = "BCGO_10178";

    /// <summary>
    /// Total number of billing records of Billing code total record does not match with total of transaction records of the source code.
    /// </summary>
    public const string InvalidNumberOfBillingRecordsOfBillingCodeTotalRecord = "BCGO_10179";

    /// <summary>
    /// To validate Detail count
    /// </summary>
    public const string InvalidDetailCount = "BCGO_10180";

    /// <summary>
    /// Invalid total number of records for source code total and invoice total
    /// </summary>
    public const string InvalidTotalNumberOfRecords = "BCGO_10181";

    /// <summary>
    /// Invalid total WeightCharge of invoice total
    /// </summary>
    public const string InvalidTotalWeightChargeOfInvoiceTotal = "BCGO_10182";

    /// <summary>
    /// Invalid total OtherCharge of invoice total
    /// </summary>
    public const string InvalidTotalOtherChargeOfInvoiceTotal = "BCGO_10183";

    /// <summary>
    /// Invalid total ISCCharge of invoice total
    /// </summary>
    public const string InvalidTotalIscChargeOfInvoiceTotal = "BCGO_10184";

    /// <summary>
    /// Invalid total NetTotal of invoice total
    /// </summary>
    public const string InvalidNetTotalOfInvoiceTotal = "BCGO_10185";

    /// <summary>
    /// Invalid total NetBillingTotal of invoice total
    /// </summary>
    public const string InvalidNetBillingTotalOfInvoiceTotal = "BCGO_10186";

    /// <summary>
    /// Invalid total TotalNoOfBillingRecordsOfInvoiceTotal of invoice total
    /// </summary>
    public const string InvalidTotalNoOfBillingRecordsOfInvoiceTotal = "BCGO_10187";

    /// <summary>
    /// Invalid total ValuationCharge of invoice total
    /// </summary>
    public const string InvalidTotalValuationChargeOfInvoiceTotal = "BCGO_10188";

    /// <summary>
    /// Invalid total VAT Amount of invoice total
    /// </summary>
    public const string InvalidTotalVatAmountOfInvoiceTotal = "BCGO_10189";

    /// <summary>
    /// Invalid total TotalNoOfRecordsOfInvoiceTotal of invoice total
    /// </summary>
    public const string InvalidTotalNoOfRecordsOfInvoiceTotal = "BCGO_10190";

    /// <summary>
    /// Invalid total NetAmount without VAT Amount of invoice total
    /// </summary>
    public const string InvalidTotalNetAmountWithoutVatOfInvoiceTotal = "BCGO_10191";

    /// <summary>
    /// Mandatory Transaction In Invoice
    /// </summary>
    public const string MandatoryTransactionInInvoice = "BCGO_10192";

    /// <summary>
    /// Used when duplicate billing memo found
    /// </summary>
    public const string DuplicateBillingMemoFound = "BCGO_10193";

    /// <summary>
    /// Used when invalid reason code entered from UI
    /// </summary>
    public const string InvalidReasonCode = "BCGO_10194";

    /// <summary>
    /// Coupon Breakdown Record is mandatory for the reason code.
    /// </summary>
    public const string MandatoryCouponBreakdownRecord = "BCGO_10195";

    /// <summary>
    /// Invalid CorrespondenceRefNumber.
    /// </summary>
    public const string InvalidCorrespondenceRefNumber = "BCGO_10196";

    /// <summary>
    /// Used when, Correspondence Ref. No. mentioned does not exist in the correspondence table as a correspondence between the 2 members between which the BM is being created.
    /// </summary>
    public const string BillingMemoReferenceCorrespondenceDoesNotExist = "BCGO_10197";

    /// <summary>
    /// Net amount billed should be equal to amount to be settled for authority to bill
    /// </summary>
    public const string InvalidAmountToBeSettled = "BCGO_10198";

    /// <summary>
    /// Amount to be settled field should not be Zero / Negative.
    /// </summary>
    public const string CargoInvalidAmountToBeSettled = "CGOCO_00001";

    /// <summary>
    /// Correspondence Does Not Exists in the database.
    /// </summary>
    public const string CorrespondenceDoesNotExistsInTheDatabase = "BCGO_10199";

    /// <summary>
    /// Used when there is sequence error for the serial number for RM/BM/CM Coupon Breakdown records.
    /// </summary>
    public const string InvalidBdSerialNumberSequence = "BCGO_10200";

    /// <summary>
    /// Ibavalid Expiry TimeLimit for Correspondence.
    /// </summary>
    public const string TimeLimitExpiryForCorrespondence = "BCGO_10201";

    /// <summary>
    /// If Vat amount > 0 then tax breakdowns should be > 0 
    /// </summary>
    public const string ZeroVatBreakdownRecords = "BCGO_10202";

    /// <summary>
    /// Vat amount does not match with totals of vat breakdowns
    /// </summary>
    public const string InvalidTotalVatBreakdownAmounts = "BCGO_10203";

    /// <summary>
    /// Net billed amount is invalid
    /// </summary>
    public const string InvalidNetBilledAmount = "BCGO_10204";

    /// <summary>
    /// Used when Total Weight Charge is invalid.
    /// </summary>
    public const string InvalidTotalWeightCharge = "BCGO_10205";

    /// <summary>
    /// Used when Total Valuation Charge is invalid.
    /// </summary>
    public const string InvalidTotalValuationCharge = "BCGO_10206";

    /// <summary>
    /// Used when Total Other Charge is invalid.
    /// </summary>
    public const string InvalidTotalOtherCharge = "BCGO_10207";

    /// <summary>
    /// Used when Total ISC Amount is invalid.
    /// </summary>
    public const string InvalidTotalIscAmount = "BCGO_10208";

    /// <summary>
    /// Used when Total Net Billed Amount is invalid.
    /// </summary>
    public const string InvalidTotalNetBilledAmount = "BCGO_10209";

    /// <summary>
    /// Billing memo net total amount should not be 0 or negative.
    /// </summary>
    public const string BillingMemoNetTotalAmountShouldNotBeNegative = "BCGO_10210";

    /// <summary>
    /// Used when coupon record has invalid Awb date.
    /// </summary>
    public const string InvalidAwbDate = "BCGO_10211";

    /// <summary>
    /// Used when coupon record has date of Carriage.
    /// </summary>
    public const string InvalidDateOfCarriage = "BCGO_10212";


    //Amount difference should match with Billed - Accepted

    public const string WeightChargeDifferenceShouldMatchWithBilledMinusAccepted = "BCGO_10213";

    public const string ValuationChargeDifferenceShouldMatchWithBilledMinusAccepted = "BCGO_10214";

    public const string OtherChargeDifferenceShouldMatchWithBilledMinusAccepted = "BCGO_10215";

    public const string IscAmountDifferenceShouldMatchWithBilledMinusAccepted = "BCGO_10216";

    public const string VatAmountDifferenceShouldMatchWithBilledMinusAccepted = "BCGO_10217";

    //Amount difference should match with Accepted - Billed

    public const string WeightChargeDifferenceShouldMatchWithAcceptedMinusBilled = "BCGO_10218";

    public const string ValuationChargeDifferenceShouldMatchWithAcceptedMinusBilled = "BCGO_10219";

    public const string OtherChargeDifferenceShouldMatchWithAcceptedMinusBilled = "BCGO_10220";

    public const string IscAmountDifferenceShouldMatchWithAcceptedMinusBilled = "BCGO_10221";

    public const string VatAmountDifferenceShouldMatchWithAcceptedMinusBilled = "BCGO_10222";

    /// <summary>
    /// Used when PartShipMentIndicator is invalid.
    /// </summary>
    public const string InvalidPartShipMentIndicator = "BCGO_10223";

    //Total amount not matching with breakdowns

    public const string WeightChargeBilledIsNotMatchingWithSumOfBreakdowns = "BCGO_10224";

    public const string WeightChargeAcceptedIsNotMatchingWithSumOfBreakdowns = "BCGO_10225";

    public const string WeightChargeDifferenceIsNotMatchingWithSumOfBreakdowns = "BCGO_10226";

    public const string ValuationChargeBilledIsNotMatchingWithSumOfBreakdowns = "BCGO_10227";

    public const string ValuationChargeAcceptedIsNotMatchingWithSumOfBreakdowns = "BCGO_10228";

    public const string ValuationChargeDifferenceIsNotMatchingWithSumOfBreakdowns = "BCGO_10229";

    public const string VatAmountBilledIsNotMatchingWithSumOfBreakdowns = "BCGO_10230";

    public const string VatAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BCGO_10231";

    public const string VatAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BCGO_10232";

    public const string OtherChargeBilledIsNotMatchingWithSumOfBreakdowns = "BCGO_10233";

    public const string OtherChargeAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BCGO_10234";

    public const string OtherChargeAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BCGO_10235";

    public const string IscAmountBilledIsNotMatchingWithSumOfBreakdowns = "BCGO_10236";

    public const string IscAmountAcceptedIsNotMatchingWithSumOfBreakdowns = "BCGO_10237";

    public const string IscAmountDifferenceIsNotMatchingWithSumOfBreakdowns = "BCGO_10238";

    public const string TotalNetRejectAmountIsNotMatchingWithSumOfBreakdowns = "BCGO_10239";

    /// <summary>
    /// At lease one amount should be present in awbrecord,
    /// </summary>
    public const string AllAmountFieldsZeroInAwbRecord = "BCGO_10240";

    /// <summary>
    /// TotalAmount InAwb CanNot Be Negative
    /// </summary>
    public const string TotalAmountInAwbCanNotBeNegative = "BCGO_10241";

     /// <summary>
    /// TotalAmount In Awb is Invalid
    /// </summary>
    public const string InvalidtotalAmountInAwb = "BCGO_10242";

    /// <summary>
    /// FromSector In Pladder is Invalid
    /// </summary>
    public const string InvalidFromSector = "BCGO_10243";

    /// <summary>
    /// ToSector In Pladder is Invalid
    /// </summary>
    public const string InvalidToSector = "BCGO_10244";

    /// <summary>
    /// FromSector and ToSector In Pladder should not be same.
    /// </summary>
    public const string FromSectorAndToSectorofPLadderShouldNotBeSame = "BCGO_10245";

    /// <summary>
    /// CarrierPrefix In Pladder is Invalid
    /// </summary>
    public const string InvalidCarrierPrefix = "BCGO_10246";

    /// <summary>
    /// Total net reject amount should not be 0 or negative.
    /// </summary>
    public const string RejectionMemoNetTotalAmountShouldNotBeNegative = "BCGO_10247";

    /// <summary>
    /// Currency of Prorate Calulation is Invalid.
    /// </summary>
    public const string InvalidCurrencyProrateCalculation = "BCGO_10248";

    /// <summary>
    /// Invalid your invoice billing period
    /// </summary>
    public const string InvalidYourInvoiceBillingPeriod = "BCGO_10249";

    /// <summary>
    /// Should be a unique number within each Billed Airline in the Billing period.
    /// </summary>
    public const string DuplicateRejectionMemoNumber = "BCGO_10250";

    /// <summary>
    /// Net credited amount not in range of allowed min max
    /// </summary>
    public const string NetCreditedAmountNotInRangeOfMinMax = "BCGO_10251";

    /// <summary>
    /// Your invoice number doesn't exists in database.
    /// </summary>
    public const string YourInvoiceNotExists = "BCGO_10252";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Your billing Memo Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForYourBillingMemoNumber = "BCGO_10253";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Your Credit Memo Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForYourCreditMemoNumber = "BCGO_10254";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Your Rejection Memo Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForYourRejectionMemoNumber = "BCGO_10255";

    /// <summary>
    /// Rejection stage of current rejection should not be same as your rejection record
    /// </summary>
    public const string InvalidYourInvoiceRejectionStage = "BCGO_10256";

    /// <summary>
    /// Your billing memo number is blank though BMCMIndicator is SET to BM
    /// </summary>
    public const string InvalidYourBillingMemoNumber = "BCGO_10257";

    /// <summary>
    /// Your billing memo number is blank though BMCMIndicator is SET to BM
    /// </summary>
    public const string InvalidYourCreditMemoNumber = "BCGO_10258";

    /// <summary>
    /// Credit memo coupon net total amount should be negative.
    /// </summary>
    public const string CreditMemoCouponNetTotalAmountShouldBeNegative = "BCGO_10259";

    /// <summary>
    /// Used when Issuing Airline is invalid.
    /// </summary>
    public const string InvalidIssuingAirline = "BCGO_10260";

    /// <summary>
    /// Used when ISC Allowed Amount is invalid for charge collect.
    /// </summary>
    public const string InvalidIscAllowedAmountForChargeCollect = "BCGO_10261";

    /// <summary>
    /// Used when ISC Accepted Amount is invalid for charge collect.
    /// </summary>
    public const string InvalidIscAcceptedAmountForChargeCollect = "BCGO_10262";

    /// <summary>
    /// Used when ISC Allowed Amount is invalid for prepaid.
    /// </summary>
    public const string InvalidIscAllowedAmountForPrepaid = "BCGO_10263";

    /// <summary>
    /// Used when ISC Accepted Amount is invalid for prepaid.
    /// </summary>
    public const string InvalidIscAcceptedAmountForPrepaid = "BCGO_10264";

    /// <summary>
    /// Used when there is sequence error for the serial number for RM/BM/CM Coupon Breakdown records.
    /// </summary>
    public const string InvalidSerialNumberSequence = "BCGO_10265";

    /// <summary>
    /// The "Net Reject Amount" should not exceed $100,000
    /// </summary>
    public const string InvalidRMAwbNetRejectAmount = "BCGO_10266";

    /// <summary>
    /// Rejection memo AWB net total amount should not be 0 or negative.
    /// </summary>
    public const string RMAwbNetRejectAmountShouldNotBeNegative = "BCGO_10267";

    /// <summary>
    /// Audit trail fail for coupon - Ticket Issuing Airline + Serial Number
    /// </summary>
    public const string AuditTrailFailForCouponNumber = "BCGO_10268";

    /// <summary>
    /// Linked AWB does not exists as rejection is not found
    /// </summary>
    public const string LinkedAwbDoesNotExistsAsRejectionIsNoFound = "BCGO_10269";

    /// <summary>
    /// Used when InvalidIscAmount is invalid.
    /// </summary>
    public const string InvalidPIscAmount = "BCGO_10270";

    /// <summary>
    /// Invalid billing period, valid for late submission.
    /// </summary>
    public const string InvoiceValidForLateSubmission = "BCGO_10271";

    /// <summary>
    /// Invalid Memo number for Parent child.
    /// </summary>
    public const string ParentChildErrorInvalidMemoNumber = "BCGO_10272";

    /// <summary>
    /// Invalid Billing code for Parent child.
    /// </summary>
    public const string ParentChildErrorInvalidBillingCode = "BCGO_10273";

    /// <summary>
    /// For invoice type as IV, CMs are not allowed.	
    /// </summary>
    public const string CreditNotShouldNotHaveOtherTransactions = "BCGO_10274";

    /// <summary>
    /// Your invoice number should not be blank.
    /// </summary>
    public const string YourInvoiceNumberShouldNotBeBlank = "BCGO_10275";

    /// <summary>
    /// Your invoice billing month should not be blank.
    /// </summary>
    public const string YourInvoiceBillingMonthShouldNotBeBlank = "BCGO_10276";

    /// <summary>
    /// Your invoice billing month should not be blank.
    /// </summary>
    public const string YourRejectionMemoNumberShouldNotBeBlank = "BCGO_10277";

    /// <summary>
    /// Net reject amount is not in the range of allowed min max amount.
    /// </summary>
    public const string NetRejectAmountIsNotInAllowedRange = "BCGO_10278";

    /// <summary>
    /// Sum of Total Weight Charges Difference and Total Valuation Charges Difference amount is not in the range of allowed min max amount.
    /// </summary>
    public const string SumTotalWtChargeandValChargeDiffInAllowedRange = "BCGO_10294";

    /// <summary>
    /// Invalid your invoice billing date
    /// </summary>
    public const string InvalidYourInvoiceBillingDate = "BCGO_10279";

    /// <summary>
    /// Your invoice number and your invoice date should be blank or both fields should be captured.
    /// </summary>
    public const string MandatoryYourInvoiceNumberAndYourBillingDate = "BCGO_10280";

    /// <summary>
    /// Reason breakdown should not be all blanks.
    /// </summary>
    public const string MandatoryReasonBreakdown = "BCGO_10281";

    /// <summary>
    /// Invalid reason remark serial number.
    /// </summary>
    public const string InvalidReasonRemarkSerialNumber = "BCGO_10282";

    /// <summary>
    /// Used when Your Invoice Billing Data is invalid.
    /// </summary>
    public const string InvoiceBillingDateIsInvalid = "BCGO_10283";

    /// <summary>
    /// Used when the billing member is invalid.
    /// </summary>
    public const string InvalidBillingMember = "BCGO_10284";

    /// <summary>
    /// Used when the billed member is invalid.
    /// </summary>
    public const string InvalidBilledMember = "BCGO_10285";

    /// <summary>
    /// Invalid BillingAirlinecode in Parent child record.
    /// </summary>
    public const string ParentChildErrorInvalidBillingAirlineCode = "BCGO_10286";

    /// <summary>
    /// Invalid BilledAirlinecode in Parent child record.
    /// </summary>
    public const string ParentChildErrorInvalidBilledAirlineCode = "BCGO_10287";

    /// <summary>
    /// Invalid InvoiceNumber in Parent child record.
    /// </summary>
    public const string ParentChildErrorInvalidInvoiceNumber = "BCGO_10288";

    //FIMBMCMIndicator should be populated in case FIM Number/Billing Memo/Credit Memo Number is populated.
    public const string InvalidFimbmcmIndicatorForValidFimBmCmNumber = "BCGO_10289";

    //FIM Number/Billing Memo/Credit Memo Number should be populated in case FIMBMCMIndicator is populated.
    public const string InvalidFimBmCmNumberForValidFimbmcmIndicator = "BCGO_10290";

    /// <summary>
    /// Used when there is VAT and VAT Breakdown Total Amount mismatch.
    /// </summary>
    public const string VatTotalAmountMismatch = "BCGO_10291";

    /// <summary>
    /// Total line item amount of invoice summary does not match with sum of Total Weight Charge and Valuation Charge of all Cargo LineItems
    /// </summary>
    public const string InvalidTotalLineItemAmount = "BCGO_10292";

    /// <summary>
    /// OC Vat amount is mandatory
    /// </summary>
    public const string InvalidCgoOcVatAmount = "BCGO_10293";

    /// <summary>
    /// TotalAmount InAwb CanNot Be Negative
    /// </summary>
    public const string TotalAmountInAwbCanNotBePositive = "BCGO_10294";

    /// <summary>
    /// Net credited amount should not be equal to zero
    /// </summary>
    public const string NetCreditedAmountCannotBeZero = "BCGO_10295";

    /// <summary>
    /// ProratePer should be 00 if PROV/REQ /SPA applicable
    /// </summary>
    public const string InvalidProratePer2 = "BCGO_10296";

    /// <summary>
    /// Either none or both Billed Weight and KG/LB Indicator should have values.
    /// </summary>
    public const string InvalidBilledWeightKGLBIndicator = "BCGO_10297";

    /// <summary>
    /// Net reject amount is invalid
    /// </summary>
    public const string InvalidNetRejectAmount = "BCGONS_10342";

    public const string InvalidBatchSequenceNo = "BCGONS_10814";

    /// <summary>
    ///SCP85837: Duplicate Batch No
    /// </summary>
    public const string InvalidBatchNo = "BCGONS_10815";

    public const string ErrorInvoiceNotFound = "BCGONS_10762";

    /// <summary>
    /// Used when tax breakdown code record is invalid in coupon breakdown.
    /// </summary>
    public const string InvalidYourInvoiceNumber = "BCGONS_10152";

    /// <summary>
    /// Used when Your Rejection Number is empty for rejection stage 2 or 3 and billing code is 0, or Billing code is 3
    /// </summary>
    public const string InvalidYourRejectionNumber = "BCGONS_10130";

    /// <summary>
    /// Used when, Combination of fields Your Invoice Number’, ‘Your Billing Year’, 
    /// ‘Your Billing Month’ and ‘Your Billing Period’ passed in billing memo/credit memo object does not match with any other invoice in invoice table
    /// </summary>
    public const string LinkedInvoiceNotFound = "BCGONS_10140";

    /// <summary>
    /// Duplicate file name check for attachment file names
    /// </summary>
    public const string DuplicateFileName = "BCGONS_10328";

    /// <summary>
    /// Validation of the Billed Member will fail if the IS Membership Status of the Billed Member is ‘Terminated’.This will be a non-correctable error.
    /// </summary>
    public const string InvalidBilledIsMembershipStatus = "BCGONS_10710";

    /// <summary>
    /// A member cannot create an invoice/creditNote or Form C when his IS Membership is ‘Basic’, ‘Restricted’ or ‘Terminated’.
    /// </summary>
    public const string InvalidBillingIsMembershipStatus = "BCGONS_10709";

    public const string InvoiceLateSubmitted = "BCGO_10298";

    /// <summary>
    /// Used when invoice status is not valid for updating invoice
    /// </summary>
    public const string InvalidInvoiceStatusForUpdate = "BCGONS_10135";

    /// <summary>
    /// Used when same invoice number found invoice repository for given calendar year and billing member.
    /// </summary>
    public const string DuplicateInvoiceFound = "BGEN_00002";

    /// <summary>
    /// Used when Member reference data missing for billing and billed member.
    /// </summary>
    public const string InvalidMemberLocationInformation = "BCGONS_10138";

    public const string VoidPeriodValidationMsg = "BCGONS_10824";

    public const string InvalidInvoiceTotalVat = "BCGONS_10817";

    /// <summary>
    /// Used when single transaction line record not available for Prime Billing, Rejection Memo and Billing Memo
    /// </summary>
    public const string TransactionLineItemNotAvailable = "BCGONS_10114";

    public const string ErrorNegativeRMNetAmount = "BCGONS_10772";

    /// <summary>
    /// Used when 'From airport code' passed in coupon record is invalid
    /// </summary>
    public const string InvalidFromAirportCode = "BCGONS_10110";

    /// <summary>
    /// Used when 'To airport code' passed in coupon record is invalid
    /// </summary>
    public const string InvalidToAirportCode = "BCGONS_10111";

    /// <summary>
    /// Used when invalid reason code entered from UI
    /// </summary>
    public const string InvalidAirportCombination = "BCGONS_10150";

    /// <summary>
    /// Used when tax breakdown code record is invalid in coupon breakdown.
    /// </summary>
    public const string InvalidYourBillingPeriod = "BCGONS_10812";

    public const string AuthorityToBillNotSetForCorrespondence = "BCGONS_10715";

    //Special characters are not allowed in Invoice Number
    public const string SpecialCharactersAreNotAllowedInInvoiceNumber = "BGEN_00001";

    /// <summary>
    /// Listing to Billing Rate is not equal to 1 when Currency of Listing and Currency of Billing are the same.
    /// </summary>
    public const string InvalidListingToBillingRateForSameCurrencies = "BGEN_00004";

    public const string InvalidBillingMemberLocation = "BGEN_00011";

    public const string InvalidBilledMemberLocation = "BGEN_00012";

    public const string InvalidBillingToMember = "BGEN_00003";

    /// <summary>
    /// When Billed Member is not allowed to accept bill from  the blocked Billing Member from group.
    /// </summary>
    public const string InvalidBillingFromMemberGroup = "BGEN_00009";

    /// <summary>
    /// When Billing Memebr is not allowed to bill the blocked Billed Member from group.
    /// </summary>
    public const string InvalidBillingToMemberGroup = "BGEN_00010";

    /// <summary>
    /// Validation if organization designator is invalid
    /// </summary>
    public const string InvalidOrganizationDesignator = "BGEN_00008";

    public const string InvalidListingToBillingRate = "BGEN_00005";


    // For Correspondence
    public const string EnterEmailIds = "BCGONS_10716";

    public const string InvalidCorrespondenceSubject = "BCGONS_10717";

    public const string InvalidCorrespondenceNumber = "BCGONS_10718";

    public const string ExpiredCorrespondence = "BCGONS_10719";

    public const string InvalidAuthorityToBill = "BCGONS_10720";

    public const string InvalidEmailIds = "BCGONS_10721";

    public const string FailedToSendMail = "BCGONS_10722";
    
    public const string InvalidLineItemDescription = "BCGO_10301";

    public const string InvalidExchangeRate = "BCGO_10302";
    
    /// <summary>
    /// Used as error code for RM level amount validation done from Stored procedure PROC_CGO_VALIDATE_MEMO.
    /// </summary>
    public const string AmountOutsideLimit = "BCGO_10303";

    /// <summary>
    /// Used when InvalidawbTAmount is invalid.
    /// </summary>
    public const string AwbTAmountDoesNotMatchWithSumOfAllAmounts = "BCGO_10304";

    /// <summary>
    /// Net billed amount is invalid
    /// </summary>
    public const string NetBilledAmountDoesNotMatchWithSumOfAllAmounts = "BCGO_10305";

    /// <summary>
    /// Net credited amount is invalid
    /// </summary>
    public const string NetCreditedAmountDoesNotMatchWithSumOfAllAmounts = "BCGO_10306";

    /// <summary>
    /// Net credited amount is invalid
    /// </summary>
    public const string InvalidNetCreditedAmount = "BCGO_10307";

    public const string InvalidLanguage = "BCGO_10308";
   
    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and  Rejection Memo's Bm Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForBmNumber = "BCGO_10309";

    /// <summary>
    /// The combination of Your Invoice No, Your Invoice Date and Rejection Memo's  Cm Number does not exists in the IS database.
    /// </summary>
    public const string AuditTrailFailForCmNumber = "BCGO_10310";

    /// <summary>
    /// Memo level VAT breakdown should not be provided when RM/BM/CM has AWB breakdown information.
    /// </summary>
    public const string VatPresentWhenAWBBreakdownExists = "BCGO_10311";
    
    public const string BatchRecordSequenceNoReq = "BCGO_10312";

    public const string AwbSrNoAndCheckDigitRequired = "BCGO_10313";

    // CMP#459 : Error Messages for Scenario 9
    public const string BilledWeightChargesDoesnotmatchwithRejectedAwb = "BCGO_10353";
    public const string BilledValuationChargesDoesnotmatchwithRejectedAwb = "BCGO_10314";
    public const string BilledOtherChargesDoesnotmatchwithRejectedAwb = "BCGO_10315";
    public const string AllowedIscAmountDoesnotmatchwithRejectedAwb = "BCGO_10316";
    public const string BilledVatAmountDoesnotmatchwithRejectedAwb = "BCGO_10317";

    // CMP#459 : Error Messages for Scenario 10 / 11
    public const string TotalWeightChargesOfRmDoesnotmatch = "BCGO_10318";
    public const string TotalValuationAmountOfRmDoesnotmatch = "BCGO_10319";
    public const string TotalOtherChargeAmountOfRmDoesnotmatch = "BCGO_10320";
    public const string TotalIscAmountOfRmDoesnotmatch = "BCGO_10321";
    public const string TotalVatAmountOfRmDoesnotmatch = "BCGO_10322";

    // CMP#459 : Error Messages for Scenario 12 / 13
    public const string TotalWeightChargesAcceptedOfRmDoesnotmatch = "BCGO_10323";
    public const string TotalValuationAmountAcceptedOfRmDoesnotmatch = "BCGO_10324";
    public const string TotalOtherChargeAmountAcceptedOfRmDoesnotmatch = "BCGO_10325";
    public const string TotalIscAmountAcceptedOfRmDoesnotmatch = "BCGO_10326";
    public const string TotalVatAmountAcceptedOfRmDoesnotmatch = "BCGO_10327";
    
    // CMP#459 : Error Messages for ISWEB Scenario 10 / 11
    public const string IsWebTotalWeightChargesOfRmDoesnotmatch = "BCGO_10328";
    public const string IsWebTotalValuationAmountOfRmDoesnotmatch = "BCGO_10329";
    public const string IsWebTotalOtherChargeAmountOfRmDoesnotmatch = "BCGO_10330";
    public const string IsWebTotalIscAmountOfRmDoesnotmatch = "BCGO_10331";
    public const string IsWebTotalVatAmountOfRmDoesnotmatch = "BCGO_10332";

    // CMP#459 : Error Messages for ISWEB Scenario 12 / 13
    public const string IsWebTotalWeightChargesAcceptedOfRmDoesnotmatch = "BCGO_10333";
    public const string IsWebTotalValuationAmountAcceptedOfRmDoesnotmatch = "BCGO_10334";
    public const string IsWebTotalOtherChargeAmountAcceptedOfRmDoesnotmatch = "BCGO_10335";
    public const string IsWebTotalIscAmountAcceptedOfRmDoesnotmatch = "BCGO_10336";
    public const string IsWebTotalVatAmountAcceptedOfRmDoesnotmatch = "BCGO_10337";

    // CMP#459 : Error Messages for ISWEB Error Correction Scenario 9
    public const string ErrCorrBilledWeightChargesDoesnotmatchwithRejectedAwb = "BCGO_10338";
    public const string ErrCorrBilledValuationChargesDoesnotmatchwithRejectedAwb = "BCGO_10339";
    public const string ErrCorrBilledOtherChargesDoesnotmatchwithRejectedAwb = "BCGO_10340";
    public const string ErrCorrAllowedIscAmountDoesnotmatchwithRejectedAwb = "BCGO_10341";
    public const string ErrCorrBilledVatAmountDoesnotmatchwithRejectedAwb = "BCGO_10342";

    // CMP#459 : Error Messages for ISWEB Error Correction Scenario 10 / 11
    public const string ErrCorrTotalWeightChargesOfRmDoesnotmatch = "BCGO_10343";
    public const string ErrCorrTotalValuationAmountOfRmDoesnotmatch = "BCGO_10344";
    public const string ErrCorrTotalOtherChargeAmountOfRmDoesnotmatch = "BCGO_10345";
    public const string ErrCorrTotalIscAmountOfRmDoesnotmatch = "BCGO_10346";
    public const string ErrCorrTotalVatAmountOfRmDoesnotmatch = "BCGO_10347";

    // CMP#459 : Error Messages for ISWEB Error Correction Scenario 12 / 13
    public const string ErrCorrTotalWeightChargesAcceptedOfRmDoesnotmatch = "BCGO_10348";
    public const string ErrCorrTotalValuationAmountAcceptedOfRmDoesnotmatch = "BCGO_10349";
    public const string ErrCorrTotalOtherChargeAmountAcceptedOfRmDoesnotmatch = "BCGO_10350";
    public const string ErrCorrTotalIscAmountAcceptedOfRmDoesnotmatch = "BCGO_10351";
    public const string ErrCorrTotalVatAmountAcceptedOfRmDoesnotmatch = "BCGO_10352";

    /// <summary>
    /// Your Rejection Memo No. cannot be provided for this rejection stage.
    /// </summary>
    public const string YourRejectionNumberNotRequired = "BCGO_10355";
    
    //CMP#459
    public const string InvalidAllowedIscAmount = "BCGO_10356";
    public const string InvalidAcceptedIscAmount = "BCGO_10357";

    //SCP#120091 
    public const string InvalidCgoOcCodeDueToCharLength = "BCGO_10358";


    public const string BillingCodeSubTotalNotFound = "BCGO_10359";

    public const string BillingCodeSubTotalfoundButNoTrans = "BCGO_10360";

    /// <summary>
    /// Used when Rejection Stage is invalid.
    /// </summary>
    public const string InvalidRejectionStage = "BCGO_10379";

    /// <summary>
    /// CMP #613: Validation for Cargo Other Charges Breakdown 
    /// </summary>
    public const string InvalidOtherChargeBreakdownAmount = "BCGO_10381";

    #region CMP #624: ICH Rewrite-New SMI X.

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.14,  New Validation #9 point 6
    /// </summary>
    public const string RejInvBHLinkingCheckOldInvSmiX = "BCGO_10382";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.14,  New Validation #9 point 7
    /// </summary>
    public const string RejInvBHLinkingCheckOldInvSmiNotX = "BCGO_10383";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.14,  New Validation #9 point 6
    /// </summary>
    public const string StandaloneBmInvoiceLinkingCheckForSmiX = "BCGO_10384";

    /// <summary>
    ///CMP #624: ICH Rewrite-New SMI X. FRS Reference: 2.14,  New Validation #9 point 7
    /// </summary>
    public const string StandaloneBmInvLinkCheckForSmiX = "BCGO_10385";

    #endregion

    // CMP#459 : Error Messages for Scenario 9 (IS WEB)
    public const string IsWebBilledWeightChargesMismatchRejectedAwb = "BCGO_10386";
    public const string IsWebBilledValuationChargesMismatchRejectedAwb = "BCGO_10387";
    public const string IsWebBilledOtherChargesMismatchRejectedAwb = "BCGO_10388";
    public const string IsWebAllowedIscAmountMismatchRejectedAwb = "BCGO_10389";
    public const string IsWebBilledVatAmountMismatchRejectedAwb = "BCGO_10390";

      // CMP#650 Error codes for reason code validation

    public const string InvalidRMReasonCodeForBM = "BCGO_10391";
    public const string InvalidRMReasonCodeForStage1 = "BCGO_10392";
    public const string InvalidRMReasonCodeForRejectedRM = "BCGO_10393";
    public const string InvalidRMReasonCodeForStage2And3 = "BCGO_10394";

    /* Name: BPAXNS_10961	
       Value: Invalid Member Type	
       Comment: CMP #596: Length of Member Accounting Code to be Increased to 12. 3.4 Point 21 - New validation #MW2 
    */
    public const string InvalidMemberType = "BCGO_10395";

    /* Name: BCGO_10396	
     Value: Restrictions exist on your type of Accounting Code to create Invoice/Credit Notes in this Billing Category
     Comment: CMP #596: Length of Member Accounting Code to be Increased to 12. 3.4 Point 22, 27 - New validation #MW3 
    */
    public const string InvalidBillingMemberType = "BCGO_10396";

    /// <summary>
    ///CMP #553: ACH Requirement for Multiple Currency Handling Validation #2
    /// </summary>
    public const string InvalidOrInactiveCurrencyOfBilling = "BCGO_10398";

    /// <summary>
    ///CMP #553: ACH Requirement for Multiple Currency Handling Validation #3
    /// </summary>
    public const string CurrencyOfListingShouldBeSameAsCurrencyOfBilling = "BCGO_10399";


    #region CMP#674: Validation of Coupon and AWB Breakdowns in Rejections

    //Error in RM No. {0}, Batch No. {1}, Seq. No. {2} due to mismatch in AWB {3}-{4}. It was billed {5} time(s) in the rejected RM; and {6} time(s) in this RM.
    public const string CargoRMCouponMismatchIsWeb = "BCGO_10401";

    //Mismatch in AWB {0}-{1}. It was billed {2} time(s) in the rejected RM; and {3} time(s) in this RM. Other mismatches if any are not included in this report.
    public const string CargoRMCouponMismatchFileValidation = "BCGO_10402";

    #endregion

    // CMP#673: Validation on Correspondence Reference Number in PAX/CGO Billing Memos
    public const string CorrRefNumberCannotBeProvidedForNon6Aor6Bbm = "CGOBM_00025";
    public const string CorrRefNumberCannotBeProvidedForNon6Aor6BbmReasonCode = "CGOBM_00026";

    //SCP471024 : Failed Legal XML / PDF
    /// <summary>
    /// Invalid Digital Signature Value
    /// </summary>
    public const string InvalidDigitalSignatureValue = "BCGO_10403";
  }
}

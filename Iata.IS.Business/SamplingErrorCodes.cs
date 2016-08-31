namespace Iata.IS.Business
{
  public struct SamplingErrorCodes
  {
    /// <summary>
    /// Used when Form C created by the same or any other owner exists in an ‘Open’, 
    /// ‘Error’ or ‘Ready for Submission’ status; for the same combination of ‘Provisional Billing Month,
    /// ‘Provisional Billing Airline’ and Currency of Listing/Evaluation’. 
    /// The user is expected to append additional items to the existing Form C that hasn't been submitted yet.
    /// </summary>
    public const string InvalidSamplingFormC = "BPAXS_10101";

    /// <summary>
    /// If the migration status of the provisional billing airline indicates that sampling provisional billings
    /// were migrated on/before the provisional billing period indicated in the Form C header, then the ‘Currency of Listing/Evaluation’ 
    /// defined in the Form C header should match at the ‘Currency of Listing/Evaluation’ of at least one of the provisional invoices.
    /// </summary>
    public const string InvalidProvisionalListingCurrency = "BPAXS_10102";

    /// <summary>
    /// InvalidProvisionalListingCurrency is by passed in sandbox, this will not be by passed
    /// </summary>
    public const string InvalidProvListingCurrency = "BPAXS_10260";

    /// <summary>
    /// At least one transaction/line item should be present.
    /// </summary>
    public const string TransactionLineItemNotAvailable = "BPAXS_10103";

    /// <summary>
    /// Dupe is defined as a combination of Ticket Issuing Airline, Ticket/Document Number, Coupon No, Prov. Inv. No. Batch No., Prov. Inv. Seq. No. Prov. Inv.
    /// </summary>
    public const string DuplicateSamplingFormCRecordFound = "BPAXS_10104";

    /// <summary>
    /// Used when user try to add non-nil form c for provisional billing member for which nil form c already added in same billing month
    /// </summary>
    public const string SamplingFormCExistWithNilIndicator = "BPAXS_10105";

    /// <summary>
    /// Used when user try to add nil form c for provisional billing member for which non nil form c already added in same billing month
    /// </summary>
    public const string NonNilFormCExistsForSamplingFormC = "BPAXS_10106";

    /// <summary>
    /// Used when user tries to Validate Form D/E
    /// </summary>
    public const string FormDeTransactionNotFound = "BPAXS_10107";

    /// <summary>
    /// Used when user tries to add/update invoice with same invoice number.
    /// </summary>
    public const string DuplicateFormDeInvoiceFound = "BPAXS_10108";

    /// <summary>
    /// Used when user tries to add/update Form D Record with the record existing in database.
    /// </summary>
    public const string DuplicateFormDRecordFound = "BPAXS_10109";

    /// <summary>
    /// Used when Provisional Invoice Number is invalid (Reference Check).
    /// </summary>
    public const string InvalidProvisionalInvoiceNumber = "BPAXS_10110";

    /// <summary>
    /// Used when Provisional Invoice BatchNo is invalid (Reference Check).
    /// </summary>
    public const string InvalidProvisionalInvoiceBatchNo = "BPAXS_10111";

    /// <summary>
    /// Used when Provisional Invoice Batch Seq No is invalid (Reference Check).
    /// </summary>
    public const string InvalidProvisionalInvoiceBatchSeqNo = "BPAXS_10112";

    /// <summary>
    ///  Used when current airline is non migrated.
    /// </summary>
    public const string  NonMigratedAirline = "BPAXS_10114";

    /// <summary>
    /// Used when, user try to add Form C record in a Nil Form C.
    /// </summary>
    public const string FormCHeaderWithNilIndicator = "BPAXS_10114";

    /// <summary>
    /// Used when, transactions can not be added when invoice is in submitted status.
    /// </summary>
    public const string InvalidOperationOnSubmitStatus = "BPAXS_10115";

 
    /// <summary>
    /// Used when ListingToBillingRate for given combination of billing currency, Listing currency 
    /// and Prov billing month is inconsistent with previously captured ListingToBillingRate for same combination
    /// </summary>
    public const string ErrorInconsistentListingToBillingRate = "BPAXS_10116";


    /// <summary>
    /// Used when Provisional Gross Alf Amount is invalid.
    /// </summary>
    public const string InvalidProvisionalGrossAlfAmount = "BPAXS_10117";

    /// <summary>
    /// Used when Evaluated Gross Amount is invalid.
    /// </summary>
    public const string InvalidEvaluatedGrossAmount = "BPAXS_10118";

    /// <summary>
    /// Used when IscPercent is invalid.
    /// </summary>
    public const string InvalidIscPercent = "BPAXS_10119";

    /// <summary>
    /// Used when Isc Amount is invalid.
    /// </summary>
    public const string InvalidIscAmount = "BPAXS_10120";

    /// <summary>
    /// Used when Other Commission Percent is invalid.
    /// </summary>
    public const string InvalidOtherCommissionPercent = "BPAXS_10121";

    /// <summary>
    /// Used when Other Commission Amount is invalid.
    /// </summary>
    public const string InvalidOtherCommissionAmount = "BPAXS_10122";

    /// <summary>
    /// Used when Uatp Percent is invalid.
    /// </summary>
    public const string InvalidUatpPercent = "BPAXS_10123";

    /// <summary>
    /// Used when Uatp Amount is invalid.
    /// </summary>
    public const string InvalidUatpAmount = "BPAXS_10124";

    /// <summary>
    /// Used when Billing Amount is invalid.
    /// </summary>
    public const string InvalidBillingAmount = "BPAXS_10125";

    /// <summary>
    /// Used when Listing To Billing Rate is invalid.
    /// </summary>
    public const string InvalidListingToBillingRate = "BPAXS_10126";

    /// <summary>
    /// Used when rejection coupon breakdown record for combination of ‘Ticket Issuing Airline’, ‘Ticket/Document Number’ and ‘Coupon No’ 
    // which doesn’t exist in the Form D/E mentioned 
    /// </summary>
    public const string InvalidRejectionCouponRecord = "BPAXS_10127";

    /// <summary>
    /// Used when Vat calculated amount difference is not match with sum of Vat breakdown total amount.
    /// </summary>
    public const string InvalidVatDifferenceAmount = "BPAXS_10128";

    /// <summary>
    /// Used when Vat calculated amount difference is not match with sum of Vat breakdown total amount.
    /// </summary>
    public const string InvalidVatBreakdownRecord = "BPAXS_10129";

    /// <summary>
    /// Used when use try to updated billing month for Form D/E, X or Xf having more than one transaction
    /// </summary>
    public const string InvalidUpdateOperation = "BPAXS_10130";

    /// <summary>
    /// Used when use try to updated billing month for Form D/E, X or Xf having more than one transaction
    /// </summary>
    public const string InvalidAmountFormD = "BPAXS_10131";

    /// <summary>
    /// Used when use try to updated billing month for Form D/E, X or Xf having more than one transaction
    /// </summary>
    public const string InvalidPercentFormD = "BPAXS_10132";

    /// <summary>
    /// Used when the last two digits of ProvBillingMonth should be 00 in case of Sampling Form C
    /// </summary>
    public const string InvalidProvisionalBillingMonthForFormC = "BPAXS_10133";
    
    /// <summary>
    /// Form B Total Amount is not matching with Provisional Invoice total amount.
    /// </summary>
    public const string FormBTotalProvisionalInvoicetotalMismatch = "BPAXS_10153";

    /// <summary>
    /// Used when  Multiple entries for Provisional Invoice + Batch Number + Record Sequence No + Ticket Issuing Airline + Coupon Number + Ticket/Document Number exist.
    /// </summary>
    public const string InvalidCouponNumberFormD = "BPAXS_10180";

    /// <summary>
    /// Used when Gross Total of universe mismatch.
    /// </summary>
    public const string InvalidFormAbGrossTotalOfUniverse = "BPAXS_10181";

    /// <summary>
    /// Used when Gross Total of UAF mismatch.
    /// </summary>
    public const string InvalidGrossTotalOfUaf = "BPAXS_10182";

    /// <summary>
    /// Used when Universe Adjusted Gross Amount mismatch.
    /// </summary>
    public const string InvalidUniverseAdjustedGrossAmount = "BPAXS_10183";

    /// <summary>
    /// Used when Sample Adjusted Gross Amount mismatch.
    /// </summary>
    public const string InvalidSampleAdjustedGrossAmount = "BPAXS_10184";
    
    /// <summary>
    /// Used when Sampling Constant mismatch.
    /// </summary>
    public const string InvalidSamplingConstant = "BPAXS_10185";

    /// <summary>
    /// Used when Totals of Gross Amounts x Sampling Constant mismatch.
    /// </summary>
    public const string InvalidTotalOfGrossAmtXSamplingConstant = "BPAXS_10186";

    /// <summary>
    /// Used when Totals of ISC Amounts x Sampling Constant mismatch.
    /// </summary>
    public const string InvalidTotalOfIscAmtXSamplingConstant = "BPAXS_10187";

    /// <summary>
    /// Used when Totals of Other Commission Amounts x Sampling Constant mismatch.
    /// </summary>
    public const string InvalidTotalOfOtherCommissionAmtXSamplingConstant = "BPAXS_10188";

    /// <summary>
    /// Used when Totals of Uatp Coupon Amounts x Sampling Constant mismatch.
    /// </summary>
    public const string InvalidUatpCouponTotalXSamplingConstant = "BPAXS_10189";

    /// <summary>
    /// Used when Totals of Handling Fee Amounts x Sampling Constant mismatch.
    /// </summary>
    public const string InvalidHandlingFeeTotalAmtXSamplingConstant = "BPAXS_10190";


    /// <summary>
    /// Used when Totals of TaxCoupon Amounts x Sampling Constant mismatch.
    /// </summary>
    public const string InvalidTaxCouponTotalsXSamplingConstant = "BPAXS_10191";

    /// <summary>
    /// Used when Totals of Vat Coupon Amounts x Sampling Constant mismatch.
    /// </summary>
    public const string InvalidVatCouponTotalsXSamplingConstant = "BPAXS_10192";

    /// <summary>
    /// Used when NetAmountDue mismatch.
    /// </summary>
    public const string InvalidNetAmountDue = "BPAXS_10193";

    /// <summary>
    /// Used when Net AmountDue In Currency Of Billing mismatch.
    /// </summary>
    public const string InvalidNetAmountDueInCurrencyOfBilling = "BPAXS_10194";
    
    /// <summary>
    /// Used when Net Billed Credited Amount mismatch.
    /// </summary>
    public const string InvalidNetBilledCreditedAmount = "BPAXS_10195";

    /// <summary>
    /// Used when Total Amount Form B mismatch.
    /// </summary>
    public const string InvalidTotalAmountFormB = "BPAXS_10196";

    /// <summary>
    /// Form C From member id and provisional billing member id cannot be same.
    /// </summary>
    public const string InvalidFormCMemberCombination = "BPAXS_10199";

    /// <summary>
    /// Audit trail fail for coupon number + Ticket Issuing Airline + Ticket Document Number + Coupon Number
    /// </summary>
    public const string AuditTrailFailForCouponNumber = "BPAXS_10200";

    /// <summary>
    /// Used when handling fee amount in fromD record doesn't match with audit trail.
    /// </summary>
    public const string InvalidHandlingFeeAmount = "BPAXS_10201";

    /// <summary>
    /// Used when handling fee amount in fromD record doesn't match with audit trail.
    /// </summary>
    public const string ProvisionalInvoiceNotFound = "BPAXS_10202";

    /// <summary>
    /// SamplingFormCrecord should be available only when NilFormCindication equals Y
    /// </summary>
    public const string InvalidSamplingFormCRecord = "BPAXS_10204";

    /// <summary>
    /// Invalid billing MemberId
    /// </summary>
    public const string InvalidBillingMemberId = "BPAXS_10205";

    /// <summary>
    /// Invalid billed MemberId
    /// </summary>
    public const string InvalidBilledMemberId = "BPAXS_10206";

    /// <summary>
    /// Same Billing and Billed MemberId
    /// </summary>
    public const string SameBillingAndBilledMemberId = "BPAXS_10207";

    /// <summary>
    /// Should be always "51"
    /// </summary>
    public const string InvalidChargeCode = "BPAXS_10208";

    /// <summary>
    /// Form C Header cannot be updated as Form C items exist. 
    /// </summary>
    public const string InvalidOperationAsFormCItemsExist = "BPAXS_10209";

    /// <summary>
    /// Nil form C indicator should not be blank for form C
    /// </summary>
    public const string InvalidNilFormCIndicatorForFormC = "BPAXS_10210";

    /// <summary>
    /// Linked provisional invoice does not exists in the database.
    /// </summary>
    public const string InvalidLinkedProvisionalInvoice = "BPAXS_10211";

    /// <summary>
    /// ISC amount does not match with ISC percentage * Evaluated Gross Amount
    /// </summary>
    public const string InvalidEvaluatedIscAmount = "BPAXS_10212";

    /// <summary>
    /// Listing to billing rate for Form C should be 0
    /// </summary>
    public const string InvalidListingToBillingRateForFormC = "BPAXS_10213";

    /// <summary>
    /// Ticket document number, ticket coupon number and ticket issuing airline does not exists in Form AB
    /// </summary>
    public const string TicketDetailsDoesNotExistsInFormAb = "BPAXS_10214";

    /// <summary>
    /// Settlement method for form c should always be N
    /// </summary>
    public const string InvalidSettlementMethodForFormC = "BPAXS_10215";

    /// <summary>
    /// Form D ticket doc number should not be a Form C rejected coupon
    /// </summary>
    public const string InvalidTickeDocNumberForFormC = "BPAXS_10216";

    /// <summary>
    /// Total Provisional Adjustment Amount should be equal to the sum of individual absorption amounts (Fare, ISC, UATP, Tax, Handling Fee, Other Commission, VAT) 
    /// </summary>
    public const string TotalProvisionalAdjustmentAmountIsInvalid = "BPAXS_10217";

    /// <summary>
    /// Total provisional adjustment amount should be equal to "Total Gross Value" multiplied by "Provisional Adjustment Rate"
    /// </summary>
    public const string TotalProvAdjustAmountDoesNotEqualToSumOfAbsAmount = "BPAXS_10218";

    /// <summary>
    /// Used when Provisional Invoice for given combination of Invoice Number, Invoice Date 
    /// and Invoice Period is repeated
    /// </summary>
    public const string ErrorDuplicateProvisionalInvoiceRecord = "BPAXS_10219";

    /// <summary>
    /// Invoice number for Form C should be 0
    /// </summary>
    public const string InvalidInvoiceNumberForFormC = "BPAXS_10220";

    /// <summary>
    /// Billing date Form C should be 0
    /// </summary>
    public const string InvalidBillingDateForFormC = "BPAXS_10221";

    /// <summary>
    /// Billing period for Form C should be 0
    /// </summary>
    public const string InvalidBillingPeriodForFormC = "BPAXS_10222";

    /// <summary>
    /// Member Migrated for Form C but data not found. Used while creating Form D/E header.
    /// </summary>
    public const string MemberMigratedForFormCButDataNotFound = "BPAXS_10223";

    /// <summary>
    /// Member Migrated for Form D/E but data not found. Used while creating Form F header.
    /// </summary>
    public const string MemberMigratedForFormDEButDataNotFound = "BPAXS_10224";

    /// <summary>
    /// Form C coupon should not present in form c coupon list
    /// </summary>
    public const string AuditTrailFailsForFormDCoupon = "BPAXS_10225";

    public const string InvalidFormDBvcMatrix = "BPAXS_10226";

    /// <summary>
    /// Sampling form c cannot be submitted after Sample Digit Announcement.
    /// </summary>
    public const string InvalidSamlingFormCSubmission = "BPAXS_10227";

    /// <summary>
    /// Net reject amount after sampling constant does not match with net reject amount X sampling constant.
    /// </summary>
    public const string NetRejectAmountAfterScDoesNotMatchWithScXRejectAmount = "BPAXS_10228";

    /// <summary>
    /// Net reject amount after sampling constant should be zero for non sampling transactions
    /// </summary>
    public const string NetRejectAmountAfterScShouldBeZero = "BPAXS_10229";

    /// <summary>
    /// Total Net Amount without VAT should be equal to Net Total – Total Vat Amount.
    /// </summary>
    public const string TotalNetAmountWithoutVatIsInvalidForSampling = "BPAXS_10230";    

    /// <summary>
    /// Listing currency of form C does not match with provisional invoice
    /// </summary>
    public const string ListingCurrencyOfFormCDoesNotMatchWithProvInvoice = "BPAXS_10231";

    /// <summary>
    /// Used when Gross Total of universe mismatch.
    /// </summary>
    public const string InvalidFormCGrossTotalOfUniverse = "BPAXS_10232";

    /// <summary>
    /// Linked provisional invoice does not exists in the database.
    /// </summary>
    public const string NoLinkedProvisionalInvoiceForProvBillingPeriod = "BPAXS_10236";

    /// <summary>
    /// No more than one form D/E can be billed by an airline for a provisional month to same provisional billing airline.
    /// </summary>
    public const string DuplicateFormDeInvoiceForTheProvisionalMonth = "BPAXS_10237";    


    public const string InvalidProvisionalMonthYear = "BPAXS_10238";

    /// <summary>
    /// UATP amount does not match with UATP percentage * Evaluated Gross Amount
    /// </summary>
    public const string InvalidEvaluatedUatpAmount = "BPAXS_10240";

    public const string FileExistWithFormCAndNilIndicator = "BPAXS_10241";

    public const string IncorrectInvoiceTypeForNetBilledOrCreditedAmount = "BPAXS_10244";

    public const string InvalidSamplingFormCSourceCodeRecord = "BPAXS_10252";

    /// <summary>
    /// Unable to validate Gross UAF amount of Form D/E as Form AB present in SIS but Form C not present in SIS.
    /// </summary>
    public const string UnableToValidateGrossUafAsFormCNotFound = "BPAXS_10253";

    public const string FormDCouponFoundInFormC = "BPAXS_10254";

    /// <summary>
    /// CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured
    /// Size of Ticket/Document cannot be Number is greater than 10 digits. (Sampling)
    /// </summary>
    public const string TicketFimDocumentNoGreaterThanTenS = "BPAXS_10257";

    public const string AuditTrailMismatchSamplingFormDCoupon = "BPAXS_10258";
  }
}

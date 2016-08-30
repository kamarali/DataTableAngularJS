namespace Iata.IS.Data.MemberProfile.Impl
{
  public struct MemberRepositoryConstants
  {
    public const string FilterParameterName = "FILTER_I";
    public const string CurrentMemberIdParameterName = "MEMBER_ID_TO_SKIP_I";
    public const string IncludePendingParameterName = "INCLUDE_PENDING_I";
    public const string IncludeBasicParameterName = "INCLUDE_BASIC_I";
    public const string IncludeRestrictedParameterName = "INCLUDE_RESTRICTED_I";
    public const string IncludeTerminatedParameterName = "INCLUDE_TERMINATED_I";
    public const string IncludeAchParameterName = "INCLUDE_ACH_I";
    public const string IncludeIchParameterName = "INCLUDE_ICH_I";
    public const string IncludeIchZonesParameterName = "INCLUDE_ICH_ZONES_I";
    public const string MemberStringOutputParameterName = "MEMBERS_O";
    public const string ExcludeMergedMember = "EXCLUDE_MERGED_MEMBER";
    public const string IncludeMemberType = "INCLUDE_MEMBER_TYPE";
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
    * Desc: Added new parameter to exclude Type B Members. */
    public const string ExcludeTypeBMember = "EXCLUDE_TYPEB_MEMBERS";

    public const string GetMembersFunctionName = "GetMembers";

    #region GetMembersConfigDetail
    public const string MemberIdParameterName = "MEMBER_ID_I";
    public const string ConfigDetailParameterName = "CONFIG_DETAIL_ID_I";
    public const string ValueOutputParameterName = "CONFIG_VALUE_O";
    public const string GetMemberConfigFunctionName = "GetMembersConfigDetail";
    
    #endregion

    #region Child and Final Parent Members
     public const string GetFinalParentFunctionName = "GetFinalParentMember";
     public const string MemberFinalParentId = "PARENT_MEMBER_ID_O";
     public const string GetChildMembers = "GetChildMemberIds";
    #endregion

    #region CMP#520- Management of Super Users
     public const string UserIdParameterName = "USERID_I";
     public const string IsSuperUserParameterName = "IS_SUPER_USER";
     public const string ChangeUserStatusFunctionName = "CheckUserStatusChanged";
    #endregion

     #region 
     public const string MemberCodeNumeric = "MEMBER_CODE_NUMERIC_I";
     public const string MemberCodeAlpha = "MEMBER_CODE_ALPHA_I";
     public const string IsMemberShipStatus = "IS_MEMBERSHIP_STATUS_I";
     public const string IsMigrationStatus = "IS_MIGRATION_STATUS_I";
     public const string IataMemberStatus = "IATA_MEMBERSHIP_STATUS_I";
     public const string IsOpsComments = "IS_OPS_COMMENTS_I";
     public const string MemberCommercialName = "MEMBER_COMMERCIAL_NAME_I";
     public const string IsMembershipSubStatusId = "IS_MEMBERSHIP_SUB_STATUS_I";
     public const string MemberLegalName = "MEMBER_LEGAL_NAME_I";
     public const string MemberShipStatus = "MEMBERSHIP_STATUS_ID_I";
     public const string StatusChangeDate = "STATUS_CHANGE_DATE_I";
     public const string MemberType = "MEMBER_TYPE_I";
     public const string LocationCode = "LOCATION_CODE_I";
     public const string LocMemberLegalName = "LOC_MEMBER_LEGAL_NAME_I";
     public const string LocMemberCommName = "LOC_MEMBER_COMMERCIAL_NAME_I";
     public const string LocActive = "LOC_ACTIVE_I";
     public const string RegistrationId = "REGISTRATION_ID_I";
     public const string TaxVatRegistrationNumber = "TAX_VAT_REGISTRATION_NUMBER_I";
     public const string AddTaxVatRegistrationNumber = "ADD_TAX_VAT_REGISTRATION_NUM_I";
     public const string CountryCode = "COUNTRY_CODE_I";
     public const string AddressLine1 = "ADDRESS_LINE1_I";
     public const string AddressLine2 = "ADDRESS_LINE2_I";
     public const string AddressLine3 = "ADDRESS_LINE3_I";
     public const string CityName = "CITY_NAME_I";
     public const string SubDivisonCode = "SUB_DIVISION_CODE_I";
     public const string SubDivisionName = "SUB_DIVISION_NAME_I";
     public const string PostalCode = "POSTAL_CODE_I";
     public const string LegalText = "LEGAL_TEXT_I";
     public const string Iban = "IBAN_I";
     public const string Swift = "SWIFT_I";
     public const string BankCode = "BANK_CODE_I";
     public const string BranchCode = "BRANCH_CODE_I";
     public const string BankAccountNumber = "BANK_ACCOUNT_NUMBER_I";
     public const string BankAccountName = "BANK_ACCOUNT_NAME_I";
     public const string CurrencyCode = "CURRENCY_CODE_NUM_I";
     public const string BankName = "BANK_NAME_I";
     public const string MemberTypeId = "MEMBER_TYPE_ID_I";

     #region CMP#608 Create Member through CSV or ISWEB
     public const string EBillingLegalText = "E_BILLING_LEGAL_TEXT_I";
     public const string LegalArcReqMiscRecInv = "LEGAL_ARC_REQ_MISC_REC_INV_I";
     public const string LealArcReqMiscPayInv = "LEGAL_ARC_REQ_MISC_PAY_INV_I";
     public const string IncludeLstMiscRecArc = "INCLUDE_LST_MISC_REC_ARC_I";
     public const string IncludeLstMiscPayArc = "INCLUDE_LST_MISC_PAY_ARC_I";
     public const string AllowFileTypSuppDoc = "ALLOWED_FILE_TYP_SUPP_DOC_I";
     public const string BillingXmlOutput = "BILLING_XML_OUTPUT_I";
     public const string IsDailyPayXmlReq = "IS_DAILY_PAY_XML_REQ_I";
     public const string IsPdfAsOutAsBilled = "IS_PDF_AS_OUT_AS_BILLED_I";
     public const string IsListAsOutAsBilled = "IS_LIST_AS_OUT_AS_BILLED_I";
     public const string IsSuppAsOutAsBilled = "IS_SUPP_AS_OUT_AS_BILLED_I";
     public const string IsDsFilAsOutAsBilled = "IS_DS_FIL_AS_OUT_AS_BILLED_I";
     public const string MiscAccountId = "MISC_ACCOUNT_ID_I";
     public const string DigitalSignApp = "DIGITAL_SIGN_APPLICATION_I";
     public const string DigitalSignVerification = "DIGITAL_SIGN_VERIFICATION_I";
     public const string IsLegalArchReq = "IS_LEGAL_ARCHIVING_REQUIRED_I";
     public const string CdcCompartmentForInv = "CDC_COMPARTMENTID_FOR_INV_I";
     public const string RecInvDsToBeApplied = "REC_INV_DS_TO_BE_APPLIED_I";
     public const string PayInvDsToBeApplied = "PAY_INV_DS_TO_BE_APPLIED_I";
     public const string EmailId = "EMAIL_ID_I";
     public const string FirstName = "FIRST_NAME_I";
     public const string LastName = "LAST_NAME_I";
     public const string UserType = "USER_TYPE_I";
     public const string IsPassword = "IS_PASSWORD_I";
     public const string PasswordSalt = "PASSWORD_SALT_I";
     public const string PermTemplateName = "PERM_TEMPLATE_NAME_I";
     public const string IsFileLogId = "IS_FILE_LOG_ID_I"; 
     #endregion
     public const string LastUpdatedOn = "LAST_UPDATED_ON_I";
     public const string LastUpdateBy = "LAST_UPDATED_BY_I";
     public const string MemberIdOut = "MEMBER_ID_O";
     public const string CreateISMemberFunctionName = "CreateISMember";
     #endregion

     #region SCP223072: Unable to change Member Code
     public const string OldMemberCodeNumericParameterName = "OLD_MEMBER_CODE_NUMERIC_I";
     public const string NewMemberCodeNumericParameterName = "NEW_MEMBER_CODE_NUMERIC_I";
     public const string MembersIdParameterName = "MEMBER_ID_I";
     public const string CallFromParameterName = "CALL_FROM_I";
     public const string ResultParameterName = "RESULT_O";
     public const string UpdateBiConfigForNewMemberCodeNumericFunctionName = "UpdateBiConfigForNewMemberCodeNumeric";
     #endregion
  }
}
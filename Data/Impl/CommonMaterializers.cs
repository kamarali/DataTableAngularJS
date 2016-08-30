using System;
using Iata.IS.Model.MemberProfile;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Impl
{
  public class CommonMaterializers
  {
    public  readonly Materializer<Member> PaxInvoiceMemberAuditMaterializer = new Materializer<Member>(r =>
    new Member
    {
      Id = r.Field<object>("MEMBER_ID") != null ? r.Field<int>("MEMBER_ID") : 0,
      LegalName = r.Field<string>("MEMBER_LEGAL_NAME"),
      CommercialName = r.Field<string>("MEMBER_COMMERCIAL_NAME"),
      MemberCodeAlpha = r.Field<string>("MEMBER_CODE_NUMERIC"),
      MemberCodeNumeric = r.Field<string>("MEMBER_CODE_ALPHA")
    });

    public  readonly Materializer<Currency> CurrencyAuditMaterializer = new Materializer<Currency>(r =>
      new Currency
      {
        Code = r.TryGetField<string>("CURRENCY_CODE_ALPHA"),
        Id = r.TryGetField<object>("CURRENCY_CODE_NUM") != null ? r.Field<int>("CURRENCY_CODE_NUM") : 0,
        Name = r.TryGetField<string>("CURRENCY_NAME"),
        Precision = r.TryGetField<object>("CURRENCY_PRECISION") != null ? r.TryGetField<int>("CURRENCY_PRECISION") : 0,
        LastUpdatedOn = r.TryGetField<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.TryGetField<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        IsActive = r.TryGetField<int>("IS_ACTIVE") > 0
      }
      );

    public  readonly Materializer<Currency> CurrencyMaterializer = new Materializer<Currency>(r =>
     new Currency
     {
       Code = r.TryGetField<string>("CURRENCY_CODE_ALPHA"),
       Id = r.TryGetField<object>("CURRENCY_CODE_NUM") != null ? r.Field<int>("CURRENCY_CODE_NUM") : 0,
       Name = r.TryGetField<string>("CURRENCY_NAME"),
       Precision = r.TryGetField<object>("CURRENCY_PRECISION") != null ? r.TryGetField<int>("CURRENCY_PRECISION") : 0,
       LastUpdatedOn = r.TryGetField<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
       LastUpdatedBy = r.TryGetField<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
       IsActive = r.TryGetField<int>("IS_ACTIVE") > 0
     }
     );
    
    public  readonly Materializer<Member> MemberMaterializer = new Materializer<Member>(r =>
      new Member
      {
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
        IsOpsComments = r.Field<object>("IS_OPS_COMMENTS") != null ? r.Field<string>("IS_OPS_COMMENTS") : String.Empty,
        IataMemberStatus = r.Field<object>("IATA_MEMBERSHIP_STATUS") != null ? (r.Field<int>("IATA_MEMBERSHIP_STATUS") == 0 ? false : true) : false,
        IsMigrationStatusId = r.Field<object>("IS_MIGRATION_STATUS") != null ? r.Field<int?>("IS_MIGRATION_STATUS") : null,
        IsMembershipStatusId = r.Field<object>("IS_MEMBERSHIP_STATUS") != null ? r.Field<int>("IS_MEMBERSHIP_STATUS") : 0,
        IsMembershipSubStatusId = r.Field<object>("IS_MEMBERSHIP_SUB_STATUS") != null ? r.Field<int?>("IS_MEMBERSHIP_SUB_STATUS") : null,
        MemberCodeAlpha = r.Field<string>("MEMBER_CODE_ALPHA"),
        MemberCodeNumeric = r.Field<string>("MEMBER_CODE_NUMERIC"),
        CommercialName = r.Field<string>("MEMBER_COMMERCIAL_NAME"),
        Id = r.Field<object>("MEMBER_ID") != null ? r.Field<int>("MEMBER_ID") : 0,
        LegalName = r.Field<string>("MEMBER_LEGAL_NAME"),
        EnableSampMemInfoContact = r.Field<object>("ENABLE_SAMP_MEM_INFO_CONTACT") != null ? (r.Field<int>("ENABLE_SAMP_MEM_INFO_CONTACT") == 0 ? false : true) : false,
        EnableSampScMemInfoContact = r.Field<object>("ENABLE_SAMP_SC_MEM_INFO_CONT") != null ? (r.Field<int>("ENABLE_SAMP_SC_MEM_INFO_CONT") == 0 ? false : true) : false,
        EnableSampQSmartInfoContact = r.Field<object>("ENABLE_SAMP_QSMART_INFO_CONT") != null ? (r.Field<int>("ENABLE_SAMP_QSMART_INFO_CONT") == 0 ? false : true) : false,
        EnableOldIdecScInfoContact = r.Field<object>("ENABLE_OLD_IDEC_SC_INFO_CONT") != null ? (r.Field<int>("ENABLE_OLD_IDEC_SC_INFO_CONT") == 0 ? false : true) : false,
        EnableFirstNFinalInfoContact = r.Field<object>("ENABLE_FIRST_N_FINAL_INFO_CONT") != null ? (r.Field<int>("ENABLE_FIRST_N_FINAL_INFO_CONT") == 0 ? false : true) : false,
        EnableFnfAsgInfoContact = r.Field<object>("ENABLE_FNF_ASG_INFO_CONTACT") != null ? (r.Field<int>("ENABLE_FNF_ASG_INFO_CONTACT") == 0 ? false : true) : false,
        EnableFnfAiaInfoContact = r.Field<object>("ENABLE_FNF_AIA_INFO_CONTACT") != null ? (r.Field<int>("ENABLE_FNF_AIA_INFO_CONTACT") == 0 ? false : true) : false,
        EnableRawgMemInfoContact = r.Field<object>("ENABLE_RAWG_MEM_INFO_CONTACT") != null ? (r.Field<int>("ENABLE_RAWG_MEM_INFO_CONTACT") == 0 ? false : true) : false,
        EnableIawgMemInfoContact = r.Field<object>("ENABLE_IAWG_MEM_INFO_CONTACT") != null ? (r.Field<int>("ENABLE_IAWG_MEM_INFO_CONTACT") == 0 ? false : true) : false,
        EnableIchPanelInfoContact = r.Field<object>("ENABLE_ICH_PANEL_INFO_CONTACT") != null ? (r.Field<int>("ENABLE_ICH_PANEL_INFO_CONTACT") == 0 ? false : true) : false,
        EnableEInvWgInfoContact = r.Field<object>("ENABLE_E_INV_WG_INFO_CONTACT") != null ? (r.Field<int>("ENABLE_E_INV_WG_INFO_CONTACT") == 0 ? false : true) : false,
        EnableSisScInfoContact = r.Field<object>("ENABLE_SIS_SC_INFO_CONTACT") != null ? (r.Field<int>("ENABLE_SIS_SC_INFO_CONTACT") == 0 ? false : true) : false,
        AllowContactDetailsDownload = r.Field<object>("ALLOW_CONTACT_DETAILS_DOWNLOAD") != null ? (r.Field<int>("ALLOW_CONTACT_DETAILS_DOWNLOAD") == 0 ? false : true) : false,
        IsParticipateInValueConfirmation = r.Field<object>("IS_PARTICIPATE_IN_VALUE_CONFIR") != null ? (r.Field<int>("IS_PARTICIPATE_IN_VALUE_CONFIR") == 0 ? false : true) : false,
        IsParticipateInValueDetermination = r.Field<object>("IS_PARTICIPATE_IN_VALUE_DETERM") != null ? (r.Field<int>("IS_PARTICIPATE_IN_VALUE_DETERM") == 0 ? false : true) : false,
        IchMemberStatusId = r.Field<object>("ICH_MEMBERSHIP_STATUS") != null ? r.Field<int>("ICH_MEMBERSHIP_STATUS") : 0,
        AchMemberStatusId = r.Field<object>("ACH_MEMBERSHIP_STATUS") != null ? r.Field<int>("ACH_MEMBERSHIP_STATUS") : 0,
      });

    public  readonly Materializer<User> UserMaterializer = new Materializer<User>(um =>
   new User
   {
     Id = um.Field<object>("IS_USER_ID") != null ? um.Field<int>("IS_USER_ID") : 0,
     FirstName = um.Field<string>("FIRST_NAME"),
     LastName = um.Field<string>("LAST_NAME"),
     LastUpdatedBy = um.Field<object>("LAST_UPDATED_BY") != null ? um.Field<int>("LAST_UPDATED_BY") : 0,
     LastUpdatedOn = um.Field<object>("LAST_UPDATED_ON") != null ? um.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
   });

    public  readonly Materializer<Country> CountryMaterializer =
      new Materializer<Country>(r => new Country
      {
        Id = r.Field<string>("COUNTRY_CODE"),
        Name = r.Field<string>("COUNTRY_NAME"),
        DsSupportedByAtos = r.Field<int>("DS_SUPPORTED_BY_ATOS") > 0,
        DsFormat = r.Field<object>("DS_FORMAT") != null ? r.Field<string>("DS_FORMAT") : string.Empty,
        IsActive = r.Field<int>("IS_ACTIVE") > 0,
        LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
        LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0
      });

    public  readonly Materializer<MemberLocationInformation> MemberLocationInformationMaterializer = new Materializer<MemberLocationInformation>(r =>
    new MemberLocationInformation
    {
      MemberLocationCode = r.Field<string>("MEM_LOCATION_CODE"),
      IsBillingMember = r.Field<object>("IS_BILLING_MEMBER") != null ? (r.Field<int>("IS_BILLING_MEMBER") == 0 ? false : true) : false,
      InvoiceId = r.Field<byte[]>("INVOICE_ID") != null ? new Guid(r.Field<byte[]>("INVOICE_ID")) : new Guid(),
      DigitalSignatureRequiredId = r.Field<object>("DS_REQUIRED_ID") != null ? r.Field<int>("DS_REQUIRED_ID") : 0,
      LegalText = r.Field<string>("INVOICE_FOOTER_TEXT"),
      LastUpdatedOn = r.Field<object>("LAST_UPDATED_ON") != null ? r.Field<DateTime>("LAST_UPDATED_ON") : new DateTime(),
      LastUpdatedBy = r.Field<object>("LAST_UPDATED_BY") != null ? r.Field<int>("LAST_UPDATED_BY") : 0,
      PostalCode = r.Field<string>("POSTAL_CODE"),
      CountryName = r.Field<string>("COUNTRY_NAME"),
      CountryCode = r.Field<string>("COUNTRY_CODE"),
      SubdivisionName = r.Field<string>("SUB_DIVISION_NAME"),
      SubdivisionCode = r.Field<string>("SUBDIVISION_CODE"),
      CityName = r.Field<string>("CITY_NAME"),
      AddressLine3 = r.Field<string>("ADDRESS_LINE3"),
      AddressLine2 = r.Field<string>("ADDRESS_LINE2"),
      AddressLine1 = r.Field<string>("ADDRESS_LINE1"),
      CompanyRegistrationId = r.Field<string>("ORG_REG_ID"),
      TaxRegistrationId = r.Field<string>("TAX_VAT_REG_ID"),
      OrganizationName = r.Field<string>("ORG_LEGAL_NAME"),
      AdditionalTaxVatRegistrationNumber = r.Field<string>("ADD_TAX_VAT_REGISTRATION_NUM"),
      Id = r.Field<byte[]>("MEMBER_LOC_INFO_ID") != null ? new Guid(r.Field<byte[]>("MEMBER_LOC_INFO_ID")) : new Guid(),
    });
  }
}

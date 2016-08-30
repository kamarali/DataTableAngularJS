using System;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Enums;


namespace Iata.IS.Model.MemberProfile
{
    public class DownloadMemberDetails //: EntityBase<int>
    {
        // Member Details 
        public string MemberCodeNumeric { get; set; }
        public string MemberCodeAlpha { get; set; }
        public string LegalName { get; set; }
        public string CommercialName { get; set; }
        public string IsMembershipStatus { get; set; }
        public string IsMembershipSubStatus { get; set; }
        public bool IataMemberStatus { get; set; }
        public bool IchMemberStatus { get; set; }
        public bool AchMemberStatus { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }

        // Member Details End
        // Mem Location 
        public string RegistrationId { get; set; }
        public string TaxVatRegistrationNumber { get; set; }
        public string AdditionalTaxVatRegistrationNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string CityName { get; set; }
        public string SubDivisionName { get; set; }
        public string SubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Iban { get; set; }
        public string Swift { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public string CurrencyId { get; set; }
        public string IsOpsComments { get; set; }
        // Mem Location 
        // SIS OPS  Start

        public bool PaxOldIdecMember { get; set; }
        public bool CgoOldIdecMember { get; set; }
        public bool IsParticipateInValueDetermination { get; set; }
        public bool IsParticipateInValueConfirmation { get; set; }
        public bool DigitalSignApplication { get; set; }
        public bool DigitalSignVerification { get; set; }
        public bool LegalArchivingRequired { get; set; }
        public string CdcCompartmentIDforInv { get; set; }
        public bool UatpInvoiceHandledbyAtcan { get; set; }
        public string IsMerged { get; set; }

        // SIS OPS End
        #region E-BILLING

        public string LegalText { get; set; }
        public bool LegalArchRequiredforPaxRecInv { get; set; }
        public bool IncludeListingsPaxRecArch { get; set; }
        public bool LegalArchRequiredforPaxPayInv { get; set; }
        public bool IncludeListingsPaxPayArch { get; set; }
        
        public bool LegalArchRequiredforCgoRecInv { get; set; }
        public bool IncludeListingsCgoRecArch { get; set; }
        public bool LegalArchRequiredforCgoPayInv { get; set; }
        public bool IncludeListingsCgoPayArch { get; set; }
        public bool LegalArchRequiredforMiscRecInv { get; set; }
        public bool IncludeListingsMiscRecArch { get; set; }
        //CMP #666: MISC Legal Archiving Per Location ID
        public string MiscRecArchLocReq { get; set; } 
        public string MiscRecArchLocs { get; set; }
        public bool LegalArchRequiredforMiscPayInv { get; set; }
        public bool IncludeListingsMiscPayArch { get; set; }
        //CMP #666: MISC Legal Archiving Per Location ID
        public string MiscPayArchLocReq { get; set; }
        public string MiscPayArchLocs { get; set; }
        public bool LegalArchRequiredforUatpRecInv { get; set; }
        public bool IncludeListingsUatpRecArch { get; set; }
        public bool LegalArchRequiredforUatpPayInv { get; set; }
        public bool IncludeListingsUatpPayArch { get; set; }
        public bool IsHideUserNameInAuditTrail { get; set; }
        #endregion
        
       

        #region Passenger

        public bool IsPAXFutureBillingSubAllowed { get; set; }
        public string PAXRejectionOnValidationFailure { get; set; }
        public bool IsOnlineCorrectionAllowed { get; set; }
        public string PaxAllowedFileTypesForSupportingDocuments { get; set; }
        public string SamplingCareerTypeId { get; set; }
        public bool IsParticipateInAutoBilling { get; set; }
        public string InvoiceNumberRangePrefix { get; set; }
        public long? InvoiceNumberRangeFrom { get; set; }
        public long? InvoiceNumberRangeTo { get; set; }
        public bool IsIsrFileRequired { get; set; }
        public int CutOffTime { get; set; }
        public int? ListingCurrencyId { get; set; }
        public bool BilledInvoiceIdecOutput { get; set; }
        public bool BilledInvoiceXmlOutput { get; set; }
        public string IsIdecOutputVersion { get; set; }
        public string IsXmlOutputVersion { get; set; }
        public bool IsConsolidatedProvisionalBillingFileRequired { get; set; }
        public bool IsAutomatedVcDetailsReportRequired { get; set; }
        public bool DownConvertISTranToOldIdec { set; get; }
        public bool PAXIsPdfAsOtherOutputAsBilledEntity { get; set; }
        public bool PAXIsDetailListingAsOtherOutputAsBilledEntity { get; set; }
        public bool PAXIsSuppDocAsOtherOutputAsBilledEntity { get; set; }
        public bool PAXIsMemoAsOtherOutputAsBilledEntity { get; set; }
        public bool PAXIsDsFileAsOtherOutputAsBilledEntity { get; set; }
        public bool PAXIsPdfAsOtherOutputAsBillingEntity { get; set; }
        public bool PAXIsDetailListingAsOtherOutputAsBillingEntity { get; set; }
        public bool PAXIsMemoAsOtherOutputAsBillingEntity { get; set; }
        public bool PAXIsDsFileAsOtherOutputAsBillingEntity { get; set; }

        public string NonSamplePrimeBillingIsIdecMigrationStatus { get; set; }
        public DateTime? NonSamplePrimeBillingIsIdecCertifiedOn { get; set; }
        public DateTime? NonSamplePrimeBillingIsIdecMigratedDate { set; get; }

        public string NonSamplePrimeBillingIsxmlMigrationStatus { get; set; }
        public DateTime? NonSamplePrimeBillingIsxmlCertifiedOn { get; set; }
        public DateTime? NonSamplePrimeBillingIsxmlMigratedDate { set; get; }

        public string SamplingProvIsIdecMigrationStatus { get; set; }
        public DateTime? SamplingProvIsIdecCerfifiedOn { set; get; }
        public DateTime? SamplingProvIsIdecMigratedDate { set; get; }

        public string SamplingProvIsxmlMigrationStatus { get; set; }
        public DateTime? SamplingProvIsxmlCertifiedOn { set; get; }
        public DateTime? SamplingProvIsxmlMigratedDate { set; get; }

        public string NonSampleRmIsIdecMigrationStatus { get; set; }
        public DateTime? NonSampleRmIsIdecCertifiedOn { set; get; }
        public DateTime? NonSampleRmIsIdecMigratedDate { set; get; }

        public string NonSampleBmIsIdecMigrationStatus { get; set; }
        public DateTime? NonSampleBmIsIdecCertifiedOn { set; get; }
        public DateTime? NonSampleBmIsIdecMigratedDate { set; get; }

        public string NonSampleCmIsIdecMigrationStatus { get; set; }
        public DateTime? NonSampleCmIsIdecCertifiedOn { set; get; }
        public DateTime? NonSampleCmIsIdecMigratedDate { set; get; }

        public string NonSampleRmIsXmlMigrationStatus { get; set; }
        public DateTime? NonSampleRmIsXmlCertifiedOn { set; get; }
        public DateTime? NonSampleRmIsXmlMigratedDate { set; get; }

        public string NonSampleBmIsXmlMigrationStatus { get; set; }
        public DateTime? NonSampleBmIsXmlCertifiedOn { set; get; }
        public DateTime? NonSampleBmIsXmlMigratedDate { set; get; }

        public string NonSampleCmIsXmlMigrationStatus { get; set; }
        public DateTime? NonSampleCmIsXmlCertifiedOn { set; get; }
        public DateTime? NonSampleCmIsXmlMigratedDate { set; get; }

        public string SampleFormCIsIdecMigrationStatus { get; set; }
        public DateTime? SampleFormCIsIdecCertifiedOn { set; get; }
        public DateTime? SampleFormCIsIdecMigratedDate { set; get; }

        public string SampleFormCIsxmlMigrationStatus { get; set; }
        public DateTime? SampleFormCIsxmlCertifiedOn { set; get; }
        public DateTime? SampleFormCIsxmlMigratedDate { set; get; }

        
        public string SampleFormDEisxmlMigrationStatus { get; set; }
        public DateTime? SampleFormDeIsxmlCertifiedOn { set; get; }
        public DateTime? SampleFormDeIsxmlMigratedDate { set; get; }



        public string SampleFormDeIdecMigrationStatus { get; set; }
        public DateTime? SampleFormDeIdecCertifiedOn { set; get; }
        public DateTime? SampleFormDeIdecMigratedDate { set; get; }



        public string SampleFormFxfIsIdecMigrationStatus { get; set; }
        public DateTime? SampleFormFxfIsIdecCertifiedOn { set; get; }
        public DateTime? SampleFormFxfIsIdecMigratedDate { set; get; }

        public string SampleFormFxfIsxmlMigratedStatus { get; set; }
        public DateTime? SampleFormFxfIsxmlCertifiedOn { set; get; }
        public DateTime? SampleFormFxfIsxmlMigratedDate { set; get; }
        #endregion

        #region Cargo
        public string CGORejectionOnValidationFailure { get; set; }
        public bool CGOIsOnlineCorrectionAllowed { get; set; }
        public string CgoAllowedFileTypesForSupportingDocuments { get; set; }
        public bool CgoBilledInvoiceIdecOutput { get; set; }
        public bool CgoBilledInvoiceXmlOutput { get; set; }
        public bool DownConvertISTranToOldIdeccgo { set; get; }
        public string IsCgoIdecOutputVersion { get; set; }
        public string IsCgoXmlOutputVersion { get; set; }
        public bool CGOIsPdfAsOtherOutputAsBilledEntity { get; set; }
        public bool CGOIsDetailListingAsOtherOutputAsBilledEntity { get; set; }
        public bool CGOIsSuppDocAsOtherOutputAsBilledEntity { get; set; }
        public bool CGOIsMemoAsOtherOutputAsBilledEntity { get; set; }
        public bool CGOIsDsFileAsOtherOutputAsBilledEntity { get; set; }
        public bool CGOIsPdfAsOtherOutputAsBillingEntity { get; set; }
        public bool CGOIsDetailListingAsOtherOutputAsBillingEntity { get; set; }
        public bool CGOIsDsFileAsOtherOutputAsBillingEntity { get; set; }
        public bool CGOIsMemoAsOtherOutputAsBillingEntity { get; set; }
        public string PrimeBillingIsIdecMigrationStatus { get; set; }
        public DateTime? PrimeBillingIsIdecCertifiedOn { set; get; }
        public DateTime? PrimeBillingIsIdecMigratedDate { set; get; }
        public string PrimeBillingIsxmlMigrationStatus { get; set; }

        public DateTime? PrimeBillingIsxmlCertifiedOn { set; get; }
        public DateTime? PrimeBillingIsxmlMigratedDate { set; get; }

        public string RmIsIdecMigrationStatus { get; set; }


        public DateTime? RmIsIdecCertifiedOn { set; get; }
        public DateTime? RmIsIdecMigratedDate { set; get; }
        public string BmIsIdecMigrationStatus { get; set; }

        public DateTime? BmIsIdecCertifiedOn { set; get; }
        public DateTime? BmIsIdecMigratedDate { set; get; }
        public string CmIsIdecMigrationStatus { get; set; }

        public DateTime? CmIsIdecCertifiedOn { set; get; }
        public DateTime? CmIsIdecMigratedDate { set; get; }

        public string RmIsXmlMigrationStatus { get; set; }

        public DateTime? RmIsXmlCertifiedOn { set; get; }
        public DateTime? RmIsXmlMigratedDate { set; get; }
        public string BmIsXmlMigrationStatus { get; set; }

        public DateTime? BmIsXmlCertifiedOn { set; get; }
        public DateTime? BmIsXmlMigratedDate { set; get; }
        public string CmIsXmlMigrationStatus { get; set; }

        public DateTime? CmIsXmlCertifiedOn { set; get; }
        public DateTime? CmIsXmlMigratedDate { set; get; }
        #endregion

        #region UATP
        public string UATPRejectionOnValidationFailure { get; set; }
        public bool UATPIsOnlineCorrectionAllowed { get; set; }
        public string UatpAllowedFileTypesForSupportingDocuments { get; set; }
        public bool ISUatpInvIgnoreFromDsproc { get; set; }
        public bool UatpBilledInvoiceXmlOutput { get; set; }

        public bool UATPIsBillingDataSubmittedByThirdPartiesRequired { get; set; }
        public string IsUatpXmlOutputVersion { get; set; }
        public bool UATPIsPdfAsOtherOutputAsBilledEntity { get; set; }
        public bool UATPIsDetailListingAsOtherOutputAsBilledEntity { get; set; }
        public bool UATPIsSuppDocAsOtherOutputAsBilledEntity { get; set; }
        public bool UATPIsDsFileAsOtherOutputAsBilledEntity { get; set; }
        public bool UATPIsPdfAsOtherOutputAsBillingEntity { get; set; }
        public bool UATPIsDetailListingAsOtherOutputAsBillingEntity { get; set; }
        public bool UATPIsDsFileAsOtherOutputAsBillingEntity { get; set; }
        public string UATPBillingIsXmlMigrationStatus { get; set; }

        public DateTime? UATPBillingIsXmlCertifiedOn { set; get; }
        public DateTime? UATPBillingIsXmlMigrationDate { set; get; }
        public DateTime? UATPBillingIsWebMigrationDate { set; get; }
        #endregion

        #region Miscellaneous
        public string MISCRejectionOnValidationFailure { get; set; }

        public bool MISCIsOnlineCorrectionAllowed { get; set; }
        public string MISCAllowedFileTypesForSupportingDocuments { get; set; }
        public bool IsMISCFutureBillingSubAllowed { get; set; }
        public bool MiscBilledInvoiceXmlOutput { get; set; }
        public bool MISCIsBillingDataSubmittedByThirdPartiesRequired { get; set; }
        public bool IsDailyXmlRequired { get; set; }
        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
        public bool MISCPayableBilateralDailyISWEBDelivery { get; set; }
        public bool MISCPayableBilateralDailyOAR { get; set; }
        public bool MISCPayableBilateralDailyISXML { get; set; }


        public string IsMiscXmlOutputVersion { get; set; }
        
        public bool MISCIsPdfAsOtherOutputAsBilledEntity { get; set; }
        public bool MISCIsDetailListingAsOtherOutputAsBilledEntity { get; set; }
        public bool MISCIsSuppDocAsOtherOutputAsBilledEntity { get; set; }
        public bool MISCIsDsFileAsOtherOutputAsBilledEntity { get; set; }
        public bool MISCIsPdfAsOtherOutputAsBillingEntity { get; set; }
        public bool MISCIsDetailListingAsOtherOutputAsBillingEntity { get; set; }
        public bool MISCIsDsFileAsOtherOutputAsBillingEntity { get; set; }
        //CMP#622: MISC Outputs Split as per Location IDs
        public bool RecCopyOfLocSpecificMISCOutputsAtMain { get; set; }
        public string MISCBillingIsXmlMigrationStatus { get; set; }

        public DateTime? MISCBillingIsXmlCertifiedOn { set; get; }
        public DateTime? MISCBillingIsXmlMigrationDate { set; get; }
        public DateTime? MiscBillingIsWebMigrationDate { set; get; }
         
       
        #endregion

        #region ICH
        public string IchMemberShipStatus { get; set; }
        //CMP #625: New Fields in ICH Member Profile Update XML
        public string ICHiiNetAccountID { get; set; }
        // ID : 403095 - Member Profile Info error
        //Desc: Change Sponser Id from interger to String
        public string SponsoredById { get; set; }
        public string IchWebReportOptionsId { get; set; }

        public bool CanSubmitPaxInF12Files { get; set; }
        public bool CanSubmitCargoInF12Files { get; set; }
        public bool CanSubmitMiscInF12Files { get; set; }
        public bool CanSubmitUatpinF12Files { get; set; }
        #endregion

        #region ACH
        public string AchMembershipStatus { get; set; }
        #endregion

        #region Technical
        public string IspaxDeliveryMethod { get; set; }
        public string IspaxOutputServerUserId { get; set; }
        public string IspaxOutputServerPassword { get; set; }
        public string PaxIiNetFolder { get; set; }
        public string PaxAccountId { get; set; }
        //CMP 597
        public string ReferenceDataChangeFileForPaxAccountId { get; set; }
        public string ReferenceDataCompleteFileForPaxAccountId { get; set; }
        public string ContactsDataCompleteFileForPaxAccountId { get; set; }
        //
        public string IscgoDeliveryMethod { get; set; }
        public string IscgoOutputServerUserId { get; set; }
        public string IscgoOutputServerPassword { get; set; }
        public string CgoIiNetFolder { get; set; }
        public string CgoAccountId { get; set; }
        //CMP 597
        public string ReferenceDataChangeFileForCgoAccountId { get; set; }
        public string ReferenceDataCompleteFileForCgoAccountId { get; set; }
        public string ContactsDataCompleteFileForCgoAccountId { get; set; }

        public string IsmiscDeliveryMethod { get; set; }
        public string IsmiscOutputServerUserId { get; set; }
        public string IsmiscOutputServerPassword { get; set; }
        public string MiscIiNetFolder { get; set; }
        public string MiscAccountId { get; set; }
        
        //CMP 597
        public string ReferenceDataChangeFileForMiscAccountId { get; set; }
        public string ReferenceDataCompleteFileForMiscAccountId { get; set; }
        public string ContactsDataCompleteFileForMiscAccountId { get; set; }

        public string IsuatpDeliveryMethod { get; set; }
        public string IsuatpOutputServerUserId { get; set; }
        public string IsuatpOutputServerPassword { get; set; }
        public string UatpIiNetFolder { get; set; }
        public string UatpAccountId { get; set; }
        //CMP 597
        public string ReferenceDataChangeFileForUatpAccountId { get; set; }
        public string ReferenceDataCompleteFileForUatpAccountId { get; set; }
        public string ContactsDataCompleteFileForUatpAccountId { get; set; }
        #endregion

        //Merger Information 
        public string ParentMember { get; set; }
        public string ActualMergerDate { get; set; }
    }
}

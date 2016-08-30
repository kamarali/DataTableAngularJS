using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfileCSVUpload
{
    public class MemberProfileData
    {
        public string MemberCodeNumeric { get; set; }
        public string MemberCodeAlpha { get; set; }
        public string LegalName { get; set; }
        public string CommercialName { get; set; }
        public string ISMembershipStatus { get; set; }
        public int ISMembershipStatusId { get; set; }
        public string ISMembershipSubStatus { get; set; }
        public int ISMembershipSubStatusId { get; set; }
        public string RegistrationID { get; set; }
        public string TaxVatRegistrationNumber { get; set; }
        public string AddTaxVatRegistrationNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string CityName { get; set; }
        public string SubDivisionName { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string DigitalSignApplication { get; set; }
        public string DigitalSignVerification { get; set; }
        public string ReceivablesInvoicesDSToBeAppliedFor { get; set; }
        public string PayablesInvoicesDSToBeAppliedFor { get; set; }
        public string DefaultInvoiceFooterText { get; set; }
        public string LegalArchivingRequired { get; set; }
        public string CDCCompartmentIDForInv { get; set; }
        public string LegalArchRequiredForMISCRecInv { get; set; }
        public string IncludeListingsMISCRecArch { get; set; }
        //CMP #666: MISC Legal Archiving Per Location ID
        public string MiscRecArchLocReq {get;set;}
        public string MiscRecArchLocs { get; set;}
        public string LegalArchRequiredForMISCPayInv { get; set; }
        public string IncludeListingsMISCPayArch { get; set; }
        //CMP #666: MISC Legal Archiving Per Location ID
        public string MiscPayArchLocReq { get; set; }
        public string MiscPayArchLocs { get; set; }
        public string MISCAllowedFileTypesForSupportingDocuments { get; set; }
        public string MISCBilledInvoiceXMLOutput { get; set; }

        //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
        public string MiscDailyPayableOptionsForBilateralInvoices { get; set; } 
        public string MiscDailyPayableOfflineAchiveOutputs { get; set; }
        public string MiscDailyPayableIsXmlFiles { get; set; } 

        public string MISCIsPdfAsOtherOutputAsBilledEntity { get; set; }
        public string MISCIsDetailListingAsOtherOutputAsBilledEntity { get; set; }
        public string MISCIsSuppDocAsOtherOutputAsBilledEntity { get; set; }
        public string MISCIsDSFileAsOtherOutputAsBilledEntity { get; set; }
        public string MISCIINetAccountId { get; set; }
        public string NewUserEmailAddress { get; set; }
        public string NewUserFirstName { get; set; }
        public string NewUserLastName { get; set; }
        public string NewUserType { get; set; }
        public string PermissionTemplateForNewUser { get; set; }
        
        public string EntryDate { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Location { get; set; }
        public int IsLocationActive { get; set; }
        public int UserId { get; set; }

        public int isLegalArchRequired { get; set; }
        public int isLegalArchRequiredForMISCRecInv { get; set; }
        public int isLegalArchRequiredForMISCPayInv { get; set; }
        public int isIncludeListingsMISCRecArch { get; set; }
        public int isIncludeListingsMISCPayArch { get; set; }
        public int isMISCBilledInvoiceXMLOutput { get; set; }

        //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
        public int IsMiscDailyPayableOptionsForBilateralInvoices { get; set; }
        public int IsMiscDailyPayableOfflineAchiveOutputs { get; set; }
        public int IsMiscDailyPayableIsXmlFiles { get; set; } 


        public int isMISCIsPdfAsOtherOutputAsBilledEntity { get; set; }
        public int isMISCIsDetailListingAsOtherOutputAsBilledEntity { get; set; }
        public int isMISCIsSuppDocAsOtherOutputAsBilledEntity { get; set; }
        public int isMISCIsDSFileAsOtherOutputAsBilledEntity { get; set; }
        public int isDigitalSignApplication { get; set; }
        public int isDigitalSignVerification { get; set; }
        public int NewUserTypeId { get; set; }
        public string IsFileLogId { get; set; }
        public string MemberType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfileDataFile
{
  public  class CompleteReferenceDataModel
  {
    [DisplayName("Serial No.")]
    public int SerialNo { get; set; }
    
    [DisplayName("Participant Code")]
    public string ParticipantID { get; set; }

    [DisplayName("Location Id")]
    public string LocationId { get; set; }

    [DisplayName("Active")]
    public string Active { get; set; }

    [DisplayName("Member Legal Name")]
    public string CompanyLegalName { get; set; }

    [DisplayName("Tax Vat Registration Id")]
    public string TaxVatRegistrationID { get; set; }

    [DisplayName("Additional Tax Vat Registration Id")]
    public string AdditionalTaxVatRegistrationId { get; set; }

    [DisplayName("Company Registration Id")]
    public string CompanyRegistrationID { get; set; }

    [DisplayName("Address Line 1")]
    public string AddressLine1 { get; set; }

    [DisplayName("Address Line 2")]
    public string AddressLine2 { get; set; }

    [DisplayName("Address Line 3")]
    public string AddressLine3 { get; set; }

    [DisplayName("City Name")]
    public string CityName { get; set; }

    [DisplayName("Subdivision Code")]
    public string SubdivisionCode { get; set; }

    [DisplayName("Subdivision Name")]
    public string SubdivisionName { get; set; }

    [DisplayName("Country Code")]
    public string CountryCode { get; set; }

    [DisplayName("Country Name")]
    public string CountryName { get; set; }

    [DisplayName("Postal Code")]
    public string PostalCode { get; set; }
  }
}

using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Common
{
  public class MemberLocationInformation : EntityBase<Guid>
  {
    public Guid InvoiceId { get; set; }

    public string OrganizationName { get; set; }

    public string OrganizationDesignator { get; set; }

    public Guid LocationId { get; set; }

    public string TaxRegistrationId { get; set; }

    public string CompanyRegistrationId { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string AddressLine3 { get; set; }

    public string CityName { get; set; }

    public string SubdivisionCode { get; set; }

    public string SubdivisionName { get; set; }

    public string CountryCode { get; set; }

    public string CountryName { get; set; }

    public string PostalCode { get; set; }

    public DigitalSignatureRequired DigitalSignatureRequired
    {
      get
      {
        return (DigitalSignatureRequired) DigitalSignatureRequiredId;
      }
      set
      {
        DigitalSignatureRequiredId = Convert.ToInt32(value);
      }
    }

    public int DigitalSignatureRequiredId { get; set; }

    public string LegalText { get; set; }

    public bool IsBillingMember { get; set; }

    public string MemberLocationCode { get; set; }

    public InvoiceBase Invoice { get; set; }

    public string AdditionalTaxVatRegistrationNumber { get; set; }

    public string CountryCodeIcao { get; set; }

    /// <summary>
    /// Gets or sets the line item additional details.
    /// </summary>
    /// <value>The line item additional details.</value>
    public List<MemberLocationInfoAdditionalDetail> MemberLocationInfoAdditionalDetails { get; private set; }

    public MemberLocationInformation()
    {
      MemberLocationInfoAdditionalDetails = new List<MemberLocationInfoAdditionalDetail>();
    }
  }
}
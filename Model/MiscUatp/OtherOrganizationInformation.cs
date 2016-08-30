using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.MiscUatp
{
  public class OtherOrganizationInformation : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the PO line item number.
    /// </summary>
    /// <value>The PO line item number.</value>
    public string LegalName { get; set; }

    public string RegistrationId { get; set; }

    public string TaxVatRegistrationId { get; set; }

    public string AddTaxVatRegistrationNumber { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string AddressLine3 { get; set; }

    public string CityName { get; set; }

    public string SubDivisionCode { get; set; }

    public string SubDivisionName { get; set; }

    public string PostalCode { get; set; }

    public string CountryCode { get; set; }

    public string LegalText { get; set; }

    public string Iban { get; set; }

    public string Swift { get; set; }

    public string BankCode { get; set; }

    public string BankName { get; set; }

    public string BranchCode { get; set; }

    public string BankAccountNumber { get; set; }

    public string BankAccountName { get; set; }

    public int? CurrencyId { get; set; }

    public Currency Currency { get; set; }

    /// <summary>
    /// Gets or sets the type of the organization.
    /// </summary>
    /// <value>The type of the organization.</value>
    public int OrganizationTypeId { get; set; }

    /// <summary>
    /// Gets or sets the type of the organization
    /// </summary>
    /// <value>The type of the organization.</value>
    public OtherOrganizationType OtherOrganizationType
    {
      get
      {
        return (OtherOrganizationType)OrganizationTypeId;
      }
      set
      {
        OrganizationTypeId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Gets or sets the organization id.
    /// </summary>
    /// <value>The organization id.</value>
    public string OrganizationId { get; set; }

    /// <summary>
    /// Gets or sets the organization designator.
    /// </summary>
    /// <value>The organization designator.</value>
    public string OrganizationDesignator { get; set; }

    /// <summary>
    /// Gets or sets the country code icao.
    /// </summary>
    /// <value>The country code icao.</value>
    public string CountryCodeIcao { get; set; }

    /// <summary>
    /// Gets or sets the name of the country.
    /// </summary>
    /// <value>The name of the country.</value>
    public string CountryName { get; set; }

    /// <summary>
    /// Gets or sets the name of the contact.
    /// </summary>
    /// <value>The name of the contact.</value>
    public string ContactName { get; set; }

    /// <summary>
    /// For Navigation to <see cref="MiscUatpInvoice"/>. 
    /// </summary>
    public Guid InvoiceId { get; set; }

    public MiscUatpInvoice Invoice { get; set; }

    public List<OtherOrganizationAdditionalDetail> OtherOrganizationAdditionalDetails { get; private set; }

    public List<OtherOrganizationContact> OtherOrganizationContactInformations { get; set; }

    public OtherOrganizationInformation()
    {
      OtherOrganizationAdditionalDetails = new List<OtherOrganizationAdditionalDetail>();
      OtherOrganizationContactInformations = new List<OtherOrganizationContact>();
    }

  }
}
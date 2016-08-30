using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Model.MiscUatp
{
  public class ContactInformation : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the contact type (i.e. Email, Phone number etc.).
    /// </summary>
    /// <value>The type.</value>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the contact value.
    /// </summary>
    /// <value>The value.</value>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the contact description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is billing member.
    /// </summary>
    /// <value>true if this instance is billing member; otherwise, false.</value>
    public int MemberTypeId { get; set; }

    public MemberType MemberType
    {
      get
      {
        return (MemberType)MemberTypeId;
      }
      set
      {
        MemberTypeId = Convert.ToInt32(value);
      }
    }

    

    /// <summary>
    /// Gets or sets the invoice id of <see cref="MiscUatpInvoice"/>.
    /// </summary>
    /// <value>The invoice id.</value>
    public Guid InvoiceId { get; set; }

    // Commented to remove circular dependency in invoice and member contacts.
    ///// <summary>
    ///// Navigational property for <see cref="MiscUatpInvoice"/>.
    ///// </summary>
    //public MiscUatpInvoice MiscUatpInvoice { get; set; }
  }
}
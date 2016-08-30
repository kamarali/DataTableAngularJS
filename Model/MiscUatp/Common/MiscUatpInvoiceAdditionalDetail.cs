using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class MiscUatpInvoiceAdditionalDetail : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public int TypeId { get; set; }

    /// <summary>
    /// Gets or sets the type of the additional detail.
    /// </summary>
    /// <value>The type of the additional detail.</value>
    public AdditionalDetailType AdditionalDetailType
    {
      get
      {
        return (AdditionalDetailType)TypeId;
      }
      set
      {
        TypeId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    /// <value>The parent id.</value>
    public Guid InvoiceId { get; set; }

    public int RecordNumber { get; set; }
  }
}
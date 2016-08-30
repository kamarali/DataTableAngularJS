using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class NotesAdditionalDeail : MasterBase<int>
  {
    public string Description { get; set; }

    public int TypeId { get; set; }

    /// <summary>
    /// Gets or sets the type of the invoice.
    /// </summary>
    /// <value>The type of the invoice.</value>
    public AdditionalDetailType InvoiceType
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
  }
}

using System.Collections.Generic;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Web.Areas.Misc.Models
{
  public class LineItemDetailViewModel
  {
    /// <summary>
    /// LineItemDetail object which is set as model for View
    /// </summary>
    public LineItemDetail LineItemDetail { get; set; }

    /// <summary>
    /// Metadata along with value for mandatory-recommended dynamic fields
    /// </summary>
    public IList<FieldMetaData> RequiredFields  { get; set; }

    /// <summary>
    /// Metadata along with value for optional dynamic fields
    /// </summary>
    public IList<FieldMetaData> OptionalFields { get; set; }
  }
}
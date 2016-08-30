using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class DynamicGroupDetail
  {
    /// <summary>
    /// Gets or sets the metadata id of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    public Guid FieldMetadataId { get; set; }

    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    public string FieldName { get; set; }


    /// <summary>
    /// Gets or sets the display text.
    /// </summary>
    /// <value>The display text.</value>
    public string DisplayText { get; set; }

    /// <summary>
    /// Gets or sets the max occurrence.
    /// </summary>
    /// <value>The max occurrence.</value>
    public int MaxOccurrence { get; set; }
  }
}

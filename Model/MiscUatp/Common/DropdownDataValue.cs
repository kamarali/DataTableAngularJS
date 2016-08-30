using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MiscUatp.Common
{
  /// <summary>
  /// This model class is used to retrieve dictionary based values for field of type dropdown in Dynamic fields
  /// </summary>
  public class DropdownDataValue
  {
    /// <summary>
    /// Display text for dropdown
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Represents Value 
    /// </summary>
    public string Value { get; set; }
  }
}

using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Common
{
  [Serializable]
  public class DataSourceValue : EntityBase<int>
  {
    /// <summary>
    /// Gets or sets the group id to reuse field values. 
    /// </summary>
    /// <value>The group id.</value>
    public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the data source value.
    /// </summary>
    /// <value>The value.</value>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the data source id.
    /// </summary>
    /// <value>The data source id.</value>
    public int DataSourceId { get; set; }

    public int DisplayOrder { get; set; }

    public string DisplayText { get; set; }

  }
}
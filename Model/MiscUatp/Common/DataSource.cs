using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Common
{
  [Serializable]
  public class DataSource : EntityBase<int>
  {
    /// <summary>
    /// Gets or sets the data source value.
    /// </summary>
    /// <value>The data source value.</value>
    public string _DataSourceValue { get; set; }

    /// <summary>
    /// Gets or sets the name of the table.
    /// </summary>
    /// <value>The name of the table.</value>
    public string TableName { get; set; }

    /// <summary>
    /// Gets or sets the display name of the column.
    /// </summary>
    /// <value>The display name of the column.</value>
    public string DisplayColumnName { get; set; }

    /// <summary>
    /// Gets or sets the name of the value column.
    /// </summary>
    /// <value>The name of the value column.</value>
    public string ValueColumnName { get; set; }

    /// <summary>
    /// Gets or sets the Where clause for master table data.
    /// </summary>
    /// <value>Where clause.</value>
    public string WhereClause { get; set; }

    public string SubstituteValue { get; set; }

    /// <summary>
    /// Gets or sets the data source values.
    /// </summary>
    /// <value>The data source values.</value>
    public IList<DataSourceValue> DataSourceValues { get; set; }

    public DataSource()
    {
      DataSourceValues = new List<DataSourceValue>();
    }
  }
}
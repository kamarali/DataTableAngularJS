using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Model.MiscUatp.Common
{
  [Serializable]
  public class FieldMetaData : EntityBase<Guid>, ICacheable
  {
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
    /// Gets or sets the control type id.
    /// </summary>
    /// <value>The control type id.</value>
    public int ControlTypeId { get; set; }

    /// <summary>
    /// Gets or sets the type of the control.
    /// </summary>
    /// <value>The type of the control.</value>
    public ControlType ControlType
    {
      get
      {
        return (ControlType)ControlTypeId;
      }
      set
      {
        ControlTypeId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Gets or sets the name of the parent tag.
    /// </summary>
    /// <value>The name of the parent tag.</value>
    public string ParentTagName { get; set; }

    /// <summary>
    /// Gets or sets the required type id. Fetched from association table and set during load strategy.
    /// </summary>
    /// <value>The required type id.</value>
    public int RequiredTypeId { get; set; }

    /// <summary>
    /// Gets or sets the type of the required.
    /// </summary>
    /// <value>The type of the required.</value>
    public RequiredType RequiredType
    {
      get
      {
        return (RequiredType)RequiredTypeId;
      }
      set
      {
        RequiredTypeId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    /// <value>The display order.</value>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the max occurrence.
    /// </summary>
    /// <value>The max occurrence.</value>
    public int MaxOccurrence { get; set; }

    /// <summary>
    /// Gets or sets the min occurrence.
    /// </summary>
    /// <value>The min occurrence.</value>
    public int MinOccurrence { get; set; }

    /// <summary>
    /// Gets or sets the type of the data.
    /// </summary>
    /// <value>The type of the data.</value>
    public int DataTypeId { get; set; }

    /// <summary>
    /// Gets or sets the type of the data.
    /// </summary>
    /// <value>The type of the data.</value>
    public DataType DataType
    {
      get
      {
        return (DataType)DataTypeId;
      }
      set
      {
        DataTypeId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Gets or sets the length of the data.
    /// </summary>
    /// <value>The length of the data.</value>
    public string DataLength { get; set; }

    /// <summary>
    /// Gets or sets the attributes.
    /// </summary>
    /// <value>The attributes.</value>
    public List<FieldMetaData> SubFields { get; private set; }

    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    /// <value>The parent id.</value>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent meta data.
    /// </summary>
    /// <value>The parent meta data.</value>
    public FieldMetaData ParentMetaData { get; set; }

    /// <summary>
    /// Gets or sets the field values.
    /// </summary>
    /// <value>The field values.</value>
    public List<FieldValue> FieldValues { get; private set; }
    
    /// <summary>
    /// Gets or sets the data source id.
    /// </summary>
    /// <value>The data source id.</value>
    public int? DataSourceId { get; set; }

    /// <summary>
    /// Navigation property for <see cref="DataSource"/>.
    /// </summary>
    /// <value>The data source.</value>
    public DataSource DataSource { get; set; }

    /// <summary>
    /// Field Type Id stored in DB
    /// </summary>
    public int FieldTypeId { get; set; }

    /// <summary>
    /// Get Field Type of node
    /// </summary>
    public FieldType FieldType
    {
      get
      {
        return (FieldType)FieldTypeId;
      }
      set
      {
        FieldTypeId = Convert.ToInt32(value);
      }
    }

    public string FieldTypeXml
    {
      set
      {
        if (string.Compare(value.ToLower(), Enum.GetName(typeof(FieldType), Enums.FieldType.Field)) == 0)
        {
          FieldTypeId = Convert.ToInt32(Enums.FieldType.Field);
        }
        else if (string.Compare(value.ToLower(), Enum.GetName(typeof(FieldType), Enums.FieldType.Attribute)) == 0)
        {
          FieldTypeId = Convert.ToInt32(Enums.FieldType.Attribute);
        }
        else
        {
          FieldTypeId = Convert.ToInt32(Enums.FieldType.Group);
        }
      }
    }

    /// <summary>
    /// Level of field
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Get whether field has attributes associated 
    /// </summary>
    public bool HasAttributes
    {
      get
      {
        return SubFields != null && SubFields.Count > 0;
      }
    }

    /// <summary>
    /// Return whether field should appear multiple times
    /// </summary>
    public bool IsMultipleOccurrence
    {
      get
      {
        return MinOccurrence > 1 || MaxOccurrence > 1;
      }
    }

    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the field charge code mappings.
    /// </summary>
    /// <value>The field charge code mappings.</value>
    public List<FieldChargeCodeMapping> FieldChargeCodeMappings { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldMetaData"/> class.
    /// </summary>
    public FieldMetaData()
    {
      SubFields = new List<FieldMetaData>();
      FieldValues = new List<FieldValue>();
      FieldChargeCodeMappings = new List<FieldChargeCodeMapping>();
    }
  }
}

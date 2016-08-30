using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Common
{
  [Serializable]
  public class FieldValue : EntityBase<Guid>
  {
    public const string DynamicGroupCountString = "_Grp";
    public const string DynamicFieldValueSuffix = "_DFValue";

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    /// <value>The parent id.</value>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the attribute values.
    /// </summary>
    /// <value>The attribute values.</value>
    public List<FieldValue> AttributeValues { get; private set; }

    /// <summary>
    /// Gets or sets the parent value.
    /// </summary>
    /// <value>The parent value.</value>
    public FieldValue ParentValue { get; set; }

    /// <summary>
    /// Gets or sets the line item detail id.
    /// </summary>
    /// <value>The line item detail id.</value>
    public Guid LineItemDetailId { get; set; }

    /// <summary>
    /// Gets or sets the line item detail.
    /// </summary>
    /// <value>The line item detail.</value>
    public LineItemDetail LineItemDetail { get; set; }

    /// <summary>
    /// Gets or sets the field meta data id.
    /// </summary>
    /// <value>The field meta data id.</value>
    public Guid FieldMetaDataId { get; set; }

    /// <summary>
    /// Navigational property for <see cref="FieldMetaData"/>.
    /// </summary>
    /// <value>The field meta data.</value>
    public FieldMetaData FieldMetaData { get; set; }

    /// <summary>
    /// This field is used to store control Id for field on UI. Used to convert values from UI to fieldValue object in case of multiple occurrence.
    /// </summary>
    public string ControlId { get; set; }

    /// <summary>
    /// Return values of field Count
    /// </summary>
    public string ControlIdCount
    {
      get
      {
      //TODO: Change this property as extension method
        if (ControlId != null)
        {
          var idIndex = ControlId.IndexOf(DynamicFieldValueSuffix) + DynamicFieldValueSuffix.Length;
          return ControlId.Substring(idIndex, ControlId.Length - idIndex);
        }
        return string.Empty;
      }
    }

    /// <summary>
    /// Return values of Group Count which field belong to
    /// </summary>
    public string ControlIdGroupCount
    {
      get
      {
        //TODO: Change this property as extension method
        if (ControlId != null)
        {
          if (ControlId.Contains(DynamicGroupCountString))
          {
            var idIndex = ControlId.IndexOf(DynamicGroupCountString) + DynamicGroupCountString.Length;
            return ControlId.Substring(idIndex, ControlId.LastIndexOf("_") - idIndex);
          }
          else
            return string.Empty;
        }
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the parent id to be used in CSV no binding with d/b field
    /// </summary>
    /// <value>The parent id.</value>
    public string RecordParentId { get; set; }

    /// <summary>
    /// This field is used to store Id for field to be used in CSV no binding with d/b field
    /// </summary>
    public string RecordId { get; set; }

    public FieldValue()
    {
      AttributeValues = new List<FieldValue>();
    }
  }
}
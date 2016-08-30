using System;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Extensions
{
  public class Materializers
  {
    public readonly Materializer<FieldMetaData> FieldMetaDataMaterializer = new Materializer<FieldMetaData>(r =>
        new FieldMetaData
        {
          //TODO : Check null object for all fields excluding string, null able data types
          //Currently commenting rest of the properties as it do not reflect with existing mapping.
          Id = r.Field<byte[]>("FIELD_METADATA_ID") != null ? new Guid(r.Field<byte[]>("FIELD_METADATA_ID")) : new Guid(),
          ParentId = r.Field<byte[]>("PARENT_ID") != null ? new Guid(r.Field<byte[]>("PARENT_ID")) : new Guid(),
          FieldName = r.Field<string>("FIELD_NAME"),
          DataSourceId = r.Field<object>("DATA_SOURCE_ID") != null ? r.Field<int>("DATA_SOURCE_ID") : 0,
          DisplayText = r.Field<string>("DISPLAY_TEXT"),
          ControlTypeId = r.Field<object>("CONTROL_TYPE_ID") != null ? r.Field<int>("CONTROL_TYPE_ID") : 0,
          ParentTagName = r.Field<string>("PARENT_TAG_NAME"),
          DisplayOrder = r.Field<object>("DISPLAY_ORDER") != null ? r.Field<int>("DISPLAY_ORDER") : 0,
          MaxOccurrence = r.Field<object>("OCCURRENCE_MAX_VALUE") != null ? r.Field<int>("OCCURRENCE_MAX_VALUE") : 0,
          MinOccurrence = r.Field<object>("OCCURRENCE_MIN_VALUE") != null ? r.Field<int>("OCCURRENCE_MIN_VALUE") : 0,
          DataTypeId = r.Field<object>("DATA_TYPE_ID") != null ? r.Field<int>("DATA_TYPE_ID") : 0,
          DataLength = r.Field<string>("DATA_LENGTH"),
          CssClass = r.Field<object>("CSS_CLASS") != null ? r.Field<string>("CSS_CLASS") : string.Empty,
          FieldTypeId = r.Field<object>("FIELD_TYPE") != null ? r.Field<int>("FIELD_TYPE") : 0,
          Level = r.Field<object>("GROUP_LEVEL") != null ? r.Field<int>("GROUP_LEVEL") : 0,
          //RequiredTypeId = r.Field<object>("REQUIRED_TYPE_ID") != null ? r.Field<int>("REQUIRED_TYPE_ID") : 0,
          LastUpdatedOn = r.Field<DateTime>("LAST_UPDATED_ON"),
        });

    public readonly Materializer<FieldValue> FieldValueMaterializer = new Materializer<FieldValue>(r =>
        new FieldValue
        {
          //TODO : Check null object for all fields excluding string, null able data types
          //Currently commenting rest of the properties as it do not reflect with existing mapping.
          Id = r.Field<byte[]>("FIELD_VALUE_ID") != null ? new Guid(r.Field<byte[]>("FIELD_VALUE_ID")) : new Guid(),
          ParentId = r.Field<byte[]>("PARENT_ID") != null ? new Guid(r.Field<byte[]>("PARENT_ID")) : new Guid(),
          LineItemDetailId = r.Field<byte[]>("LINE_ITEMDETAIL_ID") != null ? new Guid(r.Field<byte[]>("LINE_ITEMDETAIL_ID")) : new Guid(),
          FieldMetaDataId = r.Field<byte[]>("FIELD_METADATA_ID") != null ? new Guid(r.Field<byte[]>("FIELD_METADATA_ID")) : new Guid(),
          Value = r.Field<string>("FIELD_VALUE"),
          LastUpdatedOn = r.Field<DateTime>("LAST_UPDATED_ON"),
        });
  }
}

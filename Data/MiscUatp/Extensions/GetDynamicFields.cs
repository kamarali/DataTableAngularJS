using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;

namespace Iata.IS.Data.MiscUatp.Extensions
{
  public class GetDynamicFields : StoredProcedure
  {
    public const string MetadataGroup = "MetadataGroup";
    public const string MetadataField = "MetadataField";
    public const string MetadataAttribute = "MetadataAttribute";
    public const string FieldValue = "FieldValue";

    public override string Name
    {
      get
      {
        return "PROC_GET_MU_FIELD_VALUE";
      }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new[] { 
                new SPResultObject{ EntityName = MetadataGroup, IsMain=true, ParameterName="r_cur_Field_Metadata_Group_O"},
                new SPResultObject{ EntityName=MetadataField, ParameterName="r_cur_Field_Metadata_Field_O"},
                new SPResultObject{EntityName = MetadataAttribute, ParameterName = "r_cur_Field_Metadata_Attr_O"}, 
                new SPResultObject{EntityName = FieldValue, ParameterName = "r_cur_FieldValue_O"}, 
            });
    }

  }

  public class GetDynamicFieldsForGroup : StoredProcedure
  {
    public const string MetadataGroup = "MetadataGroup";
    public const string MetadataField = "MetadataField";
    public const string MetadataAttribute = "MetadataAttribute";

    public override string Name
    {
      get
      {
        return "PROC_GET_MU_FIELD_METADATA";
      }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new[] { 
                new SPResultObject{ EntityName = MetadataGroup, IsMain=true, ParameterName="r_cur_Field_Metadata_Group_O"},
                new SPResultObject{ EntityName=MetadataField, ParameterName="r_cur_Field_Metadata_Field_O"},
                new SPResultObject{EntityName = MetadataAttribute, ParameterName = "r_cur_Field_Metadata_Attr_O"}, 
            });
    }

  }

  public class GetDynamicFieldsForOptionalGroup : StoredProcedure
  {
    public const string MetadataGroup = "MetadataGroup";
    public const string MetadataField = "MetadataField";
    public const string MetadataAttribute = "MetadataAttribute";

    public override string Name
    {
      get
      {
        return "PROC_GET_MU_OPT_FIELD_METADATA";
      }
    }

    public override List<SPResultObject> GetResultSpec()
    {
      return new List<SPResultObject>(new[] { 
                new SPResultObject{ EntityName = MetadataGroup, IsMain=true, ParameterName="r_cur_Field_Metadata_Group_O"},
                new SPResultObject{ EntityName=MetadataField, ParameterName="r_cur_Field_Metadata_Field_O"},
                new SPResultObject{EntityName = MetadataAttribute, ParameterName = "r_cur_Field_Metadata_Attr_O"}, 
            });
    }

  }
}

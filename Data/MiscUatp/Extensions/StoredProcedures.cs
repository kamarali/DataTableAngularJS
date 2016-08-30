using Iata.IS.Data.Impl;

namespace Iata.IS.Data.MiscUatp.Extensions
{
  public class StoredProcedures
  {
    public static StoredProcedure GetFieldMetadata = new GetDynamicFields();
    public static StoredProcedure GetFieldMetadataForGroup = new GetDynamicFieldsForGroup();
    public static StoredProcedure GetDynamicFieldsForOptionalGroup = new GetDynamicFieldsForOptionalGroup();
  }
}

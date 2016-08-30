namespace Iata.IS.Core.File
{
  /// <summary>
  /// Constants related to CSV processing.
  /// </summary>
  internal class Constants
  {
    private Constants()
    {
    }

    public const string ConstIncludeChild = "IncludeChild";
    public const string ConstChildClass = "ChildClass";
    public const string ConstName = "Name";
    public const string ConstIdField = "RecordId";
    public const string ConstParent = "Parent";
    public const string ConstParentIdField = "ParentId";
    public const string ConstGenerateCsv = "GenerateCSV";
    public const string ConstStringType = "System.String";
    public const string ConstDatetimeType = "System.DateTime";
    public const string ConstDecimalType = "System.Decimal";
    public const string ConstBoolType = "System.Boolean";
    public const string ConstGuidType = "System.Guid";
    public const string ConstNullable = "System.Nullable`1";
  }
}
using System.Collections.Generic;

namespace Iata.IS.Core.ValidationErrorCorrection
{
  public class ErrorCorrectionExceptionCode
  {
    public string ExceptionCode { get; set; }

    public string ValidationType { get; set; }

    public string MasterTableName { get; set; }

    public string MasterColumnName { get; set; }

    public string MasterGroupId { get; set; }

    public string MasterGroupColumnName { get; set; }

    /// <summary>
    /// To store reqular expression
    /// </summary>
    public string RegularExpression { get; set; }

    /// <summary>
    /// To identify different formates of the value
    /// e.g: DateTime : for datetime format
    /// </summary>
    public string FieldType { get; set; }

    public List<ErrorLevelModel> ErrorLevels { get; set; }

    public int BillingCategory { get; set; }

    public ErrorCorrectionExceptionCode()
    {
      ErrorLevels = new List<ErrorLevelModel>();
    }
  }

  public class ErrorLevelModel
  {
    public string ErrorLevelName { get; set; }

    public string ChildTableName { get; set; }

    public string ColumnName { get; set; }

    public string PrimaryColumnName { get; set; }
  }
}

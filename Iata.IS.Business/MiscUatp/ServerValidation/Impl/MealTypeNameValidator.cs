using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  /// <summary>
  /// Meal Type Validator class
  /// </summary>
  /// CMP #636: Standard Update Mobilization
  public class MealTypeNameValidator : IServerValidator
  {
    private const string MealType = "MealType";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      bool isValid = true;
      validationError = null;
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var mealTypeFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == MealType).ToList();

        foreach (var metaData in mealTypeFields)
        {
          foreach (var field in metaData.FieldValues)
          {
            //If attribute value is not provided then raise error
            if (field.AttributeValues.Count == 0)
            {
              validationError = new DynamicValidationError
              {
                ErrorCode = MiscErrorCodes.InvalidMealTypeName,
                ErrorDescription = Messages.ResourceManager.GetString(MiscErrorCodes.InvalidMealTypeName)
              };

              isValid = false;
            }
            /* Attribute value provided but parent field value not provided.*/
            else if (field.Value == null)
            {
              validationError = new DynamicValidationError { ErrorCode = MiscUatpErrorCodes.FieldValueRequiredForAttribute, ErrorDescription = string.Format(Messages.ResourceManager.GetString(MiscUatpErrorCodes.FieldValueRequiredForAttribute), field.FieldMetaData.DisplayText) };
              isValid = false;
            }
          }
        }
      }

      return isValid;
    }
  }
}

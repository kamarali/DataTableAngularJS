using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  public class MiscDataAtNameValidator : IServerValidator
  {
    private const string DynamicGroupName = "MiscDetails";
    private const string DynamicFieldName = "MiscData";
    private const string DynamicAttributeName = "Name";
    private const string DynamicAttributeUOMCode = "UOMCode";

    /// <summary>
    /// Server side validator for the attribute "Name" of dynamic field "MiscData" at Line item details level.
    /// </summary>
    /// <param name="fieldMetaDataList"> Field Meta Data List </param>
    /// <param name="miscUatpErrorCodes"> Misc Uatp Error Codes </param>
    /// <param name="validationError"> Validation Error </param>
    /// <returns>true or false </returns>
    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes miscUatpErrorCodes, out DynamicValidationError validationError)
    {
      validationError = null;
      bool isValid = true;

      var miscDataFieldMetaData = fieldMetaDataList.Where(fieldMetaData => fieldMetaData.FieldName == DynamicGroupName);

      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var mealTypeFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == DynamicFieldName).ToList();

        foreach (var metaData in mealTypeFields)
        {
          foreach (var field in metaData.FieldValues)
          {
            var nameField = field.AttributeValues.Where(f => f.FieldMetaData.FieldName == DynamicAttributeName).FirstOrDefault();
            var uomCodeField = field.AttributeValues.Where(f => f.FieldMetaData.FieldName == DynamicAttributeUOMCode).FirstOrDefault();

            //If UOM code is provided but attribute name and misc data field is not provided.
            if ((uomCodeField != null && !string.IsNullOrWhiteSpace(uomCodeField.Value)) && (nameField == null || (nameField != null && string.IsNullOrWhiteSpace(nameField.Value))) && string.IsNullOrWhiteSpace(field.Value))
            {
              validationError = new DynamicValidationError
              {
                ErrorCode = miscUatpErrorCodes.FieldMiscDataAndAttributeIsRequiredForTheAttribute,
                ErrorDescription = string.Format(Messages.ResourceManager.GetString(miscUatpErrorCodes.FieldMiscDataAndAttributeIsRequiredForTheAttribute), DynamicAttributeUOMCode)
              };
              isValid = false;
            }
            //If attribute value is not provided then raise error.
            else if (nameField == null || (nameField != null && string.IsNullOrWhiteSpace(nameField.Value)))
            {
              validationError = new DynamicValidationError
              {
                ErrorCode = miscUatpErrorCodes.NameAttributeIsRequiredForTheFieldMiscData,
                ErrorDescription = string.Format(Messages.ResourceManager.GetString(miscUatpErrorCodes.NameAttributeIsRequiredForTheFieldMiscData), DynamicAttributeName)
              };
              isValid = false;
            }
            /* Attribute value provided but parent field value not provided.*/
            else if (string.IsNullOrWhiteSpace(field.Value))
            {
              validationError = new DynamicValidationError
              {
                ErrorCode = miscUatpErrorCodes.FieldMiscDataIsRequiredForTheAttribute,
                ErrorDescription = string.Format(Messages.ResourceManager.GetString(miscUatpErrorCodes.FieldMiscDataIsRequiredForTheAttribute), DynamicFieldName)
              };
              isValid = false;
            }
          }
        }
      }

      return isValid;
    }
  }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Business.Pax;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  public class DistanceValidator : IServerValidator
  {
    private const string Distance = "Distance";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      bool isValid = true;
      validationError = null;
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var distanceFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == Distance).ToList();

        foreach (var metaData in distanceFields)
        {
          foreach (var field in metaData.FieldValues)
          {
            if (field.AttributeValues.Count == 0)
            {
              validationError = new DynamicValidationError
              {
                ErrorCode = MiscUatpErrorCodes.InvalidDistanceUomcode,
                ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidDistanceUomcode)
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

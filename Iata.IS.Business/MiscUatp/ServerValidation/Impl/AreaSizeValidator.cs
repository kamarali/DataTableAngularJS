﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  public class AreaSizeValidator : IServerValidator
  {
    private const string ReferenceNumber = "AreaSize";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      bool isValid = true;
      validationError = null;
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var locationFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == ReferenceNumber).ToList();

        foreach (var metaData in locationFields)
        {
          foreach (var field in metaData.FieldValues)
          {
            if (field.AttributeValues.Count == 0)
            {
              validationError = new DynamicValidationError
              {
                ErrorCode = MiscUatpErrorCodes.InvalidAreaSizeUomcode,
                ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidAreaSizeUomcode)
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

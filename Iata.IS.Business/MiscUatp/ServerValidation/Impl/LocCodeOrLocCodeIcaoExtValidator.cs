using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  public class LocCodeOrLocCodeIcaoExtValidator : IServerValidator
  {
    private const string LocationCode = "LocationCode";
    private const string LocationcodeIcao = "LocationCode_ICAO";

    /// <summary>
    /// This function is used to validate Location Code and Location Code ICAO. 
    /// This will be used to validate when combination of charge category and charge code ATC/Approach and ATC/En-Route for Route Details group.
    /// For ATC- Approach and ATC-En-route we need to switch Location_Code@Type or Location_Code_ICAO@Type to recommended.
    /// The check should be that it is recommended but if provided only one should be provided.
    /// </summary>
    /// <param name="fieldMetaDataList"></param>
    /// <param name="MiscUatpErrorCodes"></param>
    /// <param name="validationError"></param>
    /// <returns></returns>
    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      var isValid = true;
      validationError = null;
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var filteredFieldMetaData = fieldMetaData.SubFields.Count(subField => subField.FieldName == LocationCode || subField.FieldName == LocationcodeIcao) > 0;
        if (!filteredFieldMetaData) continue;

        var locationFields = fieldMetaData.SubFields.Where(subField => subField.FieldName == LocationCode || subField.FieldName == LocationcodeIcao);

        foreach (var fieldValue in fieldMetaData.FieldValues)
        {
          var locationFieldCount = fieldValue.AttributeValues.Count(attr => attr.FieldMetaData.FieldName == LocationCode);
          var locationIcaoFieldCount = fieldValue.AttributeValues.Count(attr => attr.FieldMetaData.FieldName == LocationcodeIcao);
          
          if (locationFieldCount == 1 && locationIcaoFieldCount == 1)
          {
            validationError = new DynamicValidationError
            {
              ErrorCode = MiscUatpErrorCodes.LocationOrLocationCodeIcaoRequired,
              ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.LocationOrLocationCodeIcaoRequired)
            };
            isValid = false;
          }
        }
      }

      return isValid;
    }
  }
}

using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  /// <summary>
  /// If field type is Mandatory or RA, then either 1 should be provided.
  /// If field type of both is Recommended, then only validation will be that both are not provided together.
  /// </summary>
  public class LocationCodeOrLocationCodeIcaoValidator : IServerValidator
  {
    private const string LocationCode = "LocationCode";
    private const string LocationcodeIcao = "LocationCode_ICAO";
    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      var isValid = true;
      validationError = null;
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var filteredFieldMetaData = fieldMetaData.SubFields.Count(subField => subField.FieldName == LocationCode || subField.FieldName == LocationcodeIcao) > 0;
        if (!filteredFieldMetaData) continue;

        var locationFields = fieldMetaData.SubFields.Where(subField => subField.FieldName == LocationCode || subField.FieldName == LocationcodeIcao);


        if (fieldMetaData.FieldValues.Count() == 0)
        {
          //SCP306471 - KAL: Fields not being validated during file loading(Below if condition has been commented)
          // Do below validation only for fields with required type 'Recommended Always' or 'Mandatory'.
          // Fields are 'Recommended Always' in database even though they are Mandatory in IS-XML Standard 3.2
          // if (locationFields.Count(field => field.RequiredTypeId == (int)Model.MiscUatp.Enums.RequiredType.Mandatory || field.RequiredTypeId == (int)Model.MiscUatp.Enums.RequiredType.RecommendedAlways) > 0)
          // {
            validationError = new DynamicValidationError
                                {
                                  ErrorCode = MiscUatpErrorCodes.LocationOrLocationCodeIcaoRequired,
                                  ErrorDescription =
                                    Messages.ResourceManager.GetString(
                                      MiscUatpErrorCodes.LocationOrLocationCodeIcaoRequired)
                                };
            isValid = false;
         // }
        }
        foreach (var fieldValue in fieldMetaData.FieldValues)
        {
          var locationFieldCount = fieldValue.AttributeValues.Count(attr => attr.FieldMetaData.FieldName == LocationCode);
          var locationIcaoFieldCount = fieldValue.AttributeValues.Count(attr => attr.FieldMetaData.FieldName == LocationcodeIcao);
          if (locationFieldCount == 0 && locationIcaoFieldCount == 0)
          {
            //SCP306471 - KAL: Fields not being validated during file loading(Below if condition has been commented)
            // Do below validation only for fields with required type 'Recommended Always' or 'Mandatory'.
            // Fields are 'Recommended Always' in database even though they are Mandatory in IS-XML Standard 3.2
            // if (locationFields.Count(field => field.RequiredTypeId == (int)Model.MiscUatp.Enums.RequiredType.Mandatory || field.RequiredTypeId == (int)Model.MiscUatp.Enums.RequiredType.RecommendedAlways) > 0)
            //  {
              validationError = new DynamicValidationError
                                  {
                                    ErrorCode = MiscUatpErrorCodes.LocationOrLocationCodeIcaoRequired,
                                    ErrorDescription =
                                      Messages.ResourceManager.GetString(
                                        MiscUatpErrorCodes.LocationOrLocationCodeIcaoRequired)
                                  };
              isValid = false;
         //   }
          }
          else if (locationFieldCount == 1 && locationIcaoFieldCount == 1)
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
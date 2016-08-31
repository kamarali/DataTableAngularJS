using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  public class AircraftTypeCodeOrAircraftTypeCodeIcaoValidator : IServerValidator
  {
    private const string AircraftTypeCode = "AircraftTypeCode";
    private const string AircraftTypeCodeIcao = "AircraftTypeCode_ICAO";
    
    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      var isValid = true;
      validationError = null;
      foreach (var fieldMetaData in fieldMetaDataList)
      {

        var filteredFieldMetaData = fieldMetaData.SubFields.Count(subField => subField.FieldName == AircraftTypeCode || subField.FieldName == AircraftTypeCodeIcao) > 0;
        if (!filteredFieldMetaData) continue;

        if (fieldMetaData.FieldValues.Count() == 0)
        {
          validationError = new DynamicValidationError
          {
            ErrorCode = MiscUatpErrorCodes.AircraftTypeCodeOrAircraftTypeCodeIcaoRequired,
            ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.AircraftTypeCodeOrAircraftTypeCodeIcaoRequired)
          };
          isValid = false;
        }

        foreach (var fieldValue in fieldMetaData.FieldValues)
        {
          var aircraftTypeFieldCount = fieldValue.AttributeValues.Count(attr => attr.FieldMetaData.FieldName == AircraftTypeCode);
          var aircraftTypeIcaoFieldCount = fieldValue.AttributeValues.Count(attr => attr.FieldMetaData.FieldName == AircraftTypeCodeIcao);
          if (aircraftTypeFieldCount == 0 && aircraftTypeIcaoFieldCount == 0)
          {
            validationError = new DynamicValidationError
                                {
                                  ErrorCode = MiscUatpErrorCodes.AircraftTypeCodeOrAircraftTypeCodeIcaoRequired,
                                  ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.AircraftTypeCodeOrAircraftTypeCodeIcaoRequired),
                                  FieldValue = fieldValue.Value
                                };
            isValid = false;
          }
          else if (aircraftTypeFieldCount == 1 && aircraftTypeIcaoFieldCount == 1)
          {
              validationError = new DynamicValidationError
              {
                  ErrorCode = MiscUatpErrorCodes.AircraftTypeCodeOrAircraftTypeCodeIcaoRequired,
                  ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.AircraftTypeCodeOrAircraftTypeCodeIcaoRequired)
              };
              isValid = false;
          }
        }
      }

      return isValid;
    }
  }
}
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl 
{
  class AircraftDetailValidator : IServerValidator
  {
    private const string DynamicGroupCode = "AircraftDetails";
    private const string DynamicFieldName = "MaxTakeOffWeight";
    private const string DynamicAttributeName = "UOMCode";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      var isValid = true;
      validationError = null;

      var aircraftFieldMetaData = fieldMetaDataList.Where(fieldMetaData => fieldMetaData.FieldName == DynamicGroupCode);

      foreach (var groupMetaData in aircraftFieldMetaData)
      {
        foreach (var fieldMetaData in groupMetaData.SubFields.Where(subFields => subFields.FieldName == DynamicFieldName && subFields.HasAttributes))
        {
          var count = fieldMetaData.FieldValues.Count();

          if (fieldMetaData.SubFields.Count > 0)
          {
            foreach (var attribField in fieldMetaData.SubFields.Where(attribFields => attribFields.FieldName == DynamicAttributeName))
            {
              if (count != attribField.FieldValues.Count)
              {
                validationError = new DynamicValidationError {
                                                               ErrorCode = MiscUatpErrorCodes.UomCodeRequired,
                                                               ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.UomCodeRequired)
                                                             };

                isValid = false;
              }
            }
          }
        }
      }

      return isValid;
    }
  }
}

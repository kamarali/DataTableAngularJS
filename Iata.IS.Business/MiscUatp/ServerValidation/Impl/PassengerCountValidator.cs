using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  public class PassengerCountValidator : IServerValidator
  {
    private const string FieldGoupNameFlightDetails = "FlightDetails";
    private const string FieldNamePassengerCount = "PassengerCount";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      var isValid = true;
      validationError = null;
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        if (fieldMetaData.FieldName == FieldGoupNameFlightDetails)
        {
          var passengerCountFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == FieldNamePassengerCount).ToList();
          foreach (var metaData in passengerCountFields)
          {
            foreach (var field in metaData.FieldValues)
            {
              /* Attribute value provided but parent field value not provided.*/
              if(field.Value == null && field.AttributeValues.Count > 0)
              {
                validationError = new DynamicValidationError { ErrorCode = MiscUatpErrorCodes.FieldValueRequiredForAttribute, ErrorDescription = string.Format(Messages.ResourceManager.GetString(MiscUatpErrorCodes.FieldValueRequiredForAttribute), field.FieldMetaData.DisplayText) };
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

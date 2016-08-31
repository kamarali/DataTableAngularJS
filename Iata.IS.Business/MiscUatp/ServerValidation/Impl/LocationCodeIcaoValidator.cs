using System.Collections.Generic;
using System.Linq;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  public class LocationCodeIcaoValidator : IServerValidator
  {
    private const string LocationcodeIcao = "LocationCode_ICAO";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      bool isValid = true;
      validationError = null;
      var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
      var dataSourceRepository = Ioc.Resolve<IRepository<DataSource>>(typeof(IRepository<DataSource>));

      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var locationFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == LocationcodeIcao).ToList();

        foreach (var metaData in locationFields)
        {
          foreach (var field in metaData.FieldValues)
          {
            if (field.Value != null)
              field.Value = field.Value.ToUpper();
            if (!referenceManager.IsValidLocationIcaoCode(field.Value))
            {
              var fieldValue = field;
              var dataSourceRecord = dataSourceRepository.Single(dataSource => dataSource.Id == metaData.DataSourceId);
              if (dataSourceRecord.SubstituteValue != fieldValue.Value)
              {
                validationError = new DynamicValidationError
                                    {
                                      ErrorCode = MiscUatpErrorCodes.InvalidLocationCodeIcao,
                                      ErrorDescription =
                                        Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidLocationCodeIcao),
                                      FieldValue = field.Value
                                    };
                isValid = false;
              }
            }
            /*  else CMP#609 : MISC Change required as per ISW2
              { */
              if(field.AttributeValues.Count == 0)
              {
                validationError = new DynamicValidationError
                {
                  ErrorCode = MiscUatpErrorCodes.InvalidAttributeValue,
                  ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidAttributeValue)
                };

                isValid = false;
              }
            /*}*/
          }
        }
      }

      return isValid;
    }
  }
}
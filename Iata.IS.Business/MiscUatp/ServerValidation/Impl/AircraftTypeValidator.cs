using System.Collections.Generic;
using System.Linq;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  public class AircraftTypeValidator : IServerValidator
  {
    private const string AircraftTypeCode = "AircraftTypeCode";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      bool isValid = true;
      validationError = null;
      var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
      var dataSourceRepository = Ioc.Resolve<IRepository<DataSource>>(typeof(IRepository<DataSource>));
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var locationFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == AircraftTypeCode).ToList();

        foreach (var metaData in locationFields)
        {
          foreach (var field in metaData.FieldValues)
          {
            if (field.Value != null)
              field.Value = field.Value.ToUpper();
            if (!referenceManager.IsValidAircraftTypeCode(field.Value))
            {
              var fieldValue = field;
              var dataSourceRecord = dataSourceRepository.Single(dataSource => dataSource.Id == metaData.DataSourceId);
              if (dataSourceRecord.SubstituteValue != fieldValue.Value)
              {
                validationError = new DynamicValidationError
                                    {
                                      RecordId = field.RecordId,
                                      ErrorCode = MiscUatpErrorCodes.InvalidAircraftTypeCode,
                                      ErrorDescription =
                                        Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidAircraftTypeCode),
                                        ErrorStatus = ErrorStatus.C,
                                      FieldValue = field.Value
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

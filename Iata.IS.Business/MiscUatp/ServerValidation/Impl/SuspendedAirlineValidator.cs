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
  public class SuspendedAirlineValidator : IServerValidator
  {
    private const string SuspendedAirline = "SuspendedAirline";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      bool isValid = true;
      validationError = null;
      var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
      var dataSourceRepository = Ioc.Resolve<IRepository<DataSource>>(typeof(IRepository<DataSource>));

      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var locationFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == SuspendedAirline).ToList();

        foreach (var metaData in locationFields)
        {
          foreach (var field in metaData.FieldValues)
          {
            if (!referenceManager.IsValidMemberNumbericCode(field.Value))
            {
              var fieldValue = field;
              var dataSourceRecord = dataSourceRepository.Single(dataSource => dataSource.Id == metaData.DataSourceId);
              if (dataSourceRecord.SubstituteValue != fieldValue.Value)
              {
                validationError = new DynamicValidationError
                                    {
                                      RecordId = field.RecordId,
                                      ErrorCode = MiscUatpErrorCodes.InvalidSuspendedAirline,
                                      ErrorDescription =
                                        Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidSuspendedAirline),
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
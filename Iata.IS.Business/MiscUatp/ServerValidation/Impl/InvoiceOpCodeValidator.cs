using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  /// <summary>
  /// Validation InvoiceOpCode from mst_misc_codes table
  /// </summary>
  public class InvoiceOpCodeValidator : IServerValidator
  {
    private const string InvoiceOpCode = "InvoiceOpCode";

    /// <summary>
    /// This function is used to validate invoiceOpCode. 
    /// </summary>
    /// <param name="fieldMetaDataList"></param>
    /// <param name="MiscUatpErrorCodes"></param>
    /// <param name="validationError"></param>
    /// <returns></returns>
    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      bool isValid = true;
      validationError = null;
      var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
      var dataSourceRepository = Ioc.Resolve<IRepository<DataSource>>(typeof(IRepository<DataSource>));

      foreach (var fieldMetaData in fieldMetaDataList)
      {
        //Get InvoiceOpCode field from metadata.
        var invoiceOpCodeFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == InvoiceOpCode).ToList();

        foreach (var metaData in invoiceOpCodeFields)
        {
          foreach (var field in metaData.FieldValues)
          {
            if (field.Value != null)
            {
              field.Value = field.Value.Trim().ToUpper();
            }
            if (!referenceManager.IsValidMiscCode(field.Value))
            {
              var fieldValue = field;
              //Get row from data source table.
              var dataSourceRecord = dataSourceRepository.Single(dataSource => dataSource.Id == metaData.DataSourceId);
              if (dataSourceRecord.SubstituteValue != fieldValue.Value)
              {
                validationError = new DynamicValidationError
                {
                  RecordId = field.RecordId,
                  ErrorCode = MiscErrorCodes.InvalidMiscCode,
                  ErrorDescription = Messages.ResourceManager.GetString(MiscErrorCodes.InvalidMiscCode),
                  ErrorStatus = ErrorStatus.X,
                  FieldValue = field.Value
                };
                isValid = false;
              }
            }
          }
        }
      }

      //Release instance from container.
      Ioc.Release(referenceManager);
      Ioc.Release(dataSourceRepository);

      return isValid;
    }
  }
}

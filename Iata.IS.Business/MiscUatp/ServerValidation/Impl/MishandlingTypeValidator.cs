using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Business.Common;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Data.MiscUatp;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  /// <summary>
  /// CMP # 533: RAM A13 New Validations and New Charge Code.
  /// This is Server validation for the field 'Mishandling Type'
  /// when Billing Category = Misc and Charge Category = Ground Hadling and Charge Code = Mishandling Type.
  /// </summary>
  public class MishandlingTypeValidator: IServerValidator
  {
    /// <summary>
    /// This private constant string property is used to find the 'Mishandling Type' subfield from fieldMetaData.
    /// </summary>
    private const string MishandlingType = "MishandlingType";

    /// <summary>
    /// Method to validate field MishandlingType
    /// </summary>
    /// <param name="fieldMetaDataList"> in parameter Field Meta Data List </param>
    /// <param name="miscUatpErrorCodes"> in parameter Misc Uatp Error Codes </param>
    /// <param name="validationError"> out parameter Validation Error </param>
    /// <returns> Boolian value 'True' for valid values of Mishandling Type and 'False' on invalid values. </returns>
    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes miscUatpErrorCodes, out DynamicValidationError validationError)
    {
      // Initialisation.
      bool isValid = true;
      validationError = null;

      // Crating object of IDataSourceRepository Resolving Ioc for IDataSourceRepository.
      var dataSourceValueRepository = Ioc.Resolve<IDataSourceRepository>(typeof(IDataSourceRepository));

      // To read fieldMetaData one by one from fieldMetaDataList
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        // Get the fieldMetaData in which subfield 'Mishandling Type' exists.
        var mishandlingTypeFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == MishandlingType).ToList();

        // To read metaData one by one from mishandlingTypeFields.
        foreach (var metaData in mishandlingTypeFields)
        {
          // To read field one by one from metaData.FieldValues
          foreach (var field in metaData.FieldValues)
          {
            var fieldValue = field;
            
            // Get the valid values for the field Mishandling Type from database for given DataSourceId (i.e for the given ChargeCode and ChargeCategory)
            var dataSourceValues = dataSourceValueRepository.GetDataSourceValues(metaData.DataSourceId.HasValue ? (int) metaData.DataSourceId.Value : 0);

            // validate the Mishandling Type value with valid dataSourceValues
            if (!dataSourceValues.Any(dataSourceValue => dataSourceValue.Text.Equals(fieldValue.Value)))
            {
              // Crate Validation Error.
              validationError = new DynamicValidationError
                                  {
                                    RecordId = field.RecordId,
                                    ErrorCode = MiscErrorCodes.InvalidMishandlingTypeValue,
                                    ErrorDescription = Messages.ResourceManager.GetString(MiscErrorCodes.InvalidMishandlingTypeValue),
                                    ErrorStatus = ErrorStatus.X,
                                    FieldValue = field.Value
                                  };  
              isValid = false;
              } // End if
          } // End foreach
        } // End foreach
      } // End foreach
      
      // reurn isValid.
      return isValid;
    } // End Validate()
  } // End MishandlingTypeValidator
} // End namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl

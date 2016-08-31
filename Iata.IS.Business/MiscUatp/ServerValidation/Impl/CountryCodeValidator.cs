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
  public class CountryCodeValidator : IServerValidator
  {
    private const string FieldGoupNameAddress = "Address";
    private const string FieldNameCountrycode = "CountryCode";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      var isValid = true;
      validationError = null;
      var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
      var dataSourceRepository = Ioc.Resolve<IRepository<DataSource>>(typeof(IRepository<DataSource>));
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        var addressFields = fieldMetaData.SubFields.Where(field => field.FieldName == FieldGoupNameAddress);
        if (addressFields.Count() > 0)
        {
          foreach (var addressField in addressFields)
          {
            var countryIacoFields = addressField.SubFields.Where(subFields => subFields.FieldName == FieldNameCountrycode).ToList();

            foreach (var metaData in countryIacoFields)
            {
              foreach (var field in metaData.FieldValues)
              {
                if (field.Value != null)
                  field.Value = field.Value.ToUpper();
                if (!referenceManager.IsValidCountryCode(field.Value))
                {
                  var fieldValue = field;
                  if (dataSourceRepository.GetCount(dataSource => dataSource.SubstituteValue == fieldValue.Value) == 0)
                  {
                    validationError = new DynamicValidationError { ErrorCode = MiscUatpErrorCodes.InvalidCountryCode, ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidCountryCode) };
                    isValid = false;
                  }
                }
              }
            }
          }
        }
        else
        {
          var countryIacoFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == FieldNameCountrycode).ToList();

          foreach (var metaData in countryIacoFields)
          {
            foreach (var field in metaData.FieldValues)
            {
              if (field.Value != null)
                field.Value = field.Value.ToUpper();
              if (!referenceManager.IsValidCountryCode(field.Value))
              {
                var fieldValue = field;
                if (dataSourceRepository.GetCount(dataSource => dataSource.SubstituteValue == fieldValue.Value) == 0)
                {
                  validationError = new DynamicValidationError { ErrorCode = MiscUatpErrorCodes.InvalidCountryCode, ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidCountryCode) };
                  isValid = false;

                }
              }
            }
          }
        }
      }

      return isValid;
    }
  }
}
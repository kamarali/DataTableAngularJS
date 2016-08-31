using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
  public class AmountValidator : IServerValidator
  {
    private const string FieldGoupNameSettlementDetails = "SettlementDetails";
    private const string FieldNameAmount = "Amount";

    public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
    {
      var isValid = true;
      validationError = null;
      foreach (var fieldMetaData in fieldMetaDataList)
      {
        if (fieldMetaData.FieldName == FieldGoupNameSettlementDetails)
        {

          var amountFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == FieldNameAmount).ToList();
          foreach (var metaData in amountFields)
          {
            foreach (var field in metaData.FieldValues)
            {
              if (field.AttributeValues.Count == 0)
              {
                validationError = new DynamicValidationError { ErrorCode = MiscUatpErrorCodes.InvalidAmountName, ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidAmountName) };
                isValid = false;
              }
              /* Attribute value provided but parent field value not provided.*/
              else if(field.Value == null)
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

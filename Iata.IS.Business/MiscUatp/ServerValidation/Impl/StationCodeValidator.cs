using System.Collections.Generic;
using System.Linq;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.MiscUatp.ServerValidation.Impl
{
    class StationCodeValidator : IServerValidator
    {
        private const string StationCodeCode = "StationCode";

        public bool Validate(IList<FieldMetaData> fieldMetaDataList, MiscUatpErrorCodes MiscUatpErrorCodes, out DynamicValidationError validationError)
        {
            bool isValid = true;
            validationError = null;
            var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
            foreach (var fieldMetaData in fieldMetaDataList)
            {
                var locationFields = fieldMetaData.SubFields.Where(subFields => subFields.FieldName == StationCodeCode).ToList();

                foreach (var metaData in locationFields)
                {
                    foreach (var field in metaData.FieldValues)
                    {
                        if (!referenceManager.IsValidCityAirport(field.Value))
                        {
                            validationError = new DynamicValidationError
                            {
                                ErrorCode = MiscUatpErrorCodes.InvalidStationCode,
                                ErrorDescription = Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvalidStationCode)
                            };
                            isValid = false;
                        }
                    }
                }
            }

            return isValid;
        }
    }
}

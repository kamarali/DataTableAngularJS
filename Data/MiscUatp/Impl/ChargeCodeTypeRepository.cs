using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Master;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class ChargeCodeTypeRepository : Repository<ChargeCodeType>, IChargeCodeTypeRepository
    {
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static List<ChargeCodeType> LoadEntities(ObjectSet<ChargeCodeType> objectSet, LoadStrategyResult loadStrategyResult, Action<ChargeCodeType> link, string entityName)
        {
            if (link == null)
                link = new Action<ChargeCodeType>(c => { });

            var ChargeCodeTypes = new List<ChargeCodeType>();

            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                ChargeCodeTypes = muMaterializers.ChargeCodeTypeMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            return ChargeCodeTypes;
        }

        /// <summary>
        /// This function is used to get misc charge code based on charge category, charge code and charge code type name.
        /// </summary>
        /// <param name="chargeCategoryId"></param>
        /// <param name="chargeCodeId"></param>
        /// <returns></returns>
        //CMP #636: Standard Update Mobilization
        public List<ChargeCodeTypeSearchData> GetMiscChargeCodeType(int chargeCategoryId, int chargeCodeId, string chargeCodeTypeName)
        {
          var parameters = new ObjectParameter[3];

          parameters[0] = new ObjectParameter("CHARGE_CATEGORY_ID_I", typeof(int)) { Value = chargeCategoryId };
          parameters[1] = new ObjectParameter("CHARGE_CODE_ID_I", typeof(int)) { Value = chargeCodeId };
          parameters[2] = new ObjectParameter("CHARGE_CODE_TYPE_NAME_I", typeof(int)) { Value = chargeCodeTypeName };

          //Execute stored procedure and fetch data based on criteria.
          var miscChargeCodeType = ExecuteStoredFunction<ChargeCodeTypeSearchData>("GetMiscChargeCodeType", parameters);

          return miscChargeCodeType.ToList();
        }
    }
}

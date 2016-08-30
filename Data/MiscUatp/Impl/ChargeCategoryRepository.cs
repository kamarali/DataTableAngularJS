using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class ChargeCategoryRepository
    {
        /// <summary>
        /// This will load list of ChargeCategory objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<ChargeCategory> LoadEntities(ObjectSet<ChargeCategory> objectSet, LoadStrategyResult loadStrategyResult, Action<ChargeCategory> link)
        {
            if (link == null)
                link = new Action<ChargeCategory>(c => { });

            var chargeCategories = new List<ChargeCategory>();

            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.ChargeCategory))
            {

                // first result set includes the category
                foreach (var c in
                    muMaterializers.ChargeCategoryMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    chargeCategories.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load ChargeCategoryChargeCode
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.ChargeCategoryChargeCode) && chargeCategories.Count != 0)
            {
                ChargeCodeRepository.LoadEntities(objectSet.Context.CreateObjectSet<ChargeCode>(), loadStrategyResult, null, LoadStrategy.MiscEntities.ChargeCategoryChargeCode);
            }


            return chargeCategories;
        }

    }
}

using System;
using System.Collections.Generic;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Data.Impl;
using Microsoft.Data.Extensions;
using Devart.Data.Oracle;
using System.Data.Objects;

namespace Iata.IS.Data.Pax.Impl
{
    class SamplingFormDTaxRepository : Repository<SamplingFormDTax>
    {
        #region Load strategy

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<SamplingFormDTax> LoadEntities(ObjectSet<SamplingFormDTax> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormDTax> link)
        {
            if (link == null)
                link = new Action<SamplingFormDTax>(c => { });

            List<SamplingFormDTax> samplingFormDTaxes = new List<SamplingFormDTax>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormDTax))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SamplingFormDTaxMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SamplingFormDTax>(link)
                    )
                {
                    samplingFormDTaxes.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return samplingFormDTaxes;
        }

        #endregion
    }
}

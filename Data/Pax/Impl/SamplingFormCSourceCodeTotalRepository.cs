using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class SamplingFormCSamplingFormCSourceCodeTotalRepository
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
        public static List<SamplingFormCSourceCodeTotal> LoadEntities(ObjectSet<SamplingFormCSourceCodeTotal> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormCSourceCodeTotal> link)
        {
            if (link == null)
                link = new Action<SamplingFormCSourceCodeTotal>(c => { });

            var samplingFormCSourceCodeTotals = new List<SamplingFormCSourceCodeTotal>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormCSourceCodeTotal))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SamplingFormCSourceCodeTotalMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    samplingFormCSourceCodeTotals.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return samplingFormCSourceCodeTotals;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Data.Impl;
using System.Data.Objects;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    class SamplingFormDVatRepository : Repository<SamplingFormDVat>
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
        public static List<SamplingFormDVat> LoadEntities(ObjectSet<SamplingFormDVat> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormDVat> link)
        {
            if (link == null)
                link = new Action<SamplingFormDVat>(c => { });

            List<SamplingFormDVat> samplingFormDVats = new List<SamplingFormDVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormDVat))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SamplingFormDVatMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SamplingFormDVat>(link)
                    )
                {
                    samplingFormDVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load SamplingFormDVatIdentifier by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormDVatIdentifier) && samplingFormDVats.Count != 0)
                VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>()
                       , loadStrategyResult
                       , null
                       , LoadStrategy.Entities.SamplingFormDVatIdentifier
                       );

            return samplingFormDVats;
        }

        #endregion
    }
}

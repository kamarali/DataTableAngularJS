using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class VatIdentifierRepository
    {
        /// <summary>
        /// This will load list of VatIdentifier objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<VatIdentifier> LoadEntities(ObjectSet<VatIdentifier> objectSet, LoadStrategyResult loadStrategyResult, Action<VatIdentifier> link, string entity)
        {
            if (link == null)
                link = new Action<VatIdentifier>(c => { });

            var vatIdentifierColl = new List<VatIdentifier>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entity))
            {

                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().VatIdentifierMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    vatIdentifierColl.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return vatIdentifierColl;
        }
    }
}

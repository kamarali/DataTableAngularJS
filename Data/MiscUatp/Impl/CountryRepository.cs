using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class CountryRepository
    {
        /// <summary>
        /// This will load list of Country objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<Country> LoadEntities(ObjectSet<Country> objectSet, LoadStrategyResult loadStrategyResult, Action<Country> link, string entity)
        {
            if (link == null)
                link = new Action<Country>(c => { });

            var countries = new List<Country>();

            var commonMaterializers = new CommonMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entity))
            {

                // first result set includes the category
                foreach (var c in
                    commonMaterializers.CountryMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    countries.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return countries;
        }

    }
}

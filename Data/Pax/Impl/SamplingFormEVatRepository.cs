using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class SamplingFormEVatRepository : Repository<SamplingFormEDetailVat>, ISamplingFormEVatRepository
    {
        #region ISamplingFormEVatRepository Members

        public override IQueryable<SamplingFormEDetailVat> Get(Expression<Func<SamplingFormEDetailVat, bool>> where)
        {
            IQueryable<SamplingFormEDetailVat> formEVatList = EntityObjectSet.Include("VatIdentifier").Where(where);

            return formEVatList;
        }

        #endregion

        #region Load strategy

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<SamplingFormEDetailVat> LoadEntities(ObjectSet<SamplingFormEDetailVat> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormEDetailVat> link)
        {
            if (link == null)
                link = new Action<SamplingFormEDetailVat>(c => { });
            List<SamplingFormEDetailVat> samplingFormEDetailVats = new List<SamplingFormEDetailVat>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormEDetailVat))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SamplingFormEDetailVatMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SamplingFormEDetailVat>(link)
                    )
                {
                    samplingFormEDetailVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load VatIdentifier
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormEDetailVatIdentifier) && samplingFormEDetailVats.Count != 0)
            {
                VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>(), loadStrategyResult, null, LoadStrategy.Entities.SamplingFormEDetailVatIdentifier);
            }

            return samplingFormEDetailVats;
        }

        #endregion
    }
}
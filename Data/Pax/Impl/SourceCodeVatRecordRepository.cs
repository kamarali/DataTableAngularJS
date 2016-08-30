using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;
using System.Linq;

namespace Iata.IS.Data.Pax.Impl
{
    public class SourceCodeVatRecordRepository : Repository<SourceCodeVat>, ISourceCodeVatRecordRepository
    {
        public override IQueryable<SourceCodeVat> Get(Expression<Func<SourceCodeVat, bool>> where)
        {
            return EntityObjectSet.Include("VatIdentifier").Where(where);
        }

        #region Load strategy

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<SourceCodeVat> LoadEntities(ObjectSet<SourceCodeVat> objectSet, LoadStrategyResult loadStrategyResult, Action<SourceCodeVat> link)
        {
            if (link == null)
                link = new Action<SourceCodeVat>(c => { });

            List<SourceCodeVat> sourceCodeVats = new List<SourceCodeVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SourceCodeTotalVat))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SourceCodeVatMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SourceCodeVat>(link)
                    )
                {
                    sourceCodeVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load SourceCodeVatIdentifier
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SourceCodeVatIdentifier))
            {
                ObjectSet<VatIdentifier> objectSetVatIdentifier = objectSet.Context.CreateObjectSet<VatIdentifier>();
                using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SourceCodeVatIdentifier))
                {
                    foreach (var ct in new PaxMaterializers().VatIdentifierMaterializer.Materialize(reader).Bind(objectSetVatIdentifier))
                    {
                    }
                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return sourceCodeVats;
        }

        #endregion
    }
}

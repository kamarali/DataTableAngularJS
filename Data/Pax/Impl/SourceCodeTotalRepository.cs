using System;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class SourceCodeTotalRepository : Repository<SourceCodeTotal>, ISourceCodeTotalRepository
    {
        public override IQueryable<SourceCodeTotal> Get(Expression<Func<SourceCodeTotal, bool>> where)
        {
            return EntityObjectSet.Include("Invoice").Where(where);
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
        public static List<SourceCodeTotal> LoadEntities(ObjectSet<SourceCodeTotal> objectSet, LoadStrategyResult loadStrategyResult, Action<SourceCodeTotal> link)
        {
            if (link == null)
                link = new Action<SourceCodeTotal>(c => { });

            List<SourceCodeTotal> sourceCodeTotals = new List<SourceCodeTotal>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SourceCodeTotal))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SourceCodeTotalMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SourceCodeTotal>(link)
                    )
                {
                    sourceCodeTotals.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load SourceCodeVat by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SourceCodeTotalVat) && sourceCodeTotals.Count != 0)
                SourceCodeVatRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<SourceCodeVat>()
                        , loadStrategyResult
                        , null);


            //Load SourceCode
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SourceCode))
            {
                ObjectSet<SourceCode> objectSetSourceCode = objectSet.Context.CreateObjectSet<SourceCode>();
                using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SourceCode))
                {
                    foreach (var ct in new PaxMaterializers().SourceCodeMaterializer.Materialize(reader).Bind(objectSetSourceCode))
                    {
                    }
                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return sourceCodeTotals;
        }

        #endregion
    }
}

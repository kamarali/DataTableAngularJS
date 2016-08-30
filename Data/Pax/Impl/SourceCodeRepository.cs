using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class SourceCodeRepository : Repository<SourceCode>, ISourceCodeRepository
    {
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<SourceCode> LoadAuditEntities(ObjectSet<SourceCode> objectSet, LoadStrategyResult loadStrategyResult, Action<SourceCode> link, string entity)
        {
            if (link == null)
                link = new Action<SourceCode>(c => { });

            List<SourceCode> sourceCodes = new List<SourceCode>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entity))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().PaxInoiveSourceCodeAuditMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SourceCode>(link)
                    )
                {
                    sourceCodes.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return sourceCodes;
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<SourceCode> LoadEntities(ObjectSet<SourceCode> objectSet, LoadStrategyResult loadStrategyResult, Action<SourceCode> link, string entity)
        {
            if (link == null)
                link = new Action<SourceCode>(c => { });

            var sourceCodes = new List<SourceCode>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entity))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SourceCodeMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SourceCode>(link)
                    )
                {
                    sourceCodes.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return sourceCodes;
        }
    }
}

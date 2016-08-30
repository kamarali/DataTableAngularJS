using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class RejectionMemoVatRepository
    {
        /// <summary>
        /// This will load list of RejectionMemoVat objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<RejectionMemoVat> LoadEntities(ObjectSet<RejectionMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<RejectionMemoVat> link)
        {
            if (link == null)
                link = new Action<RejectionMemoVat>(c => { });

            var rejectionMemoVatColl = new List<RejectionMemoVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemoVat))
            {

                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().RejectionMemoVatMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    rejectionMemoVatColl.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load VatIdentifier
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoVatIdentifier) && rejectionMemoVatColl.Count != 0)
            {
                VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>(), loadStrategyResult, null, LoadStrategy.Entities.RejectionMemoVatIdentifier);
            }

            return rejectionMemoVatColl;
        }

        public static List<RejectionMemoVat> LoadAuditEntities(ObjectSet<RejectionMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<RejectionMemoVat> link)
        {
            if (link == null)
                link = new Action<RejectionMemoVat>(c => { });

            var rejectionMemoVatColl = new List<RejectionMemoVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.RejectionMemoVAT))
            {

                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().RejectionMemoVatAuditMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    rejectionMemoVatColl.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return rejectionMemoVatColl;
        }
    }
}

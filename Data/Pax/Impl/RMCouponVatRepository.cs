using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class RMCouponVatRepository
    {
        /// <summary>
        /// This will load list of RMCouponVat objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<RMCouponVat> LoadEntities(ObjectSet<RMCouponVat> objectSet, LoadStrategyResult loadStrategyResult, Action<RMCouponVat> link)
        {
            if (link == null)
                link = new Action<RMCouponVat>(c => { });

            var rmCouponVats = new List<RMCouponVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemoCouponVat))
            {

                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().RMCouponVatMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    rmCouponVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load VatIdentifier
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoCouponVatIdentifier) && rmCouponVats.Count != 0)
            {
                VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>(), loadStrategyResult, null, LoadStrategy.Entities.RejectionMemoCouponVatIdentifier);
            }

            return rmCouponVats;
        }

        public static List<RMCouponVat> LoadAuditEntities(ObjectSet<RMCouponVat> objectSet, LoadStrategyResult loadStrategyResult, Action<RMCouponVat> link)
        {
            if (link == null)
                link = new Action<RMCouponVat>(c => { });

            var rmCouponVats = new List<RMCouponVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.RMCouponVAT))
            {

                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().RMCouponVatAuditMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    rmCouponVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return rmCouponVats;
        }
    }
}

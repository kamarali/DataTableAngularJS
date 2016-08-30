using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class CouponVatRecordRepository : Repository<PrimeCouponVat>, ICouponVatRecordRepository
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
        public static List<PrimeCouponVat> LoadEntities(ObjectSet<PrimeCouponVat> objectSet, LoadStrategyResult loadStrategyResult, Action<PrimeCouponVat> link)
        {
            if (link == null)
                link = new Action<PrimeCouponVat>(c => { });

            List<PrimeCouponVat> primeCouponVats = new List<PrimeCouponVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CouponVat))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().CouponVatMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<PrimeCouponVat>(link)
                    )
                {
                    primeCouponVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load PrimeCouponVatIdentifier
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CouponDataVatIdentifier) && primeCouponVats.Count > 0)
            {
                VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.CouponDataVatIdentifier);
            }

            return primeCouponVats;
        }

        #endregion

        public static List<PrimeCouponVat> LoadAuditEntities(ObjectSet<PrimeCouponVat> objectSet, LoadStrategyResult loadStrategyResult, Action<PrimeCouponVat> link)
        {
            if (link == null)
                link = new Action<PrimeCouponVat>(c => { });

            List<PrimeCouponVat> primeCouponVats = new List<PrimeCouponVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.PrimecouponVat))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().CouponVatAuditMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<PrimeCouponVat>(link)
                    )
                {
                    primeCouponVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return primeCouponVats;
        }
    }
}

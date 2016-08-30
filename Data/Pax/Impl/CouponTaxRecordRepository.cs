using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    class CouponTaxRecordRepository : Repository<PrimeCouponTax>, ICouponTaxRecordRepository
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
        public static List<PrimeCouponTax> LoadEntities(ObjectSet<PrimeCouponTax> objectSet, LoadStrategyResult loadStrategyResult, Action<PrimeCouponTax> link)
        {
            if (link == null)
                link = new Action<PrimeCouponTax>(c => { });

            List<PrimeCouponTax> primeCouponTaxs = new List<PrimeCouponTax>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CouponTax))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().CouponTaxMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<PrimeCouponTax>(link)
                    )
                {
                    primeCouponTaxs.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return primeCouponTaxs;
        }

        #endregion

        public static List<PrimeCouponTax> LoadAuditEntities(ObjectSet<PrimeCouponTax> objectSet, LoadStrategyResult loadStrategyResult, Action<PrimeCouponTax> link)
        {
            if (link == null)
                link = new Action<PrimeCouponTax>(c => { });

            List<PrimeCouponTax> primeCouponTaxs = new List<PrimeCouponTax>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.PrimecouponTax))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().CouponTaxAuditMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<PrimeCouponTax>(link)
                    )
                {
                    primeCouponTaxs.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return primeCouponTaxs;
        }

    }
}

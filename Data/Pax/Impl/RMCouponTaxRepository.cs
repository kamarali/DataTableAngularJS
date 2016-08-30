using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class RMCouponTaxRepository
    {
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<RMCouponTax> LoadEntities(ObjectSet<RMCouponTax> objectSet, LoadStrategyResult loadStrategyResult, Action<RMCouponTax> link)
        {
            if (link == null)
                link = new Action<RMCouponTax>(c => { });

            var rmCouponTaxs = new List<RMCouponTax>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemoCouponTax))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().RMCouponTaxMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<RMCouponTax>(link)
                    )
                {
                    rmCouponTaxs.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return rmCouponTaxs;
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<RMCouponTax> LoadLinkedEntities(LoadStrategyResult loadStrategyResult, Action<RMCouponTax> link)
        {
            if (link == null)
                link = new Action<RMCouponTax>(c => { });

            var rmCouponTaxs = new List<RMCouponTax>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(RMCouponBreakdownRecordRepositoryConstants.CouponTaxParameterName))
            {
                if (reader.HasRows)
                {
                    // first result set includes the category
                    foreach (var c in
                        new PaxMaterializers().RMCouponTaxMaterializer
                            .Materialize(reader)
                            .ForEach<RMCouponTax>(link)
                        )
                    {
                        rmCouponTaxs.Add(c);
                    }
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return rmCouponTaxs;
        }

        public static List<RMCouponTax> LoadAuditEntities(ObjectSet<RMCouponTax> objectSet, LoadStrategyResult loadStrategyResult, Action<RMCouponTax> link)
        {
            if (link == null)
                link = new Action<RMCouponTax>(c => { });

            var rmCouponTaxs = new List<RMCouponTax>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.RMCouponTax))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().RMCouponTaxAuditMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<RMCouponTax>(link)
                    )
                {
                    rmCouponTaxs.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return rmCouponTaxs;
        }
    }
}


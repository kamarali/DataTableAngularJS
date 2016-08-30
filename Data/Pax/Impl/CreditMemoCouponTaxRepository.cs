using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class CreditMemoCouponTaxRepository
    {
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<CMCouponTax> LoadEntities(ObjectSet<CMCouponTax> objectSet, LoadStrategyResult loadStrategyResult, Action<CMCouponTax> link)
        {
            if (link == null)
                link = new Action<CMCouponTax>(c => { });

            var cmCouponTaxs = new List<CMCouponTax>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CreditMemoCouponTax))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().CMCouponTaxMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<CMCouponTax>(link)
                    )
                {
                    cmCouponTaxs.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return cmCouponTaxs;
        }
    }
}


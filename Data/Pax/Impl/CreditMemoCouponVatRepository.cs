using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class CreditMemoCouponVatRepository
    {
        /// <summary>
        /// This will load list of CMCouponVat objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<CMCouponVat> LoadEntities(ObjectSet<CMCouponVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CMCouponVat> link)
        {
            if (link == null)
                link = new Action<CMCouponVat>(c => { });

            var cmCouponVats = new List<CMCouponVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CreditMemoCouponVat))
            {

                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().CMCouponVatMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    cmCouponVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load VatIdentifier
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemoCouponVatIdentifier) && cmCouponVats.Count != 0)
            {
                VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>(), loadStrategyResult, null, LoadStrategy.Entities.CreditMemoCouponVatIdentifier);
            }

            return cmCouponVats;
        }
    }
}

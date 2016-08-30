using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class BillingMemoCouponVatRepository : Repository<BMCouponVat>, IBillingMemoCouponVatRepository
    {
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static List<BMCouponVat> LoadEntities(ObjectSet<BMCouponVat> objectSet, LoadStrategyResult loadStrategyResult, Action<BMCouponVat> link, string entityName)
        {
            if (link == null)
                link = new Action<BMCouponVat>(c => { });

            var bmCouponVats = new List<BMCouponVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                bmCouponVats = new PaxMaterializers().BMCouponVatMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            return bmCouponVats;
        }

        public static List<BMCouponVat> LoadAuditEntities(ObjectSet<BMCouponVat> objectSet, LoadStrategyResult loadStrategyResult, Action<BMCouponVat> link, string entityName)
        {
            if (link == null)
                link = new Action<BMCouponVat>(c => { });

            var bmCouponVats = new List<BMCouponVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
              if (reader != null)
              {
                // first result set includes the category
                bmCouponVats =
                  new PaxMaterializers().BMCouponVatAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).
                    ToList();
                if (!reader.IsClosed)
                  reader.Close();
              }
            }
            return bmCouponVats;
        }
    }
}
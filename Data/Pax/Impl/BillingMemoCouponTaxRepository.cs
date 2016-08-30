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
    public class BillingMemoCouponTaxRepository : Repository<BMCouponTax>, IBillingMemoCouponTaxRepository
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
        public static List<BMCouponTax> LoadEntities(ObjectSet<BMCouponTax> objectSet, LoadStrategyResult loadStrategyResult, Action<BMCouponTax> link, string entityName)
        {
            if (link == null)
                link = new Action<BMCouponTax>(c => { });

            var bmCouponTaxs = new List<BMCouponTax>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
              if (reader != null)
              {
                // first result set includes the category
                bmCouponTaxs =
                  new PaxMaterializers().BMCouponTaxMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).
                    ToList();
                if (!reader.IsClosed)
                  reader.Close();
              }
            }
            return bmCouponTaxs;
        }

        public static List<BMCouponTax> LoadAuditEntities(ObjectSet<BMCouponTax> objectSet, LoadStrategyResult loadStrategyResult, Action<BMCouponTax> link, string entityName)
        {
            if (link == null)
                link = new Action<BMCouponTax>(c => { });

            var bmCouponTaxs = new List<BMCouponTax>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
              if (reader != null)
              {
                // first result set includes the category
                bmCouponTaxs =
                  new PaxMaterializers().BMCouponTaxAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).
                    ToList();
                if (!reader.IsClosed)
                  reader.Close();
              }
            }
            return bmCouponTaxs;
        }
    }
}
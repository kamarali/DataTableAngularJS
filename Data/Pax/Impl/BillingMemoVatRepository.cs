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
    public class BillingMemoVatRepository : Repository<BillingMemoVat>, IBillingMemoVatRepository
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
        public static List<BillingMemoVat> LoadEntities(ObjectSet<BillingMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<BillingMemoVat> link, string entityName)
        {
            if (link == null)
                link = new Action<BillingMemoVat>(c => { });

            var billingMemoVats = new List<BillingMemoVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                billingMemoVats = new PaxMaterializers().BillingMemoVatMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            return billingMemoVats;
        }

        public static List<BillingMemoVat> LoadAuditEntities(ObjectSet<BillingMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<BillingMemoVat> link, string entityName)
        {
            if (link == null)
                link = new Action<BillingMemoVat>(c => { });

            var billingMemoVats = new List<BillingMemoVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                billingMemoVats = new PaxMaterializers().BillingMemoVatAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            return billingMemoVats;
        }
    }
}
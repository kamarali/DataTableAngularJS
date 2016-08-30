using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
    public class CargoBillingMemoVatRepository : Repository<CargoBillingMemoVat>, ICargoBillingMemoVatRepository
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
        public static List<CargoBillingMemoVat> LoadEntities(ObjectSet<CargoBillingMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingMemoVat> link, string entityName)
        {
            if (link == null)
                link = new Action<CargoBillingMemoVat>(c => { });

            var billingMemoVats = new List<CargoBillingMemoVat>();
            var cargoMaterializers = new CargoMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                billingMemoVats = cargoMaterializers.CargoBillingMemoVatMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
               // billingMemoVats= new List<CargoBillingMemoVat>();
                reader.Close();
            }
            return billingMemoVats;
        }

        public static List<CargoBillingMemoVat> LoadAuditEntities(ObjectSet<CargoBillingMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingMemoVat> link, string entityName)
        {
          if (link == null)
            link = new Action<CargoBillingMemoVat>(c => { });

          var billingMemoVats = new List<CargoBillingMemoVat>();
          var cargoMaterializers = new CargoMaterializers();
          using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
          {
            // first result set includes the category
              billingMemoVats = cargoMaterializers.CargoBillingMemoVatAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
            reader.Close();
          }
          return billingMemoVats;
        }

    }
}

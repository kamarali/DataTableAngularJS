using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using System.Data.Objects;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Common.Impl
{
    public class CurrencyRepository : Repository<Currency>, ICurrencyRepository
    {
        public static List<Currency> LoadEntities(ObjectSet<Currency> objectSet, LoadStrategyResult loadStrategyResult, Action<Currency> link, string entity)
        {
            if (link == null)
                link = new Action<Currency>(c => { });

      List<Currency> invoiceCurrency = new List<Currency>();
      var commonMaterializers = new CommonMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(entity))
      {

                // first result set includes the category
                foreach (var c in
                    commonMaterializers.CurrencyMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<Currency>(link)
                    )
                {
                    invoiceCurrency.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return invoiceCurrency;
        }

        public static List<Currency> LoadAuditEntities(ObjectSet<Currency> objectSet, LoadStrategyResult loadStrategyResult, Action<Currency> link, string entity)
        {
            if (link == null)
                link = new Action<Currency>(c => { });

            List<Currency> invoiceCurrency = new List<Currency>();
            var commonMaterializers = new CommonMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entity))
            {

                // first result set includes the category
                foreach (var c in
                    commonMaterializers.CurrencyAuditMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<Currency>(link)
                    )
                {
                    invoiceCurrency.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return invoiceCurrency;
        }

    }
}

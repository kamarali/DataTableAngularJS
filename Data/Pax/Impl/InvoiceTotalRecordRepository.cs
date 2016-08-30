using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class InvoiceTotalRecordRepository : Repository<InvoiceTotal>, IInvoiceTotalRecordRepository
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
        public static List<InvoiceTotal> LoadEntities(ObjectSet<InvoiceTotal> objectSet, LoadStrategyResult loadStrategyResult, Action<InvoiceTotal> link)
        {
            if (link == null)
                link = new Action<InvoiceTotal>(c => { });

            List<InvoiceTotal> invoiceTotals = new List<InvoiceTotal>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.InvoiceTotal))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().InvoiceTotalMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<InvoiceTotal>(link)
                    )
                {
                    invoiceTotals.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return invoiceTotals;
        }

        #endregion
    }
}

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
    public class InvoiceTotalVatRepository : Repository<InvoiceVat>, IInvoiceTotalVatRepository
    {
        public override IQueryable<InvoiceVat> Get(System.Linq.Expressions.Expression<Func<InvoiceVat, bool>> where)
        {
            var InvoiceTotalVatList = EntityObjectSet
                                                   .Include("VatIdentifier").Where(where);

            return InvoiceTotalVatList;
        }

        #region Load strategy

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<InvoiceVat> LoadEntities(ObjectSet<InvoiceVat> objectSet, LoadStrategyResult loadStrategyResult, Action<InvoiceVat> link)
        {
            if (link == null)
                link = new Action<InvoiceVat>(c => { });

            List<InvoiceVat> invoiceVats = new List<InvoiceVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.InvoiceTotalVat))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().InvoiceVatMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<InvoiceVat>(link)
                    )
                {
                    invoiceVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load InvoiceTotalVatIdentifier
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.InvoiceTotalVatIdentifier))
            {
                ObjectSet<VatIdentifier> objectSetVatIdentifier = objectSet.Context.CreateObjectSet<VatIdentifier>();
                using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.InvoiceTotalVatIdentifier))
                {
                    foreach (var ct in new PaxMaterializers().VatIdentifierMaterializer.Materialize(reader).Bind(objectSetVatIdentifier))
                    {
                    }
                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return invoiceVats;
        }

        #endregion

    }
}

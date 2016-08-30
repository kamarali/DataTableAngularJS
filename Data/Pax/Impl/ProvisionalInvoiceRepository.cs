using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class ProvisionalInvoiceRepository : Repository<ProvisionalInvoiceRecordDetail>, IProvisionalInvoiceRepository
    {
        #region Load strategy

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<ProvisionalInvoiceRecordDetail> LoadEntities(ObjectSet<ProvisionalInvoiceRecordDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<ProvisionalInvoiceRecordDetail> link)
        {
            if (link == null)
                link = new Action<ProvisionalInvoiceRecordDetail>(c => { });
            List<ProvisionalInvoiceRecordDetail> provisionalInvoiceRecordDetails = new List<ProvisionalInvoiceRecordDetail>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.ProvisionalInvoiceDetails))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().ProvisionalInvoiceMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<ProvisionalInvoiceRecordDetail>(link)
                    )
                {
                    provisionalInvoiceRecordDetails.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return provisionalInvoiceRecordDetails;
        }

        #endregion

        public override IQueryable<ProvisionalInvoiceRecordDetail> Get(System.Linq.Expressions.Expression<Func<ProvisionalInvoiceRecordDetail, bool>> where)
        {
            var ProvisionalInvoiceList = EntityObjectSet.Include("InvoiceListingCurrency").Where(where);

            return ProvisionalInvoiceList;
        }
    }
}

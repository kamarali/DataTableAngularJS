using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class SamplingFormERepository : Repository<SamplingFormEDetail>, ISamplingFormERepository
    {
        #region Load strategy

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<SamplingFormEDetail> LoadEntities(ObjectSet<SamplingFormEDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormEDetail> link)
        {
            if (link == null)
                link = new Action<SamplingFormEDetail>(c => { });
            List<SamplingFormEDetail> samplingFormEDetails = new List<SamplingFormEDetail>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormEDetails))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SamplingFormEDetailMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SamplingFormEDetail>(link)
                    )
                {
                    samplingFormEDetails.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            return samplingFormEDetails;
        }

        #endregion

        /// <summary>
        /// Update the form E details 
        /// </summary>
        /// <param name="invoiceId"> The invoice id.</param>
        public void UpdateFormEDetails(Guid invoiceId)
        {
            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter(SamplingFormERepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };

            ExecuteStoredProcedure(SamplingFormERepositoryConstants.UpdateFormEDetailsFunctionname, parameters);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq.Expressions;
using Iata.IS.Core;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Data.Impl;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class SamplingFormDRepository : Repository<SamplingFormDRecord>, ISamplingFormDRepository
    {
        public override SamplingFormDRecord Single(Expression<System.Func<SamplingFormDRecord, bool>> where)
        {
            throw new NotImplementedException("Use Load Strategy overload of Single method instead.");
        }

        public void UpdateFormDInvoiceTotal(Guid invoiceId, int sourceId)
        {
            var parameters = new ObjectParameter[2];

            parameters[0] = new ObjectParameter(SamplingFormDRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(SamplingFormDRepositoryConstants.SourceIdParameterName, typeof(int)) { Value = sourceId };

            ExecuteStoredProcedure(SamplingFormDRepositoryConstants.UpdatePrimeInvoiceTotalFunctionName, parameters);
        }

        public bool IsFormDRecordDuplicate(Guid invoiceId, int batchRecordSequenceNo, int batchSequenceNo, string provisionalInvoiceNo, Guid formDId)
        {
            var parameters = new ObjectParameter[6];
            parameters[0] = new ObjectParameter(SamplingFormDRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(SamplingFormDRepositoryConstants.BatchRecordSequenceNoParameterName, typeof(int)) { Value = batchRecordSequenceNo };
            parameters[2] = new ObjectParameter(SamplingFormDRepositoryConstants.BatchSequenceNoParameterName, typeof(int)) { Value = batchSequenceNo };
            parameters[3] = new ObjectParameter(SamplingFormDRepositoryConstants.ProvisionalInvoiceNoParameterName, typeof(string)) { Value = provisionalInvoiceNo };
            parameters[4] = new ObjectParameter(SamplingFormDRepositoryConstants.FormDIdParameterName, typeof(Guid)) { Value = formDId };
            parameters[5] = new ObjectParameter(SamplingFormDRepositoryConstants.IsUniqueNoParameterName, typeof(int));

            ExecuteStoredProcedure(SamplingFormDRepositoryConstants.IsFormDRecordDuplicateFunctionName, parameters);

            return Convert.ToBoolean(parameters[5].Value);
        }

        #region Load strategy

        /// <summary>
        /// Loadstrategy method overload of Single
        /// </summary>
        /// <param name="formDRecordId">Form D Record Id</param>
        /// <returns>SamplingFormDRecord</returns>
        public SamplingFormDRecord Single(Guid formDRecordId)
        {
            var entities = new string[] { LoadStrategy.Entities.SamplingFormDRecord, LoadStrategy.Entities.SamplingFormDTax, LoadStrategy.Entities.SamplingFormDVat, 
      LoadStrategy.Entities.SamplingFormDVatIdentifier, LoadStrategy.Entities.SamplingFormDAttachment,LoadStrategy.Entities.AttachmentUploadedbyUser};

            var loadStrategy = new LoadStrategy(string.Join(",", entities));
            var formDRecordIdstr = ConvertUtil.ConvertGuidToString(formDRecordId);
            var formDRecords = GetFormDRecordLS(loadStrategy, formDRecordIdstr);

            SamplingFormDRecord formDRecord = null;
            if (formDRecords.Count > 0)
            {
                if (formDRecords.Count > 1) throw new ApplicationException("Multiple records found");
                formDRecord = formDRecords[0];
            }
            return formDRecord;
        }

        /// <summary>
        /// Gets list of SamplingFormDRecord objects
        /// </summary>
        /// <param name="loadStrategy"></param>
        /// <param name="formDId"></param>
        /// <returns></returns>
        public List<SamplingFormDRecord> GetFormDRecordLS(LoadStrategy loadStrategy, string formDId)
        {
            return base.ExecuteLoadsSP(SisStoredProcedures.GetFormDRecord,
                                      loadStrategy,
                                        new OracleParameter[] { new OracleParameter(SamplingFormDRepositoryConstants.FormDIdParameterName, formDId)
                                },
                                      this.FetchRecords);
        }

        /// <summary>
        /// Returns multiple records extracted from the result set.
        /// This is done by calling the right repository to populate the object set in the repository.
        /// </summary>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        private List<SamplingFormDRecord> FetchRecords(LoadStrategyResult loadStrategyResult)
        {
            var formDRecords = new List<SamplingFormDRecord>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormDRecord))
            {
                formDRecords = SamplingFormDRepository.LoadEntities(base.EntityObjectSet, loadStrategyResult, null);
            }

            return formDRecords;
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<SamplingFormDRecord> LoadEntities(ObjectSet<SamplingFormDRecord> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormDRecord> link)
        {
            if (link == null)
                link = new Action<SamplingFormDRecord>(c => { });
            var samplingFormDRecords = new List<SamplingFormDRecord>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormDRecord))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SamplingFormDMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SamplingFormDRecord>(link)
                    )
                {
                    samplingFormDRecords.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load SamplingFormDTax by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormDTax) && samplingFormDRecords.Count != 0)
                SamplingFormDTaxRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormDTax>()
                        , loadStrategyResult
                        , null);


            //Load SamplingFormDVat by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormDVat) && samplingFormDRecords.Count != 0)
                SamplingFormDVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormDVat>()
                        , loadStrategyResult
                        , null);


            //Load SamplingFormDAttachment by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormDAttachment) && samplingFormDRecords.Count != 0)
                SamplingFormDAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormDRecordAttachment>()
                        , loadStrategyResult
                        , null);

            return samplingFormDRecords;
        }

        public static List<SamplingFormDRecord> LoadAuditEntities(ObjectSet<SamplingFormDRecord> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormDRecord> link, string entity)
        {
            if (link == null)
                link = new Action<SamplingFormDRecord>(c => { });
            var samplingFormDRecords = new List<SamplingFormDRecord>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.FormDCoupon))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().SamplingFormDAuditMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<SamplingFormDRecord>(link)
                    )
                {
                    samplingFormDRecords.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            ////Load SamplingFormDTax by calling respective LoadEntities method
            //if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormDTax) && samplingFormDRecords.Count != 0)
            //  SamplingFormDTaxRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormDTax>()
            //          , loadStrategyResult
            //          , null);


            ////Load SamplingFormDVat by calling respective LoadEntities method
            //if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormDVat) && samplingFormDRecords.Count != 0)
            //  SamplingFormDVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormDVat>()
            //          , loadStrategyResult
            //          , null);


            //Load SamplingFormDAttachment by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormDAttachment) && samplingFormDRecords.Count != 0)
                SamplingFormDAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormDRecordAttachment>()
                        , loadStrategyResult
                        , null);

            return samplingFormDRecords;
        }
        #endregion
    }
}

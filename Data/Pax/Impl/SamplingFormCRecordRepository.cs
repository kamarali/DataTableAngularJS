using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class SamplingFormCRecordRepository : Repository<SamplingFormCRecord>, ISamplingFormCRecordRepository
    {
        public override SamplingFormCRecord Single(System.Linq.Expressions.Expression<Func<SamplingFormCRecord, bool>> where)
        {
            throw new NotImplementedException("Use overloaded Single instead.");
        }

        /// <summary>
        /// Get sampling form C record details
        /// </summary>
        /// <param name="samplingFormCRecordId"></param>
        /// <returns>samplingFormC record</returns>
        public SamplingFormCRecord Single(Guid samplingFormCRecordId)
        {
            var entities = new string[] { LoadStrategy.Entities.SamplingFormC, LoadStrategy.Entities.SamplingFormCDetails, LoadStrategy.Entities.SamplingFormCRecordAttachment,
                      LoadStrategy.Entities.AttachmentUploadedbyUser};
            var samplingFormCRecordIdstr = ConvertUtil.ConvertGuidToString(samplingFormCRecordId);
            var formCRecordColl = GetSamplingFormCRecordLS(new LoadStrategy(string.Join(",", entities)), samplingFormCRecordIdstr);
            SamplingFormCRecord formCRecord = null;
            if (formCRecordColl.Count > 0)
            {
                if (formCRecordColl.Count > 1) throw new ApplicationException("Multiple records found");
                formCRecord = formCRecordColl[0];
            }
            return formCRecord;
        }

        /// <summary>
        /// Get sampling form C record details
        /// </summary>
        /// <param name="loadStrategy"></param>
        /// <param name="samplingFormCRecordId"></param>
        /// <returns></returns>
        public List<SamplingFormCRecord> GetSamplingFormCRecordLS(LoadStrategy loadStrategy, string samplingFormCRecordId)
        {
            return base.ExecuteLoadsSP(SisStoredProcedures.GetSamplingFormC,
                                       loadStrategy,
                                       new OracleParameter[]
                                   {
                                     new OracleParameter(SamplingFormCRepositoryConstants.FormCIdParameterName, null),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMemberIdParameterName, null),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMonthParameterName, null),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ProvisionalBillingYearParameterName, null),
                                     new OracleParameter(SamplingFormCRepositoryConstants.InvoiceStatusIdsParameterName, null),
                                     new OracleParameter(SamplingFormCRepositoryConstants.FromMemberIdParameterName, null),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ListingCurrencyCodeNumParamaterName, null),
                                     new OracleParameter(SamplingFormCRepositoryConstants.FormCDeatilIdParameterName, samplingFormCRecordId),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ProcessingCompletedOnDateTimeParameterName,null), 
                                     new OracleParameter(SamplingFormCRepositoryConstants.FormCProcessingCompletedOnDateTimeParameterName,null), 
                                   },
                                       r => this.FetchRecord(r));

        }

        /// <summary>
        /// FetchRecord
        /// </summary>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        private List<SamplingFormCRecord> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            var samplingFormCRecords = new List<SamplingFormCRecord>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormCDetails))
            {
                samplingFormCRecords = SamplingFormCRecordRepository.LoadEntities(this.EntityObjectSet, loadStrategyResult, null, LoadStrategy.Entities.SamplingFormCDetails);
            }

            /* SamplingFormC is parent entiry to SamplingFormCDetails
              * It will become circular call, if it's binding is implemented into LoadEntities function 
              * so it is better to implement it in this function*/
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormC))
            {
                // first result set includes the category
                new PaxMaterializers().SamplingFormCMaterializer.Materialize(reader).Bind(this.EntityObjectSet.Context.CreateObjectSet<SamplingFormC>()).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            return samplingFormCRecords;
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static List<SamplingFormCRecord> LoadEntities(ObjectSet<SamplingFormCRecord> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormCRecord> link, string entityName)
        {
            if (link == null)
                link = new Action<SamplingFormCRecord>(c => { });

            var samplingFormCRecords = new List<SamplingFormCRecord>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                samplingFormCRecords = new PaxMaterializers().SamplingFormCRecordMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            if (samplingFormCRecords.Count > 0)
            {
                //Load ChargeCodeType by calling respective LoadEntities method
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormCRecordAttachment))
                {
                    SamplingFormCAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormCRecordAttachment>(),
                                                      loadStrategyResult, null);
                    //The fetched child records should use the Parent entities.
                }
            }

            return samplingFormCRecords;
        }


        public long IsDuplicateSamplingFormCRecordExists(string ticketIssuingAirline, long ticketDocNumber, int couponNumber,
          string provisionalInvoiceNumber, int batchNumber, int sequenceNumber, int fromMemberId, int provisionalBillingMemberId, int provisionalBillingMonth, int provisionalBillingYear)
        {
            var parameters = new ObjectParameter[11];
            parameters[0] = new ObjectParameter(SamplingFormCRepositoryConstants.TicketIssueAirlineParameterName, typeof(string)) { Value = ticketIssuingAirline };
            parameters[1] = new ObjectParameter(SamplingFormCRepositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
            parameters[2] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvInvoiceNumberParameterName, typeof(string)) { Value = provisionalInvoiceNumber };

            parameters[3] = new ObjectParameter(SamplingFormCRepositoryConstants.CouponNumberParameterName, typeof(int)) { Value = couponNumber };
            parameters[4] = new ObjectParameter(SamplingFormCRepositoryConstants.BatchNumberParameterName, typeof(int)) { Value = batchNumber };
            parameters[5] = new ObjectParameter(SamplingFormCRepositoryConstants.SquenceNumberParameterName, typeof(int)) { Value = sequenceNumber };
            parameters[6] = new ObjectParameter(SamplingFormCRepositoryConstants.FromMemberIdParameterName, typeof(int)) { Value = fromMemberId };
            parameters[7] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMemberIdParameterName, typeof(int)) { Value = provisionalBillingMemberId };
            parameters[8] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMonthParameterName, typeof(int)) { Value = provisionalBillingMonth };
            parameters[9] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingYearParameterName, typeof(int)) { Value = provisionalBillingYear };

            parameters[10] = new ObjectParameter(SamplingFormCRepositoryConstants.RecordCountParameterName, typeof(long));

            ExecuteStoredProcedure(SamplingFormCRepositoryConstants.IsSamplingFormCRecordExistsMethodName, parameters);

            return long.Parse(parameters[10].Value.ToString());
        }


    }


}
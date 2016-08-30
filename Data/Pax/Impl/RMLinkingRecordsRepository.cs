using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Pax.Common;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class RMLinkingRecordsRepository : IRMLinkingRecordsRepository
    {
        public static List<RMLinkingRecords> LoadEntities(LoadStrategyResult loadStrategyResult, Action<RMLinkingRecords> link)
        {
            List<RMLinkingRecords> records = new List<RMLinkingRecords>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(Pax.Impl.RejectionMemoRecordRepositoryConstants.MemoRecordsParameterName))
            {
                if (reader != null && reader.HasRows)
                {
                    records = RMLinkingRecordsMaterializer.Materialize(reader).ToList();
                    if (!reader.IsClosed)
                        reader.Close();
                }

            }
            return records;
        }

        public static readonly Materializer<RMLinkingRecords> RMLinkingRecordsMaterializer = new Materializer<RMLinkingRecords>(ev =>
          new RMLinkingRecords
          {
              BatchSequenceNumber = ev.TryGetField<object>("BATCH_SEQ_NO") != null ? ev.Field<int>("BATCH_SEQ_NO") : 0,
              RecordSequenceWithinBatch = ev.TryGetField<object>("BATCH_RECORD_SEQ") != null ? ev.Field<int>("BATCH_RECORD_SEQ") : 0,
              MemoId = ev.TryGetField<byte[]>("COUPON_RECORD_ID") != null ? new Guid(ev.Field<byte[]>("COUPON_RECORD_ID")) : new Guid(),

          });
    }

    public class RMLinkingAmountRepository : IRMLinkingAmountRepository
    {
        public static RMLinkingAmount LoadEntities(LoadStrategyResult loadStrategyResult, Action<RMLinkingAmount> link)
        {
            List<RMLinkingAmount> amount = new List<RMLinkingAmount>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(Pax.Impl.RejectionMemoRecordRepositoryConstants.MemoAmountsParameterName))
            {
                if (reader != null && reader.HasRows)
                {
                    amount = RMLinkingAmountMaterializer.Materialize(reader).ToList();
                    if (!reader.IsClosed)
                        reader.Close();
                }

            }
            return amount != null && amount.Count > 0 ? amount[0] : null;
        }

        public static readonly Materializer<RMLinkingAmount> RMLinkingAmountMaterializer = new Materializer<RMLinkingAmount>(ev =>
          new RMLinkingAmount
          {
              TotalGrossAmountBilled = ev.TryGetField<object>("GROSS_AMT") != null ? ev.Field<double>("GROSS_AMT") : 0.0,
              TotalTaxAmountBilled = ev.TryGetField<object>("TAX_AMT") != null ? ev.Field<double>("TAX_AMT") : 0.0,
              AllowedIscAmount = ev.TryGetField<object>("ISC_AMT") != null ? ev.Field<double>("ISC_AMT") : 0.0,
              AllowedOtherCommission = ev.TryGetField<object>("OTH_COMM_AMT") != null ? ev.Field<double>("OTH_COMM_AMT") : 0.0,
              AllowedUatpAmount = ev.TryGetField<object>("UATP_AMT") != null ? ev.Field<double>("UATP_AMT") : 0.0,
              AllowedHandlingFee = ev.TryGetField<object>("HF_AMT") != null ? ev.Field<double>("HF_AMT") : 0.0,
              TotalVatAmountBilled = ev.TryGetField<object>("VAT_AMT") != null ? ev.Field<double>("VAT_AMT") : 0.0,

          });
    }

}

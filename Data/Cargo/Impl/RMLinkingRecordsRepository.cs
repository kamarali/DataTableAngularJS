using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Cargo.Common;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class RMLinkingRecordsRepository
  {
    public static List<CgoRMLinkingRecords> LoadEntities(LoadStrategyResult loadStrategyResult, Action<CgoRMLinkingRecords> link)
    {
      var records = new List<CgoRMLinkingRecords>();
      using (OracleDataReader reader = loadStrategyResult.GetReader(RejectionMemoRecordRepositoryConstants.MemoRecordsParameterName))
      {
        if (reader != null && reader.HasRows)
        {
          records = RMLinkingRecordsMaterializer.Materialize(reader).ToList();
          reader.Close();
        }
        
      }
      return records;
    }

    public static readonly Materializer<CgoRMLinkingRecords> RMLinkingRecordsMaterializer = new Materializer<CgoRMLinkingRecords>(ev =>
      new CgoRMLinkingRecords
      {
        BatchSequenceNumber = ev.TryGetField<object>("BATCH_SEQ_NO") != null ? ev.Field<int>("BATCH_SEQ_NO") : 0,
        RecordSequenceWithinBatch = ev.TryGetField<object>("BATCH_RECORD_SEQ") != null ? ev.Field<int>("BATCH_RECORD_SEQ") : 0,
        MemoId = ev.TryGetField<byte[]>("COUPON_RECORD_ID") != null ? new Guid(ev.Field<byte[]>("COUPON_RECORD_ID")) : new Guid()
      });
  }

  public class RMLinkingAmountRepository
  {
    public static CgoRMLinkingAmount LoadEntities(LoadStrategyResult loadStrategyResult, Action<CgoRMLinkingAmount> link)
    {
      var amount = new List<CgoRMLinkingAmount>();
      using (OracleDataReader reader = loadStrategyResult.GetReader(RejectionMemoRecordRepositoryConstants.MemoAmountsParameterName))
      {
        if (reader != null && reader.HasRows)
        {
          amount = RMLinkingAmountMaterializer.Materialize(reader).ToList();
          reader.Close();
        }
        
      }
      return amount.Count > 0 ? amount[0] : null;
    }

    public static readonly Materializer<CgoRMLinkingAmount> RMLinkingAmountMaterializer = new Materializer<CgoRMLinkingAmount>(ev =>
      new CgoRMLinkingAmount
      {
        BilledTotalWeightCharge = ev.TryGetField<object>("WEIGHT_AMT") != null ? Convert.ToDecimal(ev.Field<object>("WEIGHT_AMT")) : 0,
        BilledTotalValuationCharge = ev.TryGetField<object>("VAL_AMT") != null ? Convert.ToDecimal(ev.Field<object>("VAL_AMT")) : 0,
        AllowedTotalIscAmount = ev.TryGetField<object>("ISC_AMT") != null ? Convert.ToDecimal(ev.Field<object>("ISC_AMT")) : 0,
        BilledTotalOtherChargeAmount = ev.TryGetField<object>("OTH_CHARGE_AMT") != null ? Convert.ToDecimal(ev.Field<object>("OTH_CHARGE_AMT")) : 0,
        BilledTotalVatAmount = ev.TryGetField<object>("VAT_AMT") != null ? Convert.ToDecimal(ev.Field<object>("VAT_AMT")) : 0
      });
  }
}

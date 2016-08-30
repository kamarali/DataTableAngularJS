using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Cargo.Common;
using Devart.Data.Oracle;

using Microsoft.Data.Extensions;
using Iata.IS.Data.Impl;

namespace Iata.IS.Data.Cargo.Impl
{
  public class RMLinkedAwbRepository : IRMLinkedAwbRepository
  {
    public static List<RMLinkedAwb> LoadEntities(LoadStrategyResult loadStrategyResult, Action<RMLinkedAwb> link)
    {
      var records = new List<RMLinkedAwb>();
      using (OracleDataReader reader = loadStrategyResult.GetReader(RMAwbRepositoryConstants.AwbListParameterName))
      {
        if (reader != null && reader.HasRows)
        {
          records = RMLinkedAwbMaterializer.Materialize(reader).ToList();
          reader.Close();
        }
      }

      return records;
    }


    public static readonly Materializer<RMLinkedAwb> RMLinkedAwbMaterializer = new Materializer<RMLinkedAwb>(ev =>
      new RMLinkedAwb
      {
        BreakdownSerialNo = ev.TryGetField<object>("BD_SR_NO") != null ? ev.Field<int>("BD_SR_NO") : 0,
        RMAwbId = ev.TryGetField<byte[]>("AWB_BD_ID") != null ? new Guid(ev.Field<byte[]>("AWB_BD_ID")) : new Guid(),
        RecordSeqNumber = ev.TryGetField<object>("BATCH_RECORD_SEQ") != null ? ev.Field<int>("BATCH_RECORD_SEQ") : 0,
        BatchNumber = ev.TryGetField<object>("BATCH_SEQ_NO") != null ? ev.Field<int>("BATCH_SEQ_NO") : 0,
      });

  }
}

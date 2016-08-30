using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Pax.Common;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;
using Iata.IS.Data.Impl;

namespace Iata.IS.Data.Pax.Impl
{
    public class RMLinkedCouponRepository : IRMLinkedCouponRepository
    {
        public static List<RMLinkedCoupon> LoadEntities(LoadStrategyResult loadStrategyResult, Action<RMLinkedCoupon> link)
        {
            List<RMLinkedCoupon> records = new List<RMLinkedCoupon>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(Pax.Impl.RMCouponBreakdownRecordRepositoryConstants.CouponListParameterName))
            {
                if (reader != null && reader.HasRows)
                {
                    records = RMLinkedCouponMaterializer.Materialize(reader).ToList();
                    if (!reader.IsClosed)
                        reader.Close();
                }
            }
            return records;
        }


        public static readonly Materializer<RMLinkedCoupon> RMLinkedCouponMaterializer = new Materializer<RMLinkedCoupon>(ev =>
          new RMLinkedCoupon
          {
              BreakdownSerialNo = ev.TryGetField<object>("BD_SR_NO") != null ? ev.Field<int>("BD_SR_NO") : 0,
              CouponId = ev.TryGetField<byte[]>("COUPON_BD_ID") != null ? new Guid(ev.Field<byte[]>("COUPON_BD_ID")) : new Guid(),
              RecordSeqNumber = ev.TryGetField<object>("BATCH_RECORD_SEQ") != null ? ev.Field<int>("BATCH_RECORD_SEQ") : 0,
              BatchNumber = ev.TryGetField<object>("BATCH_SEQ_NO") != null ? ev.Field<int>("BATCH_SEQ_NO") : 0,
          });

    }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.AutoBilling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Isr.Impl
{
  class PrimeCouponMarketingDetailsRepository : Repository<PrimeCouponMarketingDetails>, IPrimeCouponMarketingDetailsRepository
  {
    public static List<PrimeCouponMarketingDetails> LoadEntities(ObjectSet<PrimeCouponMarketingDetails> objectSet, LoadStrategyResult loadStrategyResult, Action<PrimeCouponMarketingDetails> link)
    {
      if (link == null)
        link = new Action<PrimeCouponMarketingDetails>(c => { });

      var couponMarketingDetails = new List<PrimeCouponMarketingDetails>();

      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CouponMarketingDetails))
      {
        // first result set includes the category
        foreach (var c in
            new PaxMaterializers().CouponTktMarketingMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<PrimeCouponMarketingDetails>(link)
            )
        {
          couponMarketingDetails.Add(c);
        }
        reader.Close();
      }

      return couponMarketingDetails;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.AutoBilling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class AutoBillingCouponTaxRecordRepository : Repository<AutoBillingPrimeCouponTax>, IAutoBillingCouponTaxRecordRepository
    {
      public static List<AutoBillingPrimeCouponTax> LoadEntities(ObjectSet<AutoBillingPrimeCouponTax> objectSet, LoadStrategyResult loadStrategyResult, Action<AutoBillingPrimeCouponTax> link)
      {
        if (link == null)
          link = new Action<AutoBillingPrimeCouponTax>(c => { });

        var autoBillCouponTaxs = new List<AutoBillingPrimeCouponTax>();

        using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.AutoBillCouponTax))
        {
          // first result set includes the category
          foreach (var c in
              new PaxMaterializers().AutoBillCouponTaxMaterializer
              .Materialize(reader)
              .Bind(objectSet)
              .ForEach<AutoBillingPrimeCouponTax>(link)
              )
          {
            autoBillCouponTaxs.Add(c);
          }
          reader.Close();
        }
        return autoBillCouponTaxs;
      }

    }
}

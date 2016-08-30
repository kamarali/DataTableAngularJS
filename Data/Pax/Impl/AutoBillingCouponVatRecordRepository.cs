using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class AutoBillingCouponVatRecordRepository : Repository<AutoBillingPrimeCouponVat>, IAutoBillingCouponVatRecordRepository
    {

      public static List<AutoBillingPrimeCouponVat> LoadEntities(ObjectSet<AutoBillingPrimeCouponVat> objectSet, LoadStrategyResult loadStrategyResult, Action<AutoBillingPrimeCouponVat> link)
      {
        if (link == null)
          link = new Action<AutoBillingPrimeCouponVat>(c => { });

        List<AutoBillingPrimeCouponVat> autoBillCouponVats = new List<AutoBillingPrimeCouponVat>();

        using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.AutoBillCouponVat))
        {
          // first result set includes the category
          foreach (var c in
              new PaxMaterializers().AutoBillCouponVatMaterializer
              .Materialize(reader)
              .Bind(objectSet)
              .ForEach<AutoBillingPrimeCouponVat>(link)
              )
          {
            autoBillCouponVats.Add(c);
          }
          reader.Close();
        }

        //Load PrimeCouponVatIdentifier
        if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CouponDataVatIdentifier) && autoBillCouponVats.Count > 0)
        {
          VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>()
                 , loadStrategyResult
                 , null, LoadStrategy.Entities.CouponDataVatIdentifier);
        }

        return autoBillCouponVats;
      }

    }
}

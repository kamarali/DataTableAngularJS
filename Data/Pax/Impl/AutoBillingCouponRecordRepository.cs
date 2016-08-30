using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.AutoBilling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class AutoBillingCouponRecordRepository : Repository<AutoBillingPrimeCoupon>, IAutoBillingCouponRecordRepository
    {
       public AutoBillingPrimeCoupon GetCouponWithAllDetails(Guid couponRecordId)
        {
            var couponRecord = EntityObjectSet
                               .Include("AutoBillingTaxBreakdown")
                               .Include("AutoBillingVatBreakdown")
                               .Include("AutoBillingAttachments").SingleOrDefault(i => i.Id == couponRecordId);

            return couponRecord;
        }

       public static List<AutoBillingPrimeCoupon> LoadEntities(ObjectSet<AutoBillingPrimeCoupon> objectSet, LoadStrategyResult loadStrategyResult, Action<AutoBillingPrimeCoupon> link)
       {
         if (link == null)
           link = new Action<AutoBillingPrimeCoupon>(c => { });
         List<AutoBillingPrimeCoupon> autoBillCoupons = new List<AutoBillingPrimeCoupon>();
         using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.AutoBillCoupon))
         {
           // first result set includes the category
           foreach (var c in
               new PaxMaterializers().AutoBillCouponMaterializer
               .Materialize(reader)
               .Bind(objectSet)
               .ForEach<AutoBillingPrimeCoupon>(link)
               )
           {
             autoBillCoupons.Add(c);
           }
           reader.Close();
         }

         //Load PrimeCouponTax by calling respective LoadEntities method
         if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AutoBillCouponTax) && autoBillCoupons.Count != 0)
           AutoBillingCouponTaxRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<AutoBillingPrimeCouponTax>()
                   , loadStrategyResult
                   , null);

         //Load PrimeCouponVat by calling respective LoadEntities method
         if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AutoBillCouponVat) && autoBillCoupons.Count != 0)
           AutoBillingCouponVatRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<AutoBillingPrimeCouponVat>()
                   , loadStrategyResult
                   , null);

         //Load PrimeCouponVat by calling respective LoadEntities method
         if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AutoBillCouponAttach) && autoBillCoupons.Count != 0)
           AutoBillingCouponRecordAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<AutoBillingPrimeCouponAttachment>()
                   , loadStrategyResult
                   , null);

         return autoBillCoupons;
       }

      /// <summary>
      /// To Update the prime Coupon status and AutoBill primeCoupon status and Is_included in daily report falg.
      /// </summary>
      /// <param name="primeCouponIds"></param>
      /// <param name="autoBillprimeCouponIds"></param>
       public void UpdateAutoBillCouponStatus(string primeCouponIds, string autoBillprimeCouponIds)
       {
         var parameters = new ObjectParameter[2];
         parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.CouponIdsParameterName, typeof(string)) { Value = primeCouponIds };
         parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.AutoBillingCouponIdsParameterName, typeof(string)) { Value = autoBillprimeCouponIds };

         ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateAutoBillCouponStatusFunctionName, parameters);
       }
    }
}

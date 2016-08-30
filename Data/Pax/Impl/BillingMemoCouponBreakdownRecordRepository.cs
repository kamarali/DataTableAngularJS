using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class BillingMemoCouponBreakdownRecordRepository : Repository<BMCoupon>, IBillingMemoCouponBreakdownRecordRepository
    {
        public override BMCoupon Single(System.Linq.Expressions.Expression<Func<BMCoupon, bool>> where)
        {
            throw new NotImplementedException("Use Load Strategy overload of Single method instead.");
        }

        /// <summary>
        /// Get Single Billing Memo Coupon record
        /// </summary>
        /// <param name="billingMemoCouponId">The Billing Memo Coupon Id</param>
        /// <returns>Single record of Billing Memo Coupon</returns>
        public BMCoupon Single(Guid billingMemoCouponId)
        {
            var entities = new string[]
                       {
                         LoadStrategy.Entities.BillingMemoCoupon, LoadStrategy.Entities.BillingMemo, LoadStrategy.Entities.BillingMemoCouponVat, LoadStrategy.Entities.BMCouponVatIdentifier,
                         LoadStrategy.Entities.BMCouponAttachments, LoadStrategy.Entities.BillingMemoCouponTax
                       };

            List<BMCoupon> bmCoupons = GetbmCouponsLS(new LoadStrategy(string.Join(",", entities)), billingMemoCouponId: billingMemoCouponId);
            var id = ConvertUtil.ConvertGuidToString(billingMemoCouponId);

            if (bmCoupons == null || bmCoupons.Count == 0)
            {
                return null;
            }
            else if (bmCoupons.Count > 1)
            {
                throw new ApplicationException("Multiple records found");
            }
            else
            {
                return bmCoupons[0];
            }
        }

        private List<BMCoupon> GetbmCouponsLS(LoadStrategy loadStrategy, Guid? billingMemoId = null, Guid? billingMemoCouponId = null)
        {
            return base.ExecuteLoadsSP(SisStoredProcedures.GetBillingMemoOrBMCoupon,
                                       loadStrategy,
                                       new OracleParameter[]
                                   {
                                     new OracleParameter(BillingMemoRecordRepositoryConstants.BillingMemoIdParameterName,
                                                         billingMemoId.HasValue ? ConvertUtil.ConvertGuidToString(billingMemoId.Value) : null),
                                     new OracleParameter(BillingMemoRecordRepositoryConstants.BillingMemoCouponIdParameterName,
                                                         billingMemoCouponId.HasValue ? ConvertUtil.ConvertGuidToString(billingMemoCouponId.Value) : null)
                                   },
                                       r => this.FetchRecord(r));
        }

        private List<BMCoupon> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            List<BMCoupon> bmCoupons = new List<BMCoupon>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemoCoupon))
            {
                bmCoupons = BillingMemoCouponBreakdownRecordRepository.LoadEntities(this.EntityObjectSet, loadStrategyResult, null, LoadStrategy.Entities.BillingMemoCoupon);
            }
            if (bmCoupons != null && bmCoupons.Count > 0 && loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemo))
            {
                using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.BillingMemo))
                {
                    new PaxMaterializers().BillingMemoMaterializer.Materialize(reader).Bind(this.EntityObjectSet.Context.CreateObjectSet<BillingMemo>()).ToList();
                    if (!reader.IsClosed)
                        reader.Close();
                }
            }
            return bmCoupons;
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
        public static List<BMCoupon> LoadEntities(ObjectSet<BMCoupon> objectSet, LoadStrategyResult loadStrategyResult, Action<BMCoupon> link, string entityName)
        {
            if (link == null)
            {
                link = new Action<BMCoupon>(c => { });
            }

            var bmCoupons = new List<BMCoupon>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                bmCoupons = new PaxMaterializers().BMCouponMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            if (bmCoupons.Count > 0)
            {
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemoCouponVat))
                {
                    BillingMemoCouponVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMCouponVat>(), loadStrategyResult, null, LoadStrategy.Entities.BillingMemoCouponVat);
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BMCouponVatIdentifier))
                {
                    VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>(), loadStrategyResult, null, LoadStrategy.Entities.BMCouponVatIdentifier);
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BMCouponAttachments))
                {
                    BillingMemoCouponAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMCouponAttachment>(), loadStrategyResult, null, LoadStrategy.Entities.BMCouponAttachments);
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemoCouponTax))
                {
                    BillingMemoCouponTaxRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMCouponTax>(), loadStrategyResult, null, LoadStrategy.Entities.BillingMemoCouponTax);
                }
            }
            return bmCoupons;
        }

        public static List<BMCoupon> LoadAuditEntities(ObjectSet<BMCoupon> objectSet, LoadStrategyResult loadStrategyResult, Action<BMCoupon> link, string entityName)
        {
            if (link == null)
            {
                link = new Action<BMCoupon>(c => { });
            }

            var bmCoupons = new List<BMCoupon>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                bmCoupons = new PaxMaterializers().BMCouponMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            if (bmCoupons.Count > 0)
            {
                if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.BMCouponVAT))
                {
                    BillingMemoCouponVatRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<BMCouponVat>(), loadStrategyResult, null, LoadStrategy.PaxEntities.BMCouponVAT);
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.BMCouponATTACHMENT))
                {
                    BillingMemoCouponAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<BMCouponAttachment>(), loadStrategyResult, null, LoadStrategy.PaxEntities.BMCouponATTACHMENT);
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.BMCouponTax))
                {
                    BillingMemoCouponTaxRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<BMCouponTax>(), loadStrategyResult, null, LoadStrategy.PaxEntities.BMCouponTax);
                }
            }

            return bmCoupons;
        }

        /// <summary>
        /// Gets the billing memo coupon duplicate count.
        /// </summary>
        /// <param name="ticketCouponNumber">The ticket coupon number.</param>
        /// <param name="ticketDocNumber">The ticket doc number.</param>
        /// <param name="issuingAirline">The issuing airline.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <param name="billingYear">The billing year.</param>
        /// <param name="billingMonth">The billing month.</param>
        /// <returns></returns>
        public long GetBillingMemoCouponDuplicateCount(int ticketCouponNumber, long ticketDocNumber, string issuingAirline, int billingMemberId, int billedMemberId, int billingYear, int billingMonth)
        {
            var parameters = new ObjectParameter[8];
            parameters[0] = new ObjectParameter(CouponRecordRespositoryConstants.TicketCouponNumberParameterName, typeof(int))
                              {
                                  Value = ticketCouponNumber
                              };
            parameters[1] = new ObjectParameter(CouponRecordRespositoryConstants.TicketDocNumberParameterName, typeof(long))
                              {
                                  Value = ticketDocNumber
                              };
            parameters[2] = new ObjectParameter(CouponRecordRespositoryConstants.IssuingAirlineParameterName, typeof(string))
                              {
                                  Value = issuingAirline
                              };
            parameters[3] = new ObjectParameter(CouponRecordRespositoryConstants.BillingMemberParameterName, typeof(int))
                              {
                                  Value = billingMemberId
                              };
            parameters[4] = new ObjectParameter(CouponRecordRespositoryConstants.BilledMemberParameterName, typeof(int))
                              {
                                  Value = billedMemberId
                              };
            parameters[5] = new ObjectParameter(CouponRecordRespositoryConstants.BillingYearParameterName, typeof(int))
                              {
                                  Value = billingYear
                              };
            parameters[6] = new ObjectParameter(CouponRecordRespositoryConstants.BillingMonthParameterName, typeof(int))
                              {
                                  Value = billingMonth
                              };

            parameters[7] = new ObjectParameter(CouponRecordRespositoryConstants.DuplicateCountParameterName, typeof(long));

            ExecuteStoredProcedure(BillingMemoRecordRepositoryConstants.GetBillingMemoCouponDuplicateCountFunctionName, parameters);

            return long.Parse(parameters[7].Value.ToString());
        }

        /// <summary>
        /// Get Single Billing Memo Coupon record having Billing memo record within it
        /// </summary>
        /// <param name="billingMemoCouponId">The Billing Memo Coupon Id</param>
        /// <returns>Single record of Billing Memo Coupon</returns>
        public BMCoupon GetBillingMemoWithCoupon(Guid billingMemoCouponId)
        {
          var entities = new string[]
                       {
                         LoadStrategy.Entities.BillingMemoCoupon, LoadStrategy.Entities.BillingMemo
                       };

          List<BMCoupon> bmCoupons = GetbmCouponsLS(new LoadStrategy(string.Join(",", entities)), billingMemoCouponId: billingMemoCouponId);
          var id = ConvertUtil.ConvertGuidToString(billingMemoCouponId);

          if (bmCoupons == null || bmCoupons.Count == 0)
          {
            return null;
          }
          else if (bmCoupons.Count > 1)
          {
            throw new ApplicationException("Multiple records found");
          }
          else
          {
            return bmCoupons[0];
          }
        }
    }
}
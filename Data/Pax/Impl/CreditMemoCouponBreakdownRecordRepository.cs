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
    public class CreditMemoCouponBreakdownRecordRepository : Repository<CMCoupon>, ICreditMemoCouponBreakdownRecordRepository
    {
        public override CMCoupon Single(System.Linq.Expressions.Expression<Func<CMCoupon, bool>> where)
        {
            throw new NotImplementedException("Use Load Strategy overload of Single method instead.");
        }

        /// <summary>
        /// Gets the credit memo coupon duplicate count.
        /// </summary>
        /// <param name="ticketCouponNumber">The ticket coupon number.</param>
        /// <param name="ticketDocNumber">The ticket doc number.</param>
        /// <param name="issuingAirline">The issuing airline.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <param name="billingYear">The billing year.</param>
        /// <param name="billingMonth">The billing month.</param>
        /// <returns></returns>
        public long GetCreditMemoCouponDuplicateCount(int ticketCouponNumber, long ticketDocNumber, string issuingAirline, int billingMemberId, int billedMemberId, int billingYear, int billingMonth)
        {
            var parameters = new ObjectParameter[8];
            parameters[0] = new ObjectParameter(CouponRecordRespositoryConstants.TicketCouponNumberParameterName, typeof(int)) { Value = ticketCouponNumber };
            parameters[1] = new ObjectParameter(CouponRecordRespositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
            parameters[2] = new ObjectParameter(CouponRecordRespositoryConstants.IssuingAirlineParameterName, typeof(string)) { Value = issuingAirline };
            parameters[3] = new ObjectParameter(CouponRecordRespositoryConstants.BillingMemberParameterName, typeof(int)) { Value = billingMemberId };
            parameters[4] = new ObjectParameter(CouponRecordRespositoryConstants.BilledMemberParameterName, typeof(int)) { Value = billedMemberId };
            parameters[5] = new ObjectParameter(CouponRecordRespositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
            parameters[6] = new ObjectParameter(CouponRecordRespositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };

            parameters[7] = new ObjectParameter(CouponRecordRespositoryConstants.DuplicateCountParameterName, typeof(long));

            ExecuteStoredProcedure(CreditMemoRecordRepositoryConstants.GetCreditMemoDuplicateCountFunctionName, parameters);

            return long.Parse(parameters[7].Value.ToString());
        }

        #region Load Strategy

        /// <summary>
        /// LoadStrategy method overload of Single method
        /// </summary>
        /// <param name="cmCouponId">CMCouponBreakdown Id</param>
        /// <returns>CMCoupon object</returns>
        public CMCoupon Single(Guid cmCouponId)
        {
            var entities = new string[] { LoadStrategy.Entities.CreditMemoCoupon,LoadStrategy.Entities.CreditMemoCouponVat,LoadStrategy.Entities.CreditMemoCouponVatIdentifier,
                                      LoadStrategy.Entities.CreditMemoCouponAttachments,LoadStrategy.Entities.CreditMemoCouponTax,LoadStrategy.Entities.CreditMemo};

            var loadStrategy = new LoadStrategy(string.Join(",", entities));

            var cmCoupons = GetCMCouponLS(loadStrategy, ConvertUtil.ConvertGuidToString(cmCouponId));

            if (cmCoupons.Count > 0)
            {
                if (cmCoupons.Count > 1) throw new ApplicationException("Multiple records found");
                return cmCoupons[0];
            }
            return null;
        }

        /// <summary>
        ///  Gets list of RMCoupon objects
        /// </summary>
        /// <param name="loadStrategy">loadStrategy string</param>
        /// <param name="cmCouponId">rmCouponId</param>
        /// <returns>List of RMCoupon objects</returns>
        private List<CMCoupon> GetCMCouponLS(LoadStrategy loadStrategy, string cmCouponId)
        {
            return base.ExecuteLoadsSP(SisStoredProcedures.GetCreditMemo,
                                      loadStrategy,
                                      new OracleParameter[] { new OracleParameter(CreditMemoRecordRepositoryConstants.CreditIdParameterName, null),  
                                    new OracleParameter(CreditMemoRecordRepositoryConstants.CMCouponIdparameterName, cmCouponId) ,
                                  },
                                      this.FetchRecord);
        }

        /// <summary>
        /// Fetches the record.
        /// </summary>
        /// <param name="loadStrategyResult">The load strategy result.</param>
        /// <returns></returns>
        private List<CMCoupon> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            var cmCoupons = new List<CMCoupon>();
            var creditMemos = new List<CreditMemo>();
            /* CreditMemo Memo is parent entity to CreditMemoCoupon
             * It will become circular call, if it's binding is implemented into LoadEntities function 
             * so it is better to implement it in this function*/
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemo))
            {
                using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CreditMemo))
                {
                    // first result set includes the category
                    creditMemos = new PaxMaterializers().CreditMemoMaterializer.Materialize(reader).Bind(this.EntityObjectSet.Context.CreateObjectSet<CreditMemo>()).ToList();
                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemoCoupon))
            {
                cmCoupons = CreditMemoCouponBreakdownRecordRepository.LoadEntities(this.EntityObjectSet, loadStrategyResult, cmc => cmc.CreditMemoRecord = creditMemos.Find(cm => cm.Id == cmc.CreditMemoId));
            }


            return cmCoupons;
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<CMCoupon> LoadEntities(ObjectSet<CMCoupon> objectSet, LoadStrategyResult loadStrategyResult, Action<CMCoupon> link)
        {
            if (link == null)
                link = new Action<CMCoupon>(c => { });
            var cmCoupons = new List<CMCoupon>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CreditMemoCoupon))
            {
                foreach (var c in
                    new PaxMaterializers().CMCouponMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<CMCoupon>(link)
                    )
                {
                    cmCoupons.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load CreditMemoCouponVat by calling respective LoadEntity
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemoCouponVat) && cmCoupons.Count != 0)
            {
                CreditMemoCouponVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMCouponVat>()
                         , loadStrategyResult
                         , null);
            }

            // Load CreditMemoCouponAttachments
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemoCouponAttachments) && cmCoupons.Count != 0)
            {
                CreditMemoCouponAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMCouponAttachment>()
                          , loadStrategyResult
                          , null);
            }

            // Load CreditMemoCouponTax
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemoCouponTax) && cmCoupons.Count != 0)
            {
                CreditMemoCouponTaxRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMCouponTax>()
                          , loadStrategyResult
                          , null);
            }

            return cmCoupons;
        }

        /// <summary>
        /// LoadStrategy method overload of Single method
        /// </summary>
        /// <param name="cmCouponId">CMCouponBreakdown Id</param>
        /// <returns>CMCoupon object</returns>
        public CMCoupon GetCmCouponWithCreditMemoObject(Guid cmCouponId)
        {
          var entities = new string[] { LoadStrategy.Entities.CreditMemoCoupon, LoadStrategy.Entities.CreditMemo};

          var loadStrategy = new LoadStrategy(string.Join(",", entities));

          var cmCoupons = GetCMCouponLS(loadStrategy, ConvertUtil.ConvertGuidToString(cmCouponId));

          if (cmCoupons.Count > 0)
          {
            if (cmCoupons.Count > 1) throw new ApplicationException("Multiple records found");
            return cmCoupons[0];
          }
          return null;
        }
        #endregion
    }
}

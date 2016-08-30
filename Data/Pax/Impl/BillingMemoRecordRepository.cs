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
    public class BillingMemoRecordRepository : Repository<BillingMemo>, IBillingMemoRecordRepository
    {
        public long GetBillingMemoDuplicateCount(string billingMemoNumber, int billedMemberId, int billingMemberId, int billingMonth, int billingYear, int billingPeriod)
        {
            var parameters = new ObjectParameter[7];
            parameters[0] = new ObjectParameter(BillingMemoRecordRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
            parameters[1] = new ObjectParameter(BillingMemoRecordRepositoryConstants.BillingMemoNumberParameterName, typeof(string)) { Value = billingMemoNumber };
            parameters[2] = new ObjectParameter(BillingMemoRecordRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
            parameters[3] = new ObjectParameter(BillingMemoRecordRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };
            parameters[4] = new ObjectParameter(BillingMemoRecordRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
            parameters[5] = new ObjectParameter(CreditMemoRecordRepositoryConstants.BillingPeriodParameterName, typeof(int)) { Value = billingPeriod };
            parameters[6] = new ObjectParameter(BillingMemoRecordRepositoryConstants.DuplicateCountParameterName, typeof(long));


            ExecuteStoredProcedure(BillingMemoRecordRepositoryConstants.GetBMDuplicateCountFunctionName, parameters);

            return long.Parse(parameters[6].Value.ToString());
        }

        public override BillingMemo Single(System.Linq.Expressions.Expression<Func<BillingMemo, bool>> where)
        {
            throw new NotImplementedException("Use Load Strategy overload of Single method instead.");
        }

        /// <summary>
        /// Get Single Billing Memo record
        /// </summary>
        /// <param name="billingMemoId">The Billing Memo Id</param>
        /// <returns>Single record of Billing Memo</returns>
        public BillingMemo Single(Guid billingMemoId)
        {
            var entities = new string[] 
                        { 
                            LoadStrategy.Entities.BillingMemo, LoadStrategy.Entities.BillingMemoVat,
                            LoadStrategy.Entities.BillingMemoVatIdentifier, LoadStrategy.Entities.BillingMemoAttachments,
                            LoadStrategy.Entities.BillingMemoCoupon,LoadStrategy.Entities.AttachmentUploadedbyUser
                        };

            List<BillingMemo> billingMemos = GetBillingMemoLS(new LoadStrategy(string.Join(",", entities)), billingMemoId: billingMemoId);
            if (billingMemos == null || billingMemos.Count == 0) return null;
            else if (billingMemos.Count > 1) throw new ApplicationException("Multiple records found");
            else return billingMemos[0];
        }


        private List<BillingMemo> GetBillingMemoLS(LoadStrategy loadStrategy, Guid? billingMemoId = null, Guid? billingMemoCouponId = null)
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

        private List<BillingMemo> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            List<BillingMemo> billingMemos = new List<BillingMemo>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemo))
            {
                billingMemos = BillingMemoRecordRepository.LoadEntities(this.EntityObjectSet, loadStrategyResult, null);
            }
            return billingMemos;
        }
        /// <summary>
        /// This will load list of BillingMemo objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<BillingMemo> LoadEntities(ObjectSet<BillingMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<BillingMemo> link)
        {
            if (link == null)
                link = new Action<BillingMemo>(c => { });

            var billingMemos = new List<BillingMemo>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.BillingMemo))
            {

                billingMemos = new PaxMaterializers().BillingMemoMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            if (billingMemos.Count > 0)
            {
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemoVat))
                {
                    BillingMemoVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<BillingMemoVat>(), loadStrategyResult, null, LoadStrategy.Entities.BillingMemoVat);
                }

                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemoVatIdentifier))
                {
                    VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>(), loadStrategyResult, null, LoadStrategy.Entities.BillingMemoVatIdentifier);
                }

                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemoAttachments))
                {
                    BillingMemoAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<BillingMemoAttachment>(), loadStrategyResult, null, LoadStrategy.Entities.BillingMemoAttachments);
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemoCoupon))
                {
                    BillingMemoCouponBreakdownRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMCoupon>(),
                                                    loadStrategyResult,
                                                    (bmc => bmc.BillingMemo = billingMemos.Find(bm => bm.Id == bmc.BillingMemoId)),
                                                    LoadStrategy.Entities.BillingMemoCoupon);
                }
            }
            return billingMemos;
        }


        public static List<BillingMemo> LoadAuditEntities(ObjectSet<BillingMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<BillingMemo> link, string entity)
        {
            if (link == null)
                link = new Action<BillingMemo>(c => { });
            List<BillingMemo> billingMemo = new List<BillingMemo>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.BillingMemo))
            {
                // first result set includes the category
                foreach (var c in
                  new PaxMaterializers().PaxInvoiceBillingMemoAuditMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<BillingMemo>(link)
                  )
                {
                    billingMemo.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            if (billingMemo.Count > 0)
            {
                if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.BMVAT))
                {
                    BillingMemoVatRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<BillingMemoVat>(), loadStrategyResult, null, LoadStrategy.PaxEntities.BMVAT);
                }

                if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.BMAttachment))
                {
                    BillingMemoAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<BillingMemoAttachment>(), loadStrategyResult, null, LoadStrategy.PaxEntities.BMAttachment);
                }
            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.BillingMemoCoupon) && billingMemo.Count != 0)
            {
                BillingMemoCouponBreakdownRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<BMCoupon>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.PaxEntities.BillingMemoCoupon);
            }

            return billingMemo;
        }

    }
}

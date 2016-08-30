using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class CreditMemoRecordRepository : Repository<CreditMemo>, ICreditMemoRecordRepository
    {
        public long GetCreditMemoDuplicateCount(string creditMemoNumber, int billedMemberId, int billingMemberId, int billingMonth, int billingYear, int billingPeriod)
        {
            var parameters = new ObjectParameter[7];
            parameters[0] = new ObjectParameter(CreditMemoRecordRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
            parameters[1] = new ObjectParameter(CreditMemoRecordRepositoryConstants.CreditMemoNumberParameterName, typeof(string)) { Value = creditMemoNumber };
            parameters[2] = new ObjectParameter(CreditMemoRecordRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
            parameters[3] = new ObjectParameter(CreditMemoRecordRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };
            parameters[4] = new ObjectParameter(CreditMemoRecordRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
            parameters[5] = new ObjectParameter(CreditMemoRecordRepositoryConstants.BillingPeriodParameterName, typeof(int)) { Value = billingPeriod };

            parameters[6] = new ObjectParameter(CreditMemoRecordRepositoryConstants.DuplicateCountParameterName, typeof(long));

            ExecuteStoredProcedure(CreditMemoRecordRepositoryConstants.GetCmDuplicateCountFunctionName, parameters);

            return long.Parse(parameters[6].Value.ToString());
        }


        /// <summary>
        /// Singles the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public override CreditMemo Single(System.Linq.Expressions.Expression<Func<CreditMemo, bool>> where)
        {
            throw new NotImplementedException("Use Load Strategy overload of Single method instead.");
        }

        #region - LoadStrategy -

        /// <summary>
        ///  LoadStrategy method overload of Single method
        /// </summary>
        /// <param name="creditMemoId">CreditMemo Id</param>
        /// <returns>CreditMemo</returns>
        public CreditMemo Single(Guid creditMemoId)
        {
            var entities = new[] { LoadStrategy.Entities.CreditMemo,LoadStrategy.Entities.CreditMemoVat,LoadStrategy.Entities.CreditMemoVatIdentifier,
                                      LoadStrategy.Entities.CreditMemoAttachments,LoadStrategy.Entities.CreditMemoCoupon,LoadStrategy.Entities.AttachmentUploadedbyUser};

            var loadStrategy = new LoadStrategy(string.Join(",", entities));

            var creditMemos = GetCreditMemoLS(loadStrategy, ConvertUtil.ConvertGuidToString(creditMemoId));
            if (creditMemos.Count > 0)
            {
                if (creditMemos.Count > 1) throw new ApplicationException("Multiple records found");
                return creditMemos[0];
            }
            return null;
        }

        /// <summary>
        /// Gets the list of Credit 
        /// </summary>
        /// <param name="loadStrategy">loadStrategy</param>
        /// <param name="creditMemoId">creditMemoId</param>
        /// <returns>RejectionMemo</returns>

        private List<CreditMemo> GetCreditMemoLS(LoadStrategy loadStrategy, string creditMemoId)
        {
            return base.ExecuteLoadsSP(SisStoredProcedures.GetCreditMemo,
                                      loadStrategy,
                                          new OracleParameter[] { new OracleParameter(CreditMemoRecordRepositoryConstants.CreditIdParameterName, creditMemoId),  
                                    new OracleParameter(CreditMemoRecordRepositoryConstants.CMCouponIdparameterName, null) 
                                  },
                                      this.FetchRecord);
        }


        /// <summary>
        /// Fetches the record.
        /// </summary>
        /// <param name="loadStrategyResult">The load strategy result.</param>
        /// <returns></returns>
        private List<CreditMemo> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            var creditMemos = new List<CreditMemo>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemo))
            {
                creditMemos = CreditMemoRecordRepository.LoadEntities(base.EntityObjectSet, loadStrategyResult, null);
            }
            return creditMemos;
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<CreditMemo> LoadEntities(ObjectSet<CreditMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<CreditMemo> link)
        {
            if (link == null)
                link = new Action<CreditMemo>(c => { });

            var creditMemos = new List<CreditMemo>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CreditMemo))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().CreditMemoMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<CreditMemo>(link)
                    )
                {
                    creditMemos.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load CreditMemoVat by calling respective LoadEntity
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemoVat) && creditMemos.Count != 0)
            {
                CreditMemoVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<CreditMemoVat>()
                          , loadStrategyResult
                          , null);
            }

            // Load CreditMemoAttachments
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemoAttachments) && creditMemos.Count != 0)
            {
                CreditMemoAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CreditMemoAttachment>()
                          , loadStrategyResult
                          , null);
            }

            //Load CreditMemoCoupon by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemoCoupon) && creditMemos.Count != 0)
            {
                CreditMemoCouponBreakdownRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMCoupon>()
                        , loadStrategyResult
                        , i => i.CreditMemoRecord = creditMemos.Find(j => j.Id == i.CreditMemoId));
            }
            return creditMemos;
        }

        #endregion
    }
}

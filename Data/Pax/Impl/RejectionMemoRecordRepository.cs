using System;
using System.Configuration;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;
using System.Collections.Generic;
using Devart.Data.Oracle;
using Iata.IS.Model.Pax.Enums;
using Microsoft.Data.Extensions;
using System.Data;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Data.Pax.Impl
{
    public class RejectionMemoRecordRepository : Repository<RejectionMemo>, IRejectionMemoRecordRepository
    {
        public long GetRejectionMemoDuplicateCount(int billedMemberId, int billingMemberId, string rejectionMemoNumber, int billingYear, int billingMonth, int billingPeriod)
        {
            var parameters = new ObjectParameter[7];
            parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
            parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoNumberParameterName, typeof(string)) { Value = rejectionMemoNumber };
            parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
            parameters[3] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };
            parameters[4] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingPeriodParameterName, typeof(int)) { Value = billingPeriod };
            parameters[5] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
            parameters[6] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.DuplicateCountParameterName, typeof(long));

            ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.GetRMDuplicateCountFunctionName, parameters);

            return long.Parse(parameters[6].Value.ToString());
        }

        public string GetDuplicateRejectionMemoNumbers(int billedMemberId, int billingMemberId, string rejectionMemoNumber, int billingYear, int billingMonth, int billingPeriod)
        {
            var parameters = new ObjectParameter[7];
            parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BilledMemberIdRMParameterName, typeof(int)) { Value = billedMemberId };
            parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoNumberRMParameterName, typeof(string)) { Value = rejectionMemoNumber };
            parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingYearRMParameterName, typeof(int)) { Value = billingYear };
            parameters[3] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMonthRMParameterName, typeof(int)) { Value = billingMonth };
            parameters[4] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingPeriodRMParameterName, typeof(int)) { Value = billingPeriod };
            parameters[5] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMemberIdRMParameterName, typeof(int)) { Value = billingMemberId };
            parameters[6] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.DuplicateRejectionMemoNosParameterName, typeof(string));

            ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.GetDuplicateRejectionMemoNumbersFunctionName, parameters);

            return parameters[6].Value.ToString();
        }

        public override RejectionMemo Single(System.Linq.Expressions.Expression<Func<RejectionMemo, bool>> where)
        {
            throw new NotImplementedException("Use Load Strategy overload of Single method instead.");
        }

        /// <summary>
        /// LoadStrategy method overload of Single method
        /// </summary>
        /// <param name="rejectionMemoId">RejectionMemo Id</param>
        /// <param name="correspondenceId">Correspondence Id</param>
        /// <returns>RejectionMemo</returns>
        public RejectionMemo Single(Guid? rejectionMemoId = null, Guid? correspondenceId = null)
        {
            var entities = new string[] { LoadStrategy.Entities.RejectionMemo,LoadStrategy.Entities.RejectionMemoVat,LoadStrategy.Entities.RejectionMemoVatIdentifier,
                                      LoadStrategy.Entities.RejectionMemoAttachments,LoadStrategy.Entities.RejectionMemoCoupon,LoadStrategy.Entities.AttachmentUploadedbyUser};

            var loadStrategy = new LoadStrategy(string.Join(",", entities));
            string rejectionMemostr = null, corrospondencestr = null;
            if (rejectionMemoId.HasValue)
                rejectionMemostr = ConvertUtil.ConvertGuidToString(rejectionMemoId.Value);
            if (correspondenceId.HasValue)
                corrospondencestr = ConvertUtil.ConvertGuidToString(correspondenceId.Value);

            var rejectionMemos = GetRejectionMemoLS(loadStrategy, rejectionMemostr, corrospondencestr);
            if (rejectionMemos.Count > 0)
            {
                if (rejectionMemos.Count > 1) throw new ApplicationException("Multiple records found");
                return rejectionMemos[0];
            }
            return null;
        }

        /// <summary>
        /// Gets list of RejectionMemo objects
        /// </summary>
        /// <param name="loadStrategy">loadStrategy string</param>
        /// <param name="rejectionMemoId">RejectionMemo Id</param>
        /// <param name="correspondenceId">Correspondence Id</param>
        /// <returns>List of RejectionMemo objects</returns>
        private List<RejectionMemo> GetRejectionMemoLS(LoadStrategy loadStrategy, string rejectionMemoId = null, string correspondenceId = null)
        {
            return base.ExecuteLoadsSP(SisStoredProcedures.GetRejectionMemo,
                                      loadStrategy,
                                          new OracleParameter[] { new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoIdParameterName, rejectionMemoId ?? null),  
                                    new OracleParameter(RejectionMemoRecordRepositoryConstants.CorrespondenceIdParamName, correspondenceId ?? null) ,
                                    new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoCouponIdParameterName, null) ,
                                  },
                                      this.FetchRecord);
        }


        /// <summary>
        /// Fetches the record.
        /// </summary>
        /// <param name="loadStrategyResult">The load strategy result.</param>
        /// <returns></returns>
        private List<RejectionMemo> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            var rejectionMemos = new List<RejectionMemo>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemo))
            {
                rejectionMemos = RejectionMemoRecordRepository.LoadEntities(base.EntityObjectSet, loadStrategyResult, null);
            }
            return rejectionMemos;
        }

        public long IsRMCouponExists(int ticketIssuingAirline, long ticketDocNumber, int couponNumber, string invoiceNumber, string rejectionMemoNumber, string billingMemoNumber, string creditMemoNumber)
        {
            var parameters = new ObjectParameter[8];
            parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.TicketIssueAirlineParameterName, typeof(int)) { Value = ticketIssuingAirline };
            parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
            parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.InvoiceNumberParameterName, typeof(string)) { Value = invoiceNumber };

            parameters[3] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CouponNumberParameterName, typeof(int)) { Value = couponNumber };
            parameters[4] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RMNumberParameterName, typeof(string)) { Value = rejectionMemoNumber };
            parameters[5] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMemoNumberParameterName, typeof(string)) { Value = billingMemoNumber };
            parameters[6] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CreditMemoNumberParameterName, typeof(string)) { Value = creditMemoNumber };

            parameters[7] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CouponCountParameterName, typeof(long));

            ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.IsRMCouponExistsFunctionName, parameters);

            return long.Parse(parameters[7].Value.ToString());
        }

        public long IsRMLinkingExists(int ticketIssuingAirline, long ticketDocNumber, int couponNumber, Guid invoiceId, long correspondenceNumber)
        {
            var parameters = new ObjectParameter[6];
            parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.TicketIssueAirlineParameterName, typeof(int)) { Value = ticketIssuingAirline };
            parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
            parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[3] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CouponNumberParameterName, typeof(int)) { Value = couponNumber };
            parameters[4] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CorrespondenceIdParameterName, typeof(long)) { Value = correspondenceNumber };

            parameters[5] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ResultCountParameterName, typeof(long));

            ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.IsRMLinkingExistsFunctionName, parameters);

            return long.Parse(parameters[5].Value.ToString());
        }

        public RMLinkingResultDetails GetRMLinkingDetails(RMLinkingCriteria criteria)
        {
            var parameters = new OracleParameter[18];
            parameters[0] = new OracleParameter(RejectionMemoRecordRepositoryConstants.ReasonCodeParameterName, criteria.ReasonCode);
            parameters[1] = new OracleParameter(RejectionMemoRecordRepositoryConstants.YourInvoiceNumberParameterName, criteria.InvoiceNumber);
            parameters[2] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingMonthParameterName, criteria.BillingMonth);
            parameters[3] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingYearParameterName, criteria.BillingYear);
            parameters[4] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingPeriodParameterName, criteria.BillingPeriod);
            parameters[5] = new OracleParameter(RejectionMemoRecordRepositoryConstants.FIMBMCMNumberParameterName, criteria.FimBMCMNumber);
            parameters[6] = new OracleParameter(RejectionMemoRecordRepositoryConstants.FIMCouponNumberParameterName, criteria.FimCouponNumber);
            parameters[7] = new OracleParameter(RejectionMemoRecordRepositoryConstants.YourRMNumberParameterName, criteria.RejectionMemoNumber);
            //SCP37078: if source code is 44,45,46 and FimBmCmIndicatorId != 2 then pass as 2(FIM) in procedure for linking.
            if (!String.IsNullOrEmpty(criteria.SourceCode) && (criteria.SourceCode == "44" || criteria.SourceCode == "45" || criteria.SourceCode == "46"))
            {
                parameters[8] = new OracleParameter(RejectionMemoRecordRepositoryConstants.FIMBMCMIndicatorIdParameterName, 2);
            }
            else
            {
                parameters[8] = new OracleParameter(RejectionMemoRecordRepositoryConstants.FIMBMCMIndicatorIdParameterName, criteria.FimBmCmIndicatorId);
            }
            parameters[9] = new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionStageParameterName, criteria.RejectionStage);
            parameters[10] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingMemberIdParameterName, criteria.BillingMemberId);
            parameters[11] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBilledMemberIdParameterName, criteria.BilledMemberId);
            var rejectedInvParameter = new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectingInvoiceIdParameterName, criteria.RejectedInvoiceId);
            rejectedInvParameter.OracleDbType = OracleDbType.Raw;
            parameters[12] = rejectedInvParameter;
            parameters[13] = new OracleParameter(RejectionMemoRecordRepositoryConstants.IgnoreValidationParameterName, criteria.IgnoreValidationOnMigrationPeriod ? 1 : 0);
            //parameters[13] = new OracleParameter(RejectionMemoRecordRepositoryConstants.IgnoreValidationParameterName,false ? 1 : 0);
            parameters[14] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName, OracleDbType = OracleDbType.Number, Direction = ParameterDirection.Output };
            parameters[15] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.IsLinkingSuccessfulParameterName, Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Number };
            parameters[16] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.HasCouponBreakdownParameterName, Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Number };
            parameters[17] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };

            return ExecuteStoredProc(SisStoredProcedures.GetRMLinkingDetails, parameters, true, r => this.FetchLinkingRecords(r));

        }

        /// <summary>
        /// Get linking details for rejection memo when multiple records are found for rejected entity then as per user selection fetch data for selected memo
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public RMLinkingResultDetails GetLinkedMemoAmountDetails(RMLinkingCriteria criteria)
        {
            var parameters = new OracleParameter[8];
            var memoIdParameter = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoIdParameterName, criteria.MemoId);
            memoIdParameter.OracleDbType = OracleDbType.Raw;
            parameters[0] = memoIdParameter;
            parameters[1] = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoSourceCodeParameterName, criteria.SourceCode);
            parameters[2] = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoReasonCodeParameterName, criteria.ReasonCode);
            var rejectedInvParameter = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoRejectedInvIdParameterName, criteria.RejectedInvoiceId);
            rejectedInvParameter.OracleDbType = OracleDbType.Raw;
            parameters[3] = rejectedInvParameter;
            parameters[4] = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoFIMBMCMIndicatorIdParameterName, criteria.FimBmCmIndicatorId);
            parameters[5] = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoRejectionStageParameterName, criteria.RejectionStage);

            parameters[6] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };
            parameters[7] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.HasCouponBreakdownParameterName, OracleDbType = OracleDbType.Number, Direction = ParameterDirection.Output };

            return ExecuteStoredProc(SisStoredProcedures.GetLinkedMemoAmountDetails, parameters, false, r => this.FetchLinkedMemoAmounts(r));

        }

        public string InheritRMCouponDetails(Guid rejectionMemoId)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RmIdParameterName, typeof(Guid)) { Value = rejectionMemoId };
            parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, typeof(string));

            ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.InheritRMCouponDetailsFunctionName, parameters);
            return parameters[1].Value.ToString();
        }



        #region Load strategy

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<RejectionMemo> LoadEntities(ObjectSet<RejectionMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<RejectionMemo> link)
        {
            if (link == null)
                link = new Action<RejectionMemo>(c => { });
            var rejectionMemos = new List<RejectionMemo>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemo))
            {
                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().RejectionMemoMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<RejectionMemo>(link)
                    )
                {
                    rejectionMemos.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load RejectionMemoVat by calling respective LoadEntity
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoVat) && rejectionMemos.Count != 0)
            {
                RejectionMemoVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<RejectionMemoVat>()
                          , loadStrategyResult
                          , null);
            }

            // Load RejectionMemoAttachments
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoAttachments) && rejectionMemos.Count != 0)
            {
                RejectionMemoAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<RejectionMemoAttachment>()
                          , loadStrategyResult
                          , null);
            }

            //Load RM Coupon's by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoCoupon) && rejectionMemos.Count != 0)
            {
                RMCouponBreakdownRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMCoupon>()
                        , loadStrategyResult
                        , rmCoupon => rmCoupon.RejectionMemoRecord = rejectionMemos.Find(i => i.Id == rmCoupon.RejectionMemoId));
            }
            return rejectionMemos;
        }

        public static List<RejectionMemo> LoadAuditEntities(ObjectSet<RejectionMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<RejectionMemo> link, string entity)
        {
            if (link == null)
                link = new Action<RejectionMemo>(c => { });
            List<RejectionMemo> rejectionMemo = new List<RejectionMemo>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.RejectionMemo))
            {
                // first result set includes the category
                foreach (var c in
                  new PaxMaterializers().PaxInvoiceRejectionMemoAuditMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<RejectionMemo>(link)
                  )
                {
                    rejectionMemo.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load Correspondence by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.Correspondence) && rejectionMemo.Count != 0)
            {
                var correspodences = PaxCorrespondenceRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<Correspondence>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.PaxEntities.Correspondence);
                foreach (var memo in rejectionMemo)
                {
                    if (memo.Correspondence != null)
                    {
                        memo.Correspondences = correspodences.Where(c => c.CorrespondenceNumber == memo.Correspondence.CorrespondenceNumber).ToList();
                    }
                }

            }

            if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RejectionMemoCoupon) && rejectionMemo.Count != 0)
            {
                RMCouponBreakdownRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RMCoupon>(),
                                                                       loadStrategyResult,
                                                                       null,
                                                                       LoadStrategy.PaxEntities.RejectionMemoCoupon);
            }

            // Load RejectionMemoVat by calling respective LoadEntity
            if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RejectionMemoVAT) && rejectionMemo.Count != 0)
            {
                RejectionMemoVatRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RejectionMemoVat>()
                          , loadStrategyResult
                          , null);
            }

            // Load RejectionMemoAttachments
            if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RMAttachment) && rejectionMemo.Count != 0)
            {
                RejectionMemoAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RejectionMemoAttachment>()
                          , loadStrategyResult
                          , null);
            }

            return rejectionMemo;
        }

        /// <summary>
        /// Fetch record for Fields
        /// </summary>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        private RMLinkingResultDetails FetchLinkingRecords(LoadStrategyResult loadStrategyResult)
        {
            RMLinkingResultDetails details = new RMLinkingResultDetails();

            if (loadStrategyResult.IsLoaded(Pax.Impl.RejectionMemoRecordRepositoryConstants.MemoAmountsParameterName))
            {
                details.MemoAmount = RMLinkingAmountRepository.LoadEntities(loadStrategyResult, null);
            }

            if (loadStrategyResult.IsLoaded(Pax.Impl.RejectionMemoRecordRepositoryConstants.MemoRecordsParameterName))
            {
                var records = RMLinkingRecordsRepository.LoadEntities(loadStrategyResult, null);
                details.Records.AddRange(records);
            }

            return details;
        }

        /// <summary>
        /// Fetch record for Fields
        /// </summary>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        private RMLinkingResultDetails FetchLinkedMemoAmounts(LoadStrategyResult loadStrategyResult)
        {
            RMLinkingResultDetails details = new RMLinkingResultDetails();

            if (loadStrategyResult.IsLoaded(Pax.Impl.RejectionMemoRecordRepositoryConstants.MemoAmountsParameterName))
            {
                details.MemoAmount = RMLinkingAmountRepository.LoadEntities(loadStrategyResult, null);
            }

            return details;
        }

        private RMLinkingResultDetails ExecuteStoredProc<T>(StoredProcedure sp, OracleParameter[] oraInputParameters, bool isCheckLinking, Func<LoadStrategyResult, T> fetch)
        {
            using (var result = new LoadStrategyResult())
            {
                using (var cmd = Context.CreateStoreCommand(sp.Name, CommandType.StoredProcedure) as OracleCommand)
                {
                    cmd.Parameters.AddRange(oraInputParameters);

                    // Add result parameters to Oracle Parameter Collection
                    foreach (SPResultObject resObj in sp.GetResultSpec())
                    {
                        var resultParam = new OracleParameter(resObj.ParameterName, OracleDbType.Cursor);
                        resultParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(resultParam);

                        //if the entity is requested, add it to the result
                        result.Add(resObj.EntityName, resultParam);
                    }

                    using (cmd.Connection.CreateConnectionScope())
                    {
                        //Execute SP

                        //Set CommandTimeout value to value given in the Config file 
                        //if it NOT in the config then it will be set to default value 0.
                        cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);

                        cmd.ExecuteNonQuery();

                        //Allow the caller to populate results
                        var rmResult = fetch(result) as RMLinkingResultDetails;
                        LoadOutParameters(cmd, rmResult, isCheckLinking);
                        return rmResult;
                    }
                }
            }
        }

        private void LoadOutParameters(OracleCommand cmd, RMLinkingResultDetails rmResult, bool isCheckLinking)
        {
            if (isCheckLinking)
            {
                if (cmd.Parameters.Contains(RejectionMemoRecordRepositoryConstants.IsLinkingSuccessfulParameterName))
                {
                    var isLinkingSuccessful = cmd.Parameters[RejectionMemoRecordRepositoryConstants.IsLinkingSuccessfulParameterName].Value.ToString();
                    if (!string.IsNullOrEmpty(isLinkingSuccessful))
                    {
                        if (isLinkingSuccessful == "1")
                            rmResult.IsLinkingSuccessful = true;
                    }
                }

                if (cmd.Parameters.Contains(RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName))
                {
                    var currConvFactor = cmd.Parameters[RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName].Value.ToString();
                    if (!string.IsNullOrEmpty(currConvFactor))
                        rmResult.CurrencyConversionFactor = Convert.ToDecimal(currConvFactor);
                }

            }
            rmResult.ErrorMessage = cmd.Parameters[RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName].Value.ToString();
            var hasBreakdown = cmd.Parameters[RejectionMemoRecordRepositoryConstants.HasCouponBreakdownParameterName].Value.ToString();
            if (!string.IsNullOrEmpty(hasBreakdown))
            {
                if (hasBreakdown == "1")
                    rmResult.HasBreakdown = true;
            }
        }
        #endregion

        public void CopyLinkedRMCouponDetails(Guid rejectionMemoGuidId)
        {
            var parameters = new ObjectParameter[1];

            parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoIdParameterName, typeof(Guid))
            {
                Value = rejectionMemoGuidId
            };

            ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.InheritRMCouponDetails, parameters);
        }

        /// <summary>
        /// Used in Form F RM.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public SamplingFormFLinkingResult GetFormDELinkingDetails(SamplingRMLinkingCriteria criteria)
        {
            var parameters = new ObjectParameter[13];
            parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ReasonCodeParameterName, typeof(string)) { Value = criteria.ReasonCode };
            parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.YourInvoiceNumberParameterName, typeof(int)) { Value = criteria.InvoiceNumber };
            parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = criteria.BillingMonth };
            parameters[3] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = criteria.BillingYear };
            parameters[4] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingPeriodParameterName, typeof(int)) { Value = criteria.BillingPeriod };
            parameters[5] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = criteria.BillingMemberId };
            parameters[6] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = criteria.BilledMemberId };
            parameters[7] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RejectingInvoiceIdParameterName, typeof(Guid)) { Value = criteria.RejectingInvoiceId };
            parameters[8] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ProvBillingMonthParameterName, typeof(int)) { Value = criteria.ProvBillingMonth };
            parameters[9] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ProvBillingYearParametername, typeof(int)) { Value = criteria.ProvBillingYear };
            parameters[10] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.IgnoreValidationParameterName, typeof(int)) { Value = criteria.IgnoreValidationOnMigrationPeriod ? 1 : 0 };
            //parameters[10] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.IgnoreValidationParameterName, typeof(int)) { Value = false ? 1 : 0 };
            parameters[11] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName, typeof(decimal));
            parameters[12] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, typeof(string));

            ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.GetDELinkingdetails, parameters);
            var deDetails = new SamplingFormFLinkingResult();
            deDetails.CurrencyConversionFactor = string.IsNullOrEmpty(parameters[11].Value.ToString()) ? 0 : decimal.Parse(parameters[11].Value.ToString());
            deDetails.ErrorMessage = parameters[12].Value.ToString();

            return deDetails;
        }

        /// <summary>
        /// Used in Form XF RM.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public RMLinkingResultDetails GetSamplingFormFLinkingDetails(SamplingRMLinkingCriteria criteria)
        {
            var parameters = new OracleParameter[14];
            parameters[0] = new OracleParameter(RejectionMemoRecordRepositoryConstants.ReasonCodeParameterName, criteria.ReasonCode);
            parameters[1] = new OracleParameter(RejectionMemoRecordRepositoryConstants.YourInvoiceNumberParameterName, criteria.InvoiceNumber);
            parameters[2] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingMonthParameterName, criteria.BillingMonth);
            parameters[3] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingYearParameterName, criteria.BillingYear);
            parameters[4] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingPeriodParameterName, criteria.BillingPeriod);
            parameters[5] = new OracleParameter(RejectionMemoRecordRepositoryConstants.YourRMNumberParameterName, criteria.RejectionMemoNumber);
            parameters[6] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingMemberIdParameterName, criteria.BillingMemberId);
            parameters[7] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBilledMemberIdParameterName, criteria.BilledMemberId);
            var rejectingInvParameter = new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectingInvoiceIdParameterName, criteria.RejectingInvoiceId);
            rejectingInvParameter.OracleDbType = OracleDbType.Raw;
            parameters[8] = rejectingInvParameter;
            parameters[9] = new OracleParameter(RejectionMemoRecordRepositoryConstants.ProvBillingYearParametername, criteria.ProvBillingYear);
            parameters[10] = new OracleParameter(RejectionMemoRecordRepositoryConstants.ProvBillingMonthParameterName, criteria.ProvBillingMonth);




            parameters[11] = new OracleParameter { ParameterName = RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName, OracleDbType = OracleDbType.Number, Direction = ParameterDirection.Output };
            //parameters[12] = new OracleParameter { ParameterName = RejectionMemoRecordRepositoryConstants.IsLinkingSuccessfulParameterName, Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Number };
            parameters[12] = new OracleParameter { ParameterName = RejectionMemoRecordRepositoryConstants.HasCouponBreakdownParameterName, Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Number };
            parameters[13] = new OracleParameter { ParameterName = RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };

            return ExecuteStoredProc(SisStoredProcedures.GetFormFLinkingDetails, parameters, true, FetchLinkingRecords);
        }

        /// <summary>
        /// Validates the rejection memo acceptable amount difference.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="billingCode">The billing code.</param>
        /// <returns></returns>
        public string ValidateRejectionMemoAcceptableAmountDifference(Guid invoiceId, int billingCode)
        {
            var parameters = new ObjectParameter[3];
            parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
            parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingCodeParameterName, typeof(int)) { Value = billingCode };
            parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, typeof(string));

            ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.ValidateRMAcceptableAmountDiffFunctionName, parameters);
            return parameters[2].Value.ToString();
        }

        //CMP#674-Validation of Coupon and AWB Breakdowns in Rejections
        public List<InvalidRejectionMemoDetails> IsYourRejectionCouponDropped(Guid invoiceId, Guid? rejectionMemoId = null, string YourInvoiceNo = null, string YourRejectionNo = null, int YourInvoiceYear = 0, int YourInvoiceMonth = 0, int YourInvoicePeriod = 0)
        {
            var parameters = new ObjectParameter[7];

            parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.InvoiceId, typeof(Guid)) { Value = invoiceId };
            if (rejectionMemoId.HasValue)
            {
                parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoId,
                                                    typeof (Guid)) {Value = rejectionMemoId.Value};
            }
            else
            {
                parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoId,
                                                    typeof(Guid)) { Value = null };
            }
            parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.YourInvoiceNo, typeof(Guid)) { Value = YourInvoiceNo };
            parameters[3] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.YourRejectionNo, typeof(Guid)) { Value = YourRejectionNo };
            parameters[4] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.YourInvoiceYear, typeof(int)) { Value = YourInvoiceYear };
            parameters[5] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.YourInvoiceMonth, typeof(int)) { Value = YourInvoiceMonth };
            parameters[6] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.YourInvoicePeriod, typeof(int)) { Value = YourInvoicePeriod };

            /* Calling SP PROC_PAX_IS_RM_CPN_DROPPED, to apply CMP#674 validation. A cursor is returned by SP. cursor has list of all RMs failing validation. */
            var rejectedRMCoupons = ExecuteStoredFunction<InvalidRejectionMemoDetails>(RejectionMemoRecordRepositoryConstants.IsPaxYourRejectionCouponDroppedFunctionName, parameters);

            return rejectedRMCoupons.ToList();

        }
    }
}

using System;
using System.Configuration;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Core;
using Iata.IS.Model.Pax;
using Iata.IS.Data.Impl;
using System.Collections.Generic;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Pax.Common;
using System.Data;

namespace Iata.IS.Data.Pax.Impl
{
    public class RMCouponBreakdownRecordRepository : Repository<RMCoupon>, IRMCouponBreakdownRecordRepository
    {
        public long GetRMCouponDuplicateCount(int rejectionStage, string ticketIssuingAirline, long ticketDocNumber, int couponNumber, int billingmemberId, int billedMemberId, string yourInvoiceNumber, int billingMonth, int billingYear)
        {
            var parameters = new ObjectParameter[10];
            parameters[0] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.RejectionStageParameterName, typeof(int)) { Value = rejectionStage };
            parameters[1] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.TicketIssuingAirlineParameterName, typeof(string)) { Value = ticketIssuingAirline };
            parameters[2] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
            parameters[3] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.CouponNumberParameterName, typeof(int)) { Value = couponNumber };
            parameters[4] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingmemberId };
            parameters[5] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };

            parameters[6] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.YourInvoiceNumberParameterName, typeof(string)) { Value = yourInvoiceNumber };
            parameters[7] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };
            parameters[8] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };

            parameters[9] = new ObjectParameter(RMCouponBreakdownRecordRepositoryConstants.DuplicateCountParameterName, typeof(long));

            ExecuteStoredProcedure(RMCouponBreakdownRecordRepositoryConstants.GetRMCouponDuplicateCountFunctionName, parameters);

            return long.Parse(parameters[9].Value.ToString());
        }

        public override RMCoupon Single(System.Linq.Expressions.Expression<Func<RMCoupon, bool>> where)
        {
            throw new NotImplementedException("Use Load Strategy overload of Single method instead.");
        }

        public RMLinkedCouponDetails GetRMCouponLinkingDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId)
        {
            var parameters = new OracleParameter[7];
            parameters[0] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.CpnTicketIssuingAirlineParameterName, issuingAirline);
            parameters[1] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.CouponNoParameterName, couponNo);
            parameters[2] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.TicketDocNoParameterName, ticketDocNo);
            var rmIdParameter = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.RejectionMemoIdParameterName, rmId);
            rmIdParameter.OracleDbType = OracleDbType.Raw;
            parameters[3] = rmIdParameter;
            parameters[4] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.BillingMemberIdParameterName, billingMemberId);
            parameters[5] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.BilledMemberIdParameterName, billedMemberId);


            parameters[6] = new OracleParameter() { ParameterName = RMCouponBreakdownRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };

            return ExecuteStoredProc(SisStoredProcedures.GetRMCouponLinkingDetails, parameters, FetchLinkedCouponAmounts);
        }

        /// <summary>
        /// Get Sampling Coupon linking Details.
        /// </summary>
        /// <param name="issuingAirline"> Issuing Airline Number.</param>
        /// <param name="couponNo">Coupon Number.</param>
        /// <param name="ticketDocNo">Ticket Document Number.</param>
        /// <param name="rmId">Rejection memo ID.</param>
        /// <param name="billingMemberId">Billing Member ID.</param>
        /// <param name="billedMemberId">Billed Member ID.</param>
        /// <returns></returns>
        public RMLinkedCouponDetails GetSamplingCouponLinkingDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId)
        {
            var parameters = new OracleParameter[7];
            parameters[0] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.CpnTicketIssuingAirlineParameterName, issuingAirline);
            parameters[1] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.CouponNoParameterName, couponNo);
            parameters[2] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.TicketDocNoParameterName, ticketDocNo);
            var rmIdParameter = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.RejectionMemoIdParameterName, rmId);
            rmIdParameter.OracleDbType = OracleDbType.Raw;
            parameters[3] = rmIdParameter;
            parameters[4] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.BillingMemberIdParameterName, billingMemberId);
            parameters[5] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.BilledMemberIdParameterName, billedMemberId);

            parameters[6] = new OracleParameter() { ParameterName = RMCouponBreakdownRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };

            return ExecuteStoredProc(SisStoredProcedures.GetSamplingCouponLinkingDetails, parameters, FetchLinkedCouponAmounts);
        }

        /// <summary>
        /// Get linking details for rejection memo Coupon when multiple records are found in rejected enity then as per user selection fetch data for selected coupon
        /// </summary>
        /// <param name="couponId"></param>
        /// <param name="rmId"></param>
        /// <param name="billingMemberId"></param>
        /// <param name="billedMemberId"></param>
        /// <returns></returns>
        public RMLinkedCouponDetails GetLinkedCouponAmountDetails(Guid couponId, Guid rmId, int billingMemberId, int billedMemberId)
        {
            var parameters = new OracleParameter[5];
            var couponIdParameter = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.CouponIdParameterName, couponId);
            couponIdParameter.OracleDbType = OracleDbType.Raw;
            parameters[0] = couponIdParameter;
            var rmIdParameter = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.RejectionMemoIdParameterName, rmId);
            rmIdParameter.OracleDbType = OracleDbType.Raw;
            parameters[1] = rmIdParameter;
            parameters[2] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.BillingMemberIdParameterName, billingMemberId);
            parameters[3] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.BilledMemberIdParameterName, billedMemberId);


            parameters[4] = new OracleParameter() { ParameterName = RMCouponBreakdownRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };

            return ExecuteStoredProc(SisStoredProcedures.GetLinkedCouponAmountDetails, parameters, r => this.FetchLinkedCouponAmounts(r));

        }

        /// <summary>
        /// Get linking details for the sampling rejection coupon on the base of selection of specified coupon.
        /// </summary>
        /// <param name="couponId">Coupon Number.</param>
        /// <param name="rmId">Rejection memo ID.</param>
        /// <param name="billingMemberId">Billing Member ID.</param>
        /// <param name="billedMemberId">Billed member ID.</param>
        /// <returns></returns>
        public RMLinkedCouponDetails GetSamplingLinkedCouponAmountDetails(Guid couponId, Guid rmId, int billingMemberId, int billedMemberId)
        {

            //Method is not tested, test the method.
            var parameters = new OracleParameter[5];
            var couponIdParameter = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.CouponIdParameterName, couponId);
            couponIdParameter.OracleDbType = OracleDbType.Raw;
            parameters[0] = couponIdParameter;
            var rmIdParameter = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.RejectionMemoIdParameterName, rmId);
            rmIdParameter.OracleDbType = OracleDbType.Raw;
            parameters[1] = rmIdParameter;
            parameters[2] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.BillingMemberIdParameterName, billingMemberId);
            parameters[3] = new OracleParameter(RMCouponBreakdownRecordRepositoryConstants.BilledMemberIdParameterName, billedMemberId);


            parameters[4] = new OracleParameter() { ParameterName = RMCouponBreakdownRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };

            return ExecuteStoredProc(SisStoredProcedures.GetSamplingLinkedCouponAmountDetails, parameters, r => this.FetchLinkedCouponAmounts(r));
        }

        #region Load strategy

        /// <summary>
        /// LoadStrategy method overload of Single method
        /// </summary>
        /// <param name="rmCouponId">RMCouponBreakdown Id</param>
        /// <returns>RMCoupon object</returns>
        public RMCoupon Single(Guid rmCouponId)
        {
            var entities = new string[] { LoadStrategy.Entities.RejectionMemoCoupon,LoadStrategy.Entities.RejectionMemoCouponVat,LoadStrategy.Entities.RejectionMemoCouponVatIdentifier,
                                      LoadStrategy.Entities.RejectionMemoCouponAttachments,LoadStrategy.Entities.RejectionMemoCouponTax,LoadStrategy.Entities.RejectionMemo};

            var loadStrategy = new LoadStrategy(string.Join(",", entities));

            var rmCoupons = GetRMCouponLS(loadStrategy, ConvertUtil.ConvertGuidToString(rmCouponId));

            if (rmCoupons.Count > 0)
            {
                if (rmCoupons.Count > 1) throw new ApplicationException("Multiple records found");
                return rmCoupons[0];
            }
            return null;
        }

        /// <summary>
        ///  Gets list of RMCoupon objects
        /// </summary>
        /// <param name="loadStrategy">loadStrategy string</param>
        /// <param name="rmCouponId">rmCouponId</param>
        /// <returns>List of RMCoupon objects</returns>
        private List<RMCoupon> GetRMCouponLS(LoadStrategy loadStrategy, string rmCouponId = null)
        {
            return base.ExecuteLoadsSP(SisStoredProcedures.GetRejectionMemo,
                                      loadStrategy,
                                      new OracleParameter[] { new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoIdParameterName, null),  
                                    new OracleParameter(RejectionMemoRecordRepositoryConstants.CorrespondenceIdParamName, null) ,
                                    new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoCouponIdParameterName, rmCouponId) ,
                                  },
                                      this.FetchRecord);
        }

        /// <summary>
        /// Fetches the record.
        /// </summary>
        /// <param name="loadStrategyResult">The load strategy result.</param>
        /// <returns></returns>
        private List<RMCoupon> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            var rmCoupons = new List<RMCoupon>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoCoupon))
            {
                rmCoupons = RMCouponBreakdownRecordRepository.LoadEntities(base.EntityObjectSet, loadStrategyResult, null);
            }

            /* Rejection Memo is parent entity to RejectionMemoCoupon
               * It will become circular call, if it's binding is implemented into LoadEntities function 
               * so it is better to implement it in this function*/
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemo))
            {
                // first result set includes the category
                new PaxMaterializers().RejectionMemoMaterializer.Materialize(reader).Bind(this.EntityObjectSet.Context.CreateObjectSet<RejectionMemo>()).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            return rmCoupons;
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<RMCoupon> LoadEntities(ObjectSet<RMCoupon> objectSet, LoadStrategyResult loadStrategyResult, Action<RMCoupon> link)
        {
            if (link == null)
                link = new Action<RMCoupon>(c => { });
            var rmCoupons = new List<RMCoupon>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemoCoupon))
            {
                foreach (var c in
                    new PaxMaterializers().RMCouponMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<RMCoupon>(link)
                    )
                {
                    rmCoupons.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load RejectionMemoCouponVat by calling respective LoadEntity
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoCouponVat) && rmCoupons.Count != 0)
            {
                RMCouponVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMCouponVat>()
                          , loadStrategyResult
                          , null);
            }

            // Load RejectionMemoCouponAttachments
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoCouponAttachments) && rmCoupons.Count != 0)
            {
                RejectionMemoCouponAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMCouponAttachment>()
                          , loadStrategyResult
                          , null);
            }

            // Load RejectionMemoCouponTax
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoCouponTax) && rmCoupons.Count != 0)
            {
                RMCouponTaxRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMCouponTax>()
                          , loadStrategyResult
                          , null);
            }

            return rmCoupons;
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<RMCoupon> LoadAuditEntities(ObjectSet<RMCoupon> objectSet, LoadStrategyResult loadStrategyResult, Action<RMCoupon> link, string entity)
        {
            if (link == null)
                link = new Action<RMCoupon>(c => { });
            var rmCoupons = new List<RMCoupon>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.RejectionMemoCoupon))
            {
                foreach (var c in
                    new PaxMaterializers().RMCouponAuditMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach<RMCoupon>(link)
                    )
                {
                    rmCoupons.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load RejectionMemoCouponVat by calling respective LoadEntity
            if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RMCouponVAT) && rmCoupons.Count != 0)
            {
                RMCouponVatRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RMCouponVat>()
                          , loadStrategyResult
                          , null);
            }

            // Load RejectionMemoCouponAttachments
            if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RMCouponAttachment) && rmCoupons.Count != 0)
            {
                RejectionMemoCouponAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RMCouponAttachment>()
                          , loadStrategyResult
                          , null);
            }

            // Load RejectionMemoCouponTax
            if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RMCouponTax) && rmCoupons.Count != 0)
            {
                RMCouponTaxRepository.LoadAuditEntities((objectSet.Context.CreateObjectSet<RMCouponTax>())
                          , loadStrategyResult
                          , null);
            }


            return rmCoupons;
        }

        private RMLinkedCouponDetails ExecuteStoredProc<T>(StoredProcedure sp, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, T> fetch)
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
                        var rmResult = fetch(result) as RMLinkedCouponDetails;

                        rmResult.ErrorMessage = cmd.Parameters[RMCouponBreakdownRecordRepositoryConstants.ErrorCodeParameterName].Value.ToString();
                        return rmResult;
                    }
                }
            }
        }

        /// <summary>
        /// Fetch record for Fields
        /// </summary>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public RMLinkedCouponDetails FetchLinkedCouponAmounts(LoadStrategyResult loadStrategyResult)
        {
            var details = new RMLinkedCouponDetails();

            if (loadStrategyResult.IsLoaded(RMCouponBreakdownRecordRepositoryConstants.CouponListParameterName))
            {
                var couponDetail = RMLinkedCouponRepository.LoadEntities(loadStrategyResult, null);
                details.RMLinkedCoupons.AddRange(couponDetail);
            }

            if (loadStrategyResult.IsLoaded(RMCouponBreakdownRecordRepositoryConstants.CouponAmountsParameterName))
            {
                var records = LoadLinkedCouponEntities(loadStrategyResult, null);
                details.Details = records;
            }
            return details;
        }
        public static RMCoupon LoadLinkedCouponEntities(LoadStrategyResult loadStrategyResult, Action<RMCoupon> link)
        {
            var records = new List<RMCoupon>();
            using (OracleDataReader reader = loadStrategyResult.GetReader(RMCouponBreakdownRecordRepositoryConstants.CouponAmountsParameterName))
            {
                if (reader != null && reader.HasRows)
                {
                    records = new PaxMaterializers().LinkedRMCouponMaterializer.Materialize(reader).ToList();
                    if (!reader.IsClosed)
                        reader.Close();
                }
            }
            if (records.Count > 0)
            {
                if (loadStrategyResult.IsLoaded(RMCouponBreakdownRecordRepositoryConstants.CouponTaxParameterName))
                {
                    var taxRecords = RMCouponTaxRepository.LoadLinkedEntities(loadStrategyResult, null);
                    records[0].TaxBreakdown.AddRange(taxRecords);
                }
            }
            return records.Count > 0 ? records[0] : null;
        }

        /// <summary>
        /// LoadStrategy method overload of Single method
        /// </summary>
        /// <param name="rmCouponId">RMCouponBreakdown Id</param>
        /// <returns>RMCoupon object</returns>
        public RMCoupon GetRmCouponWithRejectionMemoObject(Guid rmCouponId)
        {
          var entities = new string[] { LoadStrategy.Entities.RejectionMemoCoupon, LoadStrategy.Entities.RejectionMemo};

          var loadStrategy = new LoadStrategy(string.Join(",", entities));

          var rmCoupons = GetRMCouponLS(loadStrategy, ConvertUtil.ConvertGuidToString(rmCouponId));

          if (rmCoupons.Count > 0)
          {
            if (rmCoupons.Count > 1) throw new ApplicationException("Multiple records found");
            return rmCoupons[0];
          }
          return null;
        }

        #endregion
    }
}

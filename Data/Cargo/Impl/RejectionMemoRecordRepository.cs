using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Core;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Common;
//using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class RejectionMemoRecordRepository : Repository<CargoRejectionMemo>, IRejectionMemoRecordRepository
  {
    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<CargoRejectionMemo> LoadEntities(ObjectSet<CargoRejectionMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoRejectionMemo> link)
    {
      if (link == null)
        link = new Action<CargoRejectionMemo>(c => { });
      var rejectionMemos = new List<CargoRejectionMemo>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RejectionMemo))
      {
        // first result set includes the category
        foreach (var c in
            cargoMaterializers.CargoRejectionMemoMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          rejectionMemos.Add(c);
        }
        reader.Close();
      }

      // Load RejectionMemoVat by calling respective LoadEntity
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RejectionMemoVat) && rejectionMemos.Count != 0)
      {
        RejectionMemoVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<CgoRejectionMemoVat>()
                  , loadStrategyResult
                  , null);
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RejectionMemoAttachments))
      {
        CgoRejectionMemoAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CgoRejectionMemoAttachment>(), loadStrategyResult, null);
      }

      // Load RejectionMemoAwb by calling respective LoadEntity
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RmAwb) && rejectionMemos.Count != 0)
      {
        RMAwbRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMAwb>()
                  , loadStrategyResult
                  , null);
      }

      return rejectionMemos;
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

    public override CargoRejectionMemo Single(System.Linq.Expressions.Expression<Func<CargoRejectionMemo, bool>> where)
    {
      throw new NotImplementedException("Use Load Strategy overload of Single method instead.");
    }

    ///// <summary>
    ///// LoadStrategy method overload of Single method
    ///// </summary>
    ///// <param name="rejectionMemoId">RejectionMemo Id</param>
    ///// <param name="correspondenceId">Correspondence Id</param>
    ///// <returns>RejectionMemo</returns>
    public CargoRejectionMemo Single(Guid? rejectionMemoId = null, Guid? correspondenceId = null)
    {
      var entities = new string[] { LoadStrategy.CargoEntities.RejectionMemo,LoadStrategy.Entities.RejectionMemoVat,LoadStrategy.Entities.RejectionMemoVatIdentifier,
                                      LoadStrategy.Entities.RejectionMemoAttachments,LoadStrategy.CargoEntities.RmAwb,LoadStrategy.Entities.AttachmentUploadedbyUser};

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

    ///// <summary>
    ///// Gets list of RejectionMemo objects
    ///// </summary>
    ///// <param name="loadStrategy">loadStrategy string</param>
    ///// <param name="rejectionMemoId">RejectionMemo Id</param>
    ///// <param name="correspondenceId">Correspondence Id</param>
    ///// <returns>List of RejectionMemo objects</returns>
    private List<CargoRejectionMemo> GetRejectionMemoLS(LoadStrategy loadStrategy, string rejectionMemoId = null, string correspondenceId = null)
    {
      return base.ExecuteLoadsSP(SisStoredProcedures.GetCgoRejectionMemo,
                                loadStrategy,
                                    new OracleParameter[] { new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoIdParameterName, rejectionMemoId ?? null),  
                                    new OracleParameter(RejectionMemoRecordRepositoryConstants.CorrespondenceIdParamName, correspondenceId ?? null) ,
                                    new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoAwbIdParameterName, null) ,
                                  },
                                this.FetchRecord);
    }


    ///// <summary>
    ///// Fetches the record.
    ///// </summary>
    ///// <param name="loadStrategyResult">The load strategy result.</param>
    ///// <returns></returns>
    private List<CargoRejectionMemo> FetchRecord(LoadStrategyResult loadStrategyResult)
    {
      var rejectionMemos = new List<CargoRejectionMemo>();
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RejectionMemo))
      {
        rejectionMemos = LoadEntities(EntityObjectSet, loadStrategyResult, null);
      }
      return rejectionMemos;
    }

    //public long IsRMCouponExists(int ticketIssuingAirline, long ticketDocNumber, int couponNumber, string invoiceNumber, string rejectionMemoNumber, string billingMemoNumber, string creditMemoNumber)
    //{
    //  var parameters = new ObjectParameter[8];
    //  parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.TicketIssueAirlineParameterName, typeof(int)) { Value = ticketIssuingAirline };
    //  parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
    //  parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.InvoiceNumberParameterName, typeof(string)) { Value = invoiceNumber };

    //  parameters[3] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CouponNumberParameterName, typeof(int)) { Value = couponNumber };
    //  parameters[4] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RMNumberParameterName, typeof(string)) { Value = rejectionMemoNumber };
    //  parameters[5] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMemoNumberParameterName, typeof(string)) { Value = billingMemoNumber };
    //  parameters[6] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CreditMemoNumberParameterName, typeof(string)) { Value = creditMemoNumber };

    //  parameters[7] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CouponCountParameterName, typeof(long));

    //  ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.IsRMCouponExistsFunctionName, parameters);

    //  return long.Parse(parameters[7].Value.ToString());
    //}

    //public long IsRMLinkingExists(int ticketIssuingAirline, long ticketDocNumber, int couponNumber, Guid invoiceId, long correspondenceNumber)
    //{
    //  var parameters = new ObjectParameter[6];
    //  parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.TicketIssueAirlineParameterName, typeof(int)) { Value = ticketIssuingAirline };
    //  parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
    //  parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
    //  parameters[3] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CouponNumberParameterName, typeof(int)) { Value = couponNumber };
    //  parameters[4] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CorrespondenceIdParameterName, typeof(long)) { Value = correspondenceNumber };

    //  parameters[5] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ResultCountParameterName, typeof(long));

    //  ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.IsRMLinkingExistsFunctionName, parameters);

    //  return long.Parse(parameters[5].Value.ToString());
    //}

    public CgoRMLinkingResultDetails GetRMLinkingDetails(CgoRMLinkingCriteria criteria)
    {
      var parameters = new OracleParameter[19];
      parameters[0] = new OracleParameter(RejectionMemoRecordRepositoryConstants.ReasonCodeParameterName, criteria.ReasonCode);
      parameters[1] = new OracleParameter(RejectionMemoRecordRepositoryConstants.YourInvoiceNumberParameterName, criteria.InvoiceNumber);
      parameters[2] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingMonthParameterName, criteria.BillingMonth);
      parameters[3] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingYearParameterName, criteria.BillingYear);
      parameters[4] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingPeriodParameterName, criteria.BillingPeriod);
      parameters[5] = new OracleParameter(RejectionMemoRecordRepositoryConstants.BMCMNumberParameterName, criteria.YourBillingMemoNumber);
      
      parameters[6] = new OracleParameter(RejectionMemoRecordRepositoryConstants.YourRMNumberParameterName, criteria.RejectionMemoNumber);
      parameters[7] = new OracleParameter(RejectionMemoRecordRepositoryConstants.BMCMIndicatorIdParameterName, criteria.BMCMIndicatorId);
      parameters[8] = new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionStageParameterName, criteria.RejectionStage);
      parameters[9] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingMemberIdParameterName, criteria.BillingMemberId);
      parameters[10] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBilledMemberIdParameterName, criteria.BilledMemberId);
      var rejectedInvParameter = new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectingInvoiceIdParameterName, criteria.RejectedInvoiceId)
                                   {OracleDbType = OracleDbType.Raw};
      parameters[11] = rejectedInvParameter;
      parameters[12] = new OracleParameter(RejectionMemoRecordRepositoryConstants.IgnoreValidationParameterName, criteria.IgnoreValidationOnMigrationPeriod ? 1 : 0);
      parameters[13] = new OracleParameter(RejectionMemoRecordRepositoryConstants.TransactionTypeParameterName, criteria.TransactionType);
      parameters[14] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName, OracleDbType = OracleDbType.Number, Direction = ParameterDirection.Output };
      parameters[15] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.IsLinkingSuccessfulParameterName, Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Number };
      parameters[16] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.HasCouponBreakdownParameterName, Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Number };
      parameters[17] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };
      parameters[18] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.YourTransactionReasonCodeParameterName, Direction = ParameterDirection.Output };
      return ExecuteStoredProc(SisStoredProcedures.GetCgoRMLinkingDetails, parameters, true, r => FetchLinkingRecords(r));
    }

    private CgoRMLinkingResultDetails ExecuteStoredProc<T>(StoredProcedure sp, OracleParameter[] oraInputParameters, bool isCheckLinking, Func<LoadStrategyResult, T> fetch)
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
            var rmResult = fetch(result) as CgoRMLinkingResultDetails;
            LoadOutParameters(cmd, rmResult, isCheckLinking);
            return rmResult;
          }
        }
      }
    }

    private static void LoadOutParameters(OracleCommand cmd, CgoRMLinkingResultDetails rmResult, bool isCheckLinking)
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
      rmResult.ReasonCode = cmd.Parameters[RejectionMemoRecordRepositoryConstants.YourTransactionReasonCodeParameterName].Value.ToString();
      var hasBreakdown = cmd.Parameters[RejectionMemoRecordRepositoryConstants.HasCouponBreakdownParameterName].Value.ToString();
      if (!string.IsNullOrEmpty(hasBreakdown))
      {
        if (hasBreakdown == "1")
          rmResult.HasBreakdown = true;
      }
    }

    /// <summary>
    /// Get linking details for rejection memo when multiple records are found for rejected entity then as per user selection fetch data for selected memo
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public CgoRMLinkingResultDetails GetLinkedMemoAmountDetails(CgoRMLinkingCriteria criteria)
    {
      var parameters = new OracleParameter[7];
      var memoIdParameter = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoIdParameterName, criteria.MemoId);
      memoIdParameter.OracleDbType = OracleDbType.Raw;
      parameters[0] = memoIdParameter;
      parameters[1] = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoReasonCodeParameterName, criteria.ReasonCode);
      var rejectedInvParameter = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoRejectedInvIdParameterName, criteria.RejectedInvoiceId);
      rejectedInvParameter.OracleDbType = OracleDbType.Raw;
      parameters[2] = rejectedInvParameter;
      parameters[3] = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoBMCMIndicatorIdParameterName, criteria.BMCMIndicatorId);
      parameters[4] = new OracleParameter(RejectionMemoRecordRepositoryConstants.MemoRejectionStageParameterName, criteria.RejectionStage);

      parameters[5] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };
      parameters[6] = new OracleParameter() { ParameterName = RejectionMemoRecordRepositoryConstants.HasCouponBreakdownParameterName, OracleDbType = OracleDbType.Number, Direction = ParameterDirection.Output };

      return ExecuteStoredProc(SisStoredProcedures.GetCgoLinkedMemoAmountDetails, parameters, false, r => FetchLinkedMemoAmounts(r));
    }

    public string InheritRMAwbDetails(Guid rejectionMemoId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RmIdParameterName, typeof(Guid)) { Value = rejectionMemoId };
      parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, typeof(string));

      ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.InheritRMAwbDetailsFunctionName, parameters);
      return parameters[1].Value.ToString();
    }

    

    //#region Load strategy

    ///// <summary>
    ///// Load the given object set with entities from the Load Strategy Result
    ///// </summary>
    ///// <param name="objectSet"></param>
    ///// <param name="loadStrategyResult"></param>
    ///// <returns></returns>
    //public static List<RejectionMemo> LoadEntities(ObjectSet<RejectionMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<RejectionMemo> link)
    //{
    //  if (link == null)
    //    link = new Action<RejectionMemo>(c => { });
    //  var rejectionMemos = new List<RejectionMemo>();
    //  using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemo))
    //  {
    //    // first result set includes the category
    //    foreach (var c in
    //        Materializers.RejectionMemoMaterializer
    //        .Materialize(reader)
    //        .Bind(objectSet)
    //        .ForEach<RejectionMemo>(link)
    //        )
    //    {
    //      rejectionMemos.Add(c);
    //    }
    //    reader.Close();
    //  }

    //  // Load RejectionMemoVat by calling respective LoadEntity
    //  if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoVat) && rejectionMemos.Count != 0)
    //  {
    //    RejectionMemoVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<RejectionMemoVat>()
    //              , loadStrategyResult
    //              , null);
    //  }

    //  // Load RejectionMemoAttachments
    //  if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoAttachments) && rejectionMemos.Count != 0)
    //  {
    //    RejectionMemoAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<RejectionMemoAttachment>()
    //              , loadStrategyResult
    //              , null);
    //  }

    //  //Load RM Coupon's by calling respective LoadEntities method
    //  if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemoCoupon) && rejectionMemos.Count != 0)
    //  {
    //    RMCouponBreakdownRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMCoupon>()
    //            , loadStrategyResult
    //            , rmCoupon => rmCoupon.RejectionMemoRecord = rejectionMemos.Find(i => i.Id == rmCoupon.RejectionMemoId));
    //  }
    //  return rejectionMemos;
    //}

    ////public static List<RejectionMemo> LoadAuditEntities(ObjectSet<RejectionMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<RejectionMemo> link, string entity)
    ////{
    ////  if (link == null)
    ////    link = new Action<RejectionMemo>(c => { });
    ////  List<RejectionMemo> rejectionMemo = new List<RejectionMemo>();
    ////  using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.RejectionMemo))
    ////  {
    ////    // first result set includes the category
    ////    foreach (var c in
    ////      Materializers.PaxInvoiceRejectionMemoAuditMaterializer
    ////        .Materialize(reader)
    ////        .Bind(objectSet)
    ////        .ForEach<RejectionMemo>(link)
    ////      )
    ////    {
    ////      rejectionMemo.Add(c);
    ////    }
    ////    reader.Close();
    ////  }

    ////  //Load Correspondence by calling respective LoadEntities method
    ////  if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.Correspondence) && rejectionMemo.Count != 0)
    ////  {
    ////    var correspodences = PaxCorrespondenceRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<Correspondence>(),
    ////                                                           loadStrategyResult,
    ////                                                           null,
    ////                                                           LoadStrategy.PaxEntities.Correspondence);
    ////    foreach (var memo in rejectionMemo)
    ////    {
    ////      if (memo.Correspondence != null)
    ////      {
    ////        memo.Correspondences = correspodences.Where(c => c.CorrespondenceNumber == memo.Correspondence.CorrespondenceNumber).ToList();
    ////      }
    ////    }

    ////  }

    ////  if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RejectionMemoCoupon) && rejectionMemo.Count != 0)
    ////  {
    ////    RMCouponBreakdownRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RMCoupon>(),
    ////                                                           loadStrategyResult,
    ////                                                           null,
    ////                                                           LoadStrategy.PaxEntities.RejectionMemoCoupon);
    ////  }

    ////  // Load RejectionMemoVat by calling respective LoadEntity
    ////  if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RejectionMemoVAT) && rejectionMemo.Count != 0)
    ////  {
    ////    RejectionMemoVatRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RejectionMemoVat>()
    ////              , loadStrategyResult
    ////              , null);
    ////  }

    ////  // Load RejectionMemoAttachments
    ////  if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RMAttachment) && rejectionMemo.Count != 0)
    ////  {
    ////    RejectionMemoAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RejectionMemoAttachment>()
    ////              , loadStrategyResult
    ////              , null);
    ////  }

    ////  return rejectionMemo;
    ////}

    /// <summary>
    /// Fetch record for Fields
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    private static CgoRMLinkingResultDetails FetchLinkingRecords(LoadStrategyResult loadStrategyResult)
    {
      var details = new CgoRMLinkingResultDetails();

      if (loadStrategyResult.IsLoaded(RejectionMemoRecordRepositoryConstants.MemoAmountsParameterName))
      {
        details.MemoAmount = RMLinkingAmountRepository.LoadEntities(loadStrategyResult, null);
      }

      if (loadStrategyResult.IsLoaded(RejectionMemoRecordRepositoryConstants.MemoRecordsParameterName))
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
    private CgoRMLinkingResultDetails FetchLinkedMemoAmounts(LoadStrategyResult loadStrategyResult)
    {
      var details = new CgoRMLinkingResultDetails();

      if (loadStrategyResult.IsLoaded(RejectionMemoRecordRepositoryConstants.MemoAmountsParameterName))
      {
        details.MemoAmount = RMLinkingAmountRepository.LoadEntities(loadStrategyResult, null);
      }

      return details;
    }

    //private RMLinkingResultDetails ExecuteStoredProc<T>(StoredProcedure sp, OracleParameter[] oraInputParameters, bool isCheckLinking, Func<LoadStrategyResult, T> fetch)
    //{
    //  using (var result = new LoadStrategyResult())
    //  {
    //    using (var cmd = Context.CreateStoreCommand(sp.Name, CommandType.StoredProcedure) as OracleCommand)
    //    {
    //      cmd.Parameters.AddRange(oraInputParameters);

    //      // Add result parameters to Oracle Parameter Collection
    //      foreach (SPResultObject resObj in sp.GetResultSpec())
    //      {
    //        var resultParam = new OracleParameter(resObj.ParameterName, OracleDbType.Cursor);
    //        resultParam.Direction = ParameterDirection.Output;
    //        cmd.Parameters.Add(resultParam);

    //        //if the entity is requested, add it to the result
    //        result.Add(resObj.EntityName, resultParam);
    //      }

    //      using (cmd.Connection.CreateConnectionScope())
    //      {
    //        //Execute SP

    //        //Set CommandTimeout value to value given in the Config file 
    //        //if it NOT in the config then it will be set to default value 0.
    //        cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);

    //        cmd.ExecuteNonQuery();

    //        //Allow the caller to populate results
    //        var rmResult = fetch(result) as RMLinkingResultDetails;
    //        LoadOutParameters(cmd, rmResult, isCheckLinking);
    //        return rmResult;
    //      }
    //    }
    //  }
    //}

    //private void LoadOutParameters(OracleCommand cmd, RMLinkingResultDetails rmResult, bool isCheckLinking)
    //{
    //  if (isCheckLinking)
    //  {
    //    if (cmd.Parameters.Contains(RejectionMemoRecordRepositoryConstants.IsLinkingSuccessfulParameterName))
    //    {
    //      var isLinkingSuccessful = cmd.Parameters[RejectionMemoRecordRepositoryConstants.IsLinkingSuccessfulParameterName].Value.ToString();
    //      if (!string.IsNullOrEmpty(isLinkingSuccessful))
    //      {
    //        if (isLinkingSuccessful == "1")
    //          rmResult.IsLinkingSuccessful = true;
    //      }
    //    }

    //    if (cmd.Parameters.Contains(RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName))
    //    {
    //      var currConvFactor = cmd.Parameters[RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName].Value.ToString();
    //      if (!string.IsNullOrEmpty(currConvFactor))
    //        rmResult.CurrencyConversionFactor = Convert.ToDecimal(currConvFactor);
    //    }
       
    //  }
    //  rmResult.ErrorMessage = cmd.Parameters[RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName].Value.ToString();
    //  var hasBreakdown = cmd.Parameters[RejectionMemoRecordRepositoryConstants.HasCouponBreakdownParameterName].Value.ToString();
    //  if (!string.IsNullOrEmpty(hasBreakdown))
    //  {
    //    if (hasBreakdown == "1")
    //    rmResult.HasBreakdown = true;
    //  }
    //}
    
    //public void CopyLinkedRMCouponDetails(Guid rejectionMemoGuidId)
    //{
    //  var parameters = new ObjectParameter[1];

    //  parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoIdParameterName, typeof(Guid))
    //  {
    //    Value = rejectionMemoGuidId
    //  };

    //  ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.InheritRMCouponDetails, parameters);
    //}

    // <summary>
    // Used in Form F RM.
    // </summary>
    // <param name="criteria"></param>
    // <returns></returns>
    //public SamplingFormFLinkingResult GetFormDELinkingDetails(SamplingRMLinkingCriteria criteria)
    //{
    //  var parameters = new ObjectParameter[13];
    //  parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ReasonCodeParameterName, typeof(string)) { Value = criteria.ReasonCode };
    //  parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.YourInvoiceNumberParameterName,typeof(int)) { Value = criteria.InvoiceNumber };
    //  parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMonthParameterName,typeof(int)) { Value = criteria.BillingMonth }; 
    //  parameters[3] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingYearParameterName,typeof(int)) { Value = criteria.BillingYear }; 
    //  parameters[4] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingPeriodParameterName,typeof(int)) { Value = criteria.BillingPeriod }; 
    //  parameters[5] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingMemberIdParameterName,typeof(int)) { Value = criteria.BillingMemberId }; 
    //  parameters[6] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BilledMemberIdParameterName,typeof(int)) { Value = criteria.BilledMemberId }; 
    //  parameters[7] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RejectingInvoiceIdParameterName,typeof(Guid)) { Value = criteria.RejectingInvoiceId}; 
    //  parameters[8] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ProvBillingMonthParameterName,typeof(int)) { Value = criteria.ProvBillingMonth};
    //  parameters[9] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ProvBillingYearParametername, typeof(int)) { Value = criteria.ProvBillingYear };
    //  parameters[10] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.IgnoreValidationParameterName, typeof(int)) { Value = criteria.IgnoreValidationOnMigrationPeriod ? 1 : 0};
    //  parameters[10] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.IgnoreValidationParameterName, typeof(int)) { Value = false ? 1 : 0 };

    //  parameters[11] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName, typeof(decimal));
    //  parameters[12] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, typeof(string));

    //  ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.GetDELinkingdetails, parameters);
    //  var deDetails = new SamplingFormFLinkingResult();
    //  deDetails.CurrencyConversionFactor = string.IsNullOrEmpty(parameters[11].Value.ToString()) ? 0 : decimal.Parse(parameters[11].Value.ToString());
    //  deDetails.ErrorMessage = parameters[12].Value.ToString();
      
    //  return deDetails;
    //}

    ///// <summary>
    ///// Used in Form XF RM.
    ///// </summary>
    ///// <param name="criteria"></param>
    ///// <returns></returns>
    //public RMLinkingResultDetails GetSamplingFormFLinkingDetails(SamplingRMLinkingCriteria criteria)
    //{
    //  var parameters = new OracleParameter[14];
    //  parameters[0] = new OracleParameter(RejectionMemoRecordRepositoryConstants.ReasonCodeParameterName, criteria.ReasonCode);
    //  parameters[1] = new OracleParameter(RejectionMemoRecordRepositoryConstants.YourInvoiceNumberParameterName, criteria.InvoiceNumber);
    //  parameters[2] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingMonthParameterName, criteria.BillingMonth);
    //  parameters[3] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingYearParameterName, criteria.BillingYear);
    //  parameters[4] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingPeriodParameterName, criteria.BillingPeriod);
    //  parameters[5] = new OracleParameter(RejectionMemoRecordRepositoryConstants.YourRMNumberParameterName, criteria.RejectionMemoNumber);
    //  parameters[6] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBillingMemberIdParameterName, criteria.BillingMemberId);
    //  parameters[7] = new OracleParameter(RejectionMemoRecordRepositoryConstants.InvBilledMemberIdParameterName, criteria.BilledMemberId);
    //  var rejectingInvParameter = new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectingInvoiceIdParameterName, criteria.RejectingInvoiceId);
    //  rejectingInvParameter.OracleDbType = OracleDbType.Raw;
    //  parameters[8] = rejectingInvParameter;
    //  parameters[9] = new OracleParameter(RejectionMemoRecordRepositoryConstants.ProvBillingYearParametername, criteria.ProvBillingYear);
    //  parameters[10] = new OracleParameter(RejectionMemoRecordRepositoryConstants.ProvBillingMonthParameterName, criteria.ProvBillingMonth);
      

   

    //  parameters[11] = new OracleParameter { ParameterName = RejectionMemoRecordRepositoryConstants.CurrencyConversionFactorParameterName, OracleDbType = OracleDbType.Number, Direction = ParameterDirection.Output };
    //  //parameters[12] = new OracleParameter { ParameterName = RejectionMemoRecordRepositoryConstants.IsLinkingSuccessfulParameterName, Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Number };
    //  parameters[12] = new OracleParameter { ParameterName = RejectionMemoRecordRepositoryConstants.HasCouponBreakdownParameterName, Direction = ParameterDirection.Output, OracleDbType = OracleDbType.Number };
    //  parameters[13] = new OracleParameter { ParameterName = RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };

    //  return ExecuteStoredProc(SisStoredProcedures.GetFormFLinkingDetails, parameters, true, FetchLinkingRecords);
    //}

    ///// <summary>
    ///// Validates the rejection memo acceptable amount difference.
    ///// </summary>
    ///// <param name="invoiceId">The invoice id.</param>
    ///// <param name="billingCode">The billing code.</param>
    ///// <returns></returns>
    //public string ValidateRejectionMemoAcceptableAmountDifference(Guid invoiceId, int billingCode)
    //{
    //  var parameters = new ObjectParameter[3];
    //  parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
    //  parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.BillingCodeParameterName, typeof(int)) { Value = billingCode };
    //  parameters[2] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.ErrorCodeParameterName, typeof(string));

    //  ExecuteStoredProcedure(RejectionMemoRecordRepositoryConstants.ValidateRMAcceptableAmountDiffFunctionName, parameters);
    //  return parameters[2].Value.ToString();
    //}
    //  return rejectionMemos;
    //}

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

    public static List<CargoRejectionMemo> LoadAuditEntities(ObjectSet<CargoRejectionMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoRejectionMemo> link, string entity)
    {
      if (link == null)
        link = new Action<CargoRejectionMemo>(c => { });
      List<CargoRejectionMemo> rejectionMemo = new List<CargoRejectionMemo>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RejectionMemo))
      {
        // first result set includes the category
        foreach (var c in
          cargoMaterializers.CargoInvoiceRejectionMemoAuditMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<CargoRejectionMemo>(link)
          )
        {
          rejectionMemo.Add(c);
        }
        reader.Close();
      }

      //Load Correspondence by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.Correspondence) && rejectionMemo.Count != 0)
      {
        var correspodences = CargoCorrespondenceRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<CargoCorrespondence>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.CargoEntities.Correspondence);
        foreach (var memo in rejectionMemo)
        {
          if (memo.Correspondence != null)
          {
            memo.Correspondences = correspodences.Where(c => c.CorrespondenceNumber == memo.Correspondence.CorrespondenceNumber).ToList();
          }
        }

      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RmAwb) && rejectionMemo.Count != 0)
      {
        RMAwbRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RMAwb>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.CargoEntities.RmAwb);
      }

      // Load RejectionMemoVat by calling respective LoadEntity
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RejectionMemoVat) && rejectionMemo.Count != 0)
      {
        RejectionMemoVatRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<CgoRejectionMemoVat>()
                  , loadStrategyResult
                  , null);
      }

      // Load RejectionMemoAttachments
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RejectionMemoAttachments) && rejectionMemo.Count != 0)
      {
        CgoRejectionMemoAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<CgoRejectionMemoAttachment>()
                  , loadStrategyResult
                  , null);
      }

      return rejectionMemo;
    }

    //CMP#674-Validation of Coupon and AWB Breakdowns in Rejections
    public List<InvalidRejectionMemoDetails> IsYourRejectionCouponDropped(Guid invoiceId, Guid? rejectionMemoId = null, string YourInvoiceNo = null, string YourRejectionNo = null, int YourInvoiceYear = 0, int YourInvoiceMonth = 0, int YourInvoicePeriod = 0)
    {
        var parameters = new ObjectParameter[7];

        parameters[0] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.InvoiceId, typeof(Guid)) { Value = invoiceId };
        if (rejectionMemoId.HasValue)
        {
            parameters[1] = new ObjectParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoId,
                                                typeof(Guid)) { Value = rejectionMemoId.Value };
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

        /* Calling SP PROC_CGO_IS_RM_CPN_DROPPED, to apply CMP#674 validation. A cursor is returned by SP. cursor has list of all RMs failing validation. */
        var rejectedRMCoupons = ExecuteStoredFunction<InvalidRejectionMemoDetails>(RejectionMemoRecordRepositoryConstants.IsCargoYourRejectionCouponDroppedFunctionName, parameters);

        return rejectedRMCoupons.ToList();

    }
  }
}

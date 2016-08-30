using System;
using System.Configuration;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Core;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Cargo;
using Iata.IS.Data.Impl;
using System.Collections.Generic;
using Devart.Data.Oracle;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Pax.Common;
using System.Data;

namespace Iata.IS.Data.Cargo.Impl
{
  public class RMAwbRepository : Repository<RMAwb>, IRMAwbRepository
  {
    public long GetRMAwbDuplicateCount(int rejectionStage, string awbIssuingAirline, string carriageFromId, string carriageToId, int awbSerialNumber, DateTime? awbIssueDate,int billingmemberId, int billedMemberId, int billingMonth, int billingYear, int billingCode)
    {
      var parameters = new ObjectParameter[12];

      parameters[0] = new ObjectParameter(RMAwbRepositoryConstants.AwbSerialNumberParameterName, typeof(int)) { Value = awbSerialNumber };
      parameters[1] = new ObjectParameter(RMAwbRepositoryConstants.AwbIssuingAirlineParameterName, typeof(string)) { Value = awbIssuingAirline };
      parameters[2] = new ObjectParameter(RMAwbRepositoryConstants.CarriageFromParameterName, typeof(long)) { Value = carriageFromId };
      parameters[3] = new ObjectParameter(RMAwbRepositoryConstants.CarriageToParameterName, typeof(int)) { Value = carriageToId };
      parameters[4] = new ObjectParameter(RMAwbRepositoryConstants.AwbIssueDateParameterName, typeof(int)) { Value = awbIssueDate };
      parameters[5] = new ObjectParameter(RMAwbRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };
      parameters[6] = new ObjectParameter(RMAwbRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
      parameters[7] = new ObjectParameter(RMAwbRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingmemberId };
      parameters[8] = new ObjectParameter(RMAwbRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
      parameters[9] = new ObjectParameter(RMAwbRepositoryConstants.RejectionStageParameterName, typeof(int)) { Value = rejectionStage };
      parameters[10] = new ObjectParameter(RMAwbRepositoryConstants.AwbBillingCodeParameterName, typeof(int)) { Value = billingCode };

      parameters[11] = new ObjectParameter(RMAwbRepositoryConstants.DuplicateCountParameterName, typeof(long));

      ExecuteStoredProcedure(RMAwbRepositoryConstants.GetRMAwbDuplicateCountFunctionName, parameters);

      return long.Parse(parameters[11].Value.ToString());
    }

    public override RMAwb Single(System.Linq.Expressions.Expression<Func<RMAwb, bool>> where)
    {
      throw new NotImplementedException("Use Load Strategy overload of Single method instead.");
    }


    /// <summary>
    /// Gets the RM awb linking details.
    /// </summary>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <param name="serialNo">The serial no.</param>
    /// <param name="checkDigit">The check digit.</param>
    /// <param name="rmId">The rm id.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="awbBillingCode">The awb billing code.</param>
    /// <returns></returns>
    public RMLinkedAwbDetails GetRMAwbLinkingDetails(string issuingAirline, int serialNo, Guid rmId, int billingMemberId, int billedMemberId, int awbBillingCode)
    {
      var parameters = new OracleParameter[7];
      parameters[0] = new OracleParameter(RMAwbRepositoryConstants.AwbIssuingAirlineParameterName, issuingAirline);
      parameters[1] = new OracleParameter(RMAwbRepositoryConstants.AwbSerialNumberParameterName, serialNo);
      
      parameters[2] = new OracleParameter(RMAwbRepositoryConstants.AwbBillingCodeParameterName, awbBillingCode);
      var rmIdParameter = new OracleParameter(RMAwbRepositoryConstants.RejectionMemoIdParameterName, rmId) {OracleDbType = OracleDbType.Raw};
      parameters[3] = rmIdParameter;
      parameters[4] = new OracleParameter(RMAwbRepositoryConstants.BillingMemberIdParameterName, billingMemberId);
      parameters[5] = new OracleParameter(RMAwbRepositoryConstants.BilledMemberIdParameterName, billedMemberId);
      parameters[6] = new OracleParameter { ParameterName = RMAwbRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };

      return ExecuteStoredProc(SisStoredProcedures.GetRMAwbLinkingDetails, parameters, FetchLinkedAwbAmounts);
    }
    
    /// <summary>
    /// Get linking details for rejection memo Coupon when multiple records are found in rejected enity then as per user selection fetch data for selected coupon
    /// </summary>
    /// <param name="awbId"></param>
    /// <param name="rmId"></param>
    /// <returns></returns>
    public RMLinkedAwbDetails GetLinkedAwbAmountDetails(Guid awbId, Guid rmId)
    {
      var parameters = new OracleParameter[3];
      var awbIdParameter = new OracleParameter(RMAwbRepositoryConstants.AwbIdParameterName, awbId) { OracleDbType = OracleDbType.Raw };
      parameters[0] = awbIdParameter;
      var rmIdParameter = new OracleParameter(RMAwbRepositoryConstants.RejectionMemoIdParameterName, rmId) {OracleDbType = OracleDbType.Raw};
      parameters[1] = rmIdParameter;
      parameters[2] = new OracleParameter { ParameterName = RMAwbRepositoryConstants.ErrorCodeParameterName, Direction = ParameterDirection.Output };

      return ExecuteStoredProc(SisStoredProcedures.GetLinkedAwbAmountDetails, parameters, r => FetchLinkedAwbAmounts(r));
    }

   
    #region Load strategy

    /// <summary>
    /// LoadStrategy method overload of Single method
    /// </summary>
    /// <param name="rmAwbId">RMCouponBreakdown Id</param>
    /// <returns>RMCoupon object</returns>
    public RMAwb Single(Guid rmAwbId)
    {
      var entities = new[] { LoadStrategy.CargoEntities.RmAwb, LoadStrategy.CargoEntities.RmAwbVat, LoadStrategy.CargoEntities.RmAwbVatIdentifier,
                                      LoadStrategy.CargoEntities.RmAwbAttachments, LoadStrategy.CargoEntities.RmAwbOtherCharges,LoadStrategy.CargoEntities.RmAwbProrateLadder, LoadStrategy.CargoEntities.RejectionMemo, LoadStrategy.Entities.AttachmentUploadedbyUser};

      var loadStrategy = new LoadStrategy(string.Join(",", entities));

      var rmAwbRecords = GetRMAwbLS(loadStrategy, ConvertUtil.ConvertGuidToString(rmAwbId));

      if (rmAwbRecords.Count > 0)
      {
        if (rmAwbRecords.Count > 1) throw new ApplicationException("Multiple records found");
        return rmAwbRecords[0];
      }
      return null;
    }

    /// <summary>
    /// LoadStrategy method overload of Single method
    /// </summary>
    /// <param name="rmAwbId">RMCouponBreakdown Id</param>
    /// <returns>RMCoupon object</returns>
    public RMAwb GetRejectionMemoWithAwb(Guid rmAwbId)
    {
      var entities = new[] { LoadStrategy.CargoEntities.RmAwb, LoadStrategy.CargoEntities.RejectionMemo};

      var loadStrategy = new LoadStrategy(string.Join(",", entities));

      var rmAwbRecords = GetRMAwbLS(loadStrategy, ConvertUtil.ConvertGuidToString(rmAwbId));

      if (rmAwbRecords.Count > 0)
      {
        if (rmAwbRecords.Count > 1) throw new ApplicationException("Multiple records found");
        return rmAwbRecords[0];
      }
      return null;
    }

    /// <summary>
    ///  Gets list of RM AWB objects
    /// </summary>
    /// <param name="loadStrategy">loadStrategy string</param>
    /// <param name="rmAwbId">rmCouponId</param>
    /// <returns>List of RM AWB objects</returns>
    private List<RMAwb> GetRMAwbLS(LoadStrategy loadStrategy, string rmAwbId = null)
    {
      return ExecuteLoadsSP(SisStoredProcedures.GetCgoRejectionMemo,
                                loadStrategy,
                                new[] { new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoIdParameterName, null),  
                                    new OracleParameter(RejectionMemoRecordRepositoryConstants.CorrespondenceIdParamName, null) ,
                                    new OracleParameter(RejectionMemoRecordRepositoryConstants.RejectionMemoAwbIdParameterName, rmAwbId) ,
                                  },
                                FetchRecord);
    }

    /// <summary>
    /// Fetches the record.
    /// </summary>
    /// <param name="loadStrategyResult">The load strategy result.</param>
    /// <returns></returns>
    private List<RMAwb> FetchRecord(LoadStrategyResult loadStrategyResult)
    {
      var rmAwbRecords = new List<RMAwb>();
      var cargoMaterializers = new CargoMaterializers();
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RmAwb))
      {
        rmAwbRecords = LoadEntities(EntityObjectSet, loadStrategyResult, null);
      }

      /* Rejection Memo is parent entity to RejectionMemoCoupon
         * It will become circular call, if it's binding is implemented into LoadEntities function 
         * so it is better to implement it in this function*/
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemo))
      {
        // first result set includes the category
          cargoMaterializers.CargoRejectionMemoMaterializer.Materialize(reader).Bind(EntityObjectSet.Context.CreateObjectSet<CargoRejectionMemo>()).ToList();
        reader.Close();
      }

      return rmAwbRecords;
    }

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<RMAwb> LoadEntities(ObjectSet<RMAwb> objectSet, LoadStrategyResult loadStrategyResult, Action<RMAwb> link)
    {
      if (link == null)
        link = new Action<RMAwb>(c => { });

      var rmAwbs = new List<RMAwb>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RmAwb))
      {
        foreach (var c in
            cargoMaterializers.CgoRmAwbMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          rmAwbs.Add(c);
        }
        reader.Close();
      }

      // Load RejectionMemoCouponVat by calling respective LoadEntity
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RmAwbVat) && rmAwbs.Count != 0)
      {
        RMAwbVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMAwbVat>()
                  , loadStrategyResult
                  , null);
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RmAwbAttachments) && rmAwbs.Count != 0)
      {
        RMAwbAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMAwbAttachment>()
                  , loadStrategyResult
                  , null);
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RmAwbProrateLadder) && rmAwbs.Count != 0)
      {
          RMAwbProrateLadderDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMAwbProrateLadderDetail>()
                  , loadStrategyResult
                  , null);
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RmAwbOtherCharges) && rmAwbs.Count != 0)
      {
        RMAwbOtherChargeRepository.LoadEntities(objectSet.Context.CreateObjectSet<RMAwbOtherCharge>()
                  , loadStrategyResult
                  , null);
      }

      return rmAwbs;
    }

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<RMAwb> LoadAuditEntities(ObjectSet<RMAwb> objectSet, LoadStrategyResult loadStrategyResult, Action<RMAwb> link, string entity)
    {
      if (link == null)
        link = new Action<RMAwb>(c => { });
      var rmAwbs = new List<RMAwb>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RmAwb))
      {
        foreach (var c in
            cargoMaterializers.CgoRmAwbAuditMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<RMAwb>(link)
            )
        {
          rmAwbs.Add(c);
        }
        reader.Close();
      }

      // Load RejectionMemoCouponVat by calling respective LoadEntity
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RmAwbVat) && rmAwbs.Count != 0)
      {
        RMAwbVatRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RMAwbVat>()
                  , loadStrategyResult
                  , null);
      }

      // Load RejectionMemoCouponAttachments
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.RmAwbAttachments) && rmAwbs.Count != 0)
      {
        RMAwbAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RMAwbAttachment>()
                  , loadStrategyResult
                  , null);
      }

      return rmAwbs;
    }

    private RMLinkedAwbDetails ExecuteStoredProc<T>(StoredProcedure sp, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, T> fetch)
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
            var rmResult = fetch(result) as RMLinkedAwbDetails;
            if(rmResult != null)
              rmResult.ErrorMessage = cmd.Parameters[RMAwbRepositoryConstants.ErrorCodeParameterName].Value.ToString();

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
    public RMLinkedAwbDetails FetchLinkedAwbAmounts(LoadStrategyResult loadStrategyResult)
    {
      var details = new RMLinkedAwbDetails();

      if (loadStrategyResult.IsLoaded(RMAwbRepositoryConstants.AwbListParameterName))
      {
        var couponDetail = RMLinkedAwbRepository.LoadEntities(loadStrategyResult, null);
        details.RMLinkedAwbRecords.AddRange(couponDetail);
      }

      if (loadStrategyResult.IsLoaded(RMAwbRepositoryConstants.AwbAmountsParameterName))
      {
        var records = LoadLinkedAwbEntities(loadStrategyResult, null);
        details.Details = records;
      }

      return details;
    }
    public static RMAwb LoadLinkedAwbEntities(LoadStrategyResult loadStrategyResult, Action<RMCoupon> link)
    {
      var records = new List<RMAwb>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(RMAwbRepositoryConstants.AwbAmountsParameterName))
      {
        if (reader != null && reader.HasRows)
        {
            records = cargoMaterializers.LinkedRMAwbMaterializer.Materialize(reader).ToList();
          reader.Close();
        }
      }

      // Commenting this out because other charge records need not be inherited.
      //if (records.Count > 0)
      //{
      //  if (loadStrategyResult.IsLoaded(RMAwbRepositoryConstants.AwbOtherChargeParameterName))
      //  {
      //    var otherChargeRecords = RMAwbOtherChargeRepository.LoadLinkedEntities(loadStrategyResult, null);
      //    records[0].OtherCharge.AddRange(otherChargeRecords);
      //  }
      //}

      return records.Count > 0 ? records[0] : null;
    }
    #endregion
  }
}

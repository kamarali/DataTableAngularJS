using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Cargo.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoCreditMemoRecordRepository : Repository<CargoCreditMemo>, ICargoCreditMemoRecordRepository
  {
    public long GetCreditMemoDuplicateCount(string creditMemoNumber, int billedMemberId, int billingMemberId, int billingMonth, int billingYear, int billingPeriod)
    {
      var parameters = new ObjectParameter[7];
      parameters[0] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
      parameters[1] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.CreditMemoNumberParameterName, typeof(string)) { Value = creditMemoNumber };
      parameters[2] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[3] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };
      parameters[4] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
      parameters[5] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.BillingPeriodParameterName, typeof(int)) { Value = billingPeriod };

      parameters[6] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.DuplicateCountParameterName, typeof(long));

      ExecuteStoredProcedure(CargoCreditMemoRecordRepositoryConstants.GetCmDuplicateCountFunctionName, parameters);

      return long.Parse(parameters[6].Value.ToString());
    }
    
    #region - LoadStrategy -
    
    /// <summary>
    ///  LoadStrategy method overload of Single method
    /// </summary>
    /// <param name="creditMemoId">CreditMemo Id</param>
    /// <returns>CreditMemo</returns>
    public CargoCreditMemo Single(Guid creditMemoId)
    {
      var entities = new[] { LoadStrategy.CargoEntities.CreditMemo,LoadStrategy.CargoEntities.CreditMemoVat,LoadStrategy.CargoEntities.CreditMemoVatIdentifier,
                                      LoadStrategy.CargoEntities.CreditMemoAttachments,LoadStrategy.CargoEntities.CmAwb,LoadStrategy.CargoEntities.AttachmentUploadedbyUser};

      var loadStrategy = new LoadStrategy(string.Join(",", entities));

      var creditMemos = GetCargoCreditMemoLS(new LoadStrategy(string.Join(",", entities)), creditMemoId: creditMemoId);
      if (creditMemos == null || creditMemos.Count == 0) return null;
      else if (creditMemos.Count > 1) throw new ApplicationException("Multiple records found");
      else return creditMemos[0];
    }

    private List<CargoCreditMemo> GetCargoCreditMemoLS(LoadStrategy loadStrategy, Guid? creditMemoId = null, Guid? creditMemoAwbId = null)
    {
      return base.ExecuteLoadsSP(SisStoredProcedures.GetCargoCreditMemo,
                                 loadStrategy,
                                 new OracleParameter[]
                                         {
                                             new OracleParameter(CargoCreditMemoRecordRepositoryConstants.CreditMemoIdParameterName,
                                                                 creditMemoId.HasValue ? ConvertUtil.ConvertGuidToString(creditMemoId.Value) : null),
                                             new OracleParameter(CargoCreditMemoRecordRepositoryConstants.CreditMemoAwbIdparameterName,
                                                                 creditMemoAwbId.HasValue ? ConvertUtil.ConvertGuidToString(creditMemoAwbId.Value) : null)

                                         },
                                 r => this.FetchRecord(r));
    }


    private List<CargoCreditMemo> FetchRecord(LoadStrategyResult loadStrategyResult)
    {
      var creditMemos = new List<CargoCreditMemo>();
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CreditMemo))
      {
        creditMemos = LoadEntities(this.EntityObjectSet, loadStrategyResult, null);
      }
      return creditMemos;
    }

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<CargoCreditMemo> LoadEntities(ObjectSet<CargoCreditMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoCreditMemo> link)
    {
      if (link == null)
        link = new Action<CargoCreditMemo>(c => { });

      var cargoMaterializers = new CargoMaterializers();
      var creditMemos = new List<CargoCreditMemo>();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CreditMemo))
      {
        // first result set includes the category
        foreach (var c in cargoMaterializers.CargoCreditMemoMaterializer.Materialize(reader).Bind(objectSet).ForEach<CargoCreditMemo>(link))
        {
          creditMemos.Add(c);
        }
        reader.Close();
      }

      // Load CreditMemoVat by calling respective LoadEntity
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CreditMemoVat) && creditMemos.Count != 0)
      {
        CargoCreditMemoVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoCreditMemoVat>()
                  , loadStrategyResult
                  , null);
      }

      // Load CreditMemoAttachments
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CreditMemoAttachments) && creditMemos.Count != 0)
      {
        CargoCreditMemoAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoCreditMemoAttachment>()
                  , loadStrategyResult
                  , null);
      }

      //Load CreditMemoCoupon by calling respective LoadEntities method
      /* if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CmAwb) && creditMemos.Count != 0)
      {
        CargoCreditMemoAwbRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMAirWayBill>()
                , loadStrategyResult
                , i => i.CreditMemoRecord = creditMemos.Find(j => j.Id == i.CreditMemoId));
      } */
      return creditMemos;
    }

    #endregion
  }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoBillingMemoRepository : Repository<CargoBillingMemo>, ICargoBillingMemoRepository
  {
    /// <summary>
    /// Gets or sets the Billing Memo Attachment repository.
    /// </summary>
    public long GetCargoBillingMemoDuplicateCount(string billingMemoNumber, int billedMemberId, int billingMemberId, int billingMonth, int billingYear, int billingPeriod)
    {
        var parameters = new ObjectParameter[7];
        parameters[0] = new ObjectParameter(CargoBillingMemoRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
        parameters[1] = new ObjectParameter(CargoBillingMemoRepositoryConstants.BillingMemoNumberParameterName, typeof(string)) { Value = billingMemoNumber };
        parameters[2] = new ObjectParameter(CargoBillingMemoRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
        parameters[3] = new ObjectParameter(CargoBillingMemoRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };
        parameters[4] = new ObjectParameter(CargoBillingMemoRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
        parameters[5] = new ObjectParameter(CargoBillingMemoRepositoryConstants.BillingPeriodParameterName, typeof(int)) { Value = billingPeriod };
        parameters[6] = new ObjectParameter(CargoBillingMemoRepositoryConstants.DuplicateCountParameterName, typeof(long));


        ExecuteStoredProcedure(CargoBillingMemoRepositoryConstants.GetBMDuplicateCountFunctionName, parameters);

        return long.Parse(parameters[6].Value.ToString());
    }

    public CargoBillingMemo Single(Guid billingMemoId)
    {

      var entities = new string[] 
                        { 
                            LoadStrategy.CargoEntities.BillingMemo, LoadStrategy.CargoEntities.BillingMemoVat,
                            LoadStrategy.CargoEntities.BillingMemoVatIdentifier, LoadStrategy.CargoEntities.BillingMemoAttachments,
                            LoadStrategy.CargoEntities.BmAwb,LoadStrategy.CargoEntities.BMAttachmentUploadedByUser
                        };

      List<CargoBillingMemo> billingMemos = GetCargoBillingMemoLS(new LoadStrategy(string.Join(",", entities)), billingMemoId: billingMemoId);
      if (billingMemos == null || billingMemos.Count == 0) return null;
      else if (billingMemos.Count > 1) throw new ApplicationException("Multiple records found");
      else return billingMemos[0];
    }

    private List<CargoBillingMemo> GetCargoBillingMemoLS(LoadStrategy loadStrategy, Guid? billingMemoId = null, Guid? billingMemoCouponId = null)
    {
      return base.ExecuteLoadsSP(SisStoredProcedures.GetCargoBillingMemo,
                                 loadStrategy,
                                 new OracleParameter[]
                                         {

                                             new OracleParameter(CargoBillingMemoRepositoryConstants.BillingMemoIdParameterName,
                                                                 billingMemoId.HasValue ? ConvertUtil.ConvertGuidToString(billingMemoId.Value) : null),
                                             new OracleParameter(CargoBillingMemoRepositoryConstants.BillingMemAwbIdParameterName,
                                                                 billingMemoCouponId.HasValue ? ConvertUtil.ConvertGuidToString(billingMemoCouponId.Value) : null)

                                         },
                                 r => this.FetchRecord(r));
    }

    private List<CargoBillingMemo> FetchRecord(LoadStrategyResult loadStrategyResult)
    {
      List<CargoBillingMemo> billingMemos = new List<CargoBillingMemo>();
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingMemo))
      {
        billingMemos = CargoBillingMemoRepository.LoadEntities(this.EntityObjectSet, loadStrategyResult, null);
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
    public static List<CargoBillingMemo> LoadEntities(ObjectSet<CargoBillingMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingMemo> link)
    {
      if (link == null)
        link = new Action<CargoBillingMemo>(c => { });

      var billingMemos = new List<CargoBillingMemo>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.BillingMemo))
      {

          billingMemos = cargoMaterializers.CargoBillingMemoMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }

      if (billingMemos.Count > 0)
      {
        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingMemoVat))
        {
          CargoBillingMemoVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoBillingMemoVat>(), loadStrategyResult, null, LoadStrategy.CargoEntities.BillingMemoVat);
        }

        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingMemoVatIdentifier))
        {
          CargoVatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<CgoVatIdentifier>(), loadStrategyResult, null, LoadStrategy.CargoEntities.BillingMemoVatIdentifier);
        }

        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingMemoAttachments))
        {
          CargoBillingMemoAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoBillingMemoAttachment>(), loadStrategyResult, null, LoadStrategy.CargoEntities.BillingMemoAttachments);
        }

        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwb))
        {
          CargoBillingMemoAwbRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoBillingMemoAwb>(),
                                                     loadStrategyResult, null);
        }
      }
      return billingMemos;
    }

    public static List<CargoBillingMemo> LoadAuditEntities(ObjectSet<CargoBillingMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingMemo> link, string entity)
    {
      if (link == null)
        link = new Action<CargoBillingMemo>(c => { });
      List<CargoBillingMemo> billingMemo = new List<CargoBillingMemo>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.BillingMemo))
      {
        // first result set includes the category
        foreach (var c in
          cargoMaterializers.CargoBillingMemoAuditMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<CargoBillingMemo>(link)
          )
        {
          billingMemo.Add(c);
        }
        reader.Close();
      }

      if (billingMemo.Count > 0)
      {
        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingMemoVat))
        {
          CargoBillingMemoVatRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<CargoBillingMemoVat>(), loadStrategyResult, null, LoadStrategy.CargoEntities.BillingMemoVat);
        }

        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BillingMemoAttachments))
        {
          CargoBillingMemoAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<CargoBillingMemoAttachment>(), loadStrategyResult, null, LoadStrategy.CargoEntities.BillingMemoAttachments);
        }
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwb) && billingMemo.Count != 0)
      {
        CargoBillingMemoAwbRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<CargoBillingMemoAwb>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.CargoEntities.BmAwb);
      }

      return billingMemo;
    }

  }
}

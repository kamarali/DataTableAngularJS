using System;
using System.Collections.Generic;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using System.Data.Objects;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoBillingMemoAwbRepository : Repository<CargoBillingMemoAwb>,ICargoBillingMemoAwbRepository
  {
     /// <summary>
    /// This will load list of CargoBillingMemoAwb objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CargoBillingMemoAwb> LoadEntities(ObjectSet<CargoBillingMemoAwb> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingMemoAwb> link)
     {
       if (link == null)
         link = new Action<CargoBillingMemoAwb>(c => { });

       var billingMemoAwbs = new List<CargoBillingMemoAwb>();
       var cargoMaterializers = new CargoMaterializers();
       using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.BmAwb))
       {
         foreach (var c in
            cargoMaterializers.CargoBillingMemoAwbMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
         {
           billingMemoAwbs.Add(c);
         }
         reader.Close();
       }

       if (billingMemoAwbs.Count > 0)
       {
         // Load BillingMemoAwbVat by calling respective LoadEntity
         if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwbVat))
         {
           BMAwbVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMAwbVat>()
                                           , loadStrategyResult
                                           , null);
         }

         if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwbAttachments))
         {
           BMAwbAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMAwbAttachment>()
                                                  , loadStrategyResult
                                                  , null);
         }

         if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwbProrateLadder))
         {
           BMAwbProrateLadderDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMAwbProrateLadderDetail>()
                                                     , loadStrategyResult
                                                     , null);
         }

         if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwbOtherCharges))
         {
           BMAwbOtherChargeRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMAwbOtherCharge>()
                                                   , loadStrategyResult
                                                   , null);
         }
       }
       return billingMemoAwbs;
     }


    public long GetBMAwbRecordDuplicateCount(int awbSerialNumber, string awbIssuingAirline, int billingMemberId, int billedMemberId, int billingYear, int billingMonth, int awbBillingCode)
    {
        var parameters = new ObjectParameter[8];
        parameters[0] = new ObjectParameter(CargoBillingMemoAwbRepositoryConstants.BMAwbSerialNumberParameterName, typeof(int))
        {
            Value = awbSerialNumber
        };
        
        parameters[1] = new ObjectParameter(CargoBillingMemoAwbRepositoryConstants.BMAwbIssuingAirlineParameterName, typeof(string))
        {
            Value = awbIssuingAirline
        };
        parameters[2] = new ObjectParameter(CargoBillingMemoAwbRepositoryConstants.BillingMemberParameterName, typeof(int))
        {
            Value = billingMemberId
        };
        parameters[3] = new ObjectParameter(CargoBillingMemoAwbRepositoryConstants.BilledMemberParameterName, typeof(int))
        {
            Value = billedMemberId
        };
        parameters[4] = new ObjectParameter(CargoBillingMemoAwbRepositoryConstants.BillingYearParameterName, typeof(int))
        {
            Value = billingYear
        };
        parameters[5] = new ObjectParameter(CargoBillingMemoAwbRepositoryConstants.BillingMonthParameterName, typeof(int))
        {
            Value = billingMonth
        };
        parameters[6] = new ObjectParameter(CargoBillingMemoAwbRepositoryConstants.AwbBillingCodeParameterName, typeof(int))
        {
          Value = awbBillingCode
        };

        parameters[7] = new ObjectParameter(CargoBillingMemoAwbRepositoryConstants.DuplicateCountParameterName, typeof(long));

        ExecuteStoredProcedure(CargoBillingMemoAwbRepositoryConstants.GetBillingMemoAwbDuplicateCountFunctionName, parameters);

        return long.Parse(parameters[7].Value.ToString());
    }

    public CargoBillingMemoAwb Single(Guid bmAwbId)
    {
        var entities = new string[] { LoadStrategy.CargoEntities.BmAwb, LoadStrategy.CargoEntities.BmAwbAttachments, 
      LoadStrategy.CargoEntities.BmAwbVat, LoadStrategy.CargoEntities.BmAwbOtherCharges, LoadStrategy.CargoEntities.BmAwbProrateLadder,LoadStrategy.CargoEntities.BillingMemo,LoadStrategy.CargoEntities.BMAttachmentUploadedByUser };

        var loadStrategy = new LoadStrategy(string.Join(",", entities));
        var bmAwbIdstr = ConvertUtil.ConvertGuidToString(bmAwbId);
        var awbCoupons = GetBmAwbRecordsLs(loadStrategy, bmAwbIdstr);
        CargoBillingMemoAwb bmAwbrecord = null;
        if (awbCoupons.Count > 0)
        {
            if (awbCoupons.Count > 1) throw new ApplicationException("Multiple records found");
            bmAwbrecord = awbCoupons[0];
        }
        return bmAwbrecord;
    }
    /// <summary>
    /// Gets list of AwbRecord objects
    /// </summary>
    /// <param name="loadStrategy"></param>
    /// <param name="awbRecordId"></param>
    /// <returns></returns>
    public List<CargoBillingMemoAwb> GetBmAwbRecordsLs(LoadStrategy loadStrategy, string bmAwbRecordId)
    {
        return base.ExecuteLoadsSP(SisStoredProcedures.GetCargoBillingMemo,
                                  loadStrategy,
                                    new OracleParameter[] { new OracleParameter(CargoBillingMemoRepositoryConstants.BillingMemoIdParameterName, null),
                                        new OracleParameter(CargoBillingMemoRepositoryConstants.BillingMemAwbIdParameterName, bmAwbRecordId)
                                },
                                  this.FetchRecords);
    }

    /// <summary>
    /// Returns multiple records extracted from the result set.
    /// This is done by calling the right repository to populate the object set in the repository.
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    private List<CargoBillingMemoAwb> FetchRecords(LoadStrategyResult loadStrategyResult)
    {
        var bmAwbRecords = new List<CargoBillingMemoAwb>();
        var cargoMaterializers = new CargoMaterializers();
        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwb))
        {
            bmAwbRecords = LoadEntities(base.EntityObjectSet, loadStrategyResult, null);
        }
        using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.BillingMemo))
        {
            // first result set includes the category
            cargoMaterializers.CargoBillingMemoMaterializer.Materialize(reader).Bind(EntityObjectSet.Context.CreateObjectSet<CargoBillingMemo>()).ToList();
            reader.Close();
        }

        return bmAwbRecords;
    }

    public void UpdateBMAwbInvoiceTotal(Guid invoiceId, int userId)
    {
        throw new NotImplementedException();
    }

    public static List<CargoBillingMemoAwb> LoadAuditEntities(ObjectSet<CargoBillingMemoAwb> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingMemoAwb> link, string entityName)
    {
      if (link == null)
      {
        link = new Action<CargoBillingMemoAwb>(c => { });
      }

      var bmAwbs = new List<CargoBillingMemoAwb>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
      {
        // first result set includes the category
          bmAwbs = cargoMaterializers.CargoBillingMemoAwbAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }

      if (bmAwbs.Count > 0)
      {
        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwbVat))
        {
            BMAwbVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMAwbVat>(), loadStrategyResult, null);
        }
        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwbAttachments))
        {
            BMAwbAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMAwbAttachment>(), loadStrategyResult, null);
        }
        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwbOtherCharges))
        {
            BMAwbOtherChargeRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMAwbOtherCharge>(), loadStrategyResult, null);
        }
        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BmAwbProrateLadder))
        {
            BMAwbProrateLadderDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<BMAwbProrateLadderDetail>(), loadStrategyResult, null);
        }
        
      }

      return bmAwbs;
    }

    public CargoBillingMemoAwb GetBillingMemoWithAwb(Guid bmAwbId)
    {
      var entities = new string[] { LoadStrategy.CargoEntities.BmAwb,LoadStrategy.CargoEntities.BillingMemo };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var bmAwbIdstr = ConvertUtil.ConvertGuidToString(bmAwbId);
      var awbCoupons = GetBmAwbRecordsLs(loadStrategy, bmAwbIdstr);
      CargoBillingMemoAwb bmAwbrecord = null;
      if (awbCoupons.Count > 0)
      {
        if (awbCoupons.Count > 1) throw new ApplicationException("Multiple records found");
        bmAwbrecord = awbCoupons[0];
      }
      return bmAwbrecord;
    }
  }
}

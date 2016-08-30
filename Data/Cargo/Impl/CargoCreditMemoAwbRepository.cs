using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoCreditMemoAwbRepository : Repository<CMAirWayBill>, ICargoCreditMemoAwbRepository
  {
    /// <summary>
    /// This will load list of CargoBillingMemoAwb objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CMAirWayBill> LoadEntities(ObjectSet<CMAirWayBill> objectSet, LoadStrategyResult loadStrategyResult, Action<CMAirWayBill> link)
    {
      if (link == null)
        link = new Action<CMAirWayBill>(c => { });

      var cmAirWayBills = new List<CMAirWayBill>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CmAwb))
      {
        foreach (var c in cargoMaterializers.CmAirWayBillMaterializer.Materialize(reader).Bind(objectSet).ForEach(link))
        {
          cmAirWayBills.Add(c);
        }
        reader.Close();
      }

      // Load BillingMemoAwbVat by calling respective LoadEntity
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CmAwbVat) && cmAirWayBills.Count != 0)
      {
        CMAwbVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMAwbVat>()
                  , loadStrategyResult
                  , null);
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CmAwbAttachments) && cmAirWayBills.Count != 0)
      {
        CMAwbAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMAwbAttachment>()
                  , loadStrategyResult
                  , null);
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CmAwbProrateLadder) && cmAirWayBills.Count != 0)
      {
        CargoCMAwbProrateLadderDRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMAwbProrateLadderDetail>()
                  , loadStrategyResult
                  , null);
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CmAwbOtherCharges) && cmAirWayBills.Count != 0)
      {
        CMAwbOtherChargeRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMAwbOtherCharge>()
                  , loadStrategyResult
                  , null);
      }
      return cmAirWayBills;
    }

    public CMAirWayBill Single(Guid cmAwbId)
    {
      var entities = new string[] { LoadStrategy.CargoEntities.CmAwb, LoadStrategy.CargoEntities.CmAwbAttachments, 
      LoadStrategy.CargoEntities.CmAwbVat, LoadStrategy.CargoEntities.CmAwbOtherCharges, LoadStrategy.CargoEntities.CmAwbProrateLadder,LoadStrategy.CargoEntities.CreditMemo };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var cmAwbIdstr = ConvertUtil.ConvertGuidToString(cmAwbId);
      var awbCoupons = GetCmAwbRecordsLs(loadStrategy, cmAwbIdstr);
      CMAirWayBill cmAwbrecord = null;
      if (awbCoupons.Count > 0)
      {
        if (awbCoupons.Count > 1) throw new ApplicationException("Multiple records found");
        cmAwbrecord = awbCoupons[0];
      }
      return cmAwbrecord;
    }

    /// <summary>
    /// Gets list of AwbRecord objects
    /// </summary>
    /// <param name="loadStrategy"></param>
    /// <param name="cmAwbRecordId"></param>
    /// <returns></returns>
    public List<CMAirWayBill> GetCmAwbRecordsLs(LoadStrategy loadStrategy, string cmAwbRecordId)
    {
      return base.ExecuteLoadsSP(SisStoredProcedures.GetCargoCreditMemo,
                                loadStrategy,
                                  new OracleParameter[] { new OracleParameter(CargoCreditMemoRecordRepositoryConstants.CreditMemoIdParameterName, null),
                                        new OracleParameter(CargoCreditMemoRecordRepositoryConstants.CreditMemoAwbIdparameterName, cmAwbRecordId)
                                },
                                this.FetchRecords);
    }

    /// <summary>
    /// Returns multiple records extracted from the result set.
    /// This is done by calling the right repository to populate the object set in the repository.
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    private List<CMAirWayBill> FetchRecords(LoadStrategyResult loadStrategyResult)
    {
      var cmAwbRecords = new List<CMAirWayBill>();
      var cargoMaterializers = new CargoMaterializers();
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CmAwb))
      {
        cmAwbRecords = LoadEntities(base.EntityObjectSet, loadStrategyResult, null);
      }
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CreditMemo))
      {
        // first result set includes the category
        cargoMaterializers.CargoCreditMemoMaterializer.Materialize(reader).Bind(EntityObjectSet.Context.CreateObjectSet<CargoCreditMemo>()).ToList();
        reader.Close();
      }
      
      return cmAwbRecords; 
    }

    public void UpdateCMAwbInvoiceTotal(Guid invoiceId, int userId)
    {
      throw new NotImplementedException();
    }

    public long GetCMAwbRecordDuplicateCount(int awbSerialNumber, string awbIssuingAirline, int billingMemberId, int billedMemberId, int billingYear, int billingMonth, int awbBillingCode)
    {
      var parameters = new ObjectParameter[8];
      parameters[0] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.CMAwbSerialNumberParameterName, typeof(int))
      {
        Value = awbSerialNumber
      };

      parameters[1] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.CMAwbIssuingAirlineParameterName, typeof(string))
      {
        Value = awbIssuingAirline
      };
      parameters[2] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.BillingMemberParameterName, typeof(int))
      {
        Value = billingMemberId
      };
      parameters[3] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.BilledMemberParameterName, typeof(int))
      {
        Value = billedMemberId
      };
      parameters[4] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.AwbBillingYearParameterName, typeof(int))
      {
        Value = billingYear
      };
      parameters[5] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.AwbBillingMonthParameterName, typeof(int))
      {
        Value = billingMonth
      };
      parameters[6] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.AwbBillingCodeParameterName, typeof(int))
      {
        Value = awbBillingCode
      };

      parameters[7] = new ObjectParameter(CargoCreditMemoRecordRepositoryConstants.AwbDuplicateCountParameterName, typeof(long));

      ExecuteStoredProcedure(CargoCreditMemoRecordRepositoryConstants.GetCreditMemoAwbDuplicateCountFunctionName, parameters);

      return long.Parse(parameters[7].Value.ToString());
    }

    public CMAirWayBill GetCreditMemoWithAwb(Guid cmAwbId)
    {
      var entities = new string[] { LoadStrategy.CargoEntities.CmAwb, LoadStrategy.CargoEntities.CreditMemo };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var cmAwbIdstr = ConvertUtil.ConvertGuidToString(cmAwbId);
      var awbCoupons = GetCmAwbRecordsLs(loadStrategy, cmAwbIdstr);
      CMAirWayBill cmAwbrecord = null;
      if (awbCoupons.Count > 0)
      {
        if (awbCoupons.Count > 1) throw new ApplicationException("Multiple records found");
        cmAwbrecord = awbCoupons[0];
      }
      return cmAwbrecord;
    }
  }
}

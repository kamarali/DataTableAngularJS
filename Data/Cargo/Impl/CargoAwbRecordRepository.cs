using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoAwbRecordRepository : Repository<AwbRecord>, ICargoAwbRecordRepository
  {
    /// <summary>
    /// This will load list of AwbRecord objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<AwbRecord> LoadEntities(ObjectSet<AwbRecord> objectSet, LoadStrategyResult loadStrategyResult, Action<AwbRecord> link)
    {
      if (link == null)
        link = new Action<AwbRecord>(c => { });

      var cgoAwbRecord = new List<AwbRecord>();
      var cargoMaterializers = new CargoMaterializers();
      using (var reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.AwbRecord))
      {
        // first result set includes the category
          cgoAwbRecord.AddRange(cargoMaterializers.CargoAirwayBillMaterializer.Materialize(reader).Bind(objectSet).ForEach<AwbRecord>(link));
        reader.Close();
      }

      //Load AwbRecordVat by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.AwbRecordVat) && cgoAwbRecord.Count != 0)
      {
        CargoAwbRecordVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<AwbVat>(), loadStrategyResult, null);
      }

      //Load AwbRecordOther Caharge by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.AwbOtherCharge) && cgoAwbRecord.Count != 0)
      {
        CargoAwbOtherChargesRepository.LoadEntities(objectSet.Context.CreateObjectSet<AwbOtherCharge>(), loadStrategyResult, null);
      }

      //Load AwbRecordAttachment by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.AwbAttachment) && cgoAwbRecord.Count != 0)
      {
        CargoAwbAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<AwbAttachment>(), loadStrategyResult, null, LoadStrategy.CargoEntities.AwbAttachment);
      }

      return cgoAwbRecord;
    }

    /// <summary>
    /// Loadstrategy method overload of Single
    /// </summary>
    /// <param name="awbRecordId">awbRecord Id</param>
    /// <returns>SamplingFormDRecord</returns>
    public AwbRecord Single(Guid awbRecordId)
    {
      var entities = new string[] { LoadStrategy.CargoEntities.AwbRecord, LoadStrategy.CargoEntities.AwbRecordVat, LoadStrategy.CargoEntities.AwbOtherCharge,
      LoadStrategy.CargoEntities.AwbDataVatIdentifier, LoadStrategy.CargoEntities.AwbAttachment, LoadStrategy.CargoEntities.AttachmentUploadedbyUser };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var couponIdstr = ConvertUtil.ConvertGuidToString(awbRecordId);
      var awbCoupons = GetAwbRecordsLs(loadStrategy, couponIdstr);

      AwbRecord coupon = null;
      if (awbCoupons.Count > 0)
      {
        if (awbCoupons.Count > 1) throw new ApplicationException("Multiple records found");
        coupon = awbCoupons[0];
      }
      return coupon;
    }

    /// <summary>
    /// Gets list of AwbRecord objects
    /// </summary>
    /// <param name="loadStrategy"></param>
    /// <param name="awbRecordId"></param>
    /// <returns></returns>
    public List<AwbRecord> GetAwbRecordsLs(LoadStrategy loadStrategy, string awbRecordId)
    {
      return base.ExecuteLoadsSP(SisStoredProcedures.GetAwbRecord,
                                loadStrategy,
                                  new OracleParameter[] { new OracleParameter(CargoInvoiceRepositoryConstants.AwbRecordIdParameterName, awbRecordId)
                                },
                                this.FetchRecords);
    }

    /// <summary>
    /// Returns multiple records extracted from the result set.
    /// This is done by calling the right repository to populate the object set in the repository.
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    private List<AwbRecord> FetchRecords(LoadStrategyResult loadStrategyResult)
    {
      var awbRecords = new List<AwbRecord>();
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.AwbRecord))
      {
        awbRecords = LoadEntities(base.EntityObjectSet, loadStrategyResult, null);
      }

      return awbRecords;
    }

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<AwbRecord> LoadAuditEntities(ObjectSet<AwbRecord> objectSet, LoadStrategyResult loadStrategyResult, Action<AwbRecord> link)
    {
      if (link == null)
        link = new Action<AwbRecord>(c => { });
      List<AwbRecord> coupons = new List<AwbRecord>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.AwbRecord))
      {
        // first result set includes the category
        foreach (var c in
          cargoMaterializers.CargoInvoiceAwbAuditMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<AwbRecord>(link)
          )
        {
          coupons.Add(c);
        }
        reader.Close();
      }

      
      //Load SamplingFormDVat by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.AwbRecordVat) && coupons.Count != 0) CargoAwbRecordVatRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<AwbVat>(), loadStrategyResult, null);

      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.AwbAttachment) && coupons.Count != 0)
        CargoAwbAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<AwbAttachment>()
                , loadStrategyResult
                , null);

      return coupons;
    }

    /// <summary>
    /// /Get the Awb record duplicate count
    /// </summary>
    /// <param name="awbSerialNumber"></param>
    /// <param name="awbIssueingAirline"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="carriageFromId"></param>
    /// <param name="carriageToId"></param>
    /// <param name="awbDate"></param>
    /// <param name="awbBillingCode"></param>
    /// <returns></returns>
    public long GetAwbRecordDuplicateCount(int awbSerialNumber,string awbIssueingAirline, int billingMemberId, string carriageFromId, string carriageToId,  DateTime? awbDate, int awbBillingCode)
    {
      var parameters = new ObjectParameter[8];
      parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.AwbSerialNumberParameterName, typeof(int)) { Value = awbSerialNumber };
      parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.AwbIssueingAirlineParameterName, typeof(long)) { Value = awbIssueingAirline };
      parameters[2] = new ObjectParameter(CargoInvoiceRepositoryConstants.AwbBillingMemberParameterName, typeof(string)) { Value = billingMemberId };
      parameters[3] = new ObjectParameter(CargoInvoiceRepositoryConstants.AwbCarriageFromParameterName, typeof(int)) { Value = carriageFromId };
      parameters[4] = new ObjectParameter(CargoInvoiceRepositoryConstants.AwbCarriageToParameterName, typeof(int)) { Value = carriageToId };
      parameters[5] = new ObjectParameter(CargoInvoiceRepositoryConstants.AwbIssueingDateParameterName, typeof(int)) { Value = awbDate };
      parameters[6] = new ObjectParameter(CargoInvoiceRepositoryConstants.AwbBillingCodeParameterName, typeof(int)) { Value = awbBillingCode };
      
      parameters[7] = new ObjectParameter(CargoInvoiceRepositoryConstants.DuplicateCountParameterName, typeof(long));

      ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.ProcGetAwbRecDupCount, parameters);

      return long.Parse(parameters[7].Value.ToString());
    }

    /// <summary>
    /// Updates the Awb Record Invoice total.
    /// </summary>
    /// <param name="invoiceId">The Invoice id.</param>
    /// <param name="userId">The user id.</param>
    public void UpdateAwbInvoiceTotal(Guid invoiceId, int userId)
    {
      //var parameters = new ObjectParameter[3];

      //parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      //parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.SourceIdParameterName, typeof(int)) { Value = sourceId };
      //parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.UserIdParameterName, typeof(int)) { Value = userId };

      //ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdatePrimeInvoiceTotalFunctionName, parameters);
    }

    public int GetAwbRecordSeqNumber(int batchSeqNo, string invoiceNo)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.AwbBatchSeqNumber, typeof(int)) { Value = batchSeqNo };
      parameters[1] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceNumber, typeof(long)) { Value = invoiceNo };


      parameters[2] = new ObjectParameter(CargoInvoiceRepositoryConstants.AwbRecSeqNumber, typeof(long));

      ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.ProcGetAwbRecSeqNumber, parameters);

      return int.Parse(parameters[2].Value.ToString());
    }
  }
}

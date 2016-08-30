using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class RejectionMemoVatRepository
  {
    /// <summary>
    /// This will load list of RejectionMemoAttachment objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CgoRejectionMemoVat> LoadEntities(ObjectSet<CgoRejectionMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CgoRejectionMemoVat> link)
    {
      if (link == null)
        link = new Action<CgoRejectionMemoVat>(c => { });

      var rejectionMemoVats = new List<CgoRejectionMemoVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RejectionMemoVat))
      {
        // first result set includes the category
            foreach (var c in
                cargoMaterializers.CgoRejectionMemoVatMaterializer
                .Materialize(reader)
                .Bind(objectSet)
                .ForEach(link)
                )
            {
              rejectionMemoVats.Add(c);
            }
            reader.Close();
      }
      return rejectionMemoVats;
    }

    public static List<CgoRejectionMemoVat> LoadAuditEntities(ObjectSet<CgoRejectionMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CgoRejectionMemoVat> link)
    {
      if (link == null)
        link = new Action<CgoRejectionMemoVat>(c => { });

      var rejectionMemoVatColl = new List<CgoRejectionMemoVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RejectionMemoVat))
      {

        // first result set includes the category
        foreach (var c in
            cargoMaterializers.CgoRejectionMemoVatAuditMaterializer.Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          rejectionMemoVatColl.Add(c);
        }
        reader.Close();
      }

      return rejectionMemoVatColl;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class RMAwbVatRepository
  {
    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<RMAwbVat> LoadEntities(ObjectSet<RMAwbVat> objectSet, LoadStrategyResult loadStrategyResult, Action<RMAwbVat> link)
    {
      if (link == null)
        link = new Action<RMAwbVat>(c => { });

      var rmAwbVats = new List<RMAwbVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RmAwbVat))
      {
        foreach (var c in
            cargoMaterializers.CargoRMAwbVatMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          rmAwbVats.Add(c);
        }
        reader.Close();
      }
      return rmAwbVats;
    }

    public static List<RMAwbVat> LoadAuditEntities(ObjectSet<RMAwbVat> objectSet, LoadStrategyResult loadStrategyResult, Action<RMAwbVat> link)
    {
      if (link == null)
        link = new Action<RMAwbVat>(c => { });

      var rmAwbVats = new List<RMAwbVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RmAwbVat))
      {

        // first result set includes the category
        foreach (var c in
            cargoMaterializers.CargoRMAwbVatAuditMaterializer.Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          rmAwbVats.Add(c);
        }
        reader.Close();
      }

      return rmAwbVats;
    }
  }
}

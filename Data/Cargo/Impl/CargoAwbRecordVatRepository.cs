using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoAwbRecordVatRepository
  {
    /// <summary>
    /// This will load list of AwbVat objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<AwbVat> LoadEntities(ObjectSet<AwbVat> objectSet, LoadStrategyResult loadStrategyResult, Action<AwbVat> link)
    {
      if (link == null)
        link = new Action<AwbVat>(c => { });

      var cgoAwbRecordVat = new List<AwbVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (var reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.AwbRecordVat))
      {
        // first result set includes the category
          cgoAwbRecordVat.AddRange(cargoMaterializers.CargoAwbVatMaterializer.Materialize(reader).Bind(objectSet).ForEach<AwbVat>(link));
        reader.Close();
      }

      return cgoAwbRecordVat;
    }

    public static List<AwbVat> LoadAuditEntities(ObjectSet<AwbVat> objectSet, LoadStrategyResult loadStrategyResult, Action<AwbVat> link)
    {
      if (link == null)
        link = new Action<AwbVat>(c => { });

      List<AwbVat> awbRecordVats = new List<AwbVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.AwbRecordVat))
      {
        // first result set includes the category
        foreach (var c in
            cargoMaterializers.CargoAwbVatMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<AwbVat>(link)
            )
        {
          awbRecordVats.Add(c);
        }
        reader.Close();
      }
      return awbRecordVats;
    }
  }
}

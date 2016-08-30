using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoAwbOtherChargesRepository
  {
    /// <summary>
    /// This will load list of AwbOtherCharge objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<AwbOtherCharge> LoadEntities(ObjectSet<AwbOtherCharge> objectSet, LoadStrategyResult loadStrategyResult, Action<AwbOtherCharge> link)
    {
      if (link == null)
        link = new Action<AwbOtherCharge>(c => { });

      var cgoAwbRecordVat = new List<AwbOtherCharge>();
      var cargoMaterializers = new CargoMaterializers();
      using (var reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.AwbOtherCharge))
      {
        // first result set includes the category
          cgoAwbRecordVat.AddRange(cargoMaterializers.CargoAwbOtherChargeMaterializer.Materialize(reader).Bind(objectSet).ForEach<AwbOtherCharge>(link));
        reader.Close();
      }

      return cgoAwbRecordVat;
    }

  }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class BMAwbOtherChargeRepository
  {
    /// <summary>
    /// This will load list of CargoBillingMemoAwb objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<BMAwbOtherCharge> LoadEntities(ObjectSet<BMAwbOtherCharge> objectSet, LoadStrategyResult loadStrategyResult, Action<BMAwbOtherCharge> link)
    {
      if (link == null)
        link = new Action<BMAwbOtherCharge>(c => { });

      var bmAwbOtherCharges = new List<BMAwbOtherCharge>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.BmAwbOtherCharges))
      {
        foreach (var c in
          cargoMaterializers.BMAwbOtherChargeMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
          )
        {
          bmAwbOtherCharges.Add(c);
        }
        reader.Close();
      }
      return bmAwbOtherCharges;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CMAwbVatRepository
  {
    /// <summary>
    /// This will load list of CargoBillingMemoAwb objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CMAwbVat> LoadEntities(ObjectSet<CMAwbVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CMAwbVat> link)
    {
      if (link == null)
        link = new Action<CMAwbVat>(c => { });

      var cmAwbVats = new List<CMAwbVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CmAwbVat))
      {
        foreach (var c in
          cargoMaterializers.CmAwbVatMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
          )
        {
          cmAwbVats.Add(c);
        }
        reader.Close();
      }
      return cmAwbVats;
    }
  }
}

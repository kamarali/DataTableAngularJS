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
  class BMAwbVatRepository
  {
     /// <summary>
    /// Load the given object set with entities from the Load Strategy Result
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<BMAwbVat> LoadEntities(ObjectSet<BMAwbVat> objectSet, LoadStrategyResult loadStrategyResult, Action<BMAwbVat> link)
    {
      if (link == null)
        link = new Action<BMAwbVat>(c => { });

      var bmAwbVats = new List<BMAwbVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.BmAwbVat))
      {
        foreach (var c in
            cargoMaterializers.BMAwbVatMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          bmAwbVats.Add(c);
        }
        reader.Close();
      }
      return bmAwbVats;
    }

    public static List<BMAwbVat> LoadAuditEntities(ObjectSet<BMAwbVat> objectSet, LoadStrategyResult loadStrategyResult, Action<BMAwbVat> link, string entityName)
    {
      if (link == null)
        link = new Action<BMAwbVat>(c => { });

      var bmAwbVats = new List<BMAwbVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
      {
        // first result set includes the category
          bmAwbVats = cargoMaterializers.CargoBillingMemoAwbVatAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }
      return bmAwbVats;
    }
  }
}

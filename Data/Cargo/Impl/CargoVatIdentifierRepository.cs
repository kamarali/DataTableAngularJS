using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoVatIdentifierRepository
  {
    /// <summary>
    /// This will load list of VatIdentifier objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static List<CgoVatIdentifier> LoadEntities(ObjectSet<CgoVatIdentifier> objectSet, LoadStrategyResult loadStrategyResult, Action<CgoVatIdentifier> link,string entity)
    {
      if (link == null)
        link = new Action<CgoVatIdentifier>(c => { });

      var vatIdentifierColl = new List<CgoVatIdentifier>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(entity))
      {

        // first result set includes the category
        foreach (var c in
            cargoMaterializers.CargoVatIdentifierMaterializer.Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          vatIdentifierColl.Add(c);
        }
        reader.Close();
      }
      return vatIdentifierColl;
    }
  }
}

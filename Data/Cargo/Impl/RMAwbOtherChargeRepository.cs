using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class RMAwbOtherChargeRepository
  {
    /// <summary>
    /// This will load list of CargoBillingMemoAwb objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<RMAwbOtherCharge> LoadEntities(ObjectSet<RMAwbOtherCharge> objectSet, LoadStrategyResult loadStrategyResult, Action<RMAwbOtherCharge> link)
    {
      if (link == null)
        link = new Action<RMAwbOtherCharge>(c => { });

      var rmAwbOtherCharges = new List<RMAwbOtherCharge>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RmAwbOtherCharges))
      {
        foreach (var c in
          cargoMaterializers.RMAwbOtherChargeMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
          )
        {
          rmAwbOtherCharges.Add(c);
        }
        reader.Close();
      }
      return rmAwbOtherCharges;
    }

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<RMAwbOtherCharge> LoadLinkedEntities(LoadStrategyResult loadStrategyResult, Action<RMAwbOtherCharge> link)
    {
      if (link == null)
        link = new Action<RMAwbOtherCharge>(c => { });

      var rmAwbOtherCharges = new List<RMAwbOtherCharge>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(RMAwbRepositoryConstants.AwbOtherChargeParameterName))
      {
        if (reader.HasRows)
        {
          // first result set includes the category
          foreach (var c in
              cargoMaterializers.RMAwbOtherChargeMaterializer
                  .Materialize(reader)
                  .ForEach(link)
              )
          {
            rmAwbOtherCharges.Add(c);
          }
        }
        reader.Close();
      }
      return rmAwbOtherCharges;
    }
  }
}


using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoCreditMemoVatRepository
  {
    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CargoCreditMemoVat> LoadEntities(ObjectSet<CargoCreditMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoCreditMemoVat> link)
    {
      if (link == null)
        link = new Action<CargoCreditMemoVat>(c => { });

      var creditMemoVats = new List<CargoCreditMemoVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CreditMemoVat))
      {
        // first result set includes the category
          creditMemoVats = cargoMaterializers.CargoCreditMemoVatMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        // billingMemoVats= new List<CargoBillingMemoVat>();
        reader.Close();
      }
      return creditMemoVats;
    }

  }
}


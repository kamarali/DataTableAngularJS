using System;
using System.Collections.Generic;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using System.Data.Objects;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoInvoiceTotalRepository
  {
    #region Load strategy

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CargoInvoiceTotal> LoadEntities(ObjectSet<CargoInvoiceTotal> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoInvoiceTotal> link)
    {
      if (link == null)
        link = new Action<CargoInvoiceTotal>(c => { });

      var invoiceTotals = new List<CargoInvoiceTotal>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.InvoiceTotal))
      {
        // first result set includes the category
        foreach (var c in
            cargoMaterializers.CargoInvoiceTotalMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<CargoInvoiceTotal>(link)
            )
        {
          invoiceTotals.Add(c);
        }
        reader.Close();
      }

      return invoiceTotals;
    }

    #endregion
  }
}

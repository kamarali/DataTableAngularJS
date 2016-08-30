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
  public class CargoBillingCodeSubTotalVatRepository : Repository<CargoBillingCodeSubTotalVat>, ICargoBillingCodeSubTotalVatRepository
  {
    /// <summary>
    /// This will load list of CargoBillingCodeSubTotalVat objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CargoBillingCodeSubTotalVat> LoadEntities(ObjectSet<CargoBillingCodeSubTotalVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingCodeSubTotalVat> link)
    {
      if (link == null)
        link = new Action<CargoBillingCodeSubTotalVat>(c => { });

      var cgoBillingCodeSubTotalVat = new List<CargoBillingCodeSubTotalVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (var reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CargoBillingCodeSubTotalVat))
      {
        // first result set includes the category
          cgoBillingCodeSubTotalVat.AddRange(cargoMaterializers.CargoBillingCodeSubTotalVatMaterializer.Materialize(reader).Bind(objectSet).ForEach<CargoBillingCodeSubTotalVat>(link));
        reader.Close();
      }

      return cgoBillingCodeSubTotalVat;
    }
  }
}

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
  public class CargoBillingCodeSubTotalRepository : Repository<BillingCodeSubTotal>, ICargoBillingCodeSubTotalRepository
  {
    /// <summary>
    /// This will load list of BillingCodeSubTotal objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<BillingCodeSubTotal> LoadEntities(ObjectSet<BillingCodeSubTotal> objectSet, LoadStrategyResult loadStrategyResult, Action<BillingCodeSubTotal> link)
    {
      if (link == null)
        link = new Action<BillingCodeSubTotal>(c => { });

      var cgoBillingCodeSubTotal = new List<BillingCodeSubTotal>();
      var cargoMaterializers = new CargoMaterializers();
      using (var reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.BillingCodeSubTotal))
      {
        // first result set includes the category
        cgoBillingCodeSubTotal.AddRange(cargoMaterializers.BillingCodeSubTotalMaterializer.Materialize(reader).Bind(objectSet).ForEach<BillingCodeSubTotal>(link));
        reader.Close();
      }

      //Load CargoBillingCodeSubTotalVat by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CargoBillingCodeSubTotalVat) && cgoBillingCodeSubTotal.Count != 0)
      {
        CargoBillingCodeSubTotalVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoBillingCodeSubTotalVat>(), loadStrategyResult, null);
      }

      return cgoBillingCodeSubTotal;
    }
  }
}

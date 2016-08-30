using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CMAwbOtherChargeRepository
  {
     /// <summary>
    /// This will load list of CargoBillingMemoAwb objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CMAwbOtherCharge> LoadEntities(ObjectSet<CMAwbOtherCharge> objectSet, LoadStrategyResult loadStrategyResult, Action<CMAwbOtherCharge> link)
     {
       if (link == null)
         link = new Action<CMAwbOtherCharge>(c => { });

       var cmAwbOtherCharges = new List<CMAwbOtherCharge>();
       var cargoMaterializers = new CargoMaterializers();
       using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CmAwbOtherCharges))
       {
         foreach (var c in
           cargoMaterializers.CmAwbOtherChargeMaterializer
             .Materialize(reader)
             .Bind(objectSet)
             .ForEach(link)
           )
         {
           cmAwbOtherCharges.Add(c);
         }
         reader.Close();
       }
       return cmAwbOtherCharges;
     }
  }
}

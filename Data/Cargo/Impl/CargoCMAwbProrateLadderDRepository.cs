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
  public class CargoCMAwbProrateLadderDRepository
  {
    public static List<CMAwbProrateLadderDetail> LoadEntities(ObjectSet<CMAwbProrateLadderDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<CMAwbProrateLadderDetail> link)
    {
      if (link == null)
        link = new Action<CMAwbProrateLadderDetail>(c => { });

      var cmAwbProrateLadderDetails = new List<CMAwbProrateLadderDetail>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CmAwbProrateLadder))
      {
        foreach (var c in
            cargoMaterializers.CMAwbProrateLadderDetailMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          cmAwbProrateLadderDetails.Add(c);
        }
        reader.Close();
      }
      return cmAwbProrateLadderDetails;
    }
  }
}

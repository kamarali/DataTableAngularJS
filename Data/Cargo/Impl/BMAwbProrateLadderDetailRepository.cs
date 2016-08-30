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
    public class BMAwbProrateLadderDetailRepository
    { 
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
      public static List<BMAwbProrateLadderDetail> LoadEntities(ObjectSet<BMAwbProrateLadderDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<BMAwbProrateLadderDetail> link)
        {
            if (link == null)
              link = new Action<BMAwbProrateLadderDetail>(c => { });

            var bmAwbProrateLadderDetails = new List<BMAwbProrateLadderDetail>();
            var cargoMaterializers = new CargoMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.BmAwbProrateLadder))
            {
                foreach (var c in
                    cargoMaterializers.BMAwbProrateLadderDetailMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    bmAwbProrateLadderDetails.Add(c);
                }
                reader.Close();
            }
            return bmAwbProrateLadderDetails;
        }
    }
}


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
    public class RMAwbProrateLadderDetailRepository
    { 
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <returns></returns>
        public static List<RMAwbProrateLadderDetail> LoadEntities(ObjectSet<RMAwbProrateLadderDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<RMAwbProrateLadderDetail> link)
        {
            if (link == null)
                link = new Action<RMAwbProrateLadderDetail>(c => { });

            var rmAwbProrateLadderDetails = new List<RMAwbProrateLadderDetail>();
            var cargoMaterializers = new CargoMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RmAwbProrateLadder))
            {
                foreach (var c in
                    cargoMaterializers.RMAwbProrateLadderDetailMaterializer
                    .Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    rmAwbProrateLadderDetails.Add(c);
                }
                reader.Close();
            }
            return rmAwbProrateLadderDetails;
        }
    }
}


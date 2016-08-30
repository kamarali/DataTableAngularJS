using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class LineItemTaxAdditionalDetailRepository
    {
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<LineItemTaxAdditionalDetail> LoadEntities(ObjectSet<LineItemTaxAdditionalDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<LineItemTaxAdditionalDetail> link)
        {
            if (link == null)
                link = new Action<LineItemTaxAdditionalDetail>(c => { });

            var lineItemTaxAdditionalDetails = new List<LineItemTaxAdditionalDetail>();

            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.LineItemTaxAdditionalDetails))
            {
                // first result set includes the category
                lineItemTaxAdditionalDetails = muMaterializers.LineItemTaxAdditionalDetailMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            return lineItemTaxAdditionalDetails;
        }
    }
}

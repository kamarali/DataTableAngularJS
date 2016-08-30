using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class CreditMemoVatRepository
    {
        /// <summary>
        /// This will load list of CreditMemoVat objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<CreditMemoVat> LoadEntities(ObjectSet<CreditMemoVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CreditMemoVat> link)
        {
            if (link == null)
                link = new Action<CreditMemoVat>(c => { });

            var creditMemoVats = new List<CreditMemoVat>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CreditMemoVat))
            {

                // first result set includes the category
                foreach (var c in
                    new PaxMaterializers().CreditMemoVatMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    creditMemoVats.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load VatIdentifier
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemoVatIdentifier) && creditMemoVats.Count != 0)
            {
                VatIdentifierRepository.LoadEntities(objectSet.Context.CreateObjectSet<VatIdentifier>(), loadStrategyResult, null, LoadStrategy.Entities.CreditMemoVatIdentifier);
            }

            return creditMemoVats;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class InvoiceSummaryRepository
  {
    /// <summary>
    /// This will load list of InvoiceSummary objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<InvoiceSummary> LoadEntities(ObjectSet<InvoiceSummary> objectSet, LoadStrategyResult loadStrategyResult, Action<InvoiceSummary> link)
    {
      if (link == null)
        link = new Action<InvoiceSummary>(c => { });

      var invoiceSummaries = new List<InvoiceSummary>();

      var muMaterializers = new MuMaterializers();
        using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.InvoiceSummary))
      {

        // first result set includes the category
        foreach (var c in
            muMaterializers.MiscUatpInvoiceSummaryMaterializer.Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          invoiceSummaries.Add(c);
        }
        reader.Close();
      }

      return invoiceSummaries;
    }
  }
}

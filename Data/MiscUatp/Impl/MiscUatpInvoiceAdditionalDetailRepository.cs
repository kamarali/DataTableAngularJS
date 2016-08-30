using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class MiscUatpInvoiceAdditionalDetailRepository
  {
    /// <summary>
    /// This will load list of MiscUatpInvoiceAdditionalDetail objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<MiscUatpInvoiceAdditionalDetail> LoadEntities(ObjectSet<MiscUatpInvoiceAdditionalDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<MiscUatpInvoiceAdditionalDetail> link)
    {
      if (link == null)
        link = new Action<MiscUatpInvoiceAdditionalDetail>(c => { });

      var miscUatpInvoiceAdditionalDetails = new List<MiscUatpInvoiceAdditionalDetail>();

      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail))
      {
        // first result set includes the category
        foreach (var c in
            muMaterializers.MiscUatpInvoiceAdditionalDetailMaterializer.Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          miscUatpInvoiceAdditionalDetails.Add(c);
        }
        reader.Close();
      }

      return miscUatpInvoiceAdditionalDetails;
    }
  }
}

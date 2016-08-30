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
  public class MiscUatpInvoiceTaxAdditionalDetailRepository
  {
    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<MiscUatpInvoiceTaxAdditionalDetail> LoadEntities(ObjectSet<MiscUatpInvoiceTaxAdditionalDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<MiscUatpInvoiceTaxAdditionalDetail> link)
    {
      if (link == null)
        link = new Action<MiscUatpInvoiceTaxAdditionalDetail>(c => { });

      var miscUatpInvoiceTaxAdditionalDetails = new List<MiscUatpInvoiceTaxAdditionalDetail>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.MiscInvoiceTaxAdditionalDetail))
      {
        // first result set includes the category
        miscUatpInvoiceTaxAdditionalDetails = muMaterializers.MiscUatpInvoiceTaxAdditionalDetailMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }
      return miscUatpInvoiceTaxAdditionalDetails;
    }
  }
}

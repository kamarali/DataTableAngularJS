using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class InvoiceAddOnChargeRepository : Repository<InvoiceAddOnCharge>
  {
    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<InvoiceAddOnCharge> LoadEntities(ObjectSet<InvoiceAddOnCharge> objectSet, LoadStrategyResult loadStrategyResult, Action<InvoiceAddOnCharge> link)
    {
      if (link == null)
        link = new Action<InvoiceAddOnCharge>(c => { });
      var muMaterializers = new MuMaterializers();
      var addOnCharges = new List<InvoiceAddOnCharge>();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.MiscInvoiceAddOnCharge))
      {
        // first result set includes the category
        addOnCharges = muMaterializers.InvoiceAddOnChargeMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }
      return addOnCharges;
    }
  }
}


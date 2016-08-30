
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
    public class CGOBillingCodeSubTotalVatRepository : Repository<CgoBillingCodeSubTotalVat>, ICGOBillingCodeSubTotalVatRepository
  {
    /// <summary>
    /// This will load list of CargoBillingCodeSubTotalVat objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
        public static List<CgoBillingCodeSubTotalVat> LoadEntities(ObjectSet<CgoBillingCodeSubTotalVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CgoBillingCodeSubTotalVat> link)
    {
      if (link == null)
          link = new Action<CgoBillingCodeSubTotalVat>(c => { });

      var cgoBillingCodeSubTotalVat = new List<CgoBillingCodeSubTotalVat>();
      //var cargoMaterializers = new CargoMaterializers();
      //using (var reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CargoBillingCodeSubTotalVat))
      //{
      //  // first result set includes the category
      //   cgoBillingCodeSubTotalVat.AddRange(cargoMaterializers.CargoBillingCodeSubTotalVatMaterializer.Materialize(reader).Bind(objectSet).ForEach<CgoBillingCodeSubTotalVat>(link));
      //  reader.Close();
      //}

      return cgoBillingCodeSubTotalVat;
    }
        public List<CgoBillingCodeSubTotalVat> GetBillingCodeVatTotals(Guid invoiceId)
    {
        var parameters = new ObjectParameter[1];
        parameters[0] = new ObjectParameter(CargoInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
        //ExecuteStoredProcedure(CargoInvoiceRepositoryConstants.UpdateBillingCodeVatFunctionName, parameters);
        var searchResult = ExecuteStoredFunction<CgoBillingCodeSubTotalVat>(CargoInvoiceRepositoryConstants.GetCargoBillingCodeVatTotalFunctionName, parameters);
        return searchResult.ToList();
    }

  }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;
using System.Linq;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoInvoiceTotalVatRepository : Repository<CargoInvoiceTotalVat>, ICargoInvoiceTotalVatRepository
  {
    public override IQueryable<CargoInvoiceTotalVat> Get(System.Linq.Expressions.Expression<Func<CargoInvoiceTotalVat, bool>> where)
    {
      var InvoiceTotalVatList = EntityObjectSet
                                             .Include("VatIdentifier").Where(where);

      return InvoiceTotalVatList;
    }
    #region Load strategy

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CargoInvoiceTotalVat> LoadEntities(ObjectSet<CargoInvoiceTotalVat> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoInvoiceTotalVat> link)
    {
      if (link == null)
        link = new Action<CargoInvoiceTotalVat>(c => { });

      var invoiceVats = new List<CargoInvoiceTotalVat>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.InvoiceTotalVat))
      {
        // first result set includes the category
        foreach (var c in
            cargoMaterializers.CargoInvoiceTotalVatMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<CargoInvoiceTotalVat>(link)
            )
        {
          invoiceVats.Add(c);
        }
        reader.Close();
      }
      return invoiceVats;
    }

    #endregion
  }
}

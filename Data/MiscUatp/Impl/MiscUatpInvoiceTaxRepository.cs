using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class MiscUatpInvoiceTaxRepository
  {
    /// <summary>
    /// This will load list of MiscUatpInvoiceTax objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<MiscUatpInvoiceTax> LoadEntities(ObjectSet<MiscUatpInvoiceTax> objectSet, LoadStrategyResult loadStrategyResult, Action<MiscUatpInvoiceTax> link)
    {
      if (link == null)
        link = new Action<MiscUatpInvoiceTax>(c => { });

      var miscUatpInvoiceTaxColl = new List<MiscUatpInvoiceTax>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.MiscTaxBreakdown))
      {

        // first result set includes the category
        foreach (var c in
            muMaterializers.MiscUatpInvoiceTaxMaterializer.Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          miscUatpInvoiceTaxColl.Add(c);
        }
        reader.Close();
      }

      //Load MiscInvoiceTaxAdditionalDetail
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.MiscInvoiceTaxAdditionalDetail) && miscUatpInvoiceTaxColl.Count != 0)
      {
        MiscUatpInvoiceTaxAdditionalDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<MiscUatpInvoiceTaxAdditionalDetail>(), loadStrategyResult, miscUatpInvoiceTaxAdditionalDetail => miscUatpInvoiceTaxAdditionalDetail.MiscUatpInvoiceTax = miscUatpInvoiceTaxColl.Find(i => i.Id == miscUatpInvoiceTaxAdditionalDetail.MiscInvoiceTaxId));
      }

      //Load MiscTaxBreakdownCountry
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.MiscTaxBreakdownCountry) && miscUatpInvoiceTaxColl.Count != 0)
      {
        CountryRepository.LoadEntities(objectSet.Context.CreateObjectSet<Country>(), loadStrategyResult, null, LoadStrategy.MiscEntities.MiscTaxBreakdownCountry);
      }

      return miscUatpInvoiceTaxColl;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class PaymentDetailRepository
  {
    /// <summary>
    /// This will load list of PaymentDetail objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<PaymentDetail> LoadEntities(ObjectSet<PaymentDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<PaymentDetail> link)
    {
      if (link == null)
        link = new Action<PaymentDetail>(c => { });

      var paymentDetails = new List<PaymentDetail>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.PaymentDetail))
      {

        // first result set includes the category
        foreach (var c in
            muMaterializers.PaymentDetailMaterializer.Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          paymentDetails.Add(c);
        }
        reader.Close();
      }

      return paymentDetails;
    }
  }
}

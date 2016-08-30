using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class OtherOrganizationAdditionalDetailRepository
  {
    /// <summary>
    /// This will load list of OtherOrganizationAdditionalDetail objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<OtherOrganizationAdditionalDetail> LoadEntities(ObjectSet<OtherOrganizationAdditionalDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<OtherOrganizationAdditionalDetail> link)
    {
      if (link == null)
        link = new Action<OtherOrganizationAdditionalDetail>(c => { });

      var otherOrganizationAdditionalDetailColl = new List<OtherOrganizationAdditionalDetail>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.OtherOrganizationAdditionalDetails))
      {

        // first result set includes the category
        foreach (var c in
            muMaterializers.OtherOrganizationAdditionalDetailMaterializer.Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          otherOrganizationAdditionalDetailColl.Add(c);
        }
        reader.Close();
      }

      return otherOrganizationAdditionalDetailColl;
    }
  }
}

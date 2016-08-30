using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class OtherOrganizationContactRepository
  {
    /// <summary>
    /// This will load list of OtherOrganizationContact objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<OtherOrganizationContact> LoadEntities(ObjectSet<OtherOrganizationContact> objectSet, LoadStrategyResult loadStrategyResult, Action<OtherOrganizationContact> link)
    {
      if (link == null)
        link = new Action<OtherOrganizationContact>(c => { });

      var otherOrganizationContactColl = new List<OtherOrganizationContact>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.OtherOrganizationContactInformations))
      {

        // first result set includes the category
        foreach (var c in
            muMaterializers.OtherOrganizationContactMaterializer.Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
            )
        {
          otherOrganizationContactColl.Add(c);
        }
        reader.Close();
      }

      return otherOrganizationContactColl;
    }
  }
}

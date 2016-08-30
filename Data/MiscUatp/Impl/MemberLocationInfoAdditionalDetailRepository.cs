using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class MemberLocationInfoAdditionalDetailRepository : Repository<MemberLocationInfoAdditionalDetail>, IMemberLocationInfoAdditionalDetailRepository
  {
    #region Load strategy

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<MemberLocationInfoAdditionalDetail> LoadEntities(ObjectSet<MemberLocationInfoAdditionalDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<MemberLocationInfoAdditionalDetail> link)
    {
      if (link == null)
        link = new Action<MemberLocationInfoAdditionalDetail>(c => { });

      List<MemberLocationInfoAdditionalDetail> memberLocationInfoAdditionalDetails = new List<MemberLocationInfoAdditionalDetail>();
      var muMaterializer = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.MemberLocationInfoAddDetail))
      {
        // first result set includes the category
        foreach (var c in
            muMaterializer.MemberLocationInfoAdditionDetailMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<MemberLocationInfoAdditionalDetail>(link)
            )
        {
          memberLocationInfoAdditionalDetails.Add(c);
        }
        reader.Close();
      }

      return memberLocationInfoAdditionalDetails;
    }

    #endregion
  }
}
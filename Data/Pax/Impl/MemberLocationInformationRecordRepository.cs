using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.MiscUatp.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
  public class MemberLocationInformationRecordRepository : Repository<MemberLocationInformation>, IMemberLocationInformationRepository
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
    public static List<MemberLocationInformation> LoadEntities(ObjectSet<MemberLocationInformation> objectSet, LoadStrategyResult loadStrategyResult, Action<MemberLocationInformation> link)
    {
      if (link == null)
        link = new Action<MemberLocationInformation>(c => { });

      List<MemberLocationInformation> memberLocationInformations = new List<MemberLocationInformation>();
      var commonMaterializers = new CommonMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.MemberLocation))
      {
        // first result set includes the category
        foreach (var c in
            commonMaterializers.MemberLocationInformationMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<MemberLocationInformation>(link)
            )
        {
          memberLocationInformations.Add(c);
        }
        reader.Close();
      }

      //Load ChargeCode by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.MemberLocationInfoAddDetail) && memberLocationInformations.Count != 0)
      {
        MemberLocationInfoAdditionalDetailRepository.LoadEntities(
          objectSet.Context.CreateObjectSet<MemberLocationInfoAdditionalDetail>(), loadStrategyResult, null);
      }

      return memberLocationInformations;
    }

    #endregion
  }
}

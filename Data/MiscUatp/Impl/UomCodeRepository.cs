using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class UomCodeRepository 
  {
    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <param name="entityName"></param>
    /// <returns></returns>
    public static List<UomCode> LoadEntities(ObjectSet<UomCode> objectSet, LoadStrategyResult loadStrategyResult, Action<UomCode> link, string entityName)
    {
      if (link == null)
        link = new Action<UomCode>(c => { });

      List<UomCode> uomCodes;
      var muMaterializers = new MuMaterializers();
      using (var reader = loadStrategyResult.GetReader(entityName))
      {
        // first result set includes the category
        uomCodes = muMaterializers.UomCodeMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }

      return uomCodes;
    }
  }
}

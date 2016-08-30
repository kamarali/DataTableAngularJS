using System;
using System.Collections.Generic;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using System.Data.Objects;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CMAwbAttachmentRepository : Repository<CMAwbAttachment>, ICargoCreditMemoAwbAttachmentRepository
  {
    /// <summary>
    /// This will load list of CargoBillingMemoAwb objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CMAwbAttachment> LoadEntities(ObjectSet<CMAwbAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<CMAwbAttachment> link)
    {
      if (link == null)
        link = new Action<CMAwbAttachment>(c => { });

      var cmAwbAttachments = new List<CMAwbAttachment>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CmAwbAttachments))
      {
        foreach (var c in
          cargoMaterializers.CmAwbAttachmentMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
          )
        {
          cmAwbAttachments.Add(c);
        }
        reader.Close();
      }
      return cmAwbAttachments;
    }

    public override CMAwbAttachment Single(System.Linq.Expressions.Expression<Func<CMAwbAttachment, bool>> where)
    {
      var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
      return attachmentRecord;
    }

    // <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public IQueryable<CMAwbAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CMAwbAttachment, bool>> where)
    {
        var attachmentRecords = EntityObjectSet.Include("UploadedBy").Where(where);
        return attachmentRecords;
    }
  }
}

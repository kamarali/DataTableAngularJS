using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class RMAwbAttachmentRepository : Repository<RMAwbAttachment>, IRMAwbAttachmentRepository
  {
    public override RMAwbAttachment Single(System.Linq.Expressions.Expression<Func<RMAwbAttachment, bool>> where)
    {
      var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
      return attachmentRecord;
    }

    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public IQueryable<RMAwbAttachment> GetDetail(System.Linq.Expressions.Expression<Func<RMAwbAttachment, bool>> where)
    {
      var attachmentRecords = EntityObjectSet.Include("UploadedBy").Where(where);
      return attachmentRecords;
    }

    /// <summary>
    /// This will load list of RMCouponAttachment objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<RMAwbAttachment> LoadEntities(ObjectSet<RMAwbAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<RMAwbAttachment> link)
    {
      if (link == null)
        link = new Action<RMAwbAttachment>(c => { });

      var rmAwbAttachments = new List<RMAwbAttachment>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RmAwbAttachments))
      {
        foreach (var c in
          cargoMaterializers.RMAwbAttachmentMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
          )
        {
          rmAwbAttachments.Add(c);
        }
        reader.Close();
      }

      // Load Rejection Memo AWB Attachment uploaded by user details
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && rmAwbAttachments.Count > 0)
      {
        UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
               , loadStrategyResult
               , null, LoadStrategy.Entities.AttachmentUploadedbyUser);
      }

      return rmAwbAttachments;
    }

    public static List<RMAwbAttachment> LoadAuditEntities(ObjectSet<RMAwbAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<RMAwbAttachment> link)
    {
      if (link == null) link = new Action<RMAwbAttachment>(c => { });

      var attachments = new List<RMAwbAttachment>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RmAwbAttachments))
      {

        // first result set includes the category
        foreach (var c in
          cargoMaterializers.RMAwbAttachmentAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link))
        {
          attachments.Add(c);
        }
        reader.Close();
      }
      return attachments;
    }
  }
}

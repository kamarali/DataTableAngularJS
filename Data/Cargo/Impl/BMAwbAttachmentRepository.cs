using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo.Impl
{
  public class BMAwbAttachmentRepository : Repository<BMAwbAttachment>, IBMAwbAttachmentRepository
  {

    public override BMAwbAttachment Single(System.Linq.Expressions.Expression<Func<BMAwbAttachment, bool>> where)
    {
        var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
        return attachmentRecord;
    }

    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public IQueryable<BMAwbAttachment> GetDetail(System.Linq.Expressions.Expression<Func<BMAwbAttachment, bool>> where)
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
    public static List<BMAwbAttachment> LoadEntities(ObjectSet<BMAwbAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<BMAwbAttachment> link)
    {
      if (link == null)
        link = new Action<BMAwbAttachment>(c => { });

      var bmAwbAttachments = new List<BMAwbAttachment>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.BmAwbAttachments))
      {
        foreach (var c in
          cargoMaterializers.BMAwbAttachmentMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach(link)
          )
        {
          bmAwbAttachments.Add(c);
        }
        reader.Close();
      }
      //Load Billing memo Attachment uploaded by user details
      if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BMAttachmentUploadedByUser) && bmAwbAttachments.Count > 0)
      {
          UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                 , loadStrategyResult
                 , null, LoadStrategy.CargoEntities.BMAttachmentUploadedByUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
      }
      return bmAwbAttachments;
    }

    public static List<BMAwbAttachment> LoadAuditEntities(ObjectSet<BMAwbAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<BMAwbAttachment> link, string entityName)
    {
      if (link == null)
        link = new Action<BMAwbAttachment>(c => { });

      var bmAwbAttachments = new List<BMAwbAttachment>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
      {
        // first result set includes the category
          bmAwbAttachments = cargoMaterializers.CargoBillingMemoAwbAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }
      return bmAwbAttachments;
    }

  }
}

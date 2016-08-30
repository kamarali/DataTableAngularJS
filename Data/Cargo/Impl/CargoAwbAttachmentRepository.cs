using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoAwbAttachmentRepository : Repository<AwbAttachment>, ICargoAwbAttachmentRepository
  {
    public override AwbAttachment Single(System.Linq.Expressions.Expression<Func<AwbAttachment, bool>> where)
    {
      var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
      return attachmentRecord;
    }

    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public IQueryable<AwbAttachment> GetDetail(System.Linq.Expressions.Expression<Func<AwbAttachment, bool>> where)
    {
      var attachmentRecords = EntityObjectSet.Include("UploadedBy").Where(where);
      return attachmentRecords;
    }

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <param name="entityName"></param>
    /// <returns></returns>
    public static List<AwbAttachment> LoadEntities(ObjectSet<AwbAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<AwbAttachment> link, string entityName)
    {
      if (link == null)
        link = new Action<AwbAttachment>(c => { });

      var awbAttachments = new List<AwbAttachment>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
      {
        // first result set includes the category
          awbAttachments = cargoMaterializers.CargoAwbAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }

      //Load Billing memo Attachment uploaded by user details
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && awbAttachments.Count > 0)
      {
        UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
               , loadStrategyResult
               , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
      }

      return awbAttachments;
    }

    public static List<AwbAttachment> LoadAuditEntities(ObjectSet<AwbAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<AwbAttachment> link)
    {
      if (link == null)
        link = new Action<AwbAttachment>(c => { });

      var awbAttachments = new List<AwbAttachment>();
      var cargoMaterializers = new CargoMaterializers();

      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.AwbAttachment))
      {
        // first result set includes the category
        awbAttachments = cargoMaterializers.AwbAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }

      return awbAttachments;
    }

    //public static List<CargoBillingMemoAttachment> LoadAuditEntities(ObjectSet<CargoBillingMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingMemoAttachment> link, string entityName)
    //{
    //    if (link == null)
    //        link = new Action<CargoBillingMemoAttachment>(c => { });

    //    var billingMemoAttachments = new List<CargoBillingMemoAttachment>();

    //    using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
    //    {
    //        // first result set includes the category
    //        billingMemoAttachments = Materializers.BillingMemoAttachmentAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
    //        reader.Close();
    //    }
    //    return billingMemoAttachments;
    //}


  }
}

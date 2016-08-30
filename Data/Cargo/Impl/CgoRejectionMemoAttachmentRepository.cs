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
  public class CgoRejectionMemoAttachmentRepository : Repository<CgoRejectionMemoAttachment>, ICgoRejectionMemoAttachmentRepository
  {
    public override CgoRejectionMemoAttachment Single(System.Linq.Expressions.Expression<Func<CgoRejectionMemoAttachment, bool>> where)
    {
      var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
      return attachmentRecord;
    }

    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public IQueryable<CgoRejectionMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CgoRejectionMemoAttachment, bool>> where)
    {
      var attachmentRecords = EntityObjectSet.Include("UploadedBy").Where(where);
      return attachmentRecords;
    }

    /// <summary>
    /// This will load list of RejectionMemoAttachment objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CgoRejectionMemoAttachment> LoadEntities(ObjectSet<CgoRejectionMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<CgoRejectionMemoAttachment> link)
    {
      if (link == null)
        link = new Action<CgoRejectionMemoAttachment>(c => { });

      var rejectionMemoAttachments = new List<CgoRejectionMemoAttachment>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RejectionMemoAttachments))
      {
        // first result set includes the category
          rejectionMemoAttachments = cargoMaterializers.CgoRejectionMemoAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        //billingMemoAttachments=new List<CargoBillingMemoAttachment>();
        reader.Close();
      }

      //Load Billing memo Attachment uploaded by user details
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && rejectionMemoAttachments.Count > 0)
      {
        UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
               , loadStrategyResult
               , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
      }

      return rejectionMemoAttachments;

    }

    public static List<CgoRejectionMemoAttachment> LoadAuditEntities(ObjectSet<CgoRejectionMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<CgoRejectionMemoAttachment> link)
    {
      if (link == null) link = new Action<CgoRejectionMemoAttachment>(c => { });

      var attachments = new List<CgoRejectionMemoAttachment>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.RejectionMemoAttachments))
      {

        // first result set includes the category
        foreach (var c in
          cargoMaterializers.CargoRejectionMemoAuditAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link))
        {
          attachments.Add(c);
        }
        reader.Close();
      }
      return attachments;
    }
  }
}

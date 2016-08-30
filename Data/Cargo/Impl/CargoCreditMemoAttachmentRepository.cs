using System;
using System.Collections.Generic;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using System.Data.Objects;
using Iata.IS.Model.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoCreditMemoAttachmentRepository : Repository<CargoCreditMemoAttachment>, ICargoCreditMemoAttachmentRepository
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
    public static List<CargoCreditMemoAttachment> LoadEntities(ObjectSet<CargoCreditMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoCreditMemoAttachment> link)
    {
      if (link == null)
        link = new Action<CargoCreditMemoAttachment>(c => { });

      var creditMemoAttachments = new List<CargoCreditMemoAttachment>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CreditMemoAttachments))
      {
        // first result set includes the category
          creditMemoAttachments = cargoMaterializers.CargoCreditMemoAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        //billingMemoAttachments=new List<CargoBillingMemoAttachment>();
        reader.Close();
      }

      //Load Billing memo Attachment uploaded by user details
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && creditMemoAttachments.Count > 0)
      {
        UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
               , loadStrategyResult
               , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
      }

      return creditMemoAttachments;
    }

    public override CargoCreditMemoAttachment Single(System.Linq.Expressions.Expression<Func<CargoCreditMemoAttachment, bool>> where)
    {
        var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
        return attachmentRecord;
    }

    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public IQueryable<CargoCreditMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CargoCreditMemoAttachment, bool>> where)
    {
        var attachmentRecords = EntityObjectSet.Include("UploadedBy").Where(where);
        return attachmentRecords;
    }
  }
}

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
    public class CargoBillingMemoAttachmentRepository : Repository<CargoBillingMemoAttachment>, ICargoBillingMemoAttachmentRepository
    {
        public override CargoBillingMemoAttachment Single(System.Linq.Expressions.Expression<Func<CargoBillingMemoAttachment, bool>> where)
        {
            var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
            return attachmentRecord;
        }

        /// <summary>
        /// Added to display records in supporting document 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<CargoBillingMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CargoBillingMemoAttachment, bool>> where)
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
        public static List<CargoBillingMemoAttachment> LoadEntities(ObjectSet<CargoBillingMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingMemoAttachment> link, string entityName)
        {
            if (link == null)
                link = new Action<CargoBillingMemoAttachment>(c => { });

            var billingMemoAttachments = new List<CargoBillingMemoAttachment>();
            var cargoMaterializers = new CargoMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                billingMemoAttachments = cargoMaterializers.CargoBillingMemoAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                //billingMemoAttachments=new List<CargoBillingMemoAttachment>();
                reader.Close();
            }

            //Load Billing memo Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.BMAttachmentUploadedByUser) && billingMemoAttachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.CargoEntities.BMAttachmentUploadedByUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return billingMemoAttachments;
        }

        public static List<CargoBillingMemoAttachment> LoadAuditEntities(ObjectSet<CargoBillingMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoBillingMemoAttachment> link, string entityName)
        {
          if (link == null)
            link = new Action<CargoBillingMemoAttachment>(c => { });

          var billingMemoAttachments = new List<CargoBillingMemoAttachment>();
          var cargoMaterializers = new CargoMaterializers();
          using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
          {
            // first result set includes the category
              billingMemoAttachments = cargoMaterializers.CargoBillingMemoAttachmentAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
            reader.Close();
          }
          return billingMemoAttachments;
        }
    }
}

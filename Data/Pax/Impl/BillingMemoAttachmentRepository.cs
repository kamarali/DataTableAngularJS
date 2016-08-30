using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class BillingMemoAttachmentRepository : Repository<BillingMemoAttachment>, IBillingMemoAttachmentRepository
    {
        public override BillingMemoAttachment Single(System.Linq.Expressions.Expression<Func<BillingMemoAttachment, bool>> where)
        {
            var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
            return attachmentRecord;
        }

        /// <summary>
        /// Added to display records in supporting document 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<BillingMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<BillingMemoAttachment, bool>> where)
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
        public static List<BillingMemoAttachment> LoadEntities(ObjectSet<BillingMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<BillingMemoAttachment> link, string entityName)
        {
            if (link == null)
                link = new Action<BillingMemoAttachment>(c => { });

            var billingMemoAttachments = new List<BillingMemoAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                billingMemoAttachments = new PaxMaterializers().BillingMemoAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load Billing memo Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && billingMemoAttachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return billingMemoAttachments;
        }

        public static List<BillingMemoAttachment> LoadAuditEntities(ObjectSet<BillingMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<BillingMemoAttachment> link, string entityName)
        {
            if (link == null)
                link = new Action<BillingMemoAttachment>(c => { });

            var billingMemoAttachments = new List<BillingMemoAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                billingMemoAttachments = new PaxMaterializers().BillingMemoAttachmentAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            return billingMemoAttachments;
        }
    }
}

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
    public class RejectionMemoAttachmentRepository : Repository<RejectionMemoAttachment>, IRejectionMemoAttachmentRepository
    {
        public override RejectionMemoAttachment Single(System.Linq.Expressions.Expression<Func<RejectionMemoAttachment, bool>> where)
        {
            var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
            return attachmentRecord;
        }

        /// <summary>
        /// Added to display records in supporting document 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<RejectionMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<RejectionMemoAttachment, bool>> where)
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
        public static List<RejectionMemoAttachment> LoadEntities(ObjectSet<RejectionMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<RejectionMemoAttachment> link)
        {
            if (link == null) link = new Action<RejectionMemoAttachment>(c => { });

            var attachments = new List<RejectionMemoAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemoAttachments))
            {

                // first result set includes the category
                foreach (var c in
                  new PaxMaterializers().RejectionMemoAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link))
                {
                    attachments.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load Rejection memo Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && attachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return attachments;
        }

        public static List<RejectionMemoAttachment> LoadAuditEntities(ObjectSet<RejectionMemoAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<RejectionMemoAttachment> link)
        {
            if (link == null) link = new Action<RejectionMemoAttachment>(c => { });

            var attachments = new List<RejectionMemoAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.RMAttachment))
            {

                // first result set includes the category
                foreach (var c in
                  new PaxMaterializers().RejectionMemoAttachmentAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link))
                {
                    attachments.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return attachments;
        }
    }
}

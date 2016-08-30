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
    public class RejectionMemoCouponAttachmentRepository : Repository<RMCouponAttachment>, IRejectionMemoCouponAttachmentRepository
    {
        public override RMCouponAttachment Single(System.Linq.Expressions.Expression<Func<RMCouponAttachment, bool>> where)
        {
            var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
            return attachmentRecord;
        }

        /// <summary>
        /// Added to display records in supporting document 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<RMCouponAttachment> GetDetail(System.Linq.Expressions.Expression<Func<RMCouponAttachment, bool>> where)
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
        public static List<RMCouponAttachment> LoadEntities(ObjectSet<RMCouponAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<RMCouponAttachment> link)
        {
            if (link == null) link = new Action<RMCouponAttachment>(c => { });

            var attachments = new List<RMCouponAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.RejectionMemoCouponAttachments))
            {

                // first result set includes the category
                foreach (var c in
                  new PaxMaterializers().RMCouponAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link))
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

        public static List<RMCouponAttachment> LoadAuditEntities(ObjectSet<RMCouponAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<RMCouponAttachment> link)
        {
            if (link == null) link = new Action<RMCouponAttachment>(c => { });

            var attachments = new List<RMCouponAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.RMCouponAttachment))
            {

                // first result set includes the category
                foreach (var c in
                  new PaxMaterializers().RMCouponAttachmentAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link))
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

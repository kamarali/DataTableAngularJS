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
    public class CreditMemoCouponAttachmentRepository : Repository<CMCouponAttachment>, ICreditMemoCouponAttachmentRepository
    {
        public override CMCouponAttachment Single(System.Linq.Expressions.Expression<Func<CMCouponAttachment, bool>> where)
        {
            var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
            return attachmentRecord;
        }

        /// <summary>
        /// Added to display records in supporting document 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<CMCouponAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CMCouponAttachment, bool>> where)
        {
            var attachmentRecords = EntityObjectSet.Include("UploadedBy").Where(where);
            return attachmentRecords;
        }

        #region LoadStrategy

        /// <summary>
        /// This will load list of CMCouponAttachment objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<CMCouponAttachment> LoadEntities(ObjectSet<CMCouponAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<CMCouponAttachment> link)
        {
            if (link == null) link = new Action<CMCouponAttachment>(c => { });

            var attachments = new List<CMCouponAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CreditMemoCouponAttachments))
            {

                // first result set includes the category
                foreach (var c in
                  new PaxMaterializers().CMCouponAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link))
                {
                    attachments.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load Credit memo Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && attachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return attachments;
        }

        #endregion
    }
}

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
    class PaxCorrespondenceAttachmentRepository : Repository<CorrespondenceAttachment>, IPaxCorrespondenceAttachmentRepository
    {
        public override CorrespondenceAttachment Single(System.Linq.Expressions.Expression<Func<CorrespondenceAttachment, bool>> where)
        {
            var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
            return attachmentRecord;
        }
        /// <summary>
        /// This will load list of MiscUatpCorrespondenceAttachment objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<CorrespondenceAttachment> LoadEntities(ObjectSet<CorrespondenceAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<CorrespondenceAttachment> link)
        {
            if (link == null)
                link = new Action<CorrespondenceAttachment>(c => { });

            var correspondenceAttachments = new List<CorrespondenceAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CorrespondenceAttachment))
            {

                correspondenceAttachments = new PaxMaterializers().CorrespondenceAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load Correspondence Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && correspondenceAttachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return correspondenceAttachments;
        }

    }
}

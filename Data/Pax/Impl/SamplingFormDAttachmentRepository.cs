using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class SamplingFormDAttachmentRepository : Repository<SamplingFormDRecordAttachment>, ISamplingFormDAttachmentRepository
    {
        public override SamplingFormDRecordAttachment Single(System.Linq.Expressions.Expression<Func<SamplingFormDRecordAttachment, bool>> where)
        {
            var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
            return attachmentRecord;
        }

        /// <summary>
        /// Added to display records in supporting document 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<SamplingFormDRecordAttachment> GetDetail(System.Linq.Expressions.Expression<Func<SamplingFormDRecordAttachment, bool>> where)
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
        /// <returns>list of SamplingFormDRecordAttachment objects</returns>
        public static List<SamplingFormDRecordAttachment> LoadEntities(ObjectSet<SamplingFormDRecordAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormDRecordAttachment> link)
        {
            if (link == null)
                link = new Action<SamplingFormDRecordAttachment>(c => { });

            var samplingFormDRecordAttachments = new List<SamplingFormDRecordAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormDAttachment))
            {
                samplingFormDRecordAttachments = new PaxMaterializers().SamplingFormDRecordAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load Credit memo Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && samplingFormDRecordAttachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return samplingFormDRecordAttachments;
        }

    }
}
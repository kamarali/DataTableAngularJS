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
    public class SamplingFormCAttachmentRepository : Repository<SamplingFormCRecordAttachment>, ISamplingFormCAttachmentRepository
    {
        public override SamplingFormCRecordAttachment Single(System.Linq.Expressions.Expression<Func<SamplingFormCRecordAttachment, bool>> where)
        {
            var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
            return attachmentRecord;
        }

        /// <summary>
        /// Added to display records in supporting document 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<SamplingFormCRecordAttachment> GetDetail(System.Linq.Expressions.Expression<Func<SamplingFormCRecordAttachment, bool>> where)
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
        /// <returns></returns>
        public static List<SamplingFormCRecordAttachment> LoadEntities(ObjectSet<SamplingFormCRecordAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormCRecordAttachment> link)
        {
            if (link == null)
                link = new Action<SamplingFormCRecordAttachment>(c => { });

            var samplingFormCRecordAttachments = new List<SamplingFormCRecordAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormCRecordAttachment))
            {
                foreach (var c in
                   new PaxMaterializers().SamplingFormCRecordAttachmentMaterializer
                   .Materialize(reader)
                   .Bind(objectSet)
                   .ForEach<SamplingFormCRecordAttachment>(link)
                   )
                {
                    samplingFormCRecordAttachments.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load Form C Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && samplingFormCRecordAttachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return samplingFormCRecordAttachments;
        }
    }
}

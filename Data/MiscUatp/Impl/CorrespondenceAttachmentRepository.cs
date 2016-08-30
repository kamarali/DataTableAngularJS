using System;
using System.Collections.Generic;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    class MiscUatpCorrespondenceAttachmentRepository
    {
        /// <summary>
        /// This will load list of MiscUatpCorrespondenceAttachment objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<MiscUatpCorrespondenceAttachment> LoadEntities(ObjectSet<MiscUatpCorrespondenceAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<MiscUatpCorrespondenceAttachment> link)
        {
            if (link == null)
                link = new Action<MiscUatpCorrespondenceAttachment>(c => { });

            var miscUatpCorrespondenceAttachments = new List<MiscUatpCorrespondenceAttachment>();

            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CorrespondenceAttachment))
            {

                // first result set includes the category
                foreach (var c in
                    muMaterializers.MiscUatpCorrespondenceAttachmentMaterializer.Materialize(reader)
                    .Bind(objectSet)
                    .ForEach(link)
                    )
                {
                    miscUatpCorrespondenceAttachments.Add(c);
                }
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load Correspondence Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && miscUatpCorrespondenceAttachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return miscUatpCorrespondenceAttachments;
        }
    }
}

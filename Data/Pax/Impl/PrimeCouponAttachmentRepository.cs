using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Pax.Impl
{
    public class PrimeCouponAttachmentRepository
    {
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<PrimeCouponAttachment> LoadEntities(ObjectSet<PrimeCouponAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<PrimeCouponAttachment> link)
        {
            if (link == null)
                link = new Action<PrimeCouponAttachment>(c => { });

            var primeCouponAttachments = new List<PrimeCouponAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.CouponAttachment))
            {
                // first result set includes the category
                primeCouponAttachments = new PaxMaterializers().PrimeCouponAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load Prime Coupon Attachment uploaded by user details
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && primeCouponAttachments.Count > 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                       , loadStrategyResult
                       , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            }

            return primeCouponAttachments;
        }


        public static List<PrimeCouponAttachment> LoadAuditEntities(ObjectSet<PrimeCouponAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<PrimeCouponAttachment> link)
        {
            if (link == null)
                link = new Action<PrimeCouponAttachment>(c => { });

            var primeCouponAttachments = new List<PrimeCouponAttachment>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.PMAttachment))
            {
                // first result set includes the category
                primeCouponAttachments = new PaxMaterializers().PrimeCouponAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            ////Load Prime Coupon Attachment uploaded by user details
            //if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && primeCouponAttachments.Count > 0)
            //{
            //  UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
            //         , loadStrategyResult
            //         , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
            //}

            return primeCouponAttachments;
        }

    }
}

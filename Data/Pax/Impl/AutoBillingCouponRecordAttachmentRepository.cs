using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.AutoBilling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
    public class AutoBillingCouponRecordAttachmentRepository : Repository<AutoBillingPrimeCouponAttachment>, IAutoBillingCouponRecordAttachmentRepository
    {
        public IQueryable<AutoBillingPrimeCouponAttachment> GetDetail(Expression<Func<AutoBillingPrimeCouponAttachment, bool>> where)
        {
            throw new NotImplementedException();
        }

        public static List<AutoBillingPrimeCouponAttachment> LoadEntities(ObjectSet<AutoBillingPrimeCouponAttachment> objectSet, LoadStrategyResult loadStrategyResult, Action<AutoBillingPrimeCouponAttachment> link)
        {
          if (link == null)
            link = new Action<AutoBillingPrimeCouponAttachment>(c => { });

          var autoBillCouponAttachments = new List<AutoBillingPrimeCouponAttachment>();

          using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.AutoBillCouponAttach))
          {
            // first result set includes the category
            autoBillCouponAttachments = new PaxMaterializers().AutoBillCpnAttachmentMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
            reader.Close();
          }

          //Load Prime Coupon Attachment uploaded by user details
          if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AttachmentUploadedbyUser) && autoBillCouponAttachments.Count > 0)
          {
            UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>()
                   , loadStrategyResult
                   , null, LoadStrategy.Entities.AttachmentUploadedbyUser);//usr => usr.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId)
          }

          return autoBillCouponAttachments;
        }
    }
}

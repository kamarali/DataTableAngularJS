using System.Linq;
using System;
using Iata.IS.Model.Pax.AutoBilling;

namespace Iata.IS.Data.Pax
{
    public interface IAutoBillingCouponRecordAttachmentRepository : IRepository<AutoBillingPrimeCouponAttachment>
  {
        IQueryable<AutoBillingPrimeCouponAttachment> GetDetail(System.Linq.Expressions.Expression<Func<AutoBillingPrimeCouponAttachment, bool>> where);
  }
}

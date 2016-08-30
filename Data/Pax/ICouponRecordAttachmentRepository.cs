using Iata.IS.Model.Pax;
using System.Linq;
using System;

namespace Iata.IS.Data.Pax
{
  public interface ICouponRecordAttachmentRepository: IRepository<PrimeCouponAttachment>
  {
    IQueryable<PrimeCouponAttachment> GetDetail(System.Linq.Expressions.Expression<Func<PrimeCouponAttachment, bool>> where);
  }
}

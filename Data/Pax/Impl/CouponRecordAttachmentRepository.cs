using System;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;

namespace Iata.IS.Data.Pax.Impl
{
  public class CouponRecordAttachmentRepository : Repository<PrimeCouponAttachment>, ICouponRecordAttachmentRepository
  {
    public override PrimeCouponAttachment Single(System.Linq.Expressions.Expression<Func<PrimeCouponAttachment, bool>> where)
    {
      var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
      return attachmentRecord;
    }

    public IQueryable<PrimeCouponAttachment> GetDetail(System.Linq.Expressions.Expression<Func<PrimeCouponAttachment, bool>> where)
    {
      var attachmentRecords = EntityObjectSet.Include("UploadedBy").Where(where);
      return attachmentRecords;
    }
  }
}

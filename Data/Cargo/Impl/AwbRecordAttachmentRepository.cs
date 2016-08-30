using System;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo.Impl
{
  public class AwbRecordAttachmentRepository : Repository<AwbAttachment>, IAwbRecordAttachmentRepository
  {
    public override AwbAttachment Single(System.Linq.Expressions.Expression<Func<AwbAttachment, bool>> where)
    {
      var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
      return attachmentRecord;
    }

    public IQueryable<AwbAttachment> GetDetail(System.Linq.Expressions.Expression<Func<AwbAttachment, bool>> where)
    {
      var attachmentRecords = EntityObjectSet.Include("UploadedBy").Where(where);
      return attachmentRecords;
    }
  }
}

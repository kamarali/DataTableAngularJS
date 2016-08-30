using System;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class MiscCorrespondenceAttachmentRepository : Repository<MiscUatpCorrespondenceAttachment>, IMiscCorrespondenceAttachmentRepository
  {
    public override MiscUatpCorrespondenceAttachment Single(System.Linq.Expressions.Expression<Func<MiscUatpCorrespondenceAttachment, bool>> where)
    {
      var attachmentRecord = EntityObjectSet.Include("FileServer").Include("UploadedBy").SingleOrDefault(where);
      return attachmentRecord;
    }
  }
}

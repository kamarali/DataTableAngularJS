using System;
using System.Linq;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
  public interface IAwbRecordAttachmentRepository : IRepository<AwbAttachment>
  {
    IQueryable<AwbAttachment> GetDetail(System.Linq.Expressions.Expression<Func<AwbAttachment, bool>> where);
  }
}

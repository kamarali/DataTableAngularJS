using Iata.IS.Model.Cargo;
using System.Linq;
using System;

namespace Iata.IS.Data.Cargo
{
  public interface IRMAwbAttachmentRepository : IRepository<RMAwbAttachment>
  {
     /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<RMAwbAttachment> GetDetail(System.Linq.Expressions.Expression<Func<RMAwbAttachment, bool>> where);
  }
}

using Iata.IS.Model.Pax;
using System.Linq;
using System;

namespace Iata.IS.Data.Pax
{
  public interface IRejectionMemoCouponAttachmentRepository : IRepository<RMCouponAttachment>
  {
     /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<RMCouponAttachment> GetDetail(System.Linq.Expressions.Expression<Func<RMCouponAttachment, bool>> where);
  }
}

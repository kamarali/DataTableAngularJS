using Iata.IS.Model.Pax;
using System.Linq;
using System;

namespace Iata.IS.Data.Pax
{
  public interface IBillingMemoCouponAttachmentRepository : IRepository<BMCouponAttachment>
  {
    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<BMCouponAttachment> GetDetail(System.Linq.Expressions.Expression<Func<BMCouponAttachment, bool>> where);
  }
}

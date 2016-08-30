using Iata.IS.Model.Pax;
using System.Linq;
using System;

namespace Iata.IS.Data.Pax
{
  public interface ICreditMemoCouponAttachmentRepository : IRepository<CMCouponAttachment>
  {
    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<CMCouponAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CMCouponAttachment, bool>> where);
  }
}

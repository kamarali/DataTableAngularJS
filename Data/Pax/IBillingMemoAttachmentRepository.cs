using Iata.IS.Model.Pax;
using System;
using System.Linq;

namespace Iata.IS.Data.Pax
{
  public interface IBillingMemoAttachmentRepository : IRepository<BillingMemoAttachment>
  {
    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<BillingMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<BillingMemoAttachment, bool>> where);
  }
}

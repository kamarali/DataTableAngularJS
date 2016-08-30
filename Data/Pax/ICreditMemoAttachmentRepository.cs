using Iata.IS.Model.Pax;
using System;
using System.Linq;

namespace Iata.IS.Data.Pax
{
  public interface ICreditMemoAttachmentRepository : IRepository<CreditMemoAttachment>
  {
    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns> 
    IQueryable<CreditMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CreditMemoAttachment, bool>> where);
  }
}

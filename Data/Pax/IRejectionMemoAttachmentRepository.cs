using Iata.IS.Model.Pax;
using System;
using System.Linq;

namespace Iata.IS.Data.Pax
{
  public interface IRejectionMemoAttachmentRepository : IRepository<RejectionMemoAttachment>
  {
    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<RejectionMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<RejectionMemoAttachment, bool>> where);
  }
}

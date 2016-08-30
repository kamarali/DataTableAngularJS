using Iata.IS.Model.Cargo;
using System;
using System.Linq;

namespace Iata.IS.Data.Cargo
{
  public interface ICgoRejectionMemoAttachmentRepository : IRepository<CgoRejectionMemoAttachment>
  {
    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<CgoRejectionMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CgoRejectionMemoAttachment, bool>> where);
  }
}

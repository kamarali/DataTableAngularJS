using Iata.IS.Model.Cargo;
using System;
using System.Linq;

namespace Iata.IS.Data.Cargo
{
  public interface ICargoCreditMemoAttachmentRepository : IRepository<CargoCreditMemoAttachment>
  {
      /// <summary>
      /// Added to display records in supporting document 
      /// </summary>
      /// <param name="where"></param>
      /// <returns></returns>
      IQueryable<CargoCreditMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CargoCreditMemoAttachment, bool>> where);
  }
}

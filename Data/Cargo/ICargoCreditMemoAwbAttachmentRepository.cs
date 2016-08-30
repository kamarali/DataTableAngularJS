using System;
using System.Linq;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
  public interface ICargoCreditMemoAwbAttachmentRepository : IRepository<CMAwbAttachment>
  {
      /// <summary>
      /// Added to display records in supporting document 
      /// </summary>
      /// <param name="where"></param>
      /// <returns></returns>
      IQueryable<CMAwbAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CMAwbAttachment, bool>> where);

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
  public interface ICargoAwbAttachmentRepository:IRepository<AwbAttachment>
    {
        /// <summary>
        /// Gets the detail.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        IQueryable<AwbAttachment> GetDetail(System.Linq.Expressions.Expression<Func<AwbAttachment, bool>> where);
    }
}
  

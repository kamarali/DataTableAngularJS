using System;
using System.Linq;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
    public interface ICargoBillingMemoAttachmentRepository : IRepository<CargoBillingMemoAttachment>
    {
        /// <summary>
        /// Gets the detail.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        IQueryable<CargoBillingMemoAttachment> GetDetail(System.Linq.Expressions.Expression<Func<CargoBillingMemoAttachment, bool>> where);
    }
}

using System;
using System.Linq;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Data.Pax
{
  public interface ISamplingFormDAttachmentRepository : IRepository<SamplingFormDRecordAttachment>
  {
      IQueryable<SamplingFormDRecordAttachment> GetDetail(System.Linq.Expressions.Expression<Func<SamplingFormDRecordAttachment, bool>> where);
  }
}
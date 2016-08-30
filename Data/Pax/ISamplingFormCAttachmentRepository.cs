using Iata.IS.Model.Pax.Sampling;
using System.Linq;
using System;

namespace Iata.IS.Data.Pax
{
  public interface ISamplingFormCAttachmentRepository : IRepository<SamplingFormCRecordAttachment>
  {
    /// <summary>
    /// Added to display records in supporting document 
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    IQueryable<SamplingFormCRecordAttachment> GetDetail(System.Linq.Expressions.Expression<Func<SamplingFormCRecordAttachment, bool>> where);
  }
}

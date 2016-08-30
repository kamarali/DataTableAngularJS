using System;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Data.Pax
{
  public interface ISamplingFormERepository : IRepository<SamplingFormEDetail>
  {
    /// <summary>
    /// Updates form E detail.
    /// </summary>
    /// <param name="invoiceId"> The invoice id.</param>
    void UpdateFormEDetails(Guid invoiceId);
  }
}

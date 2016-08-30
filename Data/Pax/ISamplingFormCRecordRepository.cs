using System;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Data.Pax
{
  public interface ISamplingFormCRecordRepository : IRepository<SamplingFormCRecord>
  {
    /// <summary>
    /// Get sampling form C record details
    /// </summary>
    /// <param name="samplingFormCRecordId"></param>
    /// <returns>samplingFormC record</returns>
    SamplingFormCRecord Single(Guid samplingFormCRecordId);

    long IsDuplicateSamplingFormCRecordExists(string ticketIssuingAirline, long ticketDocNumber, int couponNumber, 
                                                              string provisionalInvoiceNumber, int batchNumber, int sequenceNumber,
                                                              int fromMemberId, int provisionalBillingMemberId, int provisionalBillingMonth, int provisionalBillingYear);
  }
}
using System;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Data.Pax
{
    public interface ISamplingFormDRepository : IRepository<SamplingFormDRecord>
    {
      /// <summary>
      /// Updates the Sampling Form D/E source code total and header level invoice total.
      /// </summary>
      /// <param name="invoiceId">The Invoice id.</param>
      /// <param name="sourceId">The Source id.</param>
      void UpdateFormDInvoiceTotal(Guid invoiceId, int sourceId);

      /// <summary>
      /// Loadstrategy method overload of Single
      /// </summary>
      /// <param name="formDRecordId">Form D Record Id</param>
      /// <returns>SamplingFormDRecord</returns>
      SamplingFormDRecord Single(Guid formDRecordId);

      /// <summary>
      /// Checks for duplicate form D record based on provisional invoice number, batch number and record sequence number.
      /// </summary>
      /// <param name="invoiceId"></param>
      /// <param name="batchRecordSequenceNo"></param>
      /// <param name="batchSequenceNo"></param>
      /// <param name="provisionalInvoiceNo"></param>
      /// <param name="formDId"></param>
      /// <returns></returns>
      bool IsFormDRecordDuplicate(Guid invoiceId, int batchRecordSequenceNo, int batchSequenceNo,
                                  string provisionalInvoiceNo, Guid formDId);
    }
}

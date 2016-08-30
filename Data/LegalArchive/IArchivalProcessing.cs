using System;
using System.Collections.Generic;
using Iata.IS.Model.LegalArchive;

namespace Iata.IS.Data.LegalArchive
{
    public interface IArchivalProcessing
    {
        void QueueInvForArchive(int billingYear, int billingMonth, int period,bool isReArchive = false);

        void ReQueueInvoiceForArchive(Guid invoiceId, int invoiceType, int delayInDequeue);

        // void AddArchivalLogRecord(Guid invoiceId, int archiveType, string cdcArkhineoClientIdOfIata, string cdcArkhineoCoffreId, string cdcArkhineoSectionId);

        List<LegalArchivalReturn> UpdateArchieveRetrievalJobDetailsRecord(string webserviceResponseCode, string filename, string fileLocation, string retrievalStatus, string archieveId, Guid jobSummaryId);

        void UpdateArchieveRetrievalJobSummaryRecord(string jobStatus, Guid jobSummaryId);
        LegalArchiveLog AddArchivalLogRecord(Guid invoiceId, int archiveType, string cdcArkhineoClientIdOfIata, string cdcArkhineoCoffreId, string cdcArkhineoSectionId);
    }
}

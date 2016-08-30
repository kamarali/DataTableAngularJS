using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.LegalArchive;

namespace Iata.IS.Data.LegalArchive.Impl
{
    public class ArchivalProcessing : Repository<InvoiceBase>, IArchivalProcessing
    {
        #region Constants
        // for QueueInvForArchive
        private const string BillingYear = "BILLING_YEAR_I";
        private const string BillingMonth = "BILLING_MONTH_I";
        private const string BillingPeriod = "BILLING_PERIOD_I";
        private const string IsReArchive = "IS_RE_ARCHIVE";
        private const string QueueInvoicesForArchive = "QueueInvoicesForArchive";
        private const string ReQueueInvoicesForArchive = "ReQueueInvoicesForArchive";

        // For Re-Queue Invoice for Re-Archive
        private const string InvId = "INVOICE_ID";
        private const string InvoiceType = "INVOICE_TYPE";
        private const string DelayInDeQueue = "DELAY_IN_DEQUEUE";

        // for AddArchivalLogRecord
        private const string InvoiceId = "INVOICEID";
        private const string ArchiveType = "REC_PAY_INDICATOR";
        private const string CdcArkhineoClientIdOfIata = "CDC_CLIENT_ID";
        private const string CdcArkhineoCoffreId = "CDC_COFFRE_ID";
        private const string CdcArkhineoSectionId = "CDC_SECTION_ID";
        private const string AddArchivalLogRecords = "AddArchivalLogRecords";

        #endregion

        public void QueueInvForArchive(int billingYear, int billingMonth, int period, bool isReArchive = false)
        {
            var parameters = new ObjectParameter[4];
            parameters[0] = new ObjectParameter(BillingYear, billingYear);
            parameters[1] = new ObjectParameter(BillingMonth, billingMonth);
            parameters[2] = new ObjectParameter(BillingPeriod, period);
            parameters[3] = new ObjectParameter(IsReArchive, isReArchive);

            // Execute Store Procedure PROC_QUEUE_INV_LEGAL_ARCHIVE in database for Queueing Invoices For Legal Archive
            ExecuteStoredProcedure(QueueInvoicesForArchive, parameters);

        }

        public void ReQueueInvoiceForArchive(Guid invoiceId, int invoiceType, int delayInDequeue)
        {
            var parameters = new ObjectParameter[3];
            parameters[0] = new ObjectParameter(InvId, invoiceId);
            parameters[1] = new ObjectParameter(InvoiceType, invoiceType);
            parameters[2] = new ObjectParameter(DelayInDeQueue, delayInDequeue);

            // Execute Store Procedure PROC_REQUEUE_INV_LEGAL_ARCHIVE in database for ReQueueing Invoice For Legal Archive
            ExecuteStoredProcedure(ReQueueInvoicesForArchive, parameters);
        }

        public LegalArchiveLog AddArchivalLogRecord(Guid invoiceId, int archiveType, string cdcArkhineoClientIdOfIata, string cdcArkhineoCoffreId, string cdcArkhineoSectionId)
        {
            var parameters = new ObjectParameter[5];
            parameters[0] = new ObjectParameter(InvoiceId, invoiceId);
            parameters[1] = new ObjectParameter(ArchiveType, archiveType);
            parameters[2] = new ObjectParameter(CdcArkhineoClientIdOfIata, cdcArkhineoClientIdOfIata);
            parameters[3] = new ObjectParameter(CdcArkhineoCoffreId, cdcArkhineoCoffreId);
            parameters[4] = new ObjectParameter(CdcArkhineoSectionId, cdcArkhineoSectionId);


            // Execute Store Procedure PROC_ADD_LEGAL_ARCHIVE_DATA in database for Legal Archive Log
            var legalArchiveLog = ExecuteStoredFunction<LegalArchiveLog>(AddArchivalLogRecords, parameters);
            return legalArchiveLog.First();
        }

        public List<LegalArchivalReturn> UpdateArchieveRetrievalJobDetailsRecord(string webserviceResponseCode, string filename, string fileLocation, string retrievalStatus, string archieveId, Guid jobSummaryId)
        {
            var parameters = new ObjectParameter[6];
            parameters[0] = new ObjectParameter("WEBSERVICE_RESPONSE_CODETEXT_I", typeof(string)) { Value = webserviceResponseCode };
            parameters[1] = new ObjectParameter("ZIP_FILE_NAME_I", typeof(string)) { Value = filename };
            parameters[2] = new ObjectParameter("RETRIVED_ZIP_FILE_LOCATION_I", typeof(string)) { Value = fileLocation };
            parameters[3] = new ObjectParameter("RETRIVAL_STATUS_I", typeof(string)) { Value = retrievalStatus };
            parameters[4] = new ObjectParameter("ARCHIEVE_ID_I", typeof(string)) { Value = archieveId };
            parameters[5] = new ObjectParameter("JOB_SUMMARY_ID", typeof(Guid)) { Value = jobSummaryId };

            var listRetrievalJobDetails = ExecuteStoredFunction<LegalArchivalReturn>("UpdateRetrievalJobDetailsRecord", parameters);
            return listRetrievalJobDetails.ToList();
        }

        public void UpdateArchieveRetrievalJobSummaryRecord(string jobStatus, Guid jobSummaryId)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter("JOB_STATUS_I", typeof(string)) { Value = jobStatus };
            parameters[1] = new ObjectParameter("JOB_SUMMARY_ID", typeof(string)) { Value = ConvertUtil.ConvertGuidToString(jobSummaryId) };

            ExecuteStoredProcedure("UpdateRetrievalJobSummaryRecord", parameters);
        }

    }
}

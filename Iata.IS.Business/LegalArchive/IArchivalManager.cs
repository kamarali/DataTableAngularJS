using System;

namespace Iata.IS.Business.LegalArchive
{
    /// <summary>
    /// This interface is specific to Legal Archival Processing
    /// </summary>
    public interface IArchivalManager
    {
        /// <summary>
        /// Archiving proccess of queued invoices( Service Iata.IS.LegalArchiveDeposit will use this Method).
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="archiveType"></param>
        /// <returns>True/False</returns>
        bool LegalArchivalProccess(Guid invoiceId, int archiveType);

        /// <summary>
        /// It will Queue all eligible invoices for Archiving(Archiving Job will use this Method)
        /// </summary>
        void QueueInvoicesForArchive(int period, int billingMonth, int billingYear, bool isReArchive = false);

        /// <summary>
        /// It will Create Archival Zip File 
        /// Locate folder in where offline collection is generated for the invoice
        /// Copy required files from respective folder in offline collection folder of the invoice and store it in Given location 
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="archiveType">0: Receivables 1: Payables</param>
        /// <param name="includeListings">True/False</param>
        /// <returns>SFR path of zip file</returns>
        string CreateArchiveZipFile(Guid invoiceId, int archiveType, bool includeListings);

      /// <summary>
      ///SCP400947-SRM: Legal archiving pending for July P2
      /// This method added to send email in case of any exception in processing of invoice.
      /// </summary>
      /// <param name="invoiceId">invoice id</param>
      /// <param name="invoiceType">invoice type</param>
      /// <param name="exception">exception</param>
      void SendArchivalDepositExceptionNotification(string invoiceId, string invoiceType, string exception);

    }

}

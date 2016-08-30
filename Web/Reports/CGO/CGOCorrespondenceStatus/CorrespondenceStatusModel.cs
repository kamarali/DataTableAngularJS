using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iata.IS.Web.Reports.CGO.CGOCorrespondenceStatus
{
    public class CorrespondenceStatusModel
    {
        // corr stands for correspondance
        /// <summary>
        /// property to get and set Corr. Initiating Entity Code
        /// </summary>
        public string CorrInitiatingEntityCode { get; set; }
        /// <summary>
        /// property to get and set Corr Initiating Entity Name
        /// </summary>
        public string CorrInitiatingEntityName { get; set; }
        /// <summary>
        /// Property to get and set Corr. From Entity Code
        /// </summary>
        public string CorrFromEntityCode { get; set; }

        /// <summary>
        /// Property to get and set Corr. From Entity Name
        /// </summary>
        public string CorrFromEntityName{ get; set; }

        /// <summary>
        /// Property to get and set Corr. To Entity Code
        /// </summary>
        public string CorrToEntityCode { get; set; }

        /// <summary>
        /// Property to get and set Corr. To Entity Name
        /// </summary>
        public string CorrToEntityName { get; set; }

        /// <summary>
        /// Property to get and set Corr. Status
        /// </summary>
        public string CorrStatus { get; set; }

        /// <summary>
        /// Property to get and set Corr. Sub-Status
        /// </summary>
        public string CorrSubStatus { get; set; }

        /// <summary>
        /// Property to get and set To Corr. Reference Number
        /// </summary>
        public long CorrReferenceNumber { get; set; }

        /// <summary>
        /// Property to get and set To Corr. Date
        /// </summary>
        public DateTime CorrDate { get; set; }

        /// <summary>
        /// Property to get and set Corr. Stage
        /// </summary>
        public int CorrStage { get; set; }

        /// <summary>
        /// Property to get and set Number of linked Rejection Memos
        /// </summary>
        public int NumberOfLinkedRejectionMemos { get; set; }

        /// <summary>
        /// Property to get and set Linked Rejection Invoice Number
        /// </summary>
        
        public string LinkedRejectionInvoiceNumber{ get; set; }

        /// <summary>
        /// Property to get and set Linked Rejection Invoice Billing Month
        /// </summary>
        public DateTime LinkedRejectionInvoiceBillingMonth { get; set; }

        /// <summary>
        /// Property to get and set Currency Code
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Property to get and set Amount to be Settled
        /// </summary>
        public decimal AmountToBeSettled { get; set; }

        /// <summary>
        /// Property to get and set Authority to Bill Flag
        /// </summary>
        public string AuthorityToBillFlag{ get; set; }

        /// <summary>
        /// Property to get and set No of Attachments
        /// </summary>
        public int NoOfAttachments { get; set; }

        /// <summary>
        /// Property to get and set Expiry Date
        /// </summary>
        public DateTime ExpiryDate { get; set; }
        
    } // end of CorrespondenceStatusModel
}
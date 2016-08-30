using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports
{
    public class ProcessingDashboardFileActionStatus
    {
        // Property to get and set FileID
        public Guid FileId { get; set; }

        // Property to get and set BillingMembeeId
        public int BillingMemberId { get; set; }

        // Property to get and set BillingMemberCode
        public string BillingMemberCode { get; set; }

        // Property to get and set BillingMemberName 
        public string BillingMemberName { get; set; }

        // Property to get and set FileName
        public string FileName { get; set; }

        // Property to get and set date time when files were received to IS
        public DateTime ReceivedInIs { get; set; }

        // Property to Format ReceivedInIS date.
        public string FormatedReceivedInIsDate
        {
            get
            {
                string formatedDate = string.Empty;
                formatedDate = ReceivedInIs.ToString("dd MMM yyyy HH:mm");
                return formatedDate;
            }
        }

        // Property to get and set Number of Invoices/Form Cs within a file
        public int NumberOfInvoices { get; set; }

        // Property to get and set Number of Invoices within a file for which late submission is already requested
        public int NumberOfAlreadyRequested { get; set; }

        // Property to get and set Number of Invoices on which Ation is performed within a file
        public int NumberOfActions { get; set; }

        // Property to get and set Number of Invoices for which Settlement file is created within a file
        public int NumberOfSettlementFilesCreated { get; set; }

        // Property to get and set Number of Invoices with Validation Errors within a file
        public int NumberOfValidationErrors { get; set; }

        // Property to get and set Number of Form Cs within a file
        public int NumberOfFormCs { get; set; }

        public DateTime InvoiceUpdateDate { get; set; }
    }// end ProcessingDashboardFileActionStatus class
}// end namespace

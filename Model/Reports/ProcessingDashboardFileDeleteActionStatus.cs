using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports
{
    public class ProcessingDashboardFileDeleteActionStatus
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

        // Property to get and set File Deletion Status i.e. Success of Failure
        public string DeleteStatus { get; set; }

        // Property to get and set File path
        public string FilePath { get; set; }

        public int TotalInvoices { get; set; }
        public int TotalInvoiceInError { get; set; }

    }// end ProcessingDashboardFileDeleteActionStatus class
}// end namespace

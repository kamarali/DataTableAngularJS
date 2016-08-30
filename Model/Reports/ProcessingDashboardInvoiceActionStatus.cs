using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Reports
{
    public class ProcessingDashboardInvoiceActionStatus
    {
        // Property to get and set Invoice
        public Guid InvoiceId { get; set; }

        // Property to get and set Billing Member Id
        public int BillingMemberId { get; set; }

        // Property to get and set MemberCode
        public string BillingMemberCode { get; set; }

        // Property to get and set MemberCommercialName
        public string BillingMemberName { get; set; }

        // Property to get and set InvoiceNumber
        public string InvoiceNo { get; set; }

        // Property to get and set LateSubmisssionStatus
        public string ActionStatus { get; set; }

        public int ClearingHouseId { get; set; }


        // Property to get and set Billing MemberCode
        public string BilledMemberCode { get; set; }

        // Property to get and set Billing Catagory
        public string BillingCategory { get; set; }

        // Property to get and set Currency Code
        public string CurrencyCode { get; set; }

        // Property to get and set Invoice Total Amount
        public string InvoiceAmount { get; set; }

        // Property to get and set Invoice Invoice Status
        public int InvoiceStatus { get; set; }


    }// end ProcessingDashboardInvoiceActionStatus class
}// end namespace

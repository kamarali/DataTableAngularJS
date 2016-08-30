using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.FutureSubmission
{
    public class EmailFutureSubmittedInvoices
    {
        public int BillingYear { get; set; }

        public int BillingMonth { get; set; }

        public int PeriodNo { get; set; }

        public string BilledMemberCode { get; set; }

        public int BillingCategory { get; set; }

        public string InvoiceNo { get; set; }

        public List<WebValidationError> ErrorDesc { get; set; }

        public string DetailErrorDesc { get; set; }

        public int BillingMemberId { get; set; }

        public string BillingMemberCode { get; set; }

        public string BillingMonYearPeriod { get; set; }

        public string BillingCategoryName { get; set; }
    }
}

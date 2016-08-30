using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iata.IS.Web.Reports.Miscellaneous
{
    public class MiscSubstitutionValuesReportModel
    {
        public string BillingMember { get; set; }
        public string BilledMember { get; set; }
        public string BillingPeriod { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string ChargeCategory { get; set; }
        public string ChargeCode { get; set; }
        public string FieldName { get; set; }
        public string SubstitutionValue { get; set; }
    }
}
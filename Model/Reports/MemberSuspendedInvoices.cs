using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Reports.Enums;

namespace Iata.IS.Model.Reports
{
    public class MemberSuspendedInvoices
    {
        /// <summary>
        /// Gets or sets the suspended entity code.
        /// </summary>
        /// <value>
        /// The suspended entity code.
        /// </value>
        public string SuspendedEntityCode { get; set; }

        /// <summary>
        /// Gets or sets the billing entity code.
        /// </summary>
        /// <value>
        /// The billing entity code.
        /// </value>
        public string BillingEntityCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the billing entity.
        /// </summary>
        /// <value>
        /// The name of the billing entity.
        /// </value>
        public string BillingEntityName { get; set; }

        /// <summary>
        /// Gets or sets the billed entity code.
        /// </summary>
        /// <value>
        /// The billed entity code.
        /// </value>
        public string BilledEntityCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the billed entity.
        /// </summary>
        /// <value>
        /// The name of the billed entity.
        /// </value>
        public string BilledEntityName { get; set; }

        /// <summary>
        /// Gets or sets the original clearance month period.
        /// </summary>
        /// <value>
        /// The original clearance month period.
        /// </value>
        public string OriginalClearanceMonthPeriod { get; set; }

        /// <summary>
        /// Gets or sets the settlement method indicator.
        /// </summary>
        /// <value>
        /// The settlement method indicator.
        /// </value>
        public string SettlementMethodIndicator { get; set; }

        /// <summary>
        /// Gets or sets the billing category.
        /// </summary>
        /// <value>
        /// The billing category.
        /// </value>
        public string BillingCategory { get; set; }

        /// <summary>
        /// Gets or sets the invoice no.
        /// </summary>
        /// <value>
        /// The invoice no.
        /// </value>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// Gets or sets the invoice currency.
        /// </summary>
        /// <value>
        /// The invoice currency.
        /// </value>
        public string InvoiceCurrency { get; set; }

        /// <summary>
        /// Gets or sets the invoice amount.
        /// </summary>
        /// <value>
        /// The invoice amount.
        /// </value>
        public double InvoiceAmount { get; set; }

        private string _suspensionMonthPeriod;
        /// <summary>
        /// Gets or sets the suspension month period.
        /// </summary>
        /// <value>
        /// The suspension month period.
        /// </value>
        //public string SuspensionMonthPeriod
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_suspensionMonthPeriod))
        //            return _suspensionMonthPeriod;
        //        else
        //            return Convert.ToDateTime(_suspensionMonthPeriod).ToString("yyyy-MM-dd");
        //    }
        //    set
        //    {
        //        _suspensionMonthPeriod = value;
        //    }
        //}
        public string SuspensionMonthPeriod { get; set; }

        private string _reinstatementMonthPeriod;
        /// <summary>
        /// Gets or sets the reinstatement month period.
        /// </summary>
        /// <value>
        /// The reinstatement month period.
        /// </value>
        //public string ReinstatementMonthPeriod
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_reinstatementMonthPeriod))
        //            return _reinstatementMonthPeriod;
        //        else
        //        return Convert.ToDateTime(_reinstatementMonthPeriod).ToString("yyyy-MM-dd");
        //    }
        //    set
        //    {
        //        _reinstatementMonthPeriod = value;
        //    }
        //}
        public string ReinstatementMonthPeriod { get; set; }
        /// <summary>
        /// Gets or sets the resubmission clearance month.
        /// </summary>
        /// <value>
        /// The resubmission clearance month.
        /// </value>
        public string ResubmissionClearanceMonth { get; set; }

        /// <summary>
        /// Gets or sets the resubmission status.
        /// </summary>
        /// <value>
        /// The resubmission status.
        /// </value>
        public string ResubmissionStatus { get; set; }

        /// <summary>
        /// Gets or sets the remarks.
        /// </summary>
        /// <value>
        /// The remarks.
        /// </value>
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or sets the adjustment.
        /// </summary>
        /// <value>
        /// The adjustment.
        /// </value>
        public string Adjustment { get; set; }

        public string ReportGeneratedDate { get { return string.Format("{0} {1}", DateTime.UtcNow.ToString("dd-MMM-yyyy hh:mm:ss tt"), "UTC"); } }
    }
}

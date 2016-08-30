using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Model.Base;

namespace Iata.IS.Web.Reports.SuspendedInvoice
{
    public class MemberSuspendedInvoice 
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

        /// <summary>
        /// Gets or sets the suspension month period.
        /// </summary>
        /// <value>
        /// The suspension month period.
        /// </value>
        public DateTime SuspensionMonthPeriod { get; set; }

        /// <summary>
        /// Gets or sets the reinstatement month period.
        /// </summary>
        /// <value>
        /// The reinstatement month period.
        /// </value>
        public DateTime ReinstatementMonthPeriod { get; set; }

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

        /// <summary>
        /// Gets or sets from billing year.
        /// </summary>
        /// <value>
        /// From billing year.
        /// </value>
        public int FromBillingYear { get; set; }

        /// <summary>
        /// Gets or sets from billing month.
        /// </summary>
        /// <value>
        /// From billing month.
        /// </value>
        public int FromBillingMonth { get; set; }

        /// <summary>
        /// Gets or sets from billing period.
        /// </summary>
        /// <value>
        /// From billing period.
        /// </value>
        public int FromBillingPeriod { get; set; }

        /// <summary>
        /// Gets or sets to billing year.
        /// </summary>
        /// <value>
        /// To billing year.
        /// </value>
        public int ToBillingYear { get; set; }

        /// <summary>
        /// Gets or sets to billing month.
        /// </summary>
        /// <value>
        /// To billing month.
        /// </value>
        public int ToBillingMonth { get; set; }

        /// <summary>
        /// Gets or sets to billing period.
        /// </summary>
        /// <value>
        /// To billing period.
        /// </value>
        public int ToBillingPeriod { get; set; }

        /// <summary>
        /// Gets or sets the iata member id.
        /// </summary>
        /// <value>
        /// The iata member id.
        /// </value>
        public int IataMemberId { get; set; }

        /// <summary>
        /// Gets or sets the ach member id.
        /// </summary>
        /// <value>
        /// The ach member id.
        /// </value>
        public int AchMemberId { get; set; }

        /// <summary>
        /// Gets or sets the settlement method id.
        /// </summary>
        /// <value>
        /// The settlement method id.
        /// </value>
        public int SettlementMethodId { get; set; }

        /// <summary>
        /// Gets or sets the billing category id.
        /// </summary>
        /// <value>
        /// The billing category id.
        /// </value>
        public int BillingCategoryId { get; set; }

        public string ReportGeneratedDate{ get; set; }
        
        
    }
}
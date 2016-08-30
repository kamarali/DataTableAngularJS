using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.ConfirmationSummary
{
    public class ConfirmationSummaryModel
    {
        /// <summary>
        /// Property to get and set ClearanceMonth
        /// </summary>
        [DisplayName ("Clearance Month") ]
        public string ClearanceMonth { get; set; }

        /// <summary>
        /// Property to get and set Period Number
        /// </summary>
        [DisplayName("Period Number")]
        public int PeriodNumber { get; set; }

        /// <summary>
        /// Property to get and set Billing Entity Code
        /// </summary>
        [DisplayName("Billing Entity Code")]
        public string BillingEntityCode { get; set; }

        /// <summary>
        /// Property to get and set Billing Entity Name
        /// </summary>
        [DisplayName("Billing Entity Name")]
        public string BillingEntityName { get; set; }

        /// <summary>
        /// Property to get and set Billed Entity Code
        /// </summary>
        [DisplayName("Billed Entity Code")]
        public string BilledEntityCode { get; set; }

        /// <summary>
        /// Property to get and set Billed Entity Name
        /// </summary>
        [DisplayName("Billed Entity Name")]
        public string BilledEntityName { get; set; }

        /// <summary>
        /// Property to get and set Agreement Indicator Validated
        /// </summary>
        [DisplayName("Agreement Indicator Validated")]
        public string  AgreementIndicatorValidated { get; set; }

        /// <summary>
        /// Property to get and set Original PMI
        /// </summary>
        [DisplayName("Original PMI")]
        public string OriginalPMI { get; set; }

        /// <summary>
        /// Property to get and set Agreement Validated PMI
        /// </summary>
        [DisplayName("Validated PMI")]
        public string ValidatedPMI { get; set; }

        /// <summary>
        /// Property to get and set  TotalBillingRecord
        /// </summary>
        [DisplayName("Total no of Billing Records")]
        public int TotalBillingRecord { get; set; }

        /// <summary>
        /// Property to get and set  InvoiceCount
        /// </summary>
        public int InvoiceCount { get; set; }

        public int BillingMemberid { get; set; }

        public int ParticipateInValueConfirmation { get; set; }

        public int AutomatedReportRequired { get; set; }

        public string BillingnumericCode { get; set; }

        public int CouponNo { get; set; }

        public Int64 DocNo { get; set; }

       
        public decimal AgreementPercentage { get; set; }

         [DisplayName("Percentage within Agreement Ind- Validated")]
        public string PercentageAggrement { get; set; }
    }// End ConfirmationSummaryModel
}// End namespace

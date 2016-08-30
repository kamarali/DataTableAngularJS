using System;
using System.ComponentModel;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Reports.ReportSearchCriteria
{
    public class ReportSearchCriteriaModel
    {

        /// <summary> 
        /// Get and set Billing Year From
        /// </summary>
        [DisplayName("From Billing Year")]
        public int FromYear { get; set; }

        /// <summary> 
        /// Get and set Billing Month From
        /// </summary>
        [DisplayName("From Billing Month")]
        public int FromMonth { get; set; }

        /// <summary> 
        /// Get and set Billing Period From
        /// </summary>
        [DisplayName("From Billing Period")]
        public int FromPeriod { get; set; }

        /// <summary> 
        /// Get and set Billing Year To
        /// </summary>
        [DisplayName("Billing Year To")]
        public int ToYear { get; set; }

        /// <summary> 
        /// Get and set Billing Period From
        /// </summary>
        [DisplayName("Billing Month To")]
        public int ToMonth { get; set; }

        /// <summary> 
        /// Get and set Billing Period From
        /// </summary>
        [DisplayName("Billing Period To")]
        public int ToPeriod { get; set; }


        /// <summary> 
        /// Get and set from date 
        /// </summary>
        [DisplayName("From Date")]
        public DateTime Fromdate { get; set; }

        /// <summary>
        /// get and set to date
        /// </summary>
        [DisplayName("To Date")]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Property to get and set Month
        /// </summary>
        [DisplayName("Month")]
        public int Month { get; set; }

        /// <summary>
        /// Property to get and set Year
        /// </summary>
        [DisplayName("Year")]
        public int Year { get; set; }

        /// <summary>
        /// Property to get and set Billing Entity Id
        /// </summary>
        [DisplayName("Billing Entity Id")]
        public int BillingEntityId { get; set; }

        /// <summary>
        /// Property to get and set Billing Entity Code
        /// </summary>
        [DisplayName("Billing Entity Code")]
        public string BillingEntityCode { get; set; }

        /// <summary>
        /// Property to get and set Billed Entity Id
        /// </summary>
        [DisplayName("Billed Entity ID")]
        public int BilledEntityId { get; set; }

        /// <summary>
        /// Property to get and set Billed Entity Code
        /// </summary>
        [DisplayName("Billed Entity Code")]
        public string BilledEntityCode { get; set; }

        /// <summary>
        /// Property to get and set Currency ID
        /// </summary>
        [DisplayName("Currency ID")]
        public int CurrencyId { get; set; }

        /// <summary>
        /// Property to get and set Currency Code
        /// </summary>
        [DisplayName("Currency Code")]
        public int CurrencyCode { get; set; }

        /// <summary>
        /// Property to get and set Billing Category (PAX-CGO-MISC)
        /// </summary>
        [DisplayName("Billing Category")]
        public int BillingCategory { get; set; }

        public int Period { get; set; }
        
        public string SettlementMethod { get; set; }

        public int SubmissionMethodId { set; get; }
        
        public string InvoiceNo { get; set; }

        public string MemoType { get; set; }// RM/BM/CM No

        public string RmbmcmNumber { get; set; }

        public string AirlineCode { get; set; }

        public string AirlineId { get; set; }

        public int BillingType { get; set; }

        /// <summary>
        /// Get and set charge category
        /// </summary>
        public int ChargeCategory { get; set; }

        /// <summary>
        /// Author: Sachin Pharande
        /// Purpose: Property to get and set Charge Code
        /// </summary>
        public int ChargeCode { get; set; }

        /// <summary>
        /// Author: Sachin Pharande
        /// Purpose: Property to get and set Period No
        /// </summary>
        [DisplayName("Period No")]
        public int PeriodNo { get; set; }

        /// <summary>
        /// Author: Sachin Pharande
        /// Purpose: Property to get and set Settlement Method Indiacator ID
        /// </summary>
        [DisplayName("Settlement Method Indicator ID")]
        public int SettlementMethodIndicatorId { get; set; }

        /// <summary>
        /// Author: Sachin Pharande
        /// Purpose: Property to get and set Settlement Method Indiacator
        /// </summary>
        [DisplayName("Settlement Method Indicator")]
        public int SettlementMethodIndicator { get; set; }

        /// <summary>
        /// Author: Sachin Pharande
        /// Purpose: Property to get and set Include FIM Data
        /// </summary>
        [DisplayName("Include FIM Data")]
        public bool IncludeFIMData { get; set; }

        /// <summary>
        /// Author: Sachin Pharande
        /// Purpose: Property to get and set is Totals Required
        /// </summary>
        public bool IsTotalsRequired { get; set; }

        /// <summary>
        /// Author: Sanket Shrivastava
        /// Purpose: Property to get and set Submission method
        /// </summary>
        /// <remarks>
        /// Wrapper property over SubmissionMethod Id.
        /// </remarks>
         [DisplayName("Data Source")]
        public SubmissionMethod SubmissionMethod
        {
            set
            {
                SubmissionMethodId = Convert.ToInt32(value);
            }
            get
            {
                return (SubmissionMethod)SubmissionMethodId;
            }
        }


        public int PercentVarianceThreshold { get; set; }

        public int AmountVarianceThreshold { get; set; }

        /// <summary>
        /// Author: Kamarali Dukandar
        /// Purpose: Property to get and set Invoice Type
        /// </summary>
        [DisplayName("Invoice Type")]
        public string InvoiceType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel
{
    public class ReceivableCargoSubmissionOverviewUI
    {
        /// <summary>
        /// Property to get and set BillingYearFrom
        /// </summary>
        public int BillingYearFrom { get; set; }

        /// <summary>
        /// Property to get and set BillingYearTo
        /// </summary>
        public int BillingYearTo { get; set; }

        /// <summary>
        /// Property to get and set BillingMonthFrom
        /// </summary>
        public int BillingMonthFrom { get; set; }

        /// <summary>
        /// Property to get and set BillingMonthTo
        /// </summary>
        public int BillingMonthTo { get; set; }

        /// <summary>
        /// Property to get and set PeriodNoFrom
        /// </summary>
        public int PeriodNoFrom { get; set; }

        /// <summary>
        /// Property to get and set PeriodNoTo
        /// </summary>
        public int PeriodNoTo { get; set; }

        /// <summary>
        /// Property to get and set BilledEntity
        /// </summary>
        public int BilledEntity { get; set; }

        /// <summary>
        /// Property to get and set BillingEntity
        /// </summary>
        public int BillingEntity { get; set; }

        /// <summary>
        /// Property to get and set EntityId
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Property to get and set BillingMonthFrom
        /// </summary>
        public string SettlementMethod { get; set; }

        /// <summary>
        /// Property to get and set Output
        /// </summary>
        // Need to get this clear from Mrugaja ma'am..!!
        public int Output { get; set; }

    }
}

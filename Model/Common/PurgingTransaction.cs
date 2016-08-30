using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{
    public class PurgingTransaction
    {
        /// <summary>
        /// Gets or sets Billing Period
        /// </summary>
       // public int BillingPeriod { get; set; }

        /// <summary>
        /// Gets or sets Billing Month
        /// </summary>
        //public int BillingMonth { get; set; }
        
        /// <summary>
        /// Gets or sets Billing Year
        /// </summary>
        //public int BillingYear { get; set; }

        public DateTime CurrentPeriod { get; set; }

        public int ResultCount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iata.IS.Web.Reports.InterlinePayablesAnalysis
{
    public class InterlinePayablesAnalysisReportModel
    {
        /// <summary>
        /// Property to get and set Billing Member Alpha code
        /// </summary>
        public string BillingMemberCodeAlpha { get; set; }
        /// <summary>
        /// Property to get and set Billing Billing Member Numeric code
        /// </summary>
        public string BillingMemberCodeNumeric { get; set; }
        /// <summary>
        /// Property to get and set Billing Member Name
        /// </summary>
        public string BillingMemberName { get; set; }
        /// <summary>
        /// Property to get and set Currency Code
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// Property to get and set Pax Total for queried month
        /// </summary>
        public decimal PaxTotal { get; set; }
        /// <summary>
        /// Property to get and set cargo Total for queried month
        /// </summary>
        public decimal CgoTotal { get; set; }

        /// <summary>
        /// Property to get and set Misc Total for queried month
        /// </summary>
        public decimal MscTotal { get; set; }

        /// <summary>
        /// Property to get and set UATP Total for queried month
        /// </summary>
        public decimal UatpTotal { get; set; }

        /// <summary>
        /// Property to get and set Pax Total for One Month before
        /// </summary>
        public decimal PaxTotalOneMonth { get; set; }
        /// <summary>
        /// Property to get and set cargo Total for One Month before
        /// </summary>
        public decimal CgoTotalOneMonth { get; set; }

        /// <summary>
        /// Property to get and set Misc Total for One Month before
        /// </summary>
        public decimal MscTotalOneMonth { get; set; }

        /// <summary>
        /// Property to get and set UATP Total for One Month before
        /// </summary>
        public decimal UatpTotalOneMonth { get; set; }

        /// <summary>
        /// Property to get and set Pax Total for Two Month before
        /// </summary>
        public decimal PaxTotalTwoMonth { get; set; }
        /// <summary>
        /// Property to get and set cargo Total for Two Month before
        /// </summary>
        public decimal CgoTotalTwoMonth { get; set; }

        /// <summary>
        /// Property to get and set Misc Total for Two Month before
        /// </summary>
        public decimal MscTotalTwoMonth { get; set; }

        /// <summary>
        /// Property to get and set UATP Total for Two Month before
        /// </summary>
        public decimal UatpTotalTwoMonth { get; set; }

        /// <summary>
        /// Property to get and set Pax Total for Three Month before
        /// </summary>
        public decimal PaxTotalThreeMonth { get; set; }
        /// <summary>
        /// Property to get and set cargo Total for Three Month before
        /// </summary>
        public decimal CgoTotalThreeMonth { get; set; }

        /// <summary>
        /// Property to get and set Misc Total for Three Month before
        /// </summary>
        public decimal MscTotalThreeMonth { get; set; }

        /// <summary>
        /// Property to get and set UATP Total for Three Month before
        /// </summary>
        public decimal UatpTotalThreeMonth { get; set; }

        /// <summary>
        /// Property to get and set Pax Total for Four Month before
        /// </summary>
        public decimal PaxTotalFourMonth { get; set; }
        /// <summary>
        /// Property to get and set cargo Total for Four Month before
        /// </summary>
        public decimal CgoTotalFourMonth { get; set; }

        /// <summary>
        /// Property to get and set Misc Total for Four Month before
        /// </summary>
        public decimal MscTotalFourMonth { get; set; }

        /// <summary>
        /// Property to get and set UATP Total for Four Month before
        /// </summary>
        public decimal UatpTotalFourMonth { get; set; }

        /// <summary>
        /// Property to get and set Pax Total for Five Month before
        /// </summary>
        public decimal PaxTotalFiveMonth { get; set; }
        /// <summary>
        /// Property to get and set cargo Total for Five Month before
        /// </summary>
        public decimal CgoTotalFiveMonth { get; set; }

        /// <summary>
        /// Property to get and set Misc Total for Five Month before
        /// </summary>
        public decimal MscTotalFiveMonth { get; set; }

        /// <summary>
        /// Property to get and set UATP Total for Five Month before
        /// </summary>
        public decimal UatpTotalFiveMonth { get; set; }
    }
}
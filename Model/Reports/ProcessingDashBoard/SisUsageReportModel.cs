using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.ProcessingDashBoard
{
  public  class SisUsageReportModel
    {
        public int FromPeriod { get; set; }

        public int ToPeriod { get; set; }

        public string MemberCode { get; set; }

        public int MemberId { get; set; }

        public string ParticipantType { get; set; }

        public int FromBillingYear { get; set; }

        public int ToBillingYear { get; set; }

        public int FromBillingMonth { get; set; }

        public int ToBillingMonth { get; set; }

        /// <summary>
        /// Author: Sachin Pharande
        /// Date: 21-03-2012
        /// Purpose: Prperty added for the Download file format option 
        /// </summary>
        public int DownloadFileFormats { get; set; }

        /// <summary>
        /// Search Criteria
        /// </summary>
        /// CMP #659: SIS IS-WEB Usage Report.
        public string SearchCriteria { get; set; }

        // CMP #659: SIS IS-WEB Usage Report.
        public Guid OfflineReportLogId { get; set; }
    }
}

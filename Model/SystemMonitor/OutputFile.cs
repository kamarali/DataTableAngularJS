using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Iata.IS.Model.SystemMonitor
{
    public class OutputFile
    {
        public Guid FileId { get; set; }

        public int FileFormatId { get; set; }

        public int FileMemberId { get; set; }

        public string FileMemberName { get; set; }

        public int ProvisionalBillingPeriod { get; set; }

        public int ProvisionalBillingMonth { get; set; }

        public int ProvisionalBillingYear { get; set; }

        public int FileStatusId { get; set; }

        public DateTime FileDate { get; set; }

        public DateTime? FileSubmissionFrom { get; set; }

        public DateTime? FileSubmissionTo { get; set; }

        public string FileName { get; set; }

        public string FileStatus { get; set; }

        public string FileFormat { get; set; }

        public bool IsPurged { get; set; }

        //CMP#622: MISC Outputs Split as per Location ID
        public string MiscLocationCode { get; set; }
    }
}

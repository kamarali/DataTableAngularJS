using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.SystemMonitor
{
    public class CompletedJobs
    {
        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public DateTime StartTime { get; set; }

        public string FormatedStartTime
        {

            get
            {
                string formatedDate = string.Empty;
                // SCP#448241: SRM:time format change
                // SpiraIncident#11462: Date Format in Completed Jobs grid (Year is changed from yyyy to yy and insted of space, hyphen [-] is used to seperate the dd, MMM & yy)
                formatedDate = StartTime.ToString("dd-MMM-yy HH:mm");
                return formatedDate;
            }
        }

        public DateTime EndTime { get; set; }

        public string FormatedEndtime
        {
            get
            {
                string formatedDate = string.Empty;
                // SCP#448241: SRM:time format change
                // SpiraIncident#11462: Date Format in Completed Jobs grid (Year is changed from yyyy to yy and insted of space, hyphen [-] is used to seperate the dd, MMM & yy)
                formatedDate = EndTime.ToString("dd-MMM-yy HH:mm");
                return formatedDate;
            }
        }

        public int ElapsedTime { get; set; }

        public string FileSize { get; set; }

        public string Status { get; set; }

        public bool IsPurged { get; set; }

        //SCP#440318 - SRM: current stats screen is too slow
        public int TotalRows { get; set; }

      

    }
}

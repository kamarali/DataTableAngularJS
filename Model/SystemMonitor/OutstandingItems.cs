using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.SystemMonitor
{
    public class OutstandingItems
    {
        public Guid FileId { get; set; }

        public string Request { get; set; }

        public string SendTo { get; set; }

        public string BillingEntity { get; set; }

        public string BilledEntity { get; set; }

        public string FileName { get; set; }

        public DateTime SentOnDate { get; set; }

        public string FormatedSentOnDate
        {
            get
            {
                string formatedDate = string.Empty;
                formatedDate = SentOnDate.ToString("dd MMM yyyy HH:mm");
                return formatedDate;
            }
        }

        public string CurrentStatus { get; set; }

    }
}

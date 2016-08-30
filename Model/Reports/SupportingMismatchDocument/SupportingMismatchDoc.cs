using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.SupportingMismatchDocument
{
    public class SupportingMismatchDoc
    {
        /// <summary>
        /// Property to get and set Airline Code
        /// </summary>
        public string AirlineCode { get; set; }

        /// <summary>
        /// Property to get and set Airline name
        /// </summary>
        public string AirlineName { get; set; }

        /// <summary>
        /// Property to get and Billing month and year
        /// </summary>
        public string BillingMonthAndYear { get; set; }

        /// <summary>
        /// Property to get and set Billing Period
        /// </summary>
        public int BillingPeriod { get; set; }

        /// <summary>
        /// Property to get and set SettlementMethos
        /// </summary>
        public string SettlementMethod { get; set; }

        /// <summary>
        /// Property to get and set Invoice Type
        /// </summary>
        public string InvoiceType { get; set; }

        /// <summary>
        /// Property to get and set Invoice no
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// Property to get and set Batch No
        /// </summary>
        public int BatchNo { get; set; }

        /// <summary>
        /// Property to get and set BD_SR_NO
        /// </summary>
        public int BDSerialNo { get; set; }

        /// <summary>
        /// Property to get and set Seq No
        /// </summary>
        public int SeqNo { get; set; }

        /// <summary>
        /// Property to get and set Record type
        /// </summary>
        public string RecordType { get; set; }

        /// <summary>
        /// Property to get and set DocumentNo
        /// </summary>
        public string DocNo { get; set; }

        /// <summary>
        /// Property to get and set attachment Indicator
        /// </summary>
        public int Attachment { get; set; }

        /// <summary>
        /// Property to get and set No Of Attachment
        /// </summary>
        public int NoOfAttachment { get; set; }

        /// <summary>
        /// property to get and set time on report
        /// </summary>
        public string ReportGeneratedDate { get { return string.Format("{0:dd-MMM-yyyy HH:mm:ss tt} {1}", DateTime.UtcNow, "UTC"); } }

        /// <summary>
        /// Property to get and set Attachment Indicator
        /// </summary>
        public string AttachmentIndicator { get; set; }
    }// end SupportingMismatchDoc class
}// end namespace

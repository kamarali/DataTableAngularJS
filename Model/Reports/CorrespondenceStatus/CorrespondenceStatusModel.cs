using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.CorrespondenceStatus
{
 
    public class CorrespondenceStatusModel
    {
       
        /// <summary>
        /// property to get and set alpha Initiating Entity Code
        /// </summary>
        public string InitiatingEntityCodeAlpha { get; set;}
        /// <summary>
        /// property to get and set numeric Initiating Entity Code
        /// </summary>
        public string InitiatingEntityCodeNumeric { get; set; }
        /// <summary>
        /// Property to get and set Initiating entity name
        /// </summary>
        public string InitiatingEntityName { get; set;}

        /// <summary>
        /// Property to get and set Initiating Entity
        /// </summary>
        public string InitiatingEnity { get; set; }

        /// <summary>
        /// Property to get and set alpha from entity code
        /// </summary>
        public string FromEntityCodeAlpha { get; set; }

        /// <summary>
        /// Property to get and set numeric from entity code
        /// </summary>
        public string FromEntityCodeNumeric { get; set; }

        /// <summary>
        /// Property to get and set from entity name
        /// </summary>
        public string FromEntityname { get; set; }

        /// <summary>
        /// Property to get and set From Entity
        /// </summary>
        public string FromEntity { get; set; }

        /// <summary>
        /// Property to get and set To alpha entity code
        /// </summary>
        public string ToEntityCodeAlpha { get; set; }

        /// <summary>
        /// Property to get and set To alpha entity code
        /// </summary>
        public string ToEntityCodeNumeric { get; set; }

        /// <summary>
        /// Property to get and set Entity name
        /// </summary>
        public string ToEntityName { get; set;}

        /// <summary>
        /// Property to get and set From Entity
        /// </summary>
        public string ToEntity { get; set; }

        /// <summary>
        /// Property to get and set Corr. Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Property to get and set Sub Status
        /// </summary>
        public string SubStatus { get; set; }

        /// <summary>
        /// Property to get and set Ref. No.
        /// </summary>
        public Int64 RefrenceNo { get; set; }

        public string RefNo
        {
            get { return "0" + Convert.ToString(RefrenceNo); }
            
        }
        /// <summary>
        /// Property to get and set Corr. Date
        /// </summary>
        public DateTime CorrDate { get; set; }

        /// <summary>
        /// Property to get and set Corr. Stage
        /// </summary>
        public int Stage { get; set; }

        /// <summary>
        /// Property to get and set Rejection Memo
        /// </summary>
        public string RejectionMemo { get; set; }

        /// <summary>
        /// Property to get and set Invoice Rejection Memo
        /// </summary>
        public string InvoiceRejectionMemo { get; set; }

        /// <summary>
        /// Property to get and set Rejection Period
        /// </summary>
        public string RejectionPeriod { get; set; }

        /// <summary>
        /// Property to get and set Currency code
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Property to get and set Ammount
        /// </summary>
        public decimal Ammount { get; set; }

        /// <summary>
        /// Property to get and set Authority
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Property to get and set NoOfAttachment
        /// </summary>
        public int NoOfAttachment { get; set; }

        /// <summary>
        /// Property to get and set Expiry date
        /// </summary>
        public DateTime? ExpirryDate { get; set; }

        /// <summary>
        /// Property to get and set Authority
        /// </summary>
        public string IsAuthorise { get; set; }

        /// <summary>
        /// Property to get and set chargecategory
        /// </summary>
        public string ChargeCategory { get; set; }

        /// <summary>
        /// property to get and set time on report
        /// </summary>
        public string ReportGeneratedDate { get { return string.Format("{0:dd-MMM-yyyy HH:mm:ss tt} {1}", DateTime.UtcNow, "UTC"); } }

        //CMP526 - Passenger Correspondence Identifiable by Source Code
        public int SourceCode { get; set; }

    }// End CorrespondenceStatusModel
}// End namespace

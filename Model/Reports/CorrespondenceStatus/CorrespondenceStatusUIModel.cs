using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.CorrespondenceStatus
{
 
    public class CorrespondenceStatusUIModel
    {
        /// <summary>
        /// Get and set from date 
        /// </summary>
        public DateTime Fromdate { get; set;}

        /// <summary>
        /// get and set to date
        /// </summary>
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Get and set member id of the correspondence initiating member
        /// </summary>
        public int? Initimember { get; set;}

        /// <summary>
        /// Get and set member code
        /// </summary>
        public string MemberCode { get; set; }

        /// <summary>
        /// Get and set to member code
        /// </summary>
        public string NextmemberCode { get; set;}

        /// <summary>
        /// Get and set From memberId
        /// </summary>
        public int MemberId { get; set;}

        /// <summary>
        /// Get and set To memberId
        /// </summary>
        public int NextmemberId { get; set; }

        /// <summary>
        /// Get and Set Authority to bill cases
        /// </summary>
        public bool IsAuthorityToBillCase { get; set; }

        /// <summary>
        /// Get and Set Correspondence status
        /// </summary>
        public int CorrespondenceStatusId { get; set; }

        /// <summary>
        /// Get and set Correspondence sub status
        /// </summary>
        public int CorrespondenceSubStatusId { get; set; }

        /// <summary>
        /// Get and set Correspondence stages
        /// </summary>
        public int Corrstage { get; set; }

        /// <summary>
        /// Get and set Expiry days in no.
        /// </summary>
        public int Expiryindays { get; set; }

        /// <summary>
        /// Get and set From memberId
        /// </summary>
        public int RadioMemberId { get; set; }

        /// <summary>
        /// Get and set Corr. refrence no.
        /// </summary>
        public Int64 CorrespondenceNumber { get; set;}

        /// <summary>
        /// Get and set charge category
        /// </summary>
        public int ChargeCategory { get; set; }

        //CMP526 - Passenger Correspondence Identifiable by Source Code
        public int? SourceCode { get; set; }

     
    }// End CorrespondenceStatusModel
}// End namespace

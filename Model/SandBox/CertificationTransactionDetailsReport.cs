using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Model.SandBox
{
    public class CertificationTransactionDetailsReport : EntityBase<int>
    {
        public int SerialNo { get; set; }
        public string MemberName { get; set; }
        public DateTime FileSubmittedFromdate { get; set; }

        public DateTime FileSubmittedToDate { get; set; }

        public string MemberCommercialName { get; set; }
        public string MemberCodeNumeric { get; set; }
        public string MemberCodeAlpha { get; set; }

        public string MemberCode { get; set; }

        public int MemberId { get; set; }
        public DateTime FileSubmittedDate { get; set; }



        public Guid FileId { get; set; }

        public string FileName { get; set; }



        public string RequestType { get; set; }

        public int BillingCategoryId { get; set; }

        public string BillingCategoryName { get; set; }

        public int FileFormatId { get; set; }

        public string FileFormatName { get; set; }

        public string FileStatus { get; set; }

        public int TransactionGroupId { get; set; }

        public string TransactionGroupName { get; set; }

        public string TransactionGroupStatus { get; set; }

        public int MinTransactionCount { get; set; }

        public int TotalTransactionCount { get; set; }

        public int TransactionSubmittedCount { get; set; }

        public string TransactionSubType1Label { get; set; }

        public int TransactionSubType1MinCount { get; set; }

        public int TransactionSubType1RecivedCount { get; set; }

        public string TransactionSubType1Status { get; set; }

        public string TransactionSubType2Label { get; set; }

        public int TransactionSubType2MinCount { get; set; }

        public int TransactionSubType2ReceivedCount { get; set; }

        public string TransactionSubType2Status { get; set; }

        public string TransactionSubType3Label { get; set; }

        public int TransactionSubType3MinCount { get; set; }

        public int TransactionSubType3ReceivedCount { get; set; }

        public string TransactionSubType3Status { get; set; }

        public string TransactionSubType4Label { get; set; }

        public int TransactionSubType4MinCount { get; set; }

        public int TransactionSubType4ReceivedCount { get; set; }

        public string TransactionSubType4Status { get; set; }

        public string TransactionSubType5Label { get; set; }

        public int TransactionSubType5MinCount { get; set; }

        public int TransactionSubType5ReceivedCount { get; set; }

        public string TransactionSubType5Status { get; set; }


        public string TransactionSubType6Label { get; set; }

        public int TransactionSubType6MinCount { get; set; }

        public int TransactionSubType6ReceivedCount { get; set; }

        public string TransactionSubType6Status { get; set; }

        public string TransactionSubType7Label { get; set; }

        public int TransactionSubType7MinCount { get; set; }

        public int TransactionSubType7ReceivedCount { get; set; }

        public string TransactionSubType7Status { get; set; }

        //public IsInputFile IsFileLogTransaction { get; set; }

        // public TransactionType TransactionTypeTransaction { get; set; }

    }
}

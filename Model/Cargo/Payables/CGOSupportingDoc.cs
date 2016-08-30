using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.SupportingDocuments.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Cargo.Payables
{
    public class CGOSupportingDoc : EntityBase<Guid>
    {
        public string BillingPeriod { get; set; }
        //public int BillingPeriod { get; set; }
        public int SupportingBillingCodeId { get; set; }
        public Cargo.Enums.BillingCode BillingCode { get; set; }
        /*{
            get
            {
                return (Cargo.Enums.BillingCode)SupportingBillingCodeId;
            }
            set 
            { 
                Cargo.Enums.BillingCode = (Cargo.Enums.BillingCode)value; 
            }
        }*/

        //public Cargo.Enums.BillingCode BillingCode { get; set; }
        public string SupportingBillingCodeText
        {
            get
            {
                if (BillingCode == Cargo.Enums.BillingCode.BillingMemo)
                {
                    return Enum.GetName(typeof(Cargo.Enums.BillingCode), BillingCode);
                }
                return BillingCode.ToString();
            }
        }
        public int BilledMemberId { get; set; }

        // Used in AutoPopulate
        public string BilledMemberText { set; get; }

        public int BillingMemberId { get; set; }

        // Used in AutoPopulate
        public string BillingMemberText { set; get; }

        public string InvoiceNumber { get; set; }

        //public int SourceCodeId { get; set; }
        public int? SourceCodeId { get; set; }
        //public int BatchSequenceNumber { get; set; }
        public int? BatchSequenceNumber { get; set; }
        //public int RecordSequenceWithinBatch { get; set; }
        public int? RecordSequenceWithinBatch { get; set; }

        public string RMBMCMNumber { get; set; }
        //public long AWBNumber { get; set; }
        public long? AWBNumber { get; set; }
        public bool AttachmentIndicatorValidated { get; set; }
        //public int AttachmentIndicatorValidated { get; set; }
        public string AttachmentIndicatorValidatedText
        {
            get
            {
                if (AttachmentIndicatorValidated)
                    return SupportingDocAttachmentIndicator.Yes.ToString();
                else
                    return SupportingDocAttachmentIndicator.No.ToString();
            }
        }
        public int AttachmentNumber { get; set; }
        public Guid RecordId { get; set; }
        public Guid InvoiceId { get; set; }
        public int RecordType { get; set; }
        public SupportingDocRecordType TypeofRecord
        {
            get
            {
                return (SupportingDocRecordType)RecordType;
            }
        }
        public string ChargeCategory { get; set; }
        public int BillingYear { get; set; }
        public int BillingMonth { get; set; }
        public SupportingDocType Type { get; set; }
        public int SupportingDocumentTypeId { get; set; }
        public int AttachmentIndicatorOriginal { get; set; }
        public bool IsMismatchCases { get; set; }
        public string CutOffDateEventName { get; set; }
        public CGOSupportingDoc(string cutOffDateEventName)
        {
            CutOffDateEventName = cutOffDateEventName;
        }
        public int ChargeCategoryId { get; set; }
        public CGOSupportingDoc()
        {
        }
    }
}
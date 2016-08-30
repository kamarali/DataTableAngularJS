using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using SubmissionMethod = Iata.IS.Model.Enums.SubmissionMethod;

namespace Iata.IS.Model.Cargo
{
    [Serializable]
    public class PayableSearch
    {
        public int BillingYear { get; set; }
        public int BillingMonth { get; set; }
        public int BillingPeriod { get; set; }
        public string BillingCode { get; set; }
        public string BillingMember { get; set; }
        public int InvoiceNoteNumber { get; set; }
        public int SMI { get; set; }
        public string ListingCurrency { get; set; }
        public float ListingAmount { get; set; }
        public float ExchangeRate { get; set; }
        public string BillingCurrency { get; set; }
        public float BillingAmount { get; set; }
        public int BillingMemberId { set; get; }
        public int OwnerId { get; set; }
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public int InvoiceStatusId { set; get; }
        public int BilledMemberId { set; get; }
        public string FileName { set; get; }

        public string BilledMemberText { set; get; }
        public int SubmissionMethodId { set; get; }
        public SubmissionMethod SubmissionMethod
        {
            set
            {
                SubmissionMethodId = Convert.ToInt32(value);
            }
            get
            {
                return (SubmissionMethod)SubmissionMethodId;
            }
        }
        public SMI InvoiceSmi
        {
            get
            {
                return (SMI)SMI;
            }
            set
            {
                SMI = Convert.ToInt32(value);
            }
        }

        public InvoiceStatusType InvoiceStatus
        {
            get
            {
                return (InvoiceStatusType)InvoiceStatusId;
            }
            set
            {
                InvoiceStatusId = Convert.ToInt32(value);
            }
        }
        public string BillingMemberText { get; set; }

    }


}

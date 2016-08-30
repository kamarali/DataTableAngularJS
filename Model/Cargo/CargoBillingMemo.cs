using System;
using System.Collections.Generic;
using Iata.IS.Model.Cargo.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Cargo
{

    public class CargoBillingMemo : MemoBase
    {
        public CargoBillingMemo()
        {
            AwbBreakdownRecord = new List<CargoBillingMemoAwb>();
            Attachments = new List<CargoBillingMemoAttachment>();
            BillingMemoVat = new List<CargoBillingMemoVat>();
        }

        public string BillingMemoNumber { get; set; }

        public string ReasonCode { get; set; }

        public long CorrespondenceReferenceNumber { get; set; }

        public decimal? BilledTotalWeightCharge { get; set; }

        public decimal? BilledTotalValuationAmount { get; set; }

        public decimal BilledTotalOtherChargeAmount { get; set; }

        public decimal BilledTotalIscAmount { get; set; }

        public decimal? BilledTotalVatAmount { get; set; }

        public decimal? NetBilledAmount { get; set; }

        public List<CargoBillingMemoAttachment> Attachments { get; set; }

        public List<CargoBillingMemoAwb> AwbBreakdownRecord { get; set; }

        public List<CargoBillingMemoVat> BillingMemoVat { get; set; }

        /// <summary>
        /// ErrorCorrectable = 1, ErrorNonCorrectable = 2,Validated = 3
        /// </summary>
        public int TransactionStatusId { set; get; }

        public TransactionStatus TransactionStatus
        {
          get
          {
            return (TransactionStatus)TransactionStatusId;
          }
          set
          {
            TransactionStatusId = Convert.ToInt32(value);
          }
        }

        private string _resonCodeDesription = string.Empty;
        public string ReasonCodeDescription
        {
            get
            {
                return _resonCodeDesription;
            }
            set
            {
                _resonCodeDesription = value;
            }
        }
        public bool AwbBreakdownMandatory { get; set; }
    }
}

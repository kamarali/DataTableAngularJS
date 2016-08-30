﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Reports.PayablesReport
{
    public class PayablesReportModel
    {
        public int FromPeriod { get; set; }

        public int FromBillingYear { get; set; }

        public int FromBillingMonth { get; set; }
        public string BillingMonth { get; set; }

        public string SettlementMethod { get; set; }

        public string BillingEntityCode { get; set; }
        public int BillingEntityCodeId { get; set; }
        public int SubmissionMethodId { set; get; }
        public string InvoiceNo { get; set; }

        public string MemoType { get; set; }// RM/CM/BM No

        public string MemoNo { get; set; }

        public int Stage { get; set; }

        public string ReasonCode { get; set; }

        public string CurrencyCode { get; set; }

        public decimal TotalGrossAmt { get; set; }

        public decimal TotalUATPAmt { get; set; }

        public decimal TotalHandlingFeeAmt { get; set; }

        public decimal TotalOtherCommAmt { get; set; }

        public decimal TotalISCAmount { get; set; }

        public decimal TotalVatAmount { get; set; }

        public decimal TotalTaxAmount { get; set; }

        public decimal TotalNetAmount { get; set; }

        public string AttachmentIndicator { get; set; }

        public int NoofLinkCpns { get; set; }

        public string RMBMCMNo { get; set; }

        //CMP-523: Source Code in Passenger RM BM CM Summary Reports
        public int SourceCode { get; set; }

        //public string DataSource { get; set; }
    }
}

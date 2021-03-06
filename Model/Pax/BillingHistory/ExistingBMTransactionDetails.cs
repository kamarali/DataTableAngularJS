﻿using System.Collections.Generic;

namespace Iata.IS.Model.Pax.BillingHistory
{
    //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    //Desc: Class added to return existing BM details from cursor.

    public class ExistingBMTransactionDetails
    {
        public List<ExistingBMTransaction> Transactions { get; set; }
        public bool IsTransactionOutsideTimeLimit { get; set; }
    }

    public class ExistingBMTransaction
    {
        /// <summary>
        /// Memo number in which this transaction was rejected.
        /// </summary>
        public string BillingMemoNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoicePeriod { get; set; }
        public string InvoiceStatus { get; set; }
        public string BatchNumber { get; set; }
        public string SequenceNumber { get; set; }
    }
}
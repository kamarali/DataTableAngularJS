using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    public class DuplicateInvRMLog : EntityBase<Guid>
    {
        public Guid InvoiceId
        {
            get;
            set;
        }
        public Guid IsFileLogId
        {
            get;
            set;
        }
        public string CsvProcessId
        {
            get;
            set;
        }
        public string RejectionMemoNo
        {
            get;
            set;
        }
        public string RejectedInvoiceNo
        {
            get;
            set;
        }

        public int SettlementYear
        {
            get;
            set;
        }

        public int BillingCategoryId
        {
            get;
            set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    public class DuplicateCorrBMLog : EntityBase<Guid>
    {
        public Guid InvoiceId { get; set; }
        public Guid IsFileLogId { get; set; }
        public string CsvProcessId { get; set; }
        public string BillingMemoNumber { get; set; }
        public long CorrespondenceRefNumber { get; set; }
        public int BillingCategoryId { get; set; }

        /* SCP# 400076 - Validation Report for AUG 15 Per1
        Desc: Added support to preserve source code of BM. This will then be pushed in R2 Val Report. */
        public int? SourceCode { get; set; }
    }
}

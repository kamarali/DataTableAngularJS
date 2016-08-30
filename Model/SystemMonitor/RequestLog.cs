using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.SystemMonitor
{
    [Serializable]
    public class RequestLog: EntityBase<Guid>
    {
       
        public int BillingYear { get; set; }

        public int BillingMonth { get; set; }

        public int BillingPeriod { get; set; }

        public int BillingTypeId { get; set; }

        public string ActionType { get; set; }

        public int MemberId { get; set; }

        public int BillingCategoryId { get; set; }

        public int FileTypeId { get; set; }

        public string InvoiceIds { get; set; }

        public string Remarks { get; set; }

        public int Stage { get; set; }

        public string FileGenerationDate { get; set; }
    }
}

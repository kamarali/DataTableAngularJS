using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.InterlinePayablesAnalysis
{
    public class InterlineBillingSummaryReportResultModel
    {
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public decimal PaxOutAccept { get; set; }
        public decimal UatpOutAccept { get; set; }
        public decimal CargoOutAccept { get; set; }
        public decimal MiscOutAccept { get; set; }
        public decimal PaxOutNotAccept { get; set; }
        public decimal UatpOutNotAccept { get; set; }
        public decimal CargoOutNotAccept { get; set; }
        public decimal MiscOutNotAccept { get; set; }
        public decimal PaxInAccept { get; set; }
        public decimal UatpInAccept { get; set; }
        public decimal CargoInAccept { get; set; }
        public decimal MiscInAccept { get; set; }
    }
}

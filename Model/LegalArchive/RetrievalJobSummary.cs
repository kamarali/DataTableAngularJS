using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.LegalArchive
{
    public class RetrievalJobSummary :  EntityBase<Guid>
    {
        public string Arcretrievaljobid { get; set; }
        public int Totalnoinvoicesretrieved { get; set; }
        public string Arccustdesignator { get; set; }
        public string Arccustaccounting { get; set; }
        public int Arccusisid { get; set; }
        public DateTime Requestedon { get; set; }
        public string Requestedby { get; set; }
        public string Jobstatus { get; set; }
        public string Invoicenumber { get; set; }
        public int Itype { get; set; }
        public int Billingyear { get; set; }
        public int Billingmonth { get; set; }
        public int Billingperiod { get; set; }
        public string Member { get; set; }
        public int Billingcategory { get; set; }
        public string Billinglocationcountry { get; set; }
        public string Billedlocationcountry { get; set; }
        public int Settlementmethod { get; set; }
        public DateTime? Expirydatepurging { get; set; }
        public int Lastupdatedby { get; set; }
        public DateTime Lastupdatedon { get; set; }
        public List<RetrievalJobDetails> JobDetails { get; set; }
        public string MiscLocationCodes { get; set; }
    }
}

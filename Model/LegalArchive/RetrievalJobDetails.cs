using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.LegalArchive
{
    public class RetrievalJobDetails :  EntityBase<Guid>
    {
        public Guid RetrievalJobSummaryId { get; set; }
        //public string Arcretrievaljobid { get; set; }
        public string Arccustdesignator { get; set; }
        public string Arccustaccounting { get; set; }
        public int Arccustisid { get; set; }
        public string Crbillingmemberdesignator { get; set; }
        public string Crbillingmemberaccounting { get; set; }
        public int Crbillingmemberisid { get; set; }
        public string Dbbilledmemberdesignator { get; set; }
        public string Dbbilledmemberaccounting { get; set; }
        public int Dbbilledmemberisid { get; set; }
        public int Billingcategory { get; set; }
        public int Receivablespayablesindicator { get; set; }
        public string Invoicenumber { get; set; }
        public Guid Invoiceid { get; set; }
        public string Invoicetype { get; set; }
        public int Billingyear { get; set; }
        public int Billingmonth { get; set; }
        public int Billingperiod { get; set; }
        public DateTime Invoicedate { get; set; }
        public int Settlementindicator { get; set; }
        public string Crbillingmembercountry { get; set; }
        public string Dbbilledmembercountry { get; set; }
        public string Iua { get; set; }
        public DateTime Initialendfile { get; set; }
        public string Retrivalstatus { get; set; }
        public DateTime Retrivalrequestdatetime { get; set; }
        public string Webserviceresponsecodetext { get; set; }
        public string Invzippedfileretrived { get; set; }
        public string Invretrivedzippedlocation { get; set; }
        public int Lastupdatedby { get; set; }
        public DateTime Lastupdatedon { get; set; }

        //CMP-666-MISC Legal Archiving Per Location ID
        public string MiscLocationsCode { get; set; }
    }
}

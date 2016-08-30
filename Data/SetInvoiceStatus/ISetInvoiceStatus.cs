using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.SetInvoiceStatus
{
   public  interface ISetInvoiceStatus
   {
       void SetStatusOfInvoices(string invoiceIds, string status, string typeOfService, DateTime _submissionDate);
   }
}

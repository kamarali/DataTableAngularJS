using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.SetInvoiceStatus.Impl
{
    public class SetInvoiceStatus : Repository<InvoiceBase>, ISetInvoiceStatus 
    {
        public void SetStatusOfInvoices(string invoiceIds, string status, string typeOfService, DateTime submissionDate)
        {
            var parameters = new ObjectParameter[4];
            parameters[0] = new ObjectParameter(SetInvoiceStatusConstants.InvoiceIds, invoiceIds);
            parameters[1] = new ObjectParameter(SetInvoiceStatusConstants.Status, status);
            parameters[2] = new ObjectParameter(SetInvoiceStatusConstants.TypeOfServices, typeOfService);
            parameters[3] = new ObjectParameter(SetInvoiceStatusConstants.StatusUpdateTime, submissionDate);
            ExecuteStoredProcedure(SetInvoiceStatusConstants.SetInvoiceStatus, parameters);
        }
    }
}

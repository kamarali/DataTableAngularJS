using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Core;
using Iata.IS.Model.AutoBillingClosure;
using System.Data.Objects;
using Iata.IS.Model.Base;
using Iata.IS.Data.Impl;

namespace Iata.IS.Data.AutoBillingClosure.Impl
{
    class GetAutoBillingInovices : Repository<InvoiceBase>, IGetAutoBillingInovices
    {
        private const string BillingYear = "BILLING_YEAR_I";
        private const string BillingMonth = "BILLING_MONTH_I";
        private const string BillingPeriod = "BILLING_PERIOD_I";
        private const string InvoiceId = "INVOICE_ID_I";

        private const string GetInvoices = "GetAutoBillingInvoices";
        private const string UpdateCoupons = "UpdateAutoBillingCoupons";
        private const string DeleteInvoice = "DeleteAutoBillingInvoice";

       

        public List<AutoBillingInvoice> GetAutoBillingOpenInvoice(int billingYear, int billingMonth, int period)
        {
            var parameters = new ObjectParameter[3];
            parameters[0] = new ObjectParameter(BillingYear, billingYear);
            parameters[1] = new ObjectParameter(BillingMonth, billingMonth);
            parameters[2] = new ObjectParameter(BillingPeriod, period);

            var list = ExecuteStoredFunction<AutoBillingInvoice>(GetInvoices, parameters) as IEnumerable<AutoBillingInvoice>;

            return list.ToList();
        }


        public void UpdateAutoBillingCoupons(Guid invoiceId)
        {
            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter(InvoiceId, typeof(string)) { Value = ConvertUtil.ConvertGuidToString(invoiceId) };

            ExecuteStoredProcedure(UpdateCoupons, parameters);
        }

        public void DeleteAutoBillingInvoice(Guid invoiceId)
        {
            var parameters = new ObjectParameter[1];
            parameters[0] = new ObjectParameter(InvoiceId, typeof(string)) { Value = ConvertUtil.ConvertGuidToString(invoiceId) };

            ExecuteStoredProcedure(DeleteInvoice, parameters);
        }


        //public List<AutoBillingInvoice> UpdateInvoiceStatus(int billingYear, int billingMonth, int period)
        //{
        //    var parameters = new ObjectParameter[3];
        //    parameters[0] = new ObjectParameter(billing_year, billingYear);
        //    parameters[1] = new ObjectParameter(billing_month, billingMonth);
        //    parameters[2] = new ObjectParameter(billing_period, period);

        //    var list = ExecuteStoredFunction<AutoBillingInvoice>(getInvoices, parameters) as IEnumerable<AutoBillingInvoice>;

        //    return list.ToList();
        //}
    }
}

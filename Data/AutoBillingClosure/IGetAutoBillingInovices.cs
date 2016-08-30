using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.AutoBillingClosure;

namespace Iata.IS.Data.AutoBillingClosure
{
    public interface IGetAutoBillingInovices
    {
        List<AutoBillingInvoice> GetAutoBillingOpenInvoice(int billingYear, int billingMonth, int period);

        void UpdateAutoBillingCoupons(Guid invoiceId);

        void DeleteAutoBillingInvoice(Guid invoiceId);
    }
}

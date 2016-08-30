using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;

namespace Iata.IS.Data.Pax.Impl
{
    /// <summary>
    /// SCP - 85037: Return Invoice details based on params
    /// </summary>
    public class ManagePaxInvoiceRepository : Repository<PaxInvoiceSearch>, IManagePaxInvoiceRepository
    {
        public List<PaxInvoiceSearchDetails> GetPaxManageInvoices(SearchCriteria searchCriteria, int pageSize, int pageNo,string sortColumn, string sortOrder)//int billingMemberId, int billingMonth, int billingYear)
        {
            var parameters = new ObjectParameter[16];
            parameters[0] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value =searchCriteria.BillingMemberId};
            parameters[1] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = searchCriteria.BillingMonth };
            parameters[2] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = searchCriteria.BillingYear };

            parameters[3] = new ObjectParameter("INVOICE_STAUS_ID_I", typeof(int)) { Value = searchCriteria.InvoiceStatusId };
            parameters[4] = new ObjectParameter("BILLING_PERIOD_I", typeof(int)) { Value = searchCriteria.BillingPeriod };
            parameters[5] = new ObjectParameter("BILLING_CODE_ID_I", typeof(int)) { Value = searchCriteria.BillingCode };
            parameters[6] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = searchCriteria.BilledMemberId };
            parameters[7] = new ObjectParameter("SETTELMENT_METHOD_ID_I", typeof(int)) { Value = searchCriteria.SettlementMethodId };
            parameters[8] = new ObjectParameter("SUBMISSION_METHOD_ID_I", typeof(int)) { Value = searchCriteria.SubmissionMethodId };
            parameters[9] = new ObjectParameter("OWNER_ID_I", typeof(int)) { Value = searchCriteria.OwnerId };
            parameters[10] = new ObjectParameter("PAGE_SIZE_I", typeof(int)) { Value = pageSize };
            parameters[11] = new ObjectParameter("FILENAME_I", typeof(string)) { Value = searchCriteria.FileName };
            parameters[12] = new ObjectParameter("INVOICE_NO_I", typeof(string)) { Value = searchCriteria.InvoiceNumber };
                        
            parameters[13] = new ObjectParameter("PAGE_NO_I", typeof(int)) { Value = pageNo };

            parameters[14] = new ObjectParameter("SORT_COLUMN_I", typeof(string)) { Value = sortColumn };
            parameters[15] = new ObjectParameter("SORT_ORDER_I", typeof(string)) { Value = sortOrder };

            return ExecuteStoredFunction<PaxInvoiceSearchDetails>("GetPaxManageInvoices", parameters).ToList();
        }
    }
}

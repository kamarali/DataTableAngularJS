using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports;

namespace Iata.IS.Data.Reports
{
  public interface IManageSuspendedInvoicesRepository
  {
    List<SuspendedInvoiceDetails> GetSuspendedInvoiceList(int billingMemberId, int fromBillingMonth, int toBillingMonth,
                                                          int fromBilingPeriod, int toBillingPeriod,
                                                          int settlementMethodIndicator, int resubmissionStatus,
                                                          int billedEntityId, int fromBillingYear, int toBillingYear);

    /// <summary>
    /// Gets the member suspended invoices list.
    /// </summary>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="fromClearanceYear">From clearance year.</param>
    /// <param name="fromClearanceMonth">From clearance month.</param>
    /// <param name="fromClearancePeriod">From clearance period.</param>
    /// <param name="toClearanceYear">To clearance year.</param>
    /// <param name="toClearanceMonth">To clearance month.</param>
    /// <param name="toClearancePeriod">To clearance period.</param>
    /// <param name="settlementMethodIndicatorId">The settlement method indicator id.</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <param name="suspendedEntityCode">The suspended entity code.</param>
    /// <returns></returns>
    List<MemberSuspendedInvoices> GetMemberSuspendedInvoicesList(int billingMemberId, int fromClearanceYear, int fromClearanceMonth, int fromClearancePeriod, int toClearanceYear, int toClearanceMonth, int toClearancePeriod, int settlementMethodIndicatorId, int billingCategoryId, int suspendedEntityCode, int iataMemberId, int achMemberId);

  }
}

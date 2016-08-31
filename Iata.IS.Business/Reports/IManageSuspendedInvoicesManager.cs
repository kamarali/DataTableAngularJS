using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports;

namespace Iata.IS.Business.Reports
{
  public interface IManageSuspendedInvoicesManager
  {
    List<SuspendedInvoiceDetails> GetSuspendedInvoiceList(int billingMemberId, int fromBillingMonth, int toBillingMonth,
                                                          int fromBillingPeriod, int toBillingPeriod, int smi,
                                                          int resubmissionStatus, int billedEntityId, int fromBillingYear, int toBillingYear);

    bool UpdateInvoiceRemark(Guid invoiceId, string remark);

    InvoiceBase GetInvoice(Guid invoiceId);

    bool MarkInvoicesAsResubmitted(List<string> invoiceIdList, bool resubmitInIchPreviousPeriod,bool resubmitInAchPreviousPeriod);

    bool MarkInvoicesAsBilaterallySettled(List<string> invoiceIdList);

    bool UndoBilateral(List<string> invoiceIdList);

   // bool CheckIfLateSubmissionWindowOpen(int clearingHouse);
    bool[] CheckIfLateSubmissionWindowOpen(List<string> invoiceIdList);

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
    /// <param name="iataMemberId">The iata member id.</param>
    /// <param name="achMemberId">The ach member id.</param>
    /// <returns></returns>
    List<MemberSuspendedInvoices> GetMemberSuspendedInvoicesList(int billingMemberId, int fromClearanceYear, int fromClearanceMonth, int fromClearancePeriod, int toClearanceYear, int toClearanceMonth, int toClearancePeriod, int settlementMethodIndicatorId, int billingCategoryId, int suspendedEntityCode, int iataMemberId, int achMemberId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    int? GetInvoiceResubmissionStatus(Guid invoiceId);

  }
}

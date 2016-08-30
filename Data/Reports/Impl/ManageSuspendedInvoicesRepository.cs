using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports;

namespace Iata.IS.Data.Reports.Impl
{
  public class ManageSuspendedInvoicesRepository :Repository<InvoiceBase> ,IManageSuspendedInvoicesRepository
  {
    public List<SuspendedInvoiceDetails> GetSuspendedInvoiceList(int billingMemberId, int fromBillingMonth, int toBillingMonth, int fromBilingPeriod, int toBillingPeriod, int settlementMethodIndicator, int resubmissionStatus,int billedEntityId,int fromBillingYear,int toBillingYear)
    {
      var parameters = new ObjectParameter[10];
      parameters[0] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.BillingEntityId, typeof(Int32)) { Value = billingMemberId };
      parameters[1] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.FromBillingMonth, typeof(Int32)) { Value = fromBillingMonth };
      parameters[2] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.ToBillingMonth, typeof(Int32)) { Value = toBillingMonth };
      parameters[3] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.FromBillingPeriod, typeof(Int32)) { Value = fromBilingPeriod };
      parameters[4] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.ToBillingPeriod, typeof(Int32)) { Value = toBillingPeriod };
      parameters[5] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.SettelementMethodIndicator, typeof(String)) { Value = settlementMethodIndicator };
      parameters[6] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.ResubmissionStatus, typeof(String)) { Value = resubmissionStatus };
      parameters[7] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.BilledentityId, typeof(String)) { Value = billedEntityId };
      parameters[8] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.FromBillingYear, typeof(Int32)) { Value = fromBillingYear };
      parameters[9] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.ToBillingYear, typeof(Int32)) { Value = toBillingYear };
     
      
      var list = ExecuteStoredFunction<SuspendedInvoiceDetails>(ManageSuspendedInvoicesRepositoryConstants.GetSuspendedInvoiceDetails, parameters);
      return list.ToList();
    }

    public List<MemberSuspendedInvoices> GetMemberSuspendedInvoicesList(int billingMemberId, int fromClearanceYear, int fromClearanceMonth, int fromClearancePeriod, int toClearanceYear, int toClearanceMonth, int toClearancePeriod, int settlementMethodIndicatorId, int billingCategoryId, int suspendedEntityCode, int iataMemberId, int achMemberId)
    {
        var parameters = new ObjectParameter[12];
        parameters[0] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.BillingMemberId, typeof(Int32)) { Value = billingMemberId };
        parameters[1] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.FromClearanceYear, typeof(Int32)) { Value = fromClearanceYear };
        parameters[2] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.FromClearanceMonth, typeof(Int32)) { Value = fromClearanceMonth };
        parameters[3] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.FromClearancePeriod, typeof(Int32)) { Value = fromClearancePeriod };
        parameters[4] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.ToClearanceYear, typeof(Int32)) { Value = toClearanceYear };
        parameters[5] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.ToClearanceMonth, typeof(Int32)) { Value = toClearanceMonth };
        parameters[6] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.ToClearancePeriod, typeof(Int32)) { Value = toClearancePeriod };
        parameters[7] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.SettlementMethodIndicatorId, typeof(Int32)) { Value = settlementMethodIndicatorId };
        parameters[8] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.SuspendedEntityCode, typeof(Int32)) { Value = suspendedEntityCode };
        parameters[9] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.BillingCategoryId, typeof(Int32)) { Value = billingCategoryId };
        parameters[10] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.IATAMemberId, typeof(Int32)) { Value = iataMemberId };
        parameters[11] = new ObjectParameter(ManageSuspendedInvoicesRepositoryConstants.ACTMemberId, typeof(Int32)) { Value = achMemberId };

        var list = ExecuteStoredFunction<MemberSuspendedInvoices>(ManageSuspendedInvoicesRepositoryConstants.GetMemberSuspendedInvoices, parameters);
        return list.ToList();
    }
  }
}

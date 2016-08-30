using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports;

namespace Iata.IS.Data.Reports.Impl
{
  public class ProcessingDashboardInvoiceStatusRepository : Repository<InvoiceBase>, IProcessingDashboardInvoiceStatusRepository
  {
    public List<ProcessingDashboardInvoiceStatusResultSet> GetInvoiceStatusResultForProcDashBrd(ProcessingDashboardSearchEntity searchcriteria)
    {
      //CMP559 : Add Submission Method Column to Processing Dashboard
      var parameters = new ObjectParameter[15];
      parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.BillingYear, typeof(Int32)) { Value = searchcriteria.BillingYear };
      parameters[1] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.BillingMonth, typeof(Int32)) { Value = searchcriteria.BillingMonth };
      parameters[2] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.BillingPeriod, typeof(Int32)) { Value = searchcriteria.BillingPeriod };
      parameters[3] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.BillingMemberId, typeof(Int32)) { Value = searchcriteria.BillingMemberId };
      parameters[4] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.BilledMemberId, typeof(Int32)) { Value = searchcriteria.BilledMemberId };
      parameters[5] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.BillingCategoryId, typeof(Int32)) { Value = searchcriteria.BillingCategory };
      parameters[6] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.InvoiceStatusId, typeof(Int32)) { Value = searchcriteria.InvoiceStatus };
      parameters[7] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.SettlementMethodId, typeof(Int32)) { Value = searchcriteria.SettlementMethod };
      parameters[8] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.IsUserId, typeof(Int32)) { Value = searchcriteria.IsUserId };
      parameters[9] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.IsFileName, typeof(Int32)) { Value = searchcriteria.FileName };
      parameters[10] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.IsInvoiceNo, typeof(string)) {Value = searchcriteria.InvoiceNo};
      parameters[11] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.UniqueInvoiceNo, typeof(string)) { Value = searchcriteria.UniqueInvoiceNo };
      parameters[12] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.IsShowClaimFailed, typeof(Int32)) { Value = searchcriteria.IsShowClaimFailed };
      //CMP559 : Add Submission Method Column to Processing Dashboard
      parameters[13] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.SubmissionMethodId, typeof(Int32)) { Value = searchcriteria.SubmissionMethodId };
      //CMP529 : Daily Output Generation for MISC Bilateral Invoices
      parameters[14] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.DailyDeliveryStatusId, typeof (Int32)) { Value = searchcriteria.DailyDeliverystatusId };

      var list = ExecuteStoredFunction<ProcessingDashboardInvoiceStatusResultSet>(ProcessingDashboardInvoiceFileStatusRepositoryContants.GetInvoiceStatusSearchResult, parameters);
      return list.ToList();
    }

    /// <summary>
    /// Updates the purging expiry period of transactions under an invoice when invoice is late submitted.
    /// </summary>
    /// <param name="invoiceId">The invoice Id.</param>
    public void UpdatePurgingExpiryPeriod(Guid invoiceId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.InvoiceId.ToUpper(), typeof(Guid)){ Value = invoiceId };

      ExecuteStoredProcedure(ProcessingDashboardInvoiceFileStatusRepositoryContants.SetPurgingExpiryPeriod, parameters);
    }


    /// <summary>
    /// Following method is used to pass parameters to Stored procedure for retrieving Invoice details
    /// </summary>
    /// <param name="invoiceId">InvoiceId whose details are to be retrieved</param>
    /// <returns>Invoice details</returns>
    public ProcessingDashboardInvoiceDetail GetInvoiceDetailsForProcDashBrd(Guid invoiceId)
    {
        // Pass InvoiceId parameter to StoredProcedure
        var parameters = new ObjectParameter[1];
        parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.InvoiceId, typeof(Guid)) { Value = invoiceId };

        // Execute stored procedure
        var invoiceDetails = ExecuteStoredFunction<ProcessingDashboardInvoiceDetail>(ProcessingDashboardInvoiceFileStatusRepositoryContants.GetInvoiceDetailsResult, parameters);
        // return Invoice details
        return invoiceDetails.SingleOrDefault();
    }// end GetInvoiceDetailsForProcDashBrd()
  }
}

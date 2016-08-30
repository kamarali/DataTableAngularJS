using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.LateSubmission;

namespace Iata.IS.Data.LateSubmission.Impl
{
  public class ProcessLateSubmissionRepository : Repository<InvoiceBase>,IProcessLateSubmissionRepository
  {
    public List<LateSubmissionMemberSummary> GetLateSubmittedInvoiceMemberSummary(string clearenceType, BillingPeriod billingPeriod)
    {
      //SCP 280475 - SRM: System Performance - ICH late submisson tab
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.ClearenceType, typeof(int)) { Value = clearenceType.ToUpper() };
      parameters[1] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingYear, typeof(int)) { Value = billingPeriod.Year };
      parameters[2] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingMonth, typeof(int)) { Value = billingPeriod.Month };
      parameters[3] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingPeriod, typeof(int)) { Value = billingPeriod.Period };

      var list = ExecuteStoredFunction<LateSubmissionMemberSummary>(ProcessLateSubmissionRepositoryConstants.GetLateSubMemberDetail, parameters);
      
      //var list = new List<LateSubmittedMemberHeader>();

      //LateSubmittedMemberHeader h = new LateSubmittedMemberHeader();
      //h.CargoBilling = "USD 34,556 + CAD 34,564";
      //h.MemberName = "YYY-678-UUU";
      //h.MiscBilling = "USD 34,556 + CAD 34,564";
      //h.NoOfInvoices = 34;
      //h.PassengerBilling = "USD 34,556 + CAD 34,564";
      //h.UatpBilling = "USD 34,556 + CAD 34,564";
      //list.Add(h);

      //LateSubmittedMemberHeader h1 = new LateSubmittedMemberHeader();
      //h1.CargoBilling = "USD 34,556 <br /> CAD 34,564";
      //h1.MemberName = "KKK-578-UUU";
      //h1.MiscBilling = "USD 34,556 <br /> CAD 34,564";
      //h1.NoOfInvoices = 34;
      //h1.PassengerBilling = "USD 34,556 <br /> CAD 34,564";
      //h1.UatpBilling = "USD 34,556 <br /> CAD 34,564";
      //list.Add(h1);
      
      return list.ToList();

    }

    public List<LateSubmittedInvoices> GetLateSubmittedInvoicesByMemberId(string clearenceType, int memberId, BillingPeriod billingPeriod)
    {
      //Additional Issue SCP 280475 - SRM: System Performance - ICH late submisson tab
      var parameters = new ObjectParameter[5];
     
      parameters[0] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingMemberId, typeof(int)) { Value = memberId };
      parameters[1] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.ClearenceType, typeof(int)) { Value = clearenceType.ToUpper() };
      parameters[2] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingYear, typeof(int)) { Value = billingPeriod.Year };
      parameters[3] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingMonth, typeof(int)) { Value = billingPeriod.Month };
      parameters[4] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingPeriod, typeof(int)) { Value = billingPeriod.Period };
      var list = ExecuteStoredFunction<LateSubmittedInvoices>(ProcessLateSubmissionRepositoryConstants.GetLateSubMemberInvoice, parameters);
      return list.ToList();

    }

    public string AcceptLateSubmittedInvoice(string invoiceIds)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.InvoiceIds, typeof(string)) { Value = invoiceIds };
      parameters[1] = new ObjectParameter("VALID_INVOICE_IDs_o", typeof(string));

     ExecuteStoredProcedure(ProcessLateSubmissionRepositoryConstants.AcceptLateSubmittedInvoice, parameters);
     return Convert.ToString(parameters[1].Value);
      //return String.Empty;
    }

    public List<LateSubmissionRejectInvoices> RejectLateSubmittedInvoice(string invoiceIds)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.InvoiceIds, typeof(string)) { Value = invoiceIds };

      var list = ExecuteStoredFunction<LateSubmissionRejectInvoices>(ProcessLateSubmissionRepositoryConstants.RejectLateSubmittedInvoice, parameters);
      return list.ToList();
    }

    public List<LateSubmissionRejectInvoices> RejectLateSubmittedInvoiceOnLateSubmissionWindowClosing(string clearenceType, BillingPeriod prevBillingDetails)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.ClearenceType, typeof(string)) { Value = clearenceType.ToUpper() };
      parameters[1] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingYear, typeof(int)) { Value = prevBillingDetails.Year };
      parameters[2] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingMonth, typeof(int)) { Value = prevBillingDetails.Month };
      parameters[3] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.BillingPeriod, typeof(int)) { Value = prevBillingDetails.Period };
      var list = ExecuteStoredFunction<LateSubmissionRejectInvoices>(ProcessLateSubmissionRepositoryConstants.RejectLateSubmittedInvoiceOnLateSubClosing, parameters);
      
        List<LateSubmissionRejectInvoices> invoiceList=new List<LateSubmissionRejectInvoices>();

        foreach (var lateSubmissionRejectInvoicese in list)
        {
            invoiceList.Add(lateSubmissionRejectInvoicese);
        }

        return invoiceList;
        //return list.ToList();
    }


    //public void SetInvoiceArchiveParameterForLateSubmission(Guid invoiceId, int currentBillingPeriod)
    //{
    //    var parameters = new ObjectParameter[2];
    //    parameters[0] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.InvoiceId, typeof(Guid)) { Value = invoiceId };
    //    parameters[1] = new ObjectParameter(ProcessLateSubmissionRepositoryConstants.CurrentBillingPeriod, typeof(int)) { Value = currentBillingPeriod };

    //    ExecuteStoredProcedure(ProcessLateSubmissionRepositoryConstants.SetInvoiceLegalParameterForLateSubmission, parameters);
   
    //}
  }
}

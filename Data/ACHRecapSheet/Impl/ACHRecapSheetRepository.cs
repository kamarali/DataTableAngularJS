using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.AchRecapSheet;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using System.Data.Objects;


namespace Iata.IS.Data.ACHRecapSheet.Impl
{
  class ACHRecapSheetRepository: Repository<InvoiceBase>, IACHRecapSheetRepository 
  {
    public List<AchRecapSheetDetail> GetACHRecapSheetData(BillingPeriod billingPeriod, int achRecapSheetRegenerationFleg)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingYear, typeof(Int32)) { Value = billingPeriod.Year };
      parameters[1] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingMonth, typeof(Int32)) { Value = billingPeriod.Month };
      parameters[2] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingPeriod, typeof(Int32)) { Value = billingPeriod.Period };
      parameters[3] = new ObjectParameter(ACHRecapSheetRepositoryContants.ACHRecapSheetRegenerateFlag, typeof(Int32)) { Value = achRecapSheetRegenerationFleg };
      var list = ExecuteStoredFunction<AchRecapSheetDetail>(ACHRecapSheetRepositoryContants.GetACHRecapSheetData, parameters);
      return list.ToList();
    }

    public void UpdateInvoiceACHRecapSheetStatus(BillingPeriod billingPeriod,string invoiceRecapSheetStatus)
    {
        var parameters = new ObjectParameter[4];
        parameters[0] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingYear, typeof(Int32)) { Value = billingPeriod.Year };
        parameters[1] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingMonth, typeof(Int32)) { Value = billingPeriod.Month };
        parameters[2] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingPeriod, typeof(Int32)) { Value = billingPeriod.Period };
        parameters[3] = new ObjectParameter(ACHRecapSheetRepositoryContants.ACHRecapSheetStatus, typeof(string)) { Value = invoiceRecapSheetStatus };
        ExecuteStoredProcedure(ACHRecapSheetRepositoryContants.UpdateInvoiceACHRecapSheetStatus, parameters);
    }
    public void UpdateACHRecapSheetData(BillingPeriod billingPeriod, string FileName, string FileLocation, int ACHMemberId)
    {
      var parameters = new ObjectParameter[6];
      parameters[0] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingYear, typeof(Int32)) { Value = billingPeriod.Year };
      parameters[1] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingMonth, typeof(Int32)) { Value = billingPeriod.Month };
      parameters[2] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingPeriod, typeof(Int32)) { Value = billingPeriod.Period };
      parameters[3] = new ObjectParameter(ACHRecapSheetRepositoryContants.FileName, typeof(string)) { Value = FileName };
      parameters[4] = new ObjectParameter(ACHRecapSheetRepositoryContants.FileLocation, typeof(string)) { Value = FileLocation };
      parameters[5] = new ObjectParameter(ACHRecapSheetRepositoryContants.ACHMemberId, typeof(Int32)) { Value = ACHMemberId };
     
        ExecuteStoredProcedure(ACHRecapSheetRepositoryContants.UpdateACHRecapSheetData, parameters);
    }

    

  }
}

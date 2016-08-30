using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.ValueConfirmation;

namespace Iata.IS.Data.ValueConfirmation.Impl
{
  public class AutoGenerateValueConfmReportRepository :Repository<InvoiceBase>, IAutoGenerateValueConfmReportRepository
  {
    public List<AutoGenerateValueConfirmationReport> GetValueConfirmationReport(BillingPeriod billingPeriod,int memberId)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(ValueConfirmationConstants.MemberId, typeof(Int32)) { Value = memberId };
      parameters[1] = new ObjectParameter(ValueConfirmationConstants.ClearenceMonth, typeof(Int32)) { Value = billingPeriod.Month };
      parameters[2] = new ObjectParameter(ValueConfirmationConstants.ClearenceYear, typeof(Int32)) { Value = billingPeriod.Year };
      parameters[3] = new ObjectParameter(ValueConfirmationConstants.ClearencePeriod, typeof(Int32)) { Value = billingPeriod.Period };

      var list = ExecuteStoredFunction<AutoGenerateValueConfirmationReport>(ValueConfirmationConstants.GetValueConfirmationReportData, parameters);
      return list.ToList();
    }
  }
}

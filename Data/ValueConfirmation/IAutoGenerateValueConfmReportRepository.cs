using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.ValueConfirmation;

namespace Iata.IS.Data.ValueConfirmation
{
  public interface IAutoGenerateValueConfmReportRepository
  {
    List<AutoGenerateValueConfirmationReport> GetValueConfirmationReport(BillingPeriod billingPeriod, int memberId);
  }
}

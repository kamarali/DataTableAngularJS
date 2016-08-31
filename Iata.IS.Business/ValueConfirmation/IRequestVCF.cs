using System;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Business.ValueConfirmation
{
  public interface IRequestVCF
  {
    void GenerateRequestVCF();

    void GenerateRequestVCFInternal(BillingPeriod currentIchBillingPeriod, Guid requestId, int regenerateFlag = 0);
  }
}

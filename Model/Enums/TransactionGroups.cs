using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Enums
{
  public enum TransactionGroup
  {
    PrimeBillingsNonSampling = 1,
    PassengerRejectionBillingsNonSampling = 2,
    PassengerBillingMemos = 3,
    PassengerCreditMemos = 4,
    PrimeBillingsSamplingProvisionalFormAB = 5,
    PassengerSamplingFormC = 6,
    PassengerSamplingFormDE = 7,
    PassengerSamplingFormF = 8,
    PassengerSamplingFormXF = 9,
    CargoOriginalAWB = 10,
    CargoChargesCollectAirWaybills = 11,
    CargoRejectionBillings = 12,
    CargoBillingMemos = 13,
    CargoCreditMemos = 14,
    Miscellaneous = 15,
    UATP = 16
  }
}

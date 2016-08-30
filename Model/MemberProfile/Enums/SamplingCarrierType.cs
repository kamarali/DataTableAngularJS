using System;

namespace Iata.IS.Model.MemberProfile.Enums
{
  [Flags]
  public enum SamplingCarrierType
  {
    NotASamplingCarrier = 1,
    OnlyInwardSampling = 2,
    OnlyOutwardSampling = 3,
    SamplingBothWays = 4
  }
}

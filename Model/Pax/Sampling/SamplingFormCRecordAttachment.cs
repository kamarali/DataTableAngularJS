using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Pax.Sampling
{
  public class SamplingFormCRecordAttachment : Attachment
  {
    public DateTime? ProcessingCompletedOn { get; set; }
  }
}

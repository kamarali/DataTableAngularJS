using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class IsDailyOutputProcessLog : EntityBase<Guid>
  {
    public int MemberId { get; set; }
    public DateTime TargetDate { get; set; }
    public bool NilFileRequired { get; set; }
    //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery
    public int IsXmlInvoiceCount { get; set; }
    public int DailyIsXmlGenerationStatus { get; set; }
    public DateTime? DailyIsXmlGenerationStartDate { get; set; }
    public DateTime? DailyIsXmlGenerationEndDate { get; set; }
    public int DailyIsXmlGenerationRetryCount { get; set; }
    public int DailyOarGenerationStatus { get; set; }
    public DateTime? DailyOarGenerationStartDate { get; set; }
    public DateTime? DailyOarGenerationEndDate { get; set; }
    public int DailyOarGenerationRetryCount { get; set; }
    //CMP#622: Add New Properties for Location Spec File Processing
    public string LocationId { get; set; }

    //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery
    public int OARInvoiceCount { get; set; }
  }
}

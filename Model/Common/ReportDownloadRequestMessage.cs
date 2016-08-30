using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Core;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// The oracle queue message type for Correspondence Trail Report Request
  /// </summary>
  public struct ReportDownloadRequestMessage
  {
    private Guid _recordId;
    public string StringRecordId { get; private set; }
    public int RequestingMemberId;
    public int UserId;
    public List<long> CorrespondenceNumbers;
    public string InputData;
    public string DownloadUrl { get; set; }
    public BillingCategoryType BillingCategoryType;
    //CMP508:Audit Trail Download with Supporting Documents
    public OfflineReportType OfflineReportType { get; set; }


    /// <summary>
    /// Gets or sets the record id in Guid representation.
    /// </summary>
    /// <value>The record id in Guid representation.</value>
    public Guid RecordId
    {
      get
      {
        return _recordId;
      }
      set
      {
        _recordId = value;
        StringRecordId = ConvertUtil.ConvertGuidToString(_recordId);
      }
    }




  }
}

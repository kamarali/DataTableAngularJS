using System;
using System.Reflection;
using Iata.IS.Core;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using log4net;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// The oracle queue message type for invoice offline collection 
  /// </summary>
  public struct InvoiceOfflineCollectionMessage
  {
    private Guid _recordId;
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
    /// <summary>
    /// Gets the record id in string representation.
    /// </summary>
    /// <value>The record id in string representation.</value>
    public string StringRecordId { get; private set; }
    public InvoiceStatusType InvoiceStatus;
    //public bool IsFormC;
    public string RootPath;
    public BillingCategoryType BillingCategory;
    
    public Boolean IsReGenerate;
    public int MemberId;

  }
}
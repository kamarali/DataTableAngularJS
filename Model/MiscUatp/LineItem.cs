using System;
using System.Collections.Generic;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp.Base;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.MiscUatp
{
  public class LineItem : LineItemBase
  {
    /// <summary>
    /// Gets or sets the PO line item number.
    /// </summary>
    /// <value>The PO line item number.</value>
    public int? POLineItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the charge code id.
    /// </summary>
    /// <value>The charge code id.</value>
    public int ChargeCodeId { get; set; }

    /// <summary>
    /// Navigation property for <see cref="ChargeCode"/>.
    /// </summary>
    public ChargeCode ChargeCode { get; set; }

    /// <summary>
    /// Gets or sets the charge code type id.
    /// </summary>
    /// <value>The charge code type id.</value>
    public int? ChargeCodeTypeId { get; set; }

    /// <summary>
    /// Gets or sets the type of the charge code.
    /// </summary>
    /// <value>The type of the charge code.</value>
    public ChargeCodeType ChargeCodeType { get; set; }

    /// <summary>
    /// Gets or sets the original line item number.
    /// </summary>
    /// <value>The original line item number.</value>
    public int? OriginalLineItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the detail count.
    /// </summary>
    /// <value>The detail count.</value>
    public int DetailCount { get; set; }

    /// <summary>
    /// Value of location in misc. invoice, it could be city/airport code.
    /// </summary>
    /// <value>The city airport code.</value>
    public string LocationCode { get; set; }

    /// <summary>
    /// Value of location in misc. invoice, it could be city/airport code.
    /// </summary>
    /// <value>The city airport code.</value>
    public string LocationCodeIcao { get; set; }

    /// <summary>
    /// Gets or sets the line item add on charges.
    /// </summary>
    /// <value>The add on charges.</value>
    public List<LineItemAddOnCharge> AddOnCharges { get; private set; }

    /// <summary>
    /// Gets or sets the line item additional details.
    /// </summary>
    /// <value>The line item additional details.</value>
    public List<LineItemAdditionalDetail> LineItemAdditionalDetails { get; private set; }

    /// <summary>
    /// Gets or sets the line item details.
    /// </summary>
    /// <value>The line item details.</value>
    public List<LineItemDetail> LineItemDetails { get; private set; }

    /// <summary>
    /// For Navigation to <see cref="MiscUatpInvoice"/>. 
    /// </summary>
    public Guid InvoiceId { get; set; }

    public MiscUatpInvoice Invoice { get; set; }

    /// <summary>
    /// Gets or sets the misc tax.
    /// </summary>
    /// <value>The misc tax.</value>
    public List<LineItemTax> TaxBreakdown { get; set; }
    
    public string DisplayChargeCode
    {
      get
      {
        return ChargeCode != null ? ChargeCode.Name : string.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the line item status.
    /// </summary>
    /// <value>The line item status.</value>
    public InvoiceStatusType LineItemStatus
    {
      get
      {
        return (InvoiceStatusType)LineItemStatusId;
      }
      set
      {
        LineItemStatusId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Gets or sets the name of the location.
    /// </summary>
    /// <value>The name of the location.</value>
    public string LocationName { get; set; }

    /// <summary>
    /// Gets or sets the line item status id.
    /// </summary>
    /// <value>The line item status id.</value>
    public int LineItemStatusId { set; get; }

    //CMP#502 : Rejection Reason for MISC Invoices
    public string RejectionReasonCode { get; set; }
    public string RejReasonCodeDescription { get; set; }
    //CMP#502 : [3.5] Rejection Reason for MISC Invoices
    public string RejectionReasonCodeText
    {
        get
        {
            return ((!string.IsNullOrWhiteSpace(RejectionReasonCode) && !string.IsNullOrWhiteSpace(RejReasonCodeDescription))?string.Format("{0}-{1}",RejectionReasonCode,RejReasonCodeDescription):string.Empty);
        }
    }

    public LineItem()
    {
      AddOnCharges = new List<LineItemAddOnCharge>();
      LineItemDetails = new List<LineItemDetail>();
      TaxBreakdown = new List<LineItemTax>();
      LineItemAdditionalDetails = new List<LineItemAdditionalDetail>();
    }

    public NavigationDetails NavigationDetails { get; set; }

  }
}

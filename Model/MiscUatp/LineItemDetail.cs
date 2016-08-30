using System;
using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Base;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Model.MiscUatp
{
  public class LineItemDetail : LineItemBase
  {
    /// <summary>
    /// Gets or sets the detail number.
    /// </summary>
    /// <value>The detail number.</value>
    public int DetailNumber { get; set; }

    /// <summary>
    /// Gets or sets the add on charges for line items detail.
    /// </summary>
    /// <value>The add on charges.</value>
    public List<LineItemDetailAddOnCharge> AddOnCharges { get; private set; }

    /// <summary>
    /// Gets or sets the line item detail additional details.
    /// </summary>
    /// <value>The line item detail additional details.</value>
    public List<LineItemDetailAdditionalDetail> LineItemDetailAdditionalDetails { get; private set; }

    /// <summary>
    /// Gets or sets the field values.
    /// </summary>
    /// <value>The field values.</value>
    public List<FieldValue> FieldValues { get; private set; }

    /// <summary>
    /// For Navigation to <see cref="LineItem"/>. 
    /// </summary>
    public Guid LineItemId { get; set; }

    /// <summary>
    /// Navigational property to <see cref="LineItem"/>
    /// </summary>
    /// <value>The line item.</value>
    public LineItem LineItem { get; set; }

    /// <summary>
    /// Gets or sets the misc tax.
    /// </summary>
    /// <value>The misc tax.</value>
    public List<LineItemDetailTax> TaxBreakdown { get; set; }

    public LineItemDetail()
    {
      AddOnCharges = new List<LineItemDetailAddOnCharge>();
      FieldValues = new List<FieldValue>();
      TaxBreakdown = new List<LineItemDetailTax>();

      LineItemDetailAdditionalDetails = new List<LineItemDetailAdditionalDetail>();
    }

    public NavigationDetails NavigationDetails { get; set; }

    /// <summary>
    /// To display the summary of dynamic fields added for a line item detail.
    /// </summary>
    public string DynamicFieldsSummary { get; set; }

  }
}
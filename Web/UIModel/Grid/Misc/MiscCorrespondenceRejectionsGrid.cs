using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Misc
{
  /// <summary>
  /// UIModel for Invoice Source code total list
  /// </summary>
  public class MiscCorrespondenceRejectionGrid : GridBase
  {
    public MiscCorrespondenceRejectionGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid == null)
      {
        return;
      }
      _grid.Columns = new List<JQGridColumn>
                        {
                          GridColumnHelper.HiddenKeyColumn("Id"),
                          //TFS#10000 :Firefox: v45: Text is not displaying in Linked Rejections Grid for Misc & UATP.
                          GridColumnHelper.TextColumn("BillingMemberText", "Billing Member",145),
                          GridColumnHelper.TextColumn("DisplayBillingPeriod", "Billing Period",120),
                          GridColumnHelper.TextColumn("InvoiceNumber","Invoice Number",120),
                        };
      
        var formatter = new CustomFormatter() { FormatFunction = string.Format("{0}_OpenGivenRejectionRecord", _grid.ID) };

        _grid.Columns.Find(column => column.DataField == "InvoiceNumber").Formatter = formatter;  
    }
  }
}
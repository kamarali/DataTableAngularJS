using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class CargoCorrespondenceRejectionsGrid : GridBase
  {

    public CargoCorrespondenceRejectionsGrid(string gridId, string dataUrl)
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
                          GridColumnHelper.HiddenColumn("InvoiceId"),
                          GridColumnHelper.TextColumn("BillingMemberText", "Billing Member",110),
                          GridColumnHelper.TextColumn("DisplayBillingPeriod", "Billing Period",100),
                          GridColumnHelper.TextColumn("InvoiceNumber","Invoice Number",100),
                          GridColumnHelper.TextColumn("RejectionMemoNumber","Rejection Memo Number",120),
                          GridColumnHelper.HiddenColumn("BillingCode")
                        };

      var formatter = new CustomFormatter() { FormatFunction = string.Format("{0}_OpenGivenRejectionRecord", _grid.ID) };

      _grid.Columns.Find(column => column.DataField == "RejectionMemoNumber").Formatter = formatter;
    }
  }
}
using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.MU
{
  public class LineItemDetailGrid : GridBase
  {
    public LineItemDetailGrid(string gridId, string dataUrl)
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
                            GridColumnHelper.ActionColumn("Id", 90),
                            GridColumnHelper.NumberColumn("DetailNumber", "Line Detail #", 70),
                            //GridColumnHelper.TextColumn("ChargeCode", "Charge Code", 70),
                            GridColumnHelper.DateColumn("StartDate","Service Start Date"),
                            GridColumnHelper.DateColumn("EndDate","Service End Date"),
                            GridColumnHelper.TextColumn("Description", "Description", 120),
                            GridColumnHelper.NumberColumn("Quantity", "Quantity", 70),
                            GridColumnHelper.TextColumn("UomCodeId", "UOM Code", 120),                            
                            GridColumnHelper.AmountColumn("UnitPrice", "Unit Price", 4),
                            GridColumnHelper.NumberColumn("ScalingFactor", "Scaling Factor", 70),
                            GridColumnHelper.AmountColumn("ChargeAmount", "Line Detail Total", 3),
                            GridColumnHelper.AmountColumn("TotalTaxAmount", "Tax", 3),
                            GridColumnHelper.AmountColumn("TotalVatAmount", "VAT", 3), 
                            GridColumnHelper.AmountColumn("TotalAddOnChargeAmount", "Add/ Deduct Charge", 3), 
                            GridColumnHelper.AmountColumn("TotalNetAmount", "Line Detail Net Total", 3),
                            GridColumnHelper.TextColumn("DynamicFieldsSummary", "Dynamic Fields Summary", 500)
                          };
      _grid.SortSettings.InitialSortColumn = "DetailNumber";
      var formatter = new CustomFormatter
                        {
                          FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
                        };

      // Grid Checkbox column
      _grid.MultiSelect = true;
      _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
      _grid.Columns[0].Formatter = formatter;
    }
  }
}

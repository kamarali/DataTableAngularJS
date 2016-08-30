using System.Collections.Generic;
using Iata.IS.AdminSystem;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.MU
{
  public class LineItemGrid : GridBase
  {
      public LineItemGrid(string gridId, string dataUrl, bool isDisplayChargeCategoryType = false, bool isDisplayRejectionReasonCode = false)
          : base(gridId: gridId, dataUrl: dataUrl, isDisplayChargeCategoryType: isDisplayChargeCategoryType, isDisplayRejectionReasonCode: isDisplayRejectionReasonCode)
    {
      //249863 - Request to extend the search results for PAX/MISC payables and Billing History screen to 500
      //isDisplayChargeCategoryType  is true when billing category is UATP 
      if (!isDisplayChargeCategoryType)
      {
        var pageSize = string.IsNullOrEmpty(GlobalVariables.PageSizeOptions)
                      ? SystemParameters.Instance.UIParameters.PageSizeOptions
                      : GlobalVariables.PageSizeOptions;

        this.PageSizeOptions = (pageSize.Contains("200") && !pageSize.Contains("500"))
                                 ? pageSize.Replace("200", "500")
                                 : GlobalVariables.PageSizeOptions;
      }
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
                            GridColumnHelper.NumberColumn("LineItemNumber", "Line Item #", 70),
                            GridColumnHelper.TextColumn("DisplayChargeCode", "Charge Code", 70),
                            //CMP470: add column to display charge code type.
                            _isDisplayChargeCategoryType ? GridColumnHelper.TextColumn("ChargeCodeType","Charge Code Type",70,true):GridColumnHelper.HiddenColumn("ChargeCodeType"),
                            GridColumnHelper.DateColumn("StartDate","Service Start Date"),
                            GridColumnHelper.DateColumn("EndDate","Service End Date"),
                            //CMP#502 : [3.7] Rejection Reason for MISC Invoices
                            _isDisplayRejectionReasonCode ?GridColumnHelper.TextColumn("RejectionReasonCode","Rej. Reason Code",120) : GridColumnHelper.HiddenColumn("RejectionReasonCode"),
                            
                            //SCP#366911 - IS Web Invoice Search
                            // Unhide description column
                            GridColumnHelper.TextColumn("Description", "Description", 120),
                            GridColumnHelper.NumberColumn("Quantity", "Quantity", 70),
                            GridColumnHelper.TextColumn("UomCodeId", "UOM Code", 70),                            
                            GridColumnHelper.AmountColumn("UnitPrice", "Unit Price", 4),
                            GridColumnHelper.NumberColumn("ScalingFactor", "Scaling Factor", 70),
                            GridColumnHelper.AmountColumn("Total", "Line Total", 3),
                            GridColumnHelper.AmountColumn("TotalTaxAmount", "Tax", 3),
                            GridColumnHelper.AmountColumn("TotalVatAmount", "VAT", 3), 
                            GridColumnHelper.AmountColumn("TotalAddOnChargeAmount", "Add/ Deduct Charge", 3), 
                            GridColumnHelper.AmountColumn("TotalNetAmount", "Line Net Total", 3), 
                          };
      _grid.SortSettings.InitialSortColumn = "LineItemNumber";
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

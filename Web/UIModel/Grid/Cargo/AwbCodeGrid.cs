using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  /// <summary>
  /// UIModel for Invoice AWB total list
  /// </summary>
  public class AwbCodeGrid : GridBase
  {
      public AwbCodeGrid(string gridId, string dataUrl, bool isBillingTypePayables)
      : base(gridId, dataUrl)
    {
      // Call InitializeSourceGridColumns() method which will initialize SourceCode grid columns
      InitializeSourceGridColumns(isBillingTypePayables);
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      // Note: Initialized SourceGrid columns in InitializeSourceGridColumns() method as column count varies depending on BillingType, .i.e. two new columns are added if BillingType is Payables 
    }

    private void InitializeSourceGridColumns(bool isBillingTypePayables)
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                            {
                                  GridColumnHelper.TextColumn("BillingCode", "Billing Code",100),
                                  GridColumnHelper.NumberColumn("NumberOfBillingRecords","Total No. of Billing Records",100),
                                  GridColumnHelper.AmountColumn("TotalWeightCharge","Total Weight Charge",3,100),
                                  GridColumnHelper.AmountColumn("TotalValuationCharge","Total Valuation Charge",3,120),
                                  GridColumnHelper.AmountColumn("TotalOtherCharge","Total Other Charges",3,100),
                                  GridColumnHelper.AmountColumn("TotalIscAmount","Total ISC Amount",3,110),
                                  GridColumnHelper.AmountColumn("TotalVatAmount","Total VAT Amount",3,120),
                                  GridColumnHelper.AmountColumn("BillingCodeSbTotal","Billing Code Sub Total",3,120),
                            };

        _grid.Columns.Find(column => column.DataField == "BillingCode").Formatter = new CustomFormatter { FormatFunction = "SetBillingCode" };
        // If BillingType is Payables add two new columns which will be used to display SourceCodeVatTotal Popup
        if(isBillingTypePayables)
        {
          // Add columns to Collection
          _grid.Columns.Add(GridColumnHelper.HiddenColumn("Id"));
          _grid.Columns.Add(GridColumnHelper.SortableTextColumn("NumberOfChildRecords", "Billing Code VAT", 100));
         
          // Format newly added column to display "SourceCodevatTotal" link  
          _grid.Columns.Find(c => c.DataField == "NumberOfChildRecords").Formatter = new CustomFormatter
          {
              FormatFunction = "formatBillingCodeVatTotalColumn"
          };

         // _grid.Columns.Find(column => column.DataField == "CcaIndicator").Formatter = new CustomFormatter { FormatFunction = "SetCcaIndicator" };
          
        }
      }
    }
  }
}
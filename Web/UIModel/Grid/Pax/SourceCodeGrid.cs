using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  /// <summary>
  /// UIModel for Invoice Source code total list
  /// </summary>
  public class SourceCodeGrid : GridBase
  {
    public SourceCodeGrid(string gridId, string dataUrl, bool isBillingTypePayables)
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
                                  GridColumnHelper.NumberColumn("SourceCodeId", "Source Code"),
                                  GridColumnHelper.NumberColumn("NumberOfBillingRecords","No. of Billing Records"),
                                  GridColumnHelper.AmountColumn("TotalGrossValue","Total Gross Value"),
                                  GridColumnHelper.AmountColumn("TotalIscAmount","Total ISC Amt."),
                                  GridColumnHelper.CustomColumn("TotalOtherCommission","Total Other Commission Amt.",120,TextAlign.Right, new NumberFormatter(){DecimalPlaces = 2, DecimalSeparator = ".", ThousandsSeparator = ","}),
                                  GridColumnHelper.AmountColumn("TotalUatpAmount","Total UATP Amt."),
                                  GridColumnHelper.AmountColumn("TotalHandlingFee","Total Handling Fee Amt."),
                                  GridColumnHelper.AmountColumn("TotalTaxAmount","Total Tax Amt."),
                                  GridColumnHelper.AmountColumn("TotalVatAmount","Total VAT Amt."),
                                  GridColumnHelper.AmountColumn("TotalNetAmount","Net Total"),
                            };

        // If BillingType is Payables add two new columns which will be used to display SourceCodeVatTotal Popup
        if(isBillingTypePayables)
        {
          // Add columns to Collection
          _grid.Columns.Add(GridColumnHelper.HiddenColumn("Id"));
          _grid.Columns.Add(GridColumnHelper.SortableTextColumn("NumberOfChildRecords", "Source Code VAT", 100));

          // Format newly added column to display "SourceCodevatTotal" link  
          _grid.Columns.Find(c => c.DataField == "NumberOfChildRecords").Formatter = new CustomFormatter
          {
            FormatFunction = "formatSourceCodeVatTotalColumn"
          };
        }
      }
    }
  }
}
using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class FormFSummaryGrid : GridBase
  {
      public FormFSummaryGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {

    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid == null) return;
      _grid.Columns = new List<JQGridColumn>
                        {
                          GridColumnHelper.NumberColumn("NumberOfBillingRecords", "No. Of Billing Records"),
                          GridColumnHelper.AmountColumn("TotalGrossValue", "Total Gross Value"),
                          GridColumnHelper.AmountColumn("TotalIscAmount", "Total ISC Amt."),
                          GridColumnHelper.AmountColumn("TotalOtherCommission", "Total Other Commission Amt."),
                          GridColumnHelper.AmountColumn("TotalUatpAmount", "Total UATP Amt."),
                          GridColumnHelper.AmountColumn("TotalHandlingFee", "Total Handling Fee Amt."),
                          GridColumnHelper.AmountColumn("TotalTaxAmount", "Total Tax Amt."),
                          GridColumnHelper.AmountColumn("TotalVatAmount", "Total VAT Amt."),
                          GridColumnHelper.CustomColumn("TotalNetAmount", "Net Amt. Before Sampling Constant", 145, TextAlign.Right, new NumberFormatter() { DecimalPlaces = 2, DecimalSeparator = ".", ThousandsSeparator = "," }),
                          GridColumnHelper.AmountColumn("SamplingConstant", "Sampling Constant", 3), 
                          GridColumnHelper.CustomColumn("TotalAmountAfterSamplingConstant", "Net Amt. After Sampling Constant", 145, TextAlign.Right, new NumberFormatter() { DecimalPlaces = 2, DecimalSeparator = ".", ThousandsSeparator = "," })
                        };
    }
  }
}
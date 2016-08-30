using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;


namespace Iata.IS.Web.UIModel.Grid.Pax
{
  /// <summary>
  /// Grid for Form D Source Code list
  /// </summary>
  public class SamplingFormDDetailsSourceCodeGrid:GridBase
  {
    public SamplingFormDDetailsSourceCodeGrid(string gridId, string dataUrl) : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                            {
                              GridColumnHelper.NumberColumn("NumberOfBillingRecords","No. Of Billing Records"),
                              GridColumnHelper.AmountColumn("TotalGrossValue","Total Gross Value"),
                              GridColumnHelper.AmountColumn("TotalIscAmount","Total ISC Amt."),
                              GridColumnHelper.AmountColumn("TotalOtherCommission","Total Other Commission Amt."),
                              GridColumnHelper.AmountColumn("TotalUatpAmount","Total UATP Amt."),                                             
                              GridColumnHelper.AmountColumn("TotalHandlingFee","Total Handling Fee Amt."),
                              GridColumnHelper.AmountColumn("TotalTaxAmount","Total Tax Amt."),                                             
                              GridColumnHelper.AmountColumn("TotalVatAmount","Total VAT Amt."),
                              GridColumnHelper.AmountColumn("TotalNetAmount","Net Total"),
                            };
          
      }
    }
  }
}
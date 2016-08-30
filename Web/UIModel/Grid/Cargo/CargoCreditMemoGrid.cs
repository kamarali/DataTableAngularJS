using System.Collections.Generic;
using System.Web.UI.WebControls;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  /// <summary>
  /// Grid for Credit Memo List
  /// </summary>
  public class CargoCreditMemoGrid : GridBase
  {
    public CargoCreditMemoGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {

    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid == null) return;
      _grid.Columns = new List<JQGridColumn> {
                                               GridColumnHelper.ActionColumn("Id", 90),
                                               GridColumnHelper.SortableTextColumn("BatchSequenceNumber", "Batch No.", 60),
                                               GridColumnHelper.SortableTextColumn("RecordSequenceWithinBatch", "Seq. No.", 60),
                                               GridColumnHelper.TextColumn("CreditMemoNumber", "Credit Memo No.", 70),
                                               GridColumnHelper.AmountColumn("TotalWeightCharges", "Weight Charges", 3),
                                               GridColumnHelper.AmountColumn("TotalIscAmountCredited", "ISC Amt.", 3),
                                               GridColumnHelper.AmountColumn("TotalOtherChargeAmt", "Other Charge Amt.", 3),
                                               GridColumnHelper.AmountColumn("TotalVatAmountCredited", "VAT Amt.", 3),
                                               GridColumnHelper.AmountColumn("TotalValuationAmt", "Valuation Amt.", 3),
                                               GridColumnHelper.AmountColumn("NetAmountCredited", "Net Credited Amt.", 3),
                                               GridColumnHelper.TextColumn("ReasonCode", "Reason Code", 70),
                                               GridColumnHelper.TextColumn("ReasonCodeDescription", "Reason Description", 120),
                                             };
      
      var formatter = new CustomFormatter() { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
      _grid.Columns[0].Formatter = formatter;

      _grid.SortSettings.InitialSortColumn = "BatchSequenceNumber,RecordSequenceWithinBatch";
      _grid.SortSettings.InitialSortDirection = Trirand.Web.Mvc.SortDirection.Asc;
    }
  }
}
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;
using SortDirection = Trirand.Web.Mvc.SortDirection;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  /// <summary>
  /// Grid for Credit Memo List
  /// </summary>
  public class CreditMemoGrid : GridBase
  {
    public CreditMemoGrid(string gridId, string dataUrl)
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
                                               GridColumnHelper.AmountColumn("TotalGrossAmountCredited", "Gross Fare Value"),
                                               GridColumnHelper.AmountColumn("TotalIscAmountCredited", "ISC Amt."),
                                               GridColumnHelper.AmountColumn("TotalOtherCommissionAmountCredited", "Other Comm. Amt."),
                                               GridColumnHelper.AmountColumn("TotalUatpAmountCredited", "UATP Amt."),
                                               GridColumnHelper.AmountColumn("TotalHandlingFeeCredited", "Handling Fee Amt."),
                                               GridColumnHelper.AmountColumn("TaxAmount", "Tax Amt."),
                                               GridColumnHelper.AmountColumn("VatAmount", "Vat Amt."),
                                               GridColumnHelper.AmountColumn("NetAmountCredited", "Net Credited Amt."),
                                               GridColumnHelper.TextColumn("ReasonCode", "Reason Code", 70),
                                               GridColumnHelper.TextColumn("ReasonCodeDescription", "Reason Description", 120),
                                             };
      
      var formatter = new CustomFormatter() { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
      _grid.Columns[0].Formatter = formatter;

      _grid.SortSettings.InitialSortColumn = "BatchSequenceNumber,RecordSequenceWithinBatch";
      _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
    }
  }
}
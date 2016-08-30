using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  /// <summary>
  /// Grid for Rejection Memo list
  /// </summary>
  public class FormFRejectionMemoGrid : GridBase
  {
    public FormFRejectionMemoGrid(string gridId, string dataUrl)
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
                          GridColumnHelper.ActionColumn("Id", 90),
                          GridColumnHelper.SortableTextColumn("SourceCodeId", "Source Code", 80),
                          GridColumnHelper.SortableTextColumn("BatchSequenceNumber", "Batch No.", 80),
                          GridColumnHelper.SortableTextColumn("RecordSequenceWithinBatch", "Seq. No.", 80),
                          GridColumnHelper.TextColumn("RejectionMemoNumber", "Rejection Memo No.", 90),
                          GridColumnHelper.AmountColumn("TotalGrossDifference", "Gross Fare Value"),
                          GridColumnHelper.AmountColumn("IscDifference", "ISC Amt."),
                          GridColumnHelper.AmountColumn("OtherCommissionDifference", "Other Comm. Amt."),
                          GridColumnHelper.AmountColumn("UatpAmountDifference", "UATP Amt."),
                          GridColumnHelper.AmountColumn("HandlingFeeAmountDifference", "Handling Fee Amt."),
                          GridColumnHelper.AmountColumn("TotalTaxAmountDifference", "Tax Amt."),
                          GridColumnHelper.AmountColumn("TotalVatAmountDifference", "VAT Amt."),
                          GridColumnHelper.AmountColumn("TotalNetRejectAmount", "Net Reject Amt."),
                          GridColumnHelper.AmountColumn("SamplingConstant", "Sampling Constant",3),
                          GridColumnHelper.CustomColumn("TotalNetRejectAmountAfterSamplingConstant",
                                                        "Net Amt. After Sampling Const.", 120, TextAlign.Right),
                          GridColumnHelper.TextColumn("ReasonCode", "Reason Code", 70),
                          GridColumnHelper.TextColumn("ReasonCodeDescription", "Reason Description", 70)
                        };

      var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
      _grid.Columns[0].Formatter = formatter;

      _grid.SortSettings.InitialSortColumn = "BatchSequenceNumber,RecordSequenceWithinBatch";
      _grid.SortSettings.InitialSortDirection = SortDirection.Asc;

    }
  }
}
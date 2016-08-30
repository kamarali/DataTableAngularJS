using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  /// <summary>
  /// Grid for Rejection Memo list
  /// </summary>
  public class RejectionMemoGrid:GridBase
  {
    public RejectionMemoGrid(string gridId, string dataUrl) : base(gridId, dataUrl)
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
                              /* PLEASE CHANGE IN SCRIPTHELPER IF SEQUENCE OF COLUMNS IS CHANGED.*/
                              GridColumnHelper.ActionColumn("Id",90),
                              GridColumnHelper.SortableTextColumn("SourceCodeId","Source Code", 80),
                              GridColumnHelper.SortableTextColumn("BatchSequenceNumber", "Batch No.", 80),
                              GridColumnHelper.SortableTextColumn("RecordSequenceWithinBatch","Seq. No.", 80),
                              GridColumnHelper.TextColumn("RejectionMemoNumber","Rejection Memo Number",90),
                              GridColumnHelper.AmountColumn("TotalGrossDifference","Gross Fare Value"),
                              GridColumnHelper.AmountColumn("IscDifference","ISC Amt."),
                              GridColumnHelper.AmountColumn("OtherCommissionDifference","Other Comm. Amt."),
                              GridColumnHelper.AmountColumn("UatpAmountDifference","UATP Amt."),                                             
                              GridColumnHelper.AmountColumn("HandlingFeeAmountDifference","Handling Fee Amt."),
                              GridColumnHelper.AmountColumn("TotalTaxAmountDifference","Tax Amt."),                                             
                              GridColumnHelper.AmountColumn("TotalVatAmountDifference","VAT Amt."),
                              GridColumnHelper.AmountColumn("TotalNetRejectAmount","Net Reject Amt."),
                              GridColumnHelper.TextColumn("ReasonCode","Reason Code",70),
                              GridColumnHelper.TextColumn("ReasonCodeDescription","Reason Description",100),
                              GridColumnHelper.HiddenColumn("RejectionStage")
                            };


        var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
        _grid.Columns[0].Formatter = formatter;
        _grid.SortSettings.InitialSortColumn = "BatchSequenceNumber,RecordSequenceWithinBatch";
        _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
      }
    }
  }
}
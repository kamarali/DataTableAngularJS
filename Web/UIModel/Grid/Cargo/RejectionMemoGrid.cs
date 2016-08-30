using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class RejectionMemoGrid : GridBase
  {
    public RejectionMemoGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
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
                              GridColumnHelper.SortableTextColumn("BatchSequenceNumber","Batch No.", 80),
                              GridColumnHelper.SortableTextColumn("RecordSequenceWithinBatch", "Seq. No.", 80),
                              GridColumnHelper.SortableTextColumn("RejectionMemoNumber","Rejection Memo No.", 80),
                              GridColumnHelper.NumberColumn("RejectionStage", "Rejection Stage"),
                              GridColumnHelper.AmountColumn("TotalWeightChargeDifference","Total Weight Charges", 3),
                              GridColumnHelper.AmountColumn("TotalValuationChargeDifference","Total Valuation Charges", 3),
                              GridColumnHelper.AmountColumn("TotalOtherChargeDifference","Total Other Charges", 3),
                              GridColumnHelper.AmountColumn("TotalIscAmountDifference","Total ISC Amount", 3),
                              GridColumnHelper.AmountColumn("TotalVatAmountDifference","Total VAT Amount", 3),
                              GridColumnHelper.AmountColumn("TotalNetRejectAmount","Total Net Reject Amount", 3),
                              GridColumnHelper.SortableTextColumn("ReasonCode","Reason Code",70),
                              GridColumnHelper.SortableTextColumn("ReasonCodeDescription","Reason Description",100),
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

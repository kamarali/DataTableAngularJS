using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  /// <summary>
  /// Rejection Memo with Linked Correspondence Grid 
  /// </summary>
  //CMP612: Changes to PAX CGO Correspondence Audit Trail Download.
  public class RejectionMemoLinkedCorrespondenceGrid : GridBase
  {
    /// <summary>
    /// Parameter constructor for rejection memo linked correspondence.
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="dataUrl"></param>
    public RejectionMemoLinkedCorrespondenceGrid(string gridId, string dataUrl)
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
                            GridColumnHelper.ActionColumn("Id", 90),
      	                		GridColumnHelper.TextColumn("RejectionMemoNumber", "Rejection Memo Number", 120, true),
      	                		GridColumnHelper.TextColumn("CorrespondenceNumber", "Linked Correspondence Number", 160),
      	                		GridColumnHelper.TextColumn("InvoiceNumber", "Invoice Number", 100),
      	                		GridColumnHelper.TextColumn("BillingPeriod", "Billing Period", 90),
                            GridColumnHelper.TextColumn("BillingMemberCode", "Billing Member Code", 90),
                            GridColumnHelper.TextColumn("BilledMemberCode", "Billed Member Code", 90),
                            GridColumnHelper.TextColumn("ReasonCode", "Reason Code", 60),
                            GridColumnHelper.TextColumn("NetRejectAmount", "Net Reject Amount", 100, isSortable:true),
      	                	};

        _grid.SortSettings.InitialSortColumn = "RejectionMemoNumber";
        _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
        // Grid Checkbox column
        _grid.MultiSelect = true;

        _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;

        var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_showRejection", _grid.ID) };
        _grid.Columns[0].Formatter = formatter;
      }
    }
  }
}
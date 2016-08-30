using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.MU
{
  public class ExceptionDetailsGrid : GridBase
  {
    public ExceptionDetailsGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                                     {
                                        GridColumnHelper.HiddenColumn("ExceptionDetailId",isPrimaryKey:true),
                                        GridColumnHelper.SortableTextColumn("TransactionName", "Invoice Type", 120),
                                        GridColumnHelper.SortableTextColumn("ErrorLevelDisplay", "Error Level", 120),
                                        GridColumnHelper.SortableTextColumn("FieldName","Field Name",120),
                                        GridColumnHelper.SortableTextColumn("FieldValue","Field Value",120),
                                        GridColumnHelper.HiddenColumn("ErrorLevel"),
                                        GridColumnHelper.HiddenColumn("YourInvoiceNo"),
                                        GridColumnHelper.HiddenColumn("YourInvoiceBillingDate"),
                                        GridColumnHelper.HiddenColumn("YourRejectionMemoNo"),
                                        GridColumnHelper.HiddenColumn("YourBmCmNo"),
                                        GridColumnHelper.HiddenColumn("BmCmIndicator"),
                                        GridColumnHelper.HiddenColumn("TranscationId"),
                                        GridColumnHelper.HiddenColumn("CorrespondenceRefNo"),
                                        GridColumnHelper.HiddenColumn("ReasonCode"),
                                        GridColumnHelper.HiddenColumn("RejectionStage"),
                                        GridColumnHelper.HiddenColumn("PkReferenceId"),
                                        GridColumnHelper.HiddenColumn("YourInvoiceMonth"),
                                        GridColumnHelper.HiddenColumn("YourInvoicePeriod"),
                                        GridColumnHelper.HiddenColumn("YourInvoiceYear"),
                                        //SCP252342 - SRM: ICH invoice in ready for billing status
                                        GridColumnHelper.HiddenColumn("LastUpdatedOn")
                                    };
        var clientSideEvents = new ClientSideEvents { RowSelect = "DisplayUpdateDetails", GridInitialized = "SelectFirstDetailRow" };
        //var clientSideEvents = new ClientSideEvents { GridInitialized = "SelectFirstRow" };
        _grid.ClientSideEvents = clientSideEvents;

      }
    }
  }
}
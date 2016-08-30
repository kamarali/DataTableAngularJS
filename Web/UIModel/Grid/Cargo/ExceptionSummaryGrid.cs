using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
    public class ExceptionSummaryGrid : GridBase
    {
        public ExceptionSummaryGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                        //ActionColumn for action ID but not required
                                        //GridColumnHelper.ActionColumn("ID",10),
                                        //GridColumnHelper.NumberColumn("InvoiceNumber","Invoice Number",120),
                                        GridColumnHelper.HiddenColumn("ExceptionDetailId",isPrimaryKey:true),
                                        GridColumnHelper.SortableTextColumn("LineItemOrBatchNo", "Batch Number", 120),
                                        GridColumnHelper.SortableTextColumn("LineItemDetailOrSequenceNo", "Sequence Number",
                                                                      120),
                                        GridColumnHelper.SortableTextColumn("TransactionType", "Transaction Type", 120),
                                        GridColumnHelper.SortableTextColumn("BillingEntityCode", "Iss Airline", 100),
                                        GridColumnHelper.SortableTextColumn("DocumentNo", "Document Number", 120),
                                        GridColumnHelper.SortableTextColumn("LinkedDocNo", "Linked Document Number", 140),
                                        GridColumnHelper.SortableTextColumn("ErrorLevelDisplay", "Error Level", 100),
                                        GridColumnHelper.SortableTextColumn("FieldName", "Field Name", 120),
                                        GridColumnHelper.SortableTextColumn("FieldValue", "Field Value", 120),
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
                // for the check box , not required
                //_grid.MultiSelect = true;
                //_grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;

                var clientSideEvents = new ClientSideEvents { RowSelect = "DisplayUpdateDetails", GridInitialized = "SelectFirstDetailRow" };
                //var clientSideEvents = new ClientSideEvents { GridInitialized = "SelectFirstRow" };
                _grid.ClientSideEvents = clientSideEvents;


            }
        }
    }
}
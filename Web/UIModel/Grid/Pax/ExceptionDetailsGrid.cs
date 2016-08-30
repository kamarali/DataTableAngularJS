using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
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
                                        GridColumnHelper.SortableTextColumn("LineItemOrBatchNo", "Batch No.", 120),
                                        GridColumnHelper.SortableTextColumn("LineItemDetailOrSequenceNo", "Sequence No.", 120),
                                        GridColumnHelper.SortableTextColumn("TransactionName", "Transaction Type", 120),
                                        GridColumnHelper.SortableTextColumn("BillingEntityCode", "Issuing Airline", 120),
                                        GridColumnHelper.SortableTextColumn("DocumentNo", "Document No.", 120),
                                        GridColumnHelper.SortableTextColumn("CouponNo", "Coupon No.", 120),
                                        GridColumnHelper.SortableTextColumn("LinkedDocNo", "Linked Document No.", 120),
                                        GridColumnHelper.SortableTextColumn("ErrorLevelDisplay", "Error Level", 120),
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
                                        GridColumnHelper.HiddenColumn("BatchSeqNo"),
                                        GridColumnHelper.HiddenColumn("BatchRecordSeq"),
                                        GridColumnHelper.HiddenColumn("FimBmCmNo"),
                                        GridColumnHelper.HiddenColumn("FimCouponNo"),
                                        GridColumnHelper.HiddenColumn("FimBmCmIndicator"),
                                        GridColumnHelper.HiddenColumn("SourceCodeId"),
                                        //SCP252342 - SRM: ICH invoice in ready for billing status
                                        GridColumnHelper.HiddenColumn("LastUpdatedOn")
                                    };
                var clientSideEvents = new ClientSideEvents { RowSelect = "DisplayUpdateDetails", GridInitialized = "SelectFirstDetailRow" };
                _grid.ClientSideEvents = clientSideEvents;

            }
        }

    }
}

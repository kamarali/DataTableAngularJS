using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;


namespace Iata.IS.Web.UIModel.Grid.Pax
{
    public class SamplingExceptionDetailsGrid :GridBase
    {
        public SamplingExceptionDetailsGrid(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("BillingEntityCode","Iss. Airline",120),
                                        GridColumnHelper.SortableTextColumn("DocumentNo", "Document No.", 120),
                                        GridColumnHelper.SortableTextColumn("CouponNo", "Coupon No.", 120),
                                        GridColumnHelper.SortableTextColumn("FieldName", "Field Name", 250),
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
                                        GridColumnHelper.HiddenColumn("LineItemOrBatchNo"),
                                        GridColumnHelper.HiddenColumn("LineItemDetailOrSequenceNo"),
                                        GridColumnHelper.HiddenColumn("FimBmCmNo"),
                                        GridColumnHelper.HiddenColumn("FimCouponNo"),
                                        GridColumnHelper.HiddenColumn("FimBmCmIndicator"),
                                        //SCP252342 - SRM: ICH invoice in ready for billing status
                                         GridColumnHelper.HiddenColumn("LastUpdatedOn")
                                    };
                var clientSideEvents = new ClientSideEvents { RowSelect = "SamplingDisplayUpdateDetails", GridInitialized = "SamplingSelectFirstDetailRow" };
                _grid.ClientSideEvents = clientSideEvents;

            }
        }

    }
}
using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
    public class SupportingDocSearchResultGrid : GridBase
    {
        public SupportingDocSearchResultGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        /// <summary>
        /// Initializes columns for grid
        /// </summary>
        protected override void InitializeColumns()
        {
            if (_grid == null)
            {
                return;
            }

            _grid.Columns = new List<JQGridColumn>
                        { 
                         GridColumnHelper.ActionColumn("Id", 80),
                          
                          GridColumnHelper.HiddenColumn("InvoiceId"),
                          GridColumnHelper.HiddenColumn("RecordType"),
                          GridColumnHelper.HiddenColumn("IsFormCAttachmentAllowed"),

                          //GridColumnHelper.TextColumn("DisplayBillingMonthYear", "Billing Period", 100, true),
                          //GridColumnHelper.TextColumn("BilledMemberText", "Billed Member", 100,true),
                          GridColumnHelper.TextColumn("DisplayBillingMonthYear", "Billing Period", 100, true),
                          GridColumnHelper.TextColumn("BilledMemberText", "Billed Member", 100,true),
                          //GridColumnHelper.TextColumn("BilledMemberId", "Billed Member", 100,true),
                          GridColumnHelper.TextColumn("InvoiceNumber", "Invoice No.", 90, true),
                          GridColumnHelper.TextColumn("CodeBilling", "Billing Code", 90, true),
                          GridColumnHelper.NumberColumn("BatchSequenceNumber", "Batch No.", 90, isSortable:true),
                          GridColumnHelper.NumberColumn("RecordSequenceWithinBatch", "Sequence No.", 90, isSortable:true),
                          GridColumnHelper.SortableTextColumn("RMBMCMNumber", "RM/BM/CM No.", 90),
                          GridColumnHelper.NumberColumn("AWBSerialNumber", "AWB No.", 90, isSortable:true),

                          GridColumnHelper.TextColumn("AttachmentIndicatorOriginalText", "Attachment Indicator Original", 120, true),
                          //GridColumnHelper.TextColumn("AttachmentIndicatorOriginal", "Attachment Indicator Original", 120, true),
                          GridColumnHelper.NumberColumn("AttachmentNumber", "No. of Attachments", 100, isSortable:true),
 
                        };

            var formatter = new CustomFormatter
            {
                FormatFunction = string.Format("{0}_UploadAttachment", _grid.ID)
            };
            _grid.Columns[0].Formatter = formatter;

            var formatterNullableInt = new CustomFormatter
            {
                FormatFunction = "DisplayNullableIntegerFormatter"
            };
            _grid.Columns[8].Formatter = formatterNullableInt;
            _grid.Columns[9].Formatter = formatterNullableInt;
            _grid.Columns[11].Formatter = formatterNullableInt;
            _grid.Columns[12].Formatter = formatterNullableInt;
        }
    }

}

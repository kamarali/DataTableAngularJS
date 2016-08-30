using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class PayableSupportingDocSearchResultGrid : GridBase
  {
    public PayableSupportingDocSearchResultGrid(string gridId, string dataUrl)
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
                            GridColumnHelper.ActionColumn("RecordId", 50),
                            GridColumnHelper.HiddenColumn("InvoiceId"),
                            GridColumnHelper.HiddenColumn("RecordType"),
                            GridColumnHelper.TextColumn("SupportingDocTypeText", "Type", 90,true),
                            GridColumnHelper.TextColumn("BillingPeriod", "Billing Period", 100,true),
                            GridColumnHelper.TextColumn("BillingMemberText", "Billing Member", 100,true),
                            GridColumnHelper.TextColumn("InvoiceNumber", "Invoice No.", 90,true),
                            GridColumnHelper.NumberColumn("SourceCodeId", "Source Code", isSortable:true),                           
                            GridColumnHelper.NumberColumn("BatchSequenceNumber", "Batch No", 90, isSortable:true),
                            GridColumnHelper.NumberColumn("RecordSequenceWithinBatch", "Sequence No", 90, isSortable:true),
                            GridColumnHelper.NumberColumn("RMBMCMNumber", "RM/BM/CM No", 90, isSortable:true),
                            GridColumnHelper.CustomColumn("TicketDocNumber", "Tkt./Doc. No", 90, TextAlign.Right, isSortable:true),
                            GridColumnHelper.NumberColumn("CouponNumber", "Coupon No", 90, isSortable:true),
                            GridColumnHelper.TextColumn("AttachmentIndicatorValidatedText", "Attachment Indicator Validated", 120, isSortable:true),
                            GridColumnHelper.NumberColumn("AttachmentNumber", "No. of Attachments", 100, isSortable:true),

                          };


        //CustomFormatter formatter = new CustomFormatter();
        //formatter.FormatFunction = string.Format("{0}_UploadAttachment", _grid.ID);
        //_grid.Columns[0].Formatter = formatter;

        //CustomFormatter formatterNullableInt = new CustomFormatter();
        //formatterNullableInt.FormatFunction = "DisplayNullableIntegerFormatter";
        //_grid.Columns[8].Formatter = formatterNullableInt;
        //_grid.Columns[9].Formatter = formatterNullableInt;
        //_grid.Columns[11].Formatter = formatterNullableInt;
        //_grid.Columns[12].Formatter = formatterNullableInt;
      }
    }

  }
}
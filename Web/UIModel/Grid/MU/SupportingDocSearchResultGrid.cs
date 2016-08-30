using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.MU
{
  public class SupportingDocSearchResultGrid : GridBase
  {
      public SupportingDocSearchResultGrid(string gridId, string dataUrl, bool isDisplayChargeCategory)
      : base(gridId, dataUrl,false,false,isDisplayChargeCategory)
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
                          GridColumnHelper.ActionColumn("InvoiceId", 50),
                          GridColumnHelper.TextColumn("DisplayBillingMonthYear", "Billing Period", 100, true),
                          GridColumnHelper.TextColumn("BilledMemberText", "Billed Member", MemberNumericCodeColoumnWidth,true), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21
                          GridColumnHelper.TextColumn("InvoiceNumber", "Invoice No.", 90, true),
                           _isDisplayChargeCategory ? GridColumnHelper.TextColumn("ChargeCategory", "Charge Category", 150,true):GridColumnHelper.HiddenColumn("ChargeCategory"),
                         // GridColumnHelper.TextColumn("ChargeCategory", "Charge Category", 150,true),
                          GridColumnHelper.TextColumn("AttachmentIndicatorOriginalText", "Attachment Indicator Original", 120, true),
                          GridColumnHelper.NumberColumn("AttachmentNumber", "No. of Attachments", 100, isSortable:true),
                        };


      var formatter = new CustomFormatter
                        {
                          FormatFunction = string.Format("{0}_UploadMiscAttachment", _grid.ID)
                        };
      _grid.Columns[0].Formatter = formatter;
    }
  }
}